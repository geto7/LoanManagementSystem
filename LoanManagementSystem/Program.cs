using LoanManagementSystem.Data;
using LoanManagementSystem.Entities;
using LoanManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ბაზის და სერვისის რეგისტრაცია (DI)
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("LoanDb"));
builder.Services.AddScoped<LoanService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---- სატესტო მომხმარებლის ჩამატება (Seed Data) ----
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // თუ ბაზაში კლიენტები საერთოდ არ გვყავს, ჩავამატოთ ერთი სატესტო
    if (!context.Customers.Any())
    {
        context.Customers.Add(new Customer
        {
            Id = 1, // ამ ID-ის გამოვიყენებთ სესხის მოთხოვნისას
            FirstName = "Irakli",
            LastName = "Kakabadze",
            PersonalNumber = "01020304050",
            BirthDate = new DateTime(2000, 05, 15), // 26 წლისაა, ანუ 18+ წესს აკმაყოფილებს
            CreditScore = 450 // 300-ზე მეტია, ანუ სესხი დაუმტკიცდება
        });
        context.SaveChanges();
    }
}
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();