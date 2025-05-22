namespace Saba.Domain.ViewModels;

public partial class GenericCatalogRequestModel
{
    public int Id { get; set; }

    public string CatalogName { get; set; } = null!;
    public string CatalogValue { get; set; } = null!;
}

public partial class GenericCatalogPageResponseModel
{
    public int TotalCount { get; set; }

    public IEnumerable<GenericCatalogRequestModel> Items { get; set; } = null!;
}