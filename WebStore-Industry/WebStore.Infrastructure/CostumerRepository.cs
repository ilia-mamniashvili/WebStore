using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure;

internal class CostumerRepository : BaseRepository<Application.DTOs.Customer>, ICostumerRepository
{
    public CostumerRepository(StoreDbContext context) : base(context)
    {
    }
}
