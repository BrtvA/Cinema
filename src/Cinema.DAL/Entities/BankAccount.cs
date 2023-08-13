namespace Cinema.DAL.Entities;

public class BankAccount
{
    public string CardNumber { get; set; } = null!;
    public DateOnly DateEnd { get; set; }
    public int Cvc { get; set; }
    public decimal Money { get; set; }
}
