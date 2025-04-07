namespace AwesomeGICBank.Domain.Services;

using AwesomeGICBank.Domain.Entities;
using AwesomeGICBank.Domain.Enums;

/// <summary>
/// Generates and displays monthly account statements including calculated interest.
/// </summary>
public class StatementService(Bank bank, InterestRuleService interestRuleService)
{
    private readonly Bank _bank = bank;
    private readonly InterestRuleService _interestRuleService = interestRuleService;

    /// <summary>
    /// Prints the monthly statement for a given account and month.
    /// </summary>
    /// <param name="accountId">The account ID</param>
    /// <param name="year">Year of the statement</param>
    /// <param name="month">Month of the statement</param>
    public void PrintStatement(string accountId, int year, int month)
    {
        try
        {
            var transactions = _interestRuleService.CalculateInterest(accountId, year, month);
    
            Console.WriteLine($"Statement for Account: {accountId} - {year:D4}-{month:D2}");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("| Date     | Txn Id      | Type | Amount   | Balance  |");
    
            decimal balance = 0;
    
            foreach (var txn in transactions)
            {
                // Update balance for this transaction
                balance += txn.Type switch
                {
                    TransactionType.Deposit => txn.Amount,
                    TransactionType.Withdrawal => -txn.Amount,
                    TransactionType.Interest => txn.Amount,
                    _ => 0
                };
    
                // print the statement only for requested month
                if (txn.Date.Year == year && txn.Date.Month == month)
                {
                    string typeShort = txn.Type switch
                    {
                        TransactionType.Deposit => "D",
                        TransactionType.Withdrawal => "W",
                        TransactionType.Interest => "I",
                        _ => ""
                    };
    
                    // skip printing tax id for interests
                    string txnIdDisplay = txn.Type == TransactionType.Interest ? "" : txn.TxnId;
    
                    Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txnIdDisplay,-11} |  {typeShort}   | {txn.Amount,8:F2} | {balance,8:F2} |");
                }
            }
    
            Console.WriteLine("------------------------------------------------------\n");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
