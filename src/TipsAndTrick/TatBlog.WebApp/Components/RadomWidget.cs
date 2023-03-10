using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
	public class RadomWidget:ViewComponent
	{
		IBlogResponsitory _blogResponsitory;

		public RadomWidget(IBlogResponsitory blogResponsitory)
		{
			_blogResponsitory = blogResponsitory;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var radom = await _blogResponsitory.RandomPosts(5);
			return View(radom);
		}

	}
}
