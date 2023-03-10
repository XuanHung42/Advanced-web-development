using Microsoft.EntityFrameworkCore;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Extensions
{
	public static class WebApplicationExtensions
	{
		public static WebApplicationBuilder ConfigureMvc(
		  this WebApplicationBuilder builder)
		{
			builder.Services.AddControllersWithViews();
			builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
			builder.Services.AddResponseCompression();
			return builder;
		}

		public static WebApplicationBuilder ConfigureServices(
		  this WebApplicationBuilder builder)
		{
			builder.Services.AddDbContext<BlogDdContext>(options =>
			  options.UseSqlServer(
				builder.Configuration
				  .GetConnectionString("DefaultConnection")));

			builder.Services.AddScoped<IBlogResponsitory, BLogResponsitory>();
			builder.Services.AddScoped< IAuthorResponsitory, AuthorResponsitory>();
			builder.Services.AddScoped<IDataSeeder, DataSeeder>();
			return builder;
		}

		public static WebApplication UseRequestPipeline(
		  this WebApplication app)
		{
			if (app.Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Blog/Error");

				// thêm header Strict-Transport-Security vào http response
				app.UseHsts();
			}

			// tự động nén http response
			app.UseResponseCompression();

			// chuyển hướng http sang https
			app.UseHttpsRedirection();

			app.UseStaticFiles();

			// lựa chọn endpoint phù hợp để xử lý http request
			app.UseRouting();
			return app;
		}

		// thêm dữ liệu mẫu vào csdl
		public static IApplicationBuilder UseDataSeeder(
		  this IApplicationBuilder app)
		{
			using var scope = app.ApplicationServices.CreateScope();
			try
			{
				scope.ServiceProvider
				  .GetRequiredService<IDataSeeder>()
				  .Initialize();
			}
			catch (Exception ex)
			{
				scope.ServiceProvider
					.GetRequiredService<ILogger<Program>>()
					.LogError(ex, "Could not insert data into database");
			}
			return app;
		}
	}
}