using CitiesManager.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // postavljamo da je default Content-type: application/json for response type, no request
    //all action methods will produce Content-type: application/json only
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));
}).AddXmlSerializerFormatters();

builder.Services.AddApiVersioning(config =>
{
    //ApiVersionReader identify current version of api per request url
    config.ApiVersionReader = new UrlSegmentApiVersionReader();//Reads version number from request url at "apiVersion" constraint {version:apiVersion}
    //https://localhost:7187/api/v1/cities
    //config.ApiVersionReader = new QueryStringApiVersionReader(); // Reads version number from request query string called "api-version"
    //https://localhost:7187/api/cities?api-version=1.0
    //config.ApiVersionReader = new HeaderApiVersionReader("api-version"); // Reads version number from request header called "api-version" Eg: api-version: 1.0

    //default version per running application
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

//Swagger
builder.Services.AddEndpointsApiExplorer();//Generates desceiption for all endpoints
builder.Services.AddSwaggerGen(options => 
{
    //da bi swagger prikazivao xml comments
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "api.xml"));
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Cities Web API", Version = "1.0" });
    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "Cities Web API", Version = "2.0" });
});// generates OpenAPI specificasion

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; //1.012
    options.SubstituteApiVersionInUrl = true;
});

//Add CORS (cross-origin resource sharing) http://localhost:4200, http://localhost:4100
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        //builder.WithOrigins("http://localhost:4200"); // * response can be allowed by any domain
        policyBuilder
        .WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>())
        .WithHeaders("Authorization", "origin", "accept", "content-type")
        .WithMethods("GET", "POST", "PUT", "DELETE");
    });
    //custom policy
    options.AddPolicy("4100Client",policyBuilder =>
    {
        policyBuilder
        .WithOrigins(builder.Configuration.GetSection("AllowedOrigins2").Get<string[]>())
        .WithHeaders("Authorization", "origin", "accept", "content-type")
        .WithMethods("GET");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();
app.UseHttpsRedirection();

app.UseSwagger(); // creates endpoint for swagger.json
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "1.0");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "2.0");
}); //creates swaggger UI for testing all Web API endpoints/action methods

app.UseRateLimiter();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
