#region Apache License
//-----------------------------------------------------------------------
// <copyright file="MembershipPermissions.cs" company="StrixIT">
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