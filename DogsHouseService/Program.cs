using DogsHouseService.Data.Repositories;
using DogsHouseService.Mapping;
using DogsHouseService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DogsHouseServiceDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IDogRepository, DogRepository>();

builder.Services.AddScoped<IDogService, DogService>();

builder.Services.AddAutoMapper(typeof(DogProfile));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
