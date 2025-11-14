using System.Text.Json.Serialization;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;
public sealed class TransferTransaction : BaseTransaction
{
    public Guid? CounterpartyAccountId { get; }
    public Guid? TransferGroupId { get; }

    public override TransactionType Type => TransactionType.Transfer;

    public TransferTransaction(
        Guid accountId,
        decimal amount,
        DateTime occurredOn,
        Guid counterpartyAccountId,
        Guid? transferGroupId = null,
        string? note = null)
        : base(Guid.NewGuid(), accountId, amount, occurredOn, null, note)
    {
        if (counterpartyAccountId == Guid.Empty)
        {
            throw new ValidationException("Для переказу необхідно вказати рахунок-контрагент.");
        }

        CounterpartyAccountId = counterpartyAccountId;
        TransferGroupId = transferGroupId;
    }

    [JsonConstructor]
    public TransferTransaction(
        Guid id,
        Guid accountId,
        decimal amount,
        DateTime occurredOn,
        Guid? counterpartyAccountId,
        Guid? transferGroupId,
        string? note = null)
        : base(id, accountId, amount, occurredOn, null, note)
    {
        if (counterpartyAccountId == Guid.Empty)
        {
            throw new ValidationException("Для переказу необхідно вказати рахунок-контрагент.");
        }

        if (transferGroupId == Guid.Empty)
        {
            throw new ValidationException("Група переказу має бути порожньою або валідним ідентифікатором.");
        }

        CounterpartyAccountId = counterpartyAccountId;
        TransferGroupId = transferGroupId;
    }

    /// <summary>
    /// Перевизначення методу валідації для переказів.
    /// </summary>
    protected override void ValidateSpecific()
    {
        if (CounterpartyAccountId is null || CounterpartyAccountId == Guid.Empty)
        {
            throw new ValidationException("Для переказу необхідно вказати рахунок-контрагент.");
        }

        if (TransferGroupId == Guid.Empty)
        {
            throw new ValidationException("Група переказу має бути порожньою або валідним ідентифікатором.");
        }
    }
    public override string GetDisplayName()
    {
        var direction = Amount > 0 ? "отримано" : "відправлено";
        return $"Переказ: {direction} {GetAbsoluteAmount():F2} ({OccurredOn:yyyy-MM-dd})";
    }
    public override string GetShortDescription()
    {
        return $"Переказ | {GetAbsoluteAmount():F2} | {OccurredOn:yyyy-MM-dd} | Група: {TransferGroupId}";
    }
}

