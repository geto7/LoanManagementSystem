using LoanManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<LoanSchedule> LoanSchedules { get; set; }
    public DbSet<Payment> Payments { get; set; }
}