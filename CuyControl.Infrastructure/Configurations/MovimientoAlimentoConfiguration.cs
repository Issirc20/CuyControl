using CuyControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuyControl.Infrastructure.Configurations;

public class MovimientoAlimentoConfiguration : IEntityTypeConfiguration<MovimientoAlimento>
{
    public void Configure(EntityTypeBuilder<MovimientoAlimento> builder)
    {
        builder.ToTable("MovimientosAlimento");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.TipoMovimiento)
            .IsRequired();

        builder.Property(m => m.Cantidad)
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(m => m.FechaMovimiento)
            .IsRequired();

        builder.Property(m => m.Observaciones)
            .HasMaxLength(500);

        builder.HasOne(m => m.InventarioAlimento)
            .WithMany()
            .HasForeignKey(m => m.InventarioAlimentoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(m => m.Usuario)
            .WithMany()
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}