namespace Cinema.BLL.Services.Additional;

internal interface IFileService
{
    public string FileName { get; set; }

    public Task SaveAsync();

    public void Delete();
}
