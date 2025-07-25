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
    public class UnitEntityTypeConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.ToTable("Unit");
            builder.HasKey(x => x.unitID);
            builder.Property(x => x.unitCode).IsRequired();
            builder.Property(x => x.unitDesignation).IsRequired();
            builder.Property(x => x.unitSpecialization).IsRequired();
            builder.HasIndex(o => new { o.unitCode, o.unitSpecialization, o.unitDesignation })
            .IsUnique();
            builder.Property(x => x.selfSufficienyReserve).HasDefaultValue(0);
        }
    }
}
