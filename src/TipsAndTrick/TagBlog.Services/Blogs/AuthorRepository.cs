﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Dynamic.Core;
using System.Threading;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class AuthorRepository : IAuthorRepository
{
	private readonly BlogDdContext _context;
	private readonly IMemoryCache _memoryCache;

	public AuthorRepository(BlogDdContext context, IMemoryCache memoryCache)
	{
		_context = context;
		_memoryCache = memoryCache;
	}

	public async Task<Author> GetAuthorBySlugAsync(
		string slug, CancellationToken cancellationToken = default)
	{
		return await _context.Set<Author>()
			.FirstOrDefaultAsync(a => a.UrlSlug == slug, cancellationToken);
	}

	public async Task<Author> GetCachedAuthorBySlugAsync(
		string slug, CancellationToken cancellationToken = default)
	{
		return await _memoryCache.GetOrCreateAsync(
			$"author.by-slug.{slug}",
			async (entry) =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
				return await GetAuthorBySlugAsync(slug, cancellationToken);
			});
	}

	public async Task<Author> GetAuthorByIdAsync(int authorId)
	{
		return await _context.Set<Author>().FindAsync(authorId);
	}
    public async Task<Author> GetAuthorByIdIsDetailAsync( int authorId ,bool isDetail= false, CancellationToken cancellationToken = default)
    {
		if (!isDetail)
		{
            return await _context.Set<Author>().FindAsync(authorId);
		}
        return await _context.Set<Author>().Where(a => a.Id == authorId).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Author> GetCachedAuthorByIdAsync(int authorId)
	{
		return await _memoryCache.GetOrCreateAsync(
			$"author.by-id.{authorId}",
			async (entry) =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
				return await GetAuthorByIdAsync(authorId);
			});
	}

	public async Task<IList<AuthorItem>> GetAuthorsAsync(
		CancellationToken cancellationToken = default)
	{
		return await _context.Set<Author>()
			.OrderBy(a => a.FullName)
			.Select(a => new AuthorItem()
			{
				Id = a.Id,
				FullName = a.FullName,
				Email = a.Email,
				JoinDate = a.JoinDate,
				ImageUrl = a.ImageUrl,
				UrlSlug = a.UrlSlug,
				PostCount = a.Posts.Count(p => p.Published)
			})
			.ToListAsync(cancellationToken);
	}

    public async Task<IPagedList<AuthorItem>> GetPagedAuthorsAsync(
         IPagingParams pagingParams,
         string name = null,
         CancellationToken cancellationToken = default)
    {

		return await _context.Set<Author>()
			.AsNoTracking()
			.WhereIf(!string.IsNullOrWhiteSpace(name),
				x => x.FullName.Contains(name))
			.Select(a => new AuthorItem()
			{
				Id = a.Id,
				FullName = a.FullName,
				Email = a.Email,
				JoinDate = a.JoinDate,
				ImageUrl = a.ImageUrl,
				UrlSlug = a.UrlSlug,
				PostCount = a.Posts.Count(p => p.Published)
			})
			.ToPagedListAsync(pagingParams, cancellationToken);

            
    }
    public async Task<IPagedList<Author>> GetPageAuthorAsync(
     AuthorQuery condition,
     int pageNumber = 1,
     int pageSize = 5,
     CancellationToken cancellationToken = default)
    {
        return await FilterAuthor(condition).ToPagedListAsync(
            pageNumber, pageSize,
            nameof(Author.Id), "DESC",
            cancellationToken);
    }
    private IQueryable<Author> FilterAuthor(AuthorQuery condition)
    {
        IQueryable<Author> authorQuery = _context.Set<Author>();

        if (!string.IsNullOrWhiteSpace(condition.Keyword))
        {
            authorQuery = authorQuery
              .Where(a =>
                   a.Email.Contains(condition.Keyword)
                || a.FullName.Contains(condition.Keyword));

        }



        return authorQuery;
    }

    public async Task<IPagedList<T>> GetPagedAuthorsAsync<T>(
		Func<IQueryable<Author>, IQueryable<T>> mapper,
		IPagingParams pagingParams,
		string name = null,
		CancellationToken cancellationToken = default)
	{
		var authorQuery = _context.Set<Author>().AsNoTracking();

		if (!string.IsNullOrEmpty(name))
		{
			authorQuery = authorQuery.Where(x => x.FullName.Contains(name));
		}

		return await mapper(authorQuery)
			.ToPagedListAsync(pagingParams, cancellationToken);
	}

	public async Task<bool> AddOrUpdateAsync(
		Author author, CancellationToken cancellationToken = default)
	{
		if (author.Id > 0)
		{
			_context.Authors.Update(author);
			_memoryCache.Remove($"author.by-id.{author.Id}");
		}
		else
		{
			_context.Authors.Add(author);
		}

		return await _context.SaveChangesAsync(cancellationToken) > 0;
	}
	
	public async Task<bool> DeleteAuthorAsync(
		int authorId, CancellationToken cancellationToken = default)
	{
		return await _context.Authors
			.Where(x => x.Id == authorId)
			.ExecuteDeleteAsync(cancellationToken) > 0;
	}

	public async Task<bool> IsAuthorSlugExistedAsync(
		int authorId, 
		string slug, 
		CancellationToken cancellationToken = default)
	{
		return await _context.Authors
			.AnyAsync(x => x.Id != authorId && x.UrlSlug == slug, cancellationToken);
	}

	public async Task<bool> SetImageUrlAsync(
		int authorId, string imageUrl,
		CancellationToken cancellationToken = default)
	{
		return await _context.Authors
			.Where(x => x.Id == authorId)
			.ExecuteUpdateAsync(x => 
				x.SetProperty(a => a.ImageUrl, a => imageUrl), 
				cancellationToken) > 0;
	}
    public async Task<IPagedList<Author>> GetAuthorTopPostAsync(int n, IPagingParams pagingParams, CancellationToken cancellationToken = default)
    {
        Author authorsTop = _context.Set<Author>()
       .Include(a => a.Posts)
       .OrderByDescending(a => a.Posts.Count(p => p.Published)).First();
        int top = authorsTop.Posts.Count(p => p.Published);
        return await _context.Set<Author>()
            .Include(a => a.Posts)
            .Where(a => a.Posts.Count(p => p.Published) == top)
            .Take(n).ToPagedListAsync(pagingParams, cancellationToken);

    }
    public async Task<IPagedList<T>> GetAuthorTopPostAsync<T>(
       int n,
       IPagingParams pagingParams,
       Func<IQueryable<Author>, IQueryable<T>> mapper,
       CancellationToken cancellationToken = default)
    {
        Author authorTop = _context.Set<Author>()
          .Include(a => a.Posts)
          .OrderByDescending(a => a.Posts.Count(p => p.Published))
          .First();

        int top = authorTop.Posts.Count(p => p.Published);

        IQueryable<Author> authors = _context.Set<Author>()
                .Include(a => a.Posts)
                .Where(a => a.Posts.Count > 2
                      && a.Posts.Count <= authorTop.Posts.Count)
                .Take(n);

        return await mapper(authors).ToPagedListAsync(pagingParams, cancellationToken);
    }

}