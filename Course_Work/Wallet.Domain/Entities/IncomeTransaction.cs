using System.Text.Json.Serialization;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;
public sealed class IncomeTransaction : BaseTransaction
{
    public override TransactionType Type => TransactionType.Income;

    public IncomeTransaction(
        Guid accountId,
        decimal amount,
        DateTime occurredOn,
        Guid categoryId,
        string? note = null)
        : base(Guid.NewGuid(), accountId, amount, occurredOn, categoryId, note)
    {
    }

    [JsonConstructor]
    public IncomeTransaction(
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
            throw new ValidationException("Категорія обов'язкова для доходу.");
        }

        if (Amount <= 0)
        {
            throw new ValidationException("Для доходу сума повинна бути додатною.");
        }
    }
    public override bool IsIncome()
    {
        return true;
    }
    public override string GetDisplayName()
    {
        return $"Дохід на суму {GetAbsoluteAmount():F2} ({OccurredOn:yyyy-MM-dd})";
    }
}

