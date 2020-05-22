﻿using System.Collections.Generic;
using System.Linq;
using StellarAdmin.EntityFrameworkCore.Resources;
using StellarAdmin.Fields;
using Store.Models;

namespace Store.Stellar.Resources
{
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
                        f.Label = "Order date";
                    }),
                CreateField(o => !(o.DeliveryDate == null),
                    f =>
                    {
                        f.Label = "Delivered";
                        f.HideOnForms();
                    }, name: "IsDelivered"),
                CreateField(o => o.DeliveryDate,
                    f =>
                    {
                        f.Label = "Delivery date";
                    }),
                CreateField(o => o.ShipStreetAddress, f =>
                {
                    f.HideOnList();
                    f.Label = "Address";
                }),
                CreateField(o => o.ShipCity, f =>
                {
                    f.HideOnList();
                    f.Label = "City";
                }),
                CreateField(o => o.ShipCountry, f =>
                {
                    f.HideOnList();
                    f.Label = "Country";
                }),
                CreateField(o => o.ShipPostalCode, f =>
                {
                    f.HideOnList();
                    f.Label = "Postal code";
                }),
            };
        }

        protected override IQueryable<Order> GetBaseQuery()
        {
            return base.GetBaseQuery().OrderByDescending(o => o.OrderDate);
        }
    }
}