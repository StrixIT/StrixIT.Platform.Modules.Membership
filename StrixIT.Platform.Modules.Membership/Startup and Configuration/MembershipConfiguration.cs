//-----------------------------------------------------------------------
// <copyright file="MembershipConfiguration.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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