using CuyControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuyControl.Infrastructure.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.NombreCompleto)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.NombreUsuario)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Correo)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.ContraseñaHash)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(u => u.TipoUsuarioId)
            .IsRequired();

        builder.Property(u => u.Activo)
            .IsRequired();

        builder.Property(u => u.Telefono)
            .HasMaxLength(20);

        builder.Property(u => u.FechaCreacion)
            .IsRequired()
            .HasDefaultValueSql("GETDATE()");

        builder.Property(u => u.FechaModificacion)
            .IsRequired(false);

        builder.HasIndex(u => u.NombreUsuario)
            .IsUnique();

        builder.HasIndex(u => u.Correo)
            .IsUnique();
    }
}