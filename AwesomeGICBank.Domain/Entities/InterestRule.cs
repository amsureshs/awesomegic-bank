namespace AwesomeGICBank.Domain.Entities;

public class InterestRule(DateTime Date, string RuleId, decimal Rate)
{
    public DateTime Date { get; set; } = Date;
    public string RuleId { get; set; } = RuleId;
    public decimal Rate { get; set; } = Rate;
}
