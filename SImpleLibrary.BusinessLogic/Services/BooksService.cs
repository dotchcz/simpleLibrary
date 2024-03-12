using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleLibrary.Common.Abstraction;
using SimpleLibrary.Common.Dtos;
using SimpleLibrary.Common.Requests;
using SimpleLibrary.Database;
using SimpleLibrary.Database.Models;

namespace SimpleLibrary.BusinessLogic.Services;

public class BooksService : IBooksService
{
    private readonly LibraryDbContext _libraryDbContext;
    private readonly IMapper _mapper;

    public BooksService(LibraryDbContext libraryDbContext, IMapper mapper)
    {
        _libraryDbContext = libraryDbContext;
        _mapper = mapper;
    }

    public async Task<ICollection<BookDto>> GetAll(string? title, string? author)
    {
        var filter = ApplyQueryFilter(title, author, b => b.Deleted == false);
        var notDeletedBooks = await _libraryDbContext.Books
            .Where(filter)
            .OrderBy(b => b.Id).ToListAsync();

        var books = notDeletedBooks
            .GroupJoin(
                _libraryDbContext.Loans,
                b => b.Id, l => l.BookId, (b, l) => new {Book = b, Loan = l.MaxBy(loan => loan.ReturnDate)})
            .Select(x => new
                BookDto()
                {
                    Id=x.Book.Id,
                    Author = x.Book.Author,
                    Title = x.Book.Title,
                    AvailableFrom =
                        x.Loan == null || x.Loan.ReturnDate != null && x.Loan.ReturnDate.Value < DateTime.UtcNow.Date
                            ? DateTime.UtcNow.Date
                            : x.Loan.LoanDate.AddDays(x.Book.MaxLoanDays),
                    IsAvailable = x.Loan == null ||
                                  x.Loan.ReturnDate != null && x.Loan.ReturnDate.Value < DateTime.UtcNow.Date,
                    PublicationYear = x.Book.PublicationYear
                })
            .ToList();

        return books;
    }

    public async Task<BookDto> Add(CreateBookRequest createBookRequest)
    {
        var newEntity = _mapper.Map<Book>(createBookRequest);

        await _libraryDbContext.Books.AddAsync(newEntity);
        await _libraryDbContext.SaveChangesAsync();
        return _mapper.Map<BookDto>(newEntity);
    }

    public async Task<BookDto> Edit(int id, EditBookRequest bookToEdit)
    {
        // ReSharper disable once RedundantAssignment
        var entity = await CheckAndGet(id);
        entity = _mapper.Map<Book>(bookToEdit);
        await _libraryDbContext.SaveChangesAsync();
        return _mapper.Map<BookDto>(entity);
    }

    public async Task Delete(int bookId)
    {
        var entity = await CheckAndGet(bookId);

        entity!.Deleted = true;

        await _libraryDbContext.SaveChangesAsync();
    }

    public async Task AddToFavorites(int bookId, int userId)
    {
        var favorite =
            await _libraryDbContext.Favorites.FirstOrDefaultAsync(f => f.BookId == bookId && f.UserId == userId);

        if (favorite is not null)
            return;

        var newFavorite = new Favorite()
        {
            BookId = bookId,
            UserId = userId
        };
        await _libraryDbContext.Favorites.AddAsync(newFavorite);
        await _libraryDbContext.SaveChangesAsync();
    }

    // Not implemented, just mentioned I'm aware :)
    public Task RemoveFavorites(int bookId, int userId)
    {
        throw new NotImplementedException();
    }

    private async Task<Book?> GetBook(int id)
    {
        return await _libraryDbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
    }

    private async Task<Book?> CheckAndGet(int id)
    {
        var entity = await GetBook(id);

        if (entity is null)
            throw new ArgumentException($"The book with Id {id} was not found");

        return entity;
    }

    private Expression<Func<Book, bool>> ApplyQueryFilter(string? title, string? author,
        Expression<Func<Book, bool>> deleteFilter)
    {
        var filters = new List<Expression<Func<Book, bool>>>() {deleteFilter};
        Expression<Func<Book, bool>> propertyFilter;

        if (!string.IsNullOrEmpty(title))
        {
            propertyFilter = b => b.Title.ToLower().Contains(title.ToLower());
            filters.Add(propertyFilter);
        }

        if (!string.IsNullOrEmpty(author))
        {
            propertyFilter = b => b.Author.ToLower().Contains(author.ToLower());
            filters.Add(propertyFilter);
        }

        var parameter = Expression.Parameter(typeof(Book), "b");
        Expression combinedBody = Expression.Invoke(filters[0], parameter);

        for (int i = 1; i < filters.Count; i++)
        {
            combinedBody = Expression.AndAlso(combinedBody, Expression.Invoke(filters[i], parameter));
        }

        return Expression.Lambda<Func<Book, bool>>(combinedBody, parameter);
    }
}