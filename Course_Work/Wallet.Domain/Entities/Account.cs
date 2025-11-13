using System.Text.Json.Serialization;
using Wallet.Domain.Abstractions;
using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;

public sealed class Account : IIdentifiable
{
    public Guid Id { get; }
    
    [JsonInclude]
    public string Name { get; private set; }
    
    [JsonInclude]
    public string Currency { get; private set; }
    
    [JsonInclude]
    public decimal Balance { get; private set; }

    public Account(string name, string currency = "UAH", decimal initialBalance = 0m)
        : this(Guid.NewGuid(), name, currency, initialBalance)
    {
    }

    [JsonConstructor]
    public Account(Guid id, string name, string currency, decimal balance)
    {
        if (id == Guid.Empty)
        {
            throw new ValidationException("Ідентифікатор рахунку не може бути порожнім.");
        }

        Id = id;
        Name = ValidateName(name);
        Currency = ValidateCurrency(currency);
        if (balance < 0)
        {
            throw new ValidationException("Початковий баланс не може бути від'ємним.");
        }

        Balance = balance;
    }

    public void Rename(string name)
    {
        Name = ValidateName(name);
    }

    public void ChangeCurrency(string currency)
    {
        Currency = ValidateCurrency(currency);
    }

    public void Apply(decimal amount)
    {
        var tentative = Balance + amount;
        if (tentative < 0)
        {
            throw new ValidationException("Недостатньо коштів на рахунку.");
        }

        Balance = tentative;
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Назва рахунку обов'язкова.");
        }

        var trimmed = name.Trim();
        if (trimmed.Length > 100)
        {
            throw new ValidationException("Назва рахунку занадто довга (макс. 100 символів).");
        }

        return trimmed;
    }

    private static string ValidateCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ValidationException("Код валюти обов'язковий.");
        }

        var normalized = currency.Trim().ToUpperInvariant();
        if (normalized.Length is < 3 or > 5)
        {
            throw new ValidationException("Код валюти повинен містити 3-5 символів.");
        }

        return normalized;
    }
}

