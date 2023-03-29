﻿using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Extensions;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;
namespace TatBlog.WebApi.Endpoints
{
    public static class AuthorEndpoints
    {
        public static WebApplication MapAuthorEndpoints(
          this WebApplication app)
        {
            var routeGroupBuilder = app.MapGroup("/api/authors");
            routeGroupBuilder.MapGet("/", GetAuthors)
                .WithName("GetAuthors")
                .Produces<PaginationResult<AuthorItem>>();

            routeGroupBuilder.MapGet("/{id:int}", GetAuthorDetail)
                .WithName("GetAuthorById")
                .Produces<AuthorItem>()
                .Produces(404);
            routeGroupBuilder.MapGet("/{slug:regex(^[a-z0-9 - ] + $)}/posts", GetPostByAuthorSlug)
                .WithName("GetPostsByAuthorsSlug")
                 .Produces<PaginationResult<PostDto>>();
            routeGroupBuilder.MapPost("/", AddAuthor)
              .WithName("AddNewAuthor")
              .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
              .Produces(201)
              .Produces(400)
              .Produces(409);
            routeGroupBuilder.MapPost("/{id:int}/avatar", SetAuthorPicture)
            .WithName("SetAuthorPicture")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<string>()
            .Produces(400);

            routeGroupBuilder.MapPut("/{id:int}", UpdateAuthor)
            .WithName("UpdateAnAuthor")
            .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
            .Produces(204)
            .Produces(400)
            .Produces(409);
            routeGroupBuilder.MapDelete("/", DeleteAuthor)
            .WithName("DeleteAuthor")
            .Produces(204)
            .Produces(404);
            return app;
        }


        private static async Task<IResult> GetAuthors(
			[AsParameters] AuthorFilterModel model,
			IAuthorRepository authorRepository
			)
		{
			var authorList = await authorRepository.GetPagedAuthorsAsync(model, model.Name);

			var paginationResult = new PaginationResult<AuthorItem>(authorList);
			return Results.Ok(paginationResult);
		}

        private static async Task<IResult> GetAuthorDetail(int id, IAuthorRepository authorRepository, IMapper mapper)
        {
            var author = await authorRepository.GetCachedAuthorByIdAsync(id);
            return author == null
                ? Results.NotFound($"Không tìm thấy tác giả có mã số {id}")
                :Results.Ok(mapper.Map<AuthorItem>(author));
        }
      private static async Task<IResult> GetPostByAuthorId(int id, [AsParameters] PagingModel pagingModel, IBlogResponsitory blogResponsitory)
        {
            var postQuery = new PostQuery()
            {
                AuthorId = id,
                PublishedOnly = true
            };
            var postList = await blogResponsitory.GetPagedPostsAsync(postQuery, pagingModel, posts=> posts.ProjectToType<PostDto>());
            var paginationResult = new PaginationResult<PostDto>(postList);
            return Results.Ok(paginationResult);

        }

        private static async Task<IResult> GetPostByAuthorSlug([FromRoute] string slug, [AsParameters] PagingModel pagingModel, IBlogResponsitory blogResponsitory)
        {
            var postQuery = new PostQuery()
            {
                AuthorSlug = slug,
                PublishedOnly = true
            };
            var postList = await blogResponsitory.GetPagedPostsAsync(postQuery, pagingModel, post=> post.ProjectToType<PostDto>());
            var paginationResult = new PaginationResult<PostDto>(postList);
            return Results.Ok(paginationResult);
        }

        private static async Task<IResult> AddAuthor(AuthorEditModel model, IValidator<AuthorEditModel> validator, IAuthorRepository authorRepository, IMapper mapper)
        {
          
            if(await authorRepository.IsAuthorSlugExistedAsync(0, model.UrlSlug))
            {
                return Results.Conflict($"Slug '{model.UrlSlug} đã được sử dụng");

            }
            var author = mapper.Map<Author>(model);
            await authorRepository.AddOrUpdateAsync(author);
            return Results.CreatedAtRoute("GetAuthorById", new {author.Id}, mapper.Map<AuthorItem>(author));
        }
        private static async Task<IResult> SetAuthorPicture(
            int id, 
            IFormFile imageFile, 
            IAuthorRepository authorRepository, 
            IMediaManager mediaManager)
        {
            var imageUrl = await mediaManager.SaveFileAsync(imageFile.OpenReadStream(), imageFile.FileName, imageFile.ContentType);
            if(string.IsNullOrWhiteSpace(imageUrl)) {
                return Results.BadRequest("Không lưu được tập tin");
            }
            await authorRepository.SetImageUrlAsync(id, imageUrl);
            return Results.Ok(imageUrl);
        }
        private static async Task<IResult> UpdateAuthor(int id, AuthorEditModel model, IValidator<AuthorEditModel> validator, IAuthorRepository authorRepository, IMapper mapper)
        {
           
            if(await authorRepository.IsAuthorSlugExistedAsync(id, model.UrlSlug))
            {
                return Results.Conflict($"Slug '{model.UrlSlug}' đã được sử dụng ");
            }
            var author = mapper.Map<Author>(model);
            author.Id = id;
            return await authorRepository.AddOrUpdateAsync(author)
                ?Results.NoContent()
                :Results.NotFound();
        }
        private static async Task<IResult> DeleteAuthor(int id, IAuthorRepository authorRepository)
        {
            return await authorRepository.DeleteAuthorAsync(id)
                ?Results.NoContent()
                :Results.NotFound($"Could not find author with id = {id}");

        }
    }
}
