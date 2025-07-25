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
    public class ConcreteProductionRecordEntityTypeConfiguration : IEntityTypeConfiguration<ConcreteProductionRecord>
    {
        public void Configure(EntityTypeBuilder<ConcreteProductionRecord> builder)
        {
            builder.ToTable("ConcreteProduction");
            builder.HasKey(x => x.recordID);
            builder.Property(x => x.recordDate).IsRequired();
            builder.Property(x => x.isReinforcedConcrete).IsRequired();
            builder.Property(x => x.producedConcreteAmount).IsRequired();
            builder.Property(x => x.company).IsRequired();
            builder.HasOne(x => x.mixer).WithMany(x => x.concreteRecords).HasForeignKey(x => x.mixerID).OnDelete(DeleteBehavior.ClientSetNull).IsRequired();

        }
    }
}
