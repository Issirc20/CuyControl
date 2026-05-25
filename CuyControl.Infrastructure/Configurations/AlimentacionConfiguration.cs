using CuyControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuyControl.Infrastructure.Configurations;

public class AlimentacionConfiguration : IEntityTypeConfiguration<Alimentacion>
{
    public void Configure(EntityTypeBuilder<Alimentacion> builder)
    {
        builder.ToTable("Alimentaciones");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.TipoAlimento).HasMaxLength(150);
        builder.Property(a => a.CantidadAlimento).HasColumnType("decimal(10,2)");
    }
}
