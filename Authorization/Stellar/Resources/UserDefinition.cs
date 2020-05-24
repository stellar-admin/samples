using System.Collections.Generic;
using Authorization.Data;
using Microsoft.AspNetCore.Identity;
using StellarAdmin.EntityFrameworkCore.Resources;
using StellarAdmin.Fields;

namespace Authorization.Stellar.Resources
{
    public class UserDefinition : EfCoreResourceDefinition<ApplicationDbContext, IdentityUser>
    {
        public UserDefinition(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        protected override IEnumerable<IField> CreateFields()
        {
            return new[]
            {
                CreateField(u => u.Id),
                CreateField(u => u.Email),
                CreateField(u => u.TwoFactorEnabled, f => f.HideOnForms())
            };
        }
    }
}