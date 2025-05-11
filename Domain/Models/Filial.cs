using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class Filial
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Lat { get; set; }

    public string? Lng { get; set; }

    public bool Active { get; set; }
}
