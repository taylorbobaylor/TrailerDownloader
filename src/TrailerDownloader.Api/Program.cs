using Microsoft.Extensions.Options;
using Serilog;
using TrailerDownloader.Application.Configuration;
using TrailerDownloader.Application.Interfaces;
using TrailerDownloader.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure services
builder.Services.Configure<TmdbOptions>(
    builder.Configuration.GetSection(TmdbOptions.ConfigSection));

// Register services
builder.Services.AddScoped<ITmdbService, TmdbService>();
builder.Services.AddScoped<IYoutubeService, YoutubeService>();
builder.Services.AddScoped<IMovieService, MovieService>();

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
