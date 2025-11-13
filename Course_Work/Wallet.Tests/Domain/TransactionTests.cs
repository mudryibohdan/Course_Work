using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;

namespace Wallet.Tests.Domain;

public sealed class TransactionTests
{
    [Fact]
    public void Constructor_CreatesIncome()
    {
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var transaction = new Transaction(accountId, 1200m, TransactionType.Income, DateTime.UtcNow, categoryId, note: "Премія");

        Assert.Equal(accountId, transaction.AccountId);
        Assert.Equal(categoryId, transaction.CategoryId);
        Assert.Equal(TransactionType.Income, transaction.Type);
        Assert.True(transaction.IsIncome);
        Assert.Equal(1200m, transaction.Amount);
        Assert.Equal(1200m, transaction.AbsoluteAmount);
        Assert.Equal("Премія", transaction.Note);
    }

    [Fact]
    public void Constructor_Throws_ForZeroAmount()
    {
        var accountId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        Assert.Throws<ValidationException>(() => new Transaction(accountId, 0m, TransactionType.Income, DateTime.UtcNow, categoryId));
    }

    [Fact]
    public void Constructor_Throws_WhenCategoryMissingForExpense()
    {
        var accountId = Guid.NewGuid();

        Assert.Throws<ValidationException>(() => new Transaction(accountId, -10m, TransactionType.Expense, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_Throws_WhenTransferWithoutCounterparty()
    {
        var accountId = Guid.NewGuid();

        Assert.Throws<ValidationException>(() => new Transaction(accountId, -500m, TransactionType.Transfer, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_AllowsTransferGroup()
    {
        var accountId = Guid.NewGuid();
        var otherId = Guid.NewGuid();
        var group = Guid.NewGuid();

        var transaction = new Transaction(accountId, -250m, TransactionType.Transfer, DateTime.UtcNow, counterpartyAccountId: otherId, transferGroupId: group);

        Assert.Equal(group, transaction.TransferGroupId);
        Assert.Equal(otherId, transaction.CounterpartyAccountId);
    }
}

