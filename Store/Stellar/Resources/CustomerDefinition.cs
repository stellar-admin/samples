using System.Collections.Generic;
using StellarAdmin.EntityFrameworkCore.Resources;
using StellarAdmin.Fields;
using Store.Models;

namespace Store.Stellar.Resources
{
    public class CustomerDefinition : EfCoreResourceDefinition<StoreContext, Customer>
    {
        public CustomerDefinition(StoreContext dbContext) : base(dbContext)
        {
        }

        protected override IEnumerable<IField> CreateFields()
        {
            return new IField[]
            {
                CreateField(c => c.Id, f =>
                {
                    f.IsKey = true;
                    f.HideOnForms();
                }),
                CreateField(c => $"{c.FirstName} {c.LastName}", name: "Fullname"),
                CreateField(c => c.LastName),
                CreateField(c => c.FirstName),
                CreateField(c => c.LastName),
                CreateField(c => c.Email),
                CreateField(c => c.StreetAddress, f => f.HideOnList()),
                CreateField(c => c.City, f => f.HideOnList()),
                CreateField(c => c.Country, f => f.HideOnList()),
                CreateField(c => c.PostalCode, f => f.HideOnList()),
                CreateField(c => c.Phone, f => f.HideOnList()),
            };
        }
    }
}