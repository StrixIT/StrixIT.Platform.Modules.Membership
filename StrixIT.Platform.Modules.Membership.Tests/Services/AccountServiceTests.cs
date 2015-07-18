﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was not generated by a tool. but for stylecop suppression.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership.Tests
{
    [TestClass()]
    public class AccountServiceTests
    {
        private Mock<IUserContext> _userContextMock;

        [TestInitialize]
        public void Init()
        {
            _userContextMock = TestHelpers.MockUserContext();
            StrixPlatform.Environment = new DefaultEnvironment();
            Logger.LoggingService = new Mock<ILoggingService>().Object;
        }

        [TestCleanup]
        public void Cleanup()
        {
            StrixPlatform.Environment = null;
            Logger.LoggingService = null;
        }

        #region Account

        [TestMethod()]
        public void SendPasswordResetLinkWithValidEmailShouldSetVerificationIdAndSendMail()
        {
            var mock = new AccountServiceMock();
            var userId = Guid.NewGuid();
            var verificationId = Guid.NewGuid();
            var user = new User(userId, "Test", "Test");
            user.PreferredCulture = "en";
            mock.UserManagerMock.Setup(u => u.GetId("Test")).Returns(user.Id);
            mock.UserManagerMock.Setup(u => u.Get(user.Id)).Returns(user);
            mock.MailerMock.Setup(m => m.SendSetPasswordMail("en", "Test", "Test", It.IsAny<Guid>())).Returns(true);
            var result = mock.AccountService.SendPasswordResetLink("Test");
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.Message);
            mock.DataSourceMock.Verify(m => m.SaveChanges(), Times.Once());
            mock.SecurityManagerMock.Verify(s => s.SetVerificationId(userId, It.IsAny<Guid>()), Times.Once());
            mock.MailerMock.Verify(m => m.SendSetPasswordMail("en", "Test", "Test", It.IsAny<Guid>()), Times.Once());
        }

        [TestMethod()]
        public void SendPasswordResetLinkWithInvalidEmailShouldReturnFalse()
        {
            var mock = new AccountServiceMock();
            mock.UserManagerMock.Setup(u => u.GetId("Test")).Returns(new Guid());
            var result = mock.AccountService.SendPasswordResetLink("Test");
            Assert.IsFalse(result.Success);
        }

        [TestMethod()]
        public void ChangePasswordWithValidResetKeyAndNoOldPasswordShouldChangePasswordAndSendMail()
        {
            var mock = new AccountServiceMock();
            var userId = Guid.NewGuid();
            var resetKey = Guid.NewGuid();
            var user = new User(userId, "Test", "Test");
            user.PreferredCulture = "en";
            mock.UserManagerMock.Setup(u => u.Get("Test")).Returns(user);
            mock.SecurityManagerMock.Setup(s => s.ChangePassword(userId, null, "New", resetKey)).Returns(true);
            mock.MailerMock.Setup(m => m.SendPasswordSetMail("en", "Test", "Test")).Returns(true);
            var result = mock.AccountService.ChangePassword("Test", null, "New", resetKey);
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.Message);
            mock.DataSourceMock.Verify(m => m.SaveChanges(), Times.Once());
            mock.SecurityManagerMock.Verify(s => s.ChangePassword(userId, null, "New", resetKey), Times.Once());
            mock.MailerMock.Verify(m => m.SendPasswordSetMail("en", "Test", "Test"), Times.Once());
        }

        [TestMethod()]
        public void ChangePasswordWithInvalidResetKeyAndNoOldPasswordShouldReturnFalse()
        {
            var mock = new AccountServiceMock();
            var userId = Guid.NewGuid();
            var resetKey = Guid.NewGuid();
            mock.UserManagerMock.Setup(u => u.Get("Test")).Returns(new User(userId, "Test", "Test"));
            var result = mock.AccountService.ChangePassword("Test", null, "New", Guid.NewGuid());
            Assert.IsFalse(result.Success);
        }

        [TestMethod()]
        public void ChangePasswordWithInvalidEmailShouldReturnFalse()
        {
            var mock = new AccountServiceMock();
            var resetKey = Guid.NewGuid();
            var result = mock.AccountService.ChangePassword("Test", null, "New", resetKey);
            Assert.IsFalse(result.Success);
        }

        [TestMethod()]
        public void RegisterAccountWithAutoApproveOnShouldCreateNewAccountAndSendCorrectMail()
        {
            var mock = new AccountServiceMock();
            var userId = Guid.NewGuid();
            var user = new User(userId, "Test", "Test");
            user.PreferredCulture = "en";
            mock.UserManagerMock.Setup(m => m.Create("Test", "Test", "en", It.IsAny<string>(), false, true, It.IsAny<string>())).Returns(user);
            mock.MailerMock.Setup(m => m.SendApprovedAccountMail("en", "Test", "Test", It.IsAny<Guid>())).Returns(true);
            var result = mock.AccountService.RegisterAccount(new RegisterViewModel { Name = "Test", Email = "Test", AcceptedTerms = true });
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.Message);
            mock.UserManagerMock.Verify(m => m.Create("Test", "Test", "en", It.IsAny<string>(), false, true, It.IsAny<string>()), Times.Once());
            mock.SecurityManagerMock.Verify(s => s.SetVerificationId(userId, It.IsAny<Guid>()), Times.Once());
            mock.MailerMock.Verify(s => s.SendApprovedAccountMail("en", "Test", "Test", It.IsAny<Guid>()), Times.Once());
            mock.RoleManagerMock.Verify(r => r.AddUserToRole(MembershipTestData.MainGroupId, user.Id, PlatformConstants.USERROLE, null, null), Times.Once());
            mock.DataSourceMock.Verify(m => m.SaveChanges(), Times.Exactly(2));
        }

        [TestMethod()]
        public void UpdateAccountByUserHimselfShouldUpdateEmailAndNotSendEmailChangedMail()
        {
            var mock = new AccountServiceMock();
            var userId = Guid.NewGuid();
            _userContextMock.Setup(m => m.Id).Returns(userId);
            mock.UserManagerMock.Setup(m => m.Update(userId, "Test", "New", "en")).Returns(new User(userId, "Test", "New"));
            mock.UserManagerMock.Setup(m => m.GetEmail(userId)).Returns("Test");
            mock.SecurityManagerMock.Setup(s => s.ValidateUser(userId, "Test")).Returns(ValidateUserResult.Valid);
            mock.MailerMock.Setup(m => m.SendEmailChangedMail("en", "Test", "New", "Test")).Returns(true);
            var result = mock.AccountService.UpdateAccount(new UserViewModel { Id = userId, Name = "Test", Email = "New", PreferredCulture = "en", Password = "Test" });
            _userContextMock.Setup(m => m.Id).Returns(Guid.NewGuid());
            Assert.IsTrue(result.Success);
            mock.UserManagerMock.Verify(m => m.Update(userId, "Test", "New", "en"), Times.Once());
            mock.DataSourceMock.Verify(m => m.SaveChanges(), Times.Once());
            mock.MailerMock.Verify(m => m.SendEmailChangedMail("en", "Test", "New", "Test"), Times.Never());
        }

        #endregion
    }
}
