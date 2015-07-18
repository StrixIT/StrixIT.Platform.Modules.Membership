//-----------------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for the login page.
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Resources.Interface), Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "Required")]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.Interface), Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the url to return to after logging in.
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}