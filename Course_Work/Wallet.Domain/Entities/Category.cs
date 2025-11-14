using System.Text.Json.Serialization;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;
public sealed class Category : BaseEntity
{
    
    [JsonInclude]
    public string Name { get; private set; }
    
    [JsonInclude]
    public CategoryType Type { get; private set; }
    
    [JsonInclude]
    public string? Description { get; private set; }

    public Category(string name, CategoryType type, string? description = null)
        : base()
    {
        Name = ValidateName(name);
        Type = type;
        Description = NormalizeDescription(description);
    }

    [JsonConstructor]
    public Category(Guid id, string name, CategoryType type, string? description)
        : base(id)
    {
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

    public override string GetDisplayName()
    {
        var descriptionPart = string.IsNullOrWhiteSpace(Description) 
            ? string.Empty 
            : $" - {Description}";
        return $"[{Type}] {Name}{descriptionPart}";
    }
    public override string GetShortDescription()
    {
        return $"{Type}: {Name}";
    }
}

