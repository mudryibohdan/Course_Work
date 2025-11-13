using Wallet.Business.Models;
using Wallet.Domain.Entities;

namespace Wallet.Business.Abstractions;

public interface ITransactionService
{
    Task<Transaction> AddExpenseAsync(Guid accountId, Guid categoryId, decimal amount, DateTime occurredOn, string? note = null, CancellationToken cancellationToken = default);
    Task<Transaction> AddIncomeAsync(Guid accountId, Guid categoryId, decimal amount, DateTime occurredOn, string? note = null, CancellationToken cancellationToken = default);
    Task TransferAsync(Guid fromAccountId, Guid toAccountId, decimal amount, DateTime occurredOn, string? note = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid transactionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Transaction>> GetByPeriodAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> GetByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> SearchByAmountAsync(decimal? minAmount, decimal? maxAmount, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> SearchByDateAsync(DateOnly date, CancellationToken cancellationToken = default);

    Task<PeriodTotals> GetTotalsAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DailySummary>> GetDailySummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CategorySummary>> GetCategorySummaryAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
}

