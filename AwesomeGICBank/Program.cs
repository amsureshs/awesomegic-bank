using System;
using AwesomeGICBank.Domain.Entities;
using AwesomeGICBank.Domain.Enums;
using AwesomeGICBank.Domain.Services;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var bank = new Bank();
var interestRuleService = new InterestRuleService(bank);
var statementService = new StatementService(bank, interestRuleService);

var txService = new TransactionService(bank);

// Add transactions
txService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'D', 500.00m);
txService.AddTransaction("AC001", new DateTime(2023, 6, 27), 'D', 200.00m);
txService.AddTransaction("AC001", new DateTime(2023, 6, 29), 'W', 100.00m);
txService.AddTransaction("AC001", new DateTime(2023, 7, 26), 'D', 300.00m);
txService.AddTransaction("AC001", new DateTime(2023, 7, 29), 'D', 100.00m);
txService.AddTransaction("AC001", new DateTime(2023, 7, 30), 'W', 400.00m);

// txService.AddTransaction("AC001", new DateTime(2023, 6, 1), 'D', 250.00m);
// txService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'W', 20.00m);
// txService.AddTransaction("AC001", new DateTime(2023, 6, 26), 'W', 100.00m);

// Add interest rule
// interestRuleService.AddInterestRule(new DateTime(2023, 6, 1), "RULE01", 1.90m);
interestRuleService.AddInterestRule(new DateTime(2023, 6, 15), "RULE02", 2.20m);

// Print statements
statementService.PrintStatement("AC001", 2023, 6);
statementService.PrintStatement("AC001", 2023, 7);
