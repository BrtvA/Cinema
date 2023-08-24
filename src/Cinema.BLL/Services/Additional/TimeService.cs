using Cinema.BLL.CustomExceptions;

namespace Cinema.BLL.Services.Additional;

internal static class TimeService
{
    public static DateTime CheckDate(string date)
    {
        if (date is null || date == "")
        {
            return DateTime.Now.Date;
        }
        else
        {
            return DateTime.ParseExact(date, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    public static object CheckStartTime(string date, int dateShift)
    {
        DateTime currentDate = DateTime.Now.Date;
        DateTime startDate = CheckDate(date);

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
