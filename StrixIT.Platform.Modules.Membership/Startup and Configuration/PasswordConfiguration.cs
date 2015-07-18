#region Apache License
//-----------------------------------------------------------------------
// <copyright file="PasswordConfiguration.cs" company="StrixIT">
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

using System.Collections.Generic;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class PasswordConfiguration
    {
        private static IDictionary<string, string> _settings = ModuleManager.AppSettings.ContainsKey(MembershipConstants.MEMBERSHIP) ? ModuleManager.AppSettings[MembershipConstants.MEMBERSHIP] : ModuleManager.AppSettings[PlatformConstants.PLATFORM];
        
        public int MinRequiredPasswordLength
        {
            get
            {
                return int.Parse(_settings["minRequiredPasswordLength"]);
            }
        }

        public int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return int.Parse(_settings["minRequiredNonAlphanumericCharacters"]);
            }
        }

        public int PasswordHashIterations
        {
            get
            {
                // todo: actually use this setting.
                return int.Parse(_settings["passwordHashIterations"]);
            }
        }

        public int MaxInvalidPasswordAttempts
        {
            get
            {
                return int.Parse(_settings["maxInvalidPasswordAttempts"]);
            }
        }

        /// <summary>
        /// Gets the window in minutes in which the maximum number of invalid password attempts is tracked.
        /// </summary>
        public int PasswordAttemptWindow
        {
            get
            {
                return int.Parse(_settings["passwordAttemptWindow"]);
            }
        }

        /// <summary>
        /// Gets the window in minutes in which the set password verification id is valid.
        /// </summary>
        public int VerificationIdValidWindow
        {
            get
            {
                return int.Parse(_settings["verificationIdValidWindow"]);
            }
        }
    }
}