using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request.Base;

public class DateReqDTO
{
    [StringLength(10)]
    [Required]
    [RegularExpression("^[0-9]{4}-[0-9]{2}-[0-9]{2}")]
    public string Date { get; set; } = "";
}
