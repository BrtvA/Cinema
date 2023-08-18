using Cinema.BLL.CustomExceptions;

namespace Cinema.BLL.Services.Additional;

internal static class TimeService
{
    public static object CheckStartTime(string date, int dateShift)
    {
        DateTime currentDate = DateTime.Now.Date;
        DateTime startDate;
        if (date is null || date == "")
        {
            startDate = currentDate;
        }
        else
        {
            startDate = DateTime.ParseExact(date, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
        }

        if (startDate < currentDate || startDate > currentDate.AddDays(dateShift))
        {
            return new NotFoundException("Данных не найдено");
        }
        else
        {
            return startDate;
        }
    }
}
