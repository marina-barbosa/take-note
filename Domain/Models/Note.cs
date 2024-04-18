using System.ComponentModel.DataAnnotations;
namespace take_note.Domain.Models;

public class Note
{
  [Key]
  public int Id { get; set; }
  [Required]
  public string Title { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; }
  public DateTime UpdatedAt { get; set; }
}