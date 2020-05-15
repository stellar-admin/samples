using StellarAdmin.EntityFrameworkCore.Resources;
using Store.Models;

namespace Store.Stellar.Resources
{
    public class CustomerDefinition : EfCoreResourceDefinition<StoreContext, Customer>
    {
        public CustomerDefinition(StoreContext dbContext) : base(dbContext)
        {
            HasField(c => c.Id, f => f.IsKey = true);
            HasField(c => c.FirstName);
            HasField(c => c.LastName);
            HasField(c => c.Email);
            HasField(c => c.StreetAddress, f => f.HideOnList());
            HasField(c => c.City, f => f.HideOnList());
            HasField(c => c.Country, f => f.HideOnList());
            HasField(c => c.PostalCode, f => f.HideOnList());
            HasField(c => c.Phone, f => f.HideOnList());
        }
    }
}