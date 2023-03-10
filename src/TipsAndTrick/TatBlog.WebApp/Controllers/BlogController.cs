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

        public BlogController(IBlogResponsitory blogResponsitory)
        {
            _blogResponsitory = blogResponsitory;
        }
        IPagingParams pagingParams = new PagingParams();


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
        public async Task<IActionResult> Category(
           string slug,
           [FromQuery(Name = "p")] int pageNumber = 1,
           [FromQuery(Name = "ps")] int pageSize = 6)
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
            ViewBag.Title = $"Các bài viết của chủ đề '{category.Name}'";


            return View("Index", posts);
        }

        //     private IPagingParams CreatPagingParams(
        //int pageNumber = 1,
        //int pageSize = 5)
        //     {
        //         return new PagingParams()
        //         {
        //             PageNumber = pageNumber,
        //             PageSize = pageSize
        //         };
        //     }



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
