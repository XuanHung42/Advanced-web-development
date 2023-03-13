using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs
{
	public interface ICommentResponsitory
	{
		Task<Comment> FindCommentByIdAsync(
	  int id,
	  CancellationToken cancellationToken = default);
		Task<IList<Comment>> FindCommentByPostIdAsync(
		  int postId,
		  CancellationToken cancellationToken = default);

		Task<IList<Comment>> FindCommentsByEmailAsync(
		  string email,
		  CancellationToken cancellationToken = default);
	}
}
