using System;

namespace Nanoka.Models
{
    [Flags]
    public enum UserPermissions
    {
        /// <summary>
        /// User has no special permission.
        /// </summary>
        None = 0,

        /// <summary>
        /// User is an administrator.
        /// This flag effectively enables every other flag.
        /// </summary>
        Administrator = 1 << 0
    }
}