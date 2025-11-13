namespace Wallet.Domain.Exceptions;

public sealed class ValidationException(string message)
    : WalletException(message);

