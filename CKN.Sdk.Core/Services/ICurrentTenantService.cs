using System;

namespace CKN.Sdk.Core.Services;

/// <summary>
/// Service to retrieve the TenantId for the current HTTP Request or Background Job.
/// Essential for EF Core Global Query Filters to enforce Zero-Trust tenancy.
/// </summary>
public interface ICurrentTenantService
{
    /// <summary>
    /// Gets the current TenantId. Throws if the tenant context is missing.
    /// </summary>
    Guid TenantId { get; }
}
