using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings
{
	public class CommentMap : IEntityTypeConfiguration<Comment>
	{
		public void Configure(EntityTypeBuilder<Comment> builder)
		{
			builder.ToTable("Comment");
			builder.HasKey(m => m.Id);
			builder.Property(m => m.Content)
				.IsRequired()
				.HasMaxLength(1000);
			builder.Property(m => m.UserName)
				.HasMaxLength(100);
			builder.Property(m=> m.Email)
				.IsRequired()
				.HasMaxLength(100);
			builder.Property(m => m.CommentDate)
				.HasColumnType("datetime");
			builder.HasOne(m => m.Post)
				.WithMany(p => p.Comments)
				.HasForeignKey(m => m.PostId)
				.HasConstraintName("FK_Comments_Post")
				.OnDelete(DeleteBehavior.Restrict);


		}
	}
}
