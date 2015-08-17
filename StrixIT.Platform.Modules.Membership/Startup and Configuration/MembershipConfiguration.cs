#region Apache License

//-----------------------------------------------------------------------
// <copyright file="MembershipConfiguration.cs" company="StrixIT">
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
using System.Collections.Generic;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipConfiguration
    {
        #region Public Properties

        public bool AllowUserRegistration { get; set; }

        public bool AutoApproveUsers { get; set; }

        /// <summary>
        /// Gets a value indicating whether the authentication cookie is valid only until the
        /// browser session ends.
        /// </summary>
        public bool LimitAuthenticationToBrowserSession { get; set; }

        public string MailTemplateFolder { get; set; }

        public int MaxInvalidPasswordAttempts { get; set; }

        public int MinRequiredNonAlphanumericCharacters { get; set; }

        public int MinRequiredPasswordLength { get; set; }

        /// <summary>
        /// Gets the window in minutes in which the maximum number of invalid password attempts is tracked.
        /// </summary>
        public int PasswordAttemptWindow { get; set; }

        public int PasswordHashIterations { get; set; }

        public bool UseGroups { get; set; }

        public bool UsePermissions { get; set; }

        public bool UseRegistrationComment { get; set; }

        public bool UseTerms { get; set; }

        /// <summary>
        /// Gets the window in minutes in which the set password verification id is valid.
        /// </summary>
        public int VerificationIdValidWindow { get; set; }

        #endregion Public Properties
    }
}