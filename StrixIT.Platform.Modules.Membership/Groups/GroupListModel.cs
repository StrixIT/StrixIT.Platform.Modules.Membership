//-----------------------------------------------------------------------
// <copyright file="GroupListModel.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for displaying a list of groups.
    /// </summary>
    public class GroupListModel
    {
        /// <summary>
        /// Gets or sets the id of the group.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of users.
        /// </summary>
        public int? MaxNumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the group has been selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the names of the roles the group is in.
        /// </summary>
        public string[] RoleNames { get; set; }
    }
}
