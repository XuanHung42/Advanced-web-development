using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Services.Blogs
{
	public class CommentResponsitory : ICommentResponsitory
	{
		private readonly BlogDdContext _context;

		public CommentResponsitory(BlogDdContext context)
		{
			_context = context;
		}

		public Task<Comment> FindCommentByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IList<Comment>> FindCommentByPostIdAsync(int postId, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task<IList<Comment>> FindCommentsByEmailAsync(string email, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}
	}
}
