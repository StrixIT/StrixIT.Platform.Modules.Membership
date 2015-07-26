#region Apache License

//-----------------------------------------------------------------------
// <copyright file="IMembershipMailer.cs" company="StrixIT">
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

using System;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The membership mailer interface.
    /// </summary>
    public interface IMembershipMailer
    {
        #region Public Methods

        /// <summary>
        /// Sends a mail to a user to inform him his account has been approved.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="email">The user's email address</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendAccountApprovedMail(string culture, string userName, string email);

        /// <summary>
        /// Sends an account information mail to a user.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="email">The user's email address</param>
        /// <param name="userId">The user's id</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendAccountInformationMail(string culture, string userName, string email, Guid userId);

        /// <summary>
        /// Sends a mail to a user when he registers himself and autoapproving accounts is active.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="email">The user's email address</param>
        /// <param name="passwordVerificationId">The set password verification id</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendApprovedAccountMail(string culture, string userName, string email, Guid passwordVerificationId);

        /// <summary>
        /// Sends an email changed mail to a user.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="newEmail">The user's new email address</param>
        /// <param name="oldEmail">The user's old email address</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendEmailChangedMail(string culture, string userName, string newEmail, string oldEmail);

        /// <summary>
        /// Sends a password changed mail to a user.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="email">The user's email address</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendPasswordSetMail(string culture, string userName, string email);

        /// <summary>
        /// Sends a password set mail to a user.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="email">The user's email address</param>
        /// <param name="passwordVerificationId">The set password verification id</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendSetPasswordMail(string culture, string userName, string email, Guid passwordVerificationId);

        /// <summary>
        /// Sends a mail to a user when he registers himself and autoapproving accounts is inactive.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="email">The user's email address</param>
        /// <param name="passwordVerificationId">The set password verification id</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendUnapprovedAccountMail(string culture, string userName, string email, Guid passwordVerificationId);

        #endregion Public Methods
    }
}