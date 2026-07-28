using Domain.Common;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Aggregates;

public sealed class BankAccount : AggregateRoot
{
    private readonly List<Transaction> _transactions = [];

    public string AccountNumber { get; private set; } = string.Empty;
    public string AccountHolderName { get; private set; } = string.Empty;
    public Money Balance { get; private set; } = Money.Zero();
    public AccountStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    private BankAccount() { }

    public static BankAccount Create(string accountNumber, string accountHolderName, Money initialBalance)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new DomainException("Account number is required.");

        if (string.IsNullOrWhiteSpace(accountHolderName))
            throw new DomainException("Account holder name is required.");

        var account = new BankAccount
        {
            Id = Guid.NewGuid(),
            AccountNumber = accountNumber.Trim(),
            AccountHolderName = accountHolderName.Trim(),
            Balance = initialBalance,
            Status = AccountStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        if (initialBalance.Amount > 0)
        {
            account._transactions.Add(Transaction.Create(
                account.Id,
                TransactionType.Deposit,
                initialBalance,
                initialBalance,
                reference: "Initial deposit"));
        }

        account.RaiseDomainEvent(new AccountCreatedDomainEvent(
            account.Id,
            account.AccountNumber,
            account.AccountHolderName,
            initialBalance));

        return account;
    }

    public void Deposit(Money amount, string? reference = null)
    {
        EnsureActive();
        ValidatePositiveAmount(amount);

        Balance = Balance.Add(amount);
        _transactions.Add(Transaction.Create(Id, TransactionType.Deposit, amount, Balance, reference));

        RaiseDomainEvent(new MoneyDepositedDomainEvent(Id, AccountNumber, amount, Balance));
    }

    public void Withdraw(Money amount, string? reference = null)
    {
        EnsureActive();
        ValidatePositiveAmount(amount);

        Balance = Balance.Subtract(amount);
        _transactions.Add(Transaction.Create(Id, TransactionType.Withdrawal, amount, Balance, reference));

        RaiseDomainEvent(new MoneyWithdrawnDomainEvent(Id, AccountNumber, amount, Balance));
    }

    public void ReceiveTransfer(Money amount, string sourceAccountNumber, string? reference = null)
    {
        EnsureActive();
        ValidatePositiveAmount(amount);

        Balance = Balance.Add(amount);
        _transactions.Add(Transaction.Create(
            Id,
            TransactionType.TransferIn,
            amount,
            Balance,
            reference,
            sourceAccountNumber));

        RaiseDomainEvent(new MoneyDepositedDomainEvent(Id, AccountNumber, amount, Balance));
    }

    public void SendTransfer(Money amount, string targetAccountNumber, string? reference = null)
    {
        EnsureActive();
        ValidatePositiveAmount(amount);

        Balance = Balance.Subtract(amount);
        _transactions.Add(Transaction.Create(
            Id,
            TransactionType.TransferOut,
            amount,
            Balance,
            reference,
            targetAccountNumber));

        RaiseDomainEvent(new MoneyWithdrawnDomainEvent(Id, AccountNumber, amount, Balance));
    }

    public void Freeze()
    {
        if (Status == AccountStatus.Closed)
            throw new DomainException("Cannot freeze a closed account.");

        if (Status == AccountStatus.Frozen)
            throw new DomainException("Account is already frozen.");

        Status = AccountStatus.Frozen;
        RaiseDomainEvent(new AccountFrozenDomainEvent(Id, AccountNumber));
    }

    public void Unfreeze()
    {
        if (Status != AccountStatus.Frozen)
            throw new DomainException("Only frozen accounts can be unfrozen.");

        Status = AccountStatus.Active;
    }

    public void Close()
    {
        if (Status == AccountStatus.Closed)
            throw new DomainException("Account is already closed.");

        if (Balance.Amount != 0)
            throw new DomainException("Account balance must be zero before closing.");

        Status = AccountStatus.Closed;
        RaiseDomainEvent(new AccountClosedDomainEvent(Id, AccountNumber));
    }

    private void EnsureActive()
    {
        if (Status != AccountStatus.Active)
            throw new DomainException("Account is not active.");
    }

    private static void ValidatePositiveAmount(Money amount)
    {
        if (amount.Amount <= 0)
            throw new DomainException("Transaction amount must be greater than zero.");
    }
}
