using Discord;
using PKCTracker.Web.Services.Parsers;
using PKCTracker.Web.Constants;
namespace PKCTracker.Tests;

public class StellarWebhookParserTests
{
    [Fact]
    public void Parse_SuccessfulCheckout_ReturnsCheckoutResult()
    {
        var parser = new StellarWebhookParser();

        var embed = new EmbedBuilder()
            .WithTitle(WebhookConstants.SuccessfulCheckoutTitle)
            .AddField(WebhookConstants.SiteField, WebhookConstants.PokemonCenterIdentifier)
            .AddField(WebhookConstants.ProfileField, "TestProfile")
            .AddField(WebhookConstants.OrderIdField, "12345")
            .AddField(WebhookConstants.OrderLinkField, "http://example.com/order/12345")
            .Build();

        var result = parser.Parse(embed);

        Assert.NotNull(result);
        Assert.Equal("TestProfile", result.Profile);
        Assert.Equal("12345", result.OrderNumber);
        Assert.True(result.Success);
        Assert.Equal("http://example.com/order/12345", result.OrderLink);
    }

    [Fact]
    public void Parse_PaymentDeclined_ReturnsFailedCheckoutResult()
    {
        var parser = new StellarWebhookParser();

        var embed = new EmbedBuilder()
            .WithTitle(WebhookConstants.PaymentDeclinedTitle)
            .AddField(WebhookConstants.SiteField, WebhookConstants.PokemonCenterIdentifier)
            .AddField(WebhookConstants.ProfileField, "TestProfile")
            .Build();

        var result = parser.Parse(embed);

        Assert.NotNull(result);
        Assert.Equal("TestProfile", result.Profile);
        Assert.False(result.Success);
        Assert.Null(result.OrderNumber);
        Assert.Null(result.OrderLink);
    }

    [Fact]
    public void Parse_NonPokemonCenter_ReturnsNull()
    {
        var parser = new StellarWebhookParser();

        var embed = new EmbedBuilder()
            .WithTitle(WebhookConstants.SuccessfulCheckoutTitle)
            .AddField(WebhookConstants.SiteField, "Walmart CA")
            .AddField(WebhookConstants.ProfileField, "TestProfile")
            .Build();

        var result = parser.Parse(embed);

        Assert.Null(result);
    }
}
