//-----------------------------------------------------------------------
// <copyright file="RegistrationConfiguration.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class RegistrationConfiguration
    {
        private static IDictionary<string, string> _settings = ModuleManager.AppSettings.ContainsKey(MembershipConstants.MEMBERSHIP) ? ModuleManager.AppSettings[MembershipConstants.MEMBERSHIP] : ModuleManager.AppSettings[PlatformConstants.PLATFORM];

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

        public bool UseTerms
        {
            get
            {
                return bool.Parse(_settings["useTerms"]);
            }
        }

        public bool UseRegistrationComment
        {
            get
            {
                return bool.Parse(_settings["useRegistrationComment"]);
            }
        }
    }
}