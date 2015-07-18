//-----------------------------------------------------------------------
// <copyright file="GroupInRole.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The class to track group role membership.
    /// </summary>
    public class GroupInRole : ValidationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupInRole" /> class.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="roleId">The id of the role</param>
        public GroupInRole(Guid groupId, Guid roleId) : this(groupId, roleId, DateTime.Now, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupInRole" /> class.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="roleId">The id of the role</param>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        public GroupInRole(Guid groupId, Guid roleId, DateTime startDate, DateTime? endDate)
        {
            this.GroupId = groupId;
            this.RoleId = roleId;
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        private GroupInRole() { }

        /// <summary>
        /// Gets the group id.
        /// </summary>
        [StrixRequired]
        public Guid GroupId { get; private set; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        public Group Group { get; private set; }

        /// <summary>
        /// Gets the role id.
        /// </summary>
        [StrixRequired]
        public Guid RoleId { get; private set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Gets or sets the start date and time for the group in the role.
        /// </summary>
        [StrixNotDefault]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date and time for the group in the role.
        /// </summary>
        [StrixNotDefault]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of users that can be part of the group.
        /// </summary>
        [StrixNotDefault]
        public int? MaxNumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets the current number of users in the group.
        /// </summary>
        public int CurrentNumberOfUsers { get; set; }
    }
}
