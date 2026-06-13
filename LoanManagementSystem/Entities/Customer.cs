namespace LoanManagementSystem.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PersonalNumber { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public int CreditScore { get; set; }
}