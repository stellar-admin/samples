using StellarAdmin.EntityFrameworkCore.Resources;
using Store.Models;

namespace Store.Stellar.Resources
{
    public class CategoryDefinition : EfCoreResourceDefinition<StoreContext, Category>
    {
        public CategoryDefinition(StoreContext dbContext) : base(dbContext)
        {
            HasDisplay(c => c.Name);
        }
    }
}