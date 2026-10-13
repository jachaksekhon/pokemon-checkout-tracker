using PKCTracker.Web.Interfaces;
using PKCTracker.Web.Services;
using PKCTracker.Web.Services.Parsers;
using Microsoft.EntityFrameworkCore;
using PKCTracker.Web.Data;
using PKCTracker.Web.Interfaces.Repositories;
using PKCTracker.Web.Repositories;
using PKCTracker.Web.Interfaces.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IWebhookParser, StellarWebhookParser>();

builder.Services.AddDbContext<TrackerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDropRepository, DropRepository>();
builder.Services.AddScoped<IDropService, DropService>();

builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddHostedService<DiscordBotService>();

builder.Services.AddControllers();

var app = builder.Build();
app.MapControllers();

app.Run();
