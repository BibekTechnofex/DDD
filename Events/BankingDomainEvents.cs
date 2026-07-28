using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events;

public sealed record AccountCreatedDomainEvent(
    Guid AccountId,
    string AccountNumber,
    string AccountHolderName,
    Money InitialBalance) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record MoneyDepositedDomainEvent(
    Guid AccountId,
    string AccountNumber,
    Money Amount,
    Money BalanceAfter) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record MoneyWithdrawnDomainEvent(
    Guid AccountId,
    string AccountNumber,
    Money Amount,
    Money BalanceAfter) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record MoneyTransferredDomainEvent(
    Guid SourceAccountId,
    string SourceAccountNumber,
    Guid TargetAccountId,
    string TargetAccountNumber,
    Money Amount) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record AccountClosedDomainEvent(
    Guid AccountId,
    string AccountNumber) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record AccountFrozenDomainEvent(
    Guid AccountId,
    string AccountNumber) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
