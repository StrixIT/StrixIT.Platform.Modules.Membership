//-----------------------------------------------------------------------
// <copyright file="IGroupManager.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The group manager interface.
    /// </summary>
    public interface IGroupManager
    {
        #region Exists

        /// <summary>
        /// Checks if a group with the specified name already exists.
        /// </summary>
        /// <param name="name">The name to check</param>
        /// <param name="id">The id of the group to check the name availability for</param>
        /// <returns>True if a group with the name already exists, false otherwise</returns>
        bool Exists(string name, Guid? id);

        #endregion

        #region Get

        /// <summary>
        /// Gets the specified group.
        /// </summary>
        /// <param name="id">The id of the group to get</param>
        /// <returns>The group</returns>
        Group Get(Guid id);

        /// <summary>
        /// Gets the specified group.
        /// </summary>
        /// <param name="name">The name of the group to get</param>
        /// <returns>The group</returns>
        Group Get(string name);

        #endregion

        /// <summary>
        /// Gets a query for all groups.
        /// </summary>
        /// <returns>The group query</returns>
        IQueryable<Group> Query();

        #region Save

        /// <summary>
        /// Creates a new group.
        /// </summary>
        /// <param name="name">The group name</param>
        /// <param name="usePermissions">True if the group uses permissions, false otherwise</param>
        /// <returns>The new group</returns>
        Group Create(string name, bool usePermissions);

        /// <summary>
        /// Updates a group.
        /// </summary>
        /// <param name="id">The id of the group to update</param>
        /// <param name="name">The group name</param>
        /// <param name="usePermissions">True if the group uses permissions, false otherwise</param>
        /// <returns>The updated group</returns>
        Group Update(Guid id, string name, bool usePermissions);

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the group with the specified id.
        /// </summary>
        /// <param name="id">The group id</param>
        void Delete(Guid id);

        #endregion
    }
}
