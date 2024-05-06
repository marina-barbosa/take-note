using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using take_note.Services;

public class DatabaseQueryTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITrackService _trackService;
    private readonly string _logFilePath;

    public DatabaseQueryTrackingMiddleware(RequestDelegate next, ITrackService trackService)
    {
        _next = next;
        _trackService = trackService;
        // Defina o caminho do arquivo de log
        _logFilePath = "logmiddleware.txt"; // Caminho relativo ao diretório de trabalho do aplicativo
    }

    public async Task Invoke(HttpContext context)
    {
        // Antes de passar a solicitação para o próximo middleware
        // Registra o tempo inicial
        var startTime = DateTime.Now;

        // Permite que o próximo middleware seja executado
        await _next(context);

        // Após a execução do próximo middleware
        // Registra o tempo final
        var endTime = DateTime.Now;

        // Registra as consultas de banco de dados
        var currentTime = DateTime.Now;
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var logMessage = $"{currentTime} - Método: {requestMethod} - Path: {requestPath} - Tempo de execução: {(endTime - startTime).TotalMilliseconds}ms";

        // Adiciona os dados dos parâmetros, query e corpo da solicitação ao logMessage
        if (context.Request.Query.Count > 0)
        {
            logMessage += " - Parâmetros da Query: ";
            foreach (var (key, value) in context.Request.Query)
            {
                logMessage += $"{key}={value}, ";
            }
        }

        if (context.Request.HasFormContentType)
        {
            logMessage += " - Dados do Formulário: ";
            var form = await context.Request.ReadFormAsync();
            foreach (var (key, value) in form)
            {
                logMessage += $"{key}={value}, ";
            }
        }

        if (context.Request.Method == "POST" || context.Request.Method == "PUT")
        {
            // Adiciona os dados do corpo da solicitação
            using (var reader = new StreamReader(context.Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                logMessage += $" - Dados do Corpo: {body}";
            }
        }

        // Registra a mensagem de log no arquivo
        await WriteLogToFile(logMessage);
    }

    // Método para escrever a mensagem de log no arquivo
    private async Task WriteLogToFile(string logMessage)
    {
        using (StreamWriter writer = new StreamWriter(_logFilePath, true))
        {
            await writer.WriteLineAsync(logMessage);
        }
    }
}
