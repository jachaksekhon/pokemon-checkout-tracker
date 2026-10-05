# PKCTracker

The goal of this program is to track, in real time, successful and failed orders. This will allow users to fix their profiles so they can have a chance to checkout on the failed profiles

Example web hooks are received as the following:

##### Successful checkouts:

```
Title: **Successful Checkout!**
Site: Pokemon Center CA
Mode: guest
Product (1): Pokemon TCG: Mega Evolution-Delta Reign Pokemon Center Elite Trainer Box
Price (1): 80.99
Quantity (1): 2
Product (2): Pokemon TCG: Mega Evolution-Delta Reign Booster Display Box (36 Packs)
Price (2): 218.99
Quantity (2): 1
Product (3): Pokemon TCG: Mega Evolution-Delta Reign Booster Bundle (6 Packs)
Price (3): 36.99
Quantity (3): 1
Email: ||email@email.com||
Profile: ||Profile Name||
Order ID: ||P0000000000||
Order Link: ||https://www.pokemoncenter.com/en-ca/orders/P0000000000?postalCode=ABC+123||
Proxy: ||11.11.111.11:11111:user:pass||
```

##### Failed checkouts:
```
Title: **Payment Declined!**
Site: Pokemon Center CA
Mode: guest
Product (1): Pokemon TCG: Mega Evolution-Delta Reign Pokemon Center Elite Trainer Box
Price (1): 80.99
Quantity (1): 2
Product (2): Pokemon TCG: Mega Evolution-Delta Reign Booster Display Box (36 Packs)
Price (2): 218.99
Quantity (2): 1
Product (3): Pokemon TCG: Mega Evolution-Delta Reign Booster Bundle (6 Packs)
Price (3): 36.99
Quantity (3): 1
Email: ||email@email.com||
Profile: ||Profile Name||
Proxy: ||11.11.111.11:11111:user:pass||
```

## Build and run

```powershell
dotnet build PKCTracker.sln
dotnet run --project src/PKCTracker.Web/PKCTracker.Web.csproj
```