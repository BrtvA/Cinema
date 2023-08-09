using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class GenreReqDTO
{
    [StringLength(30)]
    [MinLength(2)]
    [Required]
    [RegularExpression("^[А-Яа-яЁё\\- ]+$")]
    public string Name { get; set; } = null!;
}
