using Webstore.Infrastructure;
using WebStore.Application.Interfaces.Repositories;

namespace WebStore.Infrastructure;

internal class AdminRepository : BaseRepository<Application.DTOs.Admin>, IAdminRepository
{
    public AdminRepository(StoreDbContext context) : base(context)
    {
    }
}
