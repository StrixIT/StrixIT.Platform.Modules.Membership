#region Apache License

//-----------------------------------------------------------------------
// <copyright file="UserInRole.cs" company="StrixIT">
// Copyright 2015 StrixIT. Author R.G. Schurgers MA MSc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

#endregion Apache License

using StrixIT.Platform.Core;
using System;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The class to track user role membership.
    /// </summary>
    public class UserInRole : ValidationBase
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRole"/> class.
        /// </summary>
        /// <param name="groupRole">The group role that this user role is based on</param>
        /// <param name="userId">The id of the user</param>
        public UserInRole(GroupInRole groupRole, Guid userId) : this(groupRole, userId, DateTime.Now, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInRole"/> class.
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

        #endregion Public Constructors

        #region Private Constructors

        private UserInRole()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the end date for the user in the role.
        /// </summary>
        [StrixNotDefault]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets the group role.
        /// </summary>
        public GroupInRole GroupRole { get; private set; }

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
        /// Gets or sets the start date for the user in the role.
        /// </summary>
        [StrixNotDefault]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets the user.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// Gets the user Id.
        /// </summary>
        [StrixRequired]
        public Guid UserId { get; private set; }

        #endregion Public Properties
    }
}