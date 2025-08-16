using Arbitrage.Exchange;
using Arbitrage.Graph;
using Arbitrage.Symbols;
using Arbitrage.WebApi.Infastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Разрешить любые источники (домены)
            .AllowAnyMethod() // Разрешить любые HTTP-методы (GET, POST и т. д.)
            .AllowAnyHeader(); // Разрешить любые заголовки
    });
});

builder.Services.AddDbContext<DbContext, AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Для разработки используем SQLite
        options.UseSqlite("Data Source=arbitrage.db");
    }
    // else
    // {
        // Для продакшена - PostgreSQL
        // options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    // }
});

builder.Services.AddExchangeModule();
builder.Services.AddSymbolsModule();
builder.Services.AddGraphModule();

var app = builder.Build();

ApplyMigrations(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}

app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();

void ApplyMigrations(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}