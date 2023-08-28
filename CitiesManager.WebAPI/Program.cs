using CitiesManager.WebAPI.DatabaseContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

//Swagger
builder.Services.AddEndpointsApiExplorer();//Generates desceiption for all endpoints
builder.Services.AddSwaggerGen();// generates OpenAPI specificasion


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHsts();
app.UseHttpsRedirection();

app.UseSwagger(); // creates endpoint for swagger.json
app.UseSwaggerUI(); //creates swaggger UI for testing all Web API endpoints/action methods

app.UseAuthorization();

app.MapControllers();

app.Run();
