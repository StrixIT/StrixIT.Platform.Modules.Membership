#region Apache License

//-----------------------------------------------------------------------
// <copyright file="UserProfileValue.cs" company="StrixIT">
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

using StrixIT.Platform.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// Defines a user profile field value.
    /// </summary>
    public class UserProfileValue : CustomFieldValue<UserProfileField>
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the culture code the profile value is for.
        /// </summary>
        [StrixRequired]
        [StringLength(5)]
        public string Culture { get; set; }

        /// <summary>
        /// Gets or sets the user the profile value is for.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the id of the user the profile value is for.
        /// </summary>
        [StrixRequired]
        public Guid UserId { get; set; }

        #endregion Public Properties
    }
}