namespace Saba.Application.Helpers;

static class DatetimeHelper
{
    public static DateTime getDateTimeZoneInfo()
    {
        TimeZoneInfo zonaNicaragua = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
        DateTime fechaActual = DateTime.UtcNow;
        return TimeZoneInfo.ConvertTime(fechaActual, zonaNicaragua);
    }
}
