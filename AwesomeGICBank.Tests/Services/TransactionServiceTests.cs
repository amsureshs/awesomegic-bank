using AwesomeGICBank.Domain.Services;
using AwesomeGICBank.Domain.Enums;

namespace AwesomeGICBank.Tests.Services;

public class TransactionServiceTests
{
    [Fact]
    public void AddTransaction_Valid_Deposit_Should_Succeed()
    {
        // Arrange
        var bank = new Bank();
        var service = new TransactionService(bank);

        // Act
        service.AddTransaction("AC001", new DateTime(2025, 4, 4), 'D', 100.00m);

        // Assert
        var transactions = bank.GetTransactions("AC001");
        Assert.Single(transactions);
        Assert.Equal("20250404-01", transactions[0].TxnId);
        Assert.Equal(TransactionType.Deposit, transactions[0].Type);
        Assert.Equal(100.00m, transactions[0].Amount);
    }

    [Fact]
    public void AddTransaction_Valid_Withdrawal_Should_Succeed()
    {
        // Arrange
        var bank = new Bank();
        var service = new TransactionService(bank);
        var date = new DateTime(2025, 4, 4);

        // Act
        service.AddTransaction("AC002", date, 'D', 100.00m);
        service.AddTransaction("AC002", date, 'W', 50.00m);

        // Assert
        var transactions = bank.GetTransactions("AC002");
        Assert.Equal("20250404-02", transactions[1].TxnId);
        Assert.Equal(TransactionType.Withdrawal, transactions[1].Type);
    }

    [Fact]
    public void AddTransaction_Withdrawal_First_Should_Fail()
    {
        // Arrange
        var bank = new Bank();
        var service = new TransactionService(bank);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            service.AddTransaction("AC003", DateTime.Today, 'W', 10.00m));

        Assert.Contains("first transaction should be a Deposit", ex.Message);
    }

    [Fact]
    public void AddTransaction_Withdrawal_With_Lower_Balance_Should_Fail()
    {
        // Arrange
        var bank = new Bank();
        var service = new TransactionService(bank);
        var date = DateTime.Today;

        service.AddTransaction("AC004", date, 'D', 10.00m);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            service.AddTransaction("AC004", date, 'W', 20.00m));

        Assert.Contains("Insufficient balance", ex.Message);
    }

    [Fact]
    public void AddTransaction_Invalid_AccountId_Empty_Text_Should_Fail()
    {
        // Arrange
        var bank = new Bank();
        var service = new TransactionService(bank);
        var date = DateTime.Today;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddTransaction("", date, 'D', 20.00m));

        Assert.Contains("provide valid account", ex.Message);
    }

    [Fact]
    public void AddTransaction_Invalid_TransactionType_Should_Fail()
    {
        // Arrange
        var service = new TransactionService(new Bank());

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddTransaction("AC005", DateTime.Today, 'A', 10.00m));

        Assert.Contains("transaction type is invalid", ex.Message);
    }

    [Theory]
    [InlineData(-10)] // less than zero
    [InlineData(0)] // equal zero
    [InlineData(1000000000.00)] // more than the MaxDepositAmount
    public void AddTransaction_Invalid_Amount_Should_Fail(decimal amount)
    {
        // Arrange
        var service = new TransactionService(new Bank());

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            service.AddTransaction("AC006", DateTime.Today, 'D', amount));

        Assert.Contains("must be greater than zero and up to", ex.Message);
    }

    [Fact]
    public void GenerateTransactionId_Multiple_For_Day_Should_Succeed()
    {
        // Arrange
        var bank = new Bank();
        var service = new TransactionService(bank);
        var accountId = "AC007";
        var date = new DateTime(2025, 4, 4);

        // Act
        service.AddTransaction(accountId, date, 'D', 30);
        service.AddTransaction(accountId, date, 'W', 20);
        service.AddTransaction(accountId, date, 'D', 30);

        // Assert
        var txns = bank.GetTransactions(accountId);
        Assert.Equal("20250404-01", txns[0].TxnId);
        Assert.Equal("20250404-02", txns[1].TxnId);
        Assert.Equal("20250404-03", txns[2].TxnId);
    }
}
