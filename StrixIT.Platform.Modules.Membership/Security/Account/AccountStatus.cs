//-----------------------------------------------------------------------
// <copyright file="AccountStatus.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A class to hold the account status data for a user. Used to combine this status information with user data when listing users.
    /// This is needed, because the data is from different tables, there is no navigational property from a user to his security data,
    /// and the security data is internal.
    /// </summary>
    public class AccountStatus
    {
        /// <summary>
        /// Gets or sets the id of the user this status data is for.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user account is locked out.
        /// </summary>
        public bool LockedOut { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user account is approved.
        /// </summary>
        public bool Approved { get; set; }
    }
}
