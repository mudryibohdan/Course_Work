using Wallet.Domain.Entities;

namespace Wallet.Business.Abstractions;

public interface ICategoryService
{
    Task<Category> CreateAsync(string name, CategoryType type, string? description = null, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid categoryId, string name, CategoryType type, string? description = null, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Category>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Category> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
}

