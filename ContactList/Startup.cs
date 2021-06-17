using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactList.Data;
using ContactList.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StellarAdmin.Helpers;
using StellarAdmin.Resources;
using StellarAdmin.Validation;

namespace ContactList
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
                .AddFluentValidation(configuration =>
                    configuration.RegisterValidatorsFromAssemblyContaining<ContactValidator>());
            
            services.AddSingleton(_ => ContactDatabase.Create());
            services.AddStellarAdmin(builder =>
            {
                builder.AddResource<Contact>(rb =>
                {
                    rb.ConfigureOptions(options => options.Search.Allow = true);

                    rb.AddField(contact => contact.Id);
                    rb.AddField(contact => contact.FirstName, f => f.AllowSort());
                    rb.AddField(contact => contact.LastName, f => f.AllowSort());
                    rb.AddField(contact => contact.EmailAddress, f => f.AllowSort());
                    rb.AddField(contact => contact.PhoneNumber, f => f.AllowSort());
                    rb.AddField(contact => contact.Type, f => f.AllowSort());

                    rb.UseDataSource(options =>
                    {
                        options.OnCreate = request =>
                        {
                            var contactDatabase = request.RequestContext.RequestServices.GetRequiredService<ContactDatabase>();
                            var contact = new Contact { Id = Guid.NewGuid() };

                            // Apply form values
                            FormHelpers.SetResourceValues(contact, request.Values, request.Fields, request.RequestContext);

                            // Validate the resource
                            var validationResult = FormHelpers.ValidateResource(contact, request.RequestContext);

                            // Add to the list if it is valid
                            if (validationResult.IsValid)
                            {
                                contactDatabase.Contacts.Add(contact);
                            }

                            return Task.FromResult(validationResult);
                        };
                        options.OnDelete = request =>
                        {
                            var contactDatabase = request.RequestContext.RequestServices.GetRequiredService<ContactDatabase>();
                            var contact = contactDatabase.Contacts.FirstOrDefault(c => c.Id == new Guid(request.Key.ToString()));

                            if (contact != null)
                            {
                                contactDatabase.Contacts.Remove(contact);
                            }

                            return Task.CompletedTask;
                        };
                        options.OnUpdate = request =>
                        {
                            var contactDatabase = request.RequestContext.RequestServices.GetRequiredService<ContactDatabase>();
                            var contact = contactDatabase.Contacts.FirstOrDefault(c => c.Id == new Guid(request.Key.ToString()));
                            if (contact != null)
                            {
                                // Update the values
                                FormHelpers.SetResourceValues(contact, request.Values, request.Fields, request.RequestContext);

                                // Perform validation
                                return Task.FromResult(FormHelpers.ValidateResource(contact, request.RequestContext));
                            }

                            return Task.FromResult(new ValidationResult());
                        };
                        options.OnGetList = request =>
                        {
                            var contactDatabase = request.RequestContext.RequestServices.GetRequiredService<ContactDatabase>();
                            var skip = (request.Query.PageNo - 1) * request.Query.PageSize;
                            var take = request.Query.PageSize;

                            IEnumerable<Contact> contacts = contactDatabase.Contacts;

                            // Apply search criteria
                            if (!string.IsNullOrEmpty(request.Query.Search))
                                contacts = contacts.Where(c =>
                                    c.FirstName.Contains(request.Query.Search, StringComparison.CurrentCultureIgnoreCase)
                                    || c.LastName.Contains(request.Query.Search, StringComparison.CurrentCultureIgnoreCase));
                            var count = contacts.Count();

                            // Apply sort criteria
                            contacts = (request.Query.OrderByField, request.Query.OrderByDirection) switch
                            {
                                ("firstName", SortDirection.Ascending) => contacts.OrderBy(contact => contact.FirstName),
                                ("firstName", SortDirection.Descending) => contacts.OrderByDescending(contact => contact.FirstName),
                                ("lastName", SortDirection.Ascending) => contacts.OrderBy(contact => contact.LastName),
                                ("lastName", SortDirection.Descending) => contacts.OrderByDescending(contact => contact.LastName),
                                ("emailAddress", SortDirection.Ascending) => contacts.OrderBy(contact => contact.EmailAddress),
                                ("emailAddress", SortDirection.Descending) => contacts.OrderByDescending(contact => contact.EmailAddress),
                                ("phoneNumber", SortDirection.Ascending) => contacts.OrderBy(contact => contact.PhoneNumber),
                                ("phoneNumber", SortDirection.Descending) => contacts.OrderByDescending(contact => contact.PhoneNumber),
                                ("type", SortDirection.Ascending) => contacts.OrderBy(contact => contact.Type),
                                ("type", SortDirection.Descending) => contacts.OrderByDescending(contact => contact.Type),
                                _ => contacts
                            };

                            // Apply paging
                            contacts = contacts.Skip(skip)
                                .Take(take);

                            var pagedContactList = new PagedResourceList(contacts, request.Query.PageNo, request.Query.PageSize, count);

                            return Task.FromResult(pagedContactList);
                        };
                        options.OnGetSingle = request =>
                        {
                            var contactDatabase = request.RequestContext.RequestServices.GetRequiredService<ContactDatabase>();
                            var contact = contactDatabase.Contacts.FirstOrDefault(c => c.Id == new Guid(request.Key.ToString()));

                            return Task.FromResult(contact);
                        };
                    });
                });
            });
        }

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