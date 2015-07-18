//-----------------------------------------------------------------------
// <copyright file="AuthenticationService.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using StrixIT.Platform.Core;
using StrixIT.Platform.Web;

namespace StrixIT.Platform.Modules.Membership
{
    public class AuthenticationService : IAuthenticationService
    {
        private IMembershipDataSource _dataSource;
        private IUserManager _userManager;
        private ISecurityManager _securityManager;
        private IAuthenticationCookieService _cookieService;

        public AuthenticationService(IMembershipDataSource dataSource, IUserManager userManager, ISecurityManager securityManager, IAuthenticationCookieService cookieService)
        {
            this._dataSource = dataSource;
            this._userManager = userManager;
            this._securityManager = securityManager;
            this._cookieService = cookieService;
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
    }
}