//-----------------------------------------------------------------------
// <copyright file="UserListConfiguration.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class UserListConfiguration : ListConfiguration
    {
        public UserListConfiguration(IUserContext userContext) : base(typeof(UserListModel), new string[] { "Email" })
        {
            this.InterfaceResourceType = typeof(Resources.Interface);
            this.Fields.Add(new ListFieldConfiguration("LockedOut", "bool"));
            this.Fields.Add(new ListFieldConfiguration("Approved", "bool"));

            this.CanCreate = userContext.HasPermission(MembershipPermissions.AddUser);
            this.CanEdit = userContext.HasPermission(MembershipPermissions.EditUser);
            this.CanDelete = userContext.HasPermission(MembershipPermissions.DeleteUser);
        }
    }
}