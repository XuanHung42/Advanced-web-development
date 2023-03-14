using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
	public class PostsController:Controller
	{
		private readonly IBlogResponsitory _blogResponsitory;

		public PostsController(IBlogResponsitory blogResponsitory)
		{
			_blogResponsitory = blogResponsitory;
		}

		private async Task PopulatePostFulterModelAsync(PostFilterModel model)
		{
			var authors = await _blogResponsitory.GetAuthorsAsync();
			var categories = await _blogResponsitory.GetCategoriesAsync();

			model.AuthorList = authors.Select(a => new SelectListItem()
			{
				Text = a.FullName,
				Value = a.Id.ToString()
			});
			model.CategoryList = categories.Select(c => new SelectListItem()
			{
				Text = c.Name,
				Value = c.Id.ToString()
			});
		}
		public async Task<IActionResult> Index(PostFilterModel model)
		{
			var postQuery = new PostQuery()
			{
				Keyword = model.Keyword,
				CategoryId = model.CategoryId,
				AuthorId = model.AuthorId,
				Year = model.Year,
				Month = model.Month
			};
			ViewBag.PostsList = await _blogResponsitory.GetPagedPostsAsync(postQuery, 1, 10);
			await PopulatePostFulterModelAsync(model);
			return View(model);
		}
	}
}
