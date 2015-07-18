#region Apache License
//-----------------------------------------------------------------------
// <copyright file="RoleController.cs" company="StrixIT">
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
using System.Web.Mvc;
using StrixIT.Platform.Core;
using StrixIT.Platform.Web;

namespace StrixIT.Platform.Modules.Membership
{
    [StrixAuthorization(Permissions = MembershipPermissions.ViewRoles)]
    public class RoleController : BaseCrudController<Guid, RoleViewModel>
    {
        public RoleController(IRoleService service) : base(service) 
        {
            if (!StrixMembership.Configuration.UsePermissions)
            {
                throw new InvalidOperationException(Resources.Interface.GroupsNotEnabed);
            }
        }

        public override ActionResult Index()
        {
            var config = new RoleListConfiguration(StrixPlatform.User);
            return this.View(config);
        }
    }
}