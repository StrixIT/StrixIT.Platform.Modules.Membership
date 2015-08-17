#region Apache License

//-----------------------------------------------------------------------
// <copyright file="GroupService.cs" company="StrixIT">
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StrixIT.Platform.Modules.Membership
{
    public class GroupService : IGroupService
    {
        #region Private Fields

        private IMembershipDataSource _dataSource;
        private IEnvironment _environment;
        private IGroupManager _groupManager;
        private IRoleManager _roleManager;
        private IUserContext _user;

        #endregion Private Fields

        #region Public Constructors

        public GroupService(IMembershipDataSource dataSource, IGroupManager groupManager, IRoleManager roleManager, IEnvironment environment)
        {
            if (!environment.Configuration.GetConfiguration<MembershipConfiguration>().UseGroups)
            {
                throw new InvalidOperationException(Resources.Interface.GroupsNotEnabed);
            }

            _dataSource = dataSource;
            _groupManager = groupManager;
            _roleManager = roleManager;
            _user = environment.User;
            _environment = environment;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Delete(Guid id)
        {
            Delete(id, true);
        }

        public void Delete(Guid id, bool saveChanges)
        {
            _groupManager.Delete(id);

            if (saveChanges)
            {
                _dataSource.SaveChanges();
            }
        }

        public bool Exists(string name, Guid? id)
        {
            var result = _groupManager.Exists(name, id);
            return result;
        }

        public GroupViewModel Get(Guid? id)
        {
            GroupViewModel model;

            if (!id.HasValue || id.Value == Guid.Empty)
            {
                model = new GroupViewModel();
                model.CanEdit = _user.HasPermission(MembershipPermissions.EditGroup);
                model.CanDelete = _user.HasPermission(MembershipPermissions.DeleteGroup);
            }
            else
            {
                model = _groupManager.Get(id.Value).Map<GroupViewModel>();
            }

            FillRoleData(model);

            return model;
        }

        public IEnumerable List(FilterOptions filter)
        {
            var groupId = _environment.Membership.MainGroupId;
            return _groupManager.Query().Where(g => g.Id != groupId).Filter(filter).Map<GroupListModel>().ToList();
        }

        public SaveResult<GroupViewModel> Save(GroupViewModel model)
        {
            return Save(model, true);
        }

        public SaveResult<GroupViewModel> Save(GroupViewModel model, bool saveChanges)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            Group group;

            if (Exists(model.Name, model.Id))
            {
                return new SaveResult<GroupViewModel> { Success = false, Message = Resources.Interface.DuplicateName };
            }

            if (model.Id == Guid.Empty)
            {
                group = _groupManager.Create(model.Name, model.UsePermissions);
            }
            else
            {
                CleanupAssignments(model);
                group = _groupManager.Update(model.Id, model.Name, model.UsePermissions);
            }

            if (group == null)
            {
                return new SaveResult<GroupViewModel>();
            }

            model.Id = group.Id;

            if (model.UsePermissions)
            {
                UpdatePermissionSet(model);
            }
            else
            {
                UpdateRoleAssignments(model);
            }

            if (saveChanges)
            {
                _dataSource.SaveChanges();
            }

            model = group.Map<GroupViewModel>();

            return new SaveResult<GroupViewModel>(true, model);
        }

        #endregion Public Methods

        #region Private Methods

        private void CleanupAssignments(GroupViewModel model)
        {
            var group = _groupManager.Get(model.Id);
            var changedUsePermissions = group.UsePermissions != model.UsePermissions;

            // If changing from using permissions to roles or vice versa, do some cleaning up.
            if (changedUsePermissions)
            {
                if (!model.UsePermissions)
                {
                    // Change from using permissions to using roles. Remove all custom roles
                    // (including the permission set) and associated user assignments.
                    var roles = _roleManager.Query().Where(r => r.GroupId == model.Id).ToList();
                    var roleNames = roles.Select(r => r.Name).ToArray();
                    _roleManager.RemoveGroupFromRoles(model.Id, roleNames);

                    foreach (var role in roles)
                    {
                        _roleManager.Delete(role.Id);
                    }
                }
                else
                {
                    // Change from using roles to using permissions. Remove all group and user role assignments.
                    var roles = _groupManager.Query().SelectMany(g => g.Roles.Where(r => r.GroupId == model.Id)).Select(r => r.Role).ToList();
                    var roleNames = roles.Select(r => r.Name).ToArray();
                    _roleManager.RemoveGroupFromRoles(model.Id, roleNames);
                }
            }
        }

        private void FillRoleData(GroupViewModel model)
        {
            var groupId = model.Id == Guid.Empty ? _environment.Membership.MainGroupId : model.Id;
            var mainGroupId = model.Id == Guid.Empty ? groupId : _environment.Membership.MainGroupId;

            model.Roles = _roleManager.Query().Where(r => r.Name.ToLower() != PlatformConstants.ADMINROLE.ToLower() && (r.GroupId == groupId || r.GroupId == mainGroupId)).Select(r => new AssignRoleModel { Id = r.Id, Name = r.Name }).ToList();
            model.Permissions = _roleManager.PermissionQuery().Map<AssignPermissionModel>().ToList();
            var permissionSet = _roleManager.GetPermissionSetForGroup(_user.GroupId).Map<AssignRoleModel>();

            if (permissionSet != null)
            {
                SetPermissionSetSettings(model, permissionSet);
            }

            if (model.Id != Guid.Empty)
            {
                var groupRoles = _roleManager.QueryForGroup(model.Id).Where(g => g.Name.ToLower() != Resources.DefaultValues.PermissionSetName.ToLower()).ToList();

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

            var groupPermissions = _roleManager.Query().Where(r => r.Id == permissionSetRole.Id).SelectMany(r => r.Permissions);

            foreach (var permission in groupPermissions)
            {
                var modelPermission = model.Permissions.FirstOrDefault(p => p.Id == permission.Id);

                if (modelPermission != null)
                {
                    modelPermission.Selected = true;
                }
            }
        }

        private void UpdatePermissionSet(GroupViewModel model)
        {
            var permissionAssignment = _roleManager.GetPermissionSetForGroup(model.Id);
            var permissionIds = model.Permissions.Select(p => p.Id).ToArray();
            var permissions = _roleManager.PermissionQuery().Where(p => permissionIds.Contains(p.Id)).ToList();

            if (permissionAssignment == null)
            {
                var permissionSet = _roleManager.Create(Resources.DefaultValues.PermissionSetName, null, permissions);
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
            foreach (var role in _roleManager.Query().Where(r => r.GroupId == model.Id && r.Name.ToLower() != Resources.DefaultValues.PermissionSetName.ToLower()))
            {
                _roleManager.AddGroupToRole(model.Id, role.Name, permissionAssignment.StartDate, permissionAssignment.EndDate);
            }
        }

        private void UpdateRoleAssignments(GroupViewModel model)
        {
            var entries = _roleManager.QueryForGroup(model.Id).ToList();

            foreach (var role in model.Roles)
            {
                var entry = entries.FirstOrDefault(r => r.Id == role.Id);

                if (entry != null)
                {
                    if (role.Selected)
                    {
                        _roleManager.AddGroupToRole(model.Id, role.Name, role.StartDate, role.EndDate, role.MaxNumberOfUsers);
                    }
                    else
                    {
                        _roleManager.RemoveGroupFromRoles(model.Id, new string[] { entry.Name });
                    }
                }
                else
                {
                    _roleManager.AddGroupToRole(model.Id, role.Name, role.StartDate, role.EndDate, role.MaxNumberOfUsers);
                }
            }
        }

        #endregion Private Methods
    }
}