namespace AwesomeGICBank.Domain.Entities;

using AwesomeGICBank.Domain.Enums;

public class Transaction(DateTime Date, string TxnId, TransactionType Type, decimal Amount)
{
    public DateTime Date { get; set; } = Date;
    public string TxnId { get; set; } = TxnId;
    public TransactionType Type { get; set; } = Type;
    public decimal Amount { get; set; } = Amount;
}
