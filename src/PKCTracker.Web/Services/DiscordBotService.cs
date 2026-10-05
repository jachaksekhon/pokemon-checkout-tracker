using Discord;
using Discord.WebSocket;

namespace PKCTracker.Web.Services;

public class DiscordBotService : BackgroundService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;

    public DiscordBotService(IConfiguration configuration)
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        };

        _client = new DiscordSocketClient(config);
        _client.Ready += OnReadyAsync;
        _client.MessageReceived += OnMessageReceivedAsync;

        _configuration = configuration;
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
        Console.WriteLine($"Message received from {message.Author.Username}: {message.Content}");

        return Task.CompletedTask;
    }

    private String GetBotToken()
    {
        return _configuration["Discord:BotToken"] ?? throw new InvalidOperationException("Discord bot token is not configured.");
    }

}
