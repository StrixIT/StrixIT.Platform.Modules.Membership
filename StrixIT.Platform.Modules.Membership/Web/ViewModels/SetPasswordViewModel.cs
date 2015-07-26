#region Apache License

//-----------------------------------------------------------------------
// <copyright file="SetPasswordViewModel.cs" company="StrixIT">
// Copyright 2015 StrixIT. Author R.G. Schurgers MA MSc.
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

using StrixIT.Platform.Modules.Membership.Resources;
using System;
using System.ComponentModel.DataAnnotations;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for changing passwords.
    /// </summary>
    public class SetPasswordViewModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the confirmation of the new password.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Interface), Name = "ConfirmNewPassword")]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "PasswordAndConfirmationDoNotMatch")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the new password.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "Required")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "TooShort", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Interface), Name = "NewPassword")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Gets or sets the old password.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Interface), Name = "CurrentPassword")]
        public string OldPassword { get; set; }

        /// <summary>
        /// Gets or sets the password reset key. Used for password resets.
        /// </summary>
        public Guid PasswordResetKey { get; set; }

        /// <summary>
        /// Gets or sets the url to return to after changing the password.
        /// </summary>
        public string ReturnUrl { get; set; }

        #endregion Public Properties
    }
}