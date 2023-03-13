using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TatBlog.Core.Entities
{
	public class Comment
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string UserName { get; set; }
		public string Content { get; set; }
		public DateTime CommentDate { get; set; }
		public int PostId { get; set; }
		public Post Post { get; set; }
	}
}
