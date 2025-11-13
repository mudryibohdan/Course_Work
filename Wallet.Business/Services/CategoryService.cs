using Wallet.Business.Abstractions;
using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;
using Wallet.Domain.Repositories;

namespace Wallet.Business.Services;

public sealed class CategoryService(IRepository<Category> categoryRepository) : ICategoryService
{
    private readonly IRepository<Category> _categoryRepository = categoryRepository;

    public async Task<Category> CreateAsync(string name, CategoryType type, string? description = null, CancellationToken cancellationToken = default)
    {
        var existing = await _categoryRepository.GetAllAsync(cancellationToken);
        if (existing.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase) && c.Type == type))
        {
            throw new ValidationException("Категорія з такою назвою вже існує.");
        }

        var category = new Category(name, type, description);
        await _categoryRepository.AddAsync(category, cancellationToken);
        return category;
    }

    public async Task UpdateAsync(Guid categoryId, string name, CategoryType type, string? description = null, CancellationToken cancellationToken = default)
    {
        var category = await GetByIdAsync(categoryId, cancellationToken);
        var existing = await _categoryRepository.GetAllAsync(cancellationToken);
        if (existing.Any(c => c.Id != categoryId && string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase) && c.Type == type))
        {
            throw new ValidationException("Категорія з такою назвою вже існує.");
        }

        category.Update(name, type, description);
        await _categoryRepository.UpdateAsync(category, cancellationToken);
    }

    public async Task DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Категорію не знайдено.");
        }

        await _categoryRepository.DeleteAsync(categoryId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories
            .OrderBy(c => c.Type)
            .ThenBy(c => c.Name)
            .ToArray();
    }

    public async Task<IReadOnlyCollection<Category>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(cancellationToken);
        }

        var term = searchTerm.Trim();
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories
            .Where(c => c.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .OrderBy(c => c.Name)
            .ToArray();
    }

    public async Task<Category> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException("Категорію не знайдено.");
        }

        return category;
    }
}

