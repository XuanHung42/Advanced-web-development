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
    public class AuthorsController : Controller
    {
        private readonly IBlogResponsitory _blogResponsitory;
        private readonly IAuthorRepository _authorResponsitory;
        private readonly IMapper _mapper;
        private readonly IMediaManager _mediaManager;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IBlogResponsitory blogResponsitory, IAuthorRepository authorResponsitory, IMapper mapper, IMediaManager mediaManager, ILogger<AuthorsController> logger)
        {
            _blogResponsitory = blogResponsitory;
            _authorResponsitory = authorResponsitory;
            _mapper = mapper;
            _mediaManager = mediaManager;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(AuthorFilterModel model, [FromQuery(Name = "p")] int pageNumber = 1,

            [FromQuery(Name = "ps")] int pageSize = 3)
        {

            //var authorQuery = _mapper.Map<AuthorQuery>(model);
            var authorQuery = new AuthorQuery()
            {
                Id = model.Id,
                FullName = model.FullName,
                Keyword = model.Keyword,
                Notes = model.Notes,
                Email= model.Email,
            };
           

            ViewBag.AuthorList = await _authorResponsitory.GetPageAuthorAsync(authorQuery, pageNumber, pageSize);

            return View(model);
        }
      
        [HttpGet]
        public async Task<IActionResult> Edit(int id = 0)
        {
            var author = id > 0
                ? await _authorResponsitory.GetAuthorByIdIsDetailAsync(id, true)
                : null;

            var model = author == null
                ? new AuthorEditModel() :
                _mapper.Map<AuthorEditModel>(author);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromServices] IValidator<AuthorEditModel> authorValidator, AuthorEditModel model)
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
            var author = model.Id > 0
                ? await _authorResponsitory.GetAuthorByIdIsDetailAsync(model.Id, true)
                : null;
            if (author == null)
            {
                author = _mapper.Map<Author>(model);
                author.Id = 0;
                author.JoinDate = DateTime.Now;

            }

            if (model.ImageFile?.Length > 0)
            {
                var newImagePath = await _mediaManager.SaveFileAsync(
                    model.ImageFile.OpenReadStream(),
                    model.ImageFile.FileName,
                    model.ImageFile.ContentType);
                if (!string.IsNullOrWhiteSpace(newImagePath))
                {
                    await _mediaManager.DeleteFileAsync(author.ImageUrl);
                    author.ImageUrl = newImagePath;
                }
            }

            await _authorResponsitory.AddOrUpdateAsync(author);
            return RedirectToAction(nameof(Index));

        }
        [HttpPost]
        public async Task<IActionResult> VerifyAuthorSlug(int id, string urlSlug)
        {
            var slugExisted = await _authorResponsitory
                .IsAuthorSlugExistedAsync(id, urlSlug);
            return slugExisted
                ? Json($"Slug '{urlSlug}' đã được sử dụng")
                : Json(true);
        }
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            await _authorResponsitory.DeleteAuthorAsync(id);
            return RedirectToAction("Index");

        }

    }
}
