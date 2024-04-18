using Microsoft.EntityFrameworkCore;
using take_note.Domain;
using take_note.Domain.Models;
using take_note.Services;

public class NoteService : INoteService
{
  private readonly MySqlDbContext _context;

  public NoteService(MySqlDbContext context)
  {
    _context = context;
  }

  public async Task<IEnumerable<Note>> GetAllNotesAsync()
  {
    return await _context.Notes.ToListAsync();
  }

  public async Task<Note?> GetNoteByIdAsync(int id)
  {
    return await _context.Notes.FindAsync(id);
  }

  public async Task<Note> CreateNoteAsync(Note note)
  {
    note.CreatedAt = DateTime.UtcNow; // Set creation time to UTC
    await _context.Notes.AddAsync(note);
    await _context.SaveChangesAsync();
    return note;
  }

  public async Task UpdateNoteAsync(Note note)
  {
    note.UpdatedAt = DateTime.UtcNow; // Set update time to UTC
    _context.Notes.Update(note);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteNoteAsync(int id)
  {
    var note = await GetNoteByIdAsync(id);
    if (note != null)
    {
      _context.Notes.Remove(note);
      await _context.SaveChangesAsync();
    }
  }
}
