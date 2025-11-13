using Wallet.Domain.Entities;

namespace Wallet.Domain.Repositories;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IReadOnlyCollection<Transaction>> GetByPeriodAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Transaction>> GetByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
}

