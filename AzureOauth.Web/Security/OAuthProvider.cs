using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using AzureOauth.Module.BusinessObjects;
using AzureOauth.Module.Web.Security;


namespace AzureOauth.Web.Security
{
    public class OAuthProvider : IAuthenticationProvider
    {
        private readonly Type userType;
        private readonly SecurityStrategyComplex security;
        public bool CreateUserAutomatically { get; set; }
        public OAuthProvider(Type userType, SecurityStrategyComplex security)
        {
            Guard.ArgumentNotNull(userType, "userType");
            this.userType = userType;
            this.security = security;
        }
        public object Authenticate(IObjectSpace objectSpace)
        {
            ApplicationUser user = null;
            ClaimsIdentity externalLoginInfo = Authenticate();
            if (externalLoginInfo != null)
            {
                string userEmail = externalLoginInfo?.FindFirst("preferred_username")?.Value;
                if (userEmail != null)
                {
                    user = (ApplicationUser)objectSpace.FindObject(userType, CriteriaOperator.Parse("OAuthAuthenticationEmails[Email = ?]", userEmail));
                    if (user == null && CreateUserAutomatically)
                    {
                        user = (ApplicationUser)objectSpace.CreateObject(userType);
                        user.UserName = userEmail;
                        EmailEntity email = objectSpace.CreateObject<EmailEntity>();
                        email.Email = userEmail;
                        user.OAuthAuthenticationEmails.Add(email);
                        ((CustomSecurityStrategyComplex)security).InitializeNewUser(objectSpace, user);
                        objectSpace.CommitChanges();
                    }
                }
            }
            else
            {
                WebApplication.Redirect(WebApplication.LogonPage);
            }
            if (user == null)
            {
                throw new Exception("Login failed");
            }
            return user;
        }
        private ClaimsIdentity Authenticate()
        {
            return HttpContext.Current.GetOwinContext().Authentication.User.Identity as ClaimsIdentity;
        }
        public void Setup(params object[] args)
        {
        }
    }
}