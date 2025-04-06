namespace AwesomeGICBank.Domain.Services;

using AwesomeGICBank.Domain.Entities;

/// <summary>
/// Bank persists Accounts and Interest Rules. And manages them with proper encapsulation.
/// </summary>
public class Bank
{
    private readonly Dictionary<string, BankAccount> _accounts = new();
    private readonly List<InterestRule> _interestRules = new();

#region Bank Accounts and Transactions
    
    /// <summary>
    /// Retrieves an existing account or creates a new one if it doesn't exist.
    /// </summary>
    public BankAccount GetAccount(string accountId)
    {
        if (!_accounts.ContainsKey(accountId))
        {
            var newAccount = new BankAccount(accountId);
            _accounts[accountId] = newAccount;
        }

        return _accounts[accountId];
    }

    /// <summary>
    /// Check and get account if it exists.
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="account"></param>
    /// <returns></returns>
    public bool TryGetAccount(string accountId, out BankAccount? account)
    {
        return _accounts.TryGetValue(accountId, out account);
    }

    /// <summary>
    /// Adds a transaction to the specified account.
    /// </summary>
    public void AddTransaction(string accountId, Transaction transaction)
    {
        var account = GetAccount(accountId);
        account.AddTransaction(transaction);
    }

    /// <summary>
    /// Gets all transactions for a given account.
    /// </summary>
    public IReadOnlyList<Transaction> GetTransactions(string accountId)
    {
        return GetAccount(accountId).Transactions;
    }
#endregion

#region Interest Rules
    /// <summary>
    /// Adds or replaces the interest rule for a given date.
    /// Replace is done if a rule is found for the given data.
    /// </summary>
    public void AddOrReplaceInterestRule(InterestRule rule)
    {
        _interestRules.RemoveAll(r => r.Date == rule.Date);
        _interestRules.Add(rule);
    }

    /// <summary>
    /// Returns all interest rules ordered by date.
    /// </summary>
    public List<InterestRule> GetAllInterestRules()
    {
        return _interestRules.OrderBy(r => r.Date).ToList();
    }

    /// <summary>
    /// Gets the interest rule for a given date. Null is retuned if no any rules found.
    /// </summary>
    public InterestRule? GetApplicableRuleForDate(DateTime date)
    {
        return _interestRules
            .Where(r => r.Date <= date)
            .OrderByDescending(r => r.Date)
            .FirstOrDefault();
    }
#endregion
}
