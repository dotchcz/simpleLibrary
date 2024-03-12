namespace SimpleLibrary.Common.Dtos;

public class LoanDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; }
    public string UserName { get; set; }
    public DateTimeOffset LoanDate { get; set; }
}