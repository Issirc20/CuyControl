namespace CuyControl.Domain.Enums;

/// <summary>
/// Enumeración de estados posibles de un cuy
/// </summary>
public enum EstadoCuy
{
    /// <summary>
    /// Cuy recién nacido (0 a 3 semanas)
    /// </summary>
    Cria = 1,

    /// <summary>
    /// Cuy en crecimiento (3 a 8 semanas)
    /// </summary>
    Recria = 2,

    /// <summary>
    /// Cuy en edad reproductiva
    /// </summary>
    Reproductor = 3,

    /// <summary>
    /// Hembra en período de gestación
    /// </summary>
    Gestante = 4,

    /// <summary>
    /// Hembra en período de lactancia
    /// </summary>
    Lactante = 5,

    /// <summary>
    /// Cuy vendido
    /// </summary>
    Vendido = 6,

    /// <summary>
    /// Cuy fallecido
    /// </summary>
    Muerto = 7
}