using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;

namespace Wallet.Tests.Domain;

public sealed class AccountTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var account = new Account("Картка", "UAH", 1000m);

        Assert.Equal("Картка", account.Name);
        Assert.Equal("UAH", account.Currency);
        Assert.Equal(1000m, account.Balance);
        Assert.NotEqual(Guid.Empty, account.Id);
    }

    [Fact]
    public void Constructor_Throws_WhenNameEmpty()
    {
        Assert.Throws<ValidationException>(() => new Account("   ", "UAH", 0m));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100.5)]
    public void Constructor_Throws_WhenInitialBalanceNegative(decimal initialBalance)
    {
        Assert.Throws<ValidationException>(() => new Account("Готівка", "UAH", initialBalance));
    }

    [Fact]
    public void Rename_UpdatesName()
    {
        var account = new Account("Старе ім'я", "USD", 0m);

        account.Rename("Нове ім'я");

        Assert.Equal("Нове ім'я", account.Name);
    }

    [Fact]
    public void Rename_Throws_WhenNameInvalid()
    {
        var account = new Account("Назва", "USD", 0m);

        Assert.Throws<ValidationException>(() => account.Rename(""));
    }

    [Fact]
    public void ChangeCurrency_UpdatesCurrency()
    {
        var account = new Account("Кеш", "USD", 0m);

        account.ChangeCurrency("eur");

        Assert.Equal("EUR", account.Currency);
    }

    [Fact]
    public void ChangeCurrency_Throws_WhenCurrencyInvalid()
    {
        var account = new Account("Кеш", "USD", 0m);

        Assert.Throws<ValidationException>(() => account.ChangeCurrency("  "));
    }

    [Fact]
    public void Apply_AddsAmount()
    {
        var account = new Account("Кеш", "UAH", 100m);

        account.Apply(50m);

        Assert.Equal(150m, account.Balance);
    }

    [Fact]
    public void Apply_SubtractsAmount()
    {
        var account = new Account("Кеш", "UAH", 100m);

        account.Apply(-40m);

        Assert.Equal(60m, account.Balance);
    }

    [Fact]
    public void Apply_Throws_WhenBalanceNegative()
    {
        var account = new Account("Кеш", "UAH", 50m);

        Assert.Throws<ValidationException>(() => account.Apply(-60m));
    }
}

