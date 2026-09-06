using System;
using Microsoft.AspNetCore.Identity;

namespace CKN.Sdk.Infrastructure.Identity;

/// <summary>
/// Represents a Role in the CKN System. Extends default IdentityRole.
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName) : base(roleName) { }
}
