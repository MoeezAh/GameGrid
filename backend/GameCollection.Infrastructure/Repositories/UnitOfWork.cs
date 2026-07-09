using System;
using System.Collections;
using System.Threading.Tasks;
using GameCollection.Domain.Interfaces;
using GameCollection.Infrastructure.Data;

namespace GameCollection.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly GameDbContext _dbContext;
    private Hashtable? _repositories;
    private bool _disposed;

    public UnitOfWork(GameDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        _repositories ??= new Hashtable();

        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(EfRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _dbContext);

            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<T>)_repositories[type]!;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            _disposed = true;
        }
    }
}
