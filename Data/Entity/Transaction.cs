namespace WalletAPI.Data.Entity;

public class Transaction
{
    public Guid Id { get; set; }
    public decimal TransactionAmount { get; set; }
    public DateTime TransactionDate { get; set; }

    public Wallet Wallet { get; set; } = null!;
}