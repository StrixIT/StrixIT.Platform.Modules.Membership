#region Apache License

//-----------------------------------------------------------------------
// <copyright file="LogOnTests.cs" company="StrixIT">
// Copyright 2015 StrixIT, author R.G. Schurgers MA MSc.
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using StoryQ;
using StrixIT.Platform.Core;
using StrixIT.Platform.Core.Environment;
using StrixIT.Platform.Framework;
using StrixIT.Platform.Testing;
using System;
using System.Linq;
using System.Web.Security;

namespace StrixIT.Platform.Modules.Membership.Specs
{
    [TestClass]
    public class LogOnTests
    {
        #region Private Fields

        private string _email;
        private string _password;

        #endregion Private Fields

        #region Public Methods

        [TestMethod]
        public void SecureUserLogon()
        {
            new Story("Secure user logon")
                .InOrderTo("have a secure account")
                .AsA("user")
                .IWant("to log on using private credentials")

                        .WithScenario("User supplies valid credentials")
                            .Given(TheUserHasValidCredentialsAndAValidAccount)
                            .When(ValidatingTheCredentialsSupplied)
                            .Then(TheUserRecievesAnAuthenticationCookie)
                                .And(TheUserIsRedirectedToTheHomePageForHisCulture)
                                .And(TheUsersNameIsShown)

                        .WithScenario("User supplies invalid credentials")
                            .Given(TheUserHasSuppliedInvalidCredentials)
                            .When(ValidatingTheCredentialsSupplied)
                            .Then(TheUserRecievesAnInvalidCredentialsErrorMessageOnTheSamePage)

                        .WithScenario("User has no roles assigned")
                            .Given(TheUserHasSuppliedValidCredentialsButHasNoRoles)
                            .When(ValidatingTheCredentialsSupplied)
                            .Then(TheUserRecievesANoRolesErrorMessageOnTheSamePage)

                        .WithScenario("user is not yet approved")
                            .Given(TheUserHasSuppliedValidCredentialsButIsNotYetApproved)
                            .When(ValidatingTheCredentialsSupplied)
                            .Then(TheUserRecievesANotApprovedErrorMessageOnTheSamePage)

                        .WithScenario("User is locked out")
                            .Given(TheUserHasSuppliedValidCredentialsButIsLockedOut)
                            .When(ValidatingTheCredentialsSupplied)
                            .Then(TheUserRecievesALockedOutErrorMessageOnTheSamePage)

                .Execute();
        }

        #endregion Public Methods

        #region Private Methods

        private void CreateUser(string email, string name, string password, bool isApproved, bool isLockedOut, bool createRoles)
        {
            DependencyInjector.Injector = new StructureMapDependencyInjector();
            var dataSource = DependencyInjector.Get<IMembershipDataSource>();
            var securityService = DependencyInjector.Get<ISecurityManager>();
            var cultureService = DependencyInjector.Get<ICultureService>();
            var mainGroupId = dataSource.Query<Group>().Where(g => g.Name.ToLower() == Resources.DefaultValues.MainGroupName.ToLower()).Select(a => a.Id).FirstOrDefault();

            var user = new User(Guid.NewGuid(), email, name);
            user.PreferredCulture = cultureService.DefaultCultureCode;
            var userSecurity = new UserSecurity(user.Id);
            userSecurity.Password = securityService.EncodePassword(password);
            userSecurity.Approved = isApproved;
            userSecurity.LockedOut = isLockedOut;
            dataSource.Save(userSecurity);
            var userSession = new UserSessionStorage(user.Id);
            dataSource.Save(userSession);
            dataSource.Save(user);

            if (createRoles)
            {
                var adminRole = dataSource.Query<GroupInRole>().First(g => g.Role.Name == PlatformConstants.ADMINROLE);
                var userInRole = new UserInRole(adminRole, user.Id);
                dataSource.Save(userInRole);
            }

            dataSource.SaveChanges();
        }

        private string GetExpectedMessage(string errorMessage)
        {
            string message = errorMessage;
            string summary = Resources.Interface.LoginError;
            return string.Format("{0}\r\n{1}", summary, message);
        }

        private void TheUserHasSuppliedInvalidCredentials()
        {
            this._email = Resources.DefaultValues.AdministratorEmail;
            this._password = "Test invalid";
        }

        private void TheUserHasSuppliedValidCredentialsButHasNoRoles()
        {
            this.CreateUser("noroles@strixit.com", "No Roles", "no_roles", true, false, false);
            this._email = "noroles@strixit.com";
            this._password = "no_roles";
        }

        private void TheUserHasSuppliedValidCredentialsButIsLockedOut()
        {
            this.CreateUser("lockedout@strixit.com", "Locked out", "lockedout", true, true, true);
            this._email = "lockedout@strixit.com";
            this._password = "lockedout";
        }

        private void TheUserHasSuppliedValidCredentialsButIsNotYetApproved()
        {
            this.CreateUser("notapproved@strixit.com", "Not Approved", "not_approved", false, false, true);
            this._email = "notapproved@strixit.com";
            this._password = "not_approved";
        }

        private void TheUserHasValidCredentialsAndAValidAccount()
        {
            this._email = Resources.DefaultValues.AdministratorEmail;
            this._password = Resources.DefaultValues.AdministratorPassword;
        }

        private void TheUserIsRedirectedToTheHomePageForHisCulture()
        {
            TestManager.Browser.IsAt("/");
        }

        private void TheUserRecievesALockedOutErrorMessageOnTheSamePage()
        {
            TestManager.Browser.IsAt("/account/login");
            var expected = this.GetExpectedMessage(Resources.Interface.LockedOut);
            Assert.AreEqual(expected, TestManager.Browser.FindElementById("loginerrorcontainer").Text);
        }

        private void TheUserRecievesAnAuthenticationCookie()
        {
            var cookie = TestManager.Browser.Manage().Cookies.GetCookieNamed(FormsAuthentication.FormsCookieName);
            Assert.IsNotNull(cookie);
        }

        private void TheUserRecievesAnInvalidCredentialsErrorMessageOnTheSamePage()
        {
            TestManager.Browser.IsAt("/account/login");
            var expected = this.GetExpectedMessage(Resources.Interface.InvalidCredentials);
            Assert.AreEqual(expected, TestManager.Browser.FindElementById("loginerrorcontainer").Text);
        }

        private void TheUserRecievesANoRolesErrorMessageOnTheSamePage()
        {
            TestManager.Browser.IsAt("/account/login");
            var expected = this.GetExpectedMessage(Resources.Interface.UserHasNoRoles);
            Assert.AreEqual(expected, TestManager.Browser.FindElementById("loginerrorcontainer").Text);
        }

        private void TheUserRecievesANotApprovedErrorMessageOnTheSamePage()
        {
            TestManager.Browser.IsAt("/account/login");
            var expected = this.GetExpectedMessage(Resources.Interface.Unapproved);
            Assert.AreEqual(expected, TestManager.Browser.FindElementById("loginerrorcontainer").Text);
        }

        private void TheUsersNameIsShown()
        {
            Assert.AreEqual(Resources.DefaultValues.AdministratorName, TestManager.Browser.FindElementById("username").Text);
        }

        private void ValidatingTheCredentialsSupplied()
        {
            TestManager.Browser.LogOn(this._email, this._password);
        }

        #endregion Private Methods
    }
}