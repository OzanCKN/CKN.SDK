using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using CKN.Sdk.Core.Domain;
using CKN.Sdk.Core.Services;

namespace CKN.Sdk.EntityFramework;

/// <summary>
/// The central EF Core database context for the application.
/// Incorporates Multi-Tenancy and extensible schema generation.
/// </summary>
public abstract class CknDbContext(DbContextOptions options, ICurrentTenantService currentTenantService)
    : DbContext(options)
{
    /// <summary>
    /// Gets the current tenant service.
    /// </summary>
    public ICurrentTenantService CurrentTenantService { get; } = currentTenantService;

    /// <summary>
    /// Configures the schema and applies global query filters.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. Scan and apply all configurations (IEntityTypeConfiguration<T>) in the executing assembly.
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 2. Global Query Filter for Multi-Tenancy (Zero-Trust isolation)
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(IMustHaveTenant).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType)
                    .HasQueryFilter(CreateTenantFilter(entityType.ClrType));
            }
        }
    }

    /// <summary>
    /// Creates a lambda expression: e => e.TenantId == CurrentTenantService.TenantId
    /// </summary>
    private System.Linq.Expressions.LambdaExpression CreateTenantFilter(Type entityType)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(IMustHaveTenant.TenantId));

        var tenantServiceProperty = System.Linq.Expressions.Expression.Property(
            System.Linq.Expressions.Expression.Constant(this),
            nameof(CurrentTenantService));

        var tenantIdProperty = System.Linq.Expressions.Expression.Property(tenantServiceProperty, nameof(ICurrentTenantService.TenantId));

        var condition = System.Linq.Expressions.Expression.Equal(property, tenantIdProperty);

        return System.Linq.Expressions.Expression.Lambda(condition, parameter);
    }
}
