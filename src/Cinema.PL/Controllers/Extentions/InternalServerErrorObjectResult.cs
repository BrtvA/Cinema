using Microsoft.AspNetCore.Mvc;

namespace Cinema.PL.Controllers.Extentions;

public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object? value)
        : base(value)
    {
        StatusCode = 500;
    }
}
