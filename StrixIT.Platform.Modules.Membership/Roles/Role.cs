#region Apache License
//-----------------------------------------------------------------------
// <copyright file="Role.cs" company="StrixIT">
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
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The Role class.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Role" /> class.
        /// </summary>
        /// <param name="id">The role id</param>
        /// <param name="groupId">The id of the group this role is for</param>
        /// <param name="name">The role name</param>
        public Role(Guid id, Guid groupId, string name)
        {
            this.Id = id;
            this.GroupId = groupId;
            this.Name = name;
        }

        private Role() { }

        /// <summary>
        /// Gets the role id.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the id of the group the role was created by.
        /// </summary>
        [StrixNotDefault]
        public Guid GroupId { get; private set; }

        /// <summary>
        /// Gets the group the role was created by.
        /// </summary>
        public Group Group { get; private set; }

        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        [StrixRequired]
        [StringLength(250)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description for the role.
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the groups that are a member of this role.
        /// </summary>
        public ICollection<GroupInRole> Groups { get; set; }

        /// <summary>
        /// Gets or sets the permissions this role has.
        /// </summary>
        public ICollection<Permission> Permissions { get; set; }
    }
}
