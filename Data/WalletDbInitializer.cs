using WalletAPI.Data.Entity;

namespace WalletAPI.Data;

public static class WalletDbInitializer
{
    public static void Initialize(WalletDbContext dbContext)
    {
        if (dbContext.Wallets.Any())
            return;

        var prevMonth = DateTime.UtcNow.AddMonths(-1);
        var currMonth = DateTime.UtcNow;

        var wallets = Enumerable.Range(1, 5)
            .Select(it => new Wallet
            {
                UserId = $"user_id_{it}",
                UserLogin = $"user_login_{it}",
                IsIdentified = it > 3,
                Transactions =
                [
                    new Transaction
                    {
                        TransactionAmount = 1_000 * it * (it > 3 ? 10 : 1),
                        TransactionDate = it % 2 == 0 ? prevMonth : currMonth
                    }
                ]
            });

        dbContext.AddRange(wallets);
        dbContext.SaveChanges();
    }
}