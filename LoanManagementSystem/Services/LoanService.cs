using LoanManagementSystem.Data;
using LoanManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanManagementSystem.Services;

public class LoanService
{
    private readonly AppDbContext _context;

    // კონსტრუქტორი - Dependency Injection-ით შემოგვაქვს ბაზის კონტექსტი
    public LoanService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// სესხის განაცხადის შექმნა, ვალიდაციები, კრედიტ სკორის შემოწმება და გრაფიკის დაგენერირება
    /// </summary>
    public async Task<Loan> CreateLoanApplicationAsync(int customerId, decimal amount, decimal interestRate, int termMonths)
    {
        // 1. ვალიდაცია: არსებობს თუ არა მომხმარებელი ბაზაში
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
            throw new Exception("მომხმარებელი ვერ მოიძებნა!");

        // 2. ვალიდაცია: მომხმარებლის ასაკი უნდა იყოს მინიმუმ 18 წელი
        int age = DateTime.Today.Year - customer.BirthDate.Year;
        if (customer.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;
        if (age < 18)
            throw new Exception("სესხის ასაღებად მომხმარებელი უნდა იყოს მინიმუმ 18 წლის!");

        // 3. ვალიდაცია: თანხა უნდა იყოს 500₾-დან 50,000₾-მდე
        if (amount < 500 || amount > 50000)
            throw new Exception("თანხა უნდა იყოს 500₾-დან 50,000₾-მდე!");

        // 4. ვალიდაცია: ვადა უნდა იყოს 6-დან 60 თვემდე
        if (termMonths < 6 || termMonths > 60)
            throw new Exception("სესხის ვადა უნდა იყოს 6-დან 60 თვემდე!");

        // 5. ბიზნეს ლოგიკა: კრედიტ სკორის მიხედვით სტატუსის განსაზღვრა
        LoanStatus status = customer.CreditScore < 300 ? LoanStatus.Rejected : LoanStatus.Approved;

        // 6. ყოველთვიური გადასახადის (PMT) გამოთვლა ფინანსური ფორმულით
        decimal monthlyRate = (interestRate / 100) / 12;
        decimal monthlyPayment = 0;

        if (status == LoanStatus.Approved)
        {
            if (monthlyRate > 0)
            {
                double pow = Math.Pow((double)(1 + monthlyRate), -termMonths);
                monthlyPayment = amount * (monthlyRate / (decimal)(1 - pow));
            }
            else
            {
                monthlyPayment = amount / termMonths;
            }
        }

        // 7. სესხის ობიექტის შექმნა და ბაზაში შენახვა
        var loan = new Loan
        {
            CustomerId = customerId,
            Amount = amount,
            InterestRate = interestRate,
            TermMonths = termMonths,
            MonthlyPayment = Math.Round(monthlyPayment, 2),
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync(); // აქ იქმნება სესხი და ენიჭება უნიკალური Id

        // 8. გრაფიკის (LoanSchedule) ავტომატური დაგენერირება და ბაზაში შენახვა (მხოლოდ დამტკიცებულ სესხზე)
        if (status == LoanStatus.Approved)
        {
            for (int i = 1; i <= termMonths; i++)
            {
                var schedule = new LoanSchedule
                {
                    LoanId = loan.Id,
                    PMT = Math.Round(monthlyPayment, 2),
                    Date = DateTime.UtcNow.AddMonths(i)
                };
                _context.LoanSchedules.Add(schedule);
            }
            await _context.SaveChangesAsync(); // ვინახავთ გრაფიკს ბაზაში
        }

        return loan;
    }

    /// <summary>
    /// გადახდის განხორციელება და შენახვა Payments ცხრილში
    /// </summary>
    public async Task ProcessPaymentAsync(int loanId, decimal amount)
    {
        // 1. ვალიდაცია: გადახდის თანხა უნდა იყოს 0-ზე მეტი
        if (amount <= 0)
            throw new Exception("გადახდის თანხა უნდა იყოს 0-ზე მეტი!");

        // 2. ვალიდაცია: სესხი უნდა არსებობდეს
        var loan = await _context.Loans.FindAsync(loanId);
        if (loan == null)
            throw new Exception("სესხი ვერ მოიძებნა!");

        // 3. ვალიდაცია: დახურულ ან უარყოფილ სესხზე გადახდა არ უნდა განხორციელდეს
        if (loan.Status == LoanStatus.Closed)
            throw new Exception("დახურულ სესხზე გადახდა ვერ განხორციელდება!");
        if (loan.Status == LoanStatus.Rejected)
            throw new Exception("უარყოფილ სესხზე გადახდა ვერ განხორციელდება!");

        // 4. გადახდის რეგისტრაცია და შენახვა
        var payment = new Payment
        {
            LoanId = loanId,
            Amount = amount,
            PaymentDate = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// მომხმარებლის სესხების ისტორიის წამოღება
    /// </summary>
    public async Task<IEnumerable<Loan>> GetCustomerLoansAsync(int customerId)
    {
        return await _context.Loans
            .Where(l => l.CustomerId == customerId)
            .ToListAsync();
    }
}