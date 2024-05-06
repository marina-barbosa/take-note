using Serilog.Core;
using Serilog.Events;

public class CustomLogFilter : ILogEventFilter
{
    public bool IsEnabled(LogEvent logEvent)
    {
        var messageTemplate = logEvent.MessageTemplate.Text;

        return messageTemplate.Contains("Request starting") &&
               messageTemplate.Contains("/swagger/") ||
               messageTemplate.Contains("Executing endpoint") ||
               messageTemplate.Contains("Executed DbCommand") ||
               messageTemplate.Contains("Request finished") &&
               messageTemplate.Contains("/swagger/");
    }

}