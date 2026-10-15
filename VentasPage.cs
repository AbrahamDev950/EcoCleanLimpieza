using MiniInventario.Models;
using MiniInventario.Services;

namespace MiniInventario;

public partial class VentasPage : ContentPage
{
    private readonly DatabaseService _database;

    public VentasPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarAsync();
    }

    private async Task CargarAsync()
    {
        try
        {
            ProductoPicker.ItemsSource = await _database.ObtenerProductosAsync();
            VentasCollection.ItemsSource = await _database.ObtenerVentasAsync();
            StockLabel.Text = string.Empty;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
        }
    }

    private void OnProductoChanged(object sender, EventArgs e)
    {
        StockLabel.Text = ProductoPicker.SelectedItem is Producto p
            ? $"Stock disponible: {p.Cantidad}"
            : string.Empty;
    }

    private async void OnRegistrarClicked(object sender, EventArgs e)
    {
        if (ProductoPicker.SelectedItem is not Producto producto)
        {
            await DisplayAlertAsync("Validación", "Selecciona un producto.", "OK");
            return;
        }

        if (!float.TryParse(CantidadEntry.Text, out float cantidad) || cantidad <= 0)
        {
            await DisplayAlertAsync("Validación", "Introduce una cantidad válida.", "OK");
            return;
        }

        try
        {
            await _database.RegistrarVentaAsync(producto.Id, cantidad);
        }
        catch (InvalidOperationException ex)
        {
            await DisplayAlertAsync("No se pudo registrar", ex.Message, "OK");
            return;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", ex.Message, "OK");
            return;
        }

        await DisplayAlertAsync("Éxito", "Venta registrada correctamente.", "OK");

        CantidadEntry.Text = string.Empty;
        await CargarAsync();
    }
}