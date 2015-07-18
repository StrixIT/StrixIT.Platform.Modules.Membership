//-----------------------------------------------------------------------
// <copyright file="UserService.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class UserService : IUserService
    {
        #region Private Properties

        private IMembershipDataSource _dataSource;
        private ISecurityManager _securityManager;
        private IUserManager _userManager;
        private IGroupManager _groupManager;
        private IRoleManager _roleManager;
        private IMembershipMailer _mailer;

        #endregion

        #region Constructor

        public UserService(IMembershipDataSource dataSource, ISecurityManager securityManager, IUserManager userManager, IGroupManager groupManager, IRoleManager roleManager, IMembershipMailer membershipMailer)
        {
            this._dataSource = dataSource;
            this._securityManager = securityManager;
            this._userManager = userManager;
            this._groupManager = groupManager;
            this._roleManager = roleManager;
            this._mailer = membershipMailer;
        }

        #endregion

        #region Checks

        public bool Exists(string email, Guid? id)
        {
            var groupId = StrixPlatform.User.GroupId;
            var result = this._userManager.Query().Any(u => u.Email.ToLower().Equals(email.ToLower()) && u.Roles.Any(r => r.GroupRoleGroupId == groupId) && (!id.HasValue || u.Id != id));
            return result;
        }

        #endregion

        #region Get

        public UserViewModel Get(Guid? id)
        {
            UserViewModel model;

            if (!id.HasValue)
            {
                model = new UserViewModel();
                model.PreferredCulture = StrixPlatform.DefaultCultureCode;
            }
            else
            {
                var user = this._userManager.Get(id.Value);
                model = user.Map<UserViewModel>();
                this.FillAccountData(model);
            }

            this.FillRoleData(model);
            return model;
        }

        public dynamic GetProfile(Guid id)
        {
            var culture = StrixPlatform.CurrentCultureCode;
            return CustomFields.GetCustomFieldsList<UserProfileField, UserProfileValue>(this._userManager.ProfileQuery().Where(x => x.Culture == culture && x.UserId == id), "UserId").FirstOrDefault();
        }

        public IList<dynamic> GetProfileList()
        {
            var culture = StrixPlatform.CurrentCultureCode;
            return CustomFields.GetCustomFieldsList<UserProfileField, UserProfileValue>(this._userManager.ProfileQuery().Where(x => x.Culture == culture), "UserId");
        }

        public IEnumerable List(FilterOptions filter)
        {
            var query = this._userManager.Query();

            if (filter != null)
            {
                query = this.FilterQuery(filter, query);

                if (filter.Sort.Any())
                {
                    query = this.OrderQuery(filter, query);
                }
                else
                {
                    query = query.OrderBy(q => q.Name);
                }

                query = query.Page(filter).Cast<User>();
            }

            var modelList = query.Map<UserListModel>().ToList();
            var ids = modelList.Select(m => m.Id).ToArray();
            var accountData = this._securityManager.GetAccountStatusData(ids);

            foreach (var data in accountData)
            {
                var model = modelList.First(m => m.Id == data.Id);
                model.LockedOut = data.LockedOut;
                model.Approved = data.Approved;
            }

            return modelList;
        }

        #endregion

        #region Save

        public SaveResult<UserViewModel> Save(UserViewModel model)
        {
            return this.Save(model, true);
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
                var groupId = StrixPlatform.User.GroupId;
                GroupInRole permissionSet = null;

                if (this._roleManager.GroupUsesPermissions(groupId))
                {
                    permissionSet = this._roleManager.GetPermissionSetForGroup(groupId);

                    if (permissionSet.MaxNumberOfUsers.HasValue && permissionSet.MaxNumberOfUsers.Value <= permissionSet.CurrentNumberOfUsers)
                    {
                        return result;
                    }
                }

                var password = this._securityManager.GeneratePassword();
                user = this._userManager.Create(model.Name, model.Email, model.PreferredCulture, password, true, false, null);

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
                oldEmail = this._userManager.GetEmail(model.Id);
                user = this._userManager.Update(model.Id, model.Name, model.Email, model.PreferredCulture);

                if (user == null)
                {
                    return result;
                }

                if (model.Approved)
                {
                    this._securityManager.ApproveUser(model.Id);
                }

                if (!model.LockedOut)
                {
                    this._securityManager.UnlockUser(model.Id);
                }

                var roleNames = this._roleManager.QueryForUser(user.Id).Select(r => r.Name).ToArray();

                if (!roleNames.IsEmpty())
                {
                    this._roleManager.RemoveUsersFromRoles(new Guid[] { user.Id }, roleNames);
                }
            }

            // Now, assign the user to all selected roles after adjusting the start and end date to those of his group, when needed.
            if (!model.Roles.IsEmpty())
            {
                foreach (var role in model.Roles.Where(r => r.Selected))
                {
                    this._roleManager.AddUserToRole(StrixPlatform.User.GroupId, user.Id, role.Name, role.StartDate, role.EndDate);
                }
            }

            if (model.Id == Guid.Empty)
            {
                if (!this._mailer.SendAccountInformationMail(user.PreferredCulture, user.Name, user.Email, user.Id))
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
                    if (!this._mailer.SendEmailChangedMail(user.PreferredCulture, user.Name, user.Email, oldEmail))
                    {
                        result.Message = Resources.Interface.ErrorSendingPasswordChangedMail;
                        Logger.Log(string.Format("An error occurred while sending the email changed mail to user {0}", user.Name), LogLevel.Error);
                    }
                }
            }

            if (saveChanges)
            {
                this._dataSource.SaveChanges();
            }

            result.Entity = user;
            return result;
        }

        #endregion

        #region Delete

        public void Delete(Guid id)
        {
            this.Delete(id, true);
        }

        public void Delete(Guid id, bool saveChanges)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty");
            }

            // Todo: refactor isuserinrole for use here, then remove isuserinrole from role manager.
            if (StrixPlatform.User.IsInRole(PlatformConstants.GROUPADMINROLE) && this._roleManager.IsUserInRole(id, PlatformConstants.ADMINROLE))
            {
                var message = string.Format("Group administrators cannot delete administrators.");
                Logger.LogToAudit(AuditLogType.IllegalOperation.ToString(), message);
                throw new StrixMembershipException(message);
            }

            this._userManager.Delete(id);

            // Check if there are licences remaining before saving the user.
            var groupId = StrixPlatform.User.GroupId;

            if (this._roleManager.GroupUsesPermissions(groupId))
            {
                var permissionSet = this._roleManager.GetPermissionSetForGroup(groupId);
                permissionSet.CurrentNumberOfUsers--;
            }

            if (saveChanges)
            {
                this._dataSource.SaveChanges();
            }
        }

        #endregion

        #region Private Methods

        private void FillAccountData(UserViewModel model)
        {
            var accountData = this._securityManager.GetAccountStatusData(new Guid[] { model.Id }).First();
            model.LockedOut = accountData.LockedOut;
            model.Approved = accountData.Approved;
        }

        private void FillRoleData(UserViewModel model)
        {
            model.Roles = this._roleManager.QueryForGroup(StrixPlatform.User.GroupId).ToList();

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
                var userRoles = this._roleManager.QueryForUser(model.Id).Select(r => new { Id = r.Id, StartDate = r.StartDate, EndDate = r.EndDate }).ToArray();

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

                model.IsCompanyManager = StrixPlatform.User.IsInRole(PlatformConstants.GROUPADMINROLE);
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
                query = query.Join(this._dataSource.Query<UserSecurity>(), u => u.Id, s => s.Id, (u, s) => new { User = u, Security = s })
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
                var tempQuery = query.Join(this._dataSource.Query<UserSecurity>(), u => u.Id, s => s.Id, (u, s) => new { User = u, Security = s });

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

        #endregion
    }
}