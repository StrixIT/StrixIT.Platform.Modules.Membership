//-----------------------------------------------------------------------
// <copyright file="GroupService.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class GroupService : IGroupService
    {
        private IMembershipDataSource _dataSource;
        private IGroupManager _groupManager;
        private IUserManager _userManager;
        private IRoleManager _roleManager;

        public GroupService(IMembershipDataSource dataSource, IGroupManager groupManager, IUserManager userManager, IRoleManager roleManager)
        {
            if (!StrixMembership.Configuration.UseGroups)
            {
                throw new InvalidOperationException(Resources.Interface.GroupsNotEnabed);
            }

            this._dataSource = dataSource;
            this._groupManager = groupManager;
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public bool Exists(string name, Guid? id)
        {
            var result = this._groupManager.Exists(name, id);
            return result;
        }

        public GroupViewModel Get(Guid? id)
        {
            GroupViewModel model;

            if (!id.HasValue || id.Value == Guid.Empty)
            {
                model = new GroupViewModel();
            }
            else
            {
                model = this._groupManager.Get(id.Value).Map<GroupViewModel>();
            }

            this.FillRoleData(model);
            return model;
        }

        public IEnumerable List(FilterOptions filter)
        {
            var groupId = ApplicationHelper.GetMainGroupId(this._dataSource);
            return this._groupManager.Query().Where(g => g.Id != groupId).Filter(filter).Map<GroupListModel>().ToList();
        }

        public SaveResult<GroupViewModel> Save(GroupViewModel model)
        {
            return this.Save(model, true);
        }

        public SaveResult<GroupViewModel> Save(GroupViewModel model, bool saveChanges)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            Group group;

            if (this.Exists(model.Name, model.Id))
            {
                return new SaveResult<GroupViewModel> { Success = false, Message = Resources.Interface.DuplicateName };
            }

            if (model.Id == Guid.Empty)
            {
                group = this._groupManager.Create(model.Name, model.UsePermissions);
            }
            else
            {
                this.CleanupAssignments(model);
                group = this._groupManager.Update(model.Id, model.Name, model.UsePermissions);
            }

            if (group == null)
            {
                return new SaveResult<GroupViewModel>();
            }

            model.Id = group.Id;

            if (model.UsePermissions)
            {
                this.UpdatePermissionSet(model);
            }
            else
            {
                this.UpdateRoleAssignments(model);
            }

            if (saveChanges)
            {
                this._dataSource.SaveChanges();
            }

            return new SaveResult<GroupViewModel>(true, group.Map<GroupViewModel>());
        }

        public void Delete(Guid id)
        {
            this.Delete(id, true);
        }

        public void Delete(Guid id, bool saveChanges)
        {
            this._groupManager.Delete(id);

            if (saveChanges)
            {
                this._dataSource.SaveChanges();
            }
        }

        #region Private Methods

        private void FillRoleData(GroupViewModel model)
        {
            var groupId = model.Id == Guid.Empty ? ApplicationHelper.GetMainGroupId(this._dataSource) : model.Id;
            var mainGroupId = model.Id == Guid.Empty ? groupId : ApplicationHelper.GetMainGroupId(this._dataSource);

            model.Roles = this._roleManager.Query().Where(r => r.Name.ToLower() != PlatformConstants.ADMINROLE.ToLower() && (r.GroupId == groupId || r.GroupId == mainGroupId)).Select(r => new AssignRoleModel { Id = r.Id, Name = r.Name }).ToList();
            model.Permissions = this._roleManager.PermissionQuery().Map<AssignPermissionModel>().ToList();
            var permissionSet = this._roleManager.GetPermissionSetForGroup(StrixPlatform.User.GroupId).Map<AssignRoleModel>();

            if (permissionSet != null)
            {
                this.SetPermissionSetSettings(model, permissionSet);
            }

            if (model.Id != Guid.Empty)
            {
                var groupRoles = this._roleManager.QueryForGroup(model.Id).Where(g => g.Name.ToLower() != Resources.DefaultValues.PermissionSetName.ToLower()).ToList();

                foreach (var role in model.Roles)
                {
                    var groupRole = groupRoles.FirstOrDefault(r => r.Id == role.Id);

                    if (groupRole != null)
                    {
                        role.StartDate = groupRole.StartDate;
                        role.EndDate = groupRole.EndDate;
                        role.MaxNumberOfUsers = groupRole.MaxNumberOfUsers;
                        role.CurrentNumberOfUsers = groupRole.CurrentNumberOfUsers;
                        role.Selected = true;
                    }
                }
            }

            var index = 0;

            foreach (var role in model.Roles)
            {
                role.Index = index;
                index++;
            }
        }

        private void SetPermissionSetSettings(GroupViewModel model, AssignRoleModel permissionSetRole)
        {
            model.PermissionSetStartDate = permissionSetRole.StartDate;
            model.PermissionSetEndDate = permissionSetRole.EndDate;
            model.PermissionSetMaxNumberOfUsers = permissionSetRole.MaxNumberOfUsers;

            var groupPermissions = this._roleManager.Query().Where(r => r.Id == permissionSetRole.Id).SelectMany(r => r.Permissions);

            foreach (var permission in groupPermissions)
            {
                var modelPermission = model.Permissions.FirstOrDefault(p => p.Id == permission.Id);

                if (modelPermission != null)
                {
                    modelPermission.Selected = true;
                }
            }
        }

        private void CleanupAssignments(GroupViewModel model)
        {
            var group = this._groupManager.Get(model.Id);
            var changedUsePermissions = group.UsePermissions != model.UsePermissions;

            // If changing from using permissions to roles or vice versa, do some cleaning up.
            if (changedUsePermissions)
            {
                if (!model.UsePermissions)
                {
                    // Change from using permissions to using roles. Remove all custom roles (including the permission set) and associated user assignments.
                    var roles = this._roleManager.Query().Where(r => r.GroupId == model.Id).ToList();
                    var roleNames = roles.Select(r => r.Name).ToArray();
                    this._roleManager.RemoveGroupFromRoles(model.Id, roleNames);

                    foreach (var role in roles)
                    {
                        this._roleManager.Delete(role.Id);
                    }
                }
                else
                {
                    // Change from using roles to using permissions. Remove all group and user role assignments.
                    var roles = this._groupManager.Query().SelectMany(g => g.Roles.Where(r => r.GroupId == model.Id)).Select(r => r.Role).ToList();
                    var roleNames = roles.Select(r => r.Name).ToArray();
                    this._roleManager.RemoveGroupFromRoles(model.Id, roleNames);
                }
            }
        }

        private void UpdatePermissionSet(GroupViewModel model)
        {
            var permissionAssignment = this._roleManager.GetPermissionSetForGroup(model.Id);
            var permissionIds = model.Permissions.Select(p => p.Id).ToArray();
            var permissions = this._roleManager.PermissionQuery().Where(p => permissionIds.Contains(p.Id)).ToList();

            if (permissionAssignment == null)
            {
                var permissionSet = this._roleManager.Create(Resources.DefaultValues.PermissionSetName, null, permissions);
                permissionAssignment = new GroupInRole(model.Id, permissionSet.Id);
                permissionAssignment.Role = permissionSet;
                permissionSet.Groups = new List<GroupInRole>();
                permissionSet.Groups.Add(permissionAssignment);
            }
            else
            {
                permissionAssignment.Role.Permissions.Clear();
            }

            permissionAssignment.Role.Permissions = permissions;
            permissionAssignment.StartDate = model.PermissionSetStartDate.HasValue && model.PermissionSetStartDate.Value != new DateTime() ? model.PermissionSetStartDate.Value : DateTime.Now;
            permissionAssignment.EndDate = model.PermissionSetEndDate;
            permissionAssignment.MaxNumberOfUsers = model.PermissionSetMaxNumberOfUsers;

            // Update all custom role dates with permission set date
            foreach (var role in this._roleManager.Query().Where(r => r.GroupId == model.Id && r.Name.ToLower() != Resources.DefaultValues.PermissionSetName.ToLower()))
            {
                this._roleManager.AddGroupToRole(model.Id, role.Name, permissionAssignment.StartDate, permissionAssignment.EndDate);
            }
        }

        private void UpdateRoleAssignments(GroupViewModel model)
        {
            var entries = this._roleManager.QueryForGroup(model.Id).ToList();

            foreach (var role in model.Roles)
            {
                var entry = entries.FirstOrDefault(r => r.Id == role.Id);

                if (entry != null)
                {
                    if (role.Selected)
                    {
                        this._roleManager.AddGroupToRole(model.Id, role.Name, role.StartDate, role.EndDate, role.MaxNumberOfUsers);
                    }
                    else
                    {
                        this._roleManager.RemoveGroupFromRoles(model.Id, new string[] { entry.Name });
                    }
                }
                else
                {
                    this._roleManager.AddGroupToRole(model.Id, role.Name, role.StartDate, role.EndDate, role.MaxNumberOfUsers);
                }
            }
        }

        #endregion
    }
}