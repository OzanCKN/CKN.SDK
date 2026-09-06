using System;
using System.ComponentModel.DataAnnotations;

namespace CKN.Sdk.Infrastructure.Identity;

/// <summary>
/// Represents a Tenant in the Multi-Tenant architecture (e.g. a Corporate Customer).
/// </summary>
public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(256)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// For example: "Free", "Standard", "Enterprise"
    /// </summary>
    [MaxLength(50)]
    public string SubscriptionType { get; set; } = "Free";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
