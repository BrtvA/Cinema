using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class AdminMovieReqDTO
{
    [Required]
    public int Page { get; set; } = 1;
    [StringLength(50)]
    [MinLength(1)]
    [Required]
    [RegularExpression("^[А-Яа-яЁё0-9:\\- ]+$")]
    public string Search { get; set; } = "";
}
