using Microsoft.Data.SqlClient;
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
    public class PostMap : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");
            builder.HasKey(x => x.Id);
            builder.Property(x=> x.Title).IsRequired().HasMaxLength(500);
            builder.Property(x=> x.ShortDescription).IsRequired().HasMaxLength(5000);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(5000);
            builder.Property(x => x.UrlSlug).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Meta).IsRequired().HasMaxLength(1000);
            builder.Property(x => x.ImageUrl).HasMaxLength(1000);
            builder.Property(x=> x.ViewCount).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.Published).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.PostedDay).HasColumnType("datetime");
            builder.Property(x => x.ModifiedDate).HasColumnType("datetime");
            builder.HasOne(x => x.Catelory)
                .WithMany(c => c.Posts)
                .HasForeignKey(x => x.CategoryId)
                .HasConstraintName("FK_Posts_Categories")
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x=> x.Author)
                .WithMany(a=> a.Posts)
                .HasForeignKey(x=> x.AuthorId)
                .HasConstraintName("FK_Post_Authors")
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Tags)
                .WithMany(t => t.Posts)
                .UsingEntity(pt=> pt.ToTable("PostsTags"));
                



        }
    }
}
