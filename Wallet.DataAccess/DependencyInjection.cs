using Microsoft.Extensions.DependencyInjection;
using Wallet.DataAccess.Json;
using Wallet.Domain.Entities;
using Wallet.Domain.Repositories;

namespace Wallet.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddJsonDataAccess(this IServiceCollection services, string baseDirectory)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory))
        {
            throw new ArgumentException("Базовий каталог збереження даних не може бути порожнім.", nameof(baseDirectory));
        }

        var dataDirectory = Path.GetFullPath(baseDirectory);
        Directory.CreateDirectory(dataDirectory);

        services.AddSingleton<IRepository<Account>>(_ => new JsonRepository<Account>(Path.Combine(dataDirectory, "accounts.json")));
        services.AddSingleton<IRepository<Category>>(_ => new JsonRepository<Category>(Path.Combine(dataDirectory, "categories.json")));
        services.AddSingleton<ITransactionRepository>(_ => new JsonTransactionRepository(Path.Combine(dataDirectory, "transactions.json")));

        return services;
    }
}

