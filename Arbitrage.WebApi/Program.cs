using Arbitrage.Exchange;
using Arbitrage.Graph;
using Arbitrage.User;
using Arbitrage.Scaner;
using Arbitrage.Symbols;
using Arbitrage.WebApi.Infastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Arbitrage.Notification;
using Telegram.Bot;
using Arbitrage.Notification.Domain.Contracts;

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
        options.UseSqlite(builder.Configuration["ConnectionStrings:DefaultConnection"]),
        ServiceLifetime.Transient
    // else
    // {
    // Для продакшена - PostgreSQL
    // options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    // }
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"])
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddExchangeModule();
builder.Services.AddSymbolsModule();
builder.Services.AddGraphModule();
builder.Services.AddScanerModule();
builder.Services.AddUserModule();
builder.Services.AddNotificationModule();

var app = builder.Build();

ApplyMigrations(app);

RunTelegramBot(app);

app.UseAuthentication();
app.UseAuthorization();

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

void RunTelegramBot(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var _botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
        var botHandler = scope.ServiceProvider.GetRequiredService<ITelegramBotHandler>();

        System.Console.WriteLine("Телеграм бот запущен");

        _botClient.StartReceiving(
            updateHandler: botHandler.HandleUpdateAsync,
            errorHandler: botHandler.HandleErrorAsync
        );
    }
}