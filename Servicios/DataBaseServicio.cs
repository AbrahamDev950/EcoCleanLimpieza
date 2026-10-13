using SQLite;
using MiniInventario.Models;

namespace MiniInventario.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    private class TableInfo
    {
        public string Name { get; set; } = string.Empty;
    }

    public DatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        EnsureProductoTableSchema();
        _database.CreateTableAsync<Venta>().GetAwaiter().GetResult(); // NUEVO
    }

    private void EnsureProductoTableSchema()
    {
        // ... sin cambios ...
    }

    public Task<List<Producto>> ObtenerProductosAsync()
    {
        return _database.Table<Producto>()
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public Task<int> AgregarProductoAsync(Producto producto)
    {
        return _database.InsertAsync(producto);
    }

    public Task<int> ActualizarProductoAsync(Producto producto)
    {
        return _database.UpdateAsync(producto);
    }

    public Task<int> EliminarProductoAsync(Producto producto)
    {
        return _database.DeleteAsync(producto);
    }

    // ---------- NUEVO: ventas ----------

    public Task RegistrarVentaAsync(int productoId, int cantidad)
    {
        return _database.RunInTransactionAsync(conn =>
        {
            if (cantidad <= 0)
                throw new InvalidOperationException("La cantidad debe ser mayor a 0.");

            var producto = conn.Find<Producto>(productoId)
                ?? throw new InvalidOperationException("Producto no encontrado.");

            if (producto.Cantidad < cantidad)
                throw new InvalidOperationException(
                    $"Stock insuficiente. Disponible: {producto.Cantidad}.");

            producto.Cantidad -= cantidad;
            conn.Update(producto);

            conn.Insert(new Venta
            {
                ProductoId = producto.Id,
                ProductoNombre = producto.Nombre,
                Cantidad = cantidad,
                PrecioVenta = producto.PrecioVenta,
                PrecioCompra = producto.PrecioCompra,
                Fecha = DateTime.Now
            });
        });
    }

    public Task<List<Venta>> ObtenerVentasAsync()
    {
        return _database.Table<Venta>()
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();
    }
}