using IdleGarageBackend.Services.Interface;

namespace IdleGarageBackend.Services.Implementation;

public class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}