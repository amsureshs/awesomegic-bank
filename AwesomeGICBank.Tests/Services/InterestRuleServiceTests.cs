using AwesomeGICBank.Domain.Enums;
using AwesomeGICBank.Domain.Services;

namespace AwesomeGICBank.Tests.Services;

public class InterestRuleServiceTests
{
    [Fact]
    public void AddInterestRule_Should_Add_New_Rule()
    {
        // Arrange
        var bank = new Bank();
        var service = new InterestRuleService(bank);
        var date = new DateTime(2023, 6, 15);

        // Act
        service.AddInterestRule(date, "RULE01", 1.9m);

        // Assert
        var rules = service.GetInterestRules();
        Assert.Single(rules);
        Assert.Equal("RULE01", rules[0].RuleId);
        Assert.Equal(1.9m, rules[0].Rate);
    }

    [Fact]
    public void AddInterestRule_Should_Throw_For_Empty_RuleId()
    {
        // Arrange
        var service = new InterestRuleService(new Bank());

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.AddInterestRule(DateTime.Today, "", 2.0m));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(150)]
    public void AddInterestRule_Should_Throw_For_Invalid_Rate(decimal rate)
    {
        // Arrange
        var service = new InterestRuleService(new Bank());

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            service.AddInterestRule(DateTime.Today, "RULEX", rate));
    }

    [Fact]
    public void CalculateInterest_Should_Return_Empty_If_No_Transactions()
    {
        // Arrange
        var bank = new Bank();
        bank.GetAccount("AC001"); // Create account but no transactions
        var service = new InterestRuleService(bank);

        // Act
        var result = service.CalculateInterest("AC001", 2023, 6);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void CalculateInterest_Should_Skip_If_No_Transactions_In_That_Month()
    {
        // Arrange
        var bank = new Bank();
        var txService = new TransactionService(bank);
        var ruleService = new InterestRuleService(bank);

        // Add a transaction in May
        txService.AddTransaction("AC002", new DateTime(2023, 5, 15), 'D', 100m);
        ruleService.AddInterestRule(new DateTime(2023, 5, 1), "RULE_MAY", 2.0m);

        // Act
        var result = ruleService.CalculateInterest("AC002", 2023, 6);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void CalculateInterest_Should_Include_Interest_Txn_At_Month_End()
    {
        // Arrange
        var bank = new Bank();
        var txService = new TransactionService(bank);
        var ruleService = new InterestRuleService(bank);

        var accountId = "AC003";

        txService.AddTransaction(accountId, new DateTime(2023, 6, 26), 'D', 100m);
        txService.AddTransaction(accountId, new DateTime(2023, 6, 27), 'D', 100m);
        txService.AddTransaction(accountId, new DateTime(2023, 6, 29), 'W', 100m);

        ruleService.AddInterestRule(new DateTime(2023, 6, 15), "RULE03", 2.00m);

        // Act
        var result = ruleService.CalculateInterest(accountId, 2023, 6);

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains(result, t => t.Type == TransactionType.Interest);
        var interestTxn = result.Last();
        Assert.Equal(TransactionType.Interest, interestTxn.Type);
        Assert.Equal(new DateTime(2023, 6, 30), interestTxn.Date);
        Assert.True(interestTxn.Amount > 0);
    }

    [Fact]
    public void CalculateInterest_Should_Calculate_Correct_Value_With_2Percent_Annually()
    {
        // Arrange
        var bank = new Bank();
        var txService = new TransactionService(bank);
        var ruleService = new InterestRuleService(bank);
        var accountId = "AC004";

        // Deposit 100 on June 1
        txService.AddTransaction(accountId, new DateTime(2023, 6, 1), 'D', 100m);

        // Add interest rule from June 1
        ruleService.AddInterestRule(new DateTime(2023, 6, 1), "RULE02", 2.00m);

        // Act
        var result = ruleService.CalculateInterest(accountId, 2023, 6);

        // Assert
        var interestTxn = result.LastOrDefault(t => t.Type == TransactionType.Interest);
        Assert.NotNull(interestTxn);
        Assert.Equal(new DateTime(2023, 6, 30), interestTxn!.Date);
        Assert.Equal(0.16m, interestTxn.Amount); // 100 * 2% / 365 * 30 ≈ 0.164
    }

}
