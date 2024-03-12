namespace SimpleLibrary.Common.Requests;

public class BookRequestBase
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int PublicationYear { get; set; }
    public int MaxLoanDays { get; set; }
}