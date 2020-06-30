using System.Collections.Generic;
using StellarAdmin.EntityFrameworkCore.Resources;
using StellarAdmin.Fields;
using Store.Models;

namespace Store.Stellar.Resources
{
    using System.Linq;
    using StellarAdmin;
    using StellarAdmin.Resources;

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
                CreateField(p => p.Name, f => f.Sort.Allow = true),
                CreateReferenceField(p => p.Category,
                    f =>
                    {
                        f.Sort.Allow = true;
                        f.Sort.Apply = (products, direction) => direction == SortDirection.Ascending
                            ? products.OrderBy(p => p.Category.Name)
                            : products.OrderByDescending(p => p.Category.Name);
                    }),
                CreateField(p => p.Description, f => f.Hide(ViewType.List))
            };
        }
    }
}