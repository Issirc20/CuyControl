using CuyControl.Application.Interfaces.Repositories;
using CuyControl.Domain.Entities;
using CuyControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CuyControl.Infrastructure.Repositories;

public class AlimentacionRepository : IAlimentacionRepository
{
    private readonly ApplicationDbContext _context;

    public AlimentacionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Alimentacion>> GetAllAsync()
    {
        return await _context.Alimentaciones
            .Include(a => a.Jaula)
            .Include(a => a.Usuario)
            .AsNoTracking()
            .OrderByDescending(a => a.FechaAlimentacion)
            .ToListAsync();
    }

    public async Task<Alimentacion?> GetByIdAsync(int id)
    {
        return await _context.Alimentaciones
            .Include(a => a.Jaula)
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Alimentacion>> GetByJaulaAsync(int jaulaId)
    {
        return await _context.Alimentaciones
            .Where(a => a.JaulaId == jaulaId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Alimentacion> AddAsync(Alimentacion entity)
    {
        await _context.Alimentaciones.AddAsync(entity);
        return entity;
    }

    public async Task<Alimentacion> UpdateAsync(Alimentacion entity)
    {
        _context.Alimentaciones.Update(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Alimentaciones.FindAsync(id);
        if (entity == null) return false;
        _context.Alimentaciones.Remove(entity);
        return true;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _context.SaveChangesAsync()) > 0;
    }
}