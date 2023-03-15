using FluentValidation;
using System.Drawing;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
    public class PostValidator:AbstractValidator<PostEditModel>
    {
        private readonly IBlogResponsitory _blogResponsitory;
        public PostValidator(IBlogResponsitory blogResponsitory)
        {
            _blogResponsitory = blogResponsitory;
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(500);
            RuleFor(x => x.ShortDescription)
                .NotEmpty();
            RuleFor(x => x.Description)
                .NotEmpty();
            RuleFor(x=> x.Meta)
                .NotEmpty()
                .MaximumLength(1000);
            RuleFor(x=> x.UrlSlug)
                .MustAsync(async (postModel, slug, cancellacationToken)=> !await blogResponsitory
                .IsPostSlugExistedAsync(postModel.Id, slug, cancellacationToken))
                .WithMessage("Slug, '{PropertyValue}' đã được sử dụng");
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("Bạn phải chọn tác chủ đề của bài viết");
            RuleFor(x => x.AuthorId)
                .NotEmpty()
                .WithMessage("Bạn phải chọn tác giả của bài viết ");
            When(x => x.Id <= 0, () =>
            {
                RuleFor(x => x.ImageFile)
                .Must(x => x is { Length: > 0 })
                .WithMessage("Bạn phải chọn hình ảnh cho bài viết");
            })
                .Otherwise(() => RuleFor(x => x.ImageFile)
                .MustAsync(SetImageIfNotExist)
                .WithMessage("Bạn phải chọn hình ảnh cho bài viết"));
        }

        private bool HasAtLeastOneTag(PostEditModel postModel, string selectedTags)
        {
            return postModel.GetSelectedTags().Any();
        }

        private async Task<bool> SetImageIfNotExist( PostEditModel postModel, IFormFile imageFile, CancellationToken cancellationToken)
        {
            var post = await _blogResponsitory.GetPostbyIdAsync(postModel.Id, false, cancellationToken);
            if (!string.IsNullOrWhiteSpace(post?.ImageUrl)) {
                return true;
            }
            return imageFile is { Length: > 0 };

        }
    }
}
