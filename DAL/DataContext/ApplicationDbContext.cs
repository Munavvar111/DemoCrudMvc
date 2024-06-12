using System;
using System.Collections.Generic;
using DAL.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAL.DataContext;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<OrderStatusLog> OrderStatusLogs { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductPhoto> ProductPhotos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Get the configuration from the appsettings.json file
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("ProductManagementDatabase");

            optionsBuilder.UseNpgsql(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("Category_pkey");

            entity.Property(e => e.CategoryId).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("Customer_pkey");

            entity.Property(e => e.CustomerId).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("Order_pkey");

            entity.Property(e => e.OrderId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Order_CustomerId_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Order_ProductId_fkey");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Order_Status_fkey");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => e.OrderProductId).HasName("OrderProduct_pkey");

            entity.Property(e => e.OrderProductId).UseIdentityAlwaysColumn();
            entity.Property(e => e.PaymentId).HasDefaultValueSql("1");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderProduct_CustomerId_fkey");

            entity.HasOne(d => d.Payment).WithMany(p => p.OrderProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderProduct_PaymentId_fkey");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.StatusId).HasName("OrderStatus_pkey");

            entity.Property(e => e.StatusId).ValueGeneratedNever();
        });

        modelBuilder.Entity<OrderStatusLog>(entity =>
        {
            entity.HasKey(e => e.StatusLogId).HasName("OrderStatusLog_pkey");

            entity.Property(e => e.StatusLogId).UseIdentityAlwaysColumn();
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("Payment_pkey");

            entity.Property(e => e.PaymentId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.PaymentTypeNavigation).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Payment_PaymentType_fkey");
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.HasKey(e => e.PaymentTypeId).HasName("PaymentType_pkey");

            entity.Property(e => e.PaymentTypeId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("product_pkey");

            entity.Property(e => e.ProductId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("product_CategoryId_fkey");
        });

        modelBuilder.Entity<ProductPhoto>(entity =>
        {
            entity.HasKey(e => e.ProductPhotosId).HasName("ProductPhotos_pkey");

            entity.Property(e => e.ProductPhotosId).UseIdentityAlwaysColumn();

            entity.HasOne(d => d.Product).WithMany(p => p.ProductPhotos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ProductPhotos_ProductId_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("User_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
