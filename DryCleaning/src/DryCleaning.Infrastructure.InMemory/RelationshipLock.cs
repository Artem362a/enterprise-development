using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Infrastructure.InMemory;

/// <summary>
/// Защита операций со связанными данными в памяти
/// </summary>
public sealed class RelationshipLock : IRelationshipLock, IDisposable
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <inheritdoc />
    public async Task<IDisposable> AcquireAsync()
    {
        await _semaphore.WaitAsync();
        return new Releaser(_semaphore);
    }

    /// <summary>
    /// Освобождает ресурсы блокировки
    /// </summary>
    public void Dispose()
    {
        _semaphore.Dispose();
    }

    private sealed class Releaser(SemaphoreSlim semaphore) : IDisposable
    {
        private SemaphoreSlim? _semaphore = semaphore;

        public void Dispose()
        {
            Interlocked.Exchange(ref _semaphore, null)?.Release();
        }
    }
}
