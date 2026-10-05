using Discord;
using Discord.WebSocket;
using PKCTracker.Web.Interfaces;

namespace PKCTracker.Web.Services;

public class DiscordBotService : BackgroundService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;
    private readonly IWebhookParser _webhookParser ;

    public DiscordBotService(IConfiguration configuration, IWebhookParser webhookParser)
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(config);
        _client.Ready += OnReadyAsync;
        _client.MessageReceived += OnMessageReceivedAsync;

        _configuration = configuration;
        _webhookParser = webhookParser;
        
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var token = GetBotToken();

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task OnReadyAsync()
    {
        Console.WriteLine($"Discord bot connected as {_client.CurrentUser.Username}");

        return Task.CompletedTask;
    }

    private Task OnMessageReceivedAsync(SocketMessage message)
    {
        if (message.Embeds.Count == 0) return Task.CompletedTask;

        foreach (var embed in message.Embeds)
        {
            var checkoutResult = _webhookParser.Parse(embed);
            if (checkoutResult != null)
            {
                Console.WriteLine($"Parsed Checkout Result: Profile={checkoutResult.Profile}, OrderNumber={checkoutResult.OrderNumber}, Success={checkoutResult.Success}, OrderLink={checkoutResult.OrderLink}");
            }
        }

        return Task.CompletedTask;
    }

    private String GetBotToken()
    {
        return _configuration["Discord:BotToken"] ?? throw new InvalidOperationException("Discord bot token is not configured.");
    }

}
