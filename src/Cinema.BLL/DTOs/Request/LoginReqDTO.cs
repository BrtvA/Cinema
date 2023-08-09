using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class LoginReqDTO
{
    [StringLength(50)]
    [MinLength(4)]
    [Required]
    [RegularExpression("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")]
    public string Email { get; set; } = null!;
    [StringLength(50)]
    [MinLength(4)]
    [Required]
    [RegularExpression("^[А-Яа-яЁёA-Za-z0-9]+$")]
    public string Password { get; set; } = null!;

    public virtual void Trim()
    {
        Email = Email.Trim();
        Password = Password.Trim();
    }
}
