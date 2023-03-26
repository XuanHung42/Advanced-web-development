using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
	public class CategoriesWidget:ViewComponent
	{
		private readonly IBlogResponsitory _blogResponsitory;

		public CategoriesWidget(IBlogResponsitory blogResponsitory)
		{
			_blogResponsitory = blogResponsitory;
		}
		
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var categories = await _blogResponsitory.GetCategoriesAsync();
			return View(categories);
		}
	}
}
