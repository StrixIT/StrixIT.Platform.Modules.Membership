#region Apache License

//-----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="StrixIT">
// Copyright 2015 StrixIT. Author R.G. Schurgers MA MSc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use file except in compliance with the License.
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
using System.Linq;
using System.Web.Mvc;

namespace StrixIT.Platform.Modules.Membership
{
    [StrixAuthorization]
    public class AccountController : BaseController
    {
        #region Private Fields

        private IAccountService _accountService;
        private IAuthenticationService _authenticationService;

        #endregion Private Fields

        #region Public Constructors

        public AccountController(IEnvironment environment, IAuthenticationService authenticationService, IAccountService accountService) : base(environment)
        {
            _authenticationService = authenticationService;
            _accountService = accountService;
        }

        #endregion Public Constructors

        #region Login/Logout

        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            model.ReturnUrl = Request.Params["returnUrl"];
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (ModelState.IsValid)
            {
                var result = _authenticationService.LogOn(model.Email, model.Password);

                if (result.Success)
                {
                    var returnUrl = HtmlHelpers.HtmlDecode(model.ReturnUrl, false);

                    if (Url != null && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        var redirectParams = Environment.Cultures.Cultures.Count() > 1 ? new { area = string.Empty, language = result.PreferredCulture } : new { area = string.Empty, language = string.Empty };
                        return RedirectToAction(MvcConstants.INDEX, "Home", redirectParams);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Resources.Interface.LoginFailedContainer);
                    ViewBag.LoginError = result.Message;
                }
            }

            // If we got far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {
            _authenticationService.LogOff(Environment.User.Email);
            var returnCulture = Environment.Cultures.CurrentCultureCode == Environment.Cultures.DefaultCultureCode ? null : Environment.Cultures.CurrentCultureCode;
            return RedirectToAction(MvcConstants.INDEX, MvcConstants.HOME, new { area = string.Empty, culture = returnCulture });
        }

        #endregion Login/Logout

        #region Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (!Environment.Configuration.GetConfiguration<MembershipConfiguration>().AllowUserRegistration)
            {
                return null;
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
            if (model == null)
            {
                return null;
            }

            if (!Environment.Configuration.GetConfiguration<MembershipConfiguration>().AllowUserRegistration)
            {
                return null;
            }

            if (ModelState.IsValid)
            {
                var result = _accountService.RegisterAccount(model);

                if (result.Success && result.Message == null)
                {
                    return RedirectToAction("RegisterSuccess");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterSuccess()
        {
            if (!Environment.Configuration.GetConfiguration<MembershipConfiguration>().AllowUserRegistration)
            {
                return null;
            }

            return View();
        }

        #endregion Register

        #region Password Reset

        [AllowAnonymous]
        public ActionResult RequestSetPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RequestSetPassword(string email)
        {
            // Todo: why length? Use proper regex
            if (string.IsNullOrWhiteSpace(email) || email.Length < 8)
            {
                ModelState.AddModelError("Email", Resources.Interface.InvalidEmail);
                return View();
            }

            _accountService.SendPasswordResetLink(email);
            return RedirectToAction("SetPasswordRequested");
        }

        [AllowAnonymous]
        public ActionResult SendPasswordLink(Guid userId)
        {
            _accountService.SendPasswordResetLink(userId);
            TempData["NewAccount"] = true;
            return RedirectToAction("SetPasswordRequested");
        }

        [AllowAnonymous]
        public ActionResult SetPassword(Guid? key)
        {
            SetPasswordViewModel model = new SetPasswordViewModel();

            if (key.HasValue)
            {
                var user = _accountService.GetUserByResetKey(key.Value);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, Resources.Interface.InvalidPasswordSetKey);
                }

                model.PasswordResetKey = key.Value;
            }

            ViewBag.MinRequiredNonalphanumericCharacters = Environment.Configuration.GetConfiguration<MembershipConfiguration>().MinRequiredNonAlphanumericCharacters;
            ViewBag.PasswordLength = Environment.Configuration.GetConfiguration<MembershipConfiguration>().MinRequiredPasswordLength;
            model.ReturnUrl = Request != null && Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : null;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetPassword(SetPasswordViewModel model)
        {
            if (model == null)
            {
                return null;
            }

            if (!ControllerContext.HttpContext.Request.IsAuthenticated && model.PasswordResetKey == Guid.Empty)
            {
                return null;
            }

            if (ModelState.IsValid)
            {
                SaveResult<UserViewModel> result;
                var email = Environment.User.Email;

                if (!string.IsNullOrWhiteSpace(email) && model.OldPassword != null && model.OldPassword.ToString() != model.PasswordResetKey.ToString())
                {
                    result = _accountService.ChangePassword(email, model.OldPassword, model.NewPassword);
                }
                else
                {
                    var user = _accountService.GetUserByResetKey(model.PasswordResetKey);
                    result = _accountService.ChangePassword(user.Email, null, model.NewPassword, model.PasswordResetKey);
                }

                if (result.Success && result.Message == null)
                {
                    TempData["ReturnUrl"] = model.ReturnUrl;
                    TempData["PasswordSetMode"] = model.PasswordResetKey == Guid.Empty ? "Change" : "Register";
                    return RedirectToAction("SetPasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            ViewBag.MinRequiredNonalphanumericCharacters = Environment.Configuration.GetConfiguration<MembershipConfiguration>().MinRequiredNonAlphanumericCharacters;
            ViewBag.PasswordLength = Environment.Configuration.GetConfiguration<MembershipConfiguration>().MinRequiredPasswordLength;
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult SetPasswordRequested()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult SetPasswordSuccess()
        {
            ViewBag.ReturnUrl = TempData["ReturnUrl"];

            if (string.IsNullOrWhiteSpace(ViewBag.ReturnUrl))
            {
                ViewBag.ReturnUrl = Url.Action(MvcConstants.INDEX, "Home", new { area = string.Empty });
            }

            return View();
        }

        #endregion Password Reset
    }
}