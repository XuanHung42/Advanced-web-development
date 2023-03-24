using System.ComponentModel;

namespace TatBlog.WebApp.Areas.Admin.Models
{
    public class CategoryFilterModel
    {
        public int Id { get; set; }
        [DisplayName("Từ khóa ...")]

        public string Name { get; set; }
        public string Keyword { get; set; }
        public string Description { get; set; }
        public string UrlSlug { get; set; }
        public bool ShowMenu { get; set; }
        public int PostCount { get; set; }

    }
}
