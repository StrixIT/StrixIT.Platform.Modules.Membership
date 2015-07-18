//-----------------------------------------------------------------------
// <copyright file="RoleManager.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class RoleManager : IRoleManager
    {
        private static Guid _appId;

        private static object _lockObject = new object();

        private static Dictionary<Guid, Dictionary<Guid, string>> _roleIdsAndNames;

        private static ConcurrentDictionary<Guid, List<Tuple<string, DateTime, DateTime?>>> _userRoles = new ConcurrentDictionary<Guid, List<Tuple<string, DateTime, DateTime?>>>();

        private static Dictionary<Guid, bool> _groupsUsePermissions = new Dictionary<Guid, bool>();

        private IMembershipDataSource _dataSource;

        public RoleManager(IMembershipDataSource dataSource)
        {
            this._dataSource = dataSource;

            // Get the application id and load the roles dictionaries on first load.
            if (_appId == Guid.Empty)
            {
                _appId = ApplicationHelper.GetApplicationId(dataSource);
                this.GetRoleIdsAndNames();
                this.GetUserRoles();
            }
        }

        #region Create Roles

        public Role Create(string name, string description, IList<Permission> permissions)
        {
            if (this.Exists(name, null))
            {
                return this._dataSource.Query<Role>().First(r => r.Name.ToLower() == name.ToLower());
            }

            var currentGroupId = StrixPlatform.User.GroupId;
            var role = new Role(Guid.NewGuid(), currentGroupId, name);
            role.Description = description;
            role.Permissions = permissions;
            _roleIdsAndNames.Clear();
            return this._dataSource.Save(role);
        }

        #endregion

        #region Update

        public Role Update(Guid id, string name, string description, IList<Permission> permissions)
        {
            var role = this.Query("Permissions").First(r => r.Id == id);
            role.Name = name;
            role.Description = description;
            role.Permissions = permissions;
            _roleIdsAndNames.Clear();
            return this._dataSource.Save(role);
        }

        #endregion

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

            return this._dataSource.Query<Role>().Any(r => r.Permissions.Any(p => p.ApplicationId == _appId)
                && r.GroupId == StrixPlatform.User.GroupId
                && r.Name.ToLower() == name.ToLower()
                && (!id.HasValue || r.Id != id.Value));
        }

        #endregion

        #region IsInRole

        public bool IsUserInRole(Guid id, string roleName)
        {
            return this.IsUserInRoles(id, new string[] { roleName });
        }

        public bool IsUserInRoles(Guid id, string[] roleNames)
        {
            if (id == Guid.Empty || roleNames.IsEmpty())
            {
                throw new ArgumentException("Please specify an id and/or one or more role names.");
            }

            roleNames = roleNames.ToLower().ToArray();
            this.GetUserRoles();
            return _userRoles[id].Any(r => roleNames.Contains(r.Item1) && r.Item2 <= DateTime.Now && (!r.Item3.HasValue || r.Item3.Value > DateTime.Now));
        }

        #endregion

        #region Get

        public Role Get(Guid id)
        {
            return this.Query().FirstOrDefault(r => r.Id == id);
        }

        #endregion

        #region Delete

        public void Delete(Guid id)
        {
            var role = this.Get(id);

            if (role != null && role.Name.ToLower() != PlatformConstants.ADMINROLE.ToLower())
            {
                this._dataSource.Delete(role);
            }
        }

        #endregion

        #region Query

        public IQueryable<Role> Query()
        {
            return this.Query(null);
        }

        public IQueryable<AssignRoleModel> QueryForGroup(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentException("GroupId must have a value", "groupId");
            }

            return this._dataSource.Query<Group>().Where(g => g.Id == groupId).SelectMany(g => g.Roles.Where(r => r.Role.Permissions.Any(p => p.ApplicationId == _appId) && r.Role.Name.ToLower() != Resources.DefaultValues.PermissionSetName.ToLower())).Select(r => new AssignRoleModel { Id = r.RoleId, Name = r.Role.Name, StartDate = r.StartDate, EndDate = r.EndDate, MaxNumberOfUsers = r.MaxNumberOfUsers, CurrentNumberOfUsers = r.CurrentNumberOfUsers });
        }

        public IQueryable<AssignRoleModel> QueryForUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("userId cannot be empty", "userId");
            }

            return this._dataSource.Query<User>().Where(u => userId == u.Id).SelectMany(u => u.Roles.Where(r => r.GroupRole.Role.Permissions.Any(p => p.ApplicationId == _appId)).Select(r => new AssignRoleModel { Id = r.GroupRole.RoleId, Name = r.GroupRole.Role.Name, StartDate = r.StartDate, EndDate = r.EndDate, MaxNumberOfUsers = r.GroupRole.MaxNumberOfUsers, CurrentNumberOfUsers = r.GroupRole.CurrentNumberOfUsers }));
        }

        #endregion

        #region AddToRole

        public void AddGroupToRole(Guid groupId, string roleName, DateTime? startDate = null, DateTime? endDate = null, int? maxNumberOfUsers = null)
        {
            if (groupId == Guid.Empty || string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Please specify a group id and/or a role name.");
            }

            if (!StrixPlatform.User.IsInMainGroup && roleName.ToLower() == PlatformConstants.ADMINROLE.ToLower())
            {
                var message = string.Format("Trying to add group {0} to role {1}, which is a reserved role.", groupId, roleName);
                Logger.LogToAudit(AuditLogType.IllegalOperation.ToString(), message);
                throw new StrixMembershipException(message);
            }

            var roleId = this.GetRoleId(roleName, "group", "add");
            var entry = this._dataSource.Find<GroupInRole>(new object[] { groupId, roleId });
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
                var userAssignments = this._dataSource.Query<User>().Where(u => u.Roles.Any(r => r.GroupRoleRoleId == roleId && r.GroupRoleGroupId == groupId)).SelectMany(u => u.Roles.Where(r => r.GroupRoleRoleId == roleId)).ToList();

                // Update user role dates based on the new group dates. If the range is more limited, limit the user range as well. If it is broader,
                // broaden the range for users that have the same start and/or end date as the previous range.
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

            this._dataSource.Save(entry);
        }

        public void AddUserToRole(Guid groupId, Guid userId, string roleName, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (groupId == Guid.Empty || userId == Guid.Empty || string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Please specify a group id, user id and/or a role name.");
            }

            var roleId = this.GetRoleId(roleName, "user", "add");
            var userEntry = this._dataSource.Find<UserInRole>(new object[] { groupId, roleId, userId });
            var groupEntry = this._dataSource.Find<GroupInRole>(new object[] { groupId, roleId });

            if (groupEntry == null)
            {
                if (!StrixPlatform.User.IsInMainGroup)
                {
                    var message = string.Format("Trying to add user {0} to role {1} to which his company has no access.", userId, roleName);
                    Logger.LogToAudit(AuditLogType.IllegalOperation.ToString(), message);
                    throw new StrixMembershipException(message);
                }
                else
                {
                    groupEntry = new GroupInRole(StrixPlatform.ApplicationId, roleId);
                    this._dataSource.Save(groupEntry);
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

            this._dataSource.Save(userEntry);
            _userRoles.Clear();
        }

        #endregion

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
                roleIds.Add(this.GetRoleId(name, "group", "remove"));
            }

            // Get the group roles.
            var groupEntries = this._dataSource.Query<GroupInRole>().Where(e => e.GroupId == groupId && roleIds.Contains(e.RoleId));

            // Get the user roles.
            var userEntries = this._dataSource.Query<UserInRole>().Where(e => e.GroupRoleGroupId == groupId && roleIds.Contains(e.GroupRoleRoleId));

            // Delete the entries.
            this._dataSource.Delete(userEntries);
            this._dataSource.Delete(groupEntries);
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
                roleIds.Add(this.GetRoleId(name, "user", "remove"));
            }

            var entries = this._dataSource.Query<UserInRole>().Where(e => userIds.Contains(e.UserId) && roleIds.Contains(e.GroupRole.RoleId));
            var groupIds = entries.Select(e => e.GroupRoleGroupId).Distinct().ToArray();
            var groupEntries = this._dataSource.Query<GroupInRole>().Where(e => groupIds.Contains(e.GroupId)).ToList();

            foreach (var entry in entries)
            {
                var groupEntry = groupEntries.First(g => g.GroupId == entry.GroupRoleGroupId);
                groupEntry.CurrentNumberOfUsers--;
            }

            this._dataSource.Delete(entries);
            _userRoles.Clear();
        }

        #endregion

        #region Permissions

        public IQueryable<Permission> PermissionQuery()
        {
            return this._dataSource.Query<Permission>().Where(p => p.ApplicationId == _appId);
        }

        public bool GroupUsesPermissions(Guid groupId)
        {
            if (_groupsUsePermissions.IsEmpty())
            {
                _groupsUsePermissions = this._dataSource.Query<Group>().ToDictionary(k => k.Id, v => v.UsePermissions);
            }

            return _groupsUsePermissions[groupId];
        }

        public GroupInRole GetPermissionSetForGroup(Guid groupId)
        {
            return this._dataSource.Query<Role>().Where(r => r.Permissions.Any(p => p.ApplicationId == _appId)
                                                             && r.GroupId == groupId
                                                             && r.Name.ToLower() == Resources.DefaultValues.PermissionSetName.ToLower())
                                                 .SelectMany(g => g.Groups).FirstOrDefault(g => g.GroupId == groupId);
        }

        #endregion

        #region Private Methods

        private Dictionary<Guid, Dictionary<Guid, string>> GetRoleIdsAndNames()
        {
            if (_roleIdsAndNames.IsEmpty())
            {
                lock (_lockObject)
                {
                    if (_roleIdsAndNames.IsEmpty())
                    {
                        _roleIdsAndNames = new Dictionary<Guid, Dictionary<Guid, string>>();

                        foreach (var group in this._dataSource.Query<Role>().Where(r => r.Permissions.Any(p => p.ApplicationId == _appId)).GroupBy(r => r.GroupId).ToList())
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
                var roleEntries = this._dataSource.Query<User>().Select(u => new { Id = u.Id, Roles = u.Roles.Where(r => r.GroupRole.Role.Permissions.Any(p => p.ApplicationId == _appId)).Select(r => new { Name = r.GroupRole.Role.Name.ToLower(), StartDate = r.StartDate, EndDate = r.EndDate }) });

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

        private Guid GetRoleId(string roleName, string entity, string action)
        {
            this.GetRoleIdsAndNames();
            var usePermissions = StrixMembership.Configuration.UsePermissions;
            var mainGroupId = StrixPlatform.MainGroupId;
            var dictionary = usePermissions ? _roleIdsAndNames[StrixPlatform.User.GroupId] : _roleIdsAndNames[mainGroupId];

            if (!dictionary.ContainsValue(roleName.ToLower()))
            {
                var fromTo = action == "remove" ? "from" : "to";
                Logger.LogToAudit(AuditLogType.RoleAssignment.ToString(), string.Format("An attempt was made to {0} a {1} {2} a role ({3}) that is not available for this application", action, entity, fromTo, roleName));
            }

            return dictionary.First(t => t.Value.ToLower() == roleName.ToLower()).Key;
        }

        private IQueryable<Role> Query(string include)
        {
            var query = this._dataSource.Query<Role>();

            if (!string.IsNullOrWhiteSpace(include))
            {
                query = query.Include(include);
            }

            return query.Where(r => r.Permissions.Any(p => p.ApplicationId == _appId)
                && r.GroupId == StrixPlatform.User.GroupId
                && r.Name.ToLower() != PlatformConstants.ADMINROLE.ToLower());
        }

        #endregion
    }
}