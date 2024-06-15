using Microsoft.EntityFrameworkCore;
using WalletAPI.Data.Entity;

namespace WalletAPI.Data;

public class WalletDbContext : DbContext
{
    public DbSet<Wallet> Wallets { get; set; }

    public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
    {
    }
}