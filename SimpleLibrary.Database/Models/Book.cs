namespace SimpleLibrary.Database.Models;

public class Book: IId
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public int MaxLoanDays { get; set; }

    public bool Deleted { get; set; }
}