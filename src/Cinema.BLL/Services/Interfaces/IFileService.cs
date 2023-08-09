namespace Cinema.BLL.Services.Interfaces;

internal interface IFileService
{
    public string FileName { get; set; }

    public Task SaveAsync();

    public void Delete();
}
