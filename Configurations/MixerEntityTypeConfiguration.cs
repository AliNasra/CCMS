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
    public class MixerEntityTypeConfiguration : IEntityTypeConfiguration<Mixer>
    {
        public void Configure(EntityTypeBuilder<Mixer> builder)
        {
            builder.ToTable("Mixer");
            builder.HasKey(x => x.mixerID);
            builder.Property(x => x.mixerName).IsRequired();
            builder.Property(x => x.isOperational).IsRequired();
            builder.Property(x => x.cabbageNo).IsRequired();
            builder.HasIndex(x => x.cabbageNo).IsUnique();

        }
    }
}
