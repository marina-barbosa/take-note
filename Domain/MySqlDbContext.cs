using Microsoft.EntityFrameworkCore;
using take_note.Domain.Models;

namespace take_note.Domain;

public class MySqlDbContext : DbContext
{
  public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
      : base(options) { }
  public DbSet<Note> Notes { get; set; }
}