//-----------------------------------------------------------------------
// <copyright file="StrixMembership.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A class to allow easy access to membership functionality.
    /// </summary>
    public static class StrixMembership
    {
        private static MembershipConfiguration _config = new MembershipConfiguration();

        /// <summary>
        /// Gets the membership configuration.
        /// </summary>
        public static MembershipConfiguration Configuration
        {
            get
            {
                return _config;
            }
        }
    }
}