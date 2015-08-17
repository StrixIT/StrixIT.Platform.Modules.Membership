#region Apache License

//-----------------------------------------------------------------------
// <copyright file="GroupViewModel.cs" company="StrixIT">
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
    /// A view model for creating and editing groups.
    /// </summary>
    public class GroupViewModel : IViewModel
    {
        #region Public Properties

        public bool CanDelete { get; set; }

        public bool CanEdit { get; set; }

        public Type EntityType
        {
            get
            {
                return typeof(Group);
            }
        }

        /// <summary>
        /// Gets or sets the id of the group.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets all available permissions.
        /// </summary>
        public IList<AssignPermissionModel> Permissions { get; set; }

        /// <summary>
        /// Gets or sets the permission set end date.
        /// </summary>
        public DateTime? PermissionSetEndDate { get; set; }

        /// <summary>
        /// Gets or sets the permission set maximum number of users.
        /// </summary>
        public int? PermissionSetMaxNumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets the permission set start date.
        /// </summary>
        public DateTime? PermissionSetStartDate { get; set; }

        /// <summary>
        /// Gets or sets all available roles.
        /// </summary>
        public IList<AssignRoleModel> Roles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether permissions are to be used for the group.
        /// </summary>
        public bool UsePermissions { get; set; }

        #endregion Public Properties
    }
}