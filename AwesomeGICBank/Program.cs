using AwesomeGICBank.Domain.Enums;
using AwesomeGICBank.Domain.Services;

StartAwesomBank();

static void StartAwesomBank()
{
    var bank = new Bank();
    var transactionService = new TransactionService(bank);
    var interestService = new InterestRuleService(bank);
    var statementService = new StatementService(bank, interestService);

    while (true)
    {
        Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
        Console.WriteLine("[T] Input transactions");
        Console.WriteLine("[I] Define interest rules");
        Console.WriteLine("[P] Print statement");
        Console.WriteLine("[Q] Quit");
        Console.Write("> ");
        var input = Console.ReadLine()?.Trim().ToUpper();

        if (string.IsNullOrWhiteSpace(input)) continue;

        switch (input)
        {
            case "T":
                HandleTransactionInput(transactionService, bank);
                break;
            case "I":
                HandleInterestRuleInput(interestService);
                break;
            case "P":
                HandleStatementInput(statementService);
                break;
            case "Q":
                Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
                Console.WriteLine("Have a nice day!");
                return;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                break;
        }
    }
}

static void HandleTransactionInput(TransactionService service, Bank bank)
{
    while (true)
    {
        Console.WriteLine("Please enter transaction details in <Date> <Account> <Type> <Amount> format");
        Console.WriteLine("(or enter blank to go back to main menu):");
        Console.Write("> ");
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;

        try
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4) throw new FormatException("Invalid input format.");

            var date = DateTime.ParseExact(parts[0], "yyyyMMdd", null);
            var accountId = parts[1];
            var type = char.ToUpper(parts[2][0]);
            var amount = decimal.Parse(parts[3]);

            service.AddTransaction(accountId, date, type, amount);

            var transactions = bank.GetTransactions(accountId);
            Console.WriteLine($"Account: {accountId}");
            Console.WriteLine("| Date     | Txn Id      | Type | Amount |");
            foreach (var txn in transactions)
            {
                var typeShort = txn.Type switch
                {
                    TransactionType.Deposit => "D",
                    TransactionType.Withdrawal => "W",
                    _ => "?"
                };
                Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txn.TxnId,-10} | {typeShort,4} | {txn.Amount,7:F2} |");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

static void HandleInterestRuleInput(InterestRuleService service)
{
    while (true)
    {
        Console.WriteLine("Please enter interest rules details in <Date> <RuleId> <Rate in %> format");
        Console.WriteLine("(or enter blank to go back to main menu):");
        Console.Write("> ");
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;

        try
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3) throw new FormatException("Invalid input format.");

            var date = DateTime.ParseExact(parts[0], "yyyyMMdd", null);
            var ruleId = parts[1];
            var rate = decimal.Parse(parts[2]);

            service.AddInterestRule(date, ruleId, rate);

            Console.WriteLine("Interest rules:");
            Console.WriteLine("| Date     | RuleId | Rate (%) |");
            foreach (var rule in service.GetInterestRules())
            {
                Console.WriteLine($"| {rule.Date:yyyyMMdd} | {rule.RuleId,-6} | {rule.Rate,9:F2} |");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

static void HandleStatementInput(StatementService service)
{
    while (true)
    {
        Console.WriteLine("Please enter account and month to generate the statement <Account> <Year><Month>");
        Console.WriteLine("(or enter blank to go back to main menu):");
        Console.Write("> ");
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line)) return;

        try
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) throw new FormatException("Invalid input format.");

            var accountId = parts[0];
            var yearMonth = parts[1];
            var year = int.Parse(yearMonth.Substring(0, 4));
            var month = int.Parse(yearMonth.Substring(4, 2));

            service.PrintStatement(accountId, year, month);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
