using CuyControl.Domain.Entities;
using CuyControl.Infrastructure.Repositories;
using CuyControl.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CuyControl.Infrastructure.Data;

/// <summary>
/// Implementación del repositorio genérico base.
/// </summary>
/// <typeparam name="TEntity">Tipo de entidad.</typeparam>
public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;

    /// <summary>
    /// Constructor del repositorio base.
    /// </summary>
    protected BaseRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Obtiene todas las entidades.
    /// </summary>
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    /// <summary>
    /// Obtiene una entidad por id.
    /// </summary>
    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    /// <summary>
    /// Agrega una nueva entidad.
    /// </summary>
    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        return entity;
    }

    /// <summary>
    /// Actualiza una entidad existente.
    /// </summary>
    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        return await Task.FromResult(entity);
    }

    /// <summary>
    /// Elimina una entidad.
    /// </summary>
    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _context.Set<TEntity>().Remove(entity);
        return true;
    }

    /// <summary>
    /// Guarda los cambios en la base de datos.
    /// </summary>
    public virtual async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}

/// <summary>
/// Implementación del repositorio para Cuy.
/// </summary>
public class CuyRepository : BaseRepository<Cuy>, CuyControl.Application.Interfaces.Repositories.ICuyRepository
{
    /// <summary>
    /// Constructor del repositorio de Cuy.
    /// </summary>
    public CuyRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene un cuy por su código único.
    /// </summary>
    public async Task<Cuy?> GetByCodigoAsync(string codigo)
    {
        return await _context.Set<Cuy>()
            .FirstOrDefaultAsync(c => c.Codigo == codigo);
    }

    /// <summary>
    /// Obtiene cuyes por jaula.
    /// </summary>
    public async Task<IEnumerable<Cuy>> GetByJaulaAsync(int jaulaId)
    {
        return await _context.Set<Cuy>()
            .Where(c => c.JaulaId == jaulaId)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene cuyes activos.
    /// </summary>
    public async Task<IEnumerable<Cuy>> GetActivosAsync()
    {
        return await _context.Set<Cuy>()
            .Where(c => c.EstadoId == 1)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene cuyes por estado.
    /// </summary>
    public async Task<IEnumerable<Cuy>> GetByEstadoAsync(int estadoId)
    {
        return await _context.Set<Cuy>()
            .Where(c => c.EstadoId == estadoId)
            .ToListAsync();
    }

    /// <summary>
    /// Cuenta cuyes activos.
    /// </summary>
    public async Task<int> CountActivosAsync()
    {
        return await _context.Set<Cuy>()
            .CountAsync(c => c.EstadoId == 1);
    }
}

/// <summary>
/// Implementación del repositorio para Venta.
/// </summary>
public class VentaRepository : BaseRepository<Venta>, CuyControl.Application.Interfaces.Repositories.IVentaRepository
{
    /// <summary>
    /// Constructor del repositorio de Venta.
    /// </summary>
    public VentaRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene ventas en un rango de fechas.
    /// </summary>
    public async Task<IEnumerable<Venta>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _context.Set<Venta>()
            .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene total de ventas en un período.
    /// </summary>
    public async Task<decimal> GetTotalByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _context.Set<Venta>()
            .Where(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin)
            .SumAsync(v => v.PrecioTotal);
    }

    /// <summary>
    /// Obtiene ventas por cuy.
    /// </summary>
    public async Task<IEnumerable<Venta>> GetByCuyAsync(int cuyId)
    {
        return await _context.Set<Venta>()
            .Where(v => v.CuyId == cuyId)
            .ToListAsync();
    }

    /// <summary>
    /// Cuenta ventas en un período.
    /// </summary>
    public async Task<int> CountByFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _context.Set<Venta>()
            .CountAsync(v => v.FechaVenta >= fechaInicio && v.FechaVenta <= fechaFin);
    }
}

/// <summary>
/// Implementación del repositorio para Usuario.
/// </summary>
public class UsuarioRepository : BaseRepository<Usuario>, CuyControl.Application.Interfaces.Repositories.IUsuarioRepository
{
    /// <summary>
    /// Constructor del repositorio de Usuario.
    /// </summary>
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene un usuario por nombre de usuario.
    /// </summary>
    public async Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario)
    {
        return await _context.Set<Usuario>()
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
    }

    /// <summary>
    /// Obtiene un usuario por correo.
    /// </summary>
    public async Task<Usuario?> GetByCorreoAsync(string correo)
    {
        return await _context.Set<Usuario>()
            .FirstOrDefaultAsync(u => u.Correo == correo);
    }

    /// <summary>
    /// Obtiene usuarios activos.
    /// </summary>
    public async Task<IEnumerable<Usuario>> GetActivosAsync()
    {
        return await _context.Set<Usuario>()
            .Where(u => u.Activo)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene usuarios por tipo.
    /// </summary>
    public async Task<IEnumerable<Usuario>> GetByTipoAsync(int tipoUsuarioId)
    {
        return await _context.Set<Usuario>()
            .Where(u => u.TipoUsuarioId == tipoUsuarioId)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si existe un usuario por nombre.
    /// </summary>
    public async Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario)
    {
        return await _context.Set<Usuario>()
            .AnyAsync(u => u.NombreUsuario == nombreUsuario);
    }
}
