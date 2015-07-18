//-----------------------------------------------------------------------
// <copyright file="RoleViewModel.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for roles.
    /// </summary>
    public class RoleViewModel : BaseCrudDto
    {
        public RoleViewModel() : base(typeof(Role)) 
        {
            this.CanEdit = StrixPlatform.User.HasPermission(MembershipPermissions.EditRole);
            this.CanDelete = StrixPlatform.User.HasPermission(MembershipPermissions.DeleteRole);
        }

        /// <summary>
        /// Gets or sets the role id.
        /// </summary> 
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the role name.
        /// </summary> 
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the role description.
        /// </summary> 
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the role's permissions.
        /// </summary> 
        public IList<AssignPermissionModel> Permissions { get; set; }
    }
}
