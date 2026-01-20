using Application;
using Infrastructure;
using Infrastructure.Configuration;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Application Layer (Commands, Queries, Validators, Mappings)
builder.Services.AddApplication();

// Infrastructure (Repositories, Domain Services, etc.)
builder.Services.AddInfrastructure();

// JWT Authentication & Authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// Controllers with global exception filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<Infrastructure.Filters.GlobalExceptionFilter>();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
