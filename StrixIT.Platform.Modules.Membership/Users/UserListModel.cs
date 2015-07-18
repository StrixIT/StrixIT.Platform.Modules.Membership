#region Apache License
//-----------------------------------------------------------------------
// <copyright file="UserListModel.cs" company="StrixIT">
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
    /// A view model for displaying a list of users.
    /// </summary>
    public class UserListModel
    {
        /// <summary>
        /// Gets or sets the id of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group the user belongs to.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is locked out.
        /// </summary>
        public bool LockedOut { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Gets or sets the names of the roles the user is in.
        /// </summary>
        public string[] RoleNames { get; set; }
    }
}