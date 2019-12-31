using System;
using System.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using webapp.Data.Entities;

namespace webapp.Data
{
    public class ZFContext : DbContext
    {
        public ZFContext(DbContextOptions<ZFContext> options) : base(options)
        {
        }

        public ZFContext()
        {

        }
        public DbSet<Form> Forms { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderProduct> OrderProducts { get; set; }
        public DbSet<Oven> Ovens { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductForm> ProductForms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderProduct>().HasKey(o => new { o.OrderId, o.ProductId });
            modelBuilder.Entity<ProductForm>().HasKey(o => new { o.ProductId, o.FormId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Environment.GetEnvironmentVariable("DOCKER_ENVIRONMENT") == null) // Using no Docker i.e. IIS Express
            {
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=zf;User=sa;Password=7zc7agecM6EmRmoiQmvYF5k3v;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
            else if (Environment.GetEnvironmentVariable("DOCKER_ENVIRONMENT") == "Development")
            {

                optionsBuilder.UseSqlServer(
                    @"Server=db;Database=zf;User=sa;Password=7zc7agecM6EmRmoiQmvYF5k3v;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
            else if ((Environment.GetEnvironmentVariable("DOCKER_ENVIRONMENT") == "Production"))
            {
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["ZFContext"].ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}