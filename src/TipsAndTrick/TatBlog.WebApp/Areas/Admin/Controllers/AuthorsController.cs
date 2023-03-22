using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.DTO;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers
{
	public class AuthorsController:Controller
	{
        private readonly IBlogResponsitory _blogResponsitory;
        private readonly IAuthorResponsitory _authorResponsitory;
        private readonly IMapper _mapper;
        private readonly IMediaManager _mediaManager;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IBlogResponsitory blogResponsitory, IAuthorResponsitory authorResponsitory, IMapper mapper, IMediaManager mediaManager, ILogger<AuthorsController> logger)
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
                Keyword = model.Keyword
            };

            ViewBag.AuthorsList = await _blogResponsitory.GetPageAuthorAsync(authorQuery, pageNumber, pageSize);

            return View(model);
        }
            public async Task<IActionResult> DeleteAuthor(int id)
        {
            await _blogResponsitory.DeletePostById(id);
            return RedirectToAction("Index");

        }
    }
}
