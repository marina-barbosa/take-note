using System.ComponentModel.DataAnnotations;
namespace take_note.Domain.Models;

public class Log
{
  [Key]
  public int Id { get; set; }
  [Required]
  public string RequestStarting { get; set; } = string.Empty;
  public string ExecutingEndpoint { get; set; } = string.Empty;
  public string RequestFinished { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
  public ICollection<ExecutedDbCommand>? ExecutedDbCommands { get; set; }
}