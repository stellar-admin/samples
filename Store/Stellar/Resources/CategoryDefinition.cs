using System.Collections.Generic;
using StellarAdmin.EntityFrameworkCore.Resources;
using StellarAdmin.Fields;
using Store.Models;

namespace Store.Stellar.Resources
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using StellarAdmin;

    public class CategoryDefinition : EfCoreResourceDefinition<StoreContext, Category>
    {
        public CategoryDefinition(StoreContext dbContext) : base(dbContext)
        {
            ConfigureOptions = options =>
            {
                options.Search.Allow = true;
            };
            HasDisplay(c => c.Name);
        }

        protected override IEnumerable<IField> CreateFields()
        {
            return new IField[]
            {
                CreateField(c => c.Id),
                CreateField(c => c.Name, f => f.Sort.Allow = true),
                CreateField(c => c.Description, f =>
                {
                    f.Sort.Allow = true;
                    f.Hide(ViewType.List);
                })
            };
        }

        protected override IQueryable<Category> ApplySearchFilter(IQueryable<Category> query, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return query;
            }

            return query.Where(category => EF.Functions.Like(category.Name, $"%{search}%"));
        }
    }
}