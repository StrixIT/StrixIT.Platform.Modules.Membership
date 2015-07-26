#region Apache License

//-----------------------------------------------------------------------
// <copyright file="GroupController.cs" company="StrixIT">
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

#endregion Apache License

using StrixIT.Platform.Core;
using StrixIT.Platform.Web;
using System;
using System.Web.Mvc;

namespace StrixIT.Platform.Modules.Membership
{
    [StrixAuthorization(Permissions = MembershipPermissions.ViewGroups)]
    public class GroupController : BaseCrudController<Guid, GroupViewModel>
    {
        #region Public Constructors

        public GroupController(IGroupService service) : base(service)
        {
        }

        #endregion Public Constructors

        #region Protected Properties

        protected IGroupService Service
        {
            get
            {
                return this._service as IGroupService;
            }
        }

        #endregion Protected Properties

        #region Public Methods

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

        public override ActionResult Index()
        {
            var config = new GroupListConfiguration(StrixPlatform.User);
            return this.View(config);
        }

        #endregion Public Methods
    }
}