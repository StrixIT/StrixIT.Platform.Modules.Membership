//-----------------------------------------------------------------------
// <copyright file="GroupViewModel.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for creating and editing groups.
    /// </summary>
    public class GroupViewModel : BaseCrudDto
    {
        public GroupViewModel() : base(typeof(Group)) 
        {
            this.CanEdit = StrixPlatform.User.HasPermission(MembershipPermissions.EditGroup);
            this.CanDelete = StrixPlatform.User.HasPermission(MembershipPermissions.DeleteGroup);
        }

        /// <summary>
        /// Gets or sets the id of the group.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether permissions are to be used for the group.
        /// </summary>
        public bool UsePermissions { get; set; }

        /// <summary>
        /// Gets or sets the permission set start date.
        /// </summary>
        public DateTime? PermissionSetStartDate { get; set; }

        /// <summary>
        /// Gets or sets the permission set end date.
        /// </summary>
        public DateTime? PermissionSetEndDate { get; set; }

        /// <summary>
        /// Gets or sets the permission set maximum number of users.
        /// </summary>
        public int? PermissionSetMaxNumberOfUsers { get; set; }

        /// <summary>
        /// Gets or sets all available roles.
        /// </summary>
        public IList<AssignRoleModel> Roles { get; set; }

        /// <summary>
        /// Gets or sets all available permissions.
        /// </summary>
        public IList<AssignPermissionModel> Permissions { get; set; }
    }
}