using Microsoft.EntityFrameworkCore;
using PawCast.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// EF Core + PostgreSQL
builder.Services.AddDbContext<PawCastDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PawCastDb")
                          ?? throw new InvalidOperationException("Connection string 'PawCastDb' was not found.");

    options.UseNpgsql(connectionString);
});

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();