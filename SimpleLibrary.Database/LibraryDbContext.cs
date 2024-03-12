using Microsoft.EntityFrameworkCore;
using SimpleLibrary.Database.Models;

namespace SimpleLibrary.Database;

public class LibraryDbContext : DbContext
{
    public string DbPath { get; }
    
    public LibraryDbContext()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
        var parent = directoryInfo.Parent;
        var parentDirectory = parent!.FullName;
        DbPath = Path.Join(parentDirectory, "library.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    
    
    public virtual DbSet<Book> Books { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Loan> Loans { get; set; }
    public virtual DbSet<Favorite> Favorites { get; set; }
}
