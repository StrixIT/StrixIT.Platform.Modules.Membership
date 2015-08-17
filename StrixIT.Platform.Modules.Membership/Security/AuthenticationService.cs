#region Apache License

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
using StrixIT.Platform.Core.Environment;
using StrixIT.Platform.Web;
using System;
using System.Collections.Generic;

namespace StrixIT.Platform.Modules.Membership
{
    public class AuthenticationService : IAuthenticationService
    {
        #region Private Fields

        private IAuthenticationCookieService _cookieService;
        private IMembershipData _membershipData;
        private ISessionService _session;

        #endregion Private Fields

        #region Public Constructors

        public AuthenticationService(IMembershipData membershipData, IAuthenticationCookieService cookieService, ISessionService session)
        {
            _membershipData = membershipData;
            _cookieService = cookieService;
            _session = session;
        }

        #endregion Public Constructors

        #region Public Methods

        public void LogOff(string email)
        {
            LogOff(email, null);
        }

        public void LogOff(string email, IDictionary<string, object> sessionValues)
        {
            _cookieService.SignOut();

            if (!string.IsNullOrWhiteSpace(email))
            {
                var user = _membershipData.UserManager.Get(email);
                _membershipData.UserManager.RemoveLoggedInUser(user.Id);
                _membershipData.UserManager.SaveSession(user.Id, sessionValues != null ? sessionValues : _session.GetAll());
                _membershipData.DataSource.SaveChanges();
                Logger.LogToAudit(AuditLogType.LoginLogout.ToString(), string.Format("User {0} logged out.", user.Name));
                _session.Store(PlatformConstants.CURRENTUSEREMAIL, null);
            }

            _session.Abandon();
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
                var id = _membershipData.UserManager.GetId(email);

                if (id == null)
                {
                    return result;
                }

                var name = _membershipData.UserManager.GetName(id.Value);
                var validateResult = _membershipData.SecurityManager.ValidateUser(id.Value, password);
                _membershipData.DataSource.SaveChanges();

                if (validateResult == ValidateUserResult.Valid)
                {
                    _cookieService.SetAuthCookie(email);
                    _session.Store(PlatformConstants.CURRENTUSEREMAIL, email);
                    var user = _membershipData.UserManager.Get(id.Value);
                    _membershipData.UserManager.UpdateLoggedInUser(user);
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