#region Apache License

//-----------------------------------------------------------------------
// <copyright file="User.cs" company="StrixIT">
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
    /// The user class.
    /// </summary>
    public class User : ValidationBase
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <param name="email">The user's email</param>
        /// <param name="name">The user's name</param>
        public User(Guid id, string email, string name)
        {
            this.Id = id;
            this.Email = email;
            this.Name = name;
        }

        #endregion Public Constructors

        #region Private Constructors

        private User()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the date at which the user has accepted the terms of usage.
        /// </summary>
        [StrixNotDefault]
        public DateTime? DateAcceptedTerms { get; set; }

        /// <summary>
        /// Gets user's email address.
        /// </summary>
        [StrixRequired]
        [StringLength(250)]
        public string Email { get; internal set; }

        /// <summary>
        /// Gets the user Id.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the user's name.
        /// </summary>
        [StringLength(250)]
        public string Name { get; internal set; }

        /// <summary>
        /// Gets or sets the user's preferred culture.
        /// </summary>
        [StrixRequired]
        [StringLength(5)]
        public string PreferredCulture { get; set; }

        /// <summary>
        /// Gets or sets the profile values of this user.
        /// </summary>
        public ICollection<UserProfileValue> ProfileValues { get; set; }

        /// <summary>
        /// Gets or sets the roles this user is in.
        /// </summary>
        public ICollection<UserInRole> Roles { get; set; }

        #endregion Public Properties
    }
}