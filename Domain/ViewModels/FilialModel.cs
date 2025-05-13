namespace Saba.Domain.ViewModels;

public partial class FilialRequestModel
{
    public int Id { get; set; }

    public string InternalCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Lat { get; set; }

    public string? Lng { get; set; }

    public bool Active { get; set; }
}

public partial class FilialPageResponseModel
{
    public int TotalCount { get; set; }

    public IEnumerable<FilialRequestModel> Items { get; set; } = null!;
}