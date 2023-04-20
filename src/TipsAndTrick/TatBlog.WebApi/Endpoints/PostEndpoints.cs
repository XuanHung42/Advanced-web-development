using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using NLog.Targets.Wrappers;
using SlugGenerator;
using System.Collections.Generic;
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
            routeGroupBuilder.MapGet("/get-posts-filter", GetFilteredPosts)
               .WithName("GetFilteredPost")
               .Produces<ApiResponse<PaginationResult<PostDto>>>();
            routeGroupBuilder.MapGet("/get-filter", GetFilter)
                .WithName("GetFilter")
                .Produces<ApiResponse<PostFilterModel>>();
            routeGroupBuilder.MapGet("featured/{limit:int}", GetFeaturePosts)
              .WithName("GetPostsFeatured")
              .Produces<ApiResponse<IList<PostDto>>>();

            routeGroupBuilder.MapGet("/random/{limit:int}", GetRadomPosts)
              .WithName("GetPostsRandom")
              .Produces<ApiResponse<IList<PostDto>>>();
            routeGroupBuilder.MapGet("/{id:int}", GetPostDetail)
                .WithName("GetDetailPost")
                .Produces<ApiResponse<PostDetail>>();
            routeGroupBuilder.MapGet("/slug/{slug:regex(^[a-z0-9_-]+$)}", GetPostBySlug)
              .WithName("GetPostsDetailBySlug")
              .Produces<ApiResponse<PostDetail>>();
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
            routeGroupBuilder.MapPost("/", AddPost)
                .WithName("AddNewPost")
                .Accepts<PostEditModel>("multipart/form-data")
                .Produces(401)
                .Produces<ApiResponse<PostItem>>();

            return app;
        }
        private static async Task<IResult> GetFilter(IAuthorRepository authorRepository, IBlogResponsitory blogRepository)
        {
            var model = new PostFilterModel()
            {
                AuthorList = (await authorRepository.GetAuthorsAsync())
                .Select(a => new SelectListItem()
                {
                    Text = a.FullName,
                    Value = a.Id.ToString()
                }),
                CategoryList = (await blogRepository.GetCategoriesAsync())
                .Select(c => new SelectListItem()
                {
                    Text = c.Name,

                    Value = c.Id.ToString()
                })
            };
            return Results.Ok(ApiResponse.Success(model));
        }

        private static async Task<IResult> GetFilteredPosts(
            [AsParameters] PostFilterModel model,
            IBlogResponsitory blogRepository)
        {
            var postQuery = new PostQuery()
            {
                Keyword = model.Keyword,
                CategoryId = model.CategoryId,
                AuthorId = model.AuthorId,
                Year = model.Year,
                Month = model.Month,
            }; var postsList = await blogRepository.GetPagedPostsAsync(postQuery, model,
                posts => posts.ProjectToType<PostDto>());
            var paginationResult = new PaginationResult<PostDto>(postsList);
            return Results.Ok(ApiResponse.Success(paginationResult));
        }
        private static async Task<IResult> GetPosts(
            [AsParameters] PostFilterModel model,
            IBlogResponsitory blogResponsitory,
             IMapper mapper)
        {
            var postQuery = mapper.Map<PostQuery>(model);
            var postList = await blogResponsitory.GetPagedPostsAsync(postQuery, model, post => post.ProjectToType<PostDto>());
            var paginationResult = new PaginationResult<PostDto>(postList);
            return Results.Ok(ApiResponse.Success(paginationResult));

        }
        private static async Task<IResult> GetRadomPosts(int limit, IBlogResponsitory blogResponsitory, IMapper mapper)
        {
            //var postList = await blogResponsitory.RandomPosts(n);
            //var rdPost = mapper.Map<IList<PostDto>>(postList);
            var rdPost = await blogResponsitory.RandomPosts(limit, p => p.ProjectToType<PostDto>());
            return Results.Ok(ApiResponse.Success(rdPost));


        }
        private static async Task<IResult> GetFeaturePosts(int limit, IBlogResponsitory blogResponsitory, IMapper mapper)
        {
            var ftPost = await blogResponsitory.GetFeaturePostsAsync(limit, p => p.ProjectToType<PostDto>());
            //var ftPost = mapper.Map<IList<PostDto>>(postList);
            return Results.Ok(ApiResponse.Success(ftPost));


        }
        private static async Task<IResult> GetPostDetail(int id, IBlogResponsitory blog, IMapper mapper)
        {
            var post = await blog.GetPostbyIdAsync(id);
            return post == null
                ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết có mã số {id}"))
                : Results.Ok(ApiResponse.Success(mapper.Map<PostDetail>(post)));
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
        //private static async Task<IResult> AddPost(PostEditModel model,
        //    IValidator<PostEditModel> validator, IBlogResponsitory blogResponsitory,
        //    IAuthorRepository authorRepository,
        //    IMapper mapper)
        //{
        //    if (await blogResponsitory.IsPostSlugExistedAsync(0, model.UrlSlug))
        //    {
        //        return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug {model.UrlSlug} đã tồn tại"));
        //    };
        //    if (await blogResponsitory.GetCategoryByIdAsync(model.CategoryId) == null)
        //    {
        //        return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Không tìm thấy bài viết với chủ đề có id{model.CategoryId}"));

        //    }
        //    if (await authorRepository.GetAuthorByIdAsync(model.AuthorId) == null)
        //    {
        //        return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Không tìm thấy bài viết với tác giả có id{model.AuthorId}"));
        //    }
        //    var post = mapper.Map<Post>(model);
        //    post.PostedDate = DateTime.UtcNow;
        //    await blogResponsitory.CreateOrUpdatePostAsync(post, model.GetSelectedTags());
        //    return Results.Ok(ApiResponse.Success(mapper.Map<PostDto>(post), HttpStatusCode.Created));
        //}

        private static async Task<IResult> AddPost(
            HttpContext context,
            IBlogResponsitory blogResponsitory,
            IMapper mapper, IMediaManager media)
        {
            var model = await PostEditModel.BindAsync(context);
            var slug = model.Title.GenerateSlug();
            if (await blogResponsitory.IsPostSlugExistedAsync(model.Id, slug))
            {
                return Results.Ok(ApiResponse.Fail(
                    HttpStatusCode.Conflict, $"Slug {slug} đã được sử dụng cho bài viết khác"));
            }
            var post = model.Id > 0 ? await blogResponsitory.GetPostbyIdAsync(model.Id) : null;
            if (post == null)
            {
                post = new Post()
                {
                    PostedDate = DateTime.Now
                };

            }

            post.Title = model.Title;
            post.AuthorId = model.AuthorId;
            post.CategoryId = model.CategoryId;
            post.ShortDescription = model.ShortDescription;
            post.Description = model.Description;
            post.Meta = model.Meta;
            post.Published = model.Published;
            post.ModifiedDate = DateTime.Now;
            post.UrlSlug = model.Title.GenerateSlug();

            if (model.ImageFile?.Length > 0)
            {
                string hostname = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}/",
                    uploadedPath = await media.SaveFileAsync(
                        model.ImageFile.OpenReadStream(), model.ImageFile.FileName, model.ImageFile.ContentType);
                if (!string.IsNullOrWhiteSpace(uploadedPath))
                {
                    post.ImageUrl = uploadedPath;
                }
            }
            await blogResponsitory.CreateOrUpdatePostAsync(post, model.GetSelectedTags());
            return Results.Ok(ApiResponse.Success(mapper.Map<PostItem>(post), HttpStatusCode.Created));

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
