var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5952");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddCors(options =>
        options.AddPolicy(
            "Support",
            policy =>
                policy
                    .WithOrigins("http://localhost:7777")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
        )
    );

var app = builder.Build();

app.UseCors("Support");

app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/bookings", async (HttpContext context) =>
{
    var repoUri = Environment.GetEnvironmentVariable("REPO_URI") ?? "http://repo:5030";
    using var httpClient = new HttpClient
    {
        BaseAddress = new Uri(repoUri)
    };

    // Forward the request to the repository service
    var response = await httpClient.GetAsync("/bookings");
    if (!response.IsSuccessStatusCode)
    {
        return Results.StatusCode((int)response.StatusCode);
    }

    // Return the data from the repository service
    var data = await response.Content.ReadAsStringAsync();
    return Results.Content(data, "application/json");
});

app.Run();