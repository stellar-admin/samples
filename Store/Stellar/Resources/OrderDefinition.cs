using System.Collections.Generic;
using System.Linq;
using StellarAdmin.EntityFrameworkCore.Resources;
using StellarAdmin.Fields;
using Store.Models;

namespace Store.Stellar.Resources
{
    using StellarAdmin;

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
                CreateReferenceField(o => o.Customer),
                CreateField(o => o.OrderDate,
                    f =>
                    {
                        f.Display.Label = "Order date";
                    }),
                CreateField(o => !(o.DeliveryDate == null),
                    f =>
                    {
                        f.Display.Label = "Delivered";
                        f.Hide(ViewType.Create, ViewType.Update);
                    }, name: "IsDelivered"),
                CreateField(o => o.DeliveryDate,
                    f =>
                    {
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