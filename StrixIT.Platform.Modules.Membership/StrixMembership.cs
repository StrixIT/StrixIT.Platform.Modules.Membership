﻿#region Apache License

//-----------------------------------------------------------------------
// <copyright file="StrixMembership.cs" company="StrixIT">
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

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A class to allow easy access to membership functionality.
    /// </summary>
    public static class StrixMembership
    {
        #region Private Fields

        private static MembershipConfiguration _config = new MembershipConfiguration();

        #endregion Private Fields

        #region Public Properties

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

        #endregion Public Properties
    }
}