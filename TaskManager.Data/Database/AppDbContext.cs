using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;

namespace TaskManager.Data.Database;

public class AppDbContext : DbContext
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<AppSettings> Settings => Set<AppSettings>();

    public AppDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=tasks.db");
    }
}