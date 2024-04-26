using System.ComponentModel.DataAnnotations;
namespace take_note.Domain.Models;

public class LogEntry
{
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime Date { get; set; }
    public string Content { get; set; } = string.Empty;
}
