using System;
using System.Collections.Generic;

namespace Saba.Domain.Models;

public partial class SurveyUser
{
    public int Id { get; set; }

    public int SurveyId { get; set; }

    public int UserId { get; set; }

    public int FilialId { get; set; }

    /// <summary>
    /// 1 Pendiente, 2 Proceso, 3 Finalizada 4 Incompleto
    /// </summary>
    public int SurveyUserStateId { get; set; }

    public string? Observation { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? EditedAt { get; set; }

    public int? EditedByUserId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public decimal? Distance { get; set; }

    public string? AdministratorNameFilial { get; set; }

    public string? OwnerFilial { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual Filial Filial { get; set; } = null!;

    public virtual Survey Survey { get; set; } = null!;

    public virtual ICollection<SurveyUserResponse> SurveyUserResponses { get; set; } = new List<SurveyUserResponse>();

    public virtual SurveyUserState SurveyUserState { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
