using PKCTracker.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<DiscordBotService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
