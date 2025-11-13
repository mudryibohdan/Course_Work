namespace Wallet.Domain.Exceptions;

public sealed class NotFoundException(string message)
    : WalletException(message);

