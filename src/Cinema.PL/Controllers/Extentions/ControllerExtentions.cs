using Cinema.BLL;
using Cinema.BLL.CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cinema.PL.Controllers.Extentions;

public static class ControllerExtentions
{
    private static IActionResult ErrorResult<SResult>(
        this ServiceResult<SResult> result)
    {
        return result.Exception switch
        {
            BadRequestException => new BadRequestObjectResult(result.Error),
            NotFoundException => new NotFoundObjectResult(result.Error),
            _ => new InternalServerErrorObjectResult(result.Error)
        };
    }

    public static async Task<IActionResult> ToDoAsync<SResult>(
        this ServiceResult<SResult> result, Func<SResult, Task<IActionResult>> action)
    {
        if (result.Exception is null && result.Value is not null)
        {
            return await action(result.Value);
        }
        else
        {
            return result.ErrorResult();
        }
    }

    public static IActionResult ToDo<SResult>(
        this ServiceResult<SResult> result, Func<SResult, IActionResult> action)
    {
        if (result.Exception is null 
            && (result.Value is decimal || result.Value is not null))
        {
            return action(result.Value);
        }
        else
        {
            return result.ErrorResult();
        }
    }

    public static async Task<IActionResult> ToValidate(
        this ModelStateDictionary modelState,
        Func<Task<IActionResult>> action)
    {
        if (modelState.IsValid)
        {
            return await action();
        }
        else
        {
            return new BadRequestObjectResult("Валидация не пройдена");
        }
    }
}
