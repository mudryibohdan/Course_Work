using Microsoft.Extensions.DependencyInjection;
using Wallet.Business;
using Wallet.Business.Abstractions;
using Wallet.DataAccess;
using Wallet.ConsoleApp;

var services = new ServiceCollection()
    .AddJsonDataAccess(Path.Combine(AppContext.BaseDirectory, "data"))
    .AddBusinessServices()
    .BuildServiceProvider();

var application = new WalletApplication(
    services.GetRequiredService<IAccountService>(),
    services.GetRequiredService<ICategoryService>(),
    services.GetRequiredService<ITransactionService>());

await application.RunAsync();
