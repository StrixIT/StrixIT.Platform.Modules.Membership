#region Apache License

//-----------------------------------------------------------------------
// <copyright file="Application.cs" company="StrixIT">
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
    /// The application for users and entities. Used to allow multiple applications to use the same
    /// database with or without sharing data.
    /// </summary>
    public class Application : ValidationBase
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="id">The id of the application</param>
        /// <param name="name">The name of the application</param>
        public Application(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        #endregion Public Constructors

        #region Private Constructors

        private Application()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        /// Gets the application id.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        [StrixRequired]
        [StringLength(250)]
        public string Name { get; private set; }

        #endregion Public Properties
    }
}