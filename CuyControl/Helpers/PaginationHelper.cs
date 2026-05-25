namespace CuyControl.Web.Helpers;

/// <summary>
/// Clase auxiliar para utilidades de paginación.
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// Página actual.
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// Total de páginas.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Tamaño de página.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de registros.
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// Items de la página actual.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Indica si hay página anterior.
    /// </summary>
    public bool HasPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Indica si hay página siguiente.
    /// </summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Constructor del resultado paginado.
    /// </summary>
    public PagedResult(IEnumerable<T> items, int totalRecords, int pageNumber, int pageSize)
    {
        Items = items;
        TotalRecords = totalRecords;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
    }
}
