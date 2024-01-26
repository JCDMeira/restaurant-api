using RestaurantApi.Services;
using RestaurantApi.Models;
using restaurant_api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<RestaurantDatabaseSettings>(
    builder.Configuration.GetSection("RestaurantDatabase"));
builder.Services.AddTransient<RestaurantService>();

builder.Services.Configure<CategoriesDatabaseSettings>(
    builder.Configuration.GetSection("CategoriesDatabase"));
builder.Services.AddTransient<CategoriesService>();

// comando para instalar o mongo => dotnet add packag MongoDB.driver

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
