using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Transaction : Entity
{
    public Guid BankAccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public Money Amount { get; private set; } = Money.Zero();
    public Money BalanceAfter { get; private set; } = Money.Zero();
    public string? Reference { get; private set; }
    public string? CounterpartyAccountNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() { }

    internal static Transaction Create(
        Guid bankAccountId,
        TransactionType type,
        Money amount,
        Money balanceAfter,
        string? reference = null,
        string? counterpartyAccountNumber = null)
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            BankAccountId = bankAccountId,
            Type = type,
            Amount = amount,
            BalanceAfter = balanceAfter,
            Reference = reference,
            CounterpartyAccountNumber = counterpartyAccountNumber,
            CreatedAt = DateTime.UtcNow
        };
    }
}
