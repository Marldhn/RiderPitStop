using RiderPitStop.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RiderPitStop.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<RequestDetails> RequestDetails { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<RequestOrder> RequestOrders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RequestOrder>()
                .HasKey(ro => new { ro.ProductId, ro.DetailsId });  // Use foreign keys

            // Define the relationships
            modelBuilder.Entity<RequestOrder>()
                .HasOne(ro => ro.Product)
                .WithMany(p => p.RequestOrders)
                .HasForeignKey(ro => ro.ProductId);

            modelBuilder.Entity<RequestOrder>()
                .HasOne(ro => ro.requestDetails)
                .WithMany(rd => rd.RequestOrders)
                .HasForeignKey(ro => ro.DetailsId);
        }
    }


}
