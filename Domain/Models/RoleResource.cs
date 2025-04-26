using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class RoleResource
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public string ResourceKey { get; set; } = null!;

    public int Action { get; set; }

    public virtual Resource ResourceKeyNavigation { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
