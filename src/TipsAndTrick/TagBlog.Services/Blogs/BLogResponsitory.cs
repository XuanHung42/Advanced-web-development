using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs
{
    public class BLogResponsitory : IBlogResponsitory
    {
        private readonly BlogDdContext _context;

        public BLogResponsitory(BlogDdContext context)
        {
            _context = context;
        }

        //tim bai viet có ten dinh danh la slug
        public async Task<Post> GetPostAsync(
            int year,
            int mouth,
            string slug,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Post> postsQuery = _context.Set<Post>()
                .Include(x => x.CategoryId)
                .Include(x => x.Author);
            if (year > 0)
            {
                postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
            }
            if (mouth > 0)
            {
                postsQuery = postsQuery.Where(x => x.PostedDate.Month == mouth);
            }
            if (!string.IsNullOrWhiteSpace(slug))
            {
                postsQuery = postsQuery.Where(x => x.UrlSlug == slug);

            }
            return await postsQuery.FirstOrDefaultAsync(cancellationToken);
        }
        //tim top bai viet duoc xem nhieu nhat
        public async Task<IList<Post>> GetPopularArticlesAsync(
            int numPosts, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .Include(x => x.Author)
                .Include(x => x.Category)
                .OrderByDescending(p => p.ViewCount)
                .Take(numPosts)
                .ToListAsync(cancellationToken);

        }
        //kiem tra ten dinh danh cua bai viet da co hay chua
        public async Task<bool> IsPostSlugExistedAsync(
            int postId,
            string slug,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<Post>()
                .AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
        }

        //Tang so luot xem cua mot bai viet

        public async Task IncreaseViewCountAsync(int postId, CancellationToken cancellationToken)
        {

            await _context.Set<Post>()
                    .Where(x => x.Id == postId)
                    .ExecuteUpdateAsync(p =>
                    p.SetProperty(p => p.ViewCount, x => x.ViewCount + 1), cancellationToken);
        }

        //Lay danh sach chuyen muc va so luong bai viet

        public async Task<IList<CategoryItem>> GetCategoriesAsync(
            bool showOnMenu = false,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Category> categories = _context.Set<Category>();
            if (showOnMenu)
            {
                categories = categories.Where(x => x.ShowOnMenu);
            }

            return await categories
                .OrderBy(x => x.Name)
                .Select(x => new CategoryItem()
                {

                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UrlSlug = x.UrlSlug,

                    ShowOnMenu = x.ShowOnMenu,
                    PostCount = x.Posts.Count(p => p.Published)

                })
                .ToListAsync(cancellationToken);
        }

        //Lay danh sach tu khoa va phan trang theo cac tham so pagingParams
   

        async Task<IPagedList.IPagedList<TagItem>> IBlogResponsitory.GetPagedTagsAsync(IPagingParams pagingParams, CancellationToken cancellationToken)
        {
            var tagQuery = _context.Set<Tag>()
                 .Select(x => new TagItem()
                 {
                     Id = x.Id,
                     Name = x.Name,
                     Description = x.Description,
                     UrlSlug = x.UrlSlug,
                     PostCount = x.Posts.Count(p => p.Published)
                 });
            return await tagQuery
                .ToPagedListAsync(pagingParams,cancellationToken);
        }
        //Tim chuyen muc theo slug
        public async Task<Tag> GetTagSlusAsync(string slug, CancellationToken cancellation = default)
        {
            IQueryable<Tag> queryTag = _context.Set<Tag>();
            queryTag.Where(x => x.UrlSlug == slug);
            return await queryTag.FirstOrDefaultAsync(cancellation);
        }
        //Lay het toan bo Tag
        public async Task<IList<TagItem>> GetTagsAsync(CancellationToken cancellationToken = default)
        {
            IQueryable<Tag> tags= _context.Set<Tag>();
            return await tags.Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                UrlSlug = x.UrlSlug,
                PostCount = x.Posts.Count(x=> x.Published)
            }).ToListAsync(cancellationToken);
        }
        //Xoa Tag voi ten dinh danh Slug
        public Task<IList<TagItem>> DeleleTagWithSlug(string slug, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        //Lay toan bo Category
        public async Task<IList<CategoryItem>> GetCategorysAsync(CancellationToken cancellationToken = default)
        {
            IQueryable<Category> tags = _context.Set<Category>();
            return await tags.Select(x => new CategoryItem()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                UrlSlug = x.UrlSlug,
                PostCount = x.Posts.Count(x => x.Published)
            }).ToListAsync(cancellationToken);
        }

        public async Task<Tag> FindTagWithId(int id, CancellationToken cancellationToken = default)
        {
            IQueryable<Tag> tagQuery = _context.Set<Tag>().Where(x=> x.Id== id);
            return await tagQuery.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
