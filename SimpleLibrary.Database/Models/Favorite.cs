namespace SimpleLibrary.Database.Models;

public class Favorite: IId
{
    public int UserId { get; set; }
    public User User { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
}