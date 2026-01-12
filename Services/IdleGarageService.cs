using IdleGarageBackend.Data;

namespace IdleGarageBackend.Services;

public class IdleGarageService
{
    protected readonly IdleGarageDbContext _context;

    public IdleGarageService(IdleGarageDbContext context)
    {
        _context = context;
    }
}