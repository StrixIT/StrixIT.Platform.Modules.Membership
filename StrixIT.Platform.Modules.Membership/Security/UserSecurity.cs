#region Apache License

//-----------------------------------------------------------------------
// <copyright file="UserSecurity.cs" company="StrixIT">
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
using System.ComponentModel.DataAnnotations;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A class to track user security data.
    /// </summary>
    internal class UserSecurity
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSecurity"/> class.
        /// </summary>
        /// <param name="userId">The id of the user this security record is for</param>
        public UserSecurity(Guid userId)
        {
            this.Id = userId;
        }

        #endregion Public Constructors

        #region Private Constructors

        private UserSecurity()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the user is approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Gets or sets the number of failed password attempts.
        /// </summary>
        public int FailedPasswordAttemptCount { get; set; }

        /// <summary>
        /// Gets or sets the start date and time of the failed password attempt counter.
        /// </summary>
        [StrixNotDefault]
        public DateTime? FailedPasswordAttemptWindowStart { get; set; }

        /// <summary>
        /// Gets the security user id.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets or sets the date at which the user last logged in.
        /// </summary>
        [StrixNotDefault]
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is locked out.
        /// </summary>
        public bool LockedOut { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [StrixRequired]
        [StringLength(256)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the comment the user entered when registering himself.
        /// </summary>
        [StringLength(500)]
        public string RegistrationComment { get; set; }

        /// <summary>
        /// Gets the security user.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// Gets or sets the verification id currently set for the user for validating email or
        /// password reset requests.
        /// </summary>
        [StrixNotDefault]
        public Guid? VerificationId { get; set; }

        /// <summary>
        /// Gets or sets the start of the verification time window currently set for the user for
        /// validating email or password reset requests.
        /// </summary>
        [StrixNotDefault]
        public DateTime? VerificationWindowStart { get; set; }

        #endregion Public Properties
    }
}