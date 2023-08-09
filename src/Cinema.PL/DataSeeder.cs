using Cinema.BLL.Services;
using Cinema.DAL;

namespace Cinema.PL;

internal static class DataSeeder
{
    public static void Seed(this IHost host, IWebHostEnvironment environment)
    {
        using var scope = host.Services.CreateScope();
        using var contextDB = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        //contextDB.Database.EnsureDeleted();
        if (contextDB.Database.EnsureCreated())
        {
            FileService.ClearAll(environment.WebRootPath + "\\uploads");
        }
    }
}