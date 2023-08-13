using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Entities;

public class User : BaseShortModel
{
    public string Email { get; set; } = null!;
    public int UserTypeId { get; set; }
    public string Password { get; set; } = null!;

    public UserType? UserType { get; set; }
    public List<Order> Orders { get; set; } = new();
}