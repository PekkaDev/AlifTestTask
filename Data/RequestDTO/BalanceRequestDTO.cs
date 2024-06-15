using System.ComponentModel.DataAnnotations;

namespace WalletAPI.Data.RequestDTO;

public class BalanceRequestDTO
{
    [Required]
    public required string UserLogin { get; set; }
}