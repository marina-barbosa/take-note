
using System.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using Serilog.Events;
using take_note.Domain;
using take_note.Domain.Models;

namespace take_note.Services;

public interface ITrackService
{
  Task TrackDatabaseQueries(string texto);
  void ImportLogFromFile(string texto);



  DataTable GetDados();
}

public class TrackService : ITrackService
{
  private readonly MySqlDbContext _context;
  private readonly string arquivo = "registro.txt";
  private readonly string arquivoSerilog = "serilogs/log20240424.txt";
  public TrackService(MySqlDbContext context)
  {
    _context = context;
  }

  public async Task TrackDatabaseQueries(string texto)
  {
    using StreamWriter writer = new StreamWriter(arquivo, true);
    await writer.WriteLineAsync(texto);
  }


  public void ImportLogFromFile(string filePath)
  {
    try
    {
      var logRecords = ReadLogFromFile(filePath);

      foreach (var record in logRecords)
      {
        // Salvar no banco de dados
        _context.LogEntrys.Add(new LogEntry
        {
          Date = record.Date,
          Content = record.Content
        });
      }

      _context.SaveChanges();
    }
    catch (Exception ex)
    {
      // Log do erro
      Console.WriteLine($"Erro ao importar o arquivo de log: {ex.Message}");
      throw; // Propagar a exceção para que o caller saiba que ocorreu um erro
    }
  }

  private List<LogRecord> ReadLogFromFile(string filePath)
  {
    var logRecords = new List<LogRecord>();

    try
    {
      using (StreamReader sr = new StreamReader(filePath))
      {
        string? line;
        string? previousContent = null;
        while ((line = sr.ReadLine()) != null)
        {
          if (line.StartsWith("20") && DateTime.TryParse(line.Substring(0, 23), out var date))
          {
            // Se a linha começa com uma data válida, adicionamos o registro anterior
            if (previousContent != null)
            {
              logRecords.Add(new LogRecord
              {
                Date = date,
                Content = previousContent // Removemos espaços em branco extras
              });
            }

            // Resetamos o conteúdo anterior para a nova linha
            previousContent = line;
          }
          else
          {
            // Se a linha não começa com uma data válida, consideramos parte do conteúdo anterior
            previousContent += Environment.NewLine + line;
          }
        }

        // Adicionamos o último registro ao sair do loop
        if (previousContent != null)
        {
          logRecords.Add(new LogRecord
          {
            Date = DateTime.UtcNow, // Definimos uma data padrão para o último registro
            Content = previousContent // Removemos espaços em branco extras
          });
        }
      }
    }
    catch (Exception ex)
    {
      // Log do erro
      Console.WriteLine($"Erro ao ler o arquivo de log: {ex.Message}");
      throw; // Propagar a exceção para que o caller saiba que ocorreu um erro
    }

    return logRecords;
  }


  public DataTable GetDados()
  {
    DataTable dt = new DataTable();

    dt.TableName = "LogEntrys";

    dt.Columns.Add("Date", typeof(DateTime));
    dt.Columns.Add("Content", typeof(string));

    var dados = _context.LogEntrys.ToList();

    if (dados.Count > 0)
    {
      dados.ForEach(d => dt.Rows.Add(d.Date, d.Content));
    }

    return dt;
  }

}


public class CustomLogFilter : ILogEventFilter
{
  public bool IsEnabled(LogEvent logEvent)
  {
    return logEvent.MessageTemplate.Text.Contains("Request starting") ||
           logEvent.MessageTemplate.Text.Contains("Executing endpoint") ||
           logEvent.MessageTemplate.Text.Contains("Executed DbCommand") ||
           logEvent.MessageTemplate.Text.Contains("Request finished");
  }
}


