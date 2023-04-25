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
	public class CategoriesController:Controller
	{

        private readonly IBlogResponsitory _blogResponsitory;
        private readonly IAuthorRepository _authorResponsitory;
        private readonly IMapper _mapper;
        private readonly IMediaManager _mediaManager;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(IBlogResponsitory blogResponsitory, IAuthorRepository authorResponsitory, IMapper mapper, IMediaManager mediaManager, ILogger<CategoriesController> logger)
        {
            _blogResponsitory = blogResponsitory;
            _authorResponsitory = authorResponsitory;
            _mapper = mapper;
            _mediaManager = mediaManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CategoryFilterModel model, [FromQuery(Name = "p")] int pageNumber = 1,

        [FromQuery(Name = "ps")] int pageSize = 3)
        {

            //var categoryQuery = _mapper.Map<CategoryQuery>(model);
            var categoryQuery = new CategoryQuery()
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                ShowMenu = model.ShowMenu
            };


            ViewBag.CategoriesList = await _blogResponsitory.GetPageCategoriesAsync(categoryQuery, pageNumber, pageSize);

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            var author = id > 0
                ? await _authorResponsitory.GetAuthorByIdIsDetailAsync(id, true)
                : null;

            var model = author == null
                ? new CategoryEditModel() :
                _mapper.Map<CategoryEditModel>(author);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromServices] IValidator<CategoryEditModel> authorValidator, CategoryEditModel model)
        {
            var validationResult = await authorValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var category = model.Id > 0
                ? await _blogResponsitory.FindCategoriesByIdAsync(model.Id, true)
                : null;
            if (category == null)
            {
                category = _mapper.Map<Category>(model);
                category.Id = 0;
              

            }

            await _blogResponsitory.CreateOrUpdateCategoryAsync(category);
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> VerifyCategorySlug(int id, string urlSlug)
        {
            var slugExisted = await _blogResponsitory
                .IsCategoryExistSlugAsync(id, urlSlug);
            return slugExisted
                ? Json($"Slug '{urlSlug}' đã được sử dụng")
                : Json(true);
        }
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _blogResponsitory.DeleteCategoryById(id);
            return RedirectToAction("Index");

        }
    }
}

