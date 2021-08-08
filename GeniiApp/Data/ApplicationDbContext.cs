using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeniiApp.Models;

namespace GeniiApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductInvoice>()
                .HasKey(bc => new { bc.ProductId, bc.InvoiceId });

            modelBuilder.Entity<ProductInvoice>()
                .HasOne(bc => bc.Product)
                .WithMany(b => b.ProductInvoices)
                .HasForeignKey(bc => bc.ProductId);

            modelBuilder.Entity<ProductInvoice>()
                .HasOne(bc => bc.Invoice)
                .WithMany(c => c.ProductInvoices)
                .HasForeignKey(bc => bc.InvoiceId);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ProductInvoice> ProductInvoices { get; set; }
    }
}
