using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Controllers
{
    public class BlogController:Controller
    {
        private readonly IBlogResponsitory _blogResponsitory;

		public BlogController(IBlogResponsitory blogResponsitory)
		{
			_blogResponsitory = blogResponsitory;
		}

		public async Task<IActionResult> Index(


           [FromQuery(Name ="p")] int pageNumber =1 ,
           [FromQuery(Name ="ps")] int pageSize = 10)
        
        {
            var postQuery = new PostQuery()
            {
                PublishedOnly = true,
            };
            var postsList = await _blogResponsitory.GetPagedPostsAsync(postQuery, pageNumber, pageSize);

            ViewBag.PostQuery = postQuery;
            return View(postsList);
        }
    }
}
