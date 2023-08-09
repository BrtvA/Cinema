namespace Cinema.BLL.CustomExceptions;

/// <summary>
/// 400 Bad Request status code
/// </summary>
public class BadRequestException : ApplicationException
{
    public BadRequestException() { }

    public BadRequestException(string message) : base(message) { }
    public BadRequestException(string message, Exception inner) : base(message, inner) { }
}
