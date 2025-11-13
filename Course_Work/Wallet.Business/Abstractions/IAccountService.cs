using Wallet.Domain.Entities;

namespace Wallet.Business.Abstractions;

public interface IAccountService
{
    Task<Account> CreateAsync(string name, string currency = "UAH", decimal initialBalance = 0m, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid accountId, string name, string currency, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Account>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Account> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task<decimal> GetBalanceAsync(Guid accountId, CancellationToken cancellationToken = default);
}

