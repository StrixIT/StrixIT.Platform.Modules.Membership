#region Apache License

//-----------------------------------------------------------------------
// <copyright file="IMembershipDataSource.cs" company="StrixIT">
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
using System.Linq;

namespace StrixIT.Platform.Modules.Membership
{
    /// <summary>
    /// The membership data source interface.
    /// </summary>
    public interface IMembershipDataSource : IDataSource
    {
        #region Public Methods

        /// <summary>
        /// Finds an object in memory or the data source using its key values.
        /// </summary>
        /// <typeparam name="T">The type of the object to find</typeparam>
        /// <param name="keys">The object's key values</param>
        /// <returns>The object</returns>
        T Find<T>(object[] keys) where T : class;

        /// <summary>
        /// Gets a query for the specified object type with the specified includes.
        /// </summary>
        /// <typeparam name="T">The type of the object to get the query for</typeparam>
        /// <param name="includes">The includes</param>
        /// <returns>The query</returns>
        IQueryable<T> Query<T>(string includes) where T : class;

        #endregion Public Methods
    }
}