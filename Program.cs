using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using axiosTest.Models;
using axiosTest.Repositories;
using MongoDB.Driver;
// using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using axiosTest.Services;

// using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

using StackExchange.Redis;

using Oracle.ManagedDataAccess.Client;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var jwtSettings = configuration.GetSection("Jwt").Get<Jwt>();

var redisConfiguration = builder.Configuration.GetSection("ConnectionStrings:Redis");

string host = redisConfiguration["Host"];
int port = int.Parse(redisConfiguration["Port"]);
string password = redisConfiguration["Password"];
bool useSsl = bool.Parse(redisConfiguration["UseSsl"]);

var redisOptions = new ConfigurationOptions
{
    EndPoints = { $"{host}:{port}" },
    Ssl = useSsl,
    Password = password,
    AbortOnConnectFail = false
};

redisOptions.CertificateValidation += (sender, certificate, chain, sslPolicyErrors) => true;

var redis = ConnectionMultiplexer.Connect(redisOptions);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
// Console.WriteLine($"Redis connection failed: {ex.Message}");

builder.Services.Configure<MongoDbSettings>(
    configuration.GetSection("ConnectionStrings:MongoDbSettings")
);

builder.Services.AddSingleton<IMongoClient>(s =>
{
    var settings = configuration.GetSection("ConnectionStrings:MongoDbSettings").Get<MongoDbSettings>();
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<MongoDbService>();

/*
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseMySql(
        configuration.GetConnectionString("DefaultConnection"),
        Microsoft.EntityFrameworkCore.ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
    ));
*/

builder.Services.AddDbContext<OracleDbContext>(options => 
{
    options.UseOracle(
    	configuration.GetConnectionString("OracleDbConnection")
    );
});

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var connectionString = configuration.GetConnectionString("OracleDbConnection");
    return new OracleConnection(connectionString);
});

builder.Services.AddScoped<OracleRepository>();
builder.Services.AddScoped<FormInfoRepository>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSettings.Issuer,  // 替換為你的發行者
        ValidAudience = jwtSettings.Audience,  // 替換為你的觀眾
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
    
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("token"))
            {
                context.Token = context.Request.Cookies["token"];
            }
            
            return Task.CompletedTask;
        }
    };
})
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = configuration["Google:ClientId"];
    options.ClientSecret = configuration["Google:ClientSecret"];
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7188, listenOptions =>
    {
        listenOptions.UseHttps("/https/https-cert.pfx", "");
    });
});

/*
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings.Issuer,  // 替換為你的發行者
            ValidAudience = jwtSettings.Audience,  // 替換為你的觀眾
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });
*/

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 註冊 CORS
// .AllowCredentials()
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("https://localhost:5173", "https://127.0.0.1:5173", "https://localhost:5173/index/forms/system-permission-apply", "https://127.0.0.1:5173/index/forms/system-permission-apply")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
// Add services to the container.
/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.SetIsOriginAllowed(_ => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});*/

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception: {ex}");
        throw;
    }
});

app.UseCors("AllowReactApp");
// 使用身份驗證和授權中介軟體
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapControllers();  // 映射控制器
app.MapGet("/", () => "Hello HTTPS!");
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

/*
public void ConfigureServices(IServiceCollection services)
{
    services.AddDbContext<BlogDbContext>(options =>
        options.UseMySql(
            Configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection"))
        ));
}
*/
