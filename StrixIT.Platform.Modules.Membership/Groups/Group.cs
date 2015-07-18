//-----------------------------------------------------------------------
// <copyright file="Group.cs" company="StrixIT">
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
    /// Groups are used to group users on for example company membership. Groups can be nested.
    /// </summary>
    public class Group : ValidationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Group" /> class.
        /// </summary>
        /// <param name="id">The id of the group</param>
        /// <param name="name">The name of the group</param>
        public Group(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        private Group() { }

        /// <summary>
        /// Gets the group Id.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the name of the Group.
        /// </summary>
        [StrixRequired]
        [StringLength(250)]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the group uses permissions or not. If not, roles are used.
        /// </summary>
        public bool UsePermissions { get; internal set; }

        /// <summary>
        /// Gets or sets the roles of the group.
        /// </summary>
        public ICollection<GroupInRole> Roles { get; set; }

        /// <summary>
        /// Gets or sets the roles the group has created.
        /// </summary>
        public ICollection<Role> CustomRoles { get; set; }
    }
}
