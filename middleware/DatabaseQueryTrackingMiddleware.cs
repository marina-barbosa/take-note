
public class DatabaseQueryTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _logFilePath;
    private readonly string arquivoLog = "logs/middleware/registro.txt";

    public DatabaseQueryTrackingMiddleware(RequestDelegate next)
    {
        _next = next;
        _logFilePath = arquivoLog;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        var currentTime = DateTime.Now;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var logMessage = $"{currentTime} - Método: {requestMethod} - Path: {requestPath}";

        if (context.Request.Query.Count > 0)
        {
            logMessage += " - Parâmetros da Query: ";
            foreach (var (key, value) in context.Request.Query)
            {
                logMessage += $"{key}={value}, ";
            }
        }

        await WriteLogToFile(logMessage);
    }

    private async Task WriteLogToFile(string logMessage)
    {
        using (StreamWriter writer = new StreamWriter(_logFilePath, true))
        {
            await writer.WriteLineAsync(logMessage);
        }
    }
}
