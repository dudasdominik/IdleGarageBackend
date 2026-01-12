using IdleGarageBackend.Data;
using IdleGarageBackend.Services.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdleGarageBackend.Services.Implementation;

public class UnitOfWork : IdleGarageService, IUnitOfWork
{
    public UnitOfWork(IdleGarageDbContext context) : base(context)
    {
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
      return  _context.SaveChangesAsync(ct);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
       return _context.Database.BeginTransactionAsync(ct);
    }
}