using System;
using Microsoft.AspNetCore.Identity;

namespace CKN.Sdk.Infrastructure.Identity;

/// <summary>
/// Represents a User in the CKN System. Extends default IdentityUser.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// The Tenant this user belongs to. If null, the user might be a global admin or an individual consumer.
    /// </summary>
    public Guid? TenantId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
