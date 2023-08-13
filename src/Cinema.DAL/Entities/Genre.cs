using Cinema.DAL.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace Cinema.DAL.Entities;

public class Genre : BaseShortModel
{
    [Required]
    public override int Id { get; set; }
    [StringLength(30)]
    [MinLength(2)]
    [Required]
    [RegularExpression("^[А-Яа-яЁё ]+$")]
    public override string Name { get; set; } = null!;

    public List<MovieGenre> MovieGenres { get; set; } = new();

    public virtual void Trim()
    {
        Name = Name.Trim();
    }
}