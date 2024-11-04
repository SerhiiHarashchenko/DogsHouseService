using DogsHouseService.Data.Repositories;
using DogsHouseService.Mapping;
using DogsHouseService.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DogsHouseServiceDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IDogRepository, DogRepository>();

builder.Services.AddScoped<IDogService, DogService>();

builder.Services.AddAutoMapper(typeof(DogProfile));

var rateLimitSettings = builder.Configuration.GetSection("RateLimiting");

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = rateLimitSettings.GetValue<int>("PermitLimit");
        options.Window = TimeSpan.FromSeconds(rateLimitSettings.GetValue<int>("WindowSeconds"));
        options.QueueLimit = rateLimitSettings.GetValue<int>("QueueLimit");
    });
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseRateLimiter();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
