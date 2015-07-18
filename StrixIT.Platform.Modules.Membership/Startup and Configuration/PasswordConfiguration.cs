//-----------------------------------------------------------------------
// <copyright file="PasswordConfiguration.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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