#region Apache License

//-----------------------------------------------------------------------
// <copyright file="Group.cs" company="StrixIT">
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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// Groups are used to group users on for example company membership. Groups can be nested.
    /// </summary>
    public class Group : ValidationBase
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Group"/> class.
        /// </summary>
        /// <param name="id">The id of the group</param>
        /// <param name="name">The name of the group</param>
        public Group(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        #endregion Public Constructors

        #region Private Constructors

        private Group()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the roles the group has created.
        /// </summary>
        public ICollection<Role> CustomRoles { get; set; }

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
        /// Gets or sets the roles of the group.
        /// </summary>
        public ICollection<GroupInRole> Roles { get; set; }

        /// <summary>
        /// Gets a value indicating whether the group uses permissions or not. If not, roles are used.
        /// </summary>
        public bool UsePermissions { get; internal set; }

        #endregion Public Properties
    }
}