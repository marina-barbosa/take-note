using System.Threading.Tasks.Dataflow;
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

  public NoteController(INoteService noteService, MySqlDbContext context)
  {
    _noteService = noteService;
    _context = context;
  }

  [HttpGet("Hello")]
  public IActionResult HelloNotes()
  {
    var msg = "Hello Notes, tudo Ok <====";



    return Ok(msg);
  }

  [HttpGet]
  public async Task<IActionResult> GetNotes()
  {
    var notes = await _noteService.GetAllNotesAsync();
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