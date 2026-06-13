namespace LoanManagementSystem.Entities;

public class LoanSchedule
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public decimal PMT { get; set; } 
    public DateTime Date { get; set; } 
}