using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
	public class FeaturesWidget:ViewComponent
	{
		IBlogResponsitory _blogResponsitory;

		public FeaturesWidget(IBlogResponsitory blogResponsitory)
		{
			_blogResponsitory = blogResponsitory;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var features = await _blogResponsitory.GetPopularArticlesAsync(3);
			return View(features);
		}
	}
}
