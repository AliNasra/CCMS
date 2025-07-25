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
    public class CementDailyRecordEntityTypeConfiguration : IEntityTypeConfiguration<CementDailyRecord>
    {
        public void Configure(EntityTypeBuilder<CementDailyRecord> builder)
        {
            builder.ToTable("CementDailyRecord");
            builder.HasKey(x => x.recordID);
            builder.Property(x => x.recordDate).IsRequired();
            builder.HasOne(x => x.mixer).WithMany(x => x.cementRecords).HasForeignKey(x => x.mixerID).OnDelete(DeleteBehavior.ClientSetNull).IsRequired();
        }
    }
}
