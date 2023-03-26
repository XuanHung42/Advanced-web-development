using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
	public class TagCloudWidget:ViewComponent
	{
		IBlogResponsitory _blogResponsitory;

		public TagCloudWidget(IBlogResponsitory blogResponsitory)
		{
			_blogResponsitory = blogResponsitory;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var radom = await _blogResponsitory.GetTagsAsync();
			return View(radom);
		}
	}
}
