using System.ComponentModel.DataAnnotations;
namespace take_note.Domain.Models;

public class ExecutedDbCommand
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string DbCommand { get; set; } = string.Empty;
    public int LogId { get; set; }
    public virtual Log? Log { get; set; }
    public DateTime CreatedAt { get; set; }
}