using DogsHouseService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace DogsHouseService.Data.Repositories
{
    public class DogsHouseServiceDbContext : DbContext
    {
        public DogsHouseServiceDbContext(DbContextOptions<DogsHouseServiceDbContext> options) : base(options) { }

        public DbSet<Dog> Dogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dog>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Id).ValueGeneratedOnAdd();
                entity.HasIndex(d => d.Name).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

