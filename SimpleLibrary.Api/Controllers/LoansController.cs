using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLibrary.Common.Abstraction;
using SimpleLibrary.Common.Requests;

namespace SimpleLibrary.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoansController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateLoan([FromBody] CreateLoanRequest request)
    {
        await _loanService.CreateLoan(request);
        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> CancelLoan([FromBody] CancelLoanRequest request)
    {
        await _loanService.CancelLoan(request);
        return NoContent();
    }
}