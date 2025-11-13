using Wallet.Domain.Entities;

namespace Wallet.Business.Models;

public sealed record CategorySummary(Guid CategoryId, string CategoryName, CategoryType Type, decimal TotalAmount);

