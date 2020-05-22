using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Models;

namespace Store
{
    public static class Seeder
    {
        public static IHost SeedData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<StoreContext>();
        
                DoSeedData(context);
            }

            return host;
        }

        private static void DoSeedData(StoreContext context)
        {
            if (!context.Customers.Any())
            {
                var customers = new Faker<Customer>()
                    .RuleFor(c => c.FirstName, f => f.Person.FirstName)
                    .RuleFor(c => c.LastName, f => f.Person.LastName)
                    .RuleFor(c => c.Email, f => f.Person.Email)
                    .RuleFor(c => c.Phone, f => f.Person.Phone)
                    .RuleFor(c => c.StreetAddress, f => f.Address.StreetAddress())
                    .RuleFor(c => c.City, f => f.Address.City())
                    .RuleFor(c => c.Country, f => f.Address.Country())
                    .RuleFor(c => c.PostalCode, f => f.Address.ZipCode())
                    .Generate(50);
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }

            if (!context.Categories.Any())
            {
                var categories = new Faker<Category>()
                    .RuleFor(c => c.Name, f => f.Commerce.Department(1))
                    .RuleFor(c => c.Description, f => f.Lorem.Paragraph())
                    .Generate(10);
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            if (!context.Products.Any())
            {
                var products = new Faker<Product>()
                    .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                    .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                    .RuleFor(p => p.Price, f => f.Random.Number(10, 1000))
                    .RuleFor(p => p.Category, f => f.PickRandom<Category>(context.Categories))
                    .Generate(200);
                context.Products.AddRange(products);
                context.SaveChanges();
            }

            if (!context.Orders.Any())
            {
                var orderItemGenerator = new Faker<OrderItem>()
                    .RuleFor(i => i.Product, f => f.PickRandom<Product>(context.Products))
                    .RuleFor(i => i.Quantity, f => f.Random.Number(1, 5));

                var orders = new Faker<Order>()
                    .RuleFor(o => o.Customer, f => f.PickRandom<Customer>(context.Customers))
                    .RuleFor(o => o.OrderDate, f => f.Date.Past())
                    .RuleFor(o => o.DeliveryDate, (f, o) => f.Date.Soon(60, o.OrderDate).OrNull(f, 0.15F))
                    .RuleFor(o => o.ShipStreetAddress, f => f.Address.StreetAddress())
                    .RuleFor(o => o.ShipCity, f => f.Address.City())
                    .RuleFor(o => o.ShipCountry, f => f.Address.Country())
                    .RuleFor(o => o.ShipPostalCode, f => f.Address.ZipCode())
                    .RuleFor(o => o.OrderItems, () => orderItemGenerator.GenerateBetween(1, 5))
                    .Generate(100);
                context.Orders.AddRange(orders);
                context.SaveChanges();
            }
        }
    }
}