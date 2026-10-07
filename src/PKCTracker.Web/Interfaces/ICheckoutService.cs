using PKCTracker.Web.Models;

namespace PKCTracker.Web.Interfaces;

public interface ICheckoutService
{
    Task ProcessCheckoutAsync(CheckoutResult checkout);
}

