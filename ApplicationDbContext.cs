using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WpfApp2.Models;
using WpfApp2.Configurations;
using WpfApp2.Services;


namespace WebApplication1
{
    public class ApplicationDbContext : DbContext
    {
        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        //{

        //}
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=app.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new CementDailyRecordEntityTypeConfiguration().Configure(modelBuilder.Entity<CementDailyRecord>());
            new ConcreteProductionRecordEntityTypeConfiguration().Configure(modelBuilder.Entity<ConcreteProductionRecord>());
            new FuelConsumptionRecordEntityTypeConfiguration().Configure(modelBuilder.Entity<FuelConsumptionRecord>());
            new FuelDepotEntityTypeConfiguration().Configure(modelBuilder.Entity<FuelDepot>());
            new MixerEntityTypeConfiguration().Configure(modelBuilder.Entity<Mixer>());
            new PreCastWallProgressRecordEntityTypeConfiguration().Configure(modelBuilder.Entity<PreCastWallProgressRecord>());
            new UnitEntityTypeConfiguration().Configure(modelBuilder.Entity<Unit>());
        }
        public DbSet<CementDailyRecord>           cementDailyRecords         { get; set; }
        public DbSet<ConcreteProductionRecord>    concreteProductionRecords  { get; set; }
        public DbSet<FuelConsumptionRecord>       fuelConsumptionRecords     { get; set; }
        public DbSet<FuelDepot>                   depots                     { get; set; }
        public DbSet<Mixer>                       mixers                     { get; set; }
        public DbSet<PreCastWallProgressRecord>   preCastWallProgressRecords { get; set; }
        public DbSet<Unit>                        units                      { get; set; }

    }
}
