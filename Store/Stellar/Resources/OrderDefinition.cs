using System.Collections.Generic;
using System.Linq;
using StellarAdmin.EntityFrameworkCore.Resources;
using StellarAdmin.Fields;
using Store.Models;

namespace Store.Stellar.Resources
{
    using StellarAdmin;
    using StellarAdmin.Resources;

    public class OrderDefinition : EfCoreResourceDefinition<StoreContext, Order>
    {
        public OrderDefinition(StoreContext dbContext) : base(dbContext)
        {
        }

        protected override IEnumerable<IField> CreateFields()
        {
            return new IField[]
            {
                CreateField(o => o.Id),
                CreateReferenceField(o => o.Customer,
                    f =>
                    {
                        f.Sort.Allow = true;
                        f.Sort.Apply = (orders, direction) => direction == SortDirection.Ascending
                            ? orders.OrderBy(o => o.Customer.LastName)
                                .ThenBy(o => o.Customer.FirstName)
                            : orders.OrderByDescending(o => o.Customer.LastName)
                                .ThenByDescending(o => o.Customer.FirstName);
                    }),
                CreateField(o => o.OrderDate,
                    f =>
                    {
                        f.Display.Label = "Order date";
                        f.Sort.Allow = true;
                    }),
                CreateField(o => !(o.DeliveryDate == null),
                    f =>
                    {
                        f.Display.Label = "Delivered";
                        f.Sort.Allow = true;
                        f.Hide(ViewType.Create, ViewType.Update);
                    }, name: "IsDelivered"),
                CreateField(o => o.DeliveryDate,
                    f =>
                    {
                        f.Sort.Allow = true;
                        f.Display.Label = "Delivery date";
                    }),
                CreateField(o => o.ShipStreetAddress, f =>
                {
                    f.Hide(ViewType.List);
                    f.Display.Label = "Address";
                }),
                CreateField(o => o.ShipCity, f =>
                {
                    f.Hide(ViewType.List);
                    f.Display.Label = "City";
                }),
                CreateField(o => o.ShipCountry, f =>
                {
                    f.Hide(ViewType.List);
                    f.Display.Label = "Country";
                }),
                CreateField(o => o.ShipPostalCode, f =>
                {
                    f.Hide(ViewType.List);
                    f.Display.Label = "Postal code";
                }),
            };
        }

        protected override IQueryable<Order> GetBaseQuery()
        {
            return base.GetBaseQuery().OrderByDescending(o => o.OrderDate);
        }
    }
}