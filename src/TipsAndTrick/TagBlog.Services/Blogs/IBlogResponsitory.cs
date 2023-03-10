using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using static TatBlog.Core.Contracts.IPagedList;

namespace TatBlog.Services.Blogs
{
    public interface IBlogResponsitory

    {
    Task<Post> GetPostAsync(int year,int month,string slug,CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        Task<IList<Post>> GetPopularArticlesAsync(int numPosts, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        Task<bool> IsPostSlugExistedAsync(int postId, string slug, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        Task IncreaseViewCountAsync(int postId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        Task<IList<CategoryItem>> GetCategoriesAsync(
            bool showOnMenu = false,
            CancellationToken cancellationToken= default);
        Task<IPagedList<TagItem>> GetPagedTagsAsync(
            IPagingParams pagingParams, CancellationToken cancellationToken = default
            );
        Task<Category> FindCategoriesBySlugAsync(string slug, CancellationToken cancellation = default);
        Task<Tag> GetTagSlusAsync(string slug, CancellationToken cancellation = default);
        Task<IList<TagItem>> GetTagsAsync(CancellationToken cancellationToken = default);
        Task<IList<CategoryItem>> GetCategorysAsync(CancellationToken cancellationToken = default);

        Task<bool> DeleleTagWithSlugAsync(string slug, CancellationToken cancellation= default);
        Task<Tag> FindTagWithIdAsync(int id, CancellationToken cancellationToken = default);
		Task<Category> FindCategoryWithIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteCategoryWithSlugAsync(string slug, CancellationToken cancellationToken);
        Task<bool> IsCategoryExistSlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<bool> ChangedPublishedPostAsync(int id, bool published, CancellationToken cancellationToken = default);
        Task<IPagedList<Post>> FindAndPagedPostAsync(PostQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default);
        Task<IPagedList<Post>> GetPagedPostsAsync(PostQuery query, int pageNumber,int pageSize ,CancellationToken cancellationToken = default);
	}

}
