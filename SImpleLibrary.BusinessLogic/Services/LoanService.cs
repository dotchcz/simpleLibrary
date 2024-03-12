using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleLibrary.Common.Abstraction;
using SimpleLibrary.Common.Requests;
using SimpleLibrary.Database;
using SimpleLibrary.Database.Models;

namespace SimpleLibrary.BusinessLogic.Services;

public class LoanService(LibraryDbContext libraryDbContext, IMapper mapper) : ILoanService
{
    public async Task CreateLoan(CreateLoanRequest request)
    {
        var loan = await libraryDbContext.Loans.FirstOrDefaultAsync(l =>
            l.BookId == request.BookId &&
            l.ReturnDate == null);
        
        if (loan is not null && loan.UserId != request.UserId) // the book is borrowed by someone else
            throw new ArgumentException("The book is borrowed by someone else");
        
        if (loan is not null && loan.UserId == request.UserId) // the book is borrowed by the user
            return;

        // new loan
        var newLoan = mapper.Map<Loan>(request);
        await libraryDbContext.Loans.AddAsync(newLoan);
        await libraryDbContext.SaveChangesAsync();
    }

    public async Task CancelLoan(CancelLoanRequest request)
    {
        var loan = await libraryDbContext.Loans.FirstOrDefaultAsync(l =>
            l.BookId == request.BookId &&
            l.UserId == request.UserId &&
            l.UserId == request.LoanId &&
            l.ReturnDate == null);
        
        if (loan is null)
            throw new ArgumentException("Cannot cancel the loan");

        loan.ReturnDate = DateTime.UtcNow;
        await libraryDbContext.SaveChangesAsync();
    }

}