namespace SimpleLibrary.Database.Models;

public class Loan: IId
{
    public int BookId { get; set; }
    public Book Book { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}