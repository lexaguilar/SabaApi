namespace Saba.Domain.ViewModels;

public class FileRequestModel
{
    public int Id { get; set; }
    public IFormFile File { get; set; } = null!;
}

public class FileResponseModel
{   
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
}

public class FilePageResponseModel {
    public int TotalCount { get; set; }
    public IEnumerable<FileResponseModel> Items { get; set; }
}