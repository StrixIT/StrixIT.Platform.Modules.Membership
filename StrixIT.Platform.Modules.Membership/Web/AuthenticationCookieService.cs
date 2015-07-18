//-----------------------------------------------------------------------
// <copyright file="AuthenticationCookieService.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Web;
using System.Web.Security;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class AuthenticationCookieService : IAuthenticationCookieService
    {
        public void SetAuthCookie(string userName)
        {
            if (HttpContext.Current != null)
            {
                var createPersistentCookie = !StrixMembership.Configuration.LimitAuthenticationToBrowserSession;
                FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
            }
        }

        public void SignOut()
        {
            if (HttpContext.Current != null)
            {
                FormsAuthentication.SignOut();
            }
        }
    }
}