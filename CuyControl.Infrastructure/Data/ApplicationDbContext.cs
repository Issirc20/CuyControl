using CuyControl.Domain.Entities;
using CuyControl.Infrastructure.Configurations;
using CuyControl.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CuyControl.Infrastructure.Data;

/// <summary>
/// Contexto principal de Entity Framework para la aplicación CuyControl.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    /// <summary>
    /// Constructor del contexto.
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    // DbSets de entidades de dominio
    public DbSet<Cuy> Cuyes { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Galpon> Galpones { get; set; }
    public DbSet<Jaula> Jaulas { get; set; }
    public DbSet<Alimentacion> Alimentaciones { get; set; }
    public DbSet<InventarioAlimento> InventariosAlimento { get; set; }
    public DbSet<ControlPeso> ControlesPeso { get; set; }
    public DbSet<Enfermedad> Enfermedades { get; set; }
    public DbSet<Tratamiento> Tratamientos { get; set; }
    public DbSet<Reproduccion> Reproducciones { get; set; }
    public DbSet<Parto> Partos { get; set; }
    public DbSet<Mortalidad> Mortalidades { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<MovimientoAlimento> MovimientosAlimento { get; set; }

    /// <summary>
    /// Configura el modelado de las entidades.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configuraciones de entidades
        modelBuilder.ApplyConfiguration(new CuyConfiguration());
        modelBuilder.ApplyConfiguration(new VentaConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioConfiguration());
        modelBuilder.ApplyConfiguration(new MovimientoAlimentoConfiguration());
        modelBuilder.ApplyConfiguration(new InventarioAlimentoConfiguration());

        // Configuración adicional de Identity
        ConfigureIdentity(modelBuilder);

        // Configurar relaciones de otras entidades
        ConfigureRelationships(modelBuilder);
    }

    /// <summary>
    /// Configura las entidades de Identity.
    /// </summary>
    private void ConfigureIdentity(ModelBuilder modelBuilder)
    {
        // Cambiar nombre de tabla de AspNetUsers a ApplicationUsers
        modelBuilder.Entity<ApplicationUser>().ToTable("ApplicationUsers");
        modelBuilder.Entity<IdentityRole<int>>().ToTable("ApplicationRoles");
        modelBuilder.Entity<IdentityUserRole<int>>().ToTable("ApplicationUserRoles");
        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("ApplicationUserClaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("ApplicationUserLogins");
        modelBuilder.Entity<IdentityUserToken<int>>().ToTable("ApplicationUserTokens");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("ApplicationRoleClaims");
        
    }

    /// <summary>
    /// Configura las relaciones de las entidades de dominio.
    /// </summary>
    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // Configuración de Galpon
        modelBuilder.Entity<Galpon>()
            .HasMany(g => g.Jaulas)
            .WithOne(j => j.Galpon)
            .HasForeignKey(j => j.GalponId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuración de ControlPeso
        modelBuilder.Entity<ControlPeso>()
            .HasOne(cp => cp.Cuy)
            .WithMany(c => c.ControlesPeso)
            .HasForeignKey(cp => cp.CuyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuración de Enfermedad
        modelBuilder.Entity<Enfermedad>()
            .HasOne(e => e.Cuy)
            .WithMany(c => c.Enfermedades)
            .HasForeignKey(e => e.CuyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enfermedad>()
            .HasMany(e => e.Tratamientos)
            .WithOne(t => t.Enfermedad)
            .HasForeignKey(t => t.EnfermedadId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuración de Tratamiento
        modelBuilder.Entity<Tratamiento>()
            .HasOne(t => t.Enfermedad)
            .WithMany(e => e.Tratamientos)
            .HasForeignKey(t => t.EnfermedadId)
            .OnDelete(DeleteBehavior.NoAction);

        // Configuración de Reproduccion
        modelBuilder.Entity<Reproduccion>()
            .HasOne(r => r.CuyMacho)
            .WithMany(c => c.Reproducciones)
            .HasForeignKey(r => r.CuyMachoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de Parto
        modelBuilder.Entity<Parto>()
            .HasOne(p => p.Reproduccion)
            .WithMany(r => r.Partos)
            .HasForeignKey(p => p.ReproduccionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Parto>()
            .HasOne(p => p.CuyHembra)
            .WithMany()
            .HasForeignKey(p => p.CuyHembraId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configuración de Mortalidad
        modelBuilder.Entity<Mortalidad>()
            .HasOne(m => m.Cuy)
            .WithMany(c => c.Mortalidades)
            .HasForeignKey(m => m.CuyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuración de Alimentacion
            modelBuilder.Entity<Alimentacion>(entity =>
            {
                entity.ToTable("Alimentaciones");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TipoAlimento).HasMaxLength(150).IsRequired();
                entity.Property(e => e.CantidadAlimento).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.Jaula)
                      .WithMany()
                      .HasForeignKey(e => e.JaulaId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Usuario)
                      .WithMany()
                      .HasForeignKey(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
       //     .HasOne(a => a.Jaula)
       //     .WithMany()
       //     .HasForeignKey(a => a.JaulaId)
        //    .OnDelete(DeleteBehavior.Restrict);
    }
}
