using Wallet.Domain.Entities;
using Wallet.Domain.Repositories;

namespace Wallet.Tests.Infrastructure;

internal sealed class InMemoryTransactionRepository : InMemoryRepository<Transaction>, ITransactionRepository
{
    public Task<IReadOnlyCollection<Transaction>> GetByPeriodAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var result = Store.Values
            .Where(t => t.OccurredOn >= from && t.OccurredOn <= to)
            .OrderByDescending(t => t.OccurredOn)
            .ToArray();
        return Task.FromResult<IReadOnlyCollection<Transaction>>(result);
    }

    public Task<IReadOnlyCollection<Transaction>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var result = Store.Values
            .Where(t => t.CategoryId == categoryId)
            .OrderByDescending(t => t.OccurredOn)
            .ToArray();
        return Task.FromResult<IReadOnlyCollection<Transaction>>(result);
    }

    public Task<IReadOnlyCollection<Transaction>> GetByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var result = Store.Values
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.OccurredOn)
            .ToArray();
        return Task.FromResult<IReadOnlyCollection<Transaction>>(result);
    }
}

