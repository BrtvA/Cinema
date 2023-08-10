using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Cinema.BLL.DTOs.Request;

public class MovieReqDTO
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Id { get; set; }
    [StringLength(50)]
    [MinLength(1)]
    [Required]
    [RegularExpression("^[А-Яа-яЁё0-9:\\- ]+$")]
    public string Name { get; set; } = null!;
    [StringLength(200)]
    [MinLength(1)]
    [Required]
    [RegularExpression("^[А-Яа-яЁёA-Za-z0-9.,\\- ]+$")]
    public string Description { get; set; } = null!;
    [Required]
    public IFormFile? Image { get; set; }
    [Required]
    [MinLength(1)]
    public int[] GenresId { get; set; } = null!;
    [Required]
    [Range(1, int.MaxValue)]
    public int Duration { get; set; } //minutes
    [Required]
    [Range(0, Double.PositiveInfinity)]
    public decimal Price { get; set; }
    [Required]
    public bool IsActual { get; set; }

    public virtual void Trim()
    {
        Name = Name.Trim();
        Description = Description.Trim();
    }
}
