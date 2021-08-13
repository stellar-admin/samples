using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StellarAdmin;
using StellarAdmin.Fields;
using StellarAdmin.Views;
using Store.Models;

namespace Store
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StoreContext>(builder =>
                builder.UseSqlite("Data Source=store.db"));
            services.AddRazorPages();
            
            services.AddStellarAdmin(builder =>
            {
                builder.AddEntityResource<StoreContext, Category>(rb =>
                {
                    rb.ConfigureOptions(options => options.Search.Allow = true);
                    rb.ConfigureDataSource(options =>
                    {
                        options.ApplyFilters = (categories, criteria) =>
                        {
                            if (!string.IsNullOrEmpty(criteria.SearchTerm))
                            {
                                categories = categories.Where(category => EF.Functions.Like(category.Name, $"%{criteria.SearchTerm}%"));
                            }

                            return categories;
                        };
                    });
                    rb.HasTitle(c => c.Name);
                    
                    rb.AddCollection<Product>();
                    
                    rb.AddField(c => c.Name, f => f.AllowSort());
                    rb.AddField(c => c.Description,
                        f =>
                        {
                            f.AllowSort();
                            f.IsVisible(context => context.View != View.ResourceIndexView);
                        });
                });
                builder.AddEntityResource<StoreContext, Customer>(rb =>
                {
                    rb.ConfigureOptions(options => options.Search.Allow = true);
                    rb.ConfigureDataSource(options =>
                    {
                        options.ApplyFilters = (customers, criteria) =>
                        {
                            if (!string.IsNullOrEmpty(criteria.SearchTerm))
                            {
                                customers = customers.Where(customer => EF.Functions.Like(customer.FirstName, $"%{criteria.SearchTerm}%")
                                                                        || EF.Functions.Like(customer.LastName, $"%{criteria.SearchTerm}%"));
                            }

                            switch (criteria.Segment)
                            {
                                case "with-orders":
                                    customers = customers.Where(c => c.Orders.Any());
                                    break;
                                case "without-orders":
                                    customers = customers.Where(c => !c.Orders.Any());
                                    break;
                            }

                            return customers;
                        };
                    });
                    rb.HasTitle(c => c.FirstName + " " + c.LastName);
                    
                    rb.AddSegment("with-orders", "With orders");
                    rb.AddSegment("without-orders", "Without orders");
                    
                    rb.AddCollection<Order>();

                    rb.AddField(c => $"{c.FirstName} {c.LastName}", f => f.HasName("Fullname"));
                    rb.AddField(c => c.FirstName, f => f.AllowSort());
                    rb.AddField(c => c.LastName, f => f.AllowSort());
                    rb.AddField(c => c.Email, f => f.AllowSort());
                    rb.AddField(c => c.StreetAddress, f => f.IsVisible(context => context.View != View.ResourceIndexView));
                    rb.AddField(c => c.City, f => f.IsVisible(context => context.View != View.ResourceIndexView));
                    rb.AddField(c => c.Country, f => f.IsVisible(context => context.View != View.ResourceIndexView));
                    rb.AddField(c => c.PostalCode, f => f.IsVisible(context => context.View != View.ResourceIndexView));
                    rb.AddField(c => c.Phone, f => f.IsVisible(context => context.View != View.ResourceIndexView));
                });
                builder.AddEntityResource<StoreContext, Product>(rb =>
                {
                    rb.AddField(s => s.Name, f => f.AllowSort());
                    rb.AddField(s => s.Category, f => f.AllowSort((products, direction) =>
                    {
                        return direction == SortDirection.Ascending
                            ? products.OrderBy(p => p.Category.Name)
                            : products.OrderByDescending(p => p.Category.Name);
                    }));
                    rb.AddField(s => s.Description, f => f.IsVisible(context => context.View != View.ResourceIndexView));
                });
                builder.AddEntityResource<StoreContext, Order>(rb =>
                {
                    rb.ConfigureDataSource(options =>
                    {
                        options.GetBaseQuery = orders => orders.OrderByDescending(o => o.OrderDate);
                        options.ApplyFilters = (orders, criteria) =>
                        {
                            switch (criteria.Segment)
                            {
                                case "undelivered":
                                    orders = orders.Where(o => o.DeliveryDate == null);
                                    break;
                            }
                            return orders;
                        };
                    });
                    
                    rb.AddSegment("undelivered", "Undelivered");

                    rb.AddField(o => o.Id);
                    rb.AddField(o => o.Customer,
                        f =>
                        {
                            f.AllowSort((orders, direction) => direction == SortDirection.Ascending
                                ? orders.OrderBy(o => o.Customer.LastName)
                                    .ThenBy(o => o.Customer.FirstName)
                                : orders.OrderByDescending(o => o.Customer.LastName)
                                    .ThenByDescending(o => o.Customer.FirstName));
                        });
                    rb.AddField(o => o.OrderDate,
                        f =>
                        {
                            f.HasLabel("Order date");
                            f.AllowSort();
                        });
                    rb.AddSection(panel =>
                    {
                        panel.HasTitle("Delivery Status");
                        panel.AddField(o => !(o.DeliveryDate == null),
                            f =>
                            {
                                f.HasName("IsDelivered");
                                f.AllowSort();
                                f.HasLabel("Delivered");
                                f.IsVisible(context => context.View != View.ResourceCreateView && context.View != View.ResourceUpdateView);
                            });
                        panel.AddField(o => o.DeliveryDate,
                            f =>
                            {
                                f.AllowSort();
                                f.HasLabel("Delivery date");
                            });
                    });
                    rb.AddSection(panel =>
                    {
                        panel.HasTitle("Shipping address");
                        panel.HasSubTitle("The addressed to which this order is shipped");
                        panel.IsVisible(context => context.View != View.ResourceIndexView);
                        
                        panel.AddField(o => o.ShipStreetAddress, f => f.HasLabel("Address"));
                        panel.AddField(o => o.ShipCity, f => f.HasLabel("City"));
                        panel.AddField(o => o.ShipCountry, f => f.HasLabel("Country"));
                        panel.AddField(o => o.ShipPostalCode, f => f.HasLabel("Postal code"));
                    });
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                
                endpoints.MapStellarAdmin();
            });
        }
    }
}