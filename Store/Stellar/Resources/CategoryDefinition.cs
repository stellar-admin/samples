using StellarAdmin.EntityFrameworkCore.Resources;
using Store.Models;

namespace Store.Stellar.Resources
{
    public class CategoryDefinition : EfCoreResourceDefinition<StoreContext, Category>
    {
        public CategoryDefinition(StoreContext dbContext) : base(dbContext)
        {
            HasField(c => c.Id, f => f.IsKey = true);
            HasField(c => c.Name);
            HasField(c => c.Description, f => f.HideOnList());
            
            HasDisplay(c => c.Name);
        }
    }
}