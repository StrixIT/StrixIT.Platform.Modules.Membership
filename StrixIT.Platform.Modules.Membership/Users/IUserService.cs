#region Apache License

//-----------------------------------------------------------------------
// <copyright file="IUserService.cs" company="StrixIT">
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
using System.Collections.Generic;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The user service interface.
    /// </summary>
    public interface IUserService : ICrudService<Guid, UserViewModel>
    {
        #region Public Methods

        /// <summary>
        /// Gets the profile for the user with the specified id.
        /// </summary>
        /// <param name="id">The id of the user to get the profile for</param>
        /// <returns>The user's profile</returns>
        dynamic GetProfile(Guid id);

        /// <summary>
        /// Gets a list with all profiles for all users of the group the current user belongs to.
        /// </summary>
        /// <returns>The list of profiles</returns>
        IList<dynamic> GetProfileList();

        #endregion Public Methods
    }
}