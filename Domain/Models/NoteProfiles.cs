using AutoMapper;
using take_note.Domain.Models;
using take_note.DTOs;

namespace take_note.Profiles
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<Note, NoteDTO>();
            CreateMap<NoteDTO, Note>();
        }
    }
}