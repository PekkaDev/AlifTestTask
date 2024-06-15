using System.ComponentModel.DataAnnotations;

namespace WalletAPI.Data.RequestDTO;

public class TopUpRequestDTO
{
    [Required]
    public required string UserLogin { get; set; }
    [Range(1, int.MaxValue)]
    public decimal Amount { get; set; }
}