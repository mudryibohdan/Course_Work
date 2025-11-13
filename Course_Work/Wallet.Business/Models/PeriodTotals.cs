namespace Wallet.Business.Models;

public sealed record PeriodTotals(decimal Income, decimal Expenses, decimal Net)
{
    public static PeriodTotals Empty { get; } = new(0m, 0m, 0m);
}

