using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.DTO
{
	public class PostQuery
	{

		public string Keyword { get; set; }
		public string CategoryId { get; set; }
		public string AuthorId { get; set; }
		public string PostMonth { get; set; }
	}
}
