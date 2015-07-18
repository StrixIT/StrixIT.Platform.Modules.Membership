//-----------------------------------------------------------------------
// <copyright file="Permission.cs" company="StrixIT">
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
    /// The class for a role permission.
    /// </summary>
    public class Permission
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Permission" /> class.
        /// </summary>
        /// <param name="name">The permission name</param>
        public Permission(string name) : this(Guid.Empty, Guid.Empty, name) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Permission" /> class.
        /// </summary>
        /// <param name="id">The permission id</param>
        /// <param name="applicationId">The id of the application the permission belongs to</param>
        /// <param name="name">The permission name</param>
        public Permission(Guid id, Guid applicationId, string name)
        {
            this.Id = id;
            this.ApplicationId = applicationId;
            this.Name = name;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="Permission" /> class from being created.
        /// </summary>
        private Permission() { }

        /// <summary>
        /// Gets the id of the permission.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the id of the application the permission is for.
        /// </summary>
        [StrixRequired]
        public Guid ApplicationId { get; private set; }

        /// <summary>
        /// Gets the application the permission is for.
        /// </summary>
        public Application Application { get; private set; }

        /// <summary>
        /// Gets the permission name.
        /// </summary>
        [StrixRequired]
        [StringLength(100)]
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the roles that have this permission.
        /// </summary>
        public ICollection<Role> Roles { get; set; }
    }
}
