#region Apache License

//-----------------------------------------------------------------------
// <copyright file="RoleManager.cs" company="StrixIT">
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace StrixIT.Platform.Modules.Membership
{
    public class RoleManager : IRoleManager
    {
        #region Private Fields

        private static Guid _appId;
        private static Dictionary<Guid, bool> _groupsUsePermissions = new Dictionary<Guid, bool>();
        private static object _lockObject = new object();

        private static Dictionary<Guid, Dictionary<Guid, string>> _roleIdsAndNames;
        private static ConcurrentDictionary<Guid, List<Tuple<string, DateTime, DateTime?>>> _userRoles = new ConcurrentDictionary<Guid, List<Tuple<string, DateTime, DateTime?>>>();

        private IMembershipDataSource _dataSource;
        private IUserContext _user;

        #endregion Private Fields

        #region Public Constructors

        public RoleManager(IMembershipDataSource dataSource, IUserContext user)
        {
            _dataSource = dataSource;
            _user = user;

            // Get the application id and load the roles dictionaries on first load.
            if (_appId == Guid.Empty)
            {
                _appId = ApplicationHelper.GetApplicationId(dataSource);
                GetRoleIdsAndNames();
                GetUserRoles();
            }
        }

        #endregion Public Constructors

        #region Create Roles

        public Role Create(string name, string description, IList<Permission> permissions)
        {
            if (Exists(name, null))
            {
                return _dataSource.Query<Role>().First(r => r.Name.ToLower() == name.ToLower());
            }

            var currentGroupId = _user.GroupId;
            var role = new Role(Guid.NewGuid(), currentGroupId, name);
            role.Description = description;
            role.Permissions = permissions;
            _roleIdsAndNames.Clear();
            return _dataSource.Save(role);
        }

        #endregion Create Roles

        #region Update

        public Role Update(Guid id, string name, string description, IList<Permission> permissions)
        {
            var role = Query("Permissions").First(r => r.Id == id);
            role.Name = name;
            role.Description = description;
            role.Permissions = permissions;
            _roleIdsAndNames.Clear();
            return _dataSource.Save(role);
        }

        #endregion Update

        #region Exists

        public bool Exists(string name, Guid? id)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (name.ToLower() == PlatformConstants.ADMINROLE.ToLower() || name.ToLower() == Resources.DefaultValues.PermissionSetName)
            {
                return true;
            }

            return _dataSource.Query<Role>().Any(r => r.Permissions.Any(p => p.ApplicationId == _appId)
                && r.GroupId == _user.GroupId
                && r.Name.ToLower() == name.ToLower()
                && (!id.HasValue || r.Id != id.Value));
        }

        #endregion Exists

        #region IsInRole

        public bool IsUserInRole(Guid id, string roleName)
        {
            return IsUserInRoles(id, new string[] { roleName });
        }

        public bool IsUserInRoles(Guid id, string[] roleNames)
        {
            if (id == Guid.Empty || roleNames.IsEmpty())
            {
                throw new ArgumentException("Please specify an id and/or one or more role names.");
            }

            roleNames = roleNames.ToLower().ToArray();
            GetUserRoles();
            return _userRoles[id].Any(r => roleNames.Contains(r.Item1) && r.Item2 <= DateTime.Now && (!r.Item3.HasValue || r.Item3.Value > DateTime.Now));
        }

        #endregion IsInRole

        #region Get

        public Role Get(Guid id)
        {
            return Query().FirstOrDefault(r => r.Id == id);
        }

        #endregion Get

        #region Delete

        public void Delete(Guid id)
        {
            var role = Get(id);

            if (role != null && role.Name.ToLower() != PlatformConstants.ADMINROLE.ToLower())
            {
                _dataSource.Delete(role);
            }
        }

        #endregion Delete

        #region Query

        public IQueryable<Role> Query()
        {
            return Query(null);
        }

        public IQueryable<AssignRoleModel> QueryForGroup(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentException("GroupId must have a value", "groupId");
            }

            return _dataSource.Query<Group>().Where(g => g.Id == groupId).SelectMany(g => g.Roles.Where(r => r.Role.Permissions.Any(p => p.ApplicationId == _appId) && r.Role.Name.ToLower() != Resources.DefaultValues.PermissionSetName.ToLower())).Select(r => new AssignRoleModel { Id = r.RoleId, Name = r.Role.Name, StartDate = r.StartDate, EndDate = r.EndDate, MaxNumberOfUsers = r.MaxNumberOfUsers, CurrentNumberOfUsers = r.CurrentNumberOfUsers });
        }

        public IQueryable<AssignRoleModel> QueryForUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("userId cannot be empty", "userId");
            }

            return _dataSource.Query<User>().Where(u => userId == u.Id).SelectMany(u => u.Roles.Where(r => r.GroupRole.Role.Permissions.Any(p => p.ApplicationId == _appId)).Select(r => new AssignRoleModel { Id = r.GroupRole.RoleId, Name = r.GroupRole.Role.Name, StartDate = r.StartDate, EndDate = r.EndDate, MaxNumberOfUsers = r.GroupRole.MaxNumberOfUsers, CurrentNumberOfUsers = r.GroupRole.CurrentNumberOfUsers }));
        }

        #endregion Query

        #region AddToRole

        public void AddGroupToRole(Guid groupId, string roleName, DateTime? startDate = null, DateTime? endDate = null, int? maxNumberOfUsers = null)
        {
            if (groupId == Guid.Empty || string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Please specify a group id and/or a role name.");
            }

            if (!_user.IsInMainGroup && roleName.ToLower() == PlatformConstants.ADMINROLE.ToLower())
            {
                var message = string.Format("Trying to add group {0} to role {1}, which is a reserved role.", groupId, roleName);
                Logger.LogToAudit(AuditLogType.IllegalOperation.ToString(), message);
                throw new StrixMembershipException(message);
            }

            var roleId = GetRoleId(roleName, "group", "add");
            var entry = _dataSource.Find<GroupInRole>(new object[] { groupId, roleId });
            var correctedDate = startDate.HasValue && startDate != new DateTime() ? startDate.Value : DateTime.Now;
            endDate = endDate == null || endDate == new DateTime() ? null : endDate;

            if (entry == null)
            {
                entry = new GroupInRole(groupId, roleId);
                entry.StartDate = correctedDate;
                entry.EndDate = endDate;
            }
            else
            {
                var newStartDate = startDate.HasValue ? correctedDate : entry.StartDate;
                var userAssignments = _dataSource.Query<User>().Where(u => u.Roles.Any(r => r.GroupRoleRoleId == roleId && r.GroupRoleGroupId == groupId)).SelectMany(u => u.Roles.Where(r => r.GroupRoleRoleId == roleId)).ToList();

                // Update user role dates based on the new group dates. If the range is more
                // limited, limit the user range as well. If it is broader, broaden the range for
                // users that have the same start and/or end date as the previous range.
                foreach (var assignment in userAssignments)
                {
                    assignment.StartDate = assignment.StartDate <= newStartDate ? newStartDate :
                                           assignment.StartDate == entry.StartDate ? newStartDate : assignment.StartDate;
                    assignment.EndDate = !assignment.EndDate.HasValue || assignment.EndDate.Value >= endDate ? endDate :
                                         assignment.EndDate.HasValue && assignment.EndDate.Value == entry.EndDate ? endDate : assignment.EndDate;
                }

                entry.StartDate = newStartDate;
                entry.EndDate = endDate;
            }

            entry.MaxNumberOfUsers = maxNumberOfUsers.HasValue ? maxNumberOfUsers.Value : entry.MaxNumberOfUsers;

            _dataSource.Save(entry);
        }

        public void AddUserToRole(Guid groupId, Guid userId, string roleName, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (groupId == Guid.Empty || userId == Guid.Empty || string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Please specify a group id, user id and/or a role name.");
            }

            var roleId = GetRoleId(roleName, "user", "add");
            var userEntry = _dataSource.Find<UserInRole>(new object[] { groupId, roleId, userId });
            var groupEntry = _dataSource.Find<GroupInRole>(new object[] { groupId, roleId });

            if (groupEntry == null)
            {
                if (!_user.IsInMainGroup)
                {
                    var message = string.Format("Trying to add user {0} to role {1} to which his company has no access.", userId, roleName);
                    Logger.LogToAudit(AuditLogType.IllegalOperation.ToString(), message);
                    throw new StrixMembershipException(message);
                }
                else
                {
                    groupEntry = new GroupInRole(StrixPlatform.ApplicationId, roleId);
                    _dataSource.Save(groupEntry);
                }
            }

            startDate = startDate.HasValue && startDate != new DateTime() ? startDate.Value : DateTime.Now;
            startDate = startDate < groupEntry.StartDate ? groupEntry.StartDate : startDate;

            endDate = endDate == new DateTime() ? null : endDate;
            endDate = endDate.HasValue && groupEntry.EndDate.HasValue && endDate.Value > groupEntry.EndDate ? groupEntry.EndDate : endDate;
            endDate = endDate == null && groupEntry.EndDate.HasValue ? groupEntry.EndDate : endDate;

            if (userEntry == null)
            {
                if (groupEntry.MaxNumberOfUsers.HasValue && groupEntry.CurrentNumberOfUsers >= groupEntry.MaxNumberOfUsers.Value)
                {
                    throw new StrixMembershipException(string.Format("No licenses left for role {0}", roleName));
                }

                userEntry = new UserInRole(groupEntry, userId, startDate.Value, endDate);
                groupEntry.CurrentNumberOfUsers++;
            }
            else
            {
                userEntry.StartDate = startDate.Value;
                userEntry.EndDate = endDate;
            }

            _dataSource.Save(userEntry);
            _userRoles.Clear();
        }

        #endregion AddToRole

        #region RemoveFromRole

        public void RemoveGroupFromRoles(Guid groupId, string[] roleNames)
        {
            if (groupId == Guid.Empty || roleNames == null)
            {
                throw new ArgumentException("Please specify a group id and/or one or more role names.");
            }

            List<Guid> roleIds = new List<Guid>();

            foreach (var name in roleNames)
            {
                roleIds.Add(GetRoleId(name, "group", "remove"));
            }

            // Get the group roles.
            var groupEntries = _dataSource.Query<GroupInRole>().Where(e => e.GroupId == groupId && roleIds.Contains(e.RoleId));

            // Get the user roles.
            var userEntries = _dataSource.Query<UserInRole>().Where(e => e.GroupRoleGroupId == groupId && roleIds.Contains(e.GroupRoleRoleId));

            // Delete the entries.
            _dataSource.Delete(userEntries);
            _dataSource.Delete(groupEntries);
        }

        public void RemoveUsersFromRoles(Guid[] userIds, string[] roleNames)
        {
            if (userIds.IsEmpty() || roleNames == null)
            {
                throw new ArgumentException("Please specify one or more user ids and/or one or more role names.");
            }

            List<Guid> roleIds = new List<Guid>();

            foreach (var name in roleNames)
            {
                roleIds.Add(GetRoleId(name, "user", "remove"));
            }

            var entries = _dataSource.Query<UserInRole>().Where(e => userIds.Contains(e.UserId) && roleIds.Contains(e.GroupRole.RoleId));
            var groupIds = entries.Select(e => e.GroupRoleGroupId).Distinct().ToArray();
            var groupEntries = _dataSource.Query<GroupInRole>().Where(e => groupIds.Contains(e.GroupId)).ToList();

            foreach (var entry in entries)
            {
                var groupEntry = groupEntries.First(g => g.GroupId == entry.GroupRoleGroupId);
                groupEntry.CurrentNumberOfUsers--;
            }

            _dataSource.Delete(entries);
            _userRoles.Clear();
        }

        #endregion RemoveFromRole

        #region Permissions

        public GroupInRole GetPermissionSetForGroup(Guid groupId)
        {
            return _dataSource.Query<Role>().Where(r => r.Permissions.Any(p => p.ApplicationId == _appId)
                                                             && r.GroupId == groupId
                                                             && r.Name.ToLower() == Resources.DefaultValues.PermissionSetName.ToLower())
                                                 .SelectMany(g => g.Groups).FirstOrDefault(g => g.GroupId == groupId);
        }

        public bool GroupUsesPermissions(Guid groupId)
        {
            if (_groupsUsePermissions.IsEmpty())
            {
                _groupsUsePermissions = _dataSource.Query<Group>().ToDictionary(k => k.Id, v => v.UsePermissions);
            }

            return _groupsUsePermissions[groupId];
        }

        public IQueryable<Permission> PermissionQuery()
        {
            return _dataSource.Query<Permission>().Where(p => p.ApplicationId == _appId);
        }

        #endregion Permissions

        #region Private Methods

        private Guid GetRoleId(string roleName, string entity, string action)
        {
            GetRoleIdsAndNames();
            var usePermissions = StrixMembership.Configuration.UsePermissions;
            var mainGroupId = StrixPlatform.MainGroupId;
            var dictionary = usePermissions ? _roleIdsAndNames[_user.GroupId] : _roleIdsAndNames[mainGroupId];

            if (!dictionary.ContainsValue(roleName.ToLower()))
            {
                var fromTo = action == "remove" ? "from" : "to";
                Logger.LogToAudit(AuditLogType.RoleAssignment.ToString(), string.Format("An attempt was made to {0} a {1} {2} a role ({3}) that is not available for application", action, entity, fromTo, roleName));
            }

            return dictionary.First(t => t.Value.ToLower() == roleName.ToLower()).Key;
        }

        private Dictionary<Guid, Dictionary<Guid, string>> GetRoleIdsAndNames()
        {
            if (_roleIdsAndNames.IsEmpty())
            {
                lock (_lockObject)
                {
                    if (_roleIdsAndNames.IsEmpty())
                    {
                        _roleIdsAndNames = new Dictionary<Guid, Dictionary<Guid, string>>();

                        foreach (var group in _dataSource.Query<Role>().Where(r => r.Permissions.Any(p => p.ApplicationId == _appId)).GroupBy(r => r.GroupId).ToList())
                        {
                            _roleIdsAndNames.Add(group.Key, null);
                            _roleIdsAndNames[group.Key] = group.ToDictionary(k => k.Id, v => v.Name.ToLower());
                        }
                    }
                }
            }

            return _roleIdsAndNames;
        }

        private void GetUserRoles()
        {
            if (_userRoles.IsEmpty())
            {
                var roleEntries = _dataSource.Query<User>().Select(u => new { Id = u.Id, Roles = u.Roles.Where(r => r.GroupRole.Role.Permissions.Any(p => p.ApplicationId == _appId)).Select(r => new { Name = r.GroupRole.Role.Name.ToLower(), StartDate = r.StartDate, EndDate = r.EndDate }) });

                foreach (var entry in roleEntries)
                {
                    var roleList = new List<Tuple<string, DateTime, DateTime?>>();

                    foreach (var role in entry.Roles)
                    {
                        roleList.Add(new Tuple<string, DateTime, DateTime?>(role.Name, role.StartDate, role.EndDate));
                    }

                    _userRoles.GetOrAdd(entry.Id, roleList);
                }
            }
        }

        private IQueryable<Role> Query(string include)
        {
            var query = _dataSource.Query<Role>();

            if (!string.IsNullOrWhiteSpace(include))
            {
                query = query.Include(include);
            }

            return query.Where(r => r.Permissions.Any(p => p.ApplicationId == _appId)
                && r.GroupId == _user.GroupId
                && r.Name.ToLower() != PlatformConstants.ADMINROLE.ToLower());
        }

        #endregion Private Methods
    }
}