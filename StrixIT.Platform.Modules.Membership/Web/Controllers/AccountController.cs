//-----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="StrixIT">
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
    [StrixAuthorization]
    public class AccountController : BaseController
    {
        private IAuthenticationService _authenticationService;
        private IAccountService _accountService;

        public AccountController(IAuthenticationService authenticationService, IAccountService accountService) : base()
        {
            this._authenticationService = authenticationService;
            this._accountService = accountService;
        }

        #region Login/Logout

        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            model.ReturnUrl = Request.Params["returnUrl"];
            return this.View(model);
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
                var result = this._authenticationService.LogOn(model.Email, model.Password);

                if (result.Success)
                {
                    var returnUrl = Web.Helpers.HtmlDecode(model.ReturnUrl, false);

                    if (this.Url != null && Url.IsLocalUrl(returnUrl))
                    {
                        return this.Redirect(returnUrl);
                    }
                    else
                    {
                        var redirectParams = StrixPlatform.Cultures.Count() > 1 ? new { area = string.Empty, language = result.PreferredCulture } : new { area = string.Empty, language = string.Empty };
                        return this.RedirectToAction(MvcConstants.INDEX, "Home", redirectParams);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Resources.Interface.LoginFailedContainer);
                    ViewBag.LoginError = result.Message;
                }
            }

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        public ActionResult LogOff()
        {
            this._authenticationService.LogOff(StrixPlatform.Environment.CurrentUserEmail);
            var returnCulture = StrixPlatform.CurrentCultureCode == StrixPlatform.DefaultCultureCode ? null : StrixPlatform.CurrentCultureCode;
            return this.RedirectToAction(MvcConstants.INDEX, MvcConstants.HOME, new { area = string.Empty, culture = returnCulture });
        }

        #endregion

        #region Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            if (!StrixMembership.Configuration.Registration.AllowUserRegistration)
            {
                return null;
            }

            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
            if (model == null)
            {
                return null;
            }

            if (!StrixMembership.Configuration.Registration.AllowUserRegistration)
            {
                return null;
            }

            if (ModelState.IsValid)
            {
                var result = this._accountService.RegisterAccount(model);

                if (result.Success && result.Message == null)
                {
                    return this.RedirectToAction("RegisterSuccess");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            return this.View(model);
        }

        [AllowAnonymous]
        public ActionResult RegisterSuccess()
        {
            if (!StrixMembership.Configuration.Registration.AllowUserRegistration)
            {
                return null;
            }

            return this.View();
        }

        #endregion

        #region Password Reset

        [AllowAnonymous]
        public ActionResult SendPasswordLink(Guid userId)
        {
            this._accountService.SendPasswordResetLink(userId);
            this.TempData["NewAccount"] = true;
            return this.RedirectToAction("SetPasswordRequested");
        }

        [AllowAnonymous]
        public ActionResult RequestSetPassword()
        {
            return this.View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RequestSetPassword(string email)
        {
            // Todo: why this length? Use proper regex
            if (string.IsNullOrWhiteSpace(email) || email.Length < 8)
            {
                ModelState.AddModelError("Email", Resources.Interface.InvalidEmail);
                return this.View();
            }

            this._accountService.SendPasswordResetLink(email);
            return this.RedirectToAction("SetPasswordRequested");
        }

        [AllowAnonymous]
        public ActionResult SetPasswordRequested()
        {
            return this.View();
        }

        [AllowAnonymous]
        public ActionResult SetPassword(Guid? key)
        {
            SetPasswordViewModel model = new SetPasswordViewModel();

            if (key.HasValue)
            {
                var user = this._accountService.GetUserByResetKey(key.Value);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, Resources.Interface.InvalidPasswordSetKey);
                }

                model.PasswordResetKey = key.Value;
            }

            ViewBag.MinRequiredNonalphanumericCharacters = StrixMembership.Configuration.Password.MinRequiredNonAlphanumericCharacters;
            ViewBag.PasswordLength = StrixMembership.Configuration.Password.MinRequiredPasswordLength;
            model.ReturnUrl = this.Request != null && this.Request.UrlReferrer != null ? this.Request.UrlReferrer.ToString() : null;
            return this.View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SetPassword(SetPasswordViewModel model)
        {
            if (model == null)
            {
                return null;
            }

            if (!this.ControllerContext.HttpContext.Request.IsAuthenticated && model.PasswordResetKey == Guid.Empty)
            {
                return null;
            }

            if (ModelState.IsValid)
            {
                SaveResult<UserViewModel> result;
                var email = StrixPlatform.Environment.CurrentUserEmail;

                if (!string.IsNullOrWhiteSpace(email) && model.OldPassword != null && model.OldPassword.ToString() != model.PasswordResetKey.ToString())
                {
                    result = this._accountService.ChangePassword(email, model.OldPassword, model.NewPassword);
                }
                else
                {
                    var user = this._accountService.GetUserByResetKey(model.PasswordResetKey);
                    result = this._accountService.ChangePassword(user.Email, null, model.NewPassword, model.PasswordResetKey);
                }

                if (result.Success && result.Message == null)
                {
                    this.TempData["ReturnUrl"] = model.ReturnUrl;
                    this.TempData["PasswordSetMode"] = model.PasswordResetKey == Guid.Empty ? "Change" : "Register";
                    return this.RedirectToAction("SetPasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
            }

            ViewBag.MinRequiredNonalphanumericCharacters = StrixMembership.Configuration.Password.MinRequiredNonAlphanumericCharacters;
            ViewBag.PasswordLength = StrixMembership.Configuration.Password.MinRequiredPasswordLength;
            return this.View(model);
        }

        [AllowAnonymous]
        public ActionResult SetPasswordSuccess()
        {
            ViewBag.ReturnUrl = this.TempData["ReturnUrl"];

            if (string.IsNullOrWhiteSpace(ViewBag.ReturnUrl))
            {
                ViewBag.ReturnUrl = Url.Action(MvcConstants.INDEX, "Home", new { area = string.Empty });
            }

            return this.View();
        }

        #endregion
    }
}