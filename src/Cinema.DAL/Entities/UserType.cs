using Cinema.DAL.Models.Base;

namespace Cinema.DAL.Entities;

public class UserType : BaseShortModel
{
    public List<User> Users { get; set; } = new();
}