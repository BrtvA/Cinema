using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class RegisterReqDTO : LoginReqDTO
{
    [StringLength(20)]
    [MinLength(2)]
    [Required]
    [RegularExpression("^[А-Яа-яЁёA-Za-z ]+$")]
    public string Name { get; set; } = null!;

    public override void Trim()
    {
        Name = Name.Trim();
        base.Trim();
    }
}