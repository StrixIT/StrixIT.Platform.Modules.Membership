#region Apache License
//-----------------------------------------------------------------------
// <copyright file="LoginViewModel.cs" company="StrixIT">
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
#endregion

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