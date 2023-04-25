using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
    public class CategoryValidator : AbstractValidator<CategoryEditModel>
    {
        private readonly IBlogResponsitory _blogResponsitoryblog;

        public CategoryValidator(IBlogResponsitory blogResponsitoryblog)
        {
            _blogResponsitoryblog = blogResponsitoryblog;
            RuleFor(x => x.Name)
           .NotEmpty()
           .MaximumLength(500)
           .WithMessage("Tên không được để trống");






            RuleFor(x => x.UrlSlug)
              .NotEmpty()
              .MaximumLength(1000);

            RuleFor(x => x.UrlSlug)
              .MustAsync(async (authorModel, slug, cancellationToken) =>
              !await _blogResponsitoryblog.IsCategoryExistSlugAsync(
                authorModel.Id, slug, cancellationToken))
              .WithMessage(x => $"Slug '{x.UrlSlug}' đã được sử dụng");
            RuleFor(x => x.Description)
              .NotEmpty()
              .MaximumLength(1000)
              .WithMessage("Mô tả không được để trống");




        }
    }
}
