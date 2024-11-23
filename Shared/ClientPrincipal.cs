using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Scribe.Shared;

public class ClientPrincipal
{
    public string? IdentityProvider { get; init; }
    public string? UserId { get; init; }
    public string? UserDetails { get; init;}
    public List<string>? UserRoles { get; init; }

    public ClaimsIdentity ToClaimsIdentity()
    {
        ClaimsIdentity identity = new(IdentityProvider);
        if (UserId is not null)
        {
            identity.AddClaim(new(ClaimTypes.NameIdentifier, UserId));
        }
        if (UserDetails is not null)
        {
            identity.AddClaim(new(ClaimTypes.Name, UserDetails));
        }
        if (UserRoles is not null && UserRoles.Count > 0)
        {
            identity.AddClaims(UserRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        }
        return identity;
    }
}