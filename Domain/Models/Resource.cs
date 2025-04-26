using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class Resource
{
    public string ResourceKey { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? ParentResourceKey { get; set; }

    public virtual ICollection<Resource> InverseParentResourceKeyNavigation { get; set; } = new List<Resource>();

    public virtual Resource? ParentResourceKeyNavigation { get; set; }

    public virtual ICollection<RoleResource> RoleResources { get; set; } = new List<RoleResource>();
}
