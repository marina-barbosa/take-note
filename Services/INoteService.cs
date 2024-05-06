using take_note.Domain.Models;

namespace take_note.Services;

public interface INoteService
{
  Task<IEnumerable<Note>> GetAllNotesAsync(int pageNumber, int pageSize);
  Task<Note?> GetNoteByIdAsync(int id);
  Task<Note> CreateNoteAsync(Note note);
  Task UpdateNoteAsync(Note note);
  Task DeleteNoteAsync(int id);
}