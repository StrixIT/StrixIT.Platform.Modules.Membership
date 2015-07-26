#region Apache License

//-----------------------------------------------------------------------
// <copyright file="RegisterViewModel.cs" company="StrixIT">
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

using System.ComponentModel.DataAnnotations;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// A view model for user self-registration.
    /// </summary>
    public class RegisterViewModel
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the user accepted the terms of use.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Interface), Name = "AcceptedTerms")]
        public bool AcceptedTerms { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "Required")]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(Resources.Interface), Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(Resources.Interface), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Resources.Interface), Name = "Name")]
        [StringLength(250)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user registration comment.
        /// </summary>
        [Display(ResourceType = typeof(Resources.Interface), Name = "RegistrationComment")]
        public string RegistrationComment { get; set; }

        #endregion Public Properties
    }
}