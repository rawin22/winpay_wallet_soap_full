using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Tsg.UI.Main.Models.Attributes
{
    

    //[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    //public class AuthorizeUserAttribute : AuthorizeAttribute
    //{
    //        private string[] allowedUsers = new string[] { };
    //        private string[] allowedRoles = new string[] { };

    //        public AuthorizeUserAttribute()
    //        { }
    //        public AuthorizeUserAttribute(params string[] roles)
    //            : base()
    //        {
    //            Roles = string.Join(",", roles);
    //            allowedRoles = roles;
    //        }
    //        public AuthorizeUserAttribute(params UserRoleType[] roles)
    //            : base()
    //        {
    //            Roles = String.Join(",", Enum.GetNames(typeof(UserRoleType)));
    //        }

    //        protected override bool AuthorizeCore(HttpContextBase httpContext)
    //        {
    //            if (!String.IsNullOrEmpty(base.Users))
    //            {
    //                allowedUsers = base.Users.Split(new char[] { ',' });
    //                for (int i = 0; i < allowedUsers.Length; i++)
    //                {
    //                    allowedUsers[i] = allowedUsers[i].Trim();
    //                }
    //            }
    //            if (!String.IsNullOrEmpty(base.Roles))
    //            {
    //                allowedRoles = base.Roles.Split(new char[] { ',' });
    //                for (int i = 0; i < allowedRoles.Length; i++)
    //                {
    //                    allowedRoles[i] = allowedRoles[i].Trim();
    //                }
    //            }

    //            return httpContext.Request.IsAuthenticated &&
    //                   User(httpContext) && Role(httpContext);
    //        }

    //        private bool User(HttpContextBase httpContext)
    //        {
    //            if (allowedUsers.Length > 0)
    //            {
    //                return allowedUsers.Contains(httpContext.User.Identity.Name);
    //            }
    //            return true;
    //        }

    //        private bool Role(HttpContextBase httpContext)
    //        {
    //            if (allowedRoles.Length > 0)
    //            {
    //                for (int i = 0; i < allowedRoles.Length; i++)
    //                {
    //                    if (HttpContext.Current.User.IsInRole(allowedRoles[i]))
    //                        return true;
    //                }
    //                return false;
    //            }
    //            return true;
    //        }
    //    }

    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            try
            {
                if (httpContext.User != null)
                {
                    if (httpContext.User.Identity.IsAuthenticated)
                    {
                        var authCookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                        if (authCookie != null)
                        {
                            var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                            var roles = ticket.UserData.Split('|');
                            var identity = new GenericIdentity(ticket.Name);
                            httpContext.User = new GenericPrincipal(identity, roles);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return base.AuthorizeCore(httpContext);
        }
    }
    }