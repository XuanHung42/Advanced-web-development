using Microsoft.EntityFrameworkCore;
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

                    ShowOnMenu = x.ShowMenu,
                    PostCount = x.Posts.Count(p => p.Published)

                })
                .ToListAsync(cancellationToken);
        }

        //Lay danh sach tu khoa va phan trang theo cac tham so pagingParams


        async Task<IPagedList<TagItem>> IBlogResponsitory.GetPagedTagsAsync(IPagingParams pagingParams, CancellationToken cancellationToken)
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
                .ToPagedListAsync(pagingParams, cancellationToken);
        }
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
        public async Task<bool> IsCategoryExistSlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var categoryExist = await _context.Set<Category>()
                .Where(c => c.UrlSlug == slug).FirstOrDefaultAsync(cancellationToken);
            if (categoryExist == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        public async Task<bool> ChangedPublishedPostAsync(int id, bool published, CancellationToken cancellationToken = default)
        {
            var post = await _context.Set<Post>().FindAsync(id);
            if (post is null)
            {
                return false;
            }
            post.Published = !post.Published;
            await _context.SaveChangesAsync(cancellationToken);
            return post.Published;

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

        public async Task<Category> FindCategoriesBySlugAsync(string slug, CancellationToken cancellation = default)
        {
            IQueryable<Category> query = _context.Set<Category>();

            if (!string.IsNullOrEmpty(slug))
            {
                query = query.Where(c => c.UrlSlug == slug);
            }

            return await query.FirstOrDefaultAsync(cancellation);
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
        public async Task<Post> GetPostbyIdAsync(int id,bool isPublished = false, CancellationToken cancellationToken = default)
        {
			
			if (!isPublished)
            {
                return await _context.Set<Post>().FindAsync(id);
            }
           
               
            return await _context.Set<Post>()
                .Include(x=> x.Category)
                .Include(x=> x.Tags)
                .Include(x=> x.Author)
                .FirstOrDefaultAsync(cancellationToken);


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


	}
	}

