using System;
using Bookstore.Domain.Addresses;
using Bookstore.Domain.Authors;
using Bookstore.Domain.Books;
using Bookstore.Domain.Carts;
using Bookstore.Domain.Customers;
using Bookstore.Domain.Offers;
using Bookstore.Domain.Orders;
using Bookstore.Domain.Products;
using Bookstore.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data
{
    public partial class ApplicationDbContext : DbContext
    {
        static ApplicationDbContext()
        {
            // Enable legacy timestamp behavior for PostgreSQL compatibility
            // This allows DateTime values to be written to timestamp without time zone columns
            // without strict Kind checking, maintaining compatibility with MS SQL Server behavior
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Address> Address { get; set; }

        public DbSet<Book> Book { get; set; }

        public DbSet<Customer> Customer { get; set; }

        public DbSet<Order> Order { get; set; }

        public DbSet<ShoppingCart> ShoppingCart { get; set; }

        public DbSet<ShoppingCartItem> ShoppingCartItem { get; set; }

        public DbSet<OrderItem> OrderItem { get; set; }

        public DbSet<Offer> Offer { get; set; }

        public DbSet<Author> Author { get; set; }
        
        public DbSet<Product> Product { get; set; }


        public DbSet<ReferenceDataItem> ReferenceData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure table mappings for PostgreSQL
            modelBuilder.Entity<Address>().ToTable("Address", "dbo");
            modelBuilder.Entity<Book>().ToTable("Book", "dbo");
            modelBuilder.Entity<Customer>().ToTable("Customer", "dbo");
            modelBuilder.Entity<Order>().ToTable("Order", "dbo");
            modelBuilder.Entity<ShoppingCart>().ToTable("ShoppingCart", "dbo");
            modelBuilder.Entity<ShoppingCartItem>().ToTable("ShoppingCartItem", "dbo");
            modelBuilder.Entity<OrderItem>().ToTable("OrderItem", "dbo");
            modelBuilder.Entity<Offer>().ToTable("Offer", "dbo");
            modelBuilder.Entity<Author>().ToTable("Author", "dbo");
            modelBuilder.Entity<Product>().ToTable("Product", "dbo");
            modelBuilder.Entity<ReferenceDataItem>().ToTable("ReferenceData", "dbo");

            // Configure boolean to integer conversions for PostgreSQL compatibility
            // PostgreSQL uses NUMERIC(1,0) for bit columns from SQL Server
            modelBuilder.Entity<Address>().Property(e => e.IsActive).HasConversion<int>();
            modelBuilder.Entity<ShoppingCartItem>().Property(e => e.WantToBuy).HasConversion<int>();

            // Configure indexes
            modelBuilder.Entity<Customer>().HasIndex(x => x.Sub).IsUnique();

            // Configure relationships
            modelBuilder.Entity<Book>().HasOne(x => x.Publisher).WithMany().HasForeignKey(x => x.PublisherId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Book>().HasOne(x => x.BookType).WithMany().HasForeignKey(x => x.BookTypeId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Book>().HasOne(x => x.Genre).WithMany().HasForeignKey(x => x.GenreId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Book>().HasOne(x => x.Condition).WithMany().HasForeignKey(x => x.ConditionId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Offer>().HasOne(x => x.Publisher).WithMany().HasForeignKey(x => x.PublisherId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Offer>().HasOne(x => x.BookType).WithMany().HasForeignKey(x => x.BookTypeId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Offer>().HasOne(x => x.Genre).WithMany().HasForeignKey(x => x.GenreId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Offer>().HasOne(x => x.Condition).WithMany().HasForeignKey(x => x.ConditionId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>().HasOne(x => x.Customer).WithMany().OnDelete(DeleteBehavior.Restrict);

            PopulateDatabase(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}