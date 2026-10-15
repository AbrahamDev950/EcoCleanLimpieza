using System.Globalization;
using MiniInventario.Models;
using MiniInventario.Services;

namespace MiniInventario;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _database;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = CargarProductos();
    }
    
    public MainPage(DatabaseService database)
    {
        InitializeComponent();
        _database = database;

        _ = CargarProductos();
    }

    private async Task CargarProductos()
    {
        ProductosCollection.ItemsSource = await _database.ObtenerProductosAsync();
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var producto = button.BindingContext as Producto;

        if (producto is null)
        {
            await DisplayAlertAsync("Error", "No se pudo abrir el producto para editar.", "OK");
            return;
        }

        await Navigation.PushAsync(new EditarProductoPage(_database, producto));
    }
    
    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var producto = button.BindingContext as Producto;

        if (producto is null)
        {
            await DisplayAlertAsync("Error", "No se pudo eliminar el producto.", "OK");
            return;
        }

        bool confirmacion = await DisplayAlertAsync(
            "Confirmación",
            $"¿Estás seguro de que deseas eliminar el producto '{producto.Nombre}'?",
            "Sí",
            "No"
        );

        if (!confirmacion)
        {
            return;
        }

        try
        {
            await _database.EliminarProductoAsync(producto);
            _ = CargarProductos();
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"No se pudo eliminar el producto: {ex.Message}", "OK");
        }
    }
    private async void OnAgregarClicked(object sender, EventArgs e)
    {
        string nombre = NombreEntry.Text?.Trim().ToLower() ?? "";

        if (nombre.Length < 3 || nombre.Length > 50 || !nombre.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
        {
            await DisplayAlertAsync("Validación", "El nombre del producto debe tener entre 3 y 50 caracteres y solo puede contener letras y espacios.", "OK");
            return;
        }

        if (!float.TryParse(
                PrecioVentaEntry.Text,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out float precioVenta)
            || precioVenta <= 0)
        {
            await DisplayAlertAsync("Validación", "Introduce un precio válido.", "OK");
            return;
        }

        if (!float.TryParse(
                PrecioCompraEntry.Text,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out float precioCompra)
            || precioCompra <= 0 || precioCompra > precioVenta)
        {
            await DisplayAlertAsync("Validación", "Introduce un precio válido.", "OK");
            return;
        }
        if (!float.TryParse(CantidadEntry.Text, out float cantidad)
            || cantidad < 0)
        {
            await DisplayAlertAsync("Validación", "Introduce una cantidad válida.", "OK");
            return;
        }

        var producto = new Producto
        {
            Nombre = nombre,
            Descripcion = string.Empty,
            PrecioCompra = precioCompra,
            PrecioVenta = precioVenta,
            Cantidad = cantidad,
            UnidadMedicion = string.Empty
        };

        try
        {
            await _database.AgregarProductoAsync(producto);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"No se pudo guardar el producto: {ex.Message}", "OK");
            return;
        }
        await DisplayAlertAsync("Éxito", "Producto agregado correctamente.", "OK");

        NombreEntry.Text = string.Empty;
        PrecioCompraEntry.Text = string.Empty;
        PrecioVentaEntry.Text = string.Empty;
        CantidadEntry.Text = string.Empty;

        await CargarProductos();
    }
}
