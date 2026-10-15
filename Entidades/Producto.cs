using System.ComponentModel.DataAnnotations;
using SQLite;

namespace MiniInventario.Models;

public class Producto
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    public float PrecioVenta { get; set; }

    [Required]
    public float PrecioCompra { get; set; }

    [Required]
    public float Cantidad { get; set; }

    [Required]
    public string UnidadMedicion { get; set; } = string.Empty;

    public Producto()
    {
        Nombre ??= string.Empty;
        Descripcion ??= string.Empty;
        UnidadMedicion ??= string.Empty;
    }
}