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
    public class PreCastWallProgressRecordEntityTypeConfiguration : IEntityTypeConfiguration<PreCastWallProgressRecord>
    {
        public void Configure(EntityTypeBuilder<PreCastWallProgressRecord> builder)
        {
            builder.ToTable("PreCastWallProgressWall");
            builder.HasKey(x => x.recordID);
            builder.Property(x => x.recordDate).IsRequired();
            builder.HasOne(x => x.unit).WithMany(x => x.progressRecords).HasForeignKey(x => x.unitID).OnDelete(DeleteBehavior.ClientSetNull).IsRequired();

        }
    }
}
