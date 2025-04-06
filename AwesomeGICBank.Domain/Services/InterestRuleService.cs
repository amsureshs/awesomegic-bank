namespace AwesomeGICBank.Domain.Services;

using AwesomeGICBank.Domain.Entities;
using AwesomeGICBank.Domain.Enums;

/// <summary>
/// Handles logic for applying and calculating interest using defined rules.
/// </summary>
public class InterestRuleService(Bank bank)
{
    private readonly Bank _bank = bank;

    /// <summary>
    /// Adds a new interest rule or replaces an existing one for the same date.
    /// </summary>
    /// <param name="date"></param>
    /// <param name="ruleId"></param>
    /// <param name="rate"></param>
    /// <exception cref="ArgumentException"></exception>
    public void AddInterestRule(DateTime date, string ruleId, decimal rate)
    {
        // It is better to have valid text for ruleId.
        // Therefore this doesn't allow empty text, but skip adding constrains for max legth to keep simple.
        if  (string.IsNullOrWhiteSpace(ruleId))
            throw new ArgumentException("Please provide valid rule id.");

        // Interest rate should be greater than 0 and less than 100
        if (rate <= 0 || rate >= 100)
            throw new ArgumentException("Interest rate should be greater than 0 and less than 100.");

        var rule = new InterestRule(date, ruleId, rate);
        _bank.AddOrReplaceInterestRule(rule);
    }

    /// <summary>
    /// Calculates total interest earned by the account up to a given date
    /// by applying the latest rule available on or before each day.
    /// Interest transactions are insert temporary, so that user can update 
    /// rules and transactions without data constrain.
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public List<Transaction> CalculateInterest(string accountId, int year, int month)
    {
        var isAccountExist = _bank.TryGetAccount(accountId, out var existingAccount);
        if (!isAccountExist)
            throw new InvalidOperationException("Bank account not found for the given accountId.");

        var account = existingAccount!;
        var upToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));

        // when there is no any transaction for the given month
        var monthStartDate = new DateTime(year, month,1);
        var monthlyTransactionsCount = account.Transactions
            .Where(t => t.Date >= monthStartDate && t.Date <= upToDate)
            .Count();
        if (monthlyTransactionsCount == 0)
            return new List<Transaction>();

        var originalTransactions = account.Transactions
            .Where(t => t.Date <= upToDate)
            .OrderBy(t => t.Date)
            .ThenBy(t => t.TxnId)
            .ToList();

        if (originalTransactions.Count == 0)
            return new List<Transaction>();

        // Result list: new copy to hold transactions + interest, in correct order
        var result = new List<Transaction>();

        // oldest transaction date
        DateTime firstTxnDate = originalTransactions.First().Date;
        decimal balance = 0;
        decimal monthlyInterest = 0;
        int txnIndex = 0;

        for (DateTime day = firstTxnDate; day <= upToDate; day = day.AddDays(1))
        {
            // Add any actual transactions for the day and update balance
            while (txnIndex < originalTransactions.Count &&
                originalTransactions[txnIndex].Date.Date == day.Date)
            {
                var txn = originalTransactions[txnIndex];
                result.Add(txn);

                // Update balance
                balance += txn.Type switch
                {
                    Enums.TransactionType.Deposit => txn.Amount,
                    Enums.TransactionType.Withdrawal => -txn.Amount,
                    _ => 0
                };

                txnIndex++;
            }

            // Apply daily interest
            var rule = _bank.GetApplicableRuleForDate(day);
            if (rule != null)
            {
                monthlyInterest += (balance * rule.Rate / 100m) / 365m;
            }

            // If last day of the month, add interest
            if (day.Day == DateTime.DaysInMonth(day.Year, day.Month))
            {
                decimal roundedInterest = Math.Round(monthlyInterest, 2);
                var interestTxn = new Transaction(day, "Interest", TransactionType.Interest, roundedInterest);
                result.Add(interestTxn);
                balance += roundedInterest;

                monthlyInterest = 0;
            }
        }

        return result
            .OrderBy(t => t.Date)
            .ThenBy(t => t.TxnId)
            .ToList();
    }


    /// <summary>
    /// Returns all currently defined interest rules from the Bank.
    /// </summary>
    /// <returns></returns>
    public List<InterestRule> GetInterestRules() => _bank.GetAllInterestRules();
}
