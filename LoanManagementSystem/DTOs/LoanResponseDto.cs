namespace LoanManagementSystem.DTOs;

public class LoanApplicationDto
{
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
}