using System.Globalization;
using MiniInventario.Services;

namespace MiniInventario;

public partial class MetricasPage : ContentPage
{
    private readonly DatabaseService _database;

    public MetricasPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var productos = await _database.ObtenerProductosAsync();

            float valorInventario = productos.Sum(p => p.PrecioCompra * p.Cantidad);
            float valorVenta = productos.Sum(p => p.PrecioVenta * p.Cantidad);

            TotalProductosLabel.Text = productos.Count.ToString();
            ValorInventarioLabel.Text = valorInventario.ToString("C", CultureInfo.CurrentCulture);
            GananciaLabel.Text = (valorVenta - valorInventario).ToString("C", CultureInfo.CurrentCulture);
            SinStockLabel.Text = productos.Count(p => p.Cantidad == 0).ToString();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }
}