//-----------------------------------------------------------------------
// <copyright file="ValidateUserResult.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// An enumeration for the possible results when saving a user.
    /// </summary>
    public enum ValidateUserResult
    {
        Error,
        Valid,
        Invalid,
        LockedOut,
        Unapproved,
        NoRoles
    }
}