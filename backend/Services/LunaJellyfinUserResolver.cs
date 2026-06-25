using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Moonfin.Server.Services;

/// <summary>
/// LUNA-PROVENANCE:
/// Source pattern: Moonfin.Server.Api.ControllerExtensions.GetUserIdFromClaims and
/// Moonfin.Server.Api.MoonfinController ResolveQueryUser/GetAllServerUserIds helpers.
/// Reason: Luna needs a reusable, controller-independent way to resolve the currently
/// authenticated Jellyfin user for Seerr session bootstrap.
/// Change type: extracted/adapted from existing Moonfin plugin patterns; no private project
/// branding or deployment-specific assumptions.
/// </summary>
public sealed class LunaJellyfinUserResolver
{
    private static readonly Type? UserManagerType =
        Type.GetType("MediaBrowser.Controller.Library.IUserManager, MediaBrowser.Controller");

    private static readonly MethodInfo? UserManagerGetUserById =
        UserManagerType?.GetMethod("GetUserById", [typeof(Guid)]);

    public Guid? GetUserIdFromClaims(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst("Jellyfin-UserId")?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    public object? ResolveUserById(HttpContext httpContext, Guid userId)
    {
        if (UserManagerType == null || UserManagerGetUserById == null)
        {
            return null;
        }

        var userManager = httpContext.RequestServices.GetService(UserManagerType);
        return userManager == null ? null : UserManagerGetUserById.Invoke(userManager, [userId]);
    }

    public string? TryGetUserName(object jellyfinUser)
    {
        return TryGetStringProperty(jellyfinUser, "Username")
            ?? TryGetStringProperty(jellyfinUser, "Name");
    }

    private static string? TryGetStringProperty(object instance, string propertyName)
    {
        try
        {
            return instance.GetType().GetProperty(propertyName)?.GetValue(instance) as string;
        }
        catch
        {
            return null;
        }
    }
}
