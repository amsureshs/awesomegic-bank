
using AwesomeGICBank.Domain.Entities;
using AwesomeGICBank.Domain.Enums;

namespace AwesomeGICBank.Tests.Entities
{
    public class BankAccountTests
    {
        [Fact]
        public void GetCurrentBalance_Should_CalculateCorrectly()
        {
            // Arrange
            var account = new BankAccount("AC002");
            account.AddTransaction(new Transaction(new DateTime(2023, 6, 1), "01", TransactionType.Deposit, 200m));
            account.AddTransaction(new Transaction(new DateTime(2023, 6, 2), "02", TransactionType.Withdrawal, 50m));
            account.AddTransaction(new Transaction(new DateTime(2023, 6, 3), "03", TransactionType.Deposit, 150m));
            account.AddTransaction(new Transaction(new DateTime(2023, 6, 4), "04", TransactionType.Withdrawal, 100m));

            // Act
            var balance = account.GetCurrentBalance();

            // Assert
            Assert.Equal(200m, balance);
        }

        [Fact]
        public void GetTransactionsOfMonth_Should_ReturnOnlyMatchingMonth()
        {
            // Arrange
            var account = new BankAccount("AC003");
            account.AddTransaction(new Transaction(new DateTime(2023, 5, 1), "01", TransactionType.Deposit, 100m));
            account.AddTransaction(new Transaction(new DateTime(2023, 6, 10), "02", TransactionType.Withdrawal, 50m));
            account.AddTransaction(new Transaction(new DateTime(2023, 6, 15), "03", TransactionType.Deposit, 70m));

            // Act
            var juneTxns = account.GetTransactionsOfMonth(2023, 6);

            // Assert
            Assert.Equal(2, juneTxns.Count);
            Assert.All(juneTxns, t => Assert.Equal(6, t.Date.Month));
        }

        [Fact]
        public void GetBalanceUntilDate_Should_RespectDateLimit()
        {
            // Arrange
            var account = new BankAccount("AC004");
            account.AddTransaction(new Transaction(new DateTime(2023, 6, 1), "01", TransactionType.Deposit, 200m));
            account.AddTransaction(new Transaction(new DateTime(2023, 6, 5), "02", TransactionType.Withdrawal, 80m));
            account.AddTransaction(new Transaction(new DateTime(2023, 6, 20), "03", TransactionType.Deposit, 30m));

            // Act
            var balanceUpTo10th = account.GetBalanceUntilDate(new DateTime(2023, 6, 10));

            // Assert
            Assert.Equal(120m, balanceUpTo10th); // 200 - 80 = 120 (third txn ignored)
        }
    }
}
