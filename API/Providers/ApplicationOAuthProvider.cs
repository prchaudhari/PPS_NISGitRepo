// <copyright file="ApplicationOAuthProvider.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2017 Websym Solutions Pvt Ltd.
// </copyright>
// -----------------------------------------------------------------------  

namespace nIS
{
    #region References

    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.OAuth;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    #endregion

    /// <summary>
    /// This class represents oauth provider which handles login request.
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        #region Private Members

        /// <summary>
        /// Public client id
        /// </summary>
        private readonly string publicClientId;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializing instance of class
        /// </summary>
        /// <param name="publicClientId">
        /// Public client id
        /// </param>
        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            this.publicClientId = publicClientId;
        }
        #endregion

        /// <summary>
        /// This method will handle login request and generate a token using claims. 
        /// </summary>
        /// <param name="context">
        /// OAuthGrantResourceOwnerCredentialsContext object
        /// </param>
        /// <returns>
        /// 
        /// </returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            string tenantCode = context.ClientId;
            User user = null;
            try
            {
                if (string.IsNullOrWhiteSpace(context.UserName) || string.IsNullOrWhiteSpace(context.Password))
                {
                    throw new Exception(tenantCode);
                }

                user = new AuthenticationManager(Container.GetUnityContainer()).UserAuthenticate(context.UserName, context.Password, tenantCode);
            }
            catch (Exception catchException)
            {
                string message = string.Empty;
                if (catchException.Message.Contains("~"))
                {
                    message = catchException.Message.Split('~')[1];
                }
                context.SetError("invalid_grant", message);
                return;
            }

            // Adding claims
            ClaimsIdentity claimIdentity = new ClaimsIdentity("JWT");
            claimIdentity.AddClaim(new Claim(ClaimType.UserIdentifier.ToString(), user.EmailAddress.ToString()));
            claimIdentity.AddClaim(new Claim(ClaimType.TenantCode.ToString(), user.TenantCode));
            claimIdentity.AddClaim(new Claim(ClaimType.UserId.ToString(), user.Identifier.ToString()));

            List<RolePrivilege> userCliams = new List<RolePrivilege>();
            userCliams.AddRange(user.Roles.SelectMany(role => role.RolePrivileges).ToList());

            userCliams?.ToList().ForEach(privilege =>
            {
                claimIdentity.AddClaim(new Claim(privilege.EntityName, string.Join(",", privilege.RolePrivilegeOperations)));
            });

            //Add is asset connected value in claim

            string[] propertyData = new string[6]
            {
                user.Identifier.ToString(),
                user.FirstName + " " + user.LastName,
                user.EmailAddress,
                user.TenantCode,
                (userCliams?.Count>0?JsonConvert.SerializeObject(userCliams):string.Empty),
                (user.Roles.Count() > 0 ? user.Roles[0].Name : "")
            };

            AuthenticationTicket ticket = new AuthenticationTicket(claimIdentity, CreateProperties(propertyData));
            context.Validated(ticket);
        }

        /// <summary>
        /// This method will set all properties.
        /// </summary>
        /// <param name="context">
        /// OAuthTokenEndpointContext object.
        /// </param>
        /// <returns>
        /// Returns a task object.
        /// </returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// This method will validate our client id as tenant code.
        /// </summary>
        /// <param name="context">
        /// OAuthValidateClientAuthenticationContext object.
        /// </param>
        /// <returns>
        /// Returns task object.
        /// </returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            // Resource owner password credentials does not provide a client ID.
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                context.SetError("invalid_clientId", "Tenant code is not set.");
                return Task.FromResult<object>(null);
            }

            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == this.publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// This method sets properties in authentication properties object.
        /// </summary>
        /// <param name="userName">
        /// User name or user email address.
        /// </param>
        /// <param name="userIdentifier">
        /// User identifier.
        /// </param>
        /// <returns>
        /// It returns authentication properties object.
        /// </returns>
        public static AuthenticationProperties CreateProperties(string[] stringData)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "UserIdentifier", stringData[0] },
                { "UserName", stringData[1] },
                { "UserPrimaryEmailAddress" , stringData[2]},
                { "TenantCode", stringData[3] },
                { "SerializedUserClaims", stringData[4] },
                { "RoleName", stringData[5] },
                { "RoleIdentifier", stringData[6] }
            };
            return new AuthenticationProperties(data);
        }

        #region Private Method

        /// <summary>
        /// Get Resources
        /// </summary>
        /// <param name="data">Input data</param>
        /// <returns></returns>
        private string GetResource(string data)
        {
            return ""
;        //    string message = string.Empty;
         //    //Get resources
         //    var currentLocale = Thread.CurrentThread.CurrentUICulture;
         //    IUtility utilty = new Utility();
         //    IList<Websym.Core.ResourceManager.Resource> resourceList = new List<Websym.Core.ResourceManager.Resource>();
         //    Websym.Core.ResourceManager.ResourceSearchParameter resourceSearchParameter = new Websym.Core.ResourceManager.ResourceSearchParameter();
         //    resourceSearchParameter.Locale = currentLocale.ToString();

            //    string innerData = data.ToString();
            //    if (!string.IsNullOrWhiteSpace(innerData))
            //    {
            //        string[] value = innerData.Split('~');
            //        if (value.Length == 2)
            //        {
            //            resourceSearchParameter.SectionName = value[0];
            //            resourceSearchParameter.Key = value[1];

            //            try
            //            {
            //                resourceList = utilty.GetResources(resourceSearchParameter, ModelConstant.RESOURCE_BASE_URL, ModelConstant.TENANT_CODE_KEY, ModelConstant.DEFAULT_TENANT_CODE);
            //                if (resourceList.Count > 0)
            //                {
            //                    foreach (Websym.Core.ResourceManager.Resource resource in resourceList)
            //                    {
            //                        foreach (var section in resource.ResourceSections)
            //                        {
            //                            foreach (var item in section.ResourceItems)
            //                            {
            //                                message = item.Value;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                throw ex;
            //            }
            //        }
            //    }
            //    return message;
            //}

            #endregion
        }
    }
}