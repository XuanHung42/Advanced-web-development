using System.ComponentModel;

namespace TatBlog.WebApi.Models
{
    public class PostFilterModel:PagingModel
    {
        public int? AuthorId { get; set; }

        public int? CategoryId { get; set; }
        public bool? IsPublished { get; set; }
        public string Title { get; set; }

    }
}
