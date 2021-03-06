﻿#region Apache License

//-----------------------------------------------------------------------
// <copyright file="AuthenticationService.cs" company="StrixIT">
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
using StrixIT.Platform.Web;
using System;
using System.Collections.Generic;

namespace StrixIT.Platform.Modules.Membership
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Private Fields

        private IAuthenticationCookieService _cookieService;
        private IMembershipDataSource _dataSource;
        private ISecurityManager _securityManager;
        private IUserManager _userManager;

        #endregion Private Fields

        #region Public Constructors

        public AuthenticationService(IMembershipDataSource dataSource, IUserManager userManager, ISecurityManager securityManager, IAuthenticationCookieService cookieService)
        {
            this._dataSource = dataSource;
            this._userManager = userManager;
            this._securityManager = securityManager;
            this._cookieService = cookieService;
        }

        #endregion Public Constructors

        #region Public Methods

        public void LogOff(string email)
        {
            this.LogOff(email, null);
        }

        public void LogOff(string email, IDictionary<string, object> sessionValues)
        {
            this._cookieService.SignOut();

            if (!string.IsNullOrWhiteSpace(email))
            {
                var user = this._userManager.Get(email);
                this._userManager.RemoveLoggedInUser(user.Id);
                this._userManager.SaveSession(user.Id, sessionValues != null ? sessionValues : StrixPlatform.Environment.GetSessionDictionary());
                this._dataSource.SaveChanges();
                Logger.LogToAudit(AuditLogType.LoginLogout.ToString(), string.Format("User {0} logged out.", user.Name));
                StrixPlatform.Environment.StoreInSession(PlatformConstants.CURRENTUSEREMAIL, null);
            }

            StrixPlatform.Environment.AbandonSession();
        }

        public LoginUserResult LogOn(string email, string password)
        {
            var result = new LoginUserResult();
            result.Message = Resources.Interface.InvalidCredentials;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return result;
            }

            try
            {
                var id = this._userManager.GetId(email);

                if (id == null)
                {
                    return result;
                }

                var name = this._userManager.GetName(id.Value);
                var validateResult = this._securityManager.ValidateUser(id.Value, password);
                this._dataSource.SaveChanges();

                if (validateResult == ValidateUserResult.Valid)
                {
                    this._cookieService.SetAuthCookie(email);
                    StrixPlatform.Environment.StoreInSession(PlatformConstants.CURRENTUSEREMAIL, email);
                    var user = this._userManager.Get(id.Value);
                    this._userManager.UpdateLoggedInUser(user);
                    result.Success = true;
                    result.PreferredCulture = user.PreferredCulture;
                    Logger.LogToAudit(AuditLogType.LoginLogout.ToString(), string.Format("User {0} logged in.", name));
                }
                else
                {
                    switch (validateResult)
                    {
                        case ValidateUserResult.LockedOut:
                            {
                                result.Message = Resources.Interface.LockedOut;
                            }

                            break;

                        case ValidateUserResult.Unapproved:
                            {
                                result.Message = Resources.Interface.Unapproved;
                            }

                            break;

                        case ValidateUserResult.NoRoles:
                            {
                                result.Message = Resources.Interface.UserHasNoRoles;
                            }

                            break;
                    }

                    Logger.LogToAudit(AuditLogType.LoginLogout.ToString(), string.Format("Login failed for user {0}. Status: {1}.", name, result.Message));
                }
            }
            catch (Exception)
            {
                result.Message = Resources.Interface.ErrorValidatingAccount;
            }

            return result;
        }

        #endregion Public Methods
    }
}