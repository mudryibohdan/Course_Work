using Wallet.Domain.Abstractions;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;
public abstract class BaseEntity : IIdentifiable
{
    public Guid Id { get; protected set; }

    protected BaseEntity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("Ідентифікатор сутності не може бути порожнім.");
        }

        Id = id;
    }

    protected BaseEntity() : this(Guid.NewGuid())
    {
    }

    public virtual void Validate()
    {
        if (Id == Guid.Empty)
        {
            throw new ValidationException("Ідентифікатор сутності не може бути порожнім.");
        }
    }

    public virtual string GetDisplayName()
    {
        return $"{GetType().Name} ({Id})";
    }

    public override string ToString()
    {
        return GetDisplayName();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity other)
        {
            return false;
        }

        return Id == other.Id && GetType() == other.GetType();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, GetType());
    }

    public virtual string GetShortDescription()
    {
        return GetType().Name;
    }
}

