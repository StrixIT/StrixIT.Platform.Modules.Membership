//-----------------------------------------------------------------------
// <copyright file="UserController.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Web.Mvc;
using StrixIT.Platform.Core;
using StrixIT.Platform.Web;

namespace StrixIT.Platform.Modules.Membership
{
    [StrixAuthorization(Permissions = MembershipPermissions.ViewUsers)]
    public class UserController : BaseCrudController<Guid, UserViewModel>
    {
        public UserController(IUserService service) : base(service) { }

        protected IUserService Service
        {
            get
            {
                return this._service as IUserService;
            }
        }

        public override ActionResult Index()
        {
            var config = new UserListConfiguration(StrixPlatform.User);
            return this.View(config);
        }

        public JsonResult CheckEmail(string value, Guid id)
        {
            return new JsonStatusResult() { Success = !this.Service.Exists(value, id) };
        }
    }
}