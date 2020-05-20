using System.Collections.Generic;
using StellarAdmin.EntityFrameworkCore.Resources;
using StellarAdmin.Fields;
using Store.Models;

namespace Store.Stellar.Resources
{
    public class CategoryDefinition : EfCoreResourceDefinition<StoreContext, Category>
    {
        public CategoryDefinition(StoreContext dbContext) : base(dbContext)
        {
            HasDisplay(c => c.Name);
        }

        protected override IEnumerable<IField> CreateFields()
        {
            return new IField[]
            {
                CreateField(c => c.Id,
                    f =>
                    {
                        f.IsKey = true;
                        f.Hide();
                    }),
                CreateField(c => c.Name),
                CreateField(c => c.Description, f => f.HideOnList())
            };
        }
    }
}