using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations
{
    public class PostValidator : AbstractValidator<PostEditModel>
    {
        private readonly IBlogResponsitory _blogRepository;


        public PostValidator(IBlogResponsitory blogRepository)
        {
            _blogRepository = blogRepository;

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
              .MustAsync(async (postModel, slug, cancellationToken) =>
              !await blogRepository.IsPostSlugExistedAsync(
                postModel.Id, slug, cancellationToken))
              .WithMessage(x => $"Slug '{x.UrlSlug}' đã được sử dụng");

            RuleFor(x => x.CategoryId)
              .NotEmpty()
              .WithMessage("Bạn phải chọn chủ đề cho bài viết");

            RuleFor(x => x.AuthorId)
              .NotEmpty()
              .WithMessage("Bạn phải chọn tác giả của bài viết");

            RuleFor(x => x.SelectedTags)
              .Must(HasLeastOneTag)
              .WithMessage("Bạn phải nhập ít nhất một thẻ");

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

        private bool HasLeastOneTag(
          PostEditModel postModel, string selectedTags)
        {
            return postModel.GetSelectedTags().Any();
        }

        private async Task<bool> SetImageIfNotExist(
          PostEditModel postModel,
          IFormFile imageFile,
          CancellationToken cancellationToken)
        {
            var post = await _blogRepository.FindPostByIdAsync(
              postModel.Id, cancellationToken);

            if (!string.IsNullOrWhiteSpace(post?.ImageUrl))
                return true;

            return imageFile is { Length: > 0 };
        }
    }
}