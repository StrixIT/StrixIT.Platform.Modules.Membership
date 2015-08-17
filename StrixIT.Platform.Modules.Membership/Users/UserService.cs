﻿#region Apache License

//-----------------------------------------------------------------------
// <copyright file="UserService.cs" company="StrixIT">
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
using StrixIT.Platform.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace StrixIT.Platform.Modules.Membership
{
    public class UserService : IUserService
    {
        #region Private Properties

        private IEnvironment _environment;
        private IMembershipMailer _mailer;
        private IMembershipData _membershipData;

        #endregion Private Properties

        #region Constructor

        public UserService(
            IMembershipData membershipData,
            IMembershipMailer membershipMailer,
            IEnvironment environment)
        {
            _membershipData = membershipData;
            _mailer = membershipMailer;
            _environment = environment;
        }

        #endregion Constructor

        #region Checks

        public bool Exists(string email, Guid? id)
        {
            var groupId = _environment.User.GroupId;
            var result = _membershipData.UserManager.Query().Any(u => u.Email.ToLower().Equals(email.ToLower()) && u.Roles.Any(r => r.GroupRoleGroupId == groupId) && (!id.HasValue || u.Id != id));
            return result;
        }

        #endregion Checks

        #region Get

        public UserViewModel Get(Guid? id)
        {
            UserViewModel model;

            if (!id.HasValue)
            {
                model = new UserViewModel();
                model.PreferredCulture = _environment.Cultures.DefaultCultureCode;
                model.CanEdit = _environment.User.HasPermission(MembershipPermissions.EditUser);
                model.CanDelete = _environment.User.HasPermission(MembershipPermissions.DeleteUser);
            }
            else
            {
                var user = _membershipData.UserManager.Get(id.Value);
                model = user.Map<UserViewModel>();
                FillAccountData(model);
            }

            FillRoleData(model);
            return model;
        }

        public dynamic GetProfile(Guid id)
        {
            var culture = _environment.Cultures.CurrentCultureCode;
            return CustomFields.GetCustomFieldsList<UserProfileField, UserProfileValue>(_membershipData.UserManager.ProfileQuery().Where(x => x.Culture == culture && x.UserId == id), "UserId").FirstOrDefault();
        }

        public IList<dynamic> GetProfileList()
        {
            var culture = _environment.Cultures.CurrentCultureCode;
            return CustomFields.GetCustomFieldsList<UserProfileField, UserProfileValue>(_membershipData.UserManager.ProfileQuery().Where(x => x.Culture == culture), "UserId");
        }

        public IEnumerable List(FilterOptions filter)
        {
            var query = _membershipData.UserManager.Query();

            if (filter != null)
            {
                query = FilterQuery(filter, query);

                if (filter.Sort.Any())
                {
                    query = OrderQuery(filter, query);
                }
                else
                {
                    query = query.OrderBy(q => q.Name);
                }

                query = query.Page(filter).Cast<User>();
            }

            var modelList = query.Map<UserListModel>().ToList();
            var ids = modelList.Select(m => m.Id).ToArray();
            var accountData = _membershipData.SecurityManager.GetAccountStatusData(ids);

            foreach (var data in accountData)
            {
                var model = modelList.First(m => m.Id == data.Id);
                model.LockedOut = data.LockedOut;
                model.Approved = data.Approved;
            }

            return modelList;
        }

        #endregion Get

        #region Save

        public SaveResult<UserViewModel> Save(UserViewModel model)
        {
            return Save(model, true);
        }

        public SaveResult<UserViewModel> Save(UserViewModel model, bool saveChanges)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            User user;
            string oldEmail = null;
            var result = new SaveResult<UserViewModel>();

            // Save or update the user first, and send the proper mail.
            if (model.Id == Guid.Empty)
            {
                // Check if there are licences remaining before saving the user.
                var groupId = _environment.User.GroupId;
                GroupInRole permissionSet = null;

                if (_membershipData.RoleManager.GroupUsesPermissions(groupId))
                {
                    permissionSet = _membershipData.RoleManager.GetPermissionSetForGroup(groupId);

                    if (permissionSet.MaxNumberOfUsers.HasValue && permissionSet.MaxNumberOfUsers.Value <= permissionSet.CurrentNumberOfUsers)
                    {
                        return result;
                    }
                }

                var password = _membershipData.SecurityManager.GeneratePassword();
                user = _membershipData.UserManager.Create(model.Name, model.Email, model.PreferredCulture, password, true, false, null);

                if (user == null)
                {
                    return result;
                }

                if (permissionSet != null)
                {
                    permissionSet.CurrentNumberOfUsers++;
                }

                result.Success = true;
            }
            else
            {
                oldEmail = _membershipData.UserManager.GetEmail(model.Id);
                user = _membershipData.UserManager.Update(model.Id, model.Name, model.Email, model.PreferredCulture);

                if (user == null)
                {
                    return result;
                }

                if (model.Approved)
                {
                    _membershipData.SecurityManager.ApproveUser(model.Id);
                }

                if (!model.LockedOut)
                {
                    _membershipData.SecurityManager.UnlockUser(model.Id);
                }

                var roleNames = _membershipData.RoleManager.QueryForUser(user.Id).Select(r => r.Name).ToArray();

                if (!roleNames.IsEmpty())
                {
                    _membershipData.RoleManager.RemoveUsersFromRoles(new Guid[] { user.Id }, roleNames);
                }
            }

            // Now, assign the user to all selected roles after adjusting the start and end date to
            // those of his group, when needed.
            if (!model.Roles.IsEmpty())
            {
                foreach (var role in model.Roles.Where(r => r.Selected))
                {
                    _membershipData.RoleManager.AddUserToRole(_environment.User.GroupId, user.Id, role.Name, role.StartDate, role.EndDate);
                }
            }

            if (model.Id == Guid.Empty)
            {
                if (!_mailer.SendAccountInformationMail(user.PreferredCulture, user.Name, user.Email, user.Id))
                {
                    result.Message = Resources.Interface.ErrorSendingVerificationMail;
                    Logger.Log(string.Format("An error occurred while sending the account information mail to user {0}", user.Name), LogLevel.Error);
                }
            }
            else
            {
                result.Success = true;

                if (user.Email.ToLower() != oldEmail.ToLower())
                {
                    if (!_mailer.SendEmailChangedMail(user.PreferredCulture, user.Name, user.Email, oldEmail))
                    {
                        result.Message = Resources.Interface.ErrorSendingPasswordChangedMail;
                        Logger.Log(string.Format("An error occurred while sending the email changed mail to user {0}", user.Name), LogLevel.Error);
                    }
                }
            }

            if (saveChanges)
            {
                _membershipData.DataSource.SaveChanges();
            }

            result.Entity = user;
            return result;
        }

        #endregion Save

        #region Delete

        public void Delete(Guid id)
        {
            Delete(id, true);
        }

        public void Delete(Guid id, bool saveChanges)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty");
            }

            // Todo: refactor isuserinrole for use here, then remove isuserinrole from role manager.
            if (_environment.User.IsInRole(PlatformConstants.GROUPADMINROLE) && _membershipData.RoleManager.IsUserInRole(id, PlatformConstants.ADMINROLE))
            {
                var message = string.Format("Group administrators cannot delete administrators.");
                Logger.LogToAudit(AuditLogType.IllegalOperation.ToString(), message);
                throw new StrixMembershipException(message);
            }

            _membershipData.UserManager.Delete(id);

            // Check if there are licences remaining before saving the user.
            var groupId = _environment.User.GroupId;

            if (_membershipData.RoleManager.GroupUsesPermissions(groupId))
            {
                var permissionSet = _membershipData.RoleManager.GetPermissionSetForGroup(groupId);
                permissionSet.CurrentNumberOfUsers--;
            }

            if (saveChanges)
            {
                _membershipData.DataSource.SaveChanges();
            }
        }

        #endregion Delete

        #region Private Methods

        private void FillAccountData(UserViewModel model)
        {
            var accountData = _membershipData.SecurityManager.GetAccountStatusData(new Guid[] { model.Id }).First();
            model.LockedOut = accountData.LockedOut;
            model.Approved = accountData.Approved;
        }

        private void FillRoleData(UserViewModel model)
        {
            model.Roles = _membershipData.RoleManager.QueryForGroup(_environment.User.GroupId).ToList();

            var index = 0;

            foreach (var role in model.Roles)
            {
                role.GroupStartDate = role.StartDate;
                role.GroupEndDate = role.EndDate;
                role.Index = index;
                index++;
            }

            if (model.Id != Guid.Empty)
            {
                var userRoles = _membershipData.RoleManager.QueryForUser(model.Id).Select(r => new { Id = r.Id, StartDate = r.StartDate, EndDate = r.EndDate }).ToArray();

                foreach (var role in model.Roles)
                {
                    var userRole = userRoles.FirstOrDefault(r => r.Id == role.Id);

                    if (userRole != null)
                    {
                        role.Selected = true;
                        role.StartDate = userRole.StartDate;
                        role.EndDate = userRole.EndDate;
                    }
                }

                model.IsCompanyManager = _environment.User.IsInRole(PlatformConstants.GROUPADMINROLE);
            }
        }

        private IQueryable<User> FilterQuery(FilterOptions filter, IQueryable<User> query)
        {
            var nameField = filter.ExtractField("Name");

            if (nameField != null && !string.IsNullOrWhiteSpace(nameField.Value))
            {
                query = query.Where(q => q.Name.ToLower().Contains(nameField.Value.ToLower()));
            }

            var emailField = filter.ExtractField("Email");

            if (emailField != null && !string.IsNullOrWhiteSpace(emailField.Value))
            {
                query = query.Where(u => u.Email.ToLower().Contains(emailField.Value.ToLower()));
            }

            bool? approved = null;
            var accountApprovedField = filter.ExtractField("Approved");

            if (accountApprovedField != null && !string.IsNullOrWhiteSpace(accountApprovedField.Value))
            {
                approved = bool.Parse(accountApprovedField.Value);
            }

            bool? lockedOut = null;
            var lockedOutField = filter.ExtractField("LockedOut");

            if (lockedOutField != null && !string.IsNullOrWhiteSpace(lockedOutField.Value))
            {
                lockedOut = bool.Parse(lockedOutField.Value);
            }

            if (approved.HasValue || lockedOut.HasValue)
            {
                query = query.Join(_membershipData.DataSource.Query<UserSecurity>(), u => u.Id, s => s.Id, (u, s) => new { User = u, Security = s })
                    .Where(u => (approved == null || u.Security.Approved == approved.Value) && (lockedOut == null || u.Security.LockedOut == lockedOut.Value)).Select(u => u.User);
            }

            return query;
        }

        private IQueryable<User> OrderQuery(FilterOptions filter, IQueryable<User> query)
        {
            var nameSort = filter.Sort.FirstOrDefault(s => s.Field.ToLower() == "name");

            if (nameSort != null)
            {
                query = query.OrderBy("Name " + nameSort.Dir);
            }

            var emailSort = filter.Sort.FirstOrDefault(s => s.Field.ToLower() == "email");

            if (emailSort != null)
            {
                query = query.OrderBy("Email " + emailSort.Dir);
            }

            var approvedSort = filter.Sort.FirstOrDefault(s => s.Field.ToLower() == "approved");
            var lockedOutSort = filter.Sort.FirstOrDefault(s => s.Field.ToLower() == "lockedout");

            if (approvedSort != null || lockedOutSort != null)
            {
                var tempQuery = query.Join(_membershipData.DataSource.Query<UserSecurity>(), u => u.Id, s => s.Id, (u, s) => new { User = u, Security = s });

                if (approvedSort != null)
                {
                    tempQuery = tempQuery.OrderBy("Security.Approved " + approvedSort.Dir);
                }

                if (lockedOutSort != null)
                {
                    tempQuery = tempQuery.OrderBy("Security.LockedOut " + lockedOutSort.Dir);
                }

                query = tempQuery.Select(u => u.User);
            }

            return query;
        }

        #endregion Private Methods
    }
}