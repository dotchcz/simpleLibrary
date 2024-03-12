using SimpleLibrary.Common.Dtos;
using SimpleLibrary.Common.Requests;

namespace SimpleLibrary.Common.Abstraction;

public interface IBooksService
{
    Task<ICollection<BookDto>> GetAll(string? title, string? author);

    Task<BookDto> Add(CreateBookRequest createBookRequest);
    Task<BookDto> Edit(int id, EditBookRequest bookToEdit);
    Task Delete(int bookId);

    Task AddToFavorites(int bookId, int userId);
    Task RemoveFavorites(int bookId, int userId);
}