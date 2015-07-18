//-----------------------------------------------------------------------
// <copyright file="RegisterViewModel.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.ComponentModel.DataAnnotations;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for user self-registration.
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Resources.Interface), Name = "Name")]
        [StringLength(250)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "Required")]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(Resources.Interface), Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user registration comment.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Interface), Name = "RegistrationComment")]
        public string RegistrationComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user accepted the terms of use.
        /// </summary>
         [Display(ResourceType = typeof(Resources.Interface), Name = "AcceptedTerms")]
        public bool AcceptedTerms { get; set; }
    }
}