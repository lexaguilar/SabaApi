using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Saba.Domain.Models;

public partial class SabaContext : DbContext
{
    public SabaContext()
    {
    }

    public SabaContext(DbContextOptions<SabaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CatalogName> CatalogNames { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Filial> Filials { get; set; }

    public virtual DbSet<FilialUser> FilialUsers { get; set; }

    public virtual DbSet<GenericCatalog> GenericCatalogs { get; set; }

    public virtual DbSet<QuestionType> QuestionTypes { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleResource> RoleResources { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<SurveyState> SurveyStates { get; set; }

    public virtual DbSet<SurveyUser> SurveyUsers { get; set; }

    public virtual DbSet<SurveyUserResponse> SurveyUserResponses { get; set; }

    public virtual DbSet<SurveyUserResponseFile> SurveyUserResponseFiles { get; set; }

    public virtual DbSet<SurveyUserState> SurveyUserStates { get; set; }

    public virtual DbSet<Template> Templates { get; set; }

    public virtual DbSet<TemplateQuestion> TemplateQuestions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=SQL8020.site4now.net;Database=db_aad223_saba;User Id=db_aad223_saba_admin;Password=saba12345;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogName>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EditedAt).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Amount).HasColumnType("money");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DateAdded).HasColumnType("datetime");
            entity.Property(e => e.EditedAt).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Filial>(entity =>
        {
            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EditedAt).HasColumnType("datetime");
            entity.Property(e => e.InternalCode)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Lat).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Lng).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FilialUser>(entity =>
        {
            entity.HasOne(d => d.Filial).WithMany(p => p.FilialUsers)
                .HasForeignKey(d => d.FilialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FilialUsers_Filials");

            entity.HasOne(d => d.User).WithMany(p => p.FilialUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FilialUsers_Users");
        });

        modelBuilder.Entity<GenericCatalog>(entity =>
        {
            entity.Property(e => e.CatalogValue)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EditedAt).HasColumnType("datetime");

            entity.HasOne(d => d.CatalogName).WithMany(p => p.GenericCatalogs)
                .HasForeignKey(d => d.CatalogNameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GenericCatalogs_CatalogNames");
        });

        modelBuilder.Entity<QuestionType>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.ResourceKey);

            entity.Property(e => e.ResourceKey)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.ParentResourceKey)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.ParentResourceKeyNavigation).WithMany(p => p.InverseParentResourceKeyNavigation)
                .HasForeignKey(d => d.ParentResourceKey)
                .HasConstraintName("FK_Resources_Resources");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.EditedAt).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RoleResource>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.ResourceKey });

            entity.Property(e => e.ResourceKey)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.ResourceKeyNavigation).WithMany(p => p.RoleResources)
                .HasForeignKey(d => d.ResourceKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleResources_Resources");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleResources)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleResources_Roles");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.EditedAt).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.FinishedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.StartedDate).HasColumnType("datetime");

            entity.HasOne(d => d.SurveyState).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.SurveyStateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Surveys_SurveyStates");

            entity.HasOne(d => d.Template).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Surveys_Templates");
        });

        modelBuilder.Entity<SurveyState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SurveyState");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SurveyUser>(entity =>
        {
            entity.Property(e => e.AdministratorNameFilial)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Distance)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(38, 0)");
            entity.Property(e => e.EditedAt).HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Latitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Observation)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.OwnerFilial)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.SurveyUserStateId).HasComment("1 Pendiente, 2 Proceso, 3 Finalizada 4 Incompleto");

            entity.HasOne(d => d.Filial).WithMany(p => p.SurveyUsers)
                .HasForeignKey(d => d.FilialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyUsers_Filials");

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveyUsers)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyUsers_Surveys");

            entity.HasOne(d => d.SurveyUserState).WithMany(p => p.SurveyUsers)
                .HasForeignKey(d => d.SurveyUserStateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyUsers_SurveyUserStates");

            entity.HasOne(d => d.User).WithMany(p => p.SurveyUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyUsers_Users");
        });

        modelBuilder.Entity<SurveyUserResponse>(entity =>
        {
            entity.HasIndex(e => e.SurveyUserId, "IX_SurveyUserResponses").IsDescending();

            entity.Property(e => e.Comment)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.FileNameUploaded)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Latitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.Response)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Question).WithMany(p => p.SurveyUserResponses)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyUserResponses_TemplateQuestions");

            entity.HasOne(d => d.SurveyUser).WithMany(p => p.SurveyUserResponses)
                .HasForeignKey(d => d.SurveyUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyUserResponses_SurveyUsers");
        });

        modelBuilder.Entity<SurveyUserResponseFile>(entity =>
        {
            entity.HasIndex(e => e.SurveyUserResponseId, "IX_SurveyUserResponseFiles").IsDescending();

            entity.Property(e => e.ContentType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FileNameUploaded)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.SurveyUserResponse).WithMany(p => p.SurveyUserResponseFiles)
                .HasForeignKey(d => d.SurveyUserResponseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SurveyUserResponseFiles_SurveyUserResponses");
        });

        modelBuilder.Entity<SurveyUserState>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_SurveyUserState");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.EditedAt).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TemplateCode)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TemplateQuestion>(entity =>
        {
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.HasOne(d => d.CatalogName).WithMany(p => p.TemplateQuestions)
                .HasForeignKey(d => d.CatalogNameId)
                .HasConstraintName("FK_TemplateQuestions_CatalogNames");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_TemplateQuestions_TemplateQuestions");

            entity.HasOne(d => d.QuestionType).WithMany(p => p.TemplateQuestions)
                .HasForeignKey(d => d.QuestionTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TemplateQuestions_QuestionTypes");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Memberships");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FailedPasswordAttemptWindowStart).HasColumnType("datetime");
            entity.Property(e => e.LastLockoutDate).HasColumnType("datetime");
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastPasswordChangedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.PasswordSalt)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.TempToken)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TempTokenExpiration).HasColumnType("datetime");
            entity.Property(e => e.UserName)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
