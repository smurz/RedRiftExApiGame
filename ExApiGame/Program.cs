using DataLayer;
using ExApiGame.Controllers.Validators;
using GameLogic;
using GameLogic.Base;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<GameResultContext>(opt => opt.UseInMemoryDatabase(GameResultContext.ConnectionString), ServiceLifetime.Transient);

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
