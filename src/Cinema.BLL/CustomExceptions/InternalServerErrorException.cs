namespace Cinema.BLL.CustomExceptions;

/// <summary>
/// 500 Internal Server Error status code
/// </summary>
public class InternalServerErrorException : ApplicationException
{
    public InternalServerErrorException() { }

    public InternalServerErrorException(string message) : base(message) { }
    public InternalServerErrorException(string message, Exception inner) : base(message, inner) { }
}
