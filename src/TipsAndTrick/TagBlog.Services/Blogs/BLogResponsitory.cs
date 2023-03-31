﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;
using SlugGenerator;
using static TatBlog.Core.Contracts.IPagedList;
using Microsoft.Extensions.Caching.Memory;

namespace TatBlog.Services.Blogs
{
    public class BLogResponsitory : IBlogResponsitory
    {
        private readonly BlogDdContext _context;
        private readonly IMemoryCache _memoryCache;

        public BLogResponsitory(BlogDdContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }



        //tim bai viet có ten dinh danh la slug
        public async Task<Post> GetPostAsync(
            int year,
            int mouth,
            string slug,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Post> postsQuery = _context.Set<Post>()
                .Include(x => x.Category)
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
                categories = categories.Where(x => x.ShowMenu);
            }

            return await categories
                .OrderBy(x => x.Name)
                .Select(x => new CategoryItem()
                {

                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UrlSlug = x.UrlSlug,

                    ShowMenu = x.ShowMenu,
                    PostCount = x.Posts.Count(p => p.Published)

                })
                .ToListAsync(cancellationToken);
        }

        //Lay danh sach tu khoa va phan trang theo cac tham so pagingParams


       
        //Tim chuyen muc theo slug
        public async Task<Tag> FindTagSlugAsync(string slug, CancellationToken cancellation = default)
        {
            IQueryable<Tag> queryTag = _context.Set<Tag>();
            queryTag.Where(x => x.UrlSlug == slug);
            return await queryTag.FirstOrDefaultAsync(cancellation);
        }
        //Lay het toan bo Tag
        public async Task<IList<TagItem>> GetTagsAsync(CancellationToken cancellationToken = default)
        {
            IQueryable<Tag> tags = _context.Set<Tag>();
            return await tags.Select(x => new TagItem()
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                UrlSlug = x.UrlSlug,
                PostCount = x.Posts.Count(x => x.Published)
            }).ToListAsync(cancellationToken);
        }
        //Xoa Tag voi ten dinh danh Slug
        public async Task<bool> DeleleTagWithSlugAsync(string slug, CancellationToken cancellation = default)
        {
            var tagDelete = await _context.Set<Tag>()
                .Where(t => t.UrlSlug == slug)
                .FirstOrDefaultAsync(cancellation);
            if (tagDelete == null)
            {
                return false;
            }
            else
            {
                _context.Set<Tag>().Remove(tagDelete);
                await _context.SaveChangesAsync(cancellation);
                return true;
            }
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
        public async Task<IList<AuthorItem>> GetAuthorsAsync(CancellationToken cancellationToken = default)
        {
            IQueryable<Author> authors = _context.Set<Author>();
            return await authors.Select(x => new AuthorItem()
            {
                Id = x.Id,
                FullName = x.FullName,
                Email = x.Email,
                UrlSlug = x.UrlSlug,
                ImageUrl = x.ImageUrl,
                JoinDate = x.JoinDate,
                PostCount = x.Posts.Count(x => x.Published)

            }).ToListAsync(cancellationToken);
        }
        //Tim the Tag bang ID
        public async Task<Tag> FindTagWithIdAsync(int id, CancellationToken cancellationToken = default)
        {
            IQueryable<Tag> tagQuery = _context.Set<Tag>().Where(x => x.Id == id);
            return await tagQuery.FirstOrDefaultAsync(cancellationToken);
        }
        //Tim Category bang ID
        public async Task<Category> FindCategoryWithIdAsync(int id, CancellationToken cancellationToken = default)
        {
            IQueryable<Category> categoryQuery = _context.Set<Category>()
                .Where(c => c.Id == id);
            return await categoryQuery.FirstOrDefaultAsync(cancellationToken);
        }

        //Xoa Category theo Slug
        public async Task<bool> DeleteCategoryWithSlugAsync(string slug, CancellationToken cancellationToken)
        {
            var categoryDelete = await _context.Set<Category>()
                .Where(c => c.UrlSlug == slug).FirstOrDefaultAsync(cancellationToken);
            if (categoryDelete == null)
            {
                return false;
            }
            else
            {
                _context.Set<Category>().Remove(categoryDelete);
                return true;
            }
        }
        //Kiem tra Category da ton tai voi dinh danh slug hay chua
        public async Task<bool> IsCategoryExistSlugAsync( int id,string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Category>()
                 .AnyAsync(x => x.Id != id && x.UrlSlug == slug, cancellationToken);
        }
        public async Task<bool> IsTagExistSlugAsync(int id, string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Tag>()
                 .AnyAsync(x => x.Id != id && x.UrlSlug == slug, cancellationToken);
        }



        public async Task ChangedPublishedPostAsync(int id, CancellationToken cancellationToken = default)
        {
            await _context.Set<Post>().Where(x => x.Id == id)
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.Published, x => !x.Published), cancellationToken);

        }
        public async Task<IPagedList<T>> GetPagedPostsAsync<T>(PostQuery query,
        IPagingParams pagingParams,
        Func<IQueryable<Post>,
        IQueryable<T>> mapper,
        CancellationToken cancellationToken = default)
        {
            IQueryable<Post> postFindQuery = FilterPosts(query);
            IQueryable<T> tQueryResult = mapper(postFindQuery);
            return await tQueryResult.ToPagedListAsync(pagingParams, cancellationToken);
        }
        public Task<IPagedList<Post>> FindAndPagedPostAsync(PostQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        private IQueryable<Post> FilterPosts(PostQuery condition)
        {
            IQueryable<Post> posts = _context.Set<Post>()
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.Author);

            if (condition.AuthorId > 0)
            {
                posts = posts.Where(x => x.AuthorId == condition.AuthorId);
            }

            if (condition.PublishedOnly)
            {
                posts = posts.Where(x => x.Published);
            }

            if (condition.NotPublished)
            {
                posts = posts.Where(x => !x.Published);
            }

            if (condition.CategoryId > 0)
            {
                posts = posts.Where(x => x.CategoryId == condition.CategoryId);
            }
            if (condition.Year > 0)
            {
                posts = posts.Where(x => x.PostedDate.Year == condition.Year);
            }
            if (condition.Month > 0)
            {
                posts = posts.Where(x => x.PostedDate.Month == condition.Month);
            }

            if (!string.IsNullOrWhiteSpace(condition.CategorySlug))
            {
                posts = posts.Where(x => x.Category.UrlSlug == condition.CategorySlug);
            }
            if (!string.IsNullOrWhiteSpace(condition.AuthorSlug))
            {
                posts = posts.Where(x => x.Author.UrlSlug == condition.AuthorSlug);
            }

            if (!string.IsNullOrWhiteSpace(condition.TagSlug))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.UrlSlug == condition.TagSlug));
            }

            if (!string.IsNullOrWhiteSpace(condition.Keyword))
            {
                posts = posts.Where(x => x.Title.Contains(condition.Keyword) ||
                                         x.ShortDescription.Contains(condition.Keyword) ||
                                         x.Description.Contains(condition.Keyword) ||
                                         x.Category.Name.Contains(condition.Keyword) ||
                                         x.Tags.Any(t => t.Name.Contains(condition.Keyword)));
            }

            if (condition.Year > 0)
            {
                posts = posts.Where(x => x.PostedDate.Year == condition.Year);
            }

            if (condition.Month > 0)
            {
                posts = posts.Where(x => x.PostedDate.Month == condition.Month);
            }

            if (!string.IsNullOrWhiteSpace(condition.TitleSlug))
            {
                posts = posts.Where(x => x.UrlSlug == condition.TitleSlug);
            }

            return posts;
        }

        public async Task<IPagedList<Post>> GetPagedPostsAsync(
        PostQuery condition,
        int pageNumber = 1,
        int pageSize = 5,
        CancellationToken cancellationToken = default)
        {
            return await FilterPosts(condition).ToPagedListAsync(
                pageNumber, pageSize,
                nameof(Post.PostedDate), "DESC",
                cancellationToken);
        }
        public async Task<IPagedList<Category>> GetPageCategoriesAsync(
       CategoryQuery condition,
       int pageNumber = 1,
       int pageSize = 5,
       CancellationToken cancellationToken = default)
        {
            return await FilterCategory(condition).ToPagedListAsync(
                pageNumber, pageSize,
                nameof(Category.Id), "DESC",
                cancellationToken);
        }
        public async Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(
        IPagingParams pagingParams,
        string name = null,
        CancellationToken cancellationToken = default)
        {

            return await _context.Set<Category>()
                .AsNoTracking()
                .WhereIf(!string.IsNullOrWhiteSpace(name),
                    x => x.Name.Contains(name))
                .Select(a => new CategoryItem()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    ShowMenu = a.ShowMenu,
                    UrlSlug = a.UrlSlug,
                    PostCount = a.Posts.Count(p => p.Published)
                })
                .ToPagedListAsync(pagingParams, cancellationToken);


        }
        public async Task<IPagedList<TagItem>> GetPagedTagAsync(
       IPagingParams pagingParams,
       string name = null,
       CancellationToken cancellationToken = default)
        {

            return await _context.Set<Tag>()
                .AsNoTracking()
                .WhereIf(!string.IsNullOrWhiteSpace(name),
                    x => x.Name.Contains(name))
                .Select(a => new TagItem()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    UrlSlug = a.UrlSlug,
                    PostCount = a.Posts.Count(p => p.Published)
                })
                .ToPagedListAsync(pagingParams, cancellationToken);


        }

        public async Task<IPagedList<Tag>> GetPagedTagAsync(
      TagQuery condition,
      int pageNumber = 1,
      int pageSize = 5,
      CancellationToken cancellationToken = default)
        {
            return await FilterTag(condition).ToPagedListAsync(
                pageNumber, pageSize,
                nameof(Author.Id), "DESC",
                cancellationToken);
        }

       
        private IQueryable<Tag> FilterTag(TagQuery condition)
        {
            IQueryable<Tag> tagQuery = _context.Set<Tag>();

            if (!string.IsNullOrWhiteSpace(condition.Keyword))
            {
                tagQuery = tagQuery
                  .Where(a =>
                       a.Name.Contains(condition.Keyword)
                    || a.Description.Contains(condition.Keyword));

            }



            return tagQuery;
        }

        private IQueryable<Category> FilterCategory(CategoryQuery condition)
        {
            IQueryable<Category> category = _context.Set<Category>();
            if (!string.IsNullOrWhiteSpace(condition.Keyword))
            {
                category = category.Where(c=>
                                            c.Description.Contains(condition.Keyword) ||
                                            c.Name.Contains(condition.Keyword));
                    

            }
            return category;
        }


        public async Task<Category> FindCategoriesBySlugAsync(string slug, CancellationToken cancellation = default)
        {
            IQueryable<Category> query = _context.Set<Category>();

            if (!string.IsNullOrEmpty(slug))
            {
                query = query.Where(c => c.UrlSlug == slug);
            }

            return await query.FirstOrDefaultAsync(cancellation);
        }
        public async Task<Category> FindCategoriesByIdAsync(int id, bool isDetail = false ,CancellationToken cancellation = default)
        {
            if (!isDetail)
            {
                return await _context.Set<Category>().FindAsync(id, cancellation);
            }


            return await _context.Set<Category>()
       
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(cancellation);
        }
        public async Task<Tag> FindTagByIdAsync(int id, bool isDetail = false, CancellationToken cancellation = default)
        {
            if (!isDetail)
            {
                return await _context.Set<Tag>().FindAsync(id, cancellation);
            }


            return await _context.Set<Tag>()

                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(cancellation);
        }
        public async Task<IList<Post>> RandomPosts(
                int r,
                CancellationToken cancellationToken = default
                        )
        {
            return await _context.Set<Post>()
              .OrderBy(p => Guid.NewGuid())
              .Take(r)
              .ToListAsync(cancellationToken);
        }
        public async Task<IList<T>> RandomPosts<T>(int n, Func<IQueryable<Post>,IQueryable<T>> mapper,CancellationToken cancellationToken = default)
        {
            IQueryable<Post> radomPost = _context.Set<Post>()
                .Include(p=>p.Category)
                .Include(p=>p.Tags)
                .Include(p=> p.Author).
                OrderBy(p => Guid.NewGuid())
                .Take(n);
            return await mapper(radomPost).ToListAsync(cancellationToken);
        }
        public async Task<Post> GetPostBySlugAsync(
       string slug,
       bool includeDetails = false,
       CancellationToken cancellationToken = default)
        {
            if (!includeDetails)
            {
                return await _context.Set<Post>()
                  .Where(p => p.UrlSlug == slug)
                  .FirstOrDefaultAsync(cancellationToken);
            }
            return await _context.Set<Post>()
              .Include(p => p.Author)
              .Include(p => p.Tags)
              .Include(p => p.Category)
              .Where(p => p.UrlSlug == slug)
              .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<IList<T>> GetFeaturePostsAsync<T>(int n, Func<IQueryable<Post>, IQueryable<T>> mapper, CancellationToken cancellationToken = default)
        {

            IQueryable<Post> featurePost = _context.Set<Post>()
                .Include(p => p.Category)
                .Include(p => p.Tags)
                .Include(p => p.Author)
                .OrderByDescending(p=> p.ViewCount)
                .Take(n);
            return await mapper(featurePost).ToListAsync(cancellationToken);
        }
        public async Task<Post> GetPostbyIdAsync(
            int id,
            bool isDetail = false,
            CancellationToken cancellationToken = default)
        {

            if (!isDetail)
            {
                return await _context.Set<Post>().FindAsync(id, cancellationToken);
            }


            return await _context.Set<Post>()
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.Author)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<Post> GetPostbyIdAsync(
           int id,
         
           CancellationToken cancellationToken = default)
        {

           
           
            


            return await _context.Set<Post>()
                .Include(x => x.Category)
                .Include(x => x.Tags)
                .Include(x => x.Author)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<Category> CreateOrUpdateCategoryAsync(
           Category category, CancellationToken cancellationToken = default)
        {
            if (_context.Set<Category>().Any(s => s.Id == category.Id))
            {
                _context.Entry(category).State = EntityState.Modified;
            }
            else
            {
                _context.Categories.Add(category);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return category;
        }
        public async Task<bool> AddOrUpdateCategoryAsync(
      Category category, CancellationToken cancellationToken = default)
        {
            if (category.Id > 0)
            {
                _context.Categories.Update(category);
                _memoryCache.Remove($"category.by-id.{category.Id}");
            }
            else
            {
                _context.Categories.Add(category);
            }

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
        public async Task<Tag> CreateOrUpdateTagAsync(
          Tag tag, CancellationToken cancellationToken = default)
        {
            if (_context.Set<Tag>().Any(s => s.Id == tag.Id))
            {
                _context.Entry(tag).State = EntityState.Modified;
            }
            else
            {
                _context.Tags.Add(tag);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return tag;
        }
        public async Task<bool> CreateOrUpdateTagBoolAsync(
         Tag tag, CancellationToken cancellationToken = default)
        {
            if (_context.Set<Tag>().Any(s => s.Id == tag.Id))
            {
                _context.Entry(tag).State = EntityState.Modified;
            }
            else
            {
                _context.Tags.Add(tag);
            }

            return await _context.SaveChangesAsync(cancellationToken)>0;
            
        }
        public async Task<Post> CreateOrUpdatePostAsync(
        Post post, IEnumerable<string> tags,
        CancellationToken cancellationToken = default)
        {
            if (post.Id > 0)
            {
                await _context.Entry(post).Collection(x => x.Tags).LoadAsync(cancellationToken);
            }
            else
            {
                post.Tags = new List<Tag>();
            }

            foreach (var tagName in tags)
            {
                if (string.IsNullOrWhiteSpace(tagName)) continue;
                if (post.Tags.Any(x => x.Name == tagName)) continue;

                var tag = await _context.Set<Tag>()
                    .FirstOrDefaultAsync(x => x.Name == tagName, cancellationToken);

                if (tag == null)
                {
                    tag = new Tag()
                    {
                        Name = tagName,
                        Description = tagName,
                        UrlSlug = tagName.GenerateSlug()
                    };

                }

                post.Tags.Add(tag);
            }

            post.Tags = post.Tags.Where(t => tags.Contains(t.Name)).ToList();

            if (post.Id > 0)
                _context.Update(post);
            else
                _context.Add(post);

            await _context.SaveChangesAsync(cancellationToken);

            return post;
        }
        public async Task<bool> CreateOrUpdatePostBoolAsync(
       Post post, IEnumerable<string> tags,
       CancellationToken cancellationToken = default)
        {
            if (post.Id > 0)
            {
                await _context.Entry(post).Collection(x => x.Tags).LoadAsync(cancellationToken);
            }
            else
            {
                post.Tags = new List<Tag>();
            }

            foreach (var tagName in tags)
            {
                if (string.IsNullOrWhiteSpace(tagName)) continue;
                if (post.Tags.Any(x => x.Name == tagName)) continue;

                var tag = await _context.Set<Tag>()
                    .FirstOrDefaultAsync(x => x.Name == tagName, cancellationToken);

                if (tag == null)
                {
                    tag = new Tag()
                    {
                        Name = tagName,
                        Description = tagName,
                        UrlSlug = tagName.GenerateSlug()
                    };

                }

                post.Tags.Add(tag);
            }

            post.Tags = post.Tags.Where(t => tags.Contains(t.Name)).ToList();

            if (post.Id > 0)
                _context.Update(post);
            else
                _context.Add(post);

           return await _context.SaveChangesAsync(cancellationToken)>0;

           
        }
        public async Task<Post> FindPostByIdAsync(
    int id,
     CancellationToken cancellationToken = default
)
        {
            return await _context.Set<Post>()
              .Include(p => p.Tags)
              .Include(p => p.Author)
              .Include(p => p.Category)
              .Where(p => p.Id == id)
              .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<bool> DeletePostById(int id, CancellationToken cancellationToken = default)
        {
            var delPostId = await _context.Set<Post>()
                .Include(p => p.Tags).Where(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
            _context.Set<Post>().Remove(delPostId);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<bool> DeleteCategoryById(int id, CancellationToken cancellationToken = default)
        {
            var delCateId = await _context.Set<Category>()
                .Where(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
            _context.Set<Category>().Remove(delCateId);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<bool> DeleteAuthorById(int id, CancellationToken cancellationToken = default)
        {
            var delAuthorId = await _context.Set<Author>()
                .Where(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
            _context.Set<Author>().Remove(delAuthorId);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<bool> DeleteTagById(int id, CancellationToken cancellationToken = default)
        {
            var delTagId = await _context.Set<Tag>()
                .Where(p => p.Id == id).FirstOrDefaultAsync(cancellationToken);
            _context.Set<Tag>().Remove(delTagId);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<Category> GetCategoryBySlugAsync(
        string slug, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Category>()
                .FirstOrDefaultAsync(a => a.UrlSlug == slug, cancellationToken);
        }
        public async Task<Category> GetCachedCategoryBySlugAsync(
        string slug, CancellationToken cancellationToken = default)
        {
            return await _memoryCache.GetOrCreateAsync(
                $"category.by-slug.{slug}",
                async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    return await GetCategoryBySlugAsync(slug, cancellationToken);
                });
        }
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Set<Category>().FindAsync(id);
        }
        public async Task<Category> GetCachedCategoryByIdAsync(int id)
        {
            return await _memoryCache.GetOrCreateAsync(
                $"author.by-id.{id}",
                async (entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    return await GetCategoryByIdAsync(id);
                });
        }
        public async Task<bool> SetImageUrlAsync(
        int postId, string imageUrl,
        CancellationToken cancellationToken = default)
        {
            return await _context.Posts
                .Where(x => x.Id == postId)
                .ExecuteUpdateAsync(x =>
                    x.SetProperty(a => a.ImageUrl, a => imageUrl),
                    cancellationToken) > 0;
        }

    }
}

