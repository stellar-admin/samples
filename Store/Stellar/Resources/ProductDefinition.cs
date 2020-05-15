using StellarAdmin.Editors;
using StellarAdmin.EntityFrameworkCore.Resources;
using Store.Models;

namespace Store.Stellar.Resources
{
    public class ProductDefinition : EfCoreResourceDefinition<StoreContext, Product>
    {
        public ProductDefinition(StoreContext dbContext) : base(dbContext)
        {
            HasField(p => p.Id, f =>
            {
                f.IsKey = true;
            });
            HasField(p => p.Name);
            HasOne(p => p.Category, f => f.UseEditor<ReferencedResourceEditor>());
            HasField(p => p.Description, f => f.HideOnList());
        }
    }
}