//-----------------------------------------------------------------------
// <copyright file="IMembershipMailer.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The membership mailer interface.
    /// </summary>
    public interface IMembershipMailer
    {
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
        /// Sends a mail to a user when he registers himself and autoapproving accounts is inactive.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="email">The user's email address</param>
        /// <param name="passwordVerificationId">The set password verification id</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendUnapprovedAccountMail(string culture, string userName, string email, Guid passwordVerificationId);

        /// <summary>
        /// Sends a mail to a user to inform him his account has been approved.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="email">The user's email address</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendAccountApprovedMail(string culture, string userName, string email);

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
        /// Sends a password set mail to a user.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="email">The user's email address</param>
        /// <param name="passwordVerificationId">The set password verification id</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendSetPasswordMail(string culture, string userName, string email, Guid passwordVerificationId);

        /// <summary>
        /// Sends a password changed mail to a user.
        /// </summary>
        /// <param name="culture">The culture to use</param>
        /// <param name="userName">The full name of the user to send the mail to</param>
        /// <param name="email">The user's email address</param>
        /// <returns>True if the mail was send successfully, false otherwise</returns>
        bool SendPasswordSetMail(string culture, string userName, string email);
    }
}
