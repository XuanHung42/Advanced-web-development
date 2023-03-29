using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;
using TatBlog.WebApp.Validations;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
	public class PostsController:Controller
	{
		private readonly IBlogResponsitory _blogResponsitory;
		private readonly IMapper _mapper;
		private readonly IMediaManager _mediaManager;
		private readonly ILogger<PostsController> _logger;
		private readonly IAuthorRepository _authorRepository;

        public PostsController(IBlogResponsitory blogResponsitory, IMapper mapper, IMediaManager mediaManager, ILogger<PostsController> logger, IAuthorRepository authorRepository)
        {
            _blogResponsitory = blogResponsitory;
            _mapper = mapper;
            _mediaManager = mediaManager;
            _logger = logger;
            _authorRepository = authorRepository;

        }

        private async Task PopulatePostFilterModelAsync(PostFilterModel model)
		{
			var authors = await _authorRepository.GetAuthorsAsync();
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
		public async Task<IActionResult> Edit(int id = 0)
		{
			var post = id > 0
				? await _blogResponsitory.GetPostbyIdAsync(id, true)
				: null;


			var model = post == null
				? new PostEditModel()
				: _mapper.Map<PostEditModel>(post);


			Console.WriteLine(model.ToString());

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
		public async Task<IActionResult> Edit([FromServices] IValidator<PostEditModel> postValidator, PostEditModel model)
		{
            var validationResult = await postValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
            }
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

			if(model.ImageFile?.Length >0) {
				var newImagePath = await _mediaManager.SaveFileAsync(
					model.ImageFile.OpenReadStream(),
					model.ImageFile.FileName,
					model.ImageFile.ContentType);
				if(!string.IsNullOrWhiteSpace(newImagePath))
				{
					await _mediaManager.DeleteFileAsync(post.ImageUrl);
					post.ImageUrl = newImagePath;
				}
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
            var authors = await _authorRepository.GetAuthorsAsync();
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

        public async Task<IActionResult> Index(PostFilterModel model, [FromQuery(Name = "p")] int pageNumber=1, 
			[FromQuery(Name ="ps")] int pageSize=5)
		{
			_logger.LogInformation("Tạo điền kiện truy vấn");
			
			var postQuery = _mapper.Map<PostQuery>(model);
			_logger.LogInformation("Lấy danh sách bài viết từ CSDL");
			
			ViewBag.PostsList = await _blogResponsitory.GetPagedPostsAsync(postQuery, pageNumber, pageSize);
            _logger.LogInformation("Chuẩn bị dữ liệu vào cho ViewModel");

            await PopulatePostFilterModelAsync(model);
			return View(model);
		}
        public async Task<IActionResult> ChangePublishPost(int id)
        {
            await _blogResponsitory.ChangedPublishedPostAsync(id);
            return RedirectToAction("Index");
        }

        // delete post
        public async Task<IActionResult> DeletePost(int id)
        {
            await _blogResponsitory.DeletePostById(id);
            return RedirectToAction("Index");

        }


    }
}
