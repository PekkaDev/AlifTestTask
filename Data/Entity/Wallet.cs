using Microsoft.EntityFrameworkCore;

namespace WalletAPI.Data.Entity;

[Index(nameof(UserId), IsUnique = true)]
[Index(nameof(UserLogin), IsUnique = true)]
public class Wallet
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public required string UserLogin { get; set; }
    public bool IsIdentified { get; set; }

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}