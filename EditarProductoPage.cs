using System.Globalization;
using MiniInventario.Models;
using MiniInventario.Services;

namespace MiniInventario;

public partial class EditarProductoPage : ContentPage
{
    private readonly DatabaseService _database;
    private readonly Producto _producto;

    public EditarProductoPage(DatabaseService database, Producto producto)
    {
        InitializeComponent();
        _database = database;
        _producto = producto ?? throw new ArgumentNullException(nameof(producto));

        _producto.Nombre ??= string.Empty;
        _producto.Descripcion ??= string.Empty;
        _producto.UnidadMedicion ??= string.Empty;

        NombreEntry.Text = _producto.Nombre;
        PrecioVentaEntry.Text = _producto.PrecioVenta.ToString(CultureInfo.CurrentCulture);
        PrecioCompraEntry.Text = _producto.PrecioCompra.ToString(CultureInfo.CurrentCulture);
        CantidadEntry.Text = _producto.Cantidad.ToString();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        string nombre = NombreEntry.Text?.Trim() ?? "";

        if (nombre.Length < 3 || nombre.Length > 50 ||
            !nombre.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
        {
            await DisplayAlertAsync("Validación", "El nombre debe tener entre 3 y 50 caracteres y solo letras y espacios.", "OK");
            return;
        }

        if (!float.TryParse(PrecioVentaEntry.Text, NumberStyles.Number,
                CultureInfo.CurrentCulture, out float precioVenta) || precioVenta <= 0)
        {
            await DisplayAlertAsync("Validación", "Introduce un precio de venta válido.", "OK");
            return;
        }

        if (!float.TryParse(PrecioCompraEntry.Text, NumberStyles.Number,
                CultureInfo.CurrentCulture, out float precioCompra)
            || precioCompra <= 0 || precioCompra > precioVenta)
        {
            await DisplayAlertAsync("Validación", "El precio de compra debe ser mayor a 0 y no mayor al de venta.", "OK");
            return;
        }

        if (!float.TryParse(CantidadEntry.Text, out float cantidad) || cantidad < 0)
        {
            await DisplayAlertAsync("Validación", "Introduce una cantidad válida.", "OK");
            return;
        }

        _producto.Nombre = nombre;
        _producto.PrecioVenta = precioVenta;
        _producto.PrecioCompra = precioCompra;
        _producto.Cantidad = cantidad;
        _producto.Descripcion ??= string.Empty;
        _producto.UnidadMedicion ??= string.Empty;

        try
        {
            await _database.ActualizarProductoAsync(_producto);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"No se pudo guardar el producto: {ex.Message}", "OK");
            return;
        }

        await Navigation.PopAsync();
    }
}