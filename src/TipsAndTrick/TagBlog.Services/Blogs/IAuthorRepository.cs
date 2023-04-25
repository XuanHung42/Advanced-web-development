using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface IAuthorRepository
{
    Task<List<AuthorItem>> GetBestAuthors(int n, CancellationToken cancellationToken = default);

    Task<IPagedList<Author>> GetAuthorTopPostAsync(int n, IPagingParams pagingParams, CancellationToken cancellationToken = default);

    Task<Author> GetAuthorByIdIsDetailAsync(int authorId, bool isDetail = false, CancellationToken cancellationToken = default);

    Task<Author> GetAuthorBySlugAsync(
		string slug,
		CancellationToken cancellationToken = default);

	Task<Author> GetCachedAuthorBySlugAsync(
		string slug, CancellationToken cancellationToken = default);

	Task<Author> GetAuthorByIdAsync(int authorId);

	Task<Author> GetCachedAuthorByIdAsync(int authorId);

	Task<IList<AuthorItem>> GetAuthorsAsync(
		CancellationToken cancellationToken = default);

	Task<IPagedList<AuthorItem>> GetPagedAuthorsAsync(
		IPagingParams pagingParams,
		string name = null,
		CancellationToken cancellationToken = default);

	Task<IPagedList<T>> GetPagedAuthorsAsync<T>(
		Func<IQueryable<Author>, IQueryable<T>> mapper,
		IPagingParams pagingParams,
		string name = null,
		CancellationToken cancellationToken = default);

	Task<bool> AddOrUpdateAsync(
		Author author, 
		CancellationToken cancellationToken = default);
	
	Task<bool> DeleteAuthorAsync(
		int authorId, 
		CancellationToken cancellationToken = default);

	Task<bool> IsAuthorSlugExistedAsync(
		int authorId, string slug, 
		CancellationToken cancellationToken = default);

	Task<bool> SetImageUrlAsync(
		int authorId, string imageUrl,
		CancellationToken cancellationToken = default);
	Task<IPagedList<Author>> GetPageAuthorAsync(
   AuthorQuery condition,
   int pageNumber = 1,
   int pageSize = 5,
   CancellationToken cancellationToken = default);
	Task<IPagedList<T>> GetAuthorTopPostAsync<T>(
		 int n,
		 IPagingParams pagingParams,
		 Func<IQueryable<Author>, IQueryable<T>> mapper,
		 CancellationToken cancellationToken = default);
}