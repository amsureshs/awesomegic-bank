using AwesomeGICBank.Domain.Enums;

namespace AwesomeGICBank.Domain.Entities;

/// <summary>
/// Represents a bank account in the AwesomeGIC Bank.
/// 
/// For the simplisity transactions are kept inside the account. So account entity owns 
/// its behavior and transaction history.
/// 
/// If go for a large scale systems, transactions may be separated into their own service.
/// </summary>
public class BankAccount(string accountId)
{
    public string AccountId { get; set; } = accountId;

    private readonly List<Transaction> _transactions = new();
    public IReadOnlyList<Transaction> Transactions => _transactions;

    public decimal GetCurrentBalance() => GetBalanceUntilDate(DateTime.MaxValue);

    public void AddTransaction(Transaction transaction)
    {
        _transactions.Add(transaction);
    }

    public List<Transaction> GetTransactionsOfMonth(int year, int month)
    {
        return _transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month)
            .OrderBy(t => t.Date)
            .ThenBy(t => t.TxnId)
            .ToList();
    }

    public decimal GetBalanceUntilDate(DateTime date)
    {
        return _transactions
            .Where(t => t.Date <= date)
            .Sum(t => t.Type == TransactionType.Withdrawal ? -t.Amount : t.Amount);
    }
}
