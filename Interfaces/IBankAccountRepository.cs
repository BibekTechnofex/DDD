using Domain.Aggregates;

namespace Domain.Interfaces;

public interface IBankAccountRepository
{
    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task<BankAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BankAccount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string accountNumber, CancellationToken cancellationToken = default);
    Task AddAsync(BankAccount account, CancellationToken cancellationToken = default);
    Task UpdateAsync(BankAccount account, CancellationToken cancellationToken = default);
}
