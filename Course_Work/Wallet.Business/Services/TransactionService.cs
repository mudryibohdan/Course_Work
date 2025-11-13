using Wallet.Business.Abstractions;
using Wallet.Business.Models;
using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;
using Wallet.Domain.Repositories;

namespace Wallet.Business.Services;

public sealed class TransactionService(
    ITransactionRepository transactionRepository,
    IRepository<Account> accountRepository,
    IRepository<Category> categoryRepository) : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly IRepository<Account> _accountRepository = accountRepository;
    private readonly IRepository<Category> _categoryRepository = categoryRepository;

    public async Task<Transaction> AddExpenseAsync(Guid accountId, Guid categoryId, decimal amount, DateTime occurredOn, string? note = null, CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
        {
            throw new ValidationException("Сума витрати має бути більшою за 0.");
        }

        var account = await GetAccountAsync(accountId, cancellationToken);
        var category = await GetCategoryAsync(categoryId, CategoryType.Expense, cancellationToken);

        var transaction = new Transaction(
            account.Id,
            -Math.Abs(amount),
            TransactionType.Expense,
            occurredOn,
            category.Id,
            note: note);

        account.Apply(transaction.Amount);
        await _accountRepository.UpdateAsync(account, cancellationToken);
        await _transactionRepository.AddAsync(transaction, cancellationToken);
        return transaction;
    }

    public async Task<Transaction> AddIncomeAsync(Guid accountId, Guid categoryId, decimal amount, DateTime occurredOn, string? note = null, CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
        {
            throw new ValidationException("Сума доходу має бути більшою за 0.");
        }

        var account = await GetAccountAsync(accountId, cancellationToken);
        var category = await GetCategoryAsync(categoryId, CategoryType.Income, cancellationToken);

        var transaction = new Transaction(
            account.Id,
            Math.Abs(amount),
            TransactionType.Income,
            occurredOn,
            category.Id,
            note: note);

        account.Apply(transaction.Amount);
        await _accountRepository.UpdateAsync(account, cancellationToken);
        await _transactionRepository.AddAsync(transaction, cancellationToken);
        return transaction;
    }

    public async Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount, DateTime occurredOn, string? note = null, CancellationToken cancellationToken = default)
    {
        if (amount <= 0)
        {
            throw new ValidationException("Сума переказу має бути більшою за 0.");
        }

        if (fromAccountId == toAccountId)
        {
            throw new ValidationException("Неможливо виконати переказ на той самий рахунок.");
        }

        var fromAccount = await GetAccountAsync(fromAccountId, cancellationToken);
        var toAccount = await GetAccountAsync(toAccountId, cancellationToken);

        var transferGroup = Guid.NewGuid();

        var outgoing = new Transaction(
            fromAccount.Id,
            -Math.Abs(amount),
            TransactionType.Transfer,
            occurredOn,
            counterpartyAccountId: toAccount.Id,
            transferGroupId: transferGroup,
            note: note);

        var incoming = new Transaction(
            toAccount.Id,
            Math.Abs(amount),
            TransactionType.Transfer,
            occurredOn,
            counterpartyAccountId: fromAccount.Id,
            transferGroupId: transferGroup,
            note: note);

        fromAccount.Apply(outgoing.Amount);
        toAccount.Apply(incoming.Amount);

        await _accountRepository.UpdateAsync(fromAccount, cancellationToken);
        await _accountRepository.UpdateAsync(toAccount, cancellationToken);
        await _transactionRepository.AddAsync(outgoing, cancellationToken);
        await _transactionRepository.AddAsync(incoming, cancellationToken);
    }

    public async Task DeleteAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId, cancellationToken);
        if (transaction is null)
        {
            throw new NotFoundException("Транзакцію не знайдено.");
        }

        if (transaction.Type == TransactionType.Transfer && transaction.TransferGroupId is Guid transferGroupId)
        {
            await DeleteTransferGroupAsync(transferGroupId, cancellationToken);
            return;
        }

        var account = await GetAccountAsync(transaction.AccountId, cancellationToken);
        account.Apply(-transaction.Amount);
        await _accountRepository.UpdateAsync(account, cancellationToken);
        await _transactionRepository.DeleteAsync(transaction.Id, cancellationToken);
    }

    public Task<IReadOnlyCollection<Transaction>> GetByPeriodAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
        => _transactionRepository.GetByPeriodAsync(from, to, cancellationToken);

    public Task<IReadOnlyCollection<Transaction>> GetByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
        => _transactionRepository.GetByAccountAsync(accountId, cancellationToken);

    public Task<IReadOnlyCollection<Transaction>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => _transactionRepository.GetByCategoryAsync(categoryId, cancellationToken);

    public async Task<IReadOnlyCollection<Transaction>> SearchByAmountAsync(decimal? minAmount, decimal? maxAmount, CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionRepository.GetAllAsync(cancellationToken);
        return transactions
            .Where(t => t.Type != TransactionType.Transfer)
            .Where(t => minAmount is null || t.AbsoluteAmount >= minAmount.Value)
            .Where(t => maxAmount is null || t.AbsoluteAmount <= maxAmount.Value)
            .OrderByDescending(t => t.OccurredOn)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<Transaction>> SearchByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var from = date.ToDateTime(TimeOnly.MinValue);
        var to = date.ToDateTime(TimeOnly.MaxValue);
        var transactions = await _transactionRepository.GetByPeriodAsync(from, to, cancellationToken);
        return transactions.OrderByDescending(t => t.OccurredOn).ToArray();
    }

    public async Task<PeriodTotals> GetTotalsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionRepository.GetByPeriodAsync(from, to, cancellationToken);
        decimal income = 0m;
        decimal expenses = 0m;

        foreach (var transaction in transactions.Where(t => t.Type != TransactionType.Transfer))
        {
            if (transaction.Type == TransactionType.Income)
            {
                income += transaction.Amount;
            }
            else if (transaction.Type == TransactionType.Expense)
            {
                expenses += transaction.AbsoluteAmount;
            }
        }

        return new PeriodTotals(income, expenses, income - expenses);
    }

    public async Task<IReadOnlyCollection<DailySummary>> GetDailySummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionRepository.GetByPeriodAsync(from, to, cancellationToken);
        return transactions
            .Where(t => t.Type != TransactionType.Transfer)
            .GroupBy(t => DateOnly.FromDateTime(t.OccurredOn))
            .Select(g => DailySummary.FromTransactions(g.Key, g))
            .OrderBy(g => g.Date)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<CategorySummary>> GetCategorySummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var transactions = await _transactionRepository.GetByPeriodAsync(from, to, cancellationToken);
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var categoryLookup = categories.ToDictionary(c => c.Id, c => c);

        return transactions
            .Where(t => t.Type != TransactionType.Transfer && t.CategoryId is not null && categoryLookup.ContainsKey(t.CategoryId.Value))
            .GroupBy(t => t.CategoryId!.Value)
            .Select(group =>
            {
                var category = categoryLookup[group.Key];
                var total = category.Type == CategoryType.Income
                    ? group.Sum(t => t.Amount)
                    : group.Sum(t => t.AbsoluteAmount);
                return new CategorySummary(category.Id, category.Name, category.Type, total);
            })
            .OrderByDescending(summary => summary.TotalAmount)
            .ToArray();
    }

    private async Task<Account> GetAccountAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException("Рахунок не знайдено.");
        }

        return account;
    }

    private async Task<Category> GetCategoryAsync(Guid categoryId, CategoryType expectedType, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Категорію не знайдено.");
        }

        if (category.Type != expectedType)
        {
            throw new ValidationException("Категорія не відповідає типу операції.");
        }

        return category;
    }

    private async Task DeleteTransferGroupAsync(Guid transferGroupId, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllAsync(cancellationToken);
        var groupTransactions = transactions.Where(t => t.TransferGroupId == transferGroupId).ToArray();

        if (groupTransactions.Length == 0)
        {
            throw new NotFoundException("Запис переказу не знайдено.");
        }

        foreach (var transaction in groupTransactions)
        {
            var account = await GetAccountAsync(transaction.AccountId, cancellationToken);
            account.Apply(-transaction.Amount);
            await _accountRepository.UpdateAsync(account, cancellationToken);
            await _transactionRepository.DeleteAsync(transaction.Id, cancellationToken);
        }
    }
}

