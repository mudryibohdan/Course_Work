using System.Text.Json.Serialization;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;
public sealed class ExpenseTransaction : BaseTransaction
{
    public override TransactionType Type => TransactionType.Expense;

    public ExpenseTransaction(
        Guid accountId,
        decimal amount,
        DateTime occurredOn,
        Guid categoryId,
        string? note = null)
        : base(Guid.NewGuid(), accountId, -Math.Abs(amount), occurredOn, categoryId, note)
    {
    }

    [JsonConstructor]
    public ExpenseTransaction(
        Guid id,
        Guid accountId,
        decimal amount,
        DateTime occurredOn,
        Guid categoryId,
        string? note = null)
        : base(id, accountId, amount, occurredOn, categoryId, note)
    {
    }
    protected override void ValidateSpecific()
    {
        if (CategoryId is null || CategoryId == Guid.Empty)
        {
            throw new ValidationException("Категорія обов'язкова для витрати.");
        }

        if (Amount >= 0)
        {
            throw new ValidationException("Для витрати сума повинна бути від'ємною.");
        }
    }
    public override bool IsIncome()
    {
        return false;
    }
    public override string GetDisplayName()
    {
        return $"Витрата на суму {GetAbsoluteAmount():F2} ({OccurredOn:yyyy-MM-dd})";
    }
}

