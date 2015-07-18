//-----------------------------------------------------------------------
// <copyright file="UserListModel.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for displaying a list of users.
    /// </summary>
    public class UserListModel
    {
        /// <summary>
        /// Gets or sets the id of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group the user belongs to.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is locked out.
        /// </summary>
        public bool LockedOut { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Gets or sets the names of the roles the user is in.
        /// </summary>
        public string[] RoleNames { get; set; }
    }
}