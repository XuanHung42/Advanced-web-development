// See https://aka.ms/new-console-template for more information
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;

var context = new BlogDdContext();
var seeder = new DataSeeder(context);
seeder.Initialize();
var authors = context.Authors.ToList();
Console.WriteLine("{0,-4}{1,-30}{2,-30},{3,12}","ID","Full Name","Email", "Join Date");
foreach (var auth in authors)
{
    Console.WriteLine("{0,-4}{1,-30}{2,-30}{3,12:MM/dd/yyyy}", auth.Id, auth.FullName, auth.Email,auth.JoinDate);

}
