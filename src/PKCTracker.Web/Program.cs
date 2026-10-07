using PKCTracker.Web.Interfaces;
using PKCTracker.Web.Services;
using PKCTracker.Web.Services.Parsers;
using Microsoft.EntityFrameworkCore;
using PKCTracker.Web.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IWebhookParser, StellarWebhookParser>();
builder.Services.AddHostedService<DiscordBotService>();

builder.Services.AddDbContext<TrackerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
