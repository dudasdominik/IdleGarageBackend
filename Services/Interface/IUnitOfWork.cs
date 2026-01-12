using Microsoft.EntityFrameworkCore.Storage;

namespace IdleGarageBackend.Services.Interface;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
}