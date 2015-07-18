#region Apache License
//-----------------------------------------------------------------------
// <copyright file="ISecurityManager.cs" company="StrixIT">
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

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The interface for the security manager, which handles security related operations for users.
    /// </summary>
    public interface ISecurityManager
    {
        /// <summary>
        /// Validates the user using the specified credentials.
        /// </summary>
        /// <param name="id">The user id</param>
        /// <param name="password">The user password</param>
        /// <returns>True if the user credentials are valid, false otherwise</returns>
        ValidateUserResult ValidateUser(Guid id, string password);

        /// <summary>
        /// Changes the password for a user.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="oldPassword">The old password. If null, the password will be reset without validating the old credentials.</param>
        /// <param name="newPassword">The new password</param>
        /// <param name="resetKey">The guid for authenticating password reset</param>
        /// <returns>True when the password was changed successfully, false otherwise</returns>
        bool ChangePassword(Guid userId, string oldPassword, string newPassword, Guid? resetKey = null);

        /// <summary>
        /// Hashes a password.
        /// </summary>
        /// <param name="password">The password to hash</param>
        /// <returns>The hashed password</returns>
        string EncodePassword(string password);

        /// <summary>
        /// Generates a new password using the membership settings for length and required characters.
        /// </summary>
        /// <returns>The new password</returns>
        string GeneratePassword();

        /// <summary>
        /// Approves the specified user.
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>True if the user was approved successfully, false otherwise</returns>
        bool ApproveUser(Guid id);

        /// <summary>
        /// Unlocks the specified user.
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>True if the user was unlocked, false otherwise</returns>
        bool UnlockUser(Guid id);

        /// <summary>
        /// Sets a new verification id for e-mail validation or password resets.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="verificationId">The verification id</param>
        void SetVerificationId(Guid userId, Guid? verificationId);

        /// <summary>
        /// Checks the verification id for a user.
        /// </summary>
        /// <param name="verificationId">The verification id</param>
        /// <returns>True if the id is valid, false otherwise</returns>
        bool CheckVerificationId(Guid verificationId);

        /// <summary>
        /// Gets a projection of user id, locked out status and approved status.
        /// </summary>
        /// <param name="userIds">The ids of the users to get the account status data for</param>
        /// <returns>The account status data</returns>
        IList<AccountStatus> GetAccountStatusData(Guid[] userIds);

        /// <summary>
        /// Gets a user using the password reset key supplied.
        /// </summary>
        /// <param name="key">The password reset key</param>
        /// <returns>The user, if a user with a valid key with the specified id was found, or NULL</returns>
        User GetUserByResetKey(Guid key);
    }
}