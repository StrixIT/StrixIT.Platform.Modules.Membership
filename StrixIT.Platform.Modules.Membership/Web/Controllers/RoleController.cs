//-----------------------------------------------------------------------
// <copyright file="RoleController.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
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