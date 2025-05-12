using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class Client
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public bool Active { get; set; }

    public int TypeId { get; set; }

    public DateTime DateAdded { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? EditedAt { get; set; }

    public int? EditedByUserId { get; set; }
}
