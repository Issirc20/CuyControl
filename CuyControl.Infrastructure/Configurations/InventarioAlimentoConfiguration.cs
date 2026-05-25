using CuyControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuyControl.Infrastructure.Configurations;

public class InventarioAlimentoConfiguration : IEntityTypeConfiguration<InventarioAlimento>
{
    public void Configure(EntityTypeBuilder<InventarioAlimento> builder)
    {
        builder.ToTable("InventariosAlimento");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.TipoAlimento).HasMaxLength(200).IsRequired();
        builder.Property(i => i.CantidadActual).HasColumnType("decimal(10,2)");
        builder.Property(i => i.CostoUnitario).HasColumnType("decimal(10,2)");
    }
}
