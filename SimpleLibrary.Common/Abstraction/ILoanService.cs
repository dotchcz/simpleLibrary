using SimpleLibrary.Common.Requests;

namespace SimpleLibrary.Common.Abstraction;

public interface ILoanService
{
    Task CreateLoan(CreateLoanRequest request);
    Task CancelLoan(CancelLoanRequest request);
}