using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
    public class TagValidator : AbstractValidator<TagEditModel>
    {
        private readonly IBlogResponsitory _blogResponsitory;

        public TagValidator(IBlogResponsitory blogResponsitory)
        {
            this._blogResponsitory = blogResponsitory;
            RuleFor(x => x.Name)
         .NotEmpty()
         .MaximumLength(500)
         .WithMessage("Tên không được để trống");






            RuleFor(x => x.UrlSlug)
              .NotEmpty()
              .MaximumLength(1000);

            RuleFor(x => x.UrlSlug)
              .MustAsync(async (authorModel, slug, cancellationToken) =>
              !await _blogResponsitory.IsCategoryExistSlugAsync(
                authorModel.Id, slug, cancellationToken))
              .WithMessage(x => $"Slug '{x.UrlSlug}' đã được sử dụng");

        }
    }
}
