using Wallet.Business.Services;
using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;
using Wallet.Tests.Infrastructure;

namespace Wallet.Tests.Business;

public sealed class CategoryServiceTests
{
    private readonly InMemoryRepository<Category> _repository = new();
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _service = new CategoryService(_repository);
    }

    [Fact]
    public async Task CreateAsync_SavesCategory()
    {
        var category = await _service.CreateAsync("Зарплата", CategoryType.Income);

        var stored = await _repository.GetByIdAsync(category.Id);
        Assert.NotNull(stored);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenDuplicate()
    {
        await _service.CreateAsync("Бензин", CategoryType.Expense);

        await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync("бензин", CategoryType.Expense));
    }

    [Fact]
    public async Task UpdateAsync_ChangesValues()
    {
        var category = await _service.CreateAsync("Бонуси", CategoryType.Income);

        await _service.UpdateAsync(category.Id, "Премія", CategoryType.Income, "Щомісячно");

        var updated = await _repository.GetByIdAsync(category.Id);
        Assert.Equal("Премія", updated!.Name);
        Assert.Equal("Щомісячно", updated.Description);
    }
}

