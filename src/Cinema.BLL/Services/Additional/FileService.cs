using Microsoft.AspNetCore.Http;

namespace Cinema.BLL.Services.Additional;

public class FileService : IFileService
{
    private readonly string _uploadPath;
    private string FullPath
    {
        get => $"{_uploadPath}\\{FileName}";
    }
    private IFormFile? _image;
    public string FileName { get; set; }

    public FileService(IFormFile image, string uploadPath)
    {
        _image = image;
        _uploadPath = uploadPath;
        Directory.CreateDirectory(uploadPath);

        int idx = _image.FileName.LastIndexOf(".");
        string fileNameExtension = image.FileName.Substring(idx);
        FileName = Guid.NewGuid().ToString("N") + fileNameExtension;
    }

    public FileService(string uploadPath, string fileName)
    {
        _uploadPath = uploadPath;
        FileName = fileName;
    }

    public async Task SaveAsync()
    {
        using var fileStream = new FileStream(FullPath, FileMode.Create);

        if (_image is not null)
        {
            await _image.CopyToAsync(fileStream);
        }
    }

    public void Delete()
    {
        if (File.Exists(FullPath))
        {
            File.Delete(FullPath);
        }
    }

    public static void ClearAll(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            Directory.Delete(directoryPath, true);
        }
    }
}
