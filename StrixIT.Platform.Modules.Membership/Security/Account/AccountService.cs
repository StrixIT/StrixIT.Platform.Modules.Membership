#region Apache License

//-----------------------------------------------------------------------
// <copyright file="AccountService.cs" company="StrixIT">
// Copyright 2015 StrixIT. Author R.G. Schurgers MA MSc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use file except in compliance with the License.
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
        private IUserContext _user;
        private IUserManager _userManager;

        #endregion Private Fields

        #region Public Constructors

        public AccountService(
            IMembershipDataSource dataSource,
            ISecurityManager securityManager,
            IUserManager userManager,
            IRoleManager roleManager,
            IMembershipMailer mailer,
            IUserContext user)
        {
            _dataSource = dataSource;
            _securityManager = securityManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _mailer = mailer;
            _user = user;
        }

        #endregion Public Constructors

        #region Account

        public SaveResult<UserViewModel> ChangePassword(string email, string oldPassword, string newPassword, Guid? resetKey = null)
        {
            var result = new SaveResult<UserViewModel>();
            var user = _userManager.Get(email);

            if (user == null)
            {
                Logger.Log("No user found for e-mail " + email, LogLevel.Error);
                return result;
            }

            result.Success = _securityManager.ChangePassword(user.Id, oldPassword, newPassword, resetKey);

            if (result.Success)
            {
                SaveChanges();
                if (!_mailer.SendPasswordSetMail(user.PreferredCulture, user.Name, user.Email))
                {
                    result.Message = Resources.Interface.ErrorSendingPasswordChangedMail;
                    Logger.Log(string.Format("An error occurred while sending the password changed mail to user {0}", user.Name), LogLevel.Error);
                }
            }

            return result;
        }

        public UserViewModel GetUserByResetKey(Guid key)
        {
            return _securityManager.GetUserByResetKey(key).Map<UserViewModel>();
        }

        public SaveResult<UserViewModel> RegisterAccount(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var password = _securityManager.GeneratePassword();
            User user = _userManager.Create(model.Name, model.Email, StrixPlatform.CurrentCultureCode, password, false, model.AcceptedTerms, model.RegistrationComment);
            var result = new SaveResult<UserViewModel>(user != null, user.Map<UserViewModel>());

            if (user != null)
            {
                var verificationId = Guid.NewGuid();
                _securityManager.SetVerificationId(user.Id, verificationId);
                SaveChanges();

                // Add the user role to allow logging in.
                _roleManager.AddUserToRole(_user.GroupId, user.Id, PlatformConstants.USERROLE);
                SaveChanges();

                if (StrixMembership.Configuration.Registration.AutoApproveUsers)
                {
                    if (!_mailer.SendApprovedAccountMail(user.PreferredCulture, user.Name, user.Email, verificationId))
                    {
                        result.Message = Resources.Interface.ErrorSendingApprovedAccountMail;
                        Logger.Log(string.Format("An error occurred while sending the approved account mail to user {0}", user.Name), LogLevel.Error);
                    }
                }
                else
                {
                    if (!_mailer.SendUnapprovedAccountMail(user.PreferredCulture, user.Name, user.Email, verificationId))
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
            var userId = _userManager.GetId(email);

            if (!userId.HasValue)
            {
                throw new StrixMembershipException(string.Format("No user found for email {0}", email));
            }

            return SendPasswordResetLink(userId.Value);
        }

        public SaveResult<UserViewModel> SendPasswordResetLink(Guid userId)
        {
            var result = new SaveResult<UserViewModel>();
            var user = _userManager.Get(userId);

            if (user != null)
            {
                // Set a new verification key.
                var verificationId = Guid.NewGuid();
                _securityManager.SetVerificationId(user.Id, verificationId);
                SaveChanges();
                result.Success = true;

                if (!_mailer.SendSetPasswordMail(user.PreferredCulture, user.Name, user.Email, verificationId))
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
                throw new ArgumentException("The id is empty.method can only be used to update existing accounts");
            }

            var validCredentials = _securityManager.ValidateUser(model.Id, model.Password) == ValidateUserResult.Valid;
            var result = new SaveResult<UserViewModel>();

            if (validCredentials)
            {
                result.Entity = _userManager.Update(model.Id, model.Name, model.Email, model.PreferredCulture);
                result.Success = result.Entity != null;

                if (result.Success)
                {
                    SaveChanges();
                }
            }

            return result;
        }

        public bool ValidateResetKey(Guid resetKey)
        {
            return _securityManager.CheckVerificationId(resetKey);
        }

        #endregion Account

        #region Private Methods

        private void SaveChanges()
        {
            _dataSource.SaveChanges();
        }

        #endregion Private Methods
    }
}