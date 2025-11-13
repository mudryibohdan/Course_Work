namespace Wallet.Domain.Exceptions;

public sealed class DataStoreException(string message, Exception? innerException = null)
    : WalletException(message, innerException);

