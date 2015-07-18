//-----------------------------------------------------------------------
// <copyright file="IUserService.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using StrixIT.Platform.Core;
using StrixIT.Platform.Web;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The user service interface.
    /// </summary>
    public interface IUserService : ICrudService<Guid, UserViewModel>
    {
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
    }
}