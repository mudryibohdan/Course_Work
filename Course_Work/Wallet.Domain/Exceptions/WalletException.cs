namespace Wallet.Domain.Exceptions;

public abstract class WalletException(string message, Exception? innerException = null)
    : Exception(message, innerException);

