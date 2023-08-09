namespace Cinema.DAL.Entities;

public class UserType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public List<User> Users { get; set; } = new();
}