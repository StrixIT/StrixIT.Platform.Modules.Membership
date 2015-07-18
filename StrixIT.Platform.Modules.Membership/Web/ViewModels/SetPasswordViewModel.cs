//-----------------------------------------------------------------------
// <copyright file="SetPasswordViewModel.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel.DataAnnotations;
using StrixIT.Platform.Modules.Membership.Resources;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for changing passwords.
    /// </summary>
    public class SetPasswordViewModel
    {
        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Interface), Name = "CurrentPassword")]
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "TooShort", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Interface), Name = "NewPassword")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the confirmation of the new password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Interface), Name = "ConfirmNewPassword")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "PasswordAndConfirmationDoNotMatch")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the password reset key. Used for password resets.
        /// </summary>
        public Guid PasswordResetKey { get; set; }

        /// <summary>
        /// Gets or sets the url to return to after changing the password.
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}