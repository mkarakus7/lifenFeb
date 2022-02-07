using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Core.Entities;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }

  public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> OrderItem { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethod { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            if(Database.ProviderName =="Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach(var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties()
                        .Where(p=>p.PropertyType==typeof(decimal));

                        var dateTimeProp =  entityType.ClrType.GetProperties()
                                .Where( p => p.PropertyType == typeof(DateTimeOffset));

                        foreach (var property in properties)
                        {
                            modelBuilder.Entity(entityType.Name).Property(property.Name)
                                .HasConversion<double>();
                        }
                        
                        foreach (var property in dateTimeProp)
                        {
                            modelBuilder.Entity(entityType.Name).Property(property.Name)
                                .HasConversion( new DateTimeOffsetToBinaryConverter());                                
                        }

                }
            }
        }
        
    }
}