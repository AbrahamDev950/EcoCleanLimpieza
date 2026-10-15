using SQLite;

namespace MiniInventario.Models;

public class Venta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int ProductoId { get; set; }

    public string ProductoNombre { get; set; } = string.Empty;
    public float Cantidad { get; set; }
    public float PrecioVenta { get; set; }
    public float PrecioCompra { get; set; }
    public DateTime Fecha { get; set; }
}