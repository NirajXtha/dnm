using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Fakes;
using NeoErp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Framework
{
    public class WebWorkContext : IWorkContext
    {
        #region Const


        private readonly IAuthenticationService _authenticationService;
        private const string UserCookieName = "Neo.User";

        #endregion

        #region Fields

        private readonly HttpContextBase _httpContext;


        private User _cachedUser;

        #endregion

        public WebWorkContext(HttpContextBase httpContext, IAuthenticationService authenticationService)
        {
            this._httpContext = httpContext;
            this._authenticationService = authenticationService;

        }

        #region Utilities

        protected virtual HttpCookie GetCustomerCookie()
        {
            if (_httpContext == null || _httpContext.Request == null)
                return null;

            return _httpContext.Request.Cookies[UserCookieName];
        }

        protected virtual void SetCustomerCookie(Guid customerGuid)
        {
            if (_httpContext != null && _httpContext.Response != null)
            {
                var cookie = new HttpCookie(UserCookieName);
                cookie.HttpOnly = true;
                cookie.Value = customerGuid.ToString();
                if (customerGuid == Guid.Empty)
                {
                    cookie.Expires = DateTime.Now.AddMonths(-1);
                }
                else
                {
                    int cookieExpires = 24 * 365; //TODO make configurable
                    cookie.Expires = DateTime.Now.AddHours(cookieExpires);
                }

                _httpContext.Response.Cookies.Remove(UserCookieName);
                _httpContext.Response.Cookies.Add(cookie);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        public virtual User CurrentUserinformation
        {
            get
            {
                if (_cachedUser != null)
                    return _cachedUser;

                User user = null;


                //registered user
                if (user == null)
                {
                    user = _authenticationService.GetAuthenticatedCustomer();
                }


                //On first load no userGuid found which cause exception , Must resolved this issue which only cause sometime - aaku
                if (user != null)
                {

                    SetCustomerCookie(user.UserGuid);
                }
                _cachedUser = user;


                return _cachedUser;
            }
            set
            {
                SetCustomerCookie(value.UserGuid);
                _cachedUser = value;
            }
        }



        #endregion
    }
}