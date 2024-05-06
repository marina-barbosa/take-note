using System;

namespace take_note.DTOs
{
    public class NoteDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
    }
}