using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Project_SalesOrder.Models
{
    public class MyDatabaseContext : DbContext
    {
        public MyDatabaseContext() : base("MyDatabaseContext")
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Pemetaan tabel
            modelBuilder.Entity<Customer>().ToTable("COM_CUSTOMER");
            modelBuilder.Entity<Order>().ToTable("SO_ORDER");
            modelBuilder.Entity<Item>().ToTable("SO_ITEM");

            // Relasi antara tabel
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.COM_CUSTOMER_ID);

            modelBuilder.Entity<Item>()
                .HasRequired(i => i.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.SO_ORDER_ID);

            modelBuilder.Entity<Order>()
                .Property(o => o.ORDER_DATE)
                .HasColumnType("datetime");
        }
    }
}