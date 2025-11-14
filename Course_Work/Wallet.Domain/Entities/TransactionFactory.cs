using Wallet.Domain.Exceptions;

namespace Wallet.Domain.Entities;
public static class TransactionFactory
{
    public static BaseTransaction CreateIncome(
        Guid accountId,
        decimal amount,
        DateTime occurredOn,
        Guid categoryId,
        string? note = null)
    {
        return new IncomeTransaction(accountId, amount, occurredOn, categoryId, note);
    }
    public static BaseTransaction CreateExpense(
        Guid accountId,
        decimal amount,
        DateTime occurredOn,
        Guid categoryId,
        string? note = null)
    {
        return new ExpenseTransaction(accountId, amount, occurredOn, categoryId, note);
    }
    public static BaseTransaction CreateTransfer(
        Guid accountId,
        decimal amount,
        DateTime occurredOn,
        Guid counterpartyAccountId,
        Guid? transferGroupId = null,
        string? note = null)
    {
        return new TransferTransaction(accountId, amount, occurredOn, counterpartyAccountId, transferGroupId, note);
    }
    public static BaseTransaction FromLegacyTransaction(Transaction legacy)
    {
        return legacy.Type switch
        {
            TransactionType.Income when legacy.CategoryId.HasValue =>
                new IncomeTransaction(
                    legacy.Id,
                    legacy.AccountId,
                    legacy.Amount,
                    legacy.OccurredOn,
                    legacy.CategoryId.Value,
                    legacy.Note),
            
            TransactionType.Expense when legacy.CategoryId.HasValue =>
                new ExpenseTransaction(
                    legacy.Id,
                    legacy.AccountId,
                    legacy.Amount,
                    legacy.OccurredOn,
                    legacy.CategoryId.Value,
                    legacy.Note),
            
            TransactionType.Transfer when legacy.CounterpartyAccountId.HasValue =>
                new TransferTransaction(
                    legacy.Id,
                    legacy.AccountId,
                    legacy.Amount,
                    legacy.OccurredOn,
                    legacy.CounterpartyAccountId.Value,
                    legacy.TransferGroupId,
                    legacy.Note),
            
            _ => throw new ValidationException("Неможливо конвертувати транзакцію: невизначений тип або відсутні обов'язкові поля.")
        };
    }
}

