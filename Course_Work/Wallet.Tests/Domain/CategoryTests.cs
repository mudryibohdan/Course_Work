using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;

namespace Wallet.Tests.Domain;

public sealed class CategoryTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var category = new Category("Продукти", CategoryType.Expense, "Побутові витрати");

        Assert.Equal("Продукти", category.Name);
        Assert.Equal(CategoryType.Expense, category.Type);
        Assert.Equal("Побутові витрати", category.Description);
    }

    [Fact]
    public void Constructor_Throws_WhenNameEmpty()
    {
        Assert.Throws<ValidationException>(() => new Category("", CategoryType.Income));
    }

    [Fact]
    public void Update_ModifiesFields()
    {
        var category = new Category("Зарплата", CategoryType.Income);

        category.Update("Фріланс", CategoryType.Income, "Додаткові надходження");

        Assert.Equal("Фріланс", category.Name);
        Assert.Equal(CategoryType.Income, category.Type);
        Assert.Equal("Додаткові надходження", category.Description);
    }
}

