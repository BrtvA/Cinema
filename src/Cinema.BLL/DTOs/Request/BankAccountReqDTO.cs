using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class BankAccountReqDTO
{
    [Required]
    public Guid GuidId { get; set; }
    [RegularExpression("^[0-9]{16}")]
    [Required]
    public string CardNumber { get; set; } = null!;
    [Range(1, 12)]
    [Required]
    public int MonthEnd { get; set; }
    [Range(0, 99)]
    [Required]
    public int YearEnd { get; set; }
    [Range(0, 999)]
    [Required]
    public int Cvc { get; set; }
}
