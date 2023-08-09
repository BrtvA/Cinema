namespace Cinema.BLL.DTOs;

public class BuyInfoDTO
{
    public decimal Price { get; set; }
    public string? CardNumber { get; set; }

    public BuyInfoDTO(string? cardNumber, decimal price)
    {
        CardNumber = cardNumber;
        Price = price;
    }
}
