using AICodeAssistant.Api.Services;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using AICodeAssistant.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRateLimiter();
// Configure AI service (Ollama)
builder.Services.Configure<AIOptions>(builder.Configuration.GetSection("AI"));
builder.Services.AddHttpClient<IAIService, AIService>((provider, client) =>
{
    var options = provider.GetRequiredService<IOptions<AIOptions>>().Value;
    // Ollama doesn't require authentication, just set the base address
    client.BaseAddress = new Uri(options.Endpoint);
});

builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<IQueryLogService, QueryLogService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();