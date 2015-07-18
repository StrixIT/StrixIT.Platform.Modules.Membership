//-----------------------------------------------------------------------
// <copyright file="MembershipInitializer.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Linq;
using System.Web.Mvc;
using StrixIT.Platform.Core;
using StrixIT.Platform.Web;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipInitializer : IInitializer, IWebInitializer
    {
        public void Initialize()
        {
            DataMapper.CreateMap<User, UserViewModel>();
            DataMapper.CreateMap<UserViewModel, User>().ForMember(us => us.Roles, c => c.Ignore());
            DataMapper.CreateMap<UserInRole, AssignRoleModel>().ForMember(ar => ar.Id, c => c.MapFrom(ur => ur.UserId));
            DataMapper.CreateMap<GroupViewModel, Group>().ForMember(gr => gr.Roles, c => c.Ignore());
            DataMapper.CreateMap<Group, GroupListModel>();
            DataMapper.CreateMap<GroupInRole, AssignRoleModel>().ForMember(ar => ar.Id, c => c.MapFrom(gr => gr.GroupId));
            DataMapper.CreateMap<Permission, AssignPermissionModel>().ForMember(p => p.Description, c => c.Ignore());
        }

        public void WebInitialize()
        {
            var razorEngine = ViewEngines.Engines.OfType<RazorViewEngine>().First();
            razorEngine.PartialViewLocationFormats = razorEngine.PartialViewLocationFormats.Concat(new string[] { "~/Areas/Membership/Views/Shared/{0}.cshtml" }).ToArray();
            razorEngine.AreaPartialViewLocationFormats = razorEngine.AreaPartialViewLocationFormats.Concat(new string[] { "~/Areas/Membership/Views/Shared/{0}.cshtml" }).ToArray();
        }
    }
}