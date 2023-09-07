﻿using AzureOauth.Module.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using System;

namespace AzureOauth.Module.Security
{
    public class CustomAuthenticationStandardProvider : AuthenticationStandardProvider
    {
        public CustomAuthenticationStandardProvider(Type userType) : base(userType)
        {
        }
        public override object Authenticate(IObjectSpace objectSpace)
        {
            ApplicationUser user = base.Authenticate(objectSpace) as ApplicationUser;
            if (user != null && !user.EnableStandardAuthentication)
            {
                throw new InvalidOperationException("Password authentication is not allowed for this user.");
            }
            return user;
        }
    }
}
