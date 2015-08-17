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

        private IEnvironment _environment;
        private IMembershipMailer _mailer;
        private IMembershipData _membershipData;

        #endregion Private Fields

        #region Public Constructors

        public AccountService(
            IMembershipData membershipData,
            IMembershipMailer mailer,
            IEnvironment environment)
        {
            _membershipData = membershipData;
            _mailer = mailer;
            _environment = environment;
        }

        #endregion Public Constructors

        #region Account

        public SaveResult<UserViewModel> ChangePassword(string email, string oldPassword, string newPassword, Guid? resetKey = null)
        {
            var result = new SaveResult<UserViewModel>();
            var user = _membershipData.UserManager.Get(email);

            if (user == null)
            {
                Logger.Log("No user found for e-mail " + email, LogLevel.Error);
                return result;
            }

            result.Success = _membershipData.SecurityManager.ChangePassword(user.Id, oldPassword, newPassword, resetKey);

            if (result.Success)
            {
                _membershipData.DataSource.SaveChanges();

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
            return _membershipData.SecurityManager.GetUserByResetKey(key).Map<UserViewModel>();
        }

        public SaveResult<UserViewModel> RegisterAccount(RegisterViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var password = _membershipData.SecurityManager.GeneratePassword();
            User user = _membershipData.UserManager.Create(model.Name, model.Email, _environment.Cultures.CurrentCultureCode, password, false, model.AcceptedTerms, model.RegistrationComment);
            var result = new SaveResult<UserViewModel>(user != null, user.Map<UserViewModel>());

            if (user != null)
            {
                var verificationId = Guid.NewGuid();
                _membershipData.SecurityManager.SetVerificationId(user.Id, verificationId);
                _membershipData.DataSource.SaveChanges();

                // Add the user role to allow logging in.
                _membershipData.RoleManager.AddUserToRole(_environment.User.GroupId, user.Id, PlatformConstants.USERROLE);
                _membershipData.DataSource.SaveChanges();

                if (_environment.Configuration.GetConfiguration<MembershipConfiguration>().AutoApproveUsers)
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
            var userId = _membershipData.UserManager.GetId(email);

            if (!userId.HasValue)
            {
                throw new StrixMembershipException(string.Format("No user found for email {0}", email));
            }

            return SendPasswordResetLink(userId.Value);
        }

        public SaveResult<UserViewModel> SendPasswordResetLink(Guid userId)
        {
            var result = new SaveResult<UserViewModel>();
            var user = _membershipData.UserManager.Get(userId);

            if (user != null)
            {
                // Set a new verification key.
                var verificationId = Guid.NewGuid();
                _membershipData.SecurityManager.SetVerificationId(user.Id, verificationId);
                _membershipData.DataSource.SaveChanges();
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

            var validCredentials = _membershipData.SecurityManager.ValidateUser(model.Id, model.Password) == ValidateUserResult.Valid;
            var result = new SaveResult<UserViewModel>();

            if (validCredentials)
            {
                result.Entity = _membershipData.UserManager.Update(model.Id, model.Name, model.Email, model.PreferredCulture);
                result.Success = result.Entity != null;

                if (result.Success)
                {
                    _membershipData.DataSource.SaveChanges();
                }
            }

            return result;
        }

        public bool ValidateResetKey(Guid resetKey)
        {
            return _membershipData.SecurityManager.CheckVerificationId(resetKey);
        }

        #endregion Account
    }
}