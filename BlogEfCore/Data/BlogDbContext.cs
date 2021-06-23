using Microsoft.EntityFrameworkCore;

namespace BlogEfCore.Data
{
    public class BlogDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public BlogDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>(builder =>
            {
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Title)
                    .HasMaxLength(200)
                    .IsRequired();
                builder.Property(p => p.Title);
                builder.HasOne<Author>()
                    .WithMany(a => a.BlogPosts);
            });

            modelBuilder.Entity<Author>(builder =>
            {
                builder.HasKey(a => a.Id);
                builder.Property(a => a.Name)
                    .HasMaxLength(200)
                    .IsRequired();
                builder.Property(a => a.Bio)
                    .IsRequired();
                builder.Property(a => a.Photo)
                    .HasMaxLength(20);
                builder.HasMany(a => a.BlogPosts)
                    .WithOne(b => b.Author);
            });
        }
    }
}