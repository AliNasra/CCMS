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
    public class FuelDepotEntityTypeConfiguration : IEntityTypeConfiguration<FuelDepot>
    {
        public void Configure(EntityTypeBuilder<FuelDepot> builder)
        {
            builder.ToTable("Depot");
            builder.HasKey(x => x.depotID);
            builder.Property(x => x.depotName).IsRequired();
            builder.HasIndex(x => x.depotName).IsUnique();
            builder.Property(x => x.depotStorageCapacity).IsRequired();
            builder.HasOne(x => x.unit).WithMany(x => x.unitFuelDepots).HasForeignKey(x => x.unitID).OnDelete(DeleteBehavior.ClientSetNull).IsRequired();

        }
    }
}
