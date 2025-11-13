using Wallet.Business.Abstractions;
using Wallet.Business.Models;
using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;

namespace Wallet.ConsoleApp;

internal sealed class WalletApplication
{
    private readonly IAccountService _accountService;
    private readonly ICategoryService _categoryService;
    private readonly ITransactionService _transactionService;

    public WalletApplication(
        IAccountService accountService,
        ICategoryService categoryService,
        ITransactionService transactionService)
    {
        _accountService = accountService;
        _categoryService = categoryService;
        _transactionService = transactionService;
    }

    public async Task RunAsync()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var exit = false;
        while (!exit)
        {
            Console.WriteLine("  СИСТЕМА КЕРУВАННЯ ФІНАНСАМИ");
            Console.WriteLine("1. Рахунки");
            Console.WriteLine("2. Категорії");
            Console.WriteLine("3. Транзакції");
            Console.WriteLine("4. Звіти");
            Console.WriteLine("5. Пошук");
            Console.WriteLine("0. Вихід");

            var choice = ConsoleInput.ReadInt("Оберіть пункт меню", 0, 5);
            Console.WriteLine();
            try
            {
                switch (choice)
                {
                    case 1:
                        await ManageAccountsAsync();
                        break;
                    case 2:
                        await ManageCategoriesAsync();
                        break;
                    case 3:
                        await ManageTransactionsAsync();
                        break;
                    case 4:
                        await ShowReportsAsync();
                        break;
                    case 5:
                        await SearchAsync();
                        break;
                    case 0:
                        exit = true;
                        break;
                }
            }
            catch (WalletException ex)
            {
                Console.WriteLine($"ПОМИЛКА: {ex.Message}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Виникла помилка.");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
            }

            if (!exit)
            {
                Console.WriteLine("Натисніть Enter для продовження...");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    private async Task ManageAccountsAsync()
    {
        while (true)
        {
            Console.WriteLine("РАХУНКИ");
            Console.WriteLine("1. Переглянути рахунки");
            Console.WriteLine("2. Додати рахунок");
            Console.WriteLine("3. Редагувати рахунок");
            Console.WriteLine("4. Видалити рахунок");
            Console.WriteLine("5. Переглянути баланс рахунку");
            Console.WriteLine("0. Назад");

            var choice = ConsoleInput.ReadInt("Ваш вибір", 0, 5);
            Console.WriteLine();
            switch (choice)
            {
                case 0:
                    return;
                case 1:
                    await ShowAccountsAsync();
                    break;
                case 2:
                    await CreateAccountAsync();
                    break;
                case 3:
                    await UpdateAccountAsync();
                    break;
                case 4:
                    await DeleteAccountAsync();
                    break;
                case 5:
                    await ShowAccountBalanceAsync();
                    break;
            }
            Console.WriteLine();
        }
    }

    private async Task ManageCategoriesAsync()
    {
        while (true)
        {
            Console.WriteLine(" КАТЕГОРІЇ ");
            Console.WriteLine("1. Переглянути усі категорії");
            Console.WriteLine("2. Додати категорію");
            Console.WriteLine("3. Редагувати категорію");
            Console.WriteLine("4. Видалити категорію");
            Console.WriteLine("0. Назад");

            var choice = ConsoleInput.ReadInt("Ваш вибір", 0, 4);
            Console.WriteLine();
            switch (choice)
            {
                case 0:
                    return;
                case 1:
                    await ShowCategoriesAsync();
                    break;
                case 2:
                    await CreateCategoryAsync();
                    break;
                case 3:
                    await UpdateCategoryAsync();
                    break;
                case 4:
                    await DeleteCategoryAsync();
                    break;
            }
            Console.WriteLine();
        }
    }

    private async Task ManageTransactionsAsync()
    {
        while (true)
        {
            Console.WriteLine("ТРАНЗАКЦІЇ");
            Console.WriteLine("1. Зареєструвати витрату");
            Console.WriteLine("2. Зареєструвати дохід");
            Console.WriteLine("3. Переказ між рахунками");
            Console.WriteLine("4. Видалити транзакцію");
            Console.WriteLine("5. Показати транзакції за рахунком");
            Console.WriteLine("6. Показати транзакції за період");
            Console.WriteLine("0. Назад");

            var choice = ConsoleInput.ReadInt("Ваш вибір", 0, 6);
            Console.WriteLine();
            switch (choice)
            {
                case 0:
                    return;
                case 1:
                    await RegisterExpenseAsync();
                    break;
                case 2:
                    await RegisterIncomeAsync();
                    break;
                case 3:
                    await RegisterTransferAsync();
                    break;
                case 4:
                    await DeleteTransactionAsync();
                    break;
                case 5:
                    await ShowTransactionsByAccountAsync();
                    break;
                case 6:
                    await ShowTransactionsByPeriodAsync();
                    break;
            }
            Console.WriteLine();
        }
    }

    private async Task ShowReportsAsync()
    {
        while (true)
        {
            Console.WriteLine(" ЗВІТИ ");
            Console.WriteLine("1. Загальні підсумки за період");
            Console.WriteLine("2. Графік доходів/витрат за днями");
            Console.WriteLine("3. Підсумки за категоріями");
            Console.WriteLine("0. Назад");

            var choice = ConsoleInput.ReadInt("Ваш вибір", 0, 3);
            Console.WriteLine();
            switch (choice)
            {
                case 0:
                    return;
                case 1:
                    await ShowTotalsAsync();
                    break;
                case 2:
                    await ShowDailySummaryAsync();
                    break;
                case 3:
                    await ShowCategorySummaryAsync();
                    break;
            }
            Console.WriteLine();
        }
    }

    private async Task SearchAsync()
    {
        while (true)
        {
            Console.WriteLine("ПОШУК");
            Console.WriteLine("1. Пошук категорій за назвою");
            Console.WriteLine("2. Транзакції за категорією");
            Console.WriteLine("3. Транзакції за сумою");
            Console.WriteLine("4. Транзакції за датою");
            Console.WriteLine("0. Назад");

            var choice = ConsoleInput.ReadInt("Ваш вибір", 0, 4);
            Console.WriteLine();
            switch (choice)
            {
                case 0:
                    return;
                case 1:
                    await SearchCategoriesAsync();
                    break;
                case 2:
                    await SearchTransactionsByCategoryAsync();
                    break;
                case 3:
                    await SearchTransactionsByAmountAsync();
                    break;
                case 4:
                    await SearchTransactionsByDateAsync();
                    break;
            }
            Console.WriteLine();
        }
    }

    private async Task ShowAccountsAsync()
    {
        var accounts = await _accountService.GetAllAsync();
        if (accounts.Count == 0)
        {
            Console.WriteLine("Рахунки відсутні.");
            return;
        }

        Console.WriteLine("Перелік рахунків:");
        foreach (var (account, index) in accounts.Select((a, i) => (a, i + 1)))
        {
            Console.WriteLine($"{index}. {account.Name} ({account.Currency}) - баланс: {account.Balance:F2}");
        }
    }

    private async Task CreateAccountAsync()
    {
        var name = ConsoleInput.ReadRequiredString("Назва рахунку");
        var currency = ConsoleInput.ReadRequiredString("Валюта (наприклад, UAH, USD)", maxLength: 5).ToUpperInvariant();
        var initialBalance = ConsoleInput.ReadDecimal("Початковий баланс", 0m);

        var account = await _accountService.CreateAsync(name, currency, initialBalance);
        Console.WriteLine($"Рахунок '{account.Name}' створено.");
    }

    private async Task UpdateAccountAsync()
    {
        var account = await SelectAccountAsync();
        if (account is null)
        {
            return;
        }

        var name = ConsoleInput.ReadRequiredString("Нова назва");
        var currency = ConsoleInput.ReadRequiredString("Нова валюта", maxLength: 5).ToUpperInvariant();

        await _accountService.UpdateAsync(account.Id, name, currency);
        Console.WriteLine("Рахунок оновлено.");
    }

    private async Task DeleteAccountAsync()
    {
        var account = await SelectAccountAsync();
        if (account is null)
        {
            return;
        }

        await _accountService.DeleteAsync(account.Id);
        Console.WriteLine("Рахунок видалено.");
    }

    private async Task ShowAccountBalanceAsync()
    {
        var account = await SelectAccountAsync();
        if (account is null)
        {
            return;
        }

        var balance = await _accountService.GetBalanceAsync(account.Id);
        Console.WriteLine($"Баланс рахунку '{account.Name}': {balance:F2} {account.Currency}");
    }

    private async Task ShowCategoriesAsync()
    {
        var categories = await _categoryService.GetAllAsync();
        if (categories.Count == 0)
        {
            Console.WriteLine("Категорії відсутні.");
            return;
        }

        Console.WriteLine("Категорії:");
        foreach (var category in categories)
        {
            Console.WriteLine($"- [{category.Type}] {category.Name} {(string.IsNullOrEmpty(category.Description) ? string.Empty : $"- {category.Description}")}");
        }
    }

    private async Task CreateCategoryAsync()
    {
        var name = ConsoleInput.ReadRequiredString("Назва категорії");
        var type = ReadCategoryType();
        var description = ConsoleInput.ReadOptionalString("Опис");

        var category = await _categoryService.CreateAsync(name, type, description);
        Console.WriteLine($"Категорію '{category.Name}' створено.");
    }

    private async Task UpdateCategoryAsync()
    {
        var category = await SelectCategoryAsync();
        if (category is null)
        {
            return;
        }

        var name = ConsoleInput.ReadRequiredString("Нова назва");
        var type = ReadCategoryType();
        var description = ConsoleInput.ReadOptionalString("Новий опис");

        await _categoryService.UpdateAsync(category.Id, name, type, description);
        Console.WriteLine("Категорію оновлено.");
    }

    private async Task DeleteCategoryAsync()
    {
        var category = await SelectCategoryAsync();
        if (category is null)
        {
            return;
        }

        await _categoryService.DeleteAsync(category.Id);
        Console.WriteLine("Категорію видалено.");
    }

    private async Task RegisterExpenseAsync()
    {
        var account = await SelectAccountAsync();
        if (account is null)
        {
            return;
        }

        var category = await SelectCategoryByTypeAsync(CategoryType.Expense);
        if (category is null)
        {
            return;
        }

        var amount = ConsoleInput.ReadDecimal("Сума витрати", 0.01m);
        var date = ConsoleInput.ReadDate("Дата операції");
        var note = ConsoleInput.ReadOptionalString("Коментар");

        await _transactionService.AddExpenseAsync(account.Id, category.Id, amount, date, note);
        Console.WriteLine("Витрату зафіксовано.");
    }

    private async Task RegisterIncomeAsync()
    {
        var account = await SelectAccountAsync();
        if (account is null)
        {
            return;
        }

        var category = await SelectCategoryByTypeAsync(CategoryType.Income);
        if (category is null)
        {
            return;
        }

        var amount = ConsoleInput.ReadDecimal("Сума доходу", 0.01m);
        var date = ConsoleInput.ReadDate("Дата операції");
        var note = ConsoleInput.ReadOptionalString("Коментар");

        await _transactionService.AddIncomeAsync(account.Id, category.Id, amount, date, note);
        Console.WriteLine("Дохід зафіксовано.");
    }

    private async Task RegisterTransferAsync()
    {
        var fromAccount = await SelectAccountAsync("Оберіть рахунок Джерело");
        if (fromAccount is null)
        {
            return;
        }

        var toAccount = await SelectAccountAsync("Оберіть рахунок Отримувач", excludeAccountId: fromAccount.Id);
        if (toAccount is null)
        {
            return;
        }

        var amount = ConsoleInput.ReadDecimal("Сума переказу", 0.01m);
        var date = ConsoleInput.ReadDate("Дата переказу");
        var note = ConsoleInput.ReadOptionalString("Коментар");

        await _transactionService.TransferAsync(fromAccount.Id, toAccount.Id, amount, date, note);
        Console.WriteLine("Переказ виконано.");
    }

    private async Task DeleteTransactionAsync()
    {
        var transactions = await _transactionService.GetByPeriodAsync(DateTime.MinValue, DateTime.MaxValue);
        if (transactions.Count == 0)
        {
            Console.WriteLine("Транзакції відсутні.");
            return;
        }

        var categories = await _categoryService.GetAllAsync();
        var accounts = await _accountService.GetAllAsync();

        var selected = SelectFromList(transactions.OrderByDescending(t => t.OccurredOn).ToList(), t => FormatTransaction(t, categories, accounts), "Оберіть транзакцію для видалення");
        if (selected is null)
        {
            return;
        }

        await _transactionService.DeleteAsync(selected.Id);
        Console.WriteLine("Транзакцію видалено.");
    }

    private async Task ShowTransactionsByAccountAsync()
    {
        var account = await SelectAccountAsync();
        if (account is null)
        {
            return;
        }

        var transactions = await _transactionService.GetByAccountAsync(account.Id);
        await PrintTransactionsAsync(transactions, $"Транзакції для рахунку '{account.Name}'");
    }

    private async Task ShowTransactionsByPeriodAsync()
    {
        var from = ConsoleInput.ReadDate("Початкова дата");
        var to = ConsoleInput.ReadDate("Кінцева дата");

        if (to < from)
        {
            Console.WriteLine("Кінцева дата не може бути раніше початкової.");
            return;
        }

        var transactions = await _transactionService.GetByPeriodAsync(from, EndOfDay(to));
        await PrintTransactionsAsync(transactions, "Транзакції за період");
    }

    private async Task ShowTotalsAsync()
    {
        var from = ConsoleInput.ReadDate("Початкова дата");
        var to = ConsoleInput.ReadDate("Кінцева дата");

        if (to < from)
        {
            Console.WriteLine("Кінцева дата не може бути раніше початкової.");
            return;
        }

        var totals = await _transactionService.GetTotalsAsync(from, EndOfDay(to));
        Console.WriteLine($"Доходи: {totals.Income:F2}");
        Console.WriteLine($"Витрати: {totals.Expenses:F2}");
        Console.WriteLine($"Чистий результат: {totals.Net:F2}");
    }

    private async Task ShowDailySummaryAsync()
    {
        var from = ConsoleInput.ReadDate("Початкова дата");
        var to = ConsoleInput.ReadDate("Кінцева дата");

        if (to < from)
        {
            Console.WriteLine("Кінцева дата не може бути раніше початкової.");
            return;
        }

        var summaries = await _transactionService.GetDailySummaryAsync(from, EndOfDay(to));
        if (summaries.Count == 0)
        {
            Console.WriteLine("Дані відсутні.");
            return;
        }

        Console.WriteLine("Дата        | Доходи  | Витрати | Баланс");
        Console.WriteLine("-----------------------------------------");
        foreach (var summary in summaries)
        {
            Console.WriteLine($"{summary.Date:yyyy-MM-dd} | {summary.Income,7:F2} | {summary.Expenses,7:F2} | {summary.Net,7:F2}");
        }
    }

    private async Task ShowCategorySummaryAsync()
    {
        var from = ConsoleInput.ReadDate("Початкова дата");
        var to = ConsoleInput.ReadDate("Кінцева дата");

        if (to < from)
        {
            Console.WriteLine("Кінцева дата не може бути раніше початкової.");
            return;
        }

        var summaries = await _transactionService.GetCategorySummaryAsync(from, EndOfDay(to));
        if (summaries.Count == 0)
        {
            Console.WriteLine("Дані відсутні.");
            return;
        }

        Console.WriteLine("Категорія                    | Тип     | Сума");
        Console.WriteLine("---------------------------------------------");
        foreach (var summary in summaries)
        {
            Console.WriteLine($"{summary.CategoryName.PadRight(28)} | {summary.Type,-7} | {summary.TotalAmount,8:F2}");
        }
    }

    private async Task SearchCategoriesAsync()
    {
        var term = ConsoleInput.ReadRequiredString("Введіть фрагмент назви");
        var result = await _categoryService.SearchByNameAsync(term);
        if (result.Count == 0)
        {
            Console.WriteLine("Нічого не знайдено.");
            return;
        }

        Console.WriteLine("Знайдені категорії:");
        foreach (var category in result)
        {
            Console.WriteLine($"- [{category.Type}] {category.Name}");
        }
    }

    private async Task SearchTransactionsByCategoryAsync()
    {
        var category = await SelectCategoryAsync();
        if (category is null)
        {
            return;
        }

        var transactions = await _transactionService.GetByCategoryAsync(category.Id);
        await PrintTransactionsAsync(transactions, $"Транзакції для категорії '{category.Name}'");
    }

    private async Task SearchTransactionsByAmountAsync()
    {
        var (min, max) = ConsoleInput.ReadAmountRange();
        var transactions = await _transactionService.SearchByAmountAsync(min, max);
        await PrintTransactionsAsync(transactions, "Результат пошуку за сумою");
    }

    private async Task SearchTransactionsByDateAsync()
    {
        var date = ConsoleInput.ReadDate("Вкажіть дату");
        var transactions = await _transactionService.SearchByDateAsync(DateOnly.FromDateTime(date));
        await PrintTransactionsAsync(transactions, $"Транзакції за {date:yyyy-MM-dd}");
    }

    private async Task PrintTransactionsAsync(IReadOnlyCollection<Transaction> transactions, string title)
    {
        if (transactions.Count == 0)
        {
            Console.WriteLine("Транзакції відсутні.");
            return;
        }

        var categories = await _categoryService.GetAllAsync();
        var accounts = await _accountService.GetAllAsync();

        Console.WriteLine(title);
        foreach (var transaction in transactions.OrderByDescending(t => t.OccurredOn))
        {
            Console.WriteLine(FormatTransaction(transaction, categories, accounts));
        }
    }

    private async Task<Account?> SelectAccountAsync(string prompt = "Оберіть рахунок", Guid? excludeAccountId = null)
    {
        var accounts = (await _accountService.GetAllAsync()).ToList();
        if (excludeAccountId is not null)
        {
            accounts = accounts.Where(a => a.Id != excludeAccountId.Value).ToList();
        }

        if (accounts.Count == 0)
        {
            Console.WriteLine("Рахунки відсутні.");
            return null;
        }

        return SelectFromList(accounts, a => $"{a.Name} ({a.Currency}) - {a.Balance:F2}", prompt);
    }

    private async Task<Category?> SelectCategoryAsync()
    {
        var categories = (await _categoryService.GetAllAsync()).ToList();
        if (categories.Count == 0)
        {
            Console.WriteLine("Категорії відсутні.");
            return null;
        }

        return SelectFromList(categories, c => $"[{c.Type}] {c.Name}", "Оберіть категорію");
    }

    private async Task<Category?> SelectCategoryByTypeAsync(CategoryType type)
    {
        var categories = (await _categoryService.GetAllAsync()).Where(c => c.Type == type).ToList();
        if (categories.Count == 0)
        {
            Console.WriteLine("Відповідні категорії відсутні.");
            return null;
        }

        return SelectFromList(categories, c => c.Name, $"Оберіть категорію ({type})");
    }

    private static CategoryType ReadCategoryType()
    {
        Console.WriteLine("Оберіть тип категорії:");
        Console.WriteLine("1. Витрата");
        Console.WriteLine("2. Дохід");
        Console.WriteLine("3. Переказ");
        var choice = ConsoleInput.ReadInt("Ваш вибір", 1, 3);
        return choice switch
        {
            1 => CategoryType.Expense,
            2 => CategoryType.Income,
            _ => CategoryType.Transfer
        };
    }

    private static T? SelectFromList<T>(IReadOnlyList<T> items, Func<T, string> formatter, string prompt)
    {
        if (items.Count == 0)
        {
            return default;
        }

        for (var i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {formatter(items[i])}");
        }
        Console.WriteLine("0. Скасувати");
        var choice = ConsoleInput.ReadInt(prompt, 0, items.Count);
        if (choice == 0)
        {
            return default;
        }

        return items[choice - 1];
    }

    private static string FormatTransaction(Transaction transaction, IReadOnlyCollection<Category> categories, IReadOnlyCollection<Account> accounts)
    {
        var accountName = accounts.FirstOrDefault(a => a.Id == transaction.AccountId)?.Name ?? "Невідомий рахунок";
        var categoryName = transaction.CategoryId is Guid categoryId
            ? categories.FirstOrDefault(c => c.Id == categoryId)?.Name ?? "Без категорії"
            : "Переказ";

        var direction = transaction.Type switch
        {
            TransactionType.Income => "+",
            TransactionType.Expense => "-",
            TransactionType.Transfer => transaction.Amount > 0 ? "=>" : "<=",
            _ => string.Empty
        };

        var note = string.IsNullOrWhiteSpace(transaction.Note) ? string.Empty : transaction.Note;
        return $"{transaction.OccurredOn:yyyy-MM-dd HH:mm} | {accountName} | {transaction.Type,-8} | {direction}{transaction.AbsoluteAmount,8:F2} | {categoryName} | {note}";
    }

    private static DateTime EndOfDay(DateTime date) => date.Date.AddDays(1).AddTicks(-1);
}

