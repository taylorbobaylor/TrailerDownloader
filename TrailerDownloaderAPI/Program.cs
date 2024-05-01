var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register TmdbService and configure HttpClient
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<TmdbService>(client =>
{
    client.BaseAddress = new Uri("https://api.themoviedb.org/3");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigureHttpClient((serviceProvider, client) =>
{
    var apiKey = serviceProvider.GetRequiredService<IConfiguration>()["TMDB_API_KEY"];
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("The TMDB API key is not configured.");
    }
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// app.UseHttpsRedirection(); // Disabled HTTPS redirection for local development

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
