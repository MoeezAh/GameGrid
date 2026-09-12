using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GameCollection.Application.Common.Interfaces;
using GameCollection.Domain.Common;
using GameCollection.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameCollection.Infrastructure.Data;

public class GameDbContext : IdentityDbContext<IdentityUser>
{
    private readonly ICurrentUserService _currentUserService;

    public GameDbContext(
        DbContextOptions<GameDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Game> Games => Set<Game>();
    public DbSet<Developer> Developers => Set<Developer>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Platform> Platforms => Set<Platform>();
    public DbSet<DigitalService> DigitalServices => Set<DigitalService>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Theme> Themes => Set<Theme>();
    public DbSet<Franchise> Franchises => Set<Franchise>();
    public DbSet<Series> Series => Set<Series>();

    // RBAC DbSets
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<ApplicationRole> ApplicationRoles => Set<ApplicationRole>();
    public DbSet<ApplicationRolePermission> ApplicationRolePermissions => Set<ApplicationRolePermission>();
    public DbSet<ApplicationUserRole> ApplicationUserRoles => Set<ApplicationUserRole>();

    // Decoupled User Library & Game Requests
    public DbSet<UserLibraryEntry> UserLibraryEntries => Set<UserLibraryEntry>();
    public DbSet<GameRequest> GameRequests => Set<GameRequest>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configurations from Assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure Catalog Game Many-to-Many Join Tables
        builder.Entity<Game>()
            .HasMany(g => g.Developers)
            .WithMany(d => d.Games)
            .UsingEntity(j => j.ToTable("GameDevelopers"));

        builder.Entity<Game>()
            .HasMany(g => g.Publishers)
            .WithMany(p => p.Games)
            .UsingEntity(j => j.ToTable("GamePublishers"));

        builder.Entity<Game>()
            .HasMany(g => g.Genres)
            .WithMany(g => g.Games)
            .UsingEntity(j => j.ToTable("GameGenres"));

        builder.Entity<Game>()
            .HasMany(g => g.Tags)
            .WithMany(t => t.Games)
            .UsingEntity(j => j.ToTable("GameTags"));

        builder.Entity<Game>()
            .HasMany(g => g.Themes)
            .WithMany(t => t.Games)
            .UsingEntity(j => j.ToTable("GameThemes"));

        builder.Entity<Game>()
            .HasMany(g => g.Platforms)
            .WithMany(p => p.Games)
            .UsingEntity(j => j.ToTable("GamePlatforms"));

        builder.Entity<Game>()
            .HasMany(g => g.DigitalServices)
            .WithMany(s => s.Games)
            .UsingEntity(j => j.ToTable("GameServices"));

        // Configure UserLibraryEntry relationships & precision
        builder.Entity<UserLibraryEntry>()
            .Property(u => u.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Entity<UserLibraryEntry>()
            .HasOne(u => u.Game)
            .WithMany(g => g.LibraryEntries)
            .HasForeignKey(u => u.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLibraryEntry>()
            .HasMany(u => u.Platforms)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserLibraryPlatforms"));

        builder.Entity<UserLibraryEntry>()
            .HasMany(u => u.DigitalServices)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserLibraryServices"));

        // Configure RBAC relationships
        builder.Entity<ApplicationRolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationRolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUserRole>()
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        // Configure GameRequest relationships
        builder.Entity<GameRequest>()
            .HasOne(gr => gr.CreatedGame)
            .WithMany()
            .HasForeignKey(gr => gr.CreatedGameId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure soft-delete query filters for entities inheriting from BaseAuditableEntity
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType)
                    .HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
            }
        }
    }

    private static System.Linq.Expressions.LambdaExpression ConvertFilterExpression(Type type)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseAuditableEntity.IsDeleted));
        var falseConstant = System.Linq.Expressions.Expression.Constant(false);
        var equalExpression = System.Linq.Expressions.Expression.Equal(property, falseConstant);
        return System.Linq.Expressions.Expression.Lambda(equalExpression, parameter);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId ?? "System";
                    entry.Entity.CreatedDate = DateTimeOffset.UtcNow;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedBy = _currentUserService.UserId ?? "System";
                    entry.Entity.UpdatedDate = DateTimeOffset.UtcNow;
                    break;
                
                case EntityState.Deleted:
                    // Intercept physical delete and convert to soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedBy = _currentUserService.UserId ?? "System";
                    entry.Entity.UpdatedDate = DateTimeOffset.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
