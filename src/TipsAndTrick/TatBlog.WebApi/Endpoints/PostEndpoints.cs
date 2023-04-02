﻿using FluentValidation;
using Mapster;
using MapsterMapper;
using NLog.Targets.Wrappers;
using System.Net;
using System.Security.Cryptography;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints
{
    public static class PostEndpoints
    {
        public static WebApplication MapPostEndpoints(this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/posts");
            routeGroupBuilder.MapGet("/", GetPosts)
                .WithName("GetPosts")
                .Produces<ApiResponse<PaginationResult<PostDto>>>();

            routeGroupBuilder.MapGet("featured/{limit:int}", GetFeaturePosts)
              .WithName("GetPostsFeatured")
              .Produces<ApiResponse<IList<PostDto>>>();

            routeGroupBuilder.MapGet("/random/{limit:int}", GetRadomPosts)
              .WithName("GetPostsRandom")
              .Produces<ApiResponse<IList<PostDto>>>();
            routeGroupBuilder.MapGet("/{id:int}", GetPostDetail)
                .WithName("GetDetailPost")
                .Produces<ApiResponse<PostItem>>();
            routeGroupBuilder.MapPost("/{id:int}/avatar", SetPostPicture)
          .WithName("SetPostPicture")
          .Accepts<IFormFile>("multipart/form-data")
          .Produces<string>()
          .Produces(400);
            routeGroupBuilder.MapPut("/{id:int}", UpdatePost)
           .WithName("UpdatePost")
           .AddEndpointFilter<ValidatorFilter<PostEditModel>>()

           .Produces(401)
           .Produces<ApiResponse<string>>();
            routeGroupBuilder.MapDelete("/", DeletePost)
            .WithName("DeletePost")
            .Produces(401)
            .Produces<ApiResponse<string>>();
            return app;
        }
        private static async Task<IResult> GetPosts(
            [AsParameters] PostFilterModel model, 
            IBlogResponsitory blogResponsitory,
             IMapper mapper)
        {
            var postQuery = mapper.Map<PostQuery>(model);
            var postList = await blogResponsitory.GetPagedPostsAsync(postQuery,model, post=> post.ProjectToType<PostDto>());
            var paginationResult = new PaginationResult<PostDto>(postList);
            return Results.Ok(ApiResponse.Success(paginationResult));

        }
        private static async Task<IResult> GetRadomPosts(int n, IBlogResponsitory blogResponsitory)
        {
            var rdPost = await blogResponsitory.RandomPosts(n, post => post.ProjectToType<PostDto>());
            return Results.Ok(ApiResponse.Success(rdPost));


        }
        private static async Task<IResult> GetFeaturePosts(int n, IBlogResponsitory blogResponsitory)
        {
            var rdPost = await blogResponsitory.GetFeaturePostsAsync(n, post => post.ProjectToType<PostDto>());
            return Results.Ok(ApiResponse.Success(rdPost));


        }
        private static async Task<IResult> GetPostDetail(int id, IBlogResponsitory blog, IMapper mapper)
        {
            var post = await blog.FindPostByIdAsync(id);
            return post == null
                ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết có mã số {id}"))
                : Results.Ok(ApiResponse.Success(mapper.Map<PostItem>(post)));
        }
        private static async Task<IResult> GetPostBySlug(string slug, IBlogResponsitory blogResponsitory, IMapper mapper)
        {
            var post = await blogResponsitory.GetPostBySlugAsync(slug, true);
            return post == null
        ? Results.Ok(ApiResponse.Fail(
            HttpStatusCode.NotFound,
            $"Không tìm thấy bài viết '{slug}'"))
        : Results.Ok(ApiResponse.Success(
            mapper.Map<PostDetail>(post)));
        }
        private static async Task<IResult> AddPost(PostEditModel model, 
            IValidator<PostEditModel> validator, IBlogResponsitory blogResponsitory,
            IAuthorRepository authorRepository,
            IMapper mapper)
        {
            if(await blogResponsitory.IsPostSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug {model.UrlSlug} đã tồn tại"));
            };
            if(await blogResponsitory.GetCategoryByIdAsync(model.CategoryId) == null)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Không tìm thấy bài viết với chủ đề có id{model.CategoryId}"));

            }
            if(await authorRepository.GetAuthorByIdAsync(model.AuthorId) == null)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Không tìm thấy bài viết với tác giả có id{model.AuthorId}"));
            }
            var post = mapper.Map<Post>(model);
            post.PostedDate= DateTime.UtcNow;
            await blogResponsitory.CreateOrUpdatePostAsync(post, model.GetSelectedTags());
            return Results.Ok(ApiResponse.Success(mapper.Map<PostDto>(post), HttpStatusCode.Created));
        }


        private static async Task<IResult> DeletePost(
     int id,
     IBlogResponsitory blogRepository)
        {
            return await blogRepository.DeletePostById(id)
              ? Results.Ok(ApiResponse.Success(
                  $"Xóa thành công id = {id}"))
              : Results.Ok(ApiResponse.Fail(
                  HttpStatusCode.NotFound,
                  $"Không tìm thấy bài viết id = {id}"));
        }
        private static async Task<IResult> UpdatePost(
            int id, 
            PostEditModel model,
            IMapper mapper, 
            IBlogResponsitory blogResponsitory,
            IAuthorRepository authorRepository)
        {
            var post = await blogResponsitory.GetPostbyIdAsync(id);
            if (post == null)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết"));
            }
            if (await blogResponsitory.IsPostSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug {model.UrlSlug} đã tồn tại"));
            };
            if (await blogResponsitory.GetCategoryByIdAsync(model.CategoryId) == null)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Không tìm thấy bài viết với chủ đề có id{model.CategoryId}"));

            }
            if (await authorRepository.GetAuthorByIdAsync(model.AuthorId) == null)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Không tìm thấy bài viết với tác giả có id{model.AuthorId}"));
            }
             mapper.Map(model, post);
            post.Id = id;
            post.ModifiedDate = DateTime.Now;

            return await blogResponsitory.CreateOrUpdatePostBoolAsync(post, model.GetSelectedTags())
                ? Results.Ok(ApiResponse.Success("Thay đổi bài viết thành công"))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Không thể tìm thấy bài viết"));
        }
        private static async Task<IResult> SetPostPicture(
           int id,
           IFormFile imageFile,
           IBlogResponsitory blogResponsitory,
           IMediaManager mediaManager)
        {
            var imageUrl = await mediaManager.SaveFileAsync(imageFile.OpenReadStream(), imageFile.FileName, imageFile.ContentType);
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, "Không lưu được tập tin"));
            }
            await blogResponsitory.SetImageUrlAsync(id, imageUrl);
            return Results.Ok(ApiResponse.Success(imageUrl));
        }
    }

}