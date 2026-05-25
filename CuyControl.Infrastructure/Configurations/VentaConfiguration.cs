using CuyControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuyControl.Infrastructure.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Venta.
/// </summary>
public class VentaConfiguration : IEntityTypeConfiguration<Venta>
{
    /// <summary>
    /// Configura el mapeo de la entidad Venta con la base de datos.
    /// </summary>
    public void Configure(EntityTypeBuilder<Venta> builder)
    {
        builder.ToTable("Ventas");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .ValueGeneratedOnAdd();

        builder.Property(v => v.CuyId)
            .IsRequired();

        builder.Property(v => v.FechaVenta)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(v => v.PrecioUnitario)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(v => v.Cantidad)
            .IsRequired()
            .HasColumnType("int");

        builder.Property(v => v.PrecioTotal)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(v => v.NombreComprador)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnType("nvarchar(100)");

        builder.Property(v => v.ContactoComprador)
            .HasMaxLength(20)
            .HasColumnType("nvarchar(20)");

        builder.Property(v => v.MetodoPago)
            .HasMaxLength(50)
            .HasColumnType("nvarchar(50)");

        builder.Property(v => v.Observaciones)
            .HasColumnType("nvarchar(max)");

        // Propiedades de auditoría
        builder.Property(v => v.FechaCreacion)
            .IsRequired()
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(v => v.UsuarioCreacionId)
            .IsRequired();

        builder.Property(v => v.FechaModificacion)
            .HasColumnType("datetime2");

        builder.Property(v => v.UsuarioModificacionId)
            .HasColumnType("int");

        // Índices
        builder.HasIndex(v => v.FechaVenta)
            .HasDatabaseName("IX_Venta_FechaVenta");

        builder.HasIndex(v => v.CuyId)
            .HasDatabaseName("IX_Venta_CuyId");

        // Relaciones
        builder.HasOne(v => v.Cuy)
            .WithMany(c => c.Ventas)
            .HasForeignKey(v => v.CuyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.UsuarioCreacion)
            .WithMany()
            .HasForeignKey(v => v.UsuarioCreacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.UsuarioModificacion)
            .WithMany()
            .HasForeignKey(v => v.UsuarioModificacionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
