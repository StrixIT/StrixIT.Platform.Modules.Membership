//-----------------------------------------------------------------------
// <copyright file="AuditLogType.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The enum for audit log types.
    /// </summary>
    public enum AuditLogType
    {
        LoginLogout,
        PasswordReset,
        RoleAssignment,
        IllegalOperation
    }
}
