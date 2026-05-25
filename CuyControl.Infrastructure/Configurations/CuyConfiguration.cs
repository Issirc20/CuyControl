using CuyControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuyControl.Infrastructure.Configurations;

public class CuyConfiguration : IEntityTypeConfiguration<Cuy>
{
    public void Configure(EntityTypeBuilder<Cuy> builder)
    {
        builder.ToTable("Cuyes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.FechaNacimiento)
            .IsRequired();

        builder.Property(c => c.SexoId)
            .IsRequired();

        builder.Property(c => c.EstadoId)
            .IsRequired();

        builder.Property(c => c.JaulaId)
            .IsRequired(false);

        builder.Property(c => c.PesoActual)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(c => c.Raza)
            .HasMaxLength(100);

        builder.Property(c => c.Observaciones)
            .HasMaxLength(500);

        builder.Property(c => c.Activo)
            .IsRequired();

        builder.Property(c => c.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.HasIndex(c => c.Codigo)
            .IsUnique();

        builder.HasIndex(c => c.JaulaId);

        builder.HasIndex(c => c.EstadoId);

        builder.HasOne(c => c.Jaula)
            .WithMany(j => j.Cuyes)
            .HasForeignKey(c => c.JaulaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}