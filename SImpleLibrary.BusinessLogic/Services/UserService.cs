using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleLibrary.Common.Abstraction;
using SimpleLibrary.Common.Dtos;
using SimpleLibrary.Common.Requests;
using SimpleLibrary.Database;
using SimpleLibrary.Database.Models;

namespace SimpleLibrary.BusinessLogic.Services;

public class UserService: IUserService
{
    private readonly LibraryDbContext _libraryDbContext;
    private readonly IMapper _mapper;

    public UserService(LibraryDbContext libraryDbContext, IMapper mapper)
    {
        _libraryDbContext = libraryDbContext;
        _mapper = mapper;
    }

    public async Task<UserDto> Create(CreateUserRequest request)
    {
        var entity = _mapper.Map<User>(request);
        await _libraryDbContext.Users.AddAsync(entity);
        await _libraryDbContext.SaveChangesAsync();

        return _mapper.Map<UserDto>(entity);
    }

    public async Task<UserDto> GetByEmail(string email)
    {
        var user = await _libraryDbContext.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user is null)
        {
            throw new ArgumentException("User not found");
        }
        return _mapper.Map<UserDto>(user);
    }

    public async Task<ICollection<UserDto>> GetAll()
    {
        var users = await _libraryDbContext.Users.OrderBy(u => u.Id).ToListAsync();
        return _mapper.Map<List<UserDto>>(users);
    }
}