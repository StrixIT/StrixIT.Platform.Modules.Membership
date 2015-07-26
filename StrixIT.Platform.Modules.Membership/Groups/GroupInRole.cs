#region Apache License

//-----------------------------------------------------------------------
// <copyright file="GroupInRole.cs" company="StrixIT">
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
    /// The class to track group role membership.
    /// </summary>
    public class GroupInRole : ValidationBase
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupInRole"/> class.
        /// </summary>
        /// <param name="groupId">The id of the group</param>
        /// <param name="roleId">The id of the role</param>
        public GroupInRole(Guid groupId, Guid roleId) : this(groupId, roleId, DateTime.Now, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupInRole"/> class.
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

        #endregion Public Constructors

        #region Private Constructors

        private GroupInRole()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the current number of users in the group.
        /// </summary>
        public int CurrentNumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets the end date and time for the group in the role.
        /// </summary>
        [StrixNotDefault]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        public Group Group { get; private set; }

        /// <summary>
        /// Gets the group id.
        /// </summary>
        [StrixRequired]
        public Guid GroupId { get; private set; }

        /// <summary>
        /// Gets or sets the maximum number of users that can be part of the group.
        /// </summary>
        [StrixNotDefault]
        public int? MaxNumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Gets the role id.
        /// </summary>
        [StrixRequired]
        public Guid RoleId { get; private set; }

        /// <summary>
        /// Gets or sets the start date and time for the group in the role.
        /// </summary>
        [StrixNotDefault]
        public DateTime StartDate { get; set; }

        #endregion Public Properties
    }
}