using Wallet.Business.Abstractions;
using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;
using Wallet.Domain.Repositories;

namespace Wallet.Business.Services;

public sealed class AccountService(IRepository<Account> accountRepository) : IAccountService
{
    private readonly IRepository<Account> _accountRepository = accountRepository;

    public async Task<Account> CreateAsync(string name, string currency = "UAH", decimal initialBalance = 0m, CancellationToken cancellationToken = default)
    {
        var account = new Account(name, currency, initialBalance);
        await _accountRepository.AddAsync(account, cancellationToken);
        return account;
    }

    public async Task UpdateAsync(Guid accountId, string name, string currency, CancellationToken cancellationToken = default)
    {
        var account = await GetByIdAsync(accountId, cancellationToken);
        account.Rename(name);
        account.ChangeCurrency(currency);
        await _accountRepository.UpdateAsync(account, cancellationToken);
    }

    public async Task DeleteAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException("Рахунок не знайдено.");
        }

        await _accountRepository.DeleteAsync(accountId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Account>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var accounts = await _accountRepository.GetAllAsync(cancellationToken);
        return accounts;
    }

    public async Task<Account> GetByIdAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (account is null)
        {
            throw new NotFoundException("Рахунок не знайдено.");
        }

        return account;
    }

    public async Task<decimal> GetBalanceAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var account = await GetByIdAsync(accountId, cancellationToken);
        return account.Balance;
    }
}

