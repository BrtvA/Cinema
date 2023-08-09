using System.ComponentModel.DataAnnotations;

namespace Cinema.DAL.Entities;

public class Genre
{
    [Required]
    public int Id { get; set; }
    [StringLength(30)]
    [MinLength(2)]
    [Required]
    [RegularExpression("^[А-Яа-яЁё ]+$")]
    public string Name { get; set; } = null!;

    public List<MovieGenre> MovieGenres { get; set; } = new();
}