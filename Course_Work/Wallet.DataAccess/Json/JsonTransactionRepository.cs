using Wallet.Domain.Entities;
using Wallet.Domain.Repositories;

namespace Wallet.DataAccess.Json;

public sealed class JsonTransactionRepository(string filePath)
    : JsonRepository<Transaction>(filePath), ITransactionRepository
{
    public async Task<IReadOnlyCollection<Transaction>> GetByPeriodAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var all = await ReadAsync(cancellationToken);
        return all
            .Where(t => t.OccurredOn >= from && t.OccurredOn <= to)
            .OrderByDescending(t => t.OccurredOn)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<Transaction>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var all = await ReadAsync(cancellationToken);
        return all
            .Where(t => t.CategoryId == categoryId)
            .OrderByDescending(t => t.OccurredOn)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<Transaction>> GetByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var all = await ReadAsync(cancellationToken);
        return all
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.OccurredOn)
            .ToArray();
    }
}

