using DataLayer;
using ExApiGame.Controllers.Validators;
using GameLogic;
using GameLogic.Base;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (builder.Environment.EnvironmentName == "Development")
{
    builder.Services.AddDbContextFactory<GameResultContext>(options => options.UseNpgsql(defaultConnectionString));
    builder.Services.AddDbContext<GameResultContext>(options => options.UseNpgsql(defaultConnectionString));
}
else
{
    // Use connection string provided at runtime by Heroku.
    var connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    connectionUrl = connectionUrl.Replace("postgres://", string.Empty);
    var userPassSide = connectionUrl.Split("@")[0];
    var hostSide = connectionUrl.Split("@")[1];

    var user = userPassSide.Split(":")[0];
    var password = userPassSide.Split(":")[1];
    var host = hostSide.Split("/")[0];
    var database = hostSide.Split("/")[1].Split("?")[0];

    defaultConnectionString = $"Host={host};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";

    builder.Services.AddDbContextFactory<GameResultContext>(opt => opt.UseNpgsql(defaultConnectionString), ServiceLifetime.Transient);
    builder.Services.AddDbContext<GameResultContext>(opt => opt.UseNpgsql(defaultConnectionString), ServiceLifetime.Transient);

    var serviceProvider = builder.Services.BuildServiceProvider();
    try
    {
        var dbContext = serviceProvider.GetRequiredService<GameResultContext>();
        dbContext.Database.Migrate();
    }
    catch
    {
    }
}


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<GameIdValidator>();
builder.Services.AddSingleton<PlayerNameValidator>();
builder.Services.AddSingleton<ILobbiesHost, LobbiesHost>();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
