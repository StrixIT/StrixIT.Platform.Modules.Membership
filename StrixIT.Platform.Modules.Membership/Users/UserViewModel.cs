#region Apache License

//-----------------------------------------------------------------------
// <copyright file="UserViewModel.cs" company="StrixIT">
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
    /// A view model for creating and editing users.
    /// </summary>
    public class UserViewModel : BaseCrudDto
    {
        #region Public Constructors

        public UserViewModel() : base(typeof(User))
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether this user account is approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Gets or sets the date at which the user has accepted the terms.
        /// </summary>
        public DateTime? DateAcceptedTerms { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets a value indicating whether the user has accepted the terms.
        /// </summary>
        public bool HasAcceptedTerms
        {
            get
            {
                return this.DateAcceptedTerms.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the id of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is a user manager. Used to hide the
        /// start and end date, as well as the delete button for these users when edited by other
        /// user managers.
        /// </summary>
        public bool IsCompanyManager { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user account is locked.
        /// </summary>
        public bool LockedOut { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user password. Used to validate account updates.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user's preferred culture.
        /// </summary>
        public string PreferredCulture { get; set; }

        /// <summary>
        /// Gets or sets all the roles available.
        /// </summary>
        public IList<AssignRoleModel> Roles { get; set; }

        #endregion Public Properties
    }
}