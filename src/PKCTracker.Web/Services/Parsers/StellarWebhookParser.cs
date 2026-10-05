using Discord;
using PKCTracker.Web.Models;
using PKCTracker.Web.Interfaces;
using PKCTracker.Web.Constants;

namespace PKCTracker.Web.Services.Parsers;

public class StellarWebhookParser : IWebhookParser
{
    public CheckoutResult? Parse(Embed embed)
    {
        // Check for PKC CA site before parsing further

        var siteField = embed.Fields.FirstOrDefault(f => f.Name == WebhookConstants.SiteField);
        if (string.IsNullOrEmpty(siteField.Value) || siteField.Value != WebhookConstants.PokemonCenterIdentifier) return null;

        var profileField = embed.Fields.FirstOrDefault(f => f.Name == WebhookConstants.ProfileField);
        if (string.IsNullOrEmpty(profileField.Value)) return null;
        
        var profile = profileField.Value.Replace(WebhookConstants.DiscordSpoilerDelimiter, "");

        bool success;
        string? orderNumber = null;
        string? orderLink = null;

        if (embed.Title == WebhookConstants.SuccessfulCheckoutTitle)
        {
            success = true;

            var orderIdField = embed.Fields.FirstOrDefault(f => f.Name == WebhookConstants.OrderIdField);
            if (!string.IsNullOrEmpty(orderIdField.Value))
            {
                orderNumber = orderIdField.Value.Replace(WebhookConstants.DiscordSpoilerDelimiter, "");
            }

            var orderLinkField = embed.Fields.FirstOrDefault(f => f.Name == WebhookConstants.OrderLinkField);
            if (!string.IsNullOrEmpty(orderLinkField.Value))
            {
                orderLink = orderLinkField.Value.Replace(WebhookConstants.DiscordSpoilerDelimiter, "");
            }

        }
        else if (embed.Title == WebhookConstants.PaymentDeclinedTitle)
        {
            success = false;
        }
        else
        {
            return null; 
        }

        return new CheckoutResult
        {
            Profile = profile,
            OrderNumber = orderNumber,
            Success = success,
            OrderLink = orderLink
        };

    }
}
