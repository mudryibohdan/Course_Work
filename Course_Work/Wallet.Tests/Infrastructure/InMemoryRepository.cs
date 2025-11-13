using System.Collections.Concurrent;
using Wallet.Domain.Abstractions;
using Wallet.Domain.Repositories;

namespace Wallet.Tests.Infrastructure;

internal class InMemoryRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IIdentifiable
{
    protected readonly ConcurrentDictionary<Guid, TEntity> Store = new();

    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Store[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Store.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<TEntity> result = Store.Values.ToList();
        return Task.FromResult(result);
    }

    public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Store.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Store[entity.Id] = entity;
        return Task.CompletedTask;
    }
}

