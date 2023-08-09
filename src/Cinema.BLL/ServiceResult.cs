namespace Cinema.BLL;

public class ServiceResult<T>
{
    public ApplicationException? Exception { get; private set; }
    public string? Error
    {
        get
        {
            if (Exception is not null)
                return Exception.Message;
            return String.Empty;
        }
    }
    public T? Value { get; set; }

    public ServiceResult(ApplicationException resultExeption)
    {
        Exception = resultExeption;
    }

    public ServiceResult(T? value)
    {
        Value = value;
        Exception = null;
    }
}
