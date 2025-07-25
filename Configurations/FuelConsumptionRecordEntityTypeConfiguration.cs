using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp2.Models;

namespace WpfApp2.Configurations
{
    public class FuelConsumptionRecordEntityTypeConfiguration : IEntityTypeConfiguration<FuelConsumptionRecord>
    {
        public void Configure(EntityTypeBuilder<FuelConsumptionRecord> builder)
        {
            builder.ToTable("FuelConsumption");
            builder.HasKey(x => x.recordID);
            builder.Property(x => x.recordDate).IsRequired();
            builder.Property(x => x.consumedAmount).IsRequired();
            builder.HasOne(x => x.depot).WithMany(x => x.fuelConsumptionRecords).HasForeignKey(x => x.depotID).OnDelete(DeleteBehavior.ClientSetNull).IsRequired();

        }
    }
}
