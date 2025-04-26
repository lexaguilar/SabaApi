using System;
using System.Collections.Generic;

namespace Saba.Domain.ViewModels;

public partial class UserPartialModel
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string UserName { get; set; } = null!;

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime LastLoginDate { get; set; }

    public DateTime LastPasswordChangedDate { get; set; }

    public DateTime LastLockoutDate { get; set; }

    public int FailedPasswordAttemptCount { get; set; }

    public DateTime FailedPasswordAttemptWindowStart { get; set; }
}

public partial class UserModel : UserPartialModel
{

    public string Password { get; set; } = null!;

    public string PasswordSalt { get; set; } = null!;

    public string? TempToken { get; set; }

    public DateTime TempTokenExpiration { get; set; }

}
