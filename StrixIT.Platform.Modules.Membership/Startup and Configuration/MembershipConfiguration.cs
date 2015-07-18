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
#endregion

using System.Collections.Generic;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipConfiguration
    {
        private static IDictionary<string, string> _settings = ModuleManager.AppSettings.ContainsKey(MembershipConstants.MEMBERSHIP) ? ModuleManager.AppSettings[MembershipConstants.MEMBERSHIP] : ModuleManager.AppSettings[PlatformConstants.PLATFORM];
        private static PasswordConfiguration _password = new PasswordConfiguration();
        private static RegistrationConfiguration _registration = new RegistrationConfiguration();

        public string MailTemplateFolder
        {
            get
            {
                return _settings["mailTemplateFolder"] as string;
            }
        }

        public bool UseGroups
        {
            get
            {
                return bool.Parse(_settings["useGroups"]);
            }
        }

        public bool UsePermissions
        {
            get
            {
                return bool.Parse(_settings["usePermissions"]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the authentication cookie is valid only until the browser session ends.
        /// </summary>
        public bool LimitAuthenticationToBrowserSession
        {
            get
            {
                return bool.Parse(_settings["limitAuthenticationToBrowserSession"]);
            }
        }

        public RegistrationConfiguration Registration
        {
            get
            {
                return _registration;
            }
        }

        public PasswordConfiguration Password
        {
            get
            {
                return _password;
            }
        }
    }
}