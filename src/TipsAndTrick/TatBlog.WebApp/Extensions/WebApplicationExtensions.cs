using Microsoft.EntityFrameworkCore;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Extensions
{
	public static class WebApplicationExtensions
	{
		public static WebApplicationBuilder ConfigureMvc(this WebApplicationBuilder builder)
		{
			builder.Services.AddControllersWithViews();
			builder.Services.AddResponseCompression();

			return builder;
		}
		public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
		{
			builder.Services.AddDbContext<BlogDdContext>(options =>
			options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
			builder.Services.AddScoped<IBlogResponsitory, BLogResponsitory>();
			builder.Services.AddScoped<IDataSeeder, DataSeeder>();
			return builder;
		}
		public static WebApplication UseRequestPipeline(this WebApplication app)
		{
			if (app.Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Blog/Error");
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			return app;
		}

		public static IEndpointRouteBuilder UseBlogRoutes(this IEndpointRouteBuilder endpoint)
		{
			endpoint.MapControllerRoute(
	name: "posts-by-category",
	pattern: "blog/category/{slug}",
	defaults: new { controller = "Blog", action = "Category" });
			endpoint.MapControllerRoute(
				name: "posts-by-tag",
				pattern: "blog/tag/{slug}",
				defaults: new { controller = "Blog", action = "Tag" });
			endpoint.MapControllerRoute(
				name: "single-post",
				pattern: "blog/post/{year:int}/{month:int}/{day:int}/{slug}",
				defaults: new { controller = "Blog", action = "Post" });
			endpoint.MapControllerRoute(
				name: "dafault",
				pattern: "{controller=Blog}/{action=Index}/{id?}");

			return endpoint;
		}
		public static IApplicationBuilder UseDataSeeder(this IApplicationBuilder app)
		{
			using (var scope = app.ApplicationServices.CreateScope())
				try
				{

					scope.ServiceProvider.GetRequiredService<IDataSeeder>().Initialize();

				}
				catch (Exception ex)
				{
					scope.ServiceProvider.GetRequiredService<ILogger<Program>>()
						.LogError(ex, "Could not insert data into database");
				}
			return app;
		}	

	}
}
