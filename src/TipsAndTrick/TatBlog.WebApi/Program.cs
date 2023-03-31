using TatBlog.WebApi.Endpoints;
using TatBlog.WebApi.Extensions;


using TatBlog.WebApi.Mapsters;
using TatBlog.WebApi.Validations;

var builder = WebApplication.CreateBuilder(args);

{
    builder
      .ConfigureCors()
      .ConfigureNLog()
      .ConfigureServices()
      .ConfigureSwaggerOpenApi()
      .ConfigureMapster()
      .ConfigureFluentValdation();
}


var app = builder.Build();

{
    app.SetupRequestPipeline();

    app.MapAuthorEndpoints();
    app.MapTagEndpoints();
    app.MapCategoryEndpoints();
    app.MapPostEndpoints();

    app.Run();
}
