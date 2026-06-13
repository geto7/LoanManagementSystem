using LoanManagementSystem.DTOs;
using LoanManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoansController : ControllerBase
{
    private readonly LoanService _loanService;

    // Dependency Injection-ით შემოგვაქვს ჩვენი სერვისი
    public LoansController(LoanService loanService)
    {
        _loanService = loanService;
    }

    // 1. სესხის განაცხადის შექმნა
    [HttpPost("CreateApplication")]
    public async Task<IActionResult> CreateApplication([FromBody] LoanApplicationDto dto)
    {
        try
        {
            var loan = await _loanService.CreateLoanApplicationAsync(
                dto.CustomerId,
                dto.Amount,
                dto.InterestRate,
                dto.TermMonths
            );

            return StatusCode(201, loan); // 201 ნიშნავს "Created" (წარმატებით შეიქმნა)
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message }); // შეცდომის შემთხვევაში დააბრუნებს ტექსტს
        }
    }
}