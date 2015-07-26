#region Apache License

//-----------------------------------------------------------------------
// <copyright file="UserController.cs" company="StrixIT">
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
    [StrixAuthorization(Permissions = MembershipPermissions.ViewUsers)]
    public class UserController : BaseCrudController<Guid, UserViewModel>
    {
        #region Public Constructors

        public UserController(IUserService service) : base(service)
        {
        }

        #endregion Public Constructors

        #region Protected Properties

        protected IUserService Service
        {
            get
            {
                return this._service as IUserService;
            }
        }

        #endregion Protected Properties

        #region Public Methods

        public JsonResult CheckEmail(string value, Guid id)
        {
            return new JsonStatusResult() { Success = !this.Service.Exists(value, id) };
        }

        public override ActionResult Index()
        {
            var config = new UserListConfiguration(StrixPlatform.User);
            return this.View(config);
        }

        #endregion Public Methods
    }
}