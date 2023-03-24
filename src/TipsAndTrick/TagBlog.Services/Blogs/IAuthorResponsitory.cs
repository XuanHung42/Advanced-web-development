using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using static TatBlog.Core.Contracts.IPagedList;

namespace TatBlog.Services.Blogs
{
	public interface IAuthorResponsitory
	{

		Task<Author> FindAuthorBySlugAsync(string slug, CancellationToken cancellationToken = default);
		Task<Author> FindAuthorByIdAsync(int id, bool Is = false, CancellationToken cancellationToken = default);
		Task<IPagedList<AuthorItem>> GetPagedAuthorsAsync(
		   IPagingParams pagingParams, CancellationToken cancellationToken = default
		   );
		Task<Author> UpdateAuthorAsync(Author author, CancellationToken cancellationToken = default);
		Task<IPagedList<Author>> GetAuthorTopPostAsync(int n, IPagingParams pagingParams, CancellationToken cancellationToken = default);
		Task<bool> IsAuthorExistBySlugAsync(int id, string slug, CancellationToken cancellationToken = default);
		Task<bool> DeleteAuthorById(int id, CancellationToken cancellationToken = default);
		Task<Author> CreateOrUpdateAuthorAsync(
			Author author, CancellationToken cancellationToken = default);
		Task<Author> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default);



    }
}
