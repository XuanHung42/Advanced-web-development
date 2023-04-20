using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints
{
    public static class TagEndpoints
    {
        public static WebApplication MapTagEndpoints(
         this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/tags");
            routeGroupBuilder.MapGet("/", GetTagCloud)
                .WithName("GetTags")
                .Produces<ApiResponse<TagItem>>();

            routeGroupBuilder.MapGet("/{id:int}", GetTagDetail)
                .WithName("GetTagById")
                .Produces<ApiResponse<TagItem>>();


            routeGroupBuilder.MapPost("/", AddTag)

              .AddEndpointFilter<ValidatorFilter<TagEditModel>>()
              .WithName("AddNewTag")

              .Produces(401)
              .Produces<ApiResponse<TagItem>>();
            routeGroupBuilder.MapGet("/{slug:regex(^[a-z0-9_-]+$)}", GetPostsByTagsSlug)
                .WithName("GetPostsByTagsSlug")
                .Produces<ApiResponse<PaginationResult<PostDto>>>();


            routeGroupBuilder.MapPut("/{id:int}", UpdateTag)
            .WithName("UpdateTag")
            .AddEndpointFilter<ValidatorFilter<TagEditModel>>()

            .Produces(401)
            .Produces<ApiResponse<string>>();
            routeGroupBuilder.MapDelete("/", DeleteTag)
            .WithName("DeleteTag")
            .Produces(401)
            .Produces<ApiResponse<string>>();



            return app;
        }


        private static async Task<IResult> GetTags(
            [AsParameters] TagFilterModel model,
            IBlogResponsitory blogResponsitory
            )
        {
            var tagList = await blogResponsitory.GetPagedTagAsync(model, model.Name);

            var paginationResult = new PaginationResult<TagItem>(tagList);
            return Results.Ok(ApiResponse.Success(paginationResult));


        }
        private static async Task<IResult> GetTagCloud(IBlogResponsitory blogResponsitory)
        {
            var tagList = await blogResponsitory.GetTagsAsync();
            return Results.Ok(ApiResponse.Success(tagList));
        }

        private static async Task<IResult> GetTagDetail(int id, IBlogResponsitory blogRepository, IMapper mapper)
        {
            var tag = await blogRepository.GetCategoryByIdAsync(id);
            return tag == null
                ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy tác giả có mã số {id}"))
                : Results.Ok(ApiResponse.Success(mapper.Map<TagItem>(tag)));
        }



        private static async Task<IResult> AddTag(TagEditModel model, IValidator<TagEditModel> validator, IBlogResponsitory blogResponsitory, IMapper mapper)
        {

            if (await blogResponsitory.IsTagExistSlugAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug '{model.UrlSlug} đã được sử dụng"));

            }
            var tag = mapper.Map<Tag>(model);
            await blogResponsitory.CreateOrUpdateTagAsync(tag);
            return Results.Ok(ApiResponse.Success(mapper.Map<Tag>(tag), HttpStatusCode.Created));
        }

        private static async Task<IResult> UpdateTag(int id, TagEditModel model, IValidator<TagEditModel> validator, IBlogResponsitory blogResponsitory, IMapper mapper)
        {
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, validationResult));
            }

            if (await blogResponsitory.IsTagExistSlugAsync(id, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug {model.UrlSlug} đã được sử dụng"));
            }

            var tag = mapper.Map<Tag>(model);
            tag.Id = id;
            return await blogResponsitory.CreateOrUpdateTagBoolAsync(tag)
                ? Results.Ok(ApiResponse.Success("tag is updated", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find tag"));
        }
        private static async Task<IResult> DeleteTag(int id, IBlogResponsitory blogResponsitory)
        {
            return await blogResponsitory.DeleteTagById(id)
                ? Results.Ok(ApiResponse.Success("Tag is delete  ", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find tag"));

        }
        private static async Task<IResult> GetPostsByTagsSlug(
           [FromRoute] string slug,
           [AsParameters] PagingModel pagingModel,
           IBlogResponsitory blogRepository)
        {
            var postQuery = new PostQuery()
            {
                TagSlug = slug,
            };

            var tagList = await blogRepository.GetPagedPostsAsync(
                postQuery, pagingModel,
                posts => posts.ProjectToType<PostDto>());

            var paginationResult = new PaginationResult<PostDto>(tagList);

            return Results.Ok(ApiResponse.Success(paginationResult));
        }
    }
}
