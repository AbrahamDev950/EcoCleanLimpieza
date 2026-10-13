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
    }

    private void EnsureProductoTableSchema()
    {
        var columns = _database.QueryAsync<TableInfo>("PRAGMA table_info('Producto')").GetAwaiter().GetResult();
        bool hasDescripcion = columns.Any(c => c.Name == nameof(Producto.Descripcion));
        bool hasUnidadMedicion = columns.Any(c => c.Name == nameof(Producto.UnidadMedicion));

        if (hasDescripcion && hasUnidadMedicion)
        {
            return;
        }

        try
        {
            _database.ExecuteAsync("ALTER TABLE Producto RENAME TO Producto_Old").Wait();
            _database.CreateTableAsync<Producto>().Wait();
            _database.ExecuteAsync(
                "INSERT INTO Producto (Id, Nombre, Descripcion, PrecioVenta, PrecioCompra, Cantidad, UnidadMedicion) " +
                "SELECT Id, Nombre, '', PrecioVenta, PrecioCompra, Cantidad, '' FROM Producto_Old").Wait();
            _database.ExecuteAsync("DROP TABLE Producto_Old").Wait();
        }
        catch
        {
            _database.CreateTableAsync<Producto>().Wait();
        }
    }

    public Task<List<Producto>> ObtenerProductosAsync()
    {
        // Primero ordenamos por nombre y luego convertimos a lista de manera asíncrona
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
}