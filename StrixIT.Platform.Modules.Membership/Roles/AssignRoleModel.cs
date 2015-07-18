#region Apache License
//-----------------------------------------------------------------------
// <copyright file="AssignRoleModel.cs" company="StrixIT">
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

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A class to transfer role data and assign roles.
    /// </summary>
    public class AssignRoleModel
    {
        /// <summary>
        /// Gets or sets the role id.
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary> 
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the start date of the group for this role, when assigning roles to users.
        /// </summary> 
        public DateTime? GroupStartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the group for this role, when assigning roles to users.
        /// </summary> 
        public DateTime? GroupEndDate { get; set; }

        /// <summary>
        /// Gets or sets the start date for this role.
        /// </summary> 
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for this role.
        /// </summary> 
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of users in this role for an organisation.
        /// </summary> 
        public int? MaxNumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets the current number of users in this role for an organisation.
        /// </summary>        
        public int CurrentNumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the role has been selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        ///  Gets or sets the index of the role in the checkbox list.
        /// </summary>
        public int Index { get; set; }
    }
}
