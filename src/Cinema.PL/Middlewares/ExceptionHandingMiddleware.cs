using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Cinema.PL.Middlewares;

internal class ExceptionHandingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandingMiddleware> _logger;

    public ExceptionHandingMiddleware(RequestDelegate next,
                                      ILogger<ExceptionHandingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (DbUpdateException ex)
        {
            await HandleExceptionAsync(httpContext,
                $"{ex.Message}\n{ex.StackTrace}",
                HttpStatusCode.InternalServerError,
                "Такой объект уже есть");
        }
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(httpContext,
                $"{ex.Message}\n{ex.StackTrace}",
                HttpStatusCode.BadRequest,
                "Некорректное значение");
        }
        catch (OperationCanceledException ex)
        {
            await HandleExceptionAsync(httpContext,
               $"{ex.Message}\n{ex.StackTrace}",
               HttpStatusCode.InternalServerError,
               "Ошибка базы данных");
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(httpContext,
                $"{ex.Message}\n{ex.StackTrace}",
                HttpStatusCode.InternalServerError,
                "База данных не доступна");
        }
        catch(IOException ex)
        {
            await HandleExceptionAsync(httpContext,
               $"{ex.Message}\n{ex.StackTrace}",
               HttpStatusCode.InternalServerError,
               "Файл недоступен");
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext,
                $"{ex.Message}\n{ex.StackTrace}",
                HttpStatusCode.InternalServerError,
                "Непредвиденная ошибка");
        }
    }

    private async Task HandleExceptionAsync(HttpContext context,
                                            string exMsg,
                                            HttpStatusCode httpStatusCode,
                                            string message)
    {
        _logger.LogError(
            "{DateTime}: {Message}", DateTime.Now, exMsg
        );

        HttpResponse response = context.Response;
        response.ContentType = "text/html";
        response.StatusCode = (int)httpStatusCode;

        await response.WriteAsJsonAsync(message);
    }
}