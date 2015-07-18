//-----------------------------------------------------------------------
// <copyright file="MembershipAreaRegistration.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Web.Mvc;
using StrixIT.Platform.Core;
using StrixIT.Platform.Web;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Membership";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var culture = StrixPlatform.DefaultCultureCode.ToLower();

            context.MapLocalizedRoute(
                "Account_ResetLink",
                "{language}/Account/SendPasswordLink/{userId}",
                new { language = culture, controller = MembershipConstants.ACCOUNT, action = "SendPasswordLink", key = UrlParameter.Optional });

            context.MapLocalizedRoute(
                "Account_Default",
                "{language}/Account/{action}/{key}",
                new { language = culture, controller = MembershipConstants.ACCOUNT, action = MvcConstants.INDEX, key = UrlParameter.Optional });

            context.MapLocalizedRoute(
                "Membership_admin",
                "{language}/Admin/Membership/{controller}/{action}/{id}",
                new { language = culture, controller = "Base", action = MvcConstants.INDEX, id = UrlParameter.Optional });
        }
    }
}