#region Apache License

//-----------------------------------------------------------------------
// <copyright file="AccountService.cs" company="StrixIT">
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
using System;

namespace StrixIT.Platform.Modules.Membership
{
    public class AccountService : IAccountService
    {
        #region Private Fields

        private IMembershipDataSource _dataSource;
        private IMembershipMailer _mailer;
        private IRoleManager _roleManager;
        private ISecurityManager _securityManager;
        private IUserManager _userManager;

        #endregion Private Fields

        #region Public Constructors

        public AccountService(
            IMembershipDataSource dataSource,
            ISecurityManager securityManager,
            IUserManager userManager,
            IRoleManager roleManager,
            IMembershipMailer mailer)
        {
            this._dataSource = dataSource;
            this._securityManager = securityManager;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._mailer = mailer;
        }

        #endregion Public Constructors

        #region Account

        public SaveResult<UserViewModel> ChangePassword(string email, string oldPassword, string newPassword, Guid? resetKey = null)
        {
            var result = new SaveResult<UserViewModel>();
            var user = this._userManager.Get(email);

            if (user == null)
            {
                Logger.Log("No user found for e-mail " + email, LogLevel.Error);
                return result;
            }

            result.Success = this._securityManager.ChangePassword(user.Id, oldPassword, newPassword, resetKey);

            if (result.Success)
            {
                this.SaveChanges();
                if (!this._mailer.SendPasswordSetMail(user.PreferredCulture, user.Name, user.Email))
                {
                    result.Message = Resources.Interface.ErrorSendingPasswordChangedMail;
                    Logger.Log(string.Format("An error occurred while sending the password changed mail to user {0}", user.Name), LogLevel.Error);
                }
            }

            return result;
        }

        public UserViewModel GetUserByResetKey(Guid key)
        {
            return this._securityManager.GetUserByResetKey(key).Map<UserViewModel>();
        }

        public SaveResult<UserViewModel> RegisterAccount(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var password = this._securityManager.GeneratePassword();
            User user = this._userManager.Create(model.Name, model.Email, StrixPlatform.CurrentCultureCode, password, false, model.AcceptedTerms, model.RegistrationComment);
            var result = new SaveResult<UserViewModel>(user != null, user.Map<UserViewModel>());

            if (user != null)
            {
                var verificationId = Guid.NewGuid();
                this._securityManager.SetVerificationId(user.Id, verificationId);
                this.SaveChanges();

                // Add the user role to allow logging in.
                this._roleManager.AddUserToRole(StrixPlatform.User.GroupId, user.Id, PlatformConstants.USERROLE);
                this.SaveChanges();

                if (StrixMembership.Configuration.Registration.AutoApproveUsers)
                {
                    if (!this._mailer.SendApprovedAccountMail(user.PreferredCulture, user.Name, user.Email, verificationId))
                    {
                        result.Message = Resources.Interface.ErrorSendingApprovedAccountMail;
                        Logger.Log(string.Format("An error occurred while sending the approved account mail to user {0}", user.Name), LogLevel.Error);
                    }
                }
                else
                {
                    if (!this._mailer.SendUnapprovedAccountMail(user.PreferredCulture, user.Name, user.Email, verificationId))
                    {
                        result.Message = Resources.Interface.ErrorSendingUnapprovedAccountMail;
                        Logger.Log(string.Format("An error occurred while sending the unapproved account mail to user {0}", user.Name), LogLevel.Error);
                    }
                }
            }

            return result;
        }

        public SaveResult<UserViewModel> SendPasswordResetLink(string email)
        {
            var userId = this._userManager.GetId(email);

            if (!userId.HasValue)
            {
                throw new StrixMembershipException(string.Format("No user found for email {0}", email));
            }

            return this.SendPasswordResetLink(userId.Value);
        }

        public SaveResult<UserViewModel> SendPasswordResetLink(Guid userId)
        {
            var result = new SaveResult<UserViewModel>();
            var user = this._userManager.Get(userId);

            if (user != null)
            {
                // Set a new verification key.
                var verificationId = Guid.NewGuid();
                this._securityManager.SetVerificationId(user.Id, verificationId);
                this.SaveChanges();
                result.Success = true;

                if (!this._mailer.SendSetPasswordMail(user.PreferredCulture, user.Name, user.Email, verificationId))
                {
                    Logger.Log(string.Format("An error occurred while sending the password reset mail to user {0}", user.Name), LogLevel.Error);
                    result.Message = Resources.Interface.ErrorSendingPasswordResetLink;
                }
            }

            return result;
        }

        public SaveResult<UserViewModel> UpdateAccount(UserViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (model.Id == Guid.Empty)
            {
                throw new ArgumentException("The id is empty.This method can only be used to update existing accounts");
            }

            var validCredentials = this._securityManager.ValidateUser(model.Id, model.Password) == ValidateUserResult.Valid;
            var result = new SaveResult<UserViewModel>();

            if (validCredentials)
            {
                result.Entity = this._userManager.Update(model.Id, model.Name, model.Email, model.PreferredCulture);
                result.Success = result.Entity != null;

                if (result.Success)
                {
                    this.SaveChanges();
                }
            }

            return result;
        }

        public bool ValidateResetKey(Guid resetKey)
        {
            return this._securityManager.CheckVerificationId(resetKey);
        }

        #endregion Account

        #region Private Methods

        private void SaveChanges()
        {
            this._dataSource.SaveChanges();
        }

        #endregion Private Methods
    }
}