//-----------------------------------------------------------------------
// <copyright file="IAuthenticationCookieService.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// An interface to allow injecting the FormsAuthentication dependency.
    /// </summary>
    public interface IAuthenticationCookieService
    {
        /// <summary>
        /// Sets an authentication cookie using FormsAuthentication. Whether to persist the cookie accross browser sessions
        /// is configured using the LimitAuthenticationToBrowserSession app setting (default value is false).
        /// </summary>
        /// <param name="userName">The name of the user to set the cookie for.</param>
        void SetAuthCookie(string userName);

        /// <summary>
        /// Signs a user out.
        /// </summary>
        void SignOut();
    }
}