using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using Models.Models;
using Microsoft.Extensions.Configuration;

namespace Datas.Data
{
    public class CameraNowContext : IdentityDbContext<AppUser>
    {
        public CameraNowContext(DbContextOptions<CameraNowContext> options) : base (options) { }
        public CameraNowContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("HuyenThuongStore");

            optionsBuilder.UseNpgsql(connectionString)
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddSerilog()));

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Error> Errors { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FeedbackImage> FeedbackImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<UsedCoupon> UsedCoupons { get; set; }

        public DbSet<ProductImage> Images { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PermissionGroups> PermissionGroups { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>(e => e.HasKey(x => new { x.Order_ID, x.Product_ID }));
            modelBuilder.Entity<Product>(e =>
            {
                e.Property(x => x.Buy_Count).HasDefaultValueSql("0");                
            });

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.Product_Id);

            modelBuilder.Entity<Product>()
                .HasOne<ProductCategory>(p => p.ProductCategory)
                .WithMany(p => p.Products)
                .HasForeignKey(x => x.Category_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.HasDefaultSchema("public");

            base.OnModelCreating(modelBuilder);

            // Bỏ tiền tố AspNet của các bảng: mặc định
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            // -----

            // Viết thường tên bảng
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.SetTableName(entity.GetTableName().ToLower());

                // Replace column names
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToLower());
                }

                // Replace key names
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToLower());
                }

                // Replace foreign key names
                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName().ToLower());
                }

                // Replace index names
                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName().ToLower());
                }
            }
        }
    }
}
