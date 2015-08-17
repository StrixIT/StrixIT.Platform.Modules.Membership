﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was not generated by a tool. but for stylecop suppression.
// </auto-generated>
//------------------------------------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrixIT.Platform.Core;
using StrixIT.Platform.Core.DependencyInjection;
using StrixIT.Platform.Core.Environment;
using StrixIT.Platform.Web;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StrixIT.Platform.Modules.Membership.Tests
{
    [TestClass]
    public class AuthorizationAttributeTests
    {
        #region Public Methods

        [TestMethod]
        public void SkipAuthorizationShouldNotSkipWhenNotAnonymousAllowed()
        {
            var attribute = new StrixAuthorizationAttribute();
            List<Mock> mocks;
            var context = GetAuthorizationContext(out mocks);
            var request = mocks.First(m => m.GetType() == typeof(Mock<HttpRequestBase>)) as Mock<HttpRequestBase>;
            request.Setup(r => r.Headers).Returns(new NameValueCollection());
            attribute.OnAuthorization(context);
            Assert.AreEqual(typeof(HttpUnauthorizedResult), context.Result.GetType());
        }

        [TestMethod]
        public void SkipAuthorizationShouldSkipWhenAnonymousAllowedOnAction()
        {
            var attribute = new StrixAuthorizationAttribute();
            List<Mock> mocks;
            var context = GetAuthorizationContext(out mocks);
            var request = mocks.First(m => m.GetType() == typeof(Mock<HttpRequestBase>)) as Mock<HttpRequestBase>;
            request.Setup(r => r.Headers).Returns(new NameValueCollection());
            var actionDescriptor = mocks.First(m => m.GetType() == typeof(Mock<ActionDescriptor>)) as Mock<ActionDescriptor>;
            actionDescriptor.Setup(a => a.GetCustomAttributes(typeof(AllowAnonymousAttribute), It.IsAny<bool>())).Returns(new object[] { new AllowAnonymousAttribute() });
            attribute.OnAuthorization(context);
            var cache = mocks.First(m => m.GetType() == typeof(Mock<HttpCachePolicyBase>)) as Mock<HttpCachePolicyBase>;
            cache.Verify(c => c.SetProxyMaxAge(It.IsAny<TimeSpan>()), Times.Once());
        }

        [TestMethod]
        public void SkipAuthorizationShouldSkipWhenAnonymousAllowedOnController()
        {
            var attribute = new StrixAuthorizationAttribute();
            List<Mock> mocks;
            var context = GetAuthorizationContext(out mocks);
            var request = mocks.First(m => m.GetType() == typeof(Mock<HttpRequestBase>)) as Mock<HttpRequestBase>;
            request.Setup(r => r.Headers).Returns(new NameValueCollection());
            var controllerDescriptor = mocks.First(m => m.GetType() == typeof(Mock<ControllerDescriptor>)) as Mock<ControllerDescriptor>;
            controllerDescriptor.Setup(a => a.GetCustomAttributes(typeof(AllowAnonymousAttribute), It.IsAny<bool>())).Returns(new object[] { new AllowAnonymousAttribute() });
            attribute.OnAuthorization(context);
            var cache = mocks.First(m => m.GetType() == typeof(Mock<HttpCachePolicyBase>)) as Mock<HttpCachePolicyBase>;
            cache.Verify(c => c.SetProxyMaxAge(It.IsAny<TimeSpan>()), Times.Once());
        }

        [TestMethod]
        public void UnauthorizedAjaxRequestShouldSetStatusCodeTo401AndEndResponse()
        {
            var attribute = new StrixAuthorizationAttribute();
            List<Mock> mocks;
            var context = GetAuthorizationContext(out mocks);
            attribute.OnAuthorization(context);
            var result = context.Result as HttpStatusCodeResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public void UserWithoutRequiredPermissionShouldNotBeAuthorized()
        {
            var attribute = new StrixAuthorizationAttribute { Permissions = "View users" };
            List<Mock> mocks;
            var context = GetAuthorizationContext(out mocks);
            var identity = mocks.First(m => m.GetType() == typeof(Mock<IIdentity>)) as Mock<IIdentity>;
            identity.Setup(i => i.Name).Returns("Administrator");
            var userMock = mocks.First(m => m.GetType() == typeof(Mock<IUserContext>)) as Mock<IUserContext>;
            userMock.Setup(m => m.HasPermission(new string[] { "View users" })).Returns(false);
            attribute.OnAuthorization(context);
            var result = context.Result as HttpStatusCodeResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public void UserWithoutRequiredRoleShouldNotBeAuthorized()
        {
            var attribute = new StrixAuthorizationAttribute { Roles = "Administrator" };
            List<Mock> mocks;
            var context = GetAuthorizationContext(out mocks);
            var identity = mocks.First(m => m.GetType() == typeof(Mock<IIdentity>)) as Mock<IIdentity>;
            identity.Setup(i => i.Name).Returns("Editor");
            var userMock = mocks.First(m => m.GetType() == typeof(Mock<IUserContext>)) as Mock<IUserContext>;
            userMock.Setup(m => m.IsInRoles(new string[] { "Administrator" })).Returns(false);
            attribute.OnAuthorization(context);
            var result = context.Result as HttpStatusCodeResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
        }

        [TestMethod]
        public void UserWithRequiredPermissionShouldBeAuthorized()
        {
            var attribute = new StrixAuthorizationAttribute { Permissions = "View users" };
            List<Mock> mocks;
            var context = GetAuthorizationContext(out mocks);
            var identity = mocks.First(m => m.GetType() == typeof(Mock<IIdentity>)) as Mock<IIdentity>;
            identity.Setup(i => i.Name).Returns("Administrator");
            var userMock = mocks.First(m => m.GetType() == typeof(Mock<IUserContext>)) as Mock<IUserContext>;
            userMock.Setup(m => m.HasPermission(new string[] { "View users" })).Returns(true);
            attribute.OnAuthorization(context);
            var cache = mocks.First(m => m.GetType() == typeof(Mock<HttpCachePolicyBase>)) as Mock<HttpCachePolicyBase>;
            cache.Verify(c => c.SetProxyMaxAge(It.IsAny<TimeSpan>()), Times.Once());
        }

        [TestMethod]
        public void UserWithRequiredRoleShouldBeAuthorized()
        {
            var attribute = new StrixAuthorizationAttribute { Roles = "Administrator" };
            List<Mock> mocks;
            var context = GetAuthorizationContext(out mocks);
            var identity = mocks.First(m => m.GetType() == typeof(Mock<IIdentity>)) as Mock<IIdentity>;
            identity.Setup(i => i.Name).Returns("Administrator");
            var userMock = mocks.First(m => m.GetType() == typeof(Mock<IUserContext>)) as Mock<IUserContext>;
            userMock.Setup(m => m.IsInRoles(new string[] { "Administrator" })).Returns(true);
            attribute.OnAuthorization(context);
            var cache = mocks.First(m => m.GetType() == typeof(Mock<HttpCachePolicyBase>)) as Mock<HttpCachePolicyBase>;
            cache.Verify(c => c.SetProxyMaxAge(It.IsAny<TimeSpan>()), Times.Once());
        }

        #endregion Public Methods

        #region Private Methods

        private AuthorizationContext GetAuthorizationContext(out List<Mock> mocks)
        {
            mocks = new List<Mock>();
            var identity = new Mock<IIdentity>();
            mocks.Add(identity);
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.Identity).Returns(identity.Object);
            mocks.Add(principal);
            var headers = new NameValueCollection();
            headers.Add("X-Requested-With", "XMLHttpRequest");
            var request = new Mock<HttpRequestBase>();
            request.Setup(r => r.Headers).Returns(headers);
            mocks.Add(request);
            var cache = new Mock<HttpCachePolicyBase>();
            mocks.Add(cache);
            var response = new Mock<HttpResponseBase>();
            response.Setup(r => r.Cache).Returns(cache.Object);
            mocks.Add(response);
            var httpContext = new Mock<HttpContextBase>();
            httpContext.Setup(h => h.Items).Returns(new Dictionary<string, string>());
            httpContext.Setup(h => h.User).Returns(principal.Object);
            httpContext.Setup(h => h.Request).Returns(request.Object);
            httpContext.Setup(h => h.Response).Returns(response.Object);
            mocks.Add(httpContext);
            var routeData = new RouteData();
            var requestContext = new RequestContext(httpContext.Object, routeData);

            var cultureServiceMock = new Mock<ICultureService>();
            cultureServiceMock.Setup(c => c.Cultures).Returns(new List<CultureData> { new CultureData { Code = "en", Name = "English" }, new CultureData { Code = "nl", Name = "Nederlands" } });
            cultureServiceMock.Setup(c => c.DefaultCultureCode).Returns("en");
            cultureServiceMock.Setup(c => c.CurrentCultureCode).Returns("en");
            var environmentMock = new Mock<IEnvironment>();
            environmentMock.Setup(e => e.Cultures).Returns(cultureServiceMock.Object);

            var controller = new UserController(environmentMock.Object, new Mock<IUserService>().Object);
            var controllerContext = new ControllerContext(requestContext, controller);
            var controllerDescriptor = new Mock<ControllerDescriptor>();
            mocks.Add(controllerDescriptor);
            var actionDescriptor = new Mock<ActionDescriptor>();
            actionDescriptor.Setup(a => a.ControllerDescriptor).Returns(controllerDescriptor.Object);
            mocks.Add(actionDescriptor);
            var context = new AuthorizationContext(controllerContext, actionDescriptor.Object);
            var user = new Mock<IUserContext>();
            user.Setup(m => m.Id).Returns(MembershipTestData.AdminId);
            user.Setup(m => m.GroupId).Returns(MembershipTestData.MainGroupId);
            mocks.Add(user);

            var injectorMock = new Mock<IDependencyInjector>();
            injectorMock.Setup(m => m.TryGet<IUserContext>()).Returns(user.Object);
            DependencyInjector.Injector = injectorMock.Object;

            return context;
        }

        #endregion Private Methods
    }
}