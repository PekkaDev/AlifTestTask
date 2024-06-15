using Microsoft.EntityFrameworkCore;
using WalletAPI.Data;
using WalletAPI.Data.Entity;

namespace WalletAPI;

public class WalletService
{
    private const decimal MAX_IDENTIFIED_BALANCE = 100_000;
    private const decimal MAX_UNIDENTIFIED_BALANCE = 10_000;

    private readonly WalletDbContext _dbContext;

    public WalletService(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Wallet?> FindByUserLogin(string userLogin)
    {
        return await _dbContext.Wallets
            .Include(it => it.Transactions)
            .FirstOrDefaultAsync(it => it.UserLogin == userLogin);
    }

    public async Task<bool> TopUpAsync(Wallet wallet, decimal amount)
    {
        var currentBalance = GetBalance(wallet);
        var maxBalance = wallet.IsIdentified
            ? MAX_IDENTIFIED_BALANCE
            : MAX_UNIDENTIFIED_BALANCE;

        if (currentBalance + amount > maxBalance)
            return false;

        wallet.Transactions.Add(new Transaction
        {
            TransactionAmount = amount,
            Wallet = wallet,
            TransactionDate = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public ICollection<Transaction> GetTransactionsForCurrentMonth(Wallet wallet)
    {
        return wallet.Transactions
            .Where(it => it.TransactionDate.Month == DateTime.UtcNow.Month)
            .ToList();
    }

    public decimal GetBalance(Wallet wallet)
    {
        return wallet.Transactions
            .Where(it => it.Wallet == wallet)
            .Sum(it => it.TransactionAmount);
    }
}