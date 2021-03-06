﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was not generated by a tool. but for stylecop suppression.
// </auto-generated>
//------------------------------------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StrixIT.Platform.Core;
using System;
using System.Linq;

namespace StrixIT.Platform.Modules.Membership.Tests
{
    [TestClass]
    public class SecurityManagerTests
    {
        #region Public Methods

        [TestMethod()]
        public void CheckVerificationIdShouldReturnFalseForValidKeyOutsideWindow()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            var key = Guid.NewGuid();
            security.VerificationId = key;
            security.VerificationWindowStart = DateTime.Now.AddMinutes(-200);
            var result = mock.SecurityManager.CheckVerificationId(key);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void CheckVerificationIdShouldReturnTrueForValidKeyWithinWindow()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            var key = Guid.NewGuid();
            security.VerificationId = key;
            security.VerificationWindowStart = DateTime.Now.AddMinutes(-10);
            var result = mock.SecurityManager.CheckVerificationId(key);
            Assert.IsTrue(result);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Logger.LoggingService = null;
        }

        [TestMethod()]
        public void GetUserByResetKeyWithValidKeyShouldGetUser()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            var key = Guid.NewGuid();
            security.VerificationId = key;
            security.VerificationWindowStart = DateTime.Now.AddMinutes(-10);
            security.SetPropertyValue("User", MembershipTestData.Administrator);
            var result = mock.SecurityManager.GetUserByResetKey(key);
            Assert.IsNotNull(result);
        }

        [TestInitialize]
        public void Init()
        {
            Logger.LoggingService = new Mock<ILoggingService>().Object;
        }

        #endregion Public Methods

        #region Settings

        [TestMethod()]
        public void MinRequiredNonalphanumericCharactersShouldReturnConfiguredValue()
        {
            var mock = new SecurityManagerMock();
            var result = StrixMembership.Configuration.Password.MinRequiredNonAlphanumericCharacters;
            Assert.AreEqual(1, result);
        }

        [TestMethod()]
        public void MinRequiredPasswordLengthShouldReturnConfiguredValue()
        {
            var mock = new SecurityManagerMock();
            var result = StrixMembership.Configuration.Password.MinRequiredPasswordLength;
            Assert.AreEqual(8, result);
        }

        #endregion Settings

        #region Approve and Unlock User

        [TestMethod()]
        public void ApproveUserWithValidIdShouldApproveUser()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.DivingManagerId);
            security.Approved = false;
            var result = mock.SecurityManager.ApproveUser(MembershipTestData.DivingManagerId);
            Assert.AreEqual(true, security.Approved);
        }

        [TestMethod()]
        public void UnlockUserWithInvalidIdShouldReturnFalse()
        {
            var mock = new SecurityManagerMock();
            var result = mock.SecurityManager.UnlockUser(Guid.NewGuid());
            Assert.AreEqual(false, result);
        }

        [TestMethod()]
        public void UnlockUserWithValidIdShouldUnlockUserAndResetFailedAttemptsAndWindow()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.DivingManagerId);
            var result = mock.SecurityManager.UnlockUser(MembershipTestData.DivingManagerId);
            Assert.AreEqual(false, security.LockedOut);
            Assert.AreEqual(0, security.FailedPasswordAttemptCount);
            Assert.AreEqual(null, security.FailedPasswordAttemptWindowStart);
            Assert.AreEqual(true, result);
        }

        #endregion Approve and Unlock User

        #region Validate User

        [TestMethod()]
        public void ValidateLockedOutUserWithValidDetailsShouldReturnLockedOutStatus()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            security.LockedOut = true;
            var result = mock.SecurityManager.ValidateUser(MembershipTestData.AdminId, "testtest!");
            Assert.AreEqual(ValidateUserResult.LockedOut, result);
        }

        [TestMethod()]
        public void ValidateUnapprovedUserWithValidDetailsShouldReturnUnapprovedStatus()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            security.Approved = false;
            var result = mock.SecurityManager.ValidateUser(MembershipTestData.AdminId, "testtest!");
            Assert.AreEqual(ValidateUserResult.Unapproved, result);
        }

        [TestMethod()]
        public void ValidateUserWithInvalidDetailsShouldUpdateFailedFieldsAndReturnInvalidStatus()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            var result = mock.SecurityManager.ValidateUser(MembershipTestData.AdminId, "test");
            Assert.AreEqual(1, security.FailedPasswordAttemptCount);
            Assert.IsNotNull(security.FailedPasswordAttemptWindowStart);
            Assert.AreEqual(ValidateUserResult.Invalid, result);
        }

        [TestMethod()]
        public void ValidateUserWithInvalidIdShouldReturnErrorStatus()
        {
            var mock = new SecurityManagerMock();
            var result = mock.SecurityManager.ValidateUser(Guid.Empty, "test");
            Assert.AreEqual(ValidateUserResult.Error, result);
        }

        [TestMethod()]
        public void ValidateUserWithNoRolesShouldReturnNoRolesStatus()
        {
            var mock = new SecurityManagerMock();
            var user = mock.DataSourceMock.DataList<User>().First(u => u.Id == MembershipTestData.DivingEditorId);
            user.Roles.Clear();
            var result = mock.SecurityManager.ValidateUser(MembershipTestData.DivingEditorId, "testtest!");
            Assert.AreEqual(ValidateUserResult.NoRoles, result);
        }

        [TestMethod()]
        public void ValidateUserWithValidDetailsShouldClearFailedFieldsAndReturnValidStatus()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            security.LockedOut = false;
            security.FailedPasswordAttemptCount = 4;
            security.FailedPasswordAttemptWindowStart = DateTime.Now.AddMinutes(-2);
            var result = mock.SecurityManager.ValidateUser(MembershipTestData.AdminId, "testtest!");
            Assert.AreEqual(0, security.FailedPasswordAttemptCount);
            Assert.IsNull(security.FailedPasswordAttemptWindowStart);
            Assert.AreEqual(ValidateUserResult.Valid, result);
        }

        [TestMethod()]
        public void ValidateUserWithValidDetailsShouldReturnValidStatus()
        {
            var mock = new SecurityManagerMock();
            var result = mock.SecurityManager.ValidateUser(MembershipTestData.AdminId, "testtest!");
            Assert.AreEqual(ValidateUserResult.Valid, result);
        }

        #endregion Validate User

        #region Change Password

        [TestMethod()]
        public void ChangePasswordWithInvalidResetKeyInResetWindowShouldReturnFalse()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            security.VerificationWindowStart = DateTime.Now;
            var result = mock.SecurityManager.ChangePassword(MembershipTestData.AdminId, null, "testtest!2", Guid.NewGuid());
            Assert.AreEqual(false, result);
        }

        [TestMethod()]
        public void ChangePasswordWithOldPasswordWhenOldPasswordIsInvalidShouldReturnFalse()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            var result = mock.SecurityManager.ChangePassword(MembershipTestData.AdminId, "test", "testtest!2");
            Assert.AreEqual(false, result);
        }

        [TestMethod()]
        public void ChangePasswordWithOldPasswordWhenOldPasswordIsValidShouldReturnTrueAndChangeSaltAndPassword()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            var oldPassword = security.Password;
            var result = mock.SecurityManager.ChangePassword(MembershipTestData.AdminId, "testtest!", "testtest!2");
            Assert.AreEqual(true, result);
            Assert.AreNotEqual(oldPassword, security.Password);
        }

        [TestMethod()]
        public void ChangePasswordWithoutResetKeyShouldReturnFalse()
        {
            var mock = new SecurityManagerMock();
            var result = mock.SecurityManager.ChangePassword(MembershipTestData.AdminId, null, "testtest!2", null);
            Assert.AreEqual(false, result);
        }

        [TestMethod()]
        public void ChangePasswordWithValidResetKeyInResetWindowShouldReturnTrue()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            var verificationId = Guid.NewGuid();
            security.VerificationId = verificationId;
            security.VerificationWindowStart = DateTime.Now;
            var result = mock.SecurityManager.ChangePassword(MembershipTestData.AdminId, null, "testtest!2", verificationId);
            Assert.AreEqual(true, result);
        }

        #endregion Change Password

        #region Generate Password

        [TestMethod()]
        public void GeneratePasswordShouldReturnASufficientlyStrongPassword()
        {
            var mock = new SecurityManagerMock();
            string password = mock.SecurityManager.GeneratePassword();
            bool result = !string.IsNullOrWhiteSpace(password) && password.Length >= 8 && System.Text.RegularExpressions.Regex.IsMatch(password, @"[\W]");
            Assert.IsTrue(result);
        }

        #endregion Generate Password

        #region Set Verification Id

        [TestMethod()]
        public void SetVerificationIdWithNullShouldClearVerificationIdAndVerificationWindow()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            mock.SecurityManager.SetVerificationId(MembershipTestData.AdminId, null);
            Assert.IsNull(security.VerificationId);
            Assert.IsNull(security.VerificationWindowStart);
        }

        [TestMethod()]
        public void SetVerificationIdWithValueShouldSetVerificationIdAndVerificationWindow()
        {
            var mock = new SecurityManagerMock();
            var security = mock.DataSourceMock.DataList<UserSecurity>().First(s => s.Id == MembershipTestData.AdminId);
            var verificationId = Guid.NewGuid();
            mock.SecurityManager.SetVerificationId(MembershipTestData.AdminId, verificationId);
            Assert.AreEqual(verificationId, security.VerificationId);
            Assert.IsNotNull(security.VerificationWindowStart);
        }

        #endregion Set Verification Id
    }
}