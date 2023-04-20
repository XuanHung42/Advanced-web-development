using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Validations
{
    public class PostValidator : AbstractValidator<PostEditModel>
    {


        public PostValidator()
        {

            RuleFor(x => x.Title)
              .NotEmpty()
              .MaximumLength(500)
              .WithMessage("Title không được để trống");

            RuleFor(x => x.ShortDescription)
              .NotEmpty();

            RuleFor(x => x.Description)
              .NotEmpty();

            RuleFor(x => x.Meta)
              .NotEmpty()
              .MaximumLength(1000);

            RuleFor(x => x.UrlSlug)
              .NotEmpty()
              .MaximumLength(1000);

            RuleFor(x => x.UrlSlug)
              .NotEmpty()
              .WithMessage(x => $"Slug '{x.UrlSlug}' đã được sử dụng");

            RuleFor(x => x.CategoryId)
              .NotEmpty()
              .WithMessage("Bạn phải chọn chủ đề cho bài viết");

            RuleFor(x => x.AuthorId)
              .NotEmpty()
              .WithMessage("Bạn phải chọn tác giả của bài viết");

        
        }

     
    }
}
