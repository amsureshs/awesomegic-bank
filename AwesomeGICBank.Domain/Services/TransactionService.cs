namespace AwesomeGICBank.Domain.Services;

using AwesomeGICBank.Domain.Entities;
using AwesomeGICBank.Domain.Enums;

/// <summary>
/// Handles logic for validating and adding transactions.
/// </summary>
public class TransactionService(Bank bank)
{
    private readonly Bank _bank = bank;
    private const decimal MaxDepositAmount = 999999999.99m; // Move this values to to a config file

    /// <summary>
    /// Validate and add transaction to the given account.
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="date"></param>
    /// <param name="typeChar"></param>
    /// <param name="amount"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddTransaction(
        string accountId, 
        DateTime date, 
        char typeChar, 
        decimal amount)
    {
        // It is better to have valid text for accountId.
        // Therefore this doesn't allow empty text, but skip adding constrains for max legth to keep simple.
        if  (string.IsNullOrWhiteSpace(accountId))
            throw new ArgumentException("Please provide valid account id.");

        // Parse and validate transaction type
        // Type is D for deposit, W for withdrawal, case insensitive
        var type = typeChar switch
        {
            'D' or 'd' => TransactionType.Deposit,
            'W' or 'w' => TransactionType.Withdrawal,
            _ => throw new ArgumentException("The transaction type is invalid.")
        };

        // Amount must be greater than zero, decimals are allowed up to 2 decimal places
        if (amount <= 0 || amount > MaxDepositAmount)
            throw new ArgumentException("Amount must be greater than zero and up to two decimal places.");

        // Account balance 
        // An account's balance should not be less than 0. 
        // Therefore, the first transaction on an account should not be a withdrawal, 
        // and any transactions thereafter should not make balance go below 0
        var isAccountExist = _bank.TryGetAccount(accountId, out var existingAccount);

        if (type == TransactionType.Withdrawal)
        {
            if (!isAccountExist || existingAccount!.Transactions.Count == 0)
                throw new InvalidOperationException("The first transaction should be a Deposit.");

            if (existingAccount.GetCurrentBalance() < amount)
                throw new InvalidOperationException("Insufficient balance for this withdrawal.");
        }

        // makesure account create and get it
        var account = _bank.GetAccount(accountId);

        // Generate TransactionId
        var txnId = GenerateTransactionId(account.AccountId, date);

        // Add the transaction
        var transaction = new Transaction(date, txnId, type, amount);
        _bank.AddTransaction(accountId, transaction);
    }

    /// <summary>
    /// Get generated transaction id for the account id. 
    /// Each transaction should be given a unique id in YYYMMdd-xx format, where xx is a running number. 
    /// Some example transaction ids are 20230626-01 and 20230626-02
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="date"></param>
    /// <returns></returns>
    private string GenerateTransactionId(string accountId, DateTime date)
    {
        var datePrefix = date.ToString("yyyyMMdd");

        var existingTransactions = _bank
            .GetTransactions(accountId)
            .Where(t => t.Date.Date == date.Date)
            .ToList();

        var nextSequence = existingTransactions.Count + 1;

        return $"{datePrefix}-{nextSequence:D2}";
    }
}
