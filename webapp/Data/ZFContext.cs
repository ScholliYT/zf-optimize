﻿using Microsoft.EntityFrameworkCore;
using webapp.Data.Entities;

namespace webapp.Data
{
    public class ZFContext : DbContext
    {
        public ZFContext(DbContextOptions<ZFContext> options) : base(options)
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
            modelBuilder.Entity<OrderProduct>().HasKey(o => new {o.OrderId, o.ProductId});
            modelBuilder.Entity<ProductForm>().HasKey(o => new {o.ProductId, o.FormId});
        }
    }
}