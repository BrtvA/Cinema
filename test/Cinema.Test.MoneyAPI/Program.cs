using Cinema.BLL.DTOs;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/buy-info", (Guid guidId, int type) =>
{
    decimal price = 300m;
    string cinemaCardNumber = "0000000000000000";
    return Results.Json(new BuyInfoDTO(cinemaCardNumber, price));
});

app.MapGet("/buy-info-none", (Guid guidId, int type) =>
{
    decimal price = 300m;
    string? cinemaCardNumber = null;
    return Results.Json(new BuyInfoDTO(cinemaCardNumber, price));
});

app.MapGet("/buy-info-card-invalid", (Guid guidId, int type) =>
{
    decimal price = 300m;
    string cinemaCardNumber = "0000000000000001";
    return Results.Json(new BuyInfoDTO(cinemaCardNumber, price));
});

app.MapGet("/buy-info-to-much", (Guid guidId, int type) =>
{
    decimal price = 1000000m;
    string cinemaCardNumber = "0000000000000000";
    return Results.Json(new BuyInfoDTO(cinemaCardNumber, price));
});

app.Run();

public partial class Program { }
