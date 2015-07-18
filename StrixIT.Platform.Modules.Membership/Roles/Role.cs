//-----------------------------------------------------------------------
// <copyright file="Role.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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
