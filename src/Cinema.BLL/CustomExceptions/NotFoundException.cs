namespace Cinema.BLL.CustomExceptions;

/// <summary>
/// 404 Not Found status code
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException() { }

    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception inner) : base(message, inner) { }
}
