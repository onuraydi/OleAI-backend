using Microsoft.EntityFrameworkCore;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Persistence.Contexts;

namespace OleAI.Backend.Persistence.Repositories
{
    public class ShoppingListItemRepository : EfRepositoryBase<ShoppingListItem, Guid>, IShoppingListItemRepository
    {
        public ShoppingListItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}
