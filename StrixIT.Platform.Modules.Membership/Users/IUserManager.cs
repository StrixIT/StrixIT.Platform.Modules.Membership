#region Apache License
//-----------------------------------------------------------------------
// <copyright file="IUserManager.cs" company="StrixIT">
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

using System;
using System.Collections.Generic;
using System.Linq;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The user manager interface.
    /// </summary>
    public interface IUserManager
    {
        /// <summary>
        /// Updates the logged in user list with the specified user.
        /// </summary>
        /// <param name="user">The user to update the list with</param>
        void UpdateLoggedInUser(User user);

        /// <summary>
        /// Removes the logged in user entry from the logged in user list.
        /// </summary>
        /// <param name="id">The id of the user to remove</param>
        void RemoveLoggedInUser(Guid id);

        /// <summary>
        /// Gets the number of users online, optionally for a specific group.
        /// </summary>
        /// <param name="groupId">The group id.</param>
        /// <returns>The number of online users</returns>
        int GetNumberOfUsersOnline(Guid? groupId = null);

        /// <summary>
        /// Gets the id of a user using his email.
        /// </summary>
        /// <param name="email">The user email</param>
        /// <returns>The user id</returns>
        Guid? GetId(string email);

        /// <summary>
        /// Gets the email address for a user.
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>The user's email address</returns>
        string GetEmail(Guid id);

        /// <summary>
        /// Gets a user's full name.
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>The user's full name</returns>
        string GetName(Guid id);

        /// <summary>
        /// Gets a user by his id.
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>The user</returns>
        User Get(Guid id);

        /// <summary>
        /// Gets a user by his email.
        /// </summary>
        /// <param name="email">The user email</param>
        /// <returns>The user</returns>
        User Get(string email);

        /// <summary>
        /// Gets a user by his email. Optionally, users that have no roles and are in the main group are included.
        /// </summary>
        /// <param name="email">The user email</param>
        /// <param name="getForMainGroup">True when users that have no roles and are in the main group should be included, false otherwise</param>
        /// <returns>The user</returns>
        User Get(string email, bool getForMainGroup);

        /// <summary>
        /// Gets a user query.
        /// </summary>
        /// <returns>The user query</returns>
        IQueryable<User> Query();

        /// <summary>
        /// Gets a user profile query.
        /// </summary>
        /// <returns>The user profile query</returns>
        IQueryable<UserProfileValue> ProfileQuery();

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="email">The e-mail</param>
        /// <param name="preferredCulture">The culture preferred by the user</param>
        /// <param name="password">The password</param>
        /// <param name="isApproved">True if the user is approved, false otherwise</param>
        /// <param name="acceptedTerms">True if the user accepted the terms of use, false otherwise</param>
        /// <param name="registrationComment">The reason the user gave for registration</param>
        /// <returns>The new user</returns>
        User Create(string name, string email, string preferredCulture, string password, bool isApproved, bool acceptedTerms, string registrationComment);

        /// <summary>
        /// Updates a user
        /// </summary>
        /// <param name="id">The id of the user to update</param>
        /// <param name="name">The (new) name</param>
        /// <param name="email">The (new) e-mail address</param>
        /// <param name="preferredCulture">The culture preferred by the user</param>
        /// <returns>The updated user</returns>
        User Update(Guid id, string name, string email, string preferredCulture);

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The user id</param>
        void Delete(Guid id);

        /// <summary>
        /// Gets the session stored in the database for the specified user.
        /// </summary>
        /// <param name="email">The email of the user to get the session for</param>
        void GetSession(string email);

        /// <summary>
        /// Saves the session values for the specified user to the database.
        /// </summary>
        /// <param name="userId">The id of the user to save the session for</param>
        /// <param name="sessionValues">A dictionary containing the session values</param>
        void SaveSession(Guid userId, IDictionary<string, object> sessionValues);
    }
}