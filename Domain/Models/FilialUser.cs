using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class FilialUser
{
    public int UserId { get; set; }

    public int FilialId { get; set; }

    public virtual Filial Filial { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
