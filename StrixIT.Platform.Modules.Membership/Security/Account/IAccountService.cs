#region Apache License
//-----------------------------------------------------------------------
// <copyright file="IAccountService.cs" company="StrixIT">
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
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The Account Service interface.
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="model">The data for the new account</param>
        /// <returns>An update user result holding the data on account creation</returns>
        SaveResult<UserViewModel> RegisterAccount(RegisterViewModel model);

        /// <summary>
        /// Updates a user account.
        /// </summary>
        /// <param name="model">The account data</param>
        /// <returns>An update user result holding the data on the update</returns>
        SaveResult<UserViewModel> UpdateAccount(UserViewModel model);

        /// <summary>
        /// Sends a password reset link to the user.
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <returns>An update user result holding the data on the sending of the reset link</returns>
        SaveResult<UserViewModel> SendPasswordResetLink(string email);

        /// <summary>
        /// Sends a password reset link to the user.
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns>An update user result holding the data on the sending of the reset link</returns>
        SaveResult<UserViewModel> SendPasswordResetLink(Guid userId);

        /// <summary>
        /// Validates the reset key for a user.
        /// </summary>
        /// <param name="resetKey">The reset key</param>
        /// <returns>True if the key is valid, false otherwise</returns>
        bool ValidateResetKey(Guid resetKey);

        /// <summary>
        /// Changes the password for a user.
        /// </summary>
        /// <param name="email">The user's e-mail</param>
        /// <param name="oldPassword">The user's old password, or null if a reset key is used</param>
        /// <param name="newPassword">The user's new password</param>
        /// <param name="resetKey">The password reset key, if available</param>
        /// <returns>An update user result holding the data on the password change</returns>
        SaveResult<UserViewModel> ChangePassword(string email, string oldPassword, string newPassword, Guid? resetKey = null);

        /// <summary>
        /// Gets a user using the password reset key supplied.
        /// </summary>
        /// <param name="key">The password reset key</param>
        /// <returns>The user, if a user with a valid key with the specified id was found, or NULL</returns>
        UserViewModel GetUserByResetKey(Guid key);
    }
}