namespace SimpleLibrary.Common.Requests;

public class CancelLoanRequest: LoanRequestBase
{
    public int LoanId { get; set; }
}