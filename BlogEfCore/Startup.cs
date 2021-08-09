using System;
using System.Linq;
using BlogEfCore.Data;
using BlogEfCore.StellarAdmin.Actions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StellarAdmin;
using StellarAdmin.Editors;
using StellarAdmin.Fields;

namespace BlogEfCore
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
            services.AddControllers();
            services.AddRazorPages().AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<Startup>());
            services.AddDbContext<BlogDbContext>(options =>
            {
                options.UseSqlite("Data Source=blog.db");
            });
            services.AddStellarAdmin(builder =>
            {
                builder.AddEntityResource<BlogDbContext, Author>(rb =>
                {
                    rb.ConfigureDataSource(options =>
                    {
                        options.ApplyFilters = (authors, criteria) =>
                        {
                            if (!string.IsNullOrEmpty(criteria.SearchTerm))
                            {
                                authors = authors.Where(a => EF.Functions.Like(a.Name, $"%{criteria.SearchTerm}%") || EF.Functions.Like(a.Bio, $"%{criteria.SearchTerm}%"));
                            }
                            
                            return authors;
                        };
                    });
                    rb.ConfigureOptions(options =>
                    {
                        options.Search.Allow = true;
                    });
                    rb.HasDisplay(a => a.Name);

                    rb.AddCollection<BlogPost>(cb => cb.UseForeignKey(post => post.AuthorId));
                    
                    rb.AddField(a => a.Name, f => f.AllowSort());
                    rb.AddField(a => a.Photo, f => f.HasEditor<ImageEditor>((editor, context) =>
                    {
                        var request = context.HttpContext.Request;
                            
                        var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
                        editor.UploadUrl = linkGenerator.GetUriByAction(context.HttpContext, "UploadFile", "Uploads");
                        editor.UrlPrefix = $"{request.Scheme}://{request.Host}{request.PathBase}/uploads/images/";
                        editor.AltText = "Photo of author";
                    }));
                    rb.AddField(a => a.Bio, f => f.HasEditor<TextAreaEditor>());
                });
                builder.AddEntityResource<BlogDbContext, BlogPost>(rb =>
                {
                    rb.ConfigureDataSource(options =>
                    {
                        options.ApplyFilters = (blogPosts, criteria) =>
                        {
                            if (!string.IsNullOrEmpty(criteria.SearchTerm))
                            {
                                blogPosts = blogPosts.Where(b => EF.Functions.Like(b.Title, $"%{criteria.SearchTerm}%"));
                            }

                            return blogPosts;
                        };
                    });
                    rb.ConfigureOptions(options =>
                    {
                        options.Search.Allow = true;
                    });

                    rb.AddField(b => b.Title, f => f.AllowSort());
                    rb.AddField(b => b.Author,
                        f => f.AllowSort((posts, direction) => direction == SortDirection.Ascending
                            ? posts.OrderBy(p => p.Author.Name)
                            : posts.OrderByDescending(p => p.Author.Name)));
                    rb.AddField(b => b.Content, f =>
                    {
                        f.HasEditor<MarkdownEditor>((editor, context) =>
                        {
                            var linkGenerator = context.RequestServices.GetRequiredService<LinkGenerator>();
                            editor.ImageUploadUrl = linkGenerator.GetUriByAction(context.HttpContext, "UploadMarkdownEditorImage", "Uploads");
                        });
                    });

                    rb.AddSection(pb =>
                    {
                        pb.HasTitle("Publication Information");
                        pb.HasSubTitle("Various bits and pieces related to the blog post's publication");
                        pb.AddField(b => b.PublishDate > DateTime.Now,
                            f =>
                            {
                                f.IsVisible(ctx => ctx.View?.EditorKind != EditorKind.Form);
                                f.HasName("IsPublished");
                            });
                        pb.AddField(b => b.PublishDate,
                            f =>
                            {
                                f.AllowSort();
                                f.IsReadOnly();
                            });
                    });
                    
                    rb.AddAction<PublishBlogPostAction, PublishBlogPostModel>("Publish", a =>
                    {
                        a.HasDialogTitle("Publish Blog Post");
                        a.HasDialogTitle("Specify the date on which the blog post must be published");
                        a.HasConfirmActionText("Publish");
                        
                        a.AddField(m => m.PublishDate);
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
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                
                endpoints.MapStellarAdmin();
            });
        }
    }
}