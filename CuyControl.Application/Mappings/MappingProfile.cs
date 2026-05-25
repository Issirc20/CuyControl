using CuyControl.Application.DTOs;
using CuyControl.Domain.Entities;

namespace CuyControl.Application.Mappings;

/// <summary>
/// Clase para mapeo entre entidades de dominio y DTOs.
/// Configura métodos para convertir entre entidades y sus DTOs correspondientes.
/// </summary>
public static class MappingProfile
{
    // Mapeos de Cuy
    /// <summary>
    /// Mapea una entidad Cuy a CuyDto.
    /// </summary>
    public static CuyDto MapToCuyDto(Cuy cuy)
    {
        if (cuy == null) return new CuyDto();

        return new CuyDto
        {
            Id = cuy.Id,
            Codigo = cuy.Codigo,
            FechaNacimiento = cuy.FechaNacimiento,
            SexoId = cuy.SexoId,
            EstadoId = cuy.EstadoId,
            JaulaId = cuy.JaulaId,
            PesoActual = cuy.PesoActual,
            Raza = cuy.Raza,
            Observaciones = cuy.Observaciones,
            EdadDias = (int)(DateTime.Now - cuy.FechaNacimiento).TotalDays
        };
    }

    /// <summary>
    /// Mapea un CuyDto a una entidad Cuy.
    /// </summary>
    public static Cuy MapToCuy(CuyDto cuyDto)
    {
        if (cuyDto == null) return new Cuy();

        return new Cuy
        {
            Id = cuyDto.Id,
            Codigo = cuyDto.Codigo,
            FechaNacimiento = cuyDto.FechaNacimiento,
            SexoId = cuyDto.SexoId,
            EstadoId = cuyDto.EstadoId,
            JaulaId = cuyDto.JaulaId,
            PesoActual = cuyDto.PesoActual,
            Raza = cuyDto.Raza,
            Observaciones = cuyDto.Observaciones
        };
    }

    // Mapeos de Venta
    /// <summary>
    /// Mapea una entidad Venta a VentaDto.
    /// </summary>
    public static VentaDto MapToVentaDto(Venta venta)
    {
        if (venta == null) return new VentaDto();

        return new VentaDto
        {
            Id = venta.Id,
            CuyId = venta.CuyId,
            FechaVenta = venta.FechaVenta,
            PrecioUnitario = venta.PrecioUnitario,
            Cantidad = venta.Cantidad,
            PrecioTotal = venta.PrecioTotal,
            NombreComprador = venta.NombreComprador,
            ContactoComprador = venta.ContactoComprador,
            MetodoPago = venta.MetodoPago,
            Observaciones = venta.Observaciones
        };
    }

    /// <summary>
    /// Mapea un VentaDto a una entidad Venta.
    /// </summary>
    public static Venta MapToVenta(VentaDto ventaDto)
    {
        if (ventaDto == null) return new Venta();

        return new Venta
        {
            Id = ventaDto.Id,
            CuyId = ventaDto.CuyId,
            FechaVenta = ventaDto.FechaVenta,
            PrecioUnitario = ventaDto.PrecioUnitario,
            Cantidad = ventaDto.Cantidad,
            PrecioTotal = ventaDto.PrecioTotal,
            NombreComprador = ventaDto.NombreComprador,
            ContactoComprador = ventaDto.ContactoComprador,
            MetodoPago = ventaDto.MetodoPago,
            Observaciones = ventaDto.Observaciones
        };
    }

    // Mapeos de Usuario

    // Mapeos de Alimentacion
    public static AlimentacionDto MapToAlimentacionDto(Alimentacion entity)
    {
        if (entity == null) return new AlimentacionDto();

        return new AlimentacionDto
        {
            Id = entity.Id,
            JaulaId = entity.JaulaId,
            UsuarioId = entity.UsuarioId,
            FechaAlimentacion = entity.FechaAlimentacion,
            CantidadAlimento = entity.CantidadAlimento,
            TipoAlimento = entity.TipoAlimento,
            Observaciones = entity.Observaciones
        };
    }

    public static Alimentacion MapToAlimentacion(AlimentacionDto dto)
    {
        if (dto == null) return new Alimentacion();

        return new Alimentacion
        {
            Id = dto.Id,
            JaulaId = dto.JaulaId,
            UsuarioId = dto.UsuarioId,
            FechaAlimentacion = dto.FechaAlimentacion,
            CantidadAlimento = dto.CantidadAlimento,
            TipoAlimento = dto.TipoAlimento,
            Observaciones = dto.Observaciones
        };
    }

    // Mapeos de Usuario
    /// <summary>
    /// Mapea una entidad Usuario a UsuarioDto.
    /// </summary>
    public static UsuarioDto MapToUsuarioDto(Usuario usuario)
    {
        if (usuario == null) return new UsuarioDto();

        return new UsuarioDto
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            NombreUsuario = usuario.NombreUsuario,
            Correo = usuario.Correo,
            TipoUsuarioId = usuario.TipoUsuarioId,
            Activo = usuario.Activo,
            Telefono = usuario.Telefono
        };
    }

    /// <summary>
    /// Mapea un UsuarioDto a una entidad Usuario.
    /// </summary>
    public static Usuario MapToUsuario(UsuarioDto usuarioDto)
    {
        if (usuarioDto == null) return new Usuario();

        return new Usuario
        {
            Id = usuarioDto.Id,
            NombreCompleto = usuarioDto.NombreCompleto,
            NombreUsuario = usuarioDto.NombreUsuario,
            Correo = usuarioDto.Correo,
            TipoUsuarioId = usuarioDto.TipoUsuarioId,
            Activo = usuarioDto.Activo,
            Telefono = usuarioDto.Telefono
        };
    }
}
