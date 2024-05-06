namespace take_note.Domain.Models;

public class LogRecord
{
  public int Id { get; set; }
  public DateTime Date { get; set; }
  public string Content { get; set; } = string.Empty;
}
