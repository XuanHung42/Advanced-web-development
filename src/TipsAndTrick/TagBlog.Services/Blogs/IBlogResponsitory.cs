using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Extensions;
using static TatBlog.Core.Contracts.IPagedList;

namespace TatBlog.Services.Blogs
{
    public interface IBlogResponsitory

    {
        Task<IPagedList<T>> GetPagedPostsAsync<T>(PostQuery query,
         IPagingParams pagingParams,
         Func<IQueryable<Post>,
         IQueryable<T>> mapper,
         CancellationToken cancellationToken = default);
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
       
        Task<Category> FindCategoriesBySlugAsync(string slug, CancellationToken cancellation = default);
        Task<Tag> FindTagSlugAsync(string slug, CancellationToken cancellation = default);
        Task<IList<TagItem>> GetTagsAsync(CancellationToken cancellationToken = default);
        Task<IList<CategoryItem>> GetCategorysAsync(CancellationToken cancellationToken = default);

        Task<bool> DeleleTagWithSlugAsync(string slug, CancellationToken cancellation= default);
        Task<Tag> FindTagWithIdAsync(int id, CancellationToken cancellationToken = default);
		Task<Category> FindCategoryWithIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteCategoryWithSlugAsync(string slug, CancellationToken cancellationToken);
        Task<bool> IsCategoryExistSlugAsync(int id,string slug, CancellationToken cancellationToken = default);
        Task ChangedPublishedPostAsync(int id,  CancellationToken cancellationToken = default);
        Task<IPagedList<Post>> FindAndPagedPostAsync(PostQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default);
        Task<IPagedList<Post>> GetPagedPostsAsync(PostQuery query, int pageNumber,int pageSize ,CancellationToken cancellationToken = default);

        Task<IList<Post>> RandomPosts(int r,CancellationToken cancellationToken = default);
 

        Task<Post> GetPostbyIdAsync(int id,bool isPublished ,CancellationToken cancellationToken = default);
        Task<Post> CreateOrUpdatePostAsync(
        Post post, IEnumerable<string> tags,
        CancellationToken cancellationToken = default);
        Task<Post> FindPostByIdAsync(
    int id,
     CancellationToken cancellationToken = default
);
        Task<IPagedList<Category>> GetPageCategoriesAsync(
       CategoryQuery condition,
       int pageNumber = 1,
       int pageSize = 5,
       CancellationToken cancellationToken = default);
        Task<bool> DeletePostById(int id, CancellationToken cancellationToken = default);
       
        Task<Category> FindCategoriesByIdAsync(int id, bool isDetail = false, CancellationToken cancellation = default);
        Task<Category> CreateOrUpdateCategoryAsync(
             Category category, CancellationToken cancellationToken = default);
        Task<bool> DeleteCategoryById(int id, CancellationToken cancellationToken = default);
        
        Task<bool> DeleteTagById(int id, CancellationToken cancellationToken = default);
        Task<Tag> FindTagByIdAsync(int id, bool isDetail = false, CancellationToken cancellation = default);
        Task<IPagedList<Tag>> GetPagedTagAsync(
      TagQuery condition,
      int pageNumber = 1,
      int pageSize = 5,
      CancellationToken cancellationToken = default);
        Task<bool> IsTagExistSlugAsync(int id, string slug, CancellationToken cancellationToken = default);
        Task<Tag> CreateOrUpdateTagAsync(
          Tag tag, CancellationToken cancellationToken = default);
    }
   

}
