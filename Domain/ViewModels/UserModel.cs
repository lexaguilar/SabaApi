using System;
using System.Collections.Generic;

namespace Saba.Domain.ViewModels;

public partial class UserRequestModel
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    

    public string UserName { get; set; } = null!;

    public int RoleId { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }

    public int[] FilialIds { get; set; } = null!;

}

public partial class UserResponseModel : UserRequestModel
{
    public DateTime CreateDate { get; set; }
    public DateTime LastLoginDate { get; set; }
    public string CountryName { get; set; }

}

public partial class UserPageResponseModel
{
    public int TotalCount { get; set; }
    public IEnumerable<UserResponseModel> Items { get; set; } = null!;
}
