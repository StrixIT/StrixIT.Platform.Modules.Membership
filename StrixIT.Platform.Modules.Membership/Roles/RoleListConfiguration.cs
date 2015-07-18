//-----------------------------------------------------------------------
// <copyright file="RoleListConfiguration.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class RoleListConfiguration : ListConfiguration
    {
        public RoleListConfiguration(IUserContext userContext) : base(typeof(RoleViewModel), null)
        {
            this.InterfaceResourceType = typeof(Resources.Interface);
            this.CanCreate = userContext.HasPermission(MembershipPermissions.AddRole);
            this.CanEdit = userContext.HasPermission(MembershipPermissions.EditRole);
            this.CanDelete = userContext.HasPermission(MembershipPermissions.DeleteRole);
        }
    }
}