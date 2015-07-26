#region Apache License

//-----------------------------------------------------------------------
// <copyright file="IAuthenticationCookieService.cs" company="StrixIT">
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
    /// An interface to allow injecting the FormsAuthentication dependency.
    /// </summary>
    public interface IAuthenticationCookieService
    {
        #region Public Methods

        /// <summary>
        /// Sets an authentication cookie using FormsAuthentication. Whether to persist the cookie
        /// accross browser sessions is configured using the LimitAuthenticationToBrowserSession app
        /// setting (default value is false).
        /// </summary>
        /// <param name="userName">The name of the user to set the cookie for.</param>
        void SetAuthCookie(string userName);

        /// <summary>
        /// Signs a user out.
        /// </summary>
        void SignOut();

        #endregion Public Methods
    }
}