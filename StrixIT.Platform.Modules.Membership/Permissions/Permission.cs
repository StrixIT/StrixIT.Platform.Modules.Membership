#region Apache License
//-----------------------------------------------------------------------
// <copyright file="Permission.cs" company="StrixIT">
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
