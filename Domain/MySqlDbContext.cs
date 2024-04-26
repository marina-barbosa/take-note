using Microsoft.EntityFrameworkCore;
using take_note.Domain.Models;

namespace take_note.Domain;

public class MySqlDbContext : DbContext
{
  public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
      : base(options) { }
  public DbSet<Note> Notes { get; set; }
  public DbSet<Log> Logs { get; set; }
  public DbSet<LogEntry> LogEntrys { get; set; }
  public DbSet<ExecutedDbCommand> ExecutedDbCommands { get; set; }
}