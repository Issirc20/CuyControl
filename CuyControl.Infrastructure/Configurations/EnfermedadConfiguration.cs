using CuyControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CuyControl.Infrastructure.Configurations;

public class EnfermedadConfiguration : IEntityTypeConfiguration<Enfermedad>
{
    public void Configure(EntityTypeBuilder<Enfermedad> builder)
    {
        builder.ToTable("Enfermedades");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nombre).HasMaxLength(200);
    }
}
