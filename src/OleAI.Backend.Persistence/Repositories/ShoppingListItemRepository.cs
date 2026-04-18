using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;

namespace OleAI.Backend.Persistence.Repositories
{
    public class ShoppingListItemRepository : EfRepositoryBase<ShoppingListItem, Guid>, IShoppingListItemRepository
    {
        public ShoppingListItemRepository(DbContext context) : base(context)
        {
        }
    }
}
