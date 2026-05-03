namespace GameServer.Application.Common;

public class OrdinalDateString()
{
    public static string GetOrdinalDate(int? precision, bool includeDashes = false)
    {
        DateTime now = DateTime.Now;
        string mil = now.Millisecond.ToString()[2..];
        string min = now.Minute.ToString();
        string hours = now.Hour.ToString();
        string day = now.DayOfYear.ToString();
        string year = $"{now.Year:D3}";
        string fill = includeDashes ? "-" : "";
        return precision switch
        {
            0 => string.Join(fill, [year, day]),
            1 => string.Join(fill, [year, day, hours]),
            2 => string.Join(fill, [year, day, hours, min]),
            3 => string.Join(fill, [year.Substring(1,2), day, hours.Substring(1,1), min.Substring(1,1), mil[1..]]),
            _ => string.Join(fill, [year, day, hours, min, mil])
        };
    }
}