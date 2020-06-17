using System.Collections.Generic;
using StellarAdmin.EntityFrameworkCore.Resources;
using StellarAdmin.Fields;
using Store.Models;

namespace Store.Stellar.Resources
{
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using StellarAdmin;

    public class CustomerDefinition : EfCoreResourceDefinition<StoreContext, Customer>
    {
        public CustomerDefinition(StoreContext dbContext) : base(dbContext)
        {
            ConfigureOptions = options =>
            {
                options.Search.Allow = true;
            };
            HasDisplay(c => c.FirstName + " " + c.LastName);
        }


        protected override IQueryable<Customer> ApplySearchFilter(IQueryable<Customer> query, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return query;
            }

            return query.Where(customer => EF.Functions.Like(customer.FirstName, $"%{search}%")
                                           || EF.Functions.Like(customer.LastName, $"%{search}%"));
        }

        protected override IEnumerable<IField> CreateFields()
        {
            return new IField[]
            {
                CreateField(c => c.Id, f =>
                {
                    f.IsKey = true;
                    f.Hide(ViewType.Create, ViewType.Update);
                }),
                CreateField(c => $"{c.FirstName} {c.LastName}", name: "Fullname"),
                CreateField(c => c.FirstName),
                CreateField(c => c.LastName),
                CreateField(c => c.Email),
                CreateField(c => c.StreetAddress, f => f.Hide(ViewType.List)),
                CreateField(c => c.City, f => f.Hide(ViewType.List)),
                CreateField(c => c.Country, f => f.Hide(ViewType.List)),
                CreateField(c => c.PostalCode, f => f.Hide(ViewType.List)),
                CreateField(c => c.Phone, f => f.Hide(ViewType.List)),
            };
        }
    }
}