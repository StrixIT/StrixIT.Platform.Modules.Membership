#region Apache License

//-----------------------------------------------------------------------
// <copyright file="StrixMembershipException.cs" company="StrixIT">
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

using System;
using System.Runtime.Serialization;

namespace StrixIT.Platform.Modules.Membership
{
    [Serializable]
    public class StrixMembershipException : Exception
    {
        #region Public Constructors

        public StrixMembershipException() : base()
        {
        }

        public StrixMembershipException(string message) : base(message)
        {
        }

        public StrixMembershipException(string message, Exception inner) : base(message, inner)
        {
        }

        #endregion Public Constructors

        #region Protected Constructors

        protected StrixMembershipException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        #endregion Protected Constructors
    }
}