#region Apache License
//-----------------------------------------------------------------------
// <copyright file="MembershipService.cs" company="StrixIT">
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
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipService : IMembershipService
    {
        private static Guid _appId;
        private static Group _mainGroup;

        private GroupInRole _adminRole;
        private User _adminUser;

        private IMembershipDataSource _dataSource;
        private ISecurityManager _securityManager;

        public MembershipService(IMembershipDataSource dataSource, ISecurityManager securityManager)
        {
            this._dataSource = dataSource;
            this._securityManager = securityManager;
        }

        public Guid ApplicationId
        {
            get
            {
                if (_appId == Guid.Empty)
                {
                    _appId = this._dataSource.Query<Application>().Where(g => g.Name.ToLower() == StrixPlatform.Configuration.ApplicationName.ToLower()).Select(a => a.Id).First();
                }

                return _appId;
            }
        }

        public Guid MainGroupId
        {
            get
            {
                if (_mainGroup == null)
                {
                    _mainGroup = this._dataSource.Query<Group>().Where(g => g.Name.ToLower() == Resources.DefaultValues.MainGroupName).First();
                }

                return _mainGroup.Id;
            }
        }

        public Guid AdminId
        {
            get
            {
                if (this._adminUser == null)
                {
                    this._adminUser = this._dataSource.Query<User>().Where(g => g.Email.ToLower() == Resources.DefaultValues.AdministratorEmail).First();
                }

                return this._adminUser.Id;
            }
        }

        public void Initialize()
        {
            StrixPlatform.WriteStartupMessage("Check and create the application.");
            this.InitApplication();

            StrixPlatform.WriteStartupMessage("Check and create the main group.");
            this.InitMainGroup();

            StrixPlatform.WriteStartupMessage("Check and create the admin user.");
            this.InitAdminUser();

            StrixPlatform.WriteStartupMessage("Check and create the application permissions.");
            this.InitPermissions();
        }

        public void InitApplication()
        {
            if (_appId == Guid.Empty)
            {
                var appName = StrixPlatform.Configuration.ApplicationName;
                _appId = this._dataSource.Query<Application>().Where(a => a.Name.ToLower() == appName.ToLower()).Select(a => a.Id).FirstOrDefault();

                if (_appId == Guid.Empty)
                {
                    var app = new Application(Guid.NewGuid(), appName);
                    this._dataSource.Save(app);
                    _appId = app.Id;
                    this._dataSource.SaveChanges();
                }
            }
        }

        public void InitMainGroup()
        {
            if (_mainGroup == null)
            {
                _mainGroup = this._dataSource.Query<Group>("Roles.Role").Where(g => g.Name.ToLower() == Resources.DefaultValues.MainGroupName).FirstOrDefault();

                if (_mainGroup == null)
                {
                    var mainGroup = new Group(Guid.NewGuid(), Resources.DefaultValues.MainGroupName);
                    this._dataSource.Save(mainGroup);
                    var adminRole = new Role(Guid.NewGuid(), mainGroup.Id, PlatformConstants.ADMINROLE);
                    this._dataSource.Save(adminRole);
                    var groupRole = new GroupInRole(mainGroup.Id, adminRole.Id, DateTime.Now, null);
                    groupRole.CurrentNumberOfUsers = 1;
                    this._adminRole = groupRole;
                    mainGroup.Roles = new List<GroupInRole>() { groupRole };
                    _mainGroup = mainGroup;
                    this._dataSource.SaveChanges();
                }
                else
                {
                    this._adminRole = _mainGroup.Roles.First(r => r.Role.Name.ToLower() == PlatformConstants.ADMINROLE.ToLower());
                }
            }
        }

        public void InitAdminUser()
        {
            var saveChanges = false;
            this._adminUser = this._dataSource.Query<User>("Roles.GroupRole.Role").FirstOrDefault(u => u.Email.ToLower() == Resources.DefaultValues.AdministratorEmail.ToLower());

            if (this._adminUser == null)
            {
                this.CreateAdminUser();
                saveChanges = true;
            }
            else if (!this._adminUser.Roles.Any(r => r.GroupRole.Role.Name.ToLower() == PlatformConstants.ADMINROLE.ToLower()))
            {
                var userRole = new UserInRole(this._adminRole, this._adminUser.Id, DateTime.Now, null);
                this._adminUser.Roles.Add(userRole);
                saveChanges = true;
            }

            if (saveChanges)
            {
                this._dataSource.SaveChanges();
            }
        }

        public void InitPermissions()
        {
            var moduleConfigurations = ModuleManager.GetObjectList<IModuleConfiguration>().OrderBy(e => e.Name).ToList();
            var adminPermissionSet = this.CreateAdminPermissionSet();
            this.CreatePermissions(moduleConfigurations, adminPermissionSet);
            this.CreateAndUpdateRoles(moduleConfigurations);
            this.RemoveUnusedPermissions();
        }

        public IQueryable<UserData> UserData()
        {
            return this._dataSource.Query<User>().Select(u => new UserData { Id = u.Id, Name = u.Name, Email = u.Email });
        }

        public IQueryable<GroupData> GroupData()
        {
            return this._dataSource.Query<Group>().Select(g => new GroupData { Id = g.Id, Name = g.Name });
        }

        private void CreateAdminUser()
        {
            var adminId = Guid.NewGuid();
            string password = this._securityManager.EncodePassword(Resources.DefaultValues.AdministratorPassword);
            var admin = new User(adminId, Resources.DefaultValues.AdministratorEmail, Resources.DefaultValues.AdministratorName);
            admin.PreferredCulture = StrixPlatform.DefaultCultureCode;
            this._dataSource.Save(admin);

            var security = new UserSecurity(admin.Id);
            security.Password = password;
            security.Approved = true;
            this._dataSource.Save(security);

            var session = new UserSessionStorage(admin.Id);
            this._dataSource.Save(session);

            var userRole = new UserInRole(this._adminRole, admin.Id, DateTime.Now, null);
            admin.Roles = new List<UserInRole>() { userRole };
        }

        private Role CreateAdminPermissionSet()
        {
            var adminPermissionSet = this._dataSource.Query<Role>("Permissions").FirstOrDefault(r => r.Permissions.Any(p => p.ApplicationId == _appId) && r.Name.ToLower() == Resources.DefaultValues.PermissionSetName.ToLower());

            if (adminPermissionSet == null)
            {
                adminPermissionSet = new Role(Guid.NewGuid(), _mainGroup.Id, Resources.DefaultValues.PermissionSetName);
                adminPermissionSet.Permissions = new List<Permission>();
                var adminGroupPermissionSet = new GroupInRole(_mainGroup.Id, adminPermissionSet.Id);
                adminGroupPermissionSet.StartDate = DateTime.Now;
                this._dataSource.Save(adminPermissionSet);
                this._dataSource.Save(adminGroupPermissionSet);
                adminPermissionSet.Groups = new List<GroupInRole> { adminGroupPermissionSet };
                this._dataSource.SaveChanges();
            }

            return adminPermissionSet;
        }

        private void CreatePermissions(IList<IModuleConfiguration> moduleConfigurations, Role adminPermissionSet)
        {
            var allPermissions = moduleConfigurations.SelectMany(c => c.ModulePermissions.SelectMany(p => p.Value)).Distinct().ToArray();
            var permissionsToCreate = allPermissions.ToLower().Except(this._dataSource.Query<Permission>().Where(p => p.ApplicationId == _appId).Select(p => p.Name.ToLower()));

            // Create all the permissions that do not yet exist.
            var saveChanges = false;

            foreach (var permissionName in allPermissions.Where(p => permissionsToCreate.Contains(p.ToLower())))
            {
                var permission = new Permission(Guid.NewGuid(), _appId, permissionName);
                this._dataSource.Save(permission);
                adminPermissionSet.Permissions.Add(permission);
                saveChanges = true;
            }

            if (saveChanges)
            {
                this._dataSource.SaveChanges();
            }
        }

        private void CreateAndUpdateRoles(IList<IModuleConfiguration> moduleConfigurations)
        {
            var saveChanges = false;
            var existingPermissions = this._dataSource.Query<Permission>().Where(p => p.ApplicationId == _appId).ToList();
            var existingRoles = this._dataSource.Query<Role>("Permissions").Where(r => r.Permissions.Count == 0 || r.Permissions.Any(p => p.ApplicationId == _appId)).ToList();
            var allRoles = moduleConfigurations.SelectMany(c => c.ModulePermissions.Keys).Distinct().ToArray();

            // Add all the permissions to all the roles when not assigned yet, creating the roles when they don't exist.
            foreach (var roleName in allRoles)
            {
                var role = existingRoles.FirstOrDefault(r => r.Name.ToLower() == roleName.ToLower());

                if (role == null)
                {
                    role = new Role(Guid.NewGuid(), _mainGroup.Id, roleName);
                    role.Permissions = new List<Permission>();
                    role.Groups = new List<GroupInRole>();
                    var mainGroupInRole = new GroupInRole(_mainGroup.Id, role.Id);
                    role.Groups.Add(mainGroupInRole);
                    this._dataSource.Save(role);
                    saveChanges = true;
                }

                var permissionsToReview = existingPermissions.Where(p => moduleConfigurations.Where(c => c.ModulePermissions.ContainsKey(roleName)).SelectMany(c => c.ModulePermissions[roleName]).Distinct().ToLower().Contains(p.Name.ToLower())).ToList();
                var permissionsToAdd = permissionsToReview.Where(p => !role.Permissions.Any(rp => rp.Name.ToLower() == p.Name.ToLower())).ToList();
                var permissionsToRemove = role.Permissions.Where(p => !permissionsToReview.Select(pr => pr.Name.ToLower()).Contains(p.Name.ToLower())).ToList();

                foreach (var permission in permissionsToRemove)
                {
                    var thePermission = role.Permissions.First(p => p.Id == permission.Id);
                    role.Permissions.Remove(thePermission);
                    saveChanges = true;
                }

                foreach (var permission in permissionsToAdd)
                {
                    role.Permissions.Add(permission);
                    saveChanges = true;
                }
            }

            if (saveChanges)
            {
                this._dataSource.SaveChanges();
            }
        }

        private void RemoveUnusedPermissions()
        {
            var saveChanges = false;

            foreach (var permission in this._dataSource.Query<Permission>().Where(p => !p.Roles.Any(r => r.Name != Resources.DefaultValues.PermissionSetName)))
            {
                this._dataSource.Delete(permission);
                saveChanges = true;
            }

            if (saveChanges)
            {
                this._dataSource.SaveChanges();
            }
        }
    }
}