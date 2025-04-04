using AwesomeGICBank.Domain.Entities;
using AwesomeGICBank.Domain.Services;
using AwesomeGICBank.Domain.Enums;

namespace AwesomeGICBank.Tests.Services;

public class BankTests
{

#region Bank Accounts and Transactions
    [Fact]
    public void GetAccount_Should_Create_And_Return_New_Account_If_Not_Exists()
    {
        // Arrange
        var bank = new Bank();
        var accountId = "AC001";

        // Act
        var account = bank.GetAccount(accountId);

        // Assert
        Assert.NotNull(account);
        Assert.Equal(accountId, account.AccountId);
    }

    [Fact]
    public void GetAccount_Should_Return_Existing_Account()
    {
        // Arrange
        var bank = new Bank();
        var accountId = "AC002";

        // Act
        var createAccount = bank.GetAccount(accountId);
        var getAccount = bank.GetAccount(accountId);

        // Assert
        Assert.Same(createAccount, getAccount);
    }

    [Fact]
    public void AddTransaction_Should_Add_To_Correct_Account()
    {
        // Arrange
        var bank = new Bank();
        var accountId = "AC003";

        // Act
        var transaction = new Transaction(DateTime.Today, "20240403-01", TransactionType.Deposit, 100);
        bank.AddTransaction(accountId, transaction);

        var result = bank.GetTransactions(accountId);

        // Assert
        Assert.Single(result);
        Assert.Equal(transaction, result[0]);
    }
#endregion

#region Interest Rules
    [Fact]
    public void AddOrReplaceInterestRule_Should_Replace_If_Same_Date()
    {
        // Arrange
        var bank = new Bank();
        var date = new DateTime(2025, 4, 3);

        // Act
        bank.AddOrReplaceInterestRule(new InterestRule(date, "RULE1", 10.5m));
        bank.AddOrReplaceInterestRule(new InterestRule(date, "RULE2", 9.5m)); // replaces previous

        var rules = bank.GetAllInterestRules();

        // Assert
        Assert.Single(rules);
        Assert.Equal("RULE2", rules[0].RuleId);
    }

    [Fact]
    public void GetApplicableRuleForDate_Should_Return_Closest_Rule_Before_Or_On_Date()
    {
        // Arrange
        var bank = new Bank();
        bank.AddOrReplaceInterestRule(new InterestRule(new DateTime(2025, 1, 1), "RULE3", 1.5m));
        bank.AddOrReplaceInterestRule(new InterestRule(new DateTime(2025, 3, 1), "RULE4", 2.5m));

        // Act
        var result = bank.GetApplicableRuleForDate(new DateTime(2025, 3, 15));

        // Assert
        Assert.NotNull(result);
        Assert.Equal("RULE4", result.RuleId);
    }

    [Fact]
    public void GetApplicableRuleForDate_Should_Return_Null_If_No_Rule_Exist_Before_Or_On_Date()
    {
        // Arrange
        var bank = new Bank();

        // Act
        var result = bank.GetApplicableRuleForDate(new DateTime(2025, 3, 25));

        // Assert
        Assert.Null(result);
    }

#endregion
}
