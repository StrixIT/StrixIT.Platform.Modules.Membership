#region Apache License
//-----------------------------------------------------------------------
// <copyright file="IRoleManager.cs" company="StrixIT">
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
    /// <summary>
    /// The role manager interface.
    /// </summary>
    public interface IRoleManager
    {
        /// <summary>
        /// Checks whether a role with the specified name already exists for the current group.
        /// </summary>
        /// <param name="name">The name to check for</param>
        /// <param name="id">The id of the role to check the name for</param>
        /// <returns>True if a role with the name already exists, false otherwise</returns>
        bool Exists(string name, Guid? id);

        #region Create Role

        /// <summary>
        /// Create a new role with permissions.
        /// </summary>
        /// <param name="name">The name of the role</param>
        /// <param name="description">The description for the role</param>
        /// <param name="permissions">The permissions for the role</param>
        /// <returns>The created role</returns>
        Role Create(string name, string description, IList<Permission> permissions);

        #endregion

        #region IsInRole

        /// <summary>
        /// Checks whether a user is in a role.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <param name="roleName">The name of the role</param>
        /// <returns>True if the user is in the role, false otherwise</returns>
        bool IsUserInRole(Guid id, string roleName);

        /// <summary>
        /// Checks whether a user is in one of the roles.
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <param name="roleNames">The names of the roles</param>
        /// <returns>True if the user is in one of the roles, false otherwise</returns>
        bool IsUserInRoles(Guid id, string[] roleNames);

        #endregion

        /// <summary>
        /// Gets the role with the specified id.
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns>The role</returns>
        Role Get(Guid id);

        /// <summary>
        /// Updates a role.
        /// </summary>
        /// <param name="id">The id of the role</param>
        /// <param name="name">The name for the role</param>
        /// <param name="description">The description for the role</param>
        /// <param name="permissions">The permissions for the role</param>
        /// <returns>The updated role</returns>
        Role Update(Guid id, string name, string description, IList<Permission> permissions);

        #region Query

        /// <summary>
        /// Gets a query for all roles.
        /// </summary>
        /// <returns>The query for the roles</returns>
        IQueryable<Role> Query();

        /// <summary>
        /// Gets a query for the roles of the specified group.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <returns>A query for the roles of the group</returns>
        IQueryable<AssignRoleModel> QueryForGroup(Guid groupId);

        /// <summary>
        /// Gets a query for the roles of the specified user.
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>A query for the roles of the user</returns>
        IQueryable<AssignRoleModel> QueryForUser(Guid userId);

        #endregion

        #region AddToRole

        /// <summary>
        /// Adds a group to a role.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <param name="roleName">The role name</param>
        /// <param name="startDate">The start date for the group in the role</param>
        /// <param name="endDate">The end date for the group in the role</param>
        /// <param name="maxNumberOfUsers">The maximum number of users that can be in this role for the group</param>
        void AddGroupToRole(Guid groupId, string roleName, DateTime? startDate = null, DateTime? endDate = null, int? maxNumberOfUsers = null);

        /// <summary>
        /// Adds a user to a role.
        /// </summary>
        /// <param name="groupId">The user's group id</param>
        /// <param name="userId">The user id</param>
        /// <param name="roleName">The role name</param>
        /// <param name="startDate">The start date for the user in the role</param>
        /// <param name="endDate">The end date for the user in the role</param>
        void AddUserToRole(Guid groupId, Guid userId, string roleName, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Removes a group from one or more roles.
        /// </summary>
        /// <param name="groupId">The group id</param>
        /// <param name="roleNames">The names of the roles to remove the group from</param>
        void RemoveGroupFromRoles(Guid groupId, string[] roleNames);

        /// <summary>
        /// Removes users from one or more roles.
        /// </summary>
        /// <param name="userIds">The user ids</param>
        /// <param name="roleNames">The names of the roles to remove the users from</param>
        void RemoveUsersFromRoles(Guid[] userIds, string[] roleNames);

        #endregion

        #region Permissions

        IQueryable<Permission> PermissionQuery();

        /// <summary>
        /// Gets a value indicating whether the specified group uses permissions.
        /// </summary>
        /// <param name="groupId">The id of group to check using permissions for</param>
        /// <returns>True if the group uses permissions, false otherwise</returns>
        bool GroupUsesPermissions(Guid groupId);

        /// <summary>
        /// Gets the permission set for the specified group.
        /// </summary>
        /// <param name="groupId">The id of the group to get the permission set for</param>
        /// <returns>The group permission set</returns>
        GroupInRole GetPermissionSetForGroup(Guid groupId);

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the role with the specified id.
        /// </summary>
        /// <param name="id">The id</param>
        void Delete(Guid id);

        #endregion
    }
}
