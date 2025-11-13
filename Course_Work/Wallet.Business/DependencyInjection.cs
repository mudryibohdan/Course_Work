using Microsoft.Extensions.DependencyInjection;
using Wallet.Business.Abstractions;
using Wallet.Business.Services;

namespace Wallet.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITransactionService, TransactionService>();
        return services;
    }
}

