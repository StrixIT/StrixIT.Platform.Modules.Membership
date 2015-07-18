//-----------------------------------------------------------------------
// <copyright file="UserViewModel.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for creating and editing users.
    /// </summary>
    public class UserViewModel : BaseCrudDto
    {
        public UserViewModel() : base(typeof(User)) 
        {
            this.CanEdit = StrixPlatform.User.HasPermission(MembershipPermissions.EditUser);
            this.CanDelete = StrixPlatform.User.HasPermission(MembershipPermissions.DeleteUser);
        }

        /// <summary>
        /// Gets or sets the id of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user account is locked.
        /// </summary>
        public bool LockedOut { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user account is approved.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// Gets a value indicating whether the user has accepted the terms.
        /// </summary>
        public bool HasAcceptedTerms
        {
            get
            {
                return this.DateAcceptedTerms.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the date at which the user has accepted the terms.
        /// </summary>
        public DateTime? DateAcceptedTerms { get; set; }

        /// <summary>
        /// Gets or sets the user's preferred culture.
        /// </summary>
        public string PreferredCulture { get; set; }

        /// <summary>
        /// Gets or sets all the roles available.
        /// </summary>
        public IList<AssignRoleModel> Roles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is a user manager. Used to hide the start and end date, as well as
        /// the delete button for these users when edited by other user managers.
        /// </summary>
        public bool IsCompanyManager { get; set; }

        /// <summary>
        /// Gets or sets the user password. Used to validate account updates.
        /// </summary>
        public string Password { get; set; }
    }
}