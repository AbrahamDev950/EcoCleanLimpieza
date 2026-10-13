namespace MiniInventario;
using MiniInventario.Services;
using System.IO;
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		
		string dbPath = Path.Combine(
			FileSystem.AppDataDirectory,
			"inventario.db3"
		);

		builder.Services.AddSingleton(
			new DatabaseService(dbPath)
		);
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<MetricasPage>();
		builder.Services.AddTransient<VentasPage>();
		builder.Services.AddTransient<GananciasPage>();
		return builder.Build();
	}
}
