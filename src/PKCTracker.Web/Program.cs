using PKCTracker.Web.Interfaces;
using PKCTracker.Web.Services;
using PKCTracker.Web.Services.Parsers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IWebhookParser, StellarWebhookParser>();
builder.Services.AddHostedService<DiscordBotService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
