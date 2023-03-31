using MapsterMapper;
using System.Net;
using Microsoft.AspNetCore.Mvc;

using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Models;
using Mapster;
using FluentValidation;
using TatBlog.Core.Entities;
using TatBlog.WebApi.Filters;

namespace TatBlog.WebApi.Endpoints
{
    public static class CategoryEndpoints
    {
        public static WebApplication MapCategoryEndpoints(this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/categories");
            routeGroupBuilder.MapGet("/", GetCategories)
                .WithName("GetCategories")
                .Produces<ApiResponse<PaginationResult<CategoryItem>>>();
            routeGroupBuilder.MapGet("/{id:int}", GetDetailCategory)
                .WithName("GetCategoryDetail")
                .Produces<ApiResponse<CategoryItem>>();
            routeGroupBuilder.MapGet("/{slug:regex(^[a-z0-9 - ] + $)}/posts", GetPostByCateforySlug)
              .WithName("GetPostsByCategoriesSlug")
               .Produces<ApiResponse<PaginationResult<PostDto>>>();
            routeGroupBuilder.MapPost("/", AddCategory)
            .WithName("AddNewCategory")
            .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
            .Produces(401)
            .Produces<ApiResponse<CategoryItem>>();

            routeGroupBuilder.MapPut("/{id:int}", UpdateCategory)
            .WithName("UpdateAnCategory")
            .AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()

            .Produces(401)
            .Produces<ApiResponse<string>>();
            routeGroupBuilder.MapDelete("/", DeleteCategory)
            .WithName("DeleteAnCtegory")
            .Produces(401)
            .Produces<ApiResponse<string>>();
          
            return app;
        }
        private static async Task<IResult> GetCategories(
            [AsParameters] CategoryFilterModel model, IBlogResponsitory blogResponsitory)
        {
            var categoryList = await blogResponsitory.GetPagedCategoriesAsync(model, model.Name);
            var paginationResult = new PaginationResult<CategoryItem>(categoryList);
            return Results.Ok(ApiResponse.Success(paginationResult));
        }
        private static async Task<IResult> GetDetailCategory(int id, IBlogResponsitory blogResponsitory, IMapper mapper)
        {
            var category = await blogResponsitory.GetCachedCategoryByIdAsync(id);
            return category == null
                ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy thể loại có mã số {category.Id}"))
                : Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category)));
        }
        private static async Task<IResult> GetPostByCateforySlug([FromRoute] string slug, [AsParameters] PagingModel pagingModel, IBlogResponsitory blog)
        {
            var categoryQuery = new PostQuery()
            {
                CategorySlug = slug,

            };
            var categoryList = await blog.GetPagedPostsAsync(categoryQuery, pagingModel, cate => cate.ProjectToType<CategoryDto>());
            var paginationResult = new PaginationResult<CategoryDto>(categoryList);
            return Results.Ok(ApiResponse.Success(paginationResult));

        }
        private static async Task<IResult> AddCategory(CategoryEditModel model, IValidator<CategoryEditModel> validator, IBlogResponsitory blogResponsitory, IMapper mapper)
        {

            if (await blogResponsitory.IsCategoryExistSlugAsync(0, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug '{model.UrlSlug} đã được sử dụng"));

            }
            var category = mapper.Map<Category>(model);
            await blogResponsitory.CreateOrUpdateCategoryAsync(category);
            return Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category), HttpStatusCode.Created));

        }
        private static async Task<IResult> UpdateCategory(int id, CategoryEditModel model, IValidator<CategoryEditModel> validator, IBlogResponsitory blogResponsitory, IMapper mapper)
        {
            var validationResult = await validator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, validationResult));
            }

            if (await blogResponsitory.IsCategoryExistSlugAsync(id, model.UrlSlug))
            {
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug {model.UrlSlug} đã được sử dụng"));
            }

            var category = mapper.Map<Category>(model);
            category.Id = id;
            return await blogResponsitory.AddOrUpdateCategoryAsync(category)
                ? Results.Ok(ApiResponse.Success("Category is updated", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find author"));
        }
       
        private static async Task<IResult> DeleteCategory(int id, IBlogResponsitory blogResponsitory)
        {
            return await blogResponsitory.DeleteCategoryById(id)
                ? Results.Ok(ApiResponse.Success("Category is delete  ", HttpStatusCode.NoContent))
                : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not find author"));

        }
    }
}
