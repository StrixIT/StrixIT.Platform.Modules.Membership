﻿#region Apache License

//-----------------------------------------------------------------------
// <copyright file="UserManager.cs" company="StrixIT">
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

using Newtonsoft.Json;
using StrixIT.Platform.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace StrixIT.Platform.Modules.Membership
{
    public class UserManager : IUserManager
    {
        #region Private Fields

        private static List<User> _loggedInUsers = new List<User>();
        private IMembershipDataSource _dataSource;
        private IEnvironment _environment;
        private ISecurityManager _securityManager;

        #endregion Private Fields

        #region Public Constructors

        public UserManager(IMembershipDataSource dataSource, ISecurityManager securityManager, IEnvironment environment)
        {
            _dataSource = dataSource;
            _securityManager = securityManager;
            _environment = environment;
        }

        #endregion Public Constructors

        #region LoggedInUsers

        public int GetNumberOfUsersOnline(Guid? groupId = null)
        {
            if (groupId.HasValue)
            {
                return _loggedInUsers.Where(u => u.Roles.Any(r => r.GroupRoleGroupId == groupId.Value)).Count();
            }
            else
            {
                return _loggedInUsers.Count();
            }
        }

        public void RemoveLoggedInUser(Guid id)
        {
            // Get the user's entry from the list if it exists already.
            var userEntry = _loggedInUsers.Where(u => u.Id == id).FirstOrDefault();

            if (userEntry != null)
            {
                _loggedInUsers.Remove(userEntry);
            }
        }

        public void UpdateLoggedInUser(User user)
        {
            // Get the user's entry from the list if it exists already.
            var userEntry = _loggedInUsers.Where(u => u.Email.ToLower() == user.Email.ToLower()).FirstOrDefault();

            if (userEntry == null)
            {
                _loggedInUsers.Add(user);
            }
        }

        #endregion LoggedInUsers

        #region Get

        public User Get(Guid id)
        {
            return Get(id, false);
        }

        public User Get(Guid id, bool getForMainGroup)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            var user = _loggedInUsers.Where(u => u.Id == id).FirstOrDefault();

            if (user == null)
            {
                user = Query().FirstOrDefault(u => u.Id == id);
            }

            if (user == null)
            {
                Logger.Log(string.Format("No user found for id {0}", id), LogLevel.Error);
            }

            return user;
        }

        public User Get(string email)
        {
            return Get(email, false);
        }

        public User Get(string email, bool getForMainGroup)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            email = email.ToLower();

            var user = _loggedInUsers.Where(u => u.Email.ToLower() == email).FirstOrDefault();

            if (user == null)
            {
                user = Query().FirstOrDefault(u => u.Email.ToLower() == email);
            }

            if (user == null)
            {
                Logger.Log(string.Format("No user found for email {0}", email), LogLevel.Error);
            }

            return user;
        }

        public string GetEmail(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("id must be specified");
            }

            return _dataSource.Query<User>().Where(u => u.Id == id).Select(u => u.Email).FirstOrDefault();
        }

        public Guid? GetId(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email must be specified");
            }

            return _dataSource.Query<User>().Where(u => u.Email.ToLower() == email.ToLower()).Select(u => u.Id).FirstOrDefault();
        }

        public string GetName(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("id must be specified");
            }

            return _dataSource.Query<User>().Where(u => u.Id == id).Select(u => u.Name).FirstOrDefault();
        }

        #endregion Get

        #region Query

        public IQueryable<UserProfileValue> ProfileQuery()
        {
            return _dataSource.Query<UserProfileValue>().Include(v => v.CustomField);
        }

        public IQueryable<User> Query()
        {
            var getForMainGroup = _environment.User.IsInMainGroup && _environment.User.IsAdministrator;
            var groupId = _environment.User.GroupId;
            var query = _dataSource.Query<User>("Roles").Where(u => u.Roles.Any(r => r.GroupRoleGroupId == groupId) || (getForMainGroup && !u.Roles.Any()));
            return query;
        }

        #endregion Query

        #region Save

        public User Create(string name, string email, string preferredCulture, string password, bool isApproved, bool acceptedTerms, string registrationComment)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("email");
            }

            CheckEmailAvailable(email);
            User user = _dataSource.Query<User>().FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                CheckPassword(password);
                var encodedPassword = _securityManager.EncodePassword(password);

                user = new User(Guid.NewGuid(), email, name);
                user.PreferredCulture = string.IsNullOrWhiteSpace(preferredCulture) ? _environment.Cultures.CurrentCultureCode : preferredCulture;
                user.DateAcceptedTerms = acceptedTerms ? (DateTime?)DateTime.Now : null;

                var security = new UserSecurity(user.Id);
                security.Password = encodedPassword;
                security.Approved = isApproved || _environment.Configuration.GetConfiguration<MembershipConfiguration>().AutoApproveUsers;
                security.RegistrationComment = registrationComment;

                var session = new UserSessionStorage(user.Id);

                user = _dataSource.Save(user);
                security = _dataSource.Save(security);
                session = _dataSource.Save(session);

                if (security == null || session == null)
                {
                    user = null;
                }
            }

            if (user != null)
            {
                var args = new Dictionary<string, object>();
                args.Add("Id", user.Id);
                args.Add("UserName", user.Name);
                args.Add("UserEmail", user.Email);
                PlatformEvents.Raise<GeneralEvent>(new GeneralEvent("UserCreateEvent", args));
            }

            return user;
        }

        public User Update(Guid id, string name, string email, string preferredCulture)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id must have a value");
            }

            var user = Get(id);

            if (user == null)
            {
                return null;
            }

            CheckEmailAvailable(email, id);
            user.Name = name;
            user.Email = email;
            user.PreferredCulture = preferredCulture;

            var args = new Dictionary<string, object>();
            args.Add("Id", user.Id);
            args.Add("UserName", user.Name);
            args.Add("UserEmail", user.Email);
            PlatformEvents.Raise<GeneralEvent>(new GeneralEvent("UserUpdateEvent", args));

            return user;
        }

        #endregion Save

        #region Delete

        public void Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id must have a value");
            }

            // Query the datasource because deleted users must be included here.
            var user = _dataSource.Query<User>().FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return;
            }

            var groupRoles = _dataSource.Query<UserInRole>().Where(ur => ur.UserId == id).Select(g => g.GroupRole);

            foreach (var role in groupRoles)
            {
                if (role.CurrentNumberOfUsers > 0)
                {
                    role.CurrentNumberOfUsers--;
                }
            }

            var security = _dataSource.Query<UserSecurity>().Where(s => s.Id == user.Id);
            var session = _dataSource.Query<UserSessionStorage>().Where(s => s.Id == user.Id);
            var profileValues = _dataSource.Query<UserProfileValue>().Where(p => p.UserId == user.Id);
            _dataSource.Delete(security);
            _dataSource.Delete(session);
            _dataSource.Delete(profileValues);
            _dataSource.Delete(user);
        }

        #endregion Delete

        #region Session

        public void SaveSession(Guid userId, IDictionary<string, object> sessionValues)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("Parameter cannot be empty", "userId");
            }

            var session = _dataSource.Query<UserSessionStorage>().FirstOrDefault(u => u.Id == userId);

            if (session == null)
            {
                session = new UserSessionStorage(userId);
            }

            var serializableValues = JsonConvert.SerializeObject(sessionValues);
            session.Session = serializableValues;
            _dataSource.Save(session);
        }

        #endregion Session

        #region Private Methods

        private void CheckEmailAvailable(string email, Guid? id = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentNullException("email");
            }

            var available = !Query().Any(u => u.Email.ToLower() == email.ToLower() && (id == null || (id.HasValue && id.Value != u.Id)));

            if (!available)
            {
                var ex = new StrixMembershipException(string.Format("Email {0} is already in use.", email));
                Logger.Log(ex.Message, ex, LogLevel.Fatal);
                throw ex;
            }
        }

        private void CheckPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException("password");
            }

            if (password.Length < _environment.Configuration.GetConfiguration<MembershipConfiguration>().MinRequiredPasswordLength)
            {
                var ex = new StrixMembershipException();
                Logger.Log(ex.Message, ex, LogLevel.Fatal);
                throw ex;
            }

            var specialChars = 0;

            for (int index = 0; index < password.Length; ++index)
            {
                if (!char.IsLetterOrDigit(password[index]))
                {
                    ++specialChars;
                }
            }

            if (specialChars < _environment.Configuration.GetConfiguration<MembershipConfiguration>().MinRequiredNonAlphanumericCharacters)
            {
                var ex = new StrixMembershipException("The password does not have enough non-alphanumeric characters.");
                Logger.Log(ex.Message, ex, LogLevel.Fatal);
                throw ex;
            }
        }

        #endregion Private Methods
    }
}