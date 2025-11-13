using Wallet.Business.Services;
using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;
using Wallet.Tests.Infrastructure;

namespace Wallet.Tests.Business;

public sealed class AccountServiceTests
{
    private readonly InMemoryRepository<Account> _repository = new();
    private readonly AccountService _service;

    public AccountServiceTests()
    {
        _service = new AccountService(_repository);
    }

    [Fact]
    public async Task CreateAsync_PersistsAccount()
    {
        var account = await _service.CreateAsync("Основний", "UAH", 500m);

        var stored = await _repository.GetByIdAsync(account.Id);

        Assert.NotNull(stored);
        Assert.Equal(500m, stored!.Balance);
    }

    [Fact]
    public async Task UpdateAsync_ChangesNameAndCurrency()
    {
        var account = await _service.CreateAsync("Основний", "UAH", 0m);

        await _service.UpdateAsync(account.Id, "Резерв", "USD");

        var updated = await _repository.GetByIdAsync(account.Id);
        Assert.Equal("Резерв", updated!.Name);
        Assert.Equal("USD", updated.Currency);
    }

    [Fact]
    public async Task GetBalanceAsync_ReturnsBalance()
    {
        var account = await _service.CreateAsync("Основний", "UAH", 100m);

        var balance = await _service.GetBalanceAsync(account.Id);

        Assert.Equal(100m, balance);
    }

    [Fact]
    public async Task DeleteAsync_RemovesAccount()
    {
        var account = await _service.CreateAsync("Основний", "UAH", 0m);

        await _service.DeleteAsync(account.Id);

        Assert.Null(await _repository.GetByIdAsync(account.Id));
    }

    [Fact]
    public async Task GetByIdAsync_Throws_NotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(Guid.NewGuid()));
    }
}

