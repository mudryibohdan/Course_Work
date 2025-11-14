using System.Text.Json.Serialization;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;
public sealed class Transaction : BaseTransaction
{
    public Guid? CounterpartyAccountId { get; }
    public Guid? TransferGroupId { get; }
    
    public override TransactionType Type { get; }

    public Transaction(
        Guid accountId,
        decimal amount,
        TransactionType type,
        DateTime occurredOn,
        Guid? categoryId = null,
        Guid? counterpartyAccountId = null,
        Guid? transferGroupId = null,
        string? note = null)
        : this(Guid.NewGuid(), accountId, amount, type, occurredOn, categoryId, counterpartyAccountId, transferGroupId, note)
    {
    }

    [JsonConstructor]
    public Transaction(
        Guid id,
        Guid accountId,
        decimal amount,
        TransactionType type,
        DateTime occurredOn,
        Guid? categoryId = null,
        Guid? counterpartyAccountId = null,
        Guid? transferGroupId = null,
        string? note = null)
        : base(id, accountId, amount, occurredOn, categoryId, note)
    {
        Type = type;
        CounterpartyAccountId = counterpartyAccountId;
        TransferGroupId = transferGroupId;
        
        // Додаткова валідація для сумісності зі старим кодом
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
    }

    protected override void ValidateSpecific()
    {
     
    }

    public new bool IsIncome => Amount > 0;
    public decimal AbsoluteAmount => GetAbsoluteAmount();
}

