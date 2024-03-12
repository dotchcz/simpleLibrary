using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.EntityFrameworkCore;
using SimpleLibrary.BusinessLogic.Services;
using SimpleLibrary.Common.Requests;
using SimpleLibrary.Database;
using SimpleLibrary.Database.Models;
using Xunit;

namespace SimpleLibrary.BusinessLogic.Tests;

public class BookServiceTest(Fixture fixture) : IClassFixture<Fixture>
{
    private readonly IServiceProvider _serviceProvider = fixture.ServiceProvider;

    [Theory]
    [InlineData("Title", "Author", 2)]
    [InlineData(null, "Author", 2)]
    [InlineData("Title", null, 2)]
    [InlineData("TitleDoesntExist", null, 0)]
    [InlineData(null, "AuthorDoesntExist", 0)]
    public async void GetAllBooksTest_SearchValidation(string? title, string? author, int expectedCount)
    {
        // Arrange
        var dbContextMock = new Mock<LibraryDbContext>();
        dbContextMock.Setup(b => b.Books)
            .ReturnsDbSet(new List<Book>
            {
                new()
                {
                    Id = 1, Author = "Author1", Deleted = false, Title = "Title1", PublicationYear = 1234,
                    MaxLoanDays = 14
                },
                new()
                {
                    Id = 2, Author = "Author2", Deleted = false, Title = "Title2", PublicationYear = 1233,
                    MaxLoanDays = 41
                }
            });
        dbContextMock.Setup(b => b.Loans).ReturnsDbSet(new List<Loan>());

        var bookService = new BooksService(dbContextMock.Object, _serviceProvider.GetRequiredService<IMapper>());

        // Act
        var result = await bookService.GetAll(title, author);

        // Assert
        Assert.Multiple(() => { result.Should().HaveCount(expectedCount); });
    }

    [Fact]
    public async void GetAllBooksTest_CheckAvailability()
    {
        // Arrange
        var dbContextMock = new Mock<LibraryDbContext>();
        dbContextMock.Setup(b => b.Books)
            .ReturnsDbSet(new List<Book>
            {
                new()
                {
                    Id = 1, Author = "Author1", Deleted = false, Title = "Title1", PublicationYear = 2024,
                    MaxLoanDays = 5
                },
                new()
                {
                    Id = 2, Author = "Author2", Deleted = false, Title = "Title2", PublicationYear = 1999,
                    MaxLoanDays = 5
                },
                new()
                {
                    Id = 3, Author = "Author3", Deleted = false, Title = "Title3", PublicationYear = 2004,
                    MaxLoanDays = 5
                },
                new()
                {
                    Id = 4, Author = "Author4", Deleted = false, Title = "Title4", PublicationYear = 2004,
                    MaxLoanDays = 5
                },
                new()
                {
                    Id = 5, Author = "Author5", Deleted = false, Title = "Title5", PublicationYear = 2004,
                    MaxLoanDays = 5
                }
            });
        dbContextMock.Setup(b => b.Loans).ReturnsDbSet(new List<Loan>()
        {
            new() {Id = 1, BookId = 1, LoanDate = DateTime.UtcNow.Date}, // should be available
            new()
            {
                Id = 2, BookId = 2, LoanDate = DateTime.UtcNow.Date, ReturnDate = DateTime.UtcNow.AddDays(4).Date
            }, // return date after today
            new()
            {
                Id = 3, BookId = 2, LoanDate = DateTime.UtcNow.AddDays(-10).Date,
                ReturnDate = DateTime.UtcNow.AddDays(-5).Date
            }, // loan in the past
            new()
            {
                Id = 4, BookId = 3, LoanDate = DateTime.UtcNow.Date, ReturnDate = DateTime.UtcNow.AddDays(10).Date
            }, //return date after today
            new()
            {
                Id = 5, BookId = 4, LoanDate = DateTime.UtcNow.AddDays(-10).Date,
                ReturnDate = DateTime.UtcNow.AddDays(-1).Date
            }, // return in the past
        });

        var bookService = new BooksService(dbContextMock.Object, _serviceProvider.GetRequiredService<IMapper>());

        // Act
        var result = await bookService.GetAll(String.Empty, String.Empty);

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().HaveCount(5);
            result.ToArray()[0].IsAvailable.Should().BeFalse();
            result.ToArray()[1].IsAvailable.Should().BeFalse();
            result.ToArray()[2].IsAvailable.Should().BeFalse();
            result.ToArray()[3].IsAvailable.Should().BeTrue();
            result.ToArray()[4].IsAvailable.Should().BeTrue();
        });
    }

    [Theory]
    [InlineData(1, false, null)]
    [InlineData(2, true, typeof(ArgumentException))]
    public async void EditBooksTest(int id, bool riseException, Type? exceptionType)
    {
        // Arrange
        var dbContextMock = new Mock<LibraryDbContext>();
        dbContextMock.Setup(b => b.Books)
            .ReturnsDbSet(new List<Book>
            {
                new()
                {
                    Id = 1, Author = "Author1", Deleted = false, Title = "Title1", PublicationYear = 2024,
                    MaxLoanDays = 5
                }
            });
        dbContextMock.Setup(b => b.Loans).ReturnsDbSet(new List<Loan>());

        var bookService = new BooksService(dbContextMock.Object, _serviceProvider.GetRequiredService<IMapper>());

        // Act
        var record = await Record.ExceptionAsync(async () => await bookService.Edit(id, new EditBookRequest()
        {
            Author = "NewAuthor",
            PublicationYear = 2000,
            MaxLoanDays = 10,
            Title = "NewTitle"
        }));
        
        // Assert
        if (riseException)
        {
            Assert.IsType(exceptionType!, record);
        }
        else
        {
            var editedEntity = await bookService.GetAll(string.Empty, string.Empty);
            dbContextMock.Verify(m => m.SaveChangesAsync(CancellationToken.None), Times.Once());
            Assert.Multiple(() => { editedEntity.Should().HaveCount(1); });    
        }
    }

    [Fact]
    public async void CreateBookTest()
    {
        // Arrange
        var dbContextMock = new Mock<LibraryDbContext>();
        dbContextMock.Setup(b => b.Books)
            .ReturnsDbSet(new List<Book> {new Book() {Id = 1, Author = "", Deleted = false}});

        var bookService = new BooksService(dbContextMock.Object, _serviceProvider.GetRequiredService<IMapper>());

        // Act
        var record = await Record.ExceptionAsync(async () => await bookService.Add(new CreateBookRequest()
            {Author = "TestAuthor", Title = "TestTitle", PublicationYear = 2024, MaxLoanDays = 90}));


        // Assert
        record.Should().BeNull();
    }

    [Theory]
    [InlineData(1, false, null)]
    [InlineData(2, true, typeof(ArgumentException))]
    public async void DeleteTest_Success(int bookId, bool riseException, Type? exceptionType)
    {
        // Arrange
        var dbContextMock = new Mock<LibraryDbContext>();
        dbContextMock.Setup(b => b.Books)
            .ReturnsDbSet(new List<Book>
                {new() {Id = 1, Author = "TestAuthor", Deleted = false, Title = "TestTitle", MaxLoanDays = 168}});

        var bookService = new BooksService(dbContextMock.Object, _serviceProvider.GetRequiredService<IMapper>());

        // Act
        var record = await Record.ExceptionAsync(async () => await bookService.Delete(bookId));


        // Assert
        if (riseException)
        {
            Assert.NotNull(record);
            Assert.IsType(exceptionType!, record);
        }
        else
        {
            Assert.Null(record);
        }
    }
}