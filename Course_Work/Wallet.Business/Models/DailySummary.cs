using Wallet.Domain.Entities;

namespace Wallet.Business.Models;

public sealed record DailySummary(DateOnly Date, decimal Income, decimal Expenses)
{
    public decimal Net => Income - Expenses;

    public static DailySummary FromTransactions(DateOnly date, IEnumerable<Transaction> transactions)
    {
        decimal income = 0m;
        decimal expenses = 0m;

        foreach (var transaction in transactions.Where(t => t.Type != TransactionType.Transfer))
        {
            if (transaction.Type == TransactionType.Income)
            {
                income += transaction.Amount;
            }
            else if (transaction.Type == TransactionType.Expense)
            {
                expenses += transaction.AbsoluteAmount;
            }
        }

        return new DailySummary(date, income, expenses);
    }
}

