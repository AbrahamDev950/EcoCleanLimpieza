namespace MiniInventario.Models;

public class ResumenProducto
{
    public int Posicion { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Unidades { get; set; }
    public decimal Ingresos { get; set; }
    public decimal Ganancia { get; set; }
    public double Proporcion { get; set; }   // 0 a 1, para la barra
}

public class ResumenDia
{
    public string Fecha { get; set; } = string.Empty;
    public decimal Ganancia { get; set; }
    public int Ventas { get; set; }
    public double Proporcion { get; set; }
}