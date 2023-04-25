using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.DTO
{
    public class TagQuery
    {
        public int Id { get; set; }
        public string Keyword { get; set; }
        public string Name { get; set; }
        public string UrlSlug { get; set; }
    }
}
