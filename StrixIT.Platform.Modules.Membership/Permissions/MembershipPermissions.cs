//-----------------------------------------------------------------------
// <copyright file="MembershipPermissions.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Collections.Generic;

namespace StrixIT.Platform.Modules.Membership
{
    public static class MembershipPermissions
    {
        public const string ViewUsers = "StrixIT.Membership.ViewUsers";
        public const string AddUser = "StrixIT.Membership.AddUser";
        public const string EditUser = "StrixIT.Membership.EditUser";
        public const string DeleteUser = "StrixIT.Membership.DeleteUser";

        public const string ViewGroups = "StrixIT.Membership.ViewGroups";
        public const string AddGroup = "StrixIT.Membership.AddGroup";
        public const string EditGroup = "StrixIT.Membership.EditGroup";
        public const string DeleteGroup = "StrixIT.Membership.DeleteGroup";

        public const string ViewRoles = "StrixIT.Membership.ViewRoles";
        public const string AddRole = "StrixIT.Membership.AddRole";
        public const string EditRole = "StrixIT.Membership.EditRole";
        public const string DeleteRole = "StrixIT.Membership.DeleteRole";
    }
}