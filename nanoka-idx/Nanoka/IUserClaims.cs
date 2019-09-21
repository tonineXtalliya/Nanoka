using System.Collections.Generic;
using Nanoka.Models;

namespace Nanoka
{
    public interface IUserClaims
    {
        string Id { get; }
        UserPermissions Permissions { get; }
        double Reputation { get; }
        bool IsRestricted { get; }

        IReadOnlyDictionary<string, string> QueryParams { get; }
    }

    public static class UserClaimsExtensions
    {
        public static bool IsAdministrator(this IUserClaims claims)
            => claims.Permissions.HasFlag(UserPermissions.Administrator);

        public static bool HasPermissions(this IUserClaims claims, UserPermissions required)
            => claims.IsAdministrator() || claims.Permissions.HasFlag(required);

        public static bool HasAnyPermission(this IUserClaims claims, UserPermissions requiredAny)
            => claims.IsAdministrator() || (claims.Permissions & requiredAny) != 0;

        public static string GetReason(this IUserClaims claims) => claims.QueryParams.GetValueOrDefault("reason");
    }
}