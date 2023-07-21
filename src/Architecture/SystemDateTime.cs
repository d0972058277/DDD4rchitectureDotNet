namespace Architecture;

public class SystemDateTime
{
    private static Func<DateTime>? _utcNowFunc;

    public static DateTime UtcNow => _utcNowFunc is null ?
        throw new ArgumentNullException("SystemDateTime has not been initialized. Please use SystemDateTime.InitUtcNow() to initialize when the program starts.") :
        _utcNowFunc();

    public static void InitUtcNow(Func<DateTime> func) => _utcNowFunc = func;
}
