#region Apache License
//-----------------------------------------------------------------------
// <copyright file="AssignPermissionModel.cs" company="StrixIT">
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
    /// A class to transfer permission data and assign permissions.
    /// </summary>
    public class AssignPermissionModel
    {
        /// <summary>
        /// Gets or sets the permission id.
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the permission name.
        /// </summary> 
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the permission description.
        /// </summary> 
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the permission has been selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        ///  Gets or sets the index of the permission in the checkbox list.
        /// </summary>
        public int Index { get; set; }
    }
}