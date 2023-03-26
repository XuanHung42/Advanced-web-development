using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
	public class TagsController:Controller
	{
        private readonly IBlogResponsitory _blogResponsitory;
        private readonly IMapper _mapper;
        private readonly IMediaManager _mediaManager;
        private readonly ILogger<CategoriesController> _logger;

        public TagsController(IBlogResponsitory blogResponsitory, IMapper mapper, IMediaManager mediaManager, ILogger<CategoriesController> logger)
        {
            _blogResponsitory = blogResponsitory;
            _mapper = mapper;
            _mediaManager = mediaManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(TagFilterModel model, [FromQuery(Name = "p")] int pageNumber = 1,

          [FromQuery(Name = "ps")] int pageSize = 3)
        {

            var tagQuery = _mapper.Map<TagQuery>(model);
            //var authorQuery = new AuthorQuery()
            //{
            //    Id = model.Id,
            //    FullName = model.FullName,
            //    Keyword = model.Keyword,
            //    Notes = model.Notes,
            //    Email = model.Email,
            //};


            ViewBag.TagList = await _blogResponsitory.GetPagedTagAsync(tagQuery, pageNumber, pageSize);

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            var author = id > 0
                ? await _blogResponsitory.FindTagByIdAsync(id, true)
                : null;

            var model = author == null
                ? new TagEditModel() :
                _mapper.Map<TagEditModel>(author);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromServices] IValidator<TagEditModel> tagValidator, TagEditModel model)
        {
            var validationResult = await tagValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var author = model.Id > 0
                ? await _blogResponsitory.FindTagByIdAsync(model.Id, true)
                : null;
            if (author == null)
            {
                author = _mapper.Map<Tag>(model);
                author.Id = 0;

            }


            await _blogResponsitory.CreateOrUpdateTagAsync(author);
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> VerifyTagSlug(int id, string urlSlug)
        {
            var slugExisted = await _blogResponsitory
                .IsTagExistSlugAsync(id, urlSlug);
            return slugExisted
                ? Json($"Slug '{urlSlug}' đã được sử dụng")
                : Json(true);
        }
        public async Task<IActionResult> DeleteTag(int id)
        {
            await _blogResponsitory.DeleteTagById(id);
            return RedirectToAction("Index");

        }

    }
}
