using AutoMapper;
using SimpleLibrary.Common.Dtos;
using SimpleLibrary.Common.Requests;
using SimpleLibrary.Database.Models;

namespace SimpleLibrary.BusinessLogic.Mapper;

public class SimpleLibraryProfileMapper : Profile
{
    public SimpleLibraryProfileMapper()
    {
        // Book 
        CreateMap<Book, BookDto>()
            .ForMember(d => d.IsAvailable, opt => opt.Ignore())
            .ForMember(d => d.AvailableFrom, opt => opt.Ignore())
            ;

        CreateMap<CreateBookRequest, Book>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Deleted, opt => opt.Ignore())
            ;

        CreateMap<EditBookRequest, Book>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Deleted, opt => opt.Ignore())
            ;

        // Loan
        CreateMap<Loan, LoanDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(f => f.User.Username))
            .ForMember(d => d.BookTitle, opt => opt.MapFrom(f => f.Book.Title))
            ;

        CreateMap<CreateLoanRequest, Loan>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Book, opt => opt.Ignore())
            .ForMember(d => d.User, opt => opt.Ignore())
            .ForMember(d => d.ReturnDate, opt => opt.Ignore())
            ;

        CreateMap<CancelLoanRequest, Loan>()
            .ForMember(d => d.Book, opt => opt.Ignore())
            .ForMember(d => d.User, opt => opt.Ignore())
            .ForMember(d => d.ReturnDate, opt => opt.Ignore())
            .ForMember(d => d.LoanDate, opt => opt.Ignore())
            .ForMember(d => d.Id, opt => opt.MapFrom(f => f.LoanId))
            ;

        CreateMap<CreateUserRequest, User>()
            .ForMember(d => d.Id, opt => opt.Ignore());

        CreateMap<User, UserDto>().ReverseMap();
    }
}