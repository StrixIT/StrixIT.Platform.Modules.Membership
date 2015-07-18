//-----------------------------------------------------------------------
// <copyright file="GroupController.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StrixIT.Platform.Core;
using StrixIT.Platform.Web;

namespace StrixIT.Platform.Modules.Membership
{
    [StrixAuthorization(Permissions = MembershipPermissions.ViewGroups)]
    public class GroupController : BaseCrudController<Guid, GroupViewModel>
    {
        public GroupController(IGroupService service) : base(service) { }

        protected IGroupService Service
        {
            get
            {
                return this._service as IGroupService;
            }
        }

        public override ActionResult Index()
        {
            var config = new GroupListConfiguration(StrixPlatform.User);
            return this.View(config);
        }

        [HttpPost]
        [StrixAuthorization(Roles = PlatformConstants.ADMINROLE)]
        public JsonResult ChangeGroup(Guid groupId)
        {
            if (StrixPlatform.User.Groups.ContainsKey(groupId))
            {
                StrixPlatform.Environment.StoreInSession(PlatformConstants.CURRENTGROUPID, groupId);
                StrixPlatform.Environment.StoreInSession(PlatformConstants.CURRENTUSER, null);
                return this.Json(true);
            }

            return this.Json(false);
        }
    }
}