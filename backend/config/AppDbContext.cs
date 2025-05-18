using backend.model;
using Microsoft.EntityFrameworkCore;

namespace backend.config
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ProductsModel> Products { get; set; }
        public DbSet<ProductsPriceModel> ProductsPrices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductsModel>(entity =>
            {
                entity.HasKey(e => e.Code);
                entity.Property(e => e.Code).ValueGeneratedOnAdd();
                entity.Property(e => e.CodeProducts).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Barcode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Presentation).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.IsService).IsRequired();
                entity.Property(e => e.IsLote).IsRequired();
                entity.Property(e => e.IsActive).IsRequired();
                entity.Property(e => e.State).IsRequired();
                entity.Property(e => e.User).IsRequired();
                entity.Property(e => e.CreatedAt);
                entity.Property(e => e.UpdatedAt);
                entity.Property(e => e.DeletedAt);

                entity.HasIndex(e => e.CodeProducts);

                entity.HasMany(p => p.Prices)
                      .WithOne(pp => pp.Products)
                      .HasForeignKey(pp => pp.CodeProducts)
                      .HasPrincipalKey(p => p.Code);
            });

            modelBuilder.Entity<ProductsPriceModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CodeProducts).IsRequired();
                entity.Property(e => e.Stock).IsRequired();
                entity.Property(e => e.Lote).HasMaxLength(50);
                entity.Property(e => e.DateLote).HasColumnType("date");
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.HasKey(e => e.Codigo);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsActive).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LasDateEntry).IsRequired().HasColumnType("datetime");
            });
        }
    }
}
