using Wallet.Business.Services;
using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;
using Wallet.Tests.Infrastructure;

namespace Wallet.Tests.Business;

public sealed class TransactionServiceTests
{
    private readonly InMemoryRepository<Account> _accountRepository = new();
    private readonly InMemoryRepository<Category> _categoryRepository = new();
    private readonly InMemoryTransactionRepository _transactionRepository = new();
    private readonly TransactionService _service;

    private readonly Category _incomeCategory;
    private readonly Category _expenseCategory;

    public TransactionServiceTests()
    {
        _incomeCategory = new Category("Зарплата", CategoryType.Income);
        _expenseCategory = new Category("Продукти", CategoryType.Expense);

        _categoryRepository.AddAsync(_incomeCategory).GetAwaiter().GetResult();
        _categoryRepository.AddAsync(_expenseCategory).GetAwaiter().GetResult();

        _service = new TransactionService(_transactionRepository, _accountRepository, _categoryRepository);
    }

    [Fact]
    public async Task AddIncomeAsync_IncreasesBalance()
    {
        var account = await CreateAccountAsync("Основний", 0m);

        var transaction = await _service.AddIncomeAsync(account.Id, _incomeCategory.Id, 1000m, DateTime.UtcNow);

        var updatedAccount = await _accountRepository.GetByIdAsync(account.Id);
        Assert.Equal(1000m, updatedAccount!.Balance);

        var storedTransaction = await _transactionRepository.GetByIdAsync(transaction.Id);
        Assert.NotNull(storedTransaction);
        Assert.Equal(TransactionType.Income, storedTransaction!.Type);
    }

    [Fact]
    public async Task AddExpenseAsync_DecreasesBalance()
    {
        var account = await CreateAccountAsync("Основний", 500m);

        var transaction = await _service.AddExpenseAsync(account.Id, _expenseCategory.Id, 200m, DateTime.UtcNow);

        var updatedAccount = await _accountRepository.GetByIdAsync(account.Id);
        Assert.Equal(300m, updatedAccount!.Balance);
        Assert.Equal(TransactionType.Expense, transaction.Type);
    }

    [Fact]
    public async Task AddExpenseAsync_Throws_WhenInsufficientFunds()
    {
        var account = await CreateAccountAsync("Основний", 100m);

        await Assert.ThrowsAsync<ValidationException>(() => _service.AddExpenseAsync(account.Id, _expenseCategory.Id, 200m, DateTime.UtcNow));
    }

    [Fact]
    public async Task TransferAsync_UpdatesBothAccounts()
    {
        var from = await CreateAccountAsync("Карта", 1000m);
        var to = await CreateAccountAsync("Збереження", 100m);

        await _service.TransferAsync(from.Id, to.Id, 300m, DateTime.UtcNow);

        var fromUpdated = await _accountRepository.GetByIdAsync(from.Id);
        var toUpdated = await _accountRepository.GetByIdAsync(to.Id);

        Assert.Equal(700m, fromUpdated!.Balance);
        Assert.Equal(400m, toUpdated!.Balance);

        var transactions = await _transactionRepository.GetAllAsync();
        Assert.Equal(2, transactions.Count);
    }

    [Fact]
    public async Task DeleteAsync_RevertsBalance()
    {
        var account = await CreateAccountAsync("Карта", 1000m);
        var tx = await _service.AddIncomeAsync(account.Id, _incomeCategory.Id, 500m, DateTime.UtcNow);

        await _service.DeleteAsync(tx.Id);

        var updatedAccount = await _accountRepository.GetByIdAsync(account.Id);
        Assert.Equal(1000m, updatedAccount!.Balance);
        Assert.Empty(await _transactionRepository.GetAllAsync());
    }

    [Fact]
    public async Task GetTotalsAsync_ReturnsCorrectValues()
    {
        var account = await CreateAccountAsync("Карта", 0m);
        var today = DateTime.UtcNow;

        await _service.AddIncomeAsync(account.Id, _incomeCategory.Id, 1000m, today);
        await _service.AddExpenseAsync(account.Id, _expenseCategory.Id, 200m, today);

        var totals = await _service.GetTotalsAsync(today.Date, today.Date.AddDays(1));

        Assert.Equal(1000m, totals.Income);
        Assert.Equal(200m, totals.Expenses);
        Assert.Equal(800m, totals.Net);
    }

    private async Task<Account> CreateAccountAsync(string name, decimal balance)
    {
        var account = new Account(name, "UAH", balance);
        await _accountRepository.AddAsync(account);
        return account;
    }
}

