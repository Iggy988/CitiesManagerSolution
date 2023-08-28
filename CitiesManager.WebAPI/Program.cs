using CitiesManager.WebAPI.DatabaseContext;
using Microsoft.AspNetCore.Mvc;
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
});// generates OpenAPI specificasion


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();
app.UseHttpsRedirection();

app.UseSwagger(); // creates endpoint for swagger.json
app.UseSwaggerUI(); //creates swaggger UI for testing all Web API endpoints/action methods

app.UseAuthorization();

app.MapControllers();

app.Run();
