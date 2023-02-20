using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Data.Seeders
{
    public class DataSeeder:IDataSeeder
    {
        private readonly BlogDdContext _dbContext;

        public DataSeeder(BlogDdContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Initialize()

        {
            _dbContext.Database.EnsureCreated();
            if (_dbContext.Posts.Any()) return;
            var authors = AddAuthors();
            var categories = AddCategories();
            var tags = AddTags();
            var posts = AddPosts(authors, categories, tags);
        }

        private IList<Author> AddAuthors()
        {
            var authors = new List<Author>()
        {
            new()
            {
                FullName = "Masson Mouth",
                UrlSlug = "masson-mouth",
                Email ="mouth@gmail.com",
                JoinDate = new DateTime(2022, 11,11)
            },
            new()
            {
                FullName = "Wonder Woman",
                UrlSlug = "wonder-woman",
                Email = "wonder@gmail.com",
                JoinDate = new DateTime(2022, 12, 3)
            }

        };

            _dbContext.AddRange(authors);
            _dbContext.SaveChanges();
            return authors;
        }

        private IList<Category> AddCategories()
        {
            var categories = new List<Category>()
            {
                new(){ Name= "Net.Core", Description = "Net.Core", UrlSlug="net-core" },
               new(){ Name= "Architechture", Description="Architechture ", UrlSlug="architech"},
               new(){ Name="Mesaging", Description = "Mesaging", UrlSlug="mesaging"},
               new(){Name = "OPP", Description ="Ojected-Oriented Program", UrlSlug="opp"},
               new(){Name="Design Patterns", Description= "Design Patterns", UrlSlug= "design-patterns"}

            };
            _dbContext.AddRange(categories);
            _dbContext.SaveChanges();
            return categories;
        }
        private IList<Tag> AddTags()
        {
            var tags = new List<Tag>()
            {
                new(){Name ="Google", Description="Google Applications", UrlSlug="google"},
                new(){Name="ASP.NET MCV", Description = "ASP.NET MCV", UrlSlug = "asp-net"},
                new(){Name = "Razor Page", Description = "Razor Page", UrlSlug= "razorpage"},
                new(){ Name = "Blazor", Description = "Blazor", UrlSlug= "blazor"},
                new(){ Name = "Deep Learning", Description = "Deep Learning", UrlSlug= "deep-learing"},
                new(){ Name = "Neural Network", Description = "Neural Network", UrlSlug= "neural-network"}
            };
            _dbContext.AddRange(tags);
            _dbContext.SaveChanges();
            return tags;

        }
        private IList<Post> AddPosts(

            IList<Author> authors, IList<Category> categories, IList<Tag> tags)
        {
            var posts = new List<Post>()
            {
                new()
                {
                    Title = "ASP.NET Core Diagnostic Scenarios",
                    ShortDescription = "David and friend has great .....",
                    Description = "Here's a few DON'T and DO example...",
                    Meta = "Nothing to read...",
                    UrlSlug = "aspnet-core-diagnostic-scenarios",
                    Published = true,
                    PostedDay = new DateTime(2021, 9, 30, 10, 20, 0),
                    ModifiedDate = null,
                    ViewCount = 10,
                    Author = authors[0],
                    Catelory = categories[0],
                    Tags = new List<Tag>()
                    {
                        tags[0]
                    }
                }

            };
            _dbContext.AddRange(posts);
            _dbContext.SaveChanges();
            return posts;

        }


    
}
}
