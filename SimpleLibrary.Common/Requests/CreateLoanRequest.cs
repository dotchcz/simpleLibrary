namespace SimpleLibrary.Common.Requests;

public class CreateLoanRequest: LoanRequestBase
{
    public DateTime LoanDate { get; set; }
}