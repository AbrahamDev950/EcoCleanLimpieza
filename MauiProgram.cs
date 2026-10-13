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
		builder.Services.AddSingleton<MainPage>();
		return builder.Build();
	}
}
