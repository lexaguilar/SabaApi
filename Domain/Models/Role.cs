using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool Active { get; set; }

    public virtual ICollection<RoleResource> RoleResources { get; set; } = new List<RoleResource>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
