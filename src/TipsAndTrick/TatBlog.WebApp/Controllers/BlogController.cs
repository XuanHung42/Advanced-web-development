using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogResponsitory _blogResponsitory;
        private readonly IAuthorResponsitory _authorResponsitory;
        public BlogController(IBlogResponsitory blogResponsitory, IAuthorResponsitory authorResponsitory)
        {
            _blogResponsitory = blogResponsitory;
            _authorResponsitory = authorResponsitory;
        }
        IPagingParams pagingParams = new PagingParams();


        //
        public async Task<IActionResult> Index(

           [FromQuery(Name = "k")] string keyword = null,

           [FromQuery(Name = "p")] int pageNumber = 1,
           [FromQuery(Name = "ps")] int pageSize = 2)

        {
            var postQuery = new PostQuery()
            {
                PublishedOnly = true,
                Keyword = keyword,

            };
            var postsList = await _blogResponsitory.GetPagedPostsAsync(postQuery, pageNumber, pageSize);
            ViewBag.Title = "Trang chủ";

            ViewBag.PostQuery = postQuery;

            return View(postsList);
        }

        //
        public async Task<IActionResult> Category(
           string slug,
           [FromQuery(Name = "p")] int pageNumber = 1,
           [FromQuery(Name = "ps")] int pageSize = 2)
        {
            var category = await _blogResponsitory.FindCategoriesBySlugAsync(slug);

            var postQuery = new PostQuery()
            {
                PublishedOnly = true,
                CategorySlug = slug,
            };
            var posts = await _blogResponsitory
              .GetPagedPostsAsync(postQuery, pageNumber, pageSize);
            ViewBag.PostQuery = postQuery;
            ViewBag.Title = $"Tìm kiếm với chủ đề '{category.Name}'";


            return View("Index", posts);
        }

        //
        public async Task<IActionResult> Author( 
            string slug,
            [FromQuery(Name ="p")] int pageNumber = 1,
            [FromQuery(Name ="ps")] int pageSize = 5)
        {
            var author = await _authorResponsitory.FindAuthorBySlugAsync(slug);
            var postQuery = new PostQuery()
            {
                AuthorSlug = slug
            };
            var posts = await _blogResponsitory
                .GetPagedPostsAsync(postQuery, pageNumber, pageSize);
			ViewBag.Title = $"Tìm kiếm với tác giả '{author.FullName}'";

			ViewBag.PostQuery = postQuery;
            return View("Index", posts);
        }
        //
        public async Task<IActionResult> Tag(
            string slug, 
            [FromQuery(Name ="p")] int pageNumber=1, 
            [FromQuery(Name ="ps")] int pageSize = 5)
        {
           var tag = await _blogResponsitory.FindTagSlugAsync(slug);
            var postQuery = new PostQuery()
            {
                TagSlug = slug
            };
            var posts = await _blogResponsitory.GetPagedPostsAsync(postQuery, pageNumber, pageSize);
            ViewBag.PostQuery = postQuery;
            ViewBag.Title = $"Tìm kiếm với tag {tag.Name}";
            return View("Index", posts);
        }

        public async Task<IActionResult> Post(
            int year, int month, int day, string slug)
        {
            var posts = await _blogResponsitory.GetPostAsync(year, month, slug);
            if (!posts.Published)
            {
                // code loi
            }
            await _blogResponsitory.IncreaseViewCountAsync(posts.Id);
            return View("PostInfo", posts);

        }
        //
        public async Task<IActionResult> Archives(int year, int mouth, 
            [FromQuery(Name ="p")] int pageNumber =1,
            [FromQuery(Name ="ps")] int pageSize=5)
        {
            var postQuery = new PostQuery() { Year = year,
                Month =mouth            
            };
            var posts = await _blogResponsitory.GetPagedPostsAsync(postQuery, pageNumber, pageSize);
            return View("Index", posts);
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
