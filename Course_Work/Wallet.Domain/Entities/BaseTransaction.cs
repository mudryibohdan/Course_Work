using System.Text.Json.Serialization;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;
public abstract class BaseTransaction : BaseEntity
{
    public Guid AccountId { get; protected set; }
    public Guid? CategoryId { get; protected set; }
    public decimal Amount { get; protected set; }
    public DateTime OccurredOn { get; protected set; }
    public string? Note { get; protected set; }

    [JsonConstructor]
    protected BaseTransaction(
        Guid id,
        Guid accountId,
        decimal amount,
        DateTime occurredOn,
        Guid? categoryId = null,
        string? note = null)
        : base(id)
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

        AccountId = accountId;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        OccurredOn = occurredOn;
        CategoryId = categoryId;
        Note = NormalizeNote(note);
    }
    public abstract TransactionType Type { get; }
    public override void Validate()
    {
        base.Validate();

        if (AccountId == Guid.Empty)
        {
            throw new ValidationException("Ідентифікатор рахунку обов'язковий.");
        }

        if (Amount == 0)
        {
            throw new ValidationException("Сума транзакції не може дорівнювати 0.");
        }

        ValidateSpecific();
    }

    protected abstract void ValidateSpecific();

    public virtual decimal GetAbsoluteAmount()
    {
        return Math.Abs(Amount);
    }

    public virtual bool IsIncome()
    {
        return Amount > 0;
    }
   
    public override string GetDisplayName()
    {
        return $"{Type} транзакція на суму {GetAbsoluteAmount():F2} ({OccurredOn:yyyy-MM-dd})";
    }
    public override string GetShortDescription()
    {
        return $"{Type} | {GetAbsoluteAmount():F2} | {OccurredOn:yyyy-MM-dd}";
    }

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

