using Cinema.BLL.Services.Additional;
using Cinema.Test.Infrastructure.Helpers;
using System.Reflection;

namespace Cinema.Test.Infrastructure.Supports;

public class DatabaseFixture : IDisposable
{
    private readonly ApplicationContextHelper _helper;

    public string? AppDir { get; private set; }

    public DatabaseFixture()
    {
        _helper = new ApplicationContextHelper();

        AppDir = Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().Location);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool disposed = false;
    public virtual void Dispose(bool disposing)
    {

        if (disposed) return;
        if (disposing)
        {
            _helper.Context.Database.EnsureDeleted();
            _helper.Context.Dispose();

            if (AppDir is not null)
            {
                FileService.ClearAll(Path.Combine(AppDir, "ImageDb"));
            }
        }
        disposed = true;
    }

    ~DatabaseFixture()
    {
        Dispose(false);
    }
}
