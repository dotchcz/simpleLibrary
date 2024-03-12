using SimpleLibrary.Common.Dtos;
using SimpleLibrary.Common.Requests;

namespace SimpleLibrary.Common.Abstraction;

public interface IUserService
{
    Task<UserDto> Create(CreateUserRequest request);
    Task<UserDto> GetByEmail(string email);
    Task<ICollection<UserDto>> GetAll();
}