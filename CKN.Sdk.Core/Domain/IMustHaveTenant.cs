namespace CKN.Sdk.Core.Domain;

/// <summary>
/// Ensures that the implementing entity is strictly isolated by TenantId.
/// Used for Global Query Filters in Entity Framework Core to guarantee Zero-Trust multi-tenancy.
/// </summary>
public interface IMustHaveTenant
{
    /// <summary>
    /// The unique identifier of the tenant owning this record.
    /// </summary>
    Guid TenantId { get; set; }
}
