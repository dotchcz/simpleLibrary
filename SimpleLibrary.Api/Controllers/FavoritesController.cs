using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Common.Abstraction;

namespace SimpleLibrary.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly IBooksService _booksService;

    public FavoritesController(IBooksService booksService)
    {
        _booksService = booksService;
    }
    
    [HttpPost("{bookId:int}/{userId:int}")]
    public async Task<IActionResult> AddFavorites([FromRoute] int bookId, [FromRoute] int userId)
    {
        await _booksService.AddToFavorites(bookId, userId);
        return NoContent();
    }
    
    [HttpDelete("{bookId:int}/{userId:int}")]
    public async Task<IActionResult> RemoveFavorites([FromRoute] int bookId, [FromRoute] int userId)
    {
        await _booksService.RemoveFavorites(bookId, userId);
        return NoContent();
    }
}