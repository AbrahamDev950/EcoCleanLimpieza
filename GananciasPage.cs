using System.Globalization;
using MiniInventario.Models;
using MiniInventario.Services;

namespace MiniInventario;

public partial class GananciasPage : ContentPage
{
    private readonly DatabaseService _database;
    private List<Venta> _ventas = new();

    public GananciasPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;

        PeriodoPicker.ItemsSource = new List<string>
        {
            "Hoy", "Últimos 7 días", "Últimos 30 días", "Todo"
        };
        PeriodoPicker.SelectedIndex = 2;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            _ventas = await _database.ObtenerVentasAsync();
            Actualizar();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private void OnPeriodoChanged(object sender, EventArgs e) => Actualizar();

    private void Actualizar()
    {
        DateTime desde = PeriodoPicker.SelectedIndex switch
        {
            0 => DateTime.Today,
            1 => DateTime.Today.AddDays(-6),
            2 => DateTime.Today.AddDays(-29),
            _ => DateTime.MinValue
        };

        var ventas = _ventas.Where(v => v.Fecha >= desde).ToList();
        var cultura = CultureInfo.CurrentCulture;

        // ----- Totales -----
        float ingresos = ventas.Sum(v => v.PrecioVenta * v.Cantidad);
        float costo = ventas.Sum(v => v.PrecioCompra * v.Cantidad);
        float ganancia = ingresos - costo;
        float margen = ingresos > 0 ? ganancia / ingresos : 0;

        GananciaLabel.Text = ganancia.ToString("C", cultura);
        GananciaLabel.TextColor = ganancia >= 0 ? Color.FromArgb("#10B981") : Color.FromArgb("#EF4444");
        MargenLabel.Text = $"Margen: {margen.ToString("P1", cultura)}";
        IngresosLabel.Text = ingresos.ToString("C", cultura);
        CostoLabel.Text = costo.ToString("C", cultura);
        NumVentasLabel.Text = ventas.Count.ToString();
        UnidadesLabel.Text = ventas.Sum(v => v.Cantidad).ToString();

        VacioLabel.IsVisible = ventas.Count == 0;

        // ----- Por producto -----
        var porProducto = ventas
            .GroupBy(v => v.ProductoId)
            .Select(g => new ResumenProducto
            {
                Nombre = g.OrderByDescending(v => v.Fecha).First().ProductoNombre,
                Unidades = g.Sum(v => v.Cantidad),
                Ingresos = g.Sum(v => v.PrecioVenta * v.Cantidad),
                Ganancia = g.Sum(v => (v.PrecioVenta - v.PrecioCompra) * v.Cantidad)
            })
            .ToList();

        var top = porProducto
            .OrderByDescending(p => p.Unidades)
            .ThenByDescending(p => p.Ganancia)
            .Take(10)
            .ToList();

        float maxUnidades = top.Count > 0 ? top.Max(p => p.Unidades) : 0;
        for (int i = 0; i < top.Count; i++)
        {
            top[i].Posicion = i + 1;
            top[i].Proporcion = maxUnidades > 0 ? (double)top[i].Unidades / maxUnidades : 0;
        }

        TopCollection.ItemsSource = top;

        var masRentable = porProducto.OrderByDescending(p => p.Ganancia).FirstOrDefault();
        MasVendidoLabel.Text = top.Count > 0
            ? $"{top[0].Nombre} ({top[0].Unidades} uds.)"
            : "—";
        MasRentableLabel.Text = masRentable is not null
            ? $"{masRentable.Nombre} ({masRentable.Ganancia.ToString("C", cultura)})"
            : "—";

        // ----- Por día -----
        var porDia = ventas
            .GroupBy(v => v.Fecha.Date)
            .OrderByDescending(g => g.Key)
            .Take(7)
            .Select(g => new ResumenDia
            {
                Fecha = g.Key.ToString("dddd dd/MM", cultura),
                Ventas = g.Count(),
                Ganancia = g.Sum(v => (v.PrecioVenta - v.PrecioCompra) * v.Cantidad)
            })
            .ToList();

        float maxDia = porDia.Count > 0 ? porDia.Max(d => d.Ganancia) : 0;
        foreach (var d in porDia)
        {
            d.Proporcion = maxDia > 0 ? Math.Clamp((double)(d.Ganancia / maxDia), 0, 1) : 0;
        }

        DiasCollection.ItemsSource = porDia;
    }
}