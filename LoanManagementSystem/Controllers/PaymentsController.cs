using LoanManagementSystem.DTOs;
using LoanManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly LoanService _loanService;

    public PaymentsController(LoanService loanService)
    {
        _loanService = loanService;
    }

    // 2. გადახდის განხორციელება
    [HttpPost]
    public async Task<IActionResult> MakePayment([FromBody] PaymentRequestDto dto)
    {
        try
        {
            await _loanService.ProcessPaymentAsync(dto.LoanId, dto.Amount);
            return Ok(new { Message = "გადახდა წარმატებით განხორციელდა." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}