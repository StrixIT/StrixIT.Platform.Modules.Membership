#region Apache License
//-----------------------------------------------------------------------
// <copyright file="UserSessionStorage.cs" company="StrixIT">
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
    /// A class to store the user's session data in the database.
    /// </summary>
    internal class UserSessionStorage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSessionStorage" /> class.
        /// </summary>
        /// <param name="userId">The id of the user the session data is for</param>
        public UserSessionStorage(Guid userId)
        {
            this.Id = userId;
        }

        private UserSessionStorage() { }

        /// <summary>
        /// Gets the id of the user the session data is for.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets or sets the user the session data is for.
        /// </summary>
        public User User { get; internal set; }

        /// <summary>
        /// Gets the user security the session data is for.
        /// </summary>
        public UserSecurity UserSecurity { get; private set; }

        /// <summary>
        /// Gets or sets the session data in JSON format.
        /// </summary>
        public string Session { get; set; }
    }
}