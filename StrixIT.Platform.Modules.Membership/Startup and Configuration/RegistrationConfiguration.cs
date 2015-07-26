#region Apache License

//-----------------------------------------------------------------------
// <copyright file="RegistrationConfiguration.cs" company="StrixIT">
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
    public class RegistrationConfiguration
    {
        #region Private Fields

        private static IDictionary<string, string> _settings = ModuleManager.AppSettings.ContainsKey(MembershipConstants.MEMBERSHIP) ? ModuleManager.AppSettings[MembershipConstants.MEMBERSHIP] : ModuleManager.AppSettings[PlatformConstants.PLATFORM];

        #endregion Private Fields

        #region Public Properties

        public bool AllowUserRegistration
        {
            get
            {
                return bool.Parse(_settings["allowUserRegistration"]);
            }
        }

        public bool AutoApproveUsers
        {
            get
            {
                return bool.Parse(_settings["autoApproveUsers"]);
            }
        }

        public bool UseRegistrationComment
        {
            get
            {
                return bool.Parse(_settings["useRegistrationComment"]);
            }
        }

        public bool UseTerms
        {
            get
            {
                return bool.Parse(_settings["useTerms"]);
            }
        }

        #endregion Public Properties
    }
}