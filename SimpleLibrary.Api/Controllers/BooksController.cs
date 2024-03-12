using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Api.Identity;
using SimpleLibrary.Common.Abstraction;
using SimpleLibrary.Common.Requests;

namespace SimpleLibrary.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;

    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetBooks([FromQuery] string? title, [FromQuery] string? author)
    {
        var books = await _booksService.GetAll(title, author);
        return Ok(books);
    }
    
    [Authorize(Policy = IdentityConstants.AdminUserPolicyName)]
    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] CreateBookRequest request)
    {
        var book = await _booksService.Add(request);
        return Ok(book);
    }
    
    [Authorize(Policy = IdentityConstants.AdminUserPolicyName)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> EditBook([FromRoute] int id, [FromBody] EditBookRequest request)
    {
        var book = await _booksService.Edit(id, request);
        return Ok(book);
    }

    [Authorize(Policy = IdentityConstants.AdminUserPolicyName)]
    [HttpDelete( "{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _booksService.Delete(id);
        return NoContent();
    }
}