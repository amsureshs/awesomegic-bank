# AwesomeGIC Bank

This is a .NET 8 console application that simulates simple banking operations such as adding transactions, defining interest rules, and printing account statements.

---

## 💡 Features

- Input banking transactions (Deposit/Withdrawal)
- Define and manage interest rules
- Print monthly statements including interest
- In-memory only (no database required)
- Fully automated unit testing

---

## 🚀 Technologies

- .NET 8 Console Application
- xUnit for automated testing
- GitHub Actions for Continuous Integration

---

## 🧪 Running Tests

To run all tests:

```bash
dotnet test
```

This will execute:

- Unit tests for all domain services and entity classes

GitHub Actions CI pipeline is already configured in `.github/workflows`. We can also verify test execution via the CI pipeline after each push or manually run.

---

## ▶️ Running the App

To run the console application:

```bash
dotnet run --project AwesomeGICBank
```

---

## 📌 Assumptions

- **Manual dependency creation** is used instead of dependency injection to keep the application simple and focused on functionality
- **Users are allowed to enter transactions and interest rules for past dates**. No validations are enforced on chronological order to maintain ease of use
- **Transactions are stored within each `BankAccount` instance**, allowing easy access and encapsulation. In a real-world application, transactions would likely be separated into a dedicated service or database table, but this approach is chosen here to keep the solution simple and in-memory

---

## 📁 Repository Rules

- ❌ No compiled or executable files included (`*.dll`, `*.exe`, `*.bat`, `*.sh`)
- ✅ Clean, working, minimal code
- ✅ Fully tested using automated testing
- ✅ Simple console-only UI — no external dependencies or database
