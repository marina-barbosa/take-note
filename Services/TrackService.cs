
using System.Data;
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
  //private readonly string arquivoSerilog = "serilogs/log20240424.txt";
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
        _context.LogEntrys.Add(new LogEntry
        {
          Date = record.Date,
          Content = record.Content
        });
      }
      Console.WriteLine("FIM IMPORT LOG <<<<<<<<<<<<<<<<<<<<<<<");
      _context.SaveChanges();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Erro ao importar o arquivo de log: {ex.Message}");
      throw;
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
            if (previousContent != null)
            {
              logRecords.Add(new LogRecord
              {
                Date = date,
                Content = previousContent
              });
            }

            previousContent = line;
          }
          else
          {
            previousContent += Environment.NewLine + line;
          }
        }

        if (previousContent != null)
        {
          logRecords.Add(new LogRecord
          {
            Date = DateTime.UtcNow,
            Content = previousContent
          });
        }
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Erro ao ler o arquivo de log: {ex.Message}");
      throw;
    }
    Console.WriteLine("FIM READ LOG <<<<<<<<<<<<<<<<<<<<<<<");
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
    Console.WriteLine("FIM GET DADOS <<<<<<<<<<<<<<<<<<<<<<<");
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


