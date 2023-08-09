namespace Cinema.PL.Models;

public class PaymentViewModel
{
    public decimal Price { get; set; }
    public Guid GuidId { get; set; }

    public PaymentViewModel(decimal price, Guid guidId)
    {
        Price = price;
        GuidId = guidId;
    }
}
