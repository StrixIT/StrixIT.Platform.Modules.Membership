#region Apache License
//-----------------------------------------------------------------------
// <copyright file="MembershipMailer.cs" company="StrixIT">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Web;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    public class MembershipMailer : IMembershipMailer
    {
        private IFileSystemWrapper _fileSystemWrapper;
        private IMailer _mailer;
        private HttpContextBase _httpContext;

        public MembershipMailer(IFileSystemWrapper fileSystemWrapper, IMailer mailer, HttpContextBase httpContext)
        {
            this._fileSystemWrapper = fileSystemWrapper;
            this._mailer = mailer;
            this._httpContext = httpContext;
        }

        #region Send Mails

        public bool SendAccountInformationMail(string culture, string userName, string email, Guid userId)
        {
            var tokens = new Dictionary<string, string>();
            tokens.Add("[[USERNAME]]", userName);
            tokens.Add("[[USERID]]", userId.ToString());
            var result = this.SendMail(culture, "AccountInformationMail", email, tokens);
            return result;
        }

        public bool SendApprovedAccountMail(string culture, string userName, string email, Guid passwordVerificationId)
        {
            var tokens = new Dictionary<string, string>();
            tokens.Add("[[USERNAME]]", userName);
            tokens.Add("[[VERIFICATIONID]]", passwordVerificationId.ToString());
            var result = this.SendMail(culture, "ApprovedAccountMail", email, tokens);
            return result;
        }

        public bool SendUnapprovedAccountMail(string culture, string userName, string email, Guid passwordVerificationId)
        {
            var tokens = new Dictionary<string, string>();
            tokens.Add("[[USERNAME]]", userName);
            tokens.Add("[[VERIFICATIONID]]", passwordVerificationId.ToString());
            var result = this.SendMail(culture, "UnapprovedAccountMail", email, tokens);
            return result;
        }

        public bool SendAccountApprovedMail(string culture, string userName, string email)
        {
            var tokens = new Dictionary<string, string>();
            tokens.Add("[[USERNAME]]", userName);
            var result = this.SendMail(culture, "AccountApprovedMail", email, tokens);
            return result;
        }

        public bool SendEmailChangedMail(string culture, string userName, string newEmail, string oldEmail)
        {
            var tokens = new Dictionary<string, string>();
            tokens.Add("[[USERNAME]]", userName);
            tokens.Add("[[OLDEMAIL]]", oldEmail);
            tokens.Add("[[NEWEMAIL]]", newEmail);
            var result = this.SendMail(culture, "EmailChangedMail", newEmail, tokens);
            return result;
        }

        public bool SendSetPasswordMail(string culture, string userName, string email, Guid passwordVerificationId)
        {
            var tokens = new Dictionary<string, string>();
            tokens.Add("[[USERNAME]]", userName);
            tokens.Add("[[VERIFICATIONID]]", passwordVerificationId.ToString());
            var result = this.SendMail(culture, "SetPasswordMail", email, tokens);
            return result;
        }

        public bool SendPasswordSetMail(string culture, string userName, string email)
        {
            var tokens = new Dictionary<string, string>();
            tokens.Add("[[USERNAME]]", userName);
            var result = this.SendMail(culture, "PasswordSetMail", email, tokens);
            return result;
        }

        #endregion

        #region Private Methods

        private bool SendMail(string culture, string message, string email, Dictionary<string, string> tokens)
        {
            tokens.Add("[[SITENAME]]", StrixPlatform.Configuration.ApplicationName);
            tokens.Add("[[BASEURL]]", this.GetBaseUrl(culture));
            var templateDir = StrixMembership.Configuration.MailTemplateFolder;
            var directory = StrixPlatform.Environment.MapPath(templateDir);
            var template = this._fileSystemWrapper.GetHtmlTemplate(directory, "MailTemplate", culture).FirstOrDefault();
            var mail = this._fileSystemWrapper.GetHtmlTemplate(directory, message, culture).FirstOrDefault();

            if (template == null)
            {
                Logger.Log(string.Format("No template {0} found for culture {1}", message, culture));
            }

            // Raise an event to allow the mail information to be replaced.
            var args = new Dictionary<string, object>();
            args.Add("TemplateName", message);
            args.Add("Culture", culture);
            args.Add("Template", template.Body);
            args.Add("Body", mail.Body);
            args.Add("Subject", mail.Subject);
            args.Add("Tokens", tokens);

            StrixPlatform.RaiseEvent<GeneralEvent>(new GeneralEvent("SendMembershipMailEvent", args));

            template.Body = (string)args["Template"];
            mail.Body = (string)args["Body"];
            mail.Subject = (string)args["Subject"];

            mail.Body = Tokenizer.ReplaceTokens(mail.Body, tokens);
            template.Body = template.Body.Replace("[[CONTENT]]", mail.Body);
            mail.Subject = Tokenizer.ReplaceTokens(mail.Subject, tokens);

            var mailSettings = Helpers.GetConfigSectionGroup<MailSettingsSectionGroup>("system.net/mailSettings");
            string from = mailSettings.Smtp.From;

            return this._mailer.SendMail(from, email, mail.Subject, template.Body);
        }

        private string GetBaseUrl(string cultureCode)
        {
            string url = null;
            var baseUrl = this._httpContext.Request.Url.AbsoluteUri;
            var separator = "://";

            if (baseUrl.Contains(separator))
            {
                var index = baseUrl.IndexOf(separator) + 3;
                url = baseUrl.Substring(index, baseUrl.Length - index);
                url = url.Replace(this._httpContext.Request.Url.PathAndQuery, string.Empty);
                url = baseUrl.Substring(0, index) + url;
            }

            var cultureAddition = cultureCode.ToLower() != StrixPlatform.DefaultCultureCode.ToLower() ? string.Format("/{0}", cultureCode) : string.Empty;
            return url + cultureAddition;
        }
        #endregion
    }
}