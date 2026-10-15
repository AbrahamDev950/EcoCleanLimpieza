namespace MiniInventario.Models;

public class ResumenProducto
{
    public int Posicion { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public float Unidades { get; set; }
    public float Ingresos { get; set; }
    public float Ganancia { get; set; }
    public double Proporcion { get; set; }   // 0 a 1, para la barra
}

public class ResumenDia
{
    public string Fecha { get; set; } = string.Empty;
    public float Ganancia { get; set; }
    public int Ventas { get; set; }
    public double Proporcion { get; set; }
}