#region Apache License

//-----------------------------------------------------------------------
// <copyright file="UserContext.cs" company="StrixIT">
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
using System.Collections.Generic;
using System.Linq;

namespace StrixIT.Platform.Modules.Membership
{
    public class UserContext : IUserContext
    {
        #region Private Fields

        private IMembershipDataSource _dataSource;
        private IUserManager _userManager;

        #endregion Private Fields

        #region Public Constructors

        public UserContext(IMembershipDataSource dataSource, IUserManager userManager)
        {
            this._dataSource = dataSource;
            this._userManager = userManager;
        }

        #endregion Public Constructors

        #region Public Properties

        public Guid GroupId
        {
            get
            {
                var user = this.User;
                return user != null ? user.GroupId : StrixPlatform.MainGroupId;
            }
            set
            {
                this.User.GroupId = value;
            }
        }

        public string GroupName
        {
            get
            {
                var user = this.User;
                return user != null ? user.GroupName : null;
            }
            set
            {
                this.User.GroupName = value;
            }
        }

        public IDictionary<Guid, string> Groups
        {
            get
            {
                var user = this.User;
                return user != null ? user.Groups.ToDictionary(k => k.Id, v => v.Name) : null;
            }
            set
            {
                this.User.Groups = value.Select(d => new ActiveUserGroup { Id = d.Key, Name = d.Value }).ToList();
            }
        }

        public Guid Id
        {
            get
            {
                var user = this.User;
                return user != null ? user.Id : Guid.Empty;
            }
        }

        // Todo: can this be reworked and removed?
        public bool IsAdministrator
        {
            get
            {
                return this.IsInRole(PlatformConstants.ADMINROLE);
            }
        }

        // Todo: can this be reworked and removed?
        public bool IsInMainGroup
        {
            get
            {
                return StrixPlatform.MainGroupId == this.GroupId;
            }
        }

        public string Name
        {
            get
            {
                var user = this.User;
                return user != null ? user.Name : null;
            }
        }

        #endregion Public Properties

        #region Private Properties

        private ActiveUser User
        {
            get
            {
                var user = StrixPlatform.Environment.GetFromSession<ActiveUser>(PlatformConstants.CURRENTUSER);

                if (user == null)
                {
                    // Get the current user's email.
                    var identityName = StrixPlatform.Environment.CurrentUserEmail;
                    var email = !string.IsNullOrWhiteSpace(identityName) ? identityName : StrixPlatform.Environment.GetFromSession<string>(PlatformConstants.CURRENTUSEREMAIL);

                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        // Get the user's current group id, if already set.
                        var groupId = StrixPlatform.Environment.GetFromSession<Guid?>(PlatformConstants.CURRENTGROUPID);

                        // Determine if the user is an administrator.
                        var isAdmin = this._dataSource.Query<User>().Where(u => u.Email.ToLower() == email.ToLower()).Any(u => u.Roles.Any(r => r.GroupRole.Role.Name.ToLower() == PlatformConstants.ADMINROLE));

                        // Get all the groups the user has access to.
                        var groups = isAdmin ? this._dataSource.Query<Group>().Select(g => new ActiveUserGroup { Id = g.Id, Name = g.Name }).OrderBy(g => g.Name).ToList() :
                                               this._dataSource.Query<User>().Where(u => u.Email.ToLower() == email.ToLower()).SelectMany(u => u.Roles.Select(r => r.GroupRole.Group)).Select(g => new ActiveUserGroup { Id = g.Id, Name = g.Name }).OrderBy(g => g.Name).ToList();

                        if (groups.Count == 0)
                        {
                            groups = new List<ActiveUserGroup> { new ActiveUserGroup { Id = StrixPlatform.MainGroupId, Name = Resources.DefaultValues.MainGroupName } };
                        }

                        string groupName = null;

                        if (!groupId.HasValue)
                        {
                            // If there is no current group id in the session, get the user's
                            // session data from the database and try again.
                            this._userManager.GetSession(email);
                            groupId = StrixPlatform.Environment.GetFromSession<Guid?>(PlatformConstants.CURRENTGROUPID);

                            if (!groupId.HasValue)
                            {
                                // If there is still no current group id, either use the id of the
                                // main group if the user has access to it or the first group in his list.
                                if (groups.Count > 0)
                                {
                                    var mainGroup = groups.FirstOrDefault(g => g.Id == StrixPlatform.MainGroupId);
                                    groupId = mainGroup != null ? mainGroup.Id : groups.First().Id;
                                }

                                // If there is still no group id, use the main group id.
                                if (groupId.HasValue)
                                {
                                    groupName = groups.First(g => g.Id == groupId).Name;
                                }
                                else
                                {
                                    groupId = StrixPlatform.MainGroupId;
                                    groupName = Resources.DefaultValues.MainGroupName;
                                }
                            }

                            StrixPlatform.Environment.StoreInSession(PlatformConstants.CURRENTGROUPID, groupId);
                        }

                        groupName = groupName ?? groups.First(g => g.Id == groupId).Name;

                        if (isAdmin)
                        {
                            user = this._dataSource.Query<User>().Where(u => u.Email.ToLower() == email.ToLower()).Select(u => new ActiveUser
                            {
                                Id = u.Id,
                                Name = u.Name,
                                Email = u.Email,
                                GroupId = groupId.Value,
                                GroupName = groupName,
                                Roles = u.Roles.Where(r => r.GroupRoleGroupId == StrixPlatform.MainGroupId && r.GroupRole.Role.Name.ToLower() == PlatformConstants.ADMINROLE.ToLower()).Select(r => new ActiveUserRole { Name = r.GroupRole.Role.Name, StartDate = r.StartDate, EndDate = r.EndDate }).ToList(),
                                Permissions = u.Roles.Where(r => r.GroupRoleGroupId == StrixPlatform.MainGroupId && r.GroupRole.Role.Name.ToLower() == PlatformConstants.ADMINROLE.ToLower()).SelectMany(r => r.GroupRole.Role.Permissions.Select(p => new ActiveUserPermission { Name = p.Name, StartDate = r.StartDate, EndDate = r.EndDate })).ToList()
                            }).FirstOrDefault();
                        }
                        else
                        {
                            user = this._dataSource.Query<User>().Where(u => u.Email.ToLower() == email.ToLower()).Select(u => new ActiveUser
                            {
                                Id = u.Id,
                                Name = u.Name,
                                Email = u.Email,
                                GroupId = groupId.Value,
                                GroupName = groupName,
                                Roles = u.Roles.Where(r => r.GroupRoleGroupId == groupId).Select(r => new ActiveUserRole { Name = r.GroupRole.Role.Name, StartDate = r.StartDate, EndDate = r.EndDate }).ToList(),
                                Permissions = u.Roles.Where(r => r.GroupRoleGroupId == groupId).SelectMany(r => r.GroupRole.Role.Permissions.Select(p => new ActiveUserPermission { Name = p.Name, StartDate = r.StartDate, EndDate = r.EndDate })).ToList()
                            }).FirstOrDefault();
                        }

                        if (user != null)
                        {
                            user.Groups = groups;
                        }

                        StrixPlatform.Environment.StoreInSession(PlatformConstants.CURRENTUSER, user);
                    }
                }

                return user;
            }
        }

        #endregion Private Properties

        #region IsInRole

        // Todo: rework and remove, use permissions only.
        public bool IsInRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentNullException("roleName");
            }

            if (roleName.Contains(","))
            {
                var roles = roleName.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Trim();
                return this.IsInRoles(roles);
            }

            var user = this.User;

            if (user == null)
            {
                return false;
            }

            var currentDate = DateTime.Now;
            return user.Roles.Any(p => p.Name.ToLower() == roleName.ToLower() && currentDate.IsInRange(p.StartDate, p.EndDate));
        }

        public bool IsInRoles(IEnumerable<string> roleNames)
        {
            if (roleNames == null)
            {
                throw new ArgumentNullException("roleNames");
            }

            var isInRole = false;

            foreach (var roleName in roleNames)
            {
                if (this.IsInRole(roleName))
                {
                    isInRole = true;
                    break;
                }
            }

            return isInRole;
        }

        #endregion IsInRole

        #region HasPermission

        public bool HasPermission(IEnumerable<string> permissions)
        {
            if (permissions == null)
            {
                throw new ArgumentNullException("permissions");
            }

            var hasPermission = false;

            foreach (var permission in permissions)
            {
                if (this.HasPermission(permission))
                {
                    hasPermission = true;
                    break;
                }
            }

            return hasPermission;
        }

        public bool HasPermission(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
            {
                throw new ArgumentNullException("permission");
            }

            if (permission.Contains(","))
            {
                var permissions = permission.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Trim();
                return this.HasPermission(permissions);
            }

            var user = this.User;

            if (user == null)
            {
                return false;
            }

            var currentDate = DateTime.Now;
            return user.Permissions.Any(p => p.Name.ToLower() == permission.ToLower() && currentDate.IsInRange(p.StartDate, p.EndDate));
        }

        #endregion HasPermission
    }
}