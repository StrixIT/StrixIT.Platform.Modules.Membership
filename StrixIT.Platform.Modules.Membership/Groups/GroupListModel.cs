#region Apache License
//-----------------------------------------------------------------------
// <copyright file="GroupListModel.cs" company="StrixIT">
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

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for displaying a list of groups.
    /// </summary>
    public class GroupListModel
    {
        /// <summary>
        /// Gets or sets the id of the group.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of users.
        /// </summary>
        public int? MaxNumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the group has been selected.
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Gets or sets the names of the roles the group is in.
        /// </summary>
        public string[] RoleNames { get; set; }
    }
}
