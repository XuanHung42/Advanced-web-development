// See https://aka.ms/new-console-template for more information
using Azure;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using TatBlog.WinApp;

var context = new BlogDdContext();
var seeder = new DataSeeder(context);

seeder.Initialize();



//IBlogResponsitory blogRepo = new BLogResponsitory(context);

//var posts = await blogRepo.GetPopularArticlesAsync(3);


//foreach (var post in posts)
//{
//    Console.WriteLine("ID       :{0}", post.Id);
//    Console.WriteLine("Title    :{0}", post.Title);
//    Console.WriteLine("View     :{0}", post.ViewCount);
//    Console.WriteLine("Date     :{0:MM/dd/yyyy}", post.PostedDate);
//    Console.WriteLine("Author   :{0}", post.Author.FullName);
//    Console.WriteLine("Category :{0}", post.Category.Name);
//    Console.WriteLine("".PadRight(80, '+'));


//}
//var tags = await blogRepo.GetTagsAsync();
//Console.WriteLine("{0, -5}{1,-50} {2,10}", "ID", "Name", "Count");
//foreach (var tag in tags)
//{
//    Console.WriteLine("{0,-5}{1,-50}{2,10}",
//        tag.Id, tag.Name, tag.PostCount);
//}

//var pagingParams = new PagingParams
//{
//    PageNumber = 1,
//    PageSize = 5,
//    SortColumn = "Name",
//    SortOrder = "DESC"
//};
//var tagList = await blogRepo.GetPagedTagsAsync(pagingParams);
//Console.WriteLine("{0,-5}{1,-50}{2,10}", "ID", "Name", "Count");

//foreach (var tag in tagList)
//{
//    Console.WriteLine("{0,-5}{1,-50}{2,10}", tag.Id, tag.Name, tag.PostCount);

//}

//var tagsId = await blogRepo.FindTagWithId(1);
//Console.WriteLine( tagsId.UrlSlug);

