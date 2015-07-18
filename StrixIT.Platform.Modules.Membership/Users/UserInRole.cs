//-----------------------------------------------------------------------
// <copyright file="UserInRole.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The class to track user role membership.
    /// </summary>
    public class UserInRole : ValidationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRole" /> class.
        /// </summary>
        /// <param name="groupRole">The group role that this user role is based on</param>
        /// <param name="userId">The id of the user</param>
        public UserInRole(GroupInRole groupRole, Guid userId) : this(groupRole, userId, DateTime.Now, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRole" /> class.
        /// </summary>
        /// <param name="groupRole">The group role that this user role is based on</param>
        /// <param name="userId">The id of the role</param>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        public UserInRole(GroupInRole groupRole, Guid userId, DateTime startDate, DateTime? endDate)
        {
            if (groupRole == null)
            {
                throw new ArgumentNullException("groupRole");
            }

            this.GroupRole = groupRole;
            this.GroupRoleGroupId = groupRole.GroupId;
            this.GroupRoleRoleId = groupRole.RoleId;
            this.UserId = userId;
            this.StartDate = startDate;
            this.EndDate = endDate;
        }

        private UserInRole() { }

        /// <summary>
        /// Gets the group role group Id.
        /// </summary>
        [StrixRequired]
        public Guid GroupRoleGroupId { get; private set; }

        /// <summary>
        /// Gets the group role role Id.
        /// </summary>
        [StrixRequired]
        public Guid GroupRoleRoleId { get; private set; }

        /// <summary>
        /// Gets the group role.
        /// </summary>
        public GroupInRole GroupRole { get; private set; }

        /// <summary>
        /// Gets the user Id.
        /// </summary>
        [StrixRequired]
        public Guid UserId { get; private set; }

        /// <summary>
        /// Gets the user.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// Gets or sets the start date for the user in the role.
        /// </summary>
        [StrixNotDefault]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for the user in the role.
        /// </summary>
        [StrixNotDefault]
        public DateTime? EndDate { get; set; }
    }
}