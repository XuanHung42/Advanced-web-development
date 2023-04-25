using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components
{
	public class BestAuthors:ViewComponent
	{
		IAuthorRepository _authorResponsitory;
		IPagingParams pagingParams = new PagingParams()
		{
			PageNumber= 1,
			PageSize= 3,
		};

		public BestAuthors(IAuthorRepository authorResponsitory)
		{
			_authorResponsitory = authorResponsitory;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var authors = await _authorResponsitory.GetAuthorTopPostAsync(5,pagingParams);
			return View(authors);
		}
	}
}
