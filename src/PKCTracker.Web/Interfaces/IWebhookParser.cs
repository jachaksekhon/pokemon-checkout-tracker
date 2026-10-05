using Discord;
using PKCTracker.Web.Models;

namespace PKCTracker.Web.Interfaces;

public interface IWebhookParser
{
    CheckoutResult Parse(Embed embed);
}