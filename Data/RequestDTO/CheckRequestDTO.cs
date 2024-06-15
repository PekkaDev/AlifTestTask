using System.ComponentModel.DataAnnotations;

namespace WalletAPI.Data.RequestDTO;

public class CheckRequestDTO
{
    [Required]
    public required string UserLogin { get; set; }
}