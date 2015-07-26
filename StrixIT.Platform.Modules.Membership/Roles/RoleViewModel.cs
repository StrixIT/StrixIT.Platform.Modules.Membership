#region Apache License

//-----------------------------------------------------------------------
// <copyright file="RoleViewModel.cs" company="StrixIT">
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

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for roles.
    /// </summary>
    public class RoleViewModel : BaseCrudDto
    {
        #region Public Constructors

        public RoleViewModel() : base(typeof(Role))
        {
            this.CanEdit = StrixPlatform.User.HasPermission(MembershipPermissions.EditRole);
            this.CanDelete = StrixPlatform.User.HasPermission(MembershipPermissions.DeleteRole);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the role description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the role id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the role's permissions.
        /// </summary>
        public IList<AssignPermissionModel> Permissions { get; set; }

        #endregion Public Properties
    }
}