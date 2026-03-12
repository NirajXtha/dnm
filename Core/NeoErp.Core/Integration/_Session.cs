using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Integration
{
    public class _Session
    {
        public static void AddSession(string sessionKey, object sessionValue)
        {
            HttpContext.Current.Session[sessionKey] = sessionValue; //.ToString();        

        }

        public static object GetSession(string sessionKey)
        {
            if (HttpContext.Current.Session[sessionKey] != null)
                return HttpContext.Current.Session[sessionKey]; //.ToString();
            else
                return null;
        }

        public static void RemoveSession(string sessionKey)
        {
            string[] key = sessionKey.Split(',');

            foreach (var item in key)
            {
                if (HttpContext.Current.Session[item] != null)
                    HttpContext.Current.Session.Remove(item);
            }
        }


    }
}