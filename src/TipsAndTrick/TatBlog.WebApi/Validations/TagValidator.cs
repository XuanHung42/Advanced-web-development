using FluentValidation;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations
{
    public class TagValidator:AbstractValidator<TagEditModel>
    {
        public TagValidator() {
            RuleFor(a => a.Name)
                   .NotEmpty()
                   .WithMessage("Tên tác giả không được để trống ")
                   .MaximumLength(100)
                   .WithMessage("Tên tác giả tối đa 100 ký tự");

            RuleFor(a => a.UrlSlug)
                    .NotEmpty()
                    .WithMessage("UrlSlug không được để trống")
                    .MaximumLength(100)
                    .WithMessage("UrlSlug tối đa 100 ký tự");
           
        }
    }
}
