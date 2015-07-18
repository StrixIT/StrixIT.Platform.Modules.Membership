//-----------------------------------------------------------------------
// <copyright file="GroupListConfiguration.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class GroupListConfiguration : ListConfiguration
    {
        public GroupListConfiguration(IUserContext userContext) : base(typeof(GroupListModel))
        {
            this.InterfaceResourceType = typeof(Resources.Interface);
            this.CanCreate = userContext.HasPermission(MembershipPermissions.AddGroup);
            this.CanEdit = userContext.HasPermission(MembershipPermissions.EditGroup);
            this.CanDelete = userContext.HasPermission(MembershipPermissions.DeleteGroup);
        }
    }
}