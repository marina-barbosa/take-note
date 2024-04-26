using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using take_note.Domain.Models;
using take_note.Services;

namespace take_note.Domain;

[ApiController]
[Route("v1/note")]
public class NoteController : ControllerBase
{
  private readonly INoteService _noteService;
  private readonly MySqlDbContext _context;

  private readonly ITrackService _trackService;


  public NoteController(INoteService noteService, MySqlDbContext context, ITrackService trackService)
  {
    _noteService = noteService;
    _context = context;
    _trackService = trackService;
  }

  [HttpGet]
  public async Task<IActionResult> GetNotes()
  {
    var notes = await _noteService.GetAllNotesAsync();

    await _trackService.TrackDatabaseQueries($"Método: GET - GetAllNotesAsync()");

    foreach (var note in notes)
    {
      await _trackService.TrackDatabaseQueries($"Content: Título: {note.Title}, Conteúdo: {note.Content}");
    }

    return Ok(notes);
  }

  [HttpGet("{id}")]
  public async Task<IActionResult> GetNote(int id)
  {
    var note = await _noteService.GetNoteByIdAsync(id);
    if (note == null)
    {
      return NotFound();
    }
    return Ok(note);
  }

  [HttpPost]
  public async Task<IActionResult> CreateNote([FromBody] Note note)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var createdNote = await _noteService.CreateNoteAsync(note);
    return CreatedAtAction("GetNote", new { id = createdNote.Id }, createdNote);
  }

  [HttpPut("{id}")]
  public async Task<IActionResult> UpdateNote(int id, Note note)
  {
    if (id != note.Id)
    {
      return BadRequest();
    }

    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    try
    {
      await _noteService.UpdateNoteAsync(note);
    }
    catch (DbUpdateConcurrencyException)
    {
      if (!NoteExists(id))
      {
        return NotFound();
      }
      else
      {
        throw;
      }
    }

    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> DeleteNote(int id)
  {
    var note = await _noteService.GetNoteByIdAsync(id);
    if (note == null)
    {
      return NotFound();
    }

    await _noteService.DeleteNoteAsync(id);

    return NoContent();
  }

  private bool NoteExists(int id)
  {
    return _context.Notes.Any(e => e.Id == id);
  }
}