using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure;

internal class ProductRepository : BaseRepository<Application.DTOs.Product>, IProductRepository
{
    public ProductRepository(StoreDbContext context) : base(context)
    {
    }
}
