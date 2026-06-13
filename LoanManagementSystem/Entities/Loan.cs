namespace LoanManagementSystem.Entities;

public enum LoanStatus { Pending, Approved, Rejected, Closed, Overdue }

public class Loan
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public decimal MonthlyPayment { get; set; }
    public LoanStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}