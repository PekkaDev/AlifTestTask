using System.ComponentModel.DataAnnotations;

namespace WalletAPI.Data.RequestDTO;

public class MonthlySummaryRequestDTO
{
    [Required]
    public required string UserLogin { get; set; }
}