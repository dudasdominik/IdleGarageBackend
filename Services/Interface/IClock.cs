namespace IdleGarageBackend.Services.Interface;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}