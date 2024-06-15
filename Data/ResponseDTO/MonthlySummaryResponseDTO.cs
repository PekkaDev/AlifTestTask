namespace WalletAPI.Data.DTO;

public class MonthlySummaryResponseDTO
{
    public int TransactionCount { set; get; }
    public decimal TransactionSum { get; set; }
    public ICollection<TransactionDTO> TopUpTransactions { get; set; }
}

public class TransactionDTO
{
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
}