using Microsoft.EntityFrameworkCore.Storage;
using Webstore.Infrastructure;
using WebStore.Application.DTOs;
using WebStore.Application.Interfaces.Repositories;


namespace WebStore.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly StoreDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    private readonly Lazy<IAdminRepository> _Admin;
    private readonly Lazy<ICartRepository> _Cart;
    private readonly Lazy<ICategoryRepository> _Category;
    private readonly Lazy<ICostumerRepository> _Custumer;
    private readonly Lazy<IOrderRepository> _Order;
    private readonly Lazy<IProductRepository> _Product;
    private readonly Lazy<IUserRepository> _User;

    public IAdminRepository Admin => CheckDisposedAndGet(_Admin);
    public ICartRepository Cart => CheckDisposedAndGet(_Cart);
    public ICategoryRepository Category => CheckDisposedAndGet(_Category);
    public ICostumerRepository Customer => CheckDisposedAndGet(_Custumer);
    public IOrderRepository Order => CheckDisposedAndGet(_Order);
    public IProductRepository Product => CheckDisposedAndGet(_Product);
    public IUserRepository User => CheckDisposedAndGet(_User);

    public UnitOfWork(StoreDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));

        _Admin = new Lazy<IAdminRepository>(() => new AdminRepository(_context));
        _Cart = new Lazy<ICartRepository>(() => new CartRepository(_context));
        _Category = new Lazy<ICategoryRepository>(() => new CategoryRepository(_context));
        _Custumer = new Lazy<ICostumerRepository>(() => new CostumerRepository(_context));
        _Order = new Lazy<IOrderRepository>(() => new OrderRepository(_context));
        _Product = new Lazy<IProductRepository>(() => new ProductRepository(_context));
        _User = new Lazy<IUserRepository>(() => new UserRepository(_context));
    }

    public int SaveChanges()
    {
        ThrowIfDisposed();
        return _context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void BeginTransaction()
    {
        ThrowIfDisposed();
        if (_transaction != null)
            throw new ArgumentException("Transaction has already started");

        _transaction = _context.Database.BeginTransaction();
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        if (_transaction != null)
            throw new ArgumentException("Transaction has already started");

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

    }

    public void Commit()
    {
        ThrowIfDisposed();
        if (_transaction == null)
            throw new ArgumentException("Transaction has not started");

        _transaction?.Commit();
        _transaction?.Dispose();
        _transaction = null;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        if (_transaction == null)
            throw new ArgumentException("Transaction has not started");

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;

    }

    public void Rollback()
    {
        ThrowIfDisposed();
        if (_transaction == null)
            throw new ArgumentException("Transaction has not started");

        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        if (_transaction == null)
            throw new ArgumentException("Transaction has not started");

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;

    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    private T CheckDisposedAndGet<T>(Lazy<T> lazy)
    {
        ThrowIfDisposed();
        return lazy.Value;
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            if (_transaction != null)
            {
                _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            _disposed = true;
        }
    }

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this.GetType());

    ~UnitOfWork() => Dispose(false);
}