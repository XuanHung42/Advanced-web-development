using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;

namespace TatBlog.Core.DTO
{
	public class PostQuery
	{
		public string Keyword { get; set; }
		public bool PublishedOnly { get; set; }

		public bool NotPublished { get; set; }
		public int? AuthorId { get; set; }
		public int? CategoryId { get; set; }
		public string AuthorSlug { get; set; }
		public string CategorySlug { get; set; }
		public string TagSlug { get; set; }
		public string TitleSlug { get; set; }
		public string CategoryName { get; set; }
		

		public int? Year { get; set; }

		public int? Month { get; set; }
		public string UrlSlug { get; set; }
		public IList<Author> AuthorList { get; set; }
		public IList<Comment> CategoryList { get; set; }
	}
}
