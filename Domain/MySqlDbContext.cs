using Microsoft.EntityFrameworkCore;

namespace take_note.Domain;

public class MySqlDbContext : DbContext
{
  public MySqlDbContext(DbContextOptions<MySqlDbContext> options)
      : base(options)
  {
  }
}