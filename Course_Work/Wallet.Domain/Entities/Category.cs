using System.Text.Json.Serialization;
using Wallet.Domain.Abstractions;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;

public sealed class Category : IIdentifiable
{
    public Guid Id { get; }
    
    [JsonInclude]
    public string Name { get; private set; }
    
    [JsonInclude]
    public CategoryType Type { get; private set; }
    
    [JsonInclude]
    public string? Description { get; private set; }

    public Category(string name, CategoryType type, string? description = null)
        : this(Guid.NewGuid(), name, type, description)
    {
    }

    [JsonConstructor]
    public Category(Guid id, string name, CategoryType type, string? description)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("Ідентифікатор категорії не може бути порожнім.");
        }

        Id = id;
        Name = ValidateName(name);
        Type = type;
        Description = NormalizeDescription(description);
    }

    public void Update(string name, CategoryType type, string? description = null)
    {
        Name = ValidateName(name);
        Type = type;
        Description = NormalizeDescription(description);
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Назва категорії обов'язкова.");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > 100)
        {
            throw new ValidationException("Назва категорії занадто довга (макс. 100 символів).");
        }

        return trimmed;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return null;
        }

        var trimmed = description.Trim();
        return trimmed.Length <= 500
            ? trimmed
            : trimmed[..500];
    }
}

