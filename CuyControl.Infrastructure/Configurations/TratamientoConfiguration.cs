using CuyControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuyControl.Infrastructure.Configurations;

/// <summary>
/// Configuración de Entity Framework para la entidad Tratamiento.
/// </summary>
public class TratamientoConfiguration : IEntityTypeConfiguration<Tratamiento>
{
    public void Configure(EntityTypeBuilder<Tratamiento> builder)
    {
        builder.ToTable("Tratamientos");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Medicamento)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.Dosis)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.ViaAdministracion)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.FechaInicio)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.FechaFin)
            .HasColumnType("datetime2");

        builder.Property(t => t.Resultado)
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.Observaciones)
            .HasColumnType("nvarchar(max)");

        builder.Property(t => t.FechaCreacion)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.UsuarioCreacionId)
            .IsRequired();

        builder.Property(t => t.FechaModificacion)
            .HasColumnType("datetime2");

        builder.Property(t => t.UsuarioModificacionId);

        // =========================
        // RELACIONES
        // =========================

        builder.HasOne(t => t.Cuy)
            .WithMany(c => c.Tratamientos)
            .HasForeignKey(t => t.CuyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Enfermedad)
            .WithMany()
            .HasForeignKey(t => t.EnfermedadId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(t => t.UsuarioCreacionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(t => t.UsuarioModificacionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}