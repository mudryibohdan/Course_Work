using Wallet.Domain.Abstractions;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;

public sealed class Transaction : IIdentifiable
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid AccountId { get; }
    public Guid? CounterpartyAccountId { get; }
    public Guid? CategoryId { get; }
    public Guid? TransferGroupId { get; }
    public TransactionType Type { get; }
    public decimal Amount { get; }
    public DateTime OccurredOn { get; }
    public string? Note { get; }

    public Transaction(
        Guid accountId,
        decimal amount,
        TransactionType type,
        DateTime occurredOn,
        Guid? categoryId = null,
        Guid? counterpartyAccountId = null,
        Guid? transferGroupId = null,
        string? note = null)
    {
        if (accountId == Guid.Empty)
        {
            throw new ValidationException("Ідентифікатор рахунку обов'язковий.");
        }

        if (amount == 0)
        {
            throw new ValidationException("Сума транзакції не може дорівнювати 0.");
        }

        if (occurredOn.ToUniversalTime() > DateTime.UtcNow.AddMinutes(1))
        {
            throw new ValidationException("Дата транзакції не може бути з майбутнього.");
        }

        if (type != TransactionType.Transfer)
        {
            if (categoryId is null)
            {
                throw new ValidationException("Категорія обов'язкова для витрат та доходів.");
            }

            if (type == TransactionType.Expense && amount >= 0)
            {
                throw new ValidationException("Для витрати сума повинна бути від'ємною.");
            }

            if (type == TransactionType.Income && amount <= 0)
            {
                throw new ValidationException("Для доходу сума повинна бути додатною.");
            }
        }
        else
        {
            if (counterpartyAccountId is null || counterpartyAccountId == Guid.Empty)
            {
                throw new ValidationException("Для переказу необхідно вказати рахунок-контрагент.");
            }

            if (transferGroupId == Guid.Empty)
            {
                throw new ValidationException("Група переказу має бути порожньою або валідним ідентифікатором.");
            }
        }

        AccountId = accountId;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Type = type;
        OccurredOn = occurredOn;
        CategoryId = categoryId;
        CounterpartyAccountId = counterpartyAccountId;
        TransferGroupId = transferGroupId;
        Note = NormalizeNote(note);
    }

    public bool IsIncome => Amount > 0;
    public decimal AbsoluteAmount => Math.Abs(Amount);

    private static string? NormalizeNote(string? note)
    {
        if (string.IsNullOrWhiteSpace(note))
        {
            return null;
        }

        var trimmed = note.Trim();
        return trimmed.Length <= 500 ? trimmed : trimmed[..500];
    }
}

