using System.Collections.Generic;
using StellarAdmin.EntityFrameworkCore.Resources;
using StellarAdmin.Fields;
using Store.Models;

namespace Store.Stellar.Resources
{
    using StellarAdmin;

    public class ProductDefinition : EfCoreResourceDefinition<StoreContext, Product>
    {
        public ProductDefinition(StoreContext dbContext) : base(dbContext)
        {
        }

        protected override IEnumerable<IField> CreateFields()
        {
            return new IField[]
            {
                CreateField(p => p.Id, f =>
                {
                    f.IsKey = true;
                    f.Hide();
                }),
                CreateField(p => p.Name),
                CreateReferenceField(p => p.Category),
                CreateField(p => p.Description, f => f.Hide(ViewType.List))
            };
        }
    }
}