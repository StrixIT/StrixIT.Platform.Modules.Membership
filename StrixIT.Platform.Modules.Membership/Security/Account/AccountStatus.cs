#region Apache License

//-----------------------------------------------------------------------
// <copyright file="AccountStatus.cs" company="StrixIT">
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

using System;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A class to hold the account status data for a user. Used to combine this status information
    /// with user data when listing users. This is needed, because the data is from different
    /// tables, there is no navigational property from a user to his security data, and the security
    /// data is internal.
    /// </summary>
    public class AccountStatus
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the user account is approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Gets or sets the id of the user this status data is for.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user account is locked out.
        /// </summary>
        public bool LockedOut { get; set; }

        #endregion Public Properties
    }
}