using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
	public class PostsController:Controller
	{
		private readonly IBlogResponsitory _blogResponsitory;
		private readonly IMapper _mapper;

        public PostsController(IBlogResponsitory blogResponsitory, IMapper mapper)
        {
            _blogResponsitory = blogResponsitory;
            _mapper = mapper;
        }

        private async Task PopulatePostFilterModelAsync(PostFilterModel model)
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

		[HttpGet]
		public async Task<IActionResult> Edit(int id=0)
		{
			var post = id > 0
				? await _blogResponsitory.GetPostbyIdAsync(id, true)
				: null;

			var model = post == null
				? new PostEditModel()
				: _mapper.Map<PostEditModel>(post);
			//var model = _mapper.Map<PostEditModel>(post);
			//var model = new PostEditModel()
			//{
			//	Id = post.Id,
			//	AuthorId = post.AuthorId,
			//	CategoryId = post.CategoryId,
			//	ShortDescription = post.ShortDescription,
			//	Description = post.Description,
			//	ImageUrl = post.ImageUrl,
			//	UrlSlug = post.UrlSlug,
			//	SelectedTags = string.Join("\r\n", post.Tags.Select(x => x.Name))
			//};



			await PopulatePostEditModelAsync(model);

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(PostEditModel model)
		{
			if (!ModelState.IsValid)
			{
				await PopulatePostEditModelAsync(model);
				return View(model);
			}
			var post=model.Id > 0
				?await _blogResponsitory.GetPostbyIdAsync(model.Id, true)
				: null;
			if(post ==null)
			{
				post = _mapper.Map<Post>(model);
				post.Id = 0;
				post.PostedDate = DateTime.Now;  
			}
			await _blogResponsitory.CreateOrUpdatePostAsync(post, model.GetSelectedTags());
			return RedirectToAction(nameof(Index));
			
		}
		[HttpPost]
		public async Task<IActionResult> VerifyPostSlug(int id, string urlSlug)
		{
			var slugExisted = await _blogResponsitory
				.IsPostSlugExistedAsync(id, urlSlug);
			return slugExisted
				? Json($"Slug '{urlSlug}' đã được sử dụng")
				: Json(true);
		}


		private async Task PopulatePostEditModelAsync(PostEditModel model)
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
			var postQuery = _mapper.Map<PostQuery>(model);
			ViewBag.PostsList = await _blogResponsitory.GetPagedPostsAsync(postQuery, 1, 10);
			await PopulatePostFilterModelAsync(model);
			return View(model);
		}


	}
}
