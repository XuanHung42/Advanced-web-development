using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
    public class AuthorValidator:AbstractValidator<AuthorEditModel>
    {
        private readonly IBlogResponsitory _blogResponsitoryblog;
        private readonly IAuthorRepository _authorResponsitory;

        public AuthorValidator(IBlogResponsitory blogResponsitoryblog, IAuthorRepository authorResponsitory)
        {
            _blogResponsitoryblog = blogResponsitoryblog;
            _authorResponsitory = authorResponsitory;

            RuleFor(x => x.FullName)
              .NotEmpty()
              .MaximumLength(500)
              .WithMessage("Tên không được để trống");

            RuleFor(x => x.Email)
              .NotEmpty()
             .WithMessage("Email không được để trống");


            

            RuleFor(x => x.UrlSlug)
              .NotEmpty()
              .MaximumLength(1000);

            RuleFor(x => x.UrlSlug)
              .MustAsync(async (authorModel, slug, cancellationToken) =>
              !await _authorResponsitory.IsAuthorSlugExistedAsync(
                authorModel.Id, slug, cancellationToken))
              .WithMessage(x => $"Slug '{x.UrlSlug}' đã được sử dụng");


            When(x => x.Id <= 0, () =>
            {
                RuleFor(x => x.ImageFile)
                .Must(i => i is { Length: > 0 })
                .WithMessage("Bạn phải chọn hình ảnh cho bài viết");
            })
            .Otherwise(() =>
            {
                RuleFor(x => x.ImageFile)
          .MustAsync(SetImageIfNotExist)
          .WithMessage("Bạn phải chọn hình ảnh cho bài viết");
            });
        }

        

        private async Task<bool> SetImageIfNotExist(
          AuthorEditModel authorModel,
          IFormFile imageFile,
          CancellationToken cancellationToken)
        {
            var post = await _authorResponsitory.GetAuthorByIdAsync(
              authorModel.Id);

            if (!string.IsNullOrWhiteSpace(post?.ImageUrl))
                return true;

            return imageFile is { Length: > 0 };
        }
    }
    }

