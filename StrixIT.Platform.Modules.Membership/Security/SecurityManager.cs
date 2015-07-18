#region Apache License
//-----------------------------------------------------------------------
// <copyright file="SecurityManager.cs" company="StrixIT">
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class SecurityManager : ISecurityManager
    {
        private IMembershipDataSource _dataSource;

        public SecurityManager(IMembershipDataSource dataSource)
        {
            this._dataSource = dataSource;
        }

        #region Validate

        public ValidateUserResult ValidateUser(Guid id, string password)
        {
            if (id == Guid.Empty || string.IsNullOrWhiteSpace(password))
            {
                return ValidateUserResult.Error;
            }

            var security = this.GetSecurity(id);

            if (security == null)
            {
                return ValidateUserResult.Error;
            }

            var result = this.CheckPassword(security, password);

            if (!result)
            {
                return ValidateUserResult.Invalid;
            }

            if (security.LockedOut)
            {
                return ValidateUserResult.LockedOut;
            }

            if (!security.Approved)
            {
                return ValidateUserResult.Unapproved;
            }

            var hasRoles = this._dataSource.Query<User>().Any(u => u.Id == security.Id && u.Roles.Any());

            if (!hasRoles)
            {
                return ValidateUserResult.NoRoles;
            }

            return ValidateUserResult.Valid;
        }

        #endregion

        #region Password

        public string EncodePassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            // Todo: use numberOfIterations.
            return Crypto.HashPassword(password);
        }

        public bool ChangePassword(Guid userId, string oldPassword, string newPassword, Guid? resetKey = null)
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(newPassword))
            {
                return false;
            }

            var security = this.GetSecurity(userId);

            if (security == null)
            {
                return false;
            }

            var userName = this._dataSource.Query<User>().Where(u => u.Id == userId).Select(u => u.Name).First();

            if (oldPassword != null)
            {
                if (this.ValidateUser(userId, oldPassword) != ValidateUserResult.Valid)
                {
                    return false;
                }
            }
            else if (resetKey == null)
            {
                Logger.LogToAudit(AuditLogType.PasswordReset.ToString(), string.Format("User {0} tried to reset his password, but he provided no reset key", userName));
                return false;
            }
            else
            {
                if (!(security.VerificationId == resetKey && security.VerificationWindowStart.HasValue && security.VerificationWindowStart.Value.AddHours(StrixMembership.Configuration.Password.VerificationIdValidWindow) >= DateTime.Now))
                {
                    if (security.VerificationId != resetKey)
                    {
                        Logger.LogToAudit(AuditLogType.PasswordReset.ToString(), string.Format("User {0} tried to reset his password, but he provided an invalid reset key", userName));
                    }
                    else
                    {
                        Logger.LogToAudit(AuditLogType.PasswordReset.ToString(), string.Format("User {0} tried to reset his password, but the verification window expired", userName));
                    }

                    return false;
                }
            }

            if (security.LockedOut)
            {
                this.UnlockUser(userId);
            }

            var encodedPassword = this.EncodePassword(newPassword);
            security.Password = encodedPassword;

            this.SetVerificationId(userId, null);
            Logger.LogToAudit(AuditLogType.PasswordReset.ToString(), string.Format("Changed the password for user {0}.", userName));

            return true;
        }

        public string GeneratePassword()
        {
            return System.Web.Security.Membership.GeneratePassword(StrixMembership.Configuration.Password.MinRequiredPasswordLength, StrixMembership.Configuration.Password.MinRequiredNonAlphanumericCharacters);
        }

        #endregion

        public bool ApproveUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                return false;
            }

            var security = this.GetSecurity(id);

            if (security == null)
            {
                return false;
            }

            security.Approved = true;
            return true;
        }

        public bool UnlockUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                return false;
            }

            var security = this.GetSecurity(id);

            if (security == null)
            {
                return false;
            }

            security.LockedOut = false;
            security.FailedPasswordAttemptCount = 0;
            security.FailedPasswordAttemptWindowStart = null;

            return true;
        }

        public void SetVerificationId(Guid userId, Guid? verificationId)
        {
            if (userId == Guid.Empty)
            {
                return;
            }

            var security = this.GetSecurity(userId);

            if (security == null)
            {
                return;
            }

            security.VerificationId = verificationId;

            if (verificationId == null)
            {
                security.VerificationWindowStart = null;
            }
            else
            {
                security.VerificationWindowStart = DateTime.Now;
            }

            this._dataSource.SaveChanges();
        }

        public IList<AccountStatus> GetAccountStatusData(Guid[] userIds)
        {
            if (userIds.IsEmpty())
            {
                return new List<AccountStatus>();
            }

            return this._dataSource.Query<UserSecurity>().Where(s => userIds.Contains(s.Id)).Select(s => new AccountStatus { Id = s.Id, LockedOut = s.LockedOut, Approved = s.Approved }).ToList();
        }

        public bool CheckVerificationId(Guid verificationId)
        {
            if (verificationId == Guid.Empty)
            {
                return false;
            }

            var validWindow = StrixMembership.Configuration.Password.VerificationIdValidWindow;
            var start = this._dataSource.Query<UserSecurity>().Where(s => s.VerificationId == verificationId && s.VerificationWindowStart.HasValue).Select(s => s.VerificationWindowStart).FirstOrDefault();
            return start.HasValue && start.Value.AddMinutes(validWindow) >= DateTime.Now;
        }

        public User GetUserByResetKey(Guid key)
        {
            if (this.CheckVerificationId(key))
            {
                return this._dataSource.Query<UserSecurity>().Where(s => s.VerificationId == key).Select(s => s.User).FirstOrDefault();
            }

            return null;
        }

        #region Private Methods

        private UserSecurity GetSecurity(Guid userId)
        {
            var security = this._dataSource.Find<UserSecurity>(new object[] { userId });

            if (security == null)
            {
                var message = string.Format("No security record found for user id {0}", userId);
                Logger.Log(message, LogLevel.Error);
            }

            return security;
        }

        private bool CheckPassword(UserSecurity security, string password)
        {
            bool match = Crypto.VerifyHashedPassword(security.Password, password);
            DateTime utcNow = DateTime.UtcNow;

            if (!security.LockedOut && security.Approved)
            {
                if (match)
                {
                    security.LastLoginDate = DateTime.Now;

                    if (security.FailedPasswordAttemptCount > 0)
                    {
                        security.FailedPasswordAttemptCount = 0;
                        security.FailedPasswordAttemptWindowStart = null;
                    }
                }
                else
                {
                    if (security.FailedPasswordAttemptWindowStart.HasValue && utcNow > security.FailedPasswordAttemptWindowStart.Value.AddMinutes(StrixMembership.Configuration.Password.PasswordAttemptWindow))
                    {
                        security.FailedPasswordAttemptCount = 1;
                    }
                    else
                    {
                        ++security.FailedPasswordAttemptCount;
                    }

                    security.FailedPasswordAttemptWindowStart = utcNow;

                    if (security.FailedPasswordAttemptCount >= StrixMembership.Configuration.Password.MaxInvalidPasswordAttempts)
                    {
                        security.LockedOut = true;
                    }
                }
            }

            return match;
        }

        #endregion
    }
}