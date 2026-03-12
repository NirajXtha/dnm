using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace NeoErp.Core.Helpers
{
    public class IPHelper
    {
        #region IP Manipulation

        public string GetIpAddress()
        {
            string stringIpAddress;
            stringIpAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (stringIpAddress == null)
            {
                stringIpAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return stringIpAddress; //"Visitor IP Address is " + 
        }

        //Get Lan Connected IP address
        public string GetLanIPAddress()
        {
            //Get Host Name
            string stringHostName = System.Net.Dns.GetHostName(); ;
            //Get Ip Host Entry
            IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
            //Get Ip Address From The Ip Host Entry Address List
            IPAddress[] arrIpAddress = ipHostEntries.AddressList;
            return arrIpAddress[arrIpAddress.Length - 1].ToString();
        }

        public string GetIpAddress1()  // Get IP Address
        {
            string ip = "";
            IPHostEntry ipEntry = Dns.GetHostEntry(GetCompCode());
            IPAddress[] addr = ipEntry.AddressList;
            ip = addr[2].ToString();
            return ip;
        }
        public static string GetCompCode()  // Get Computer Name
        {
            string strHostName = "";
            strHostName = Dns.GetHostName();
            return strHostName;
        }


        /// <summary>
        /// Return Local IP address connected to lan
        /// </summary>
        /// <returns></returns>
        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (IP4Address != String.Empty)
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }


        public string GetClientInfo()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Remove(0, str.Length);
            str.AppendFormat("{0} : {1}\n", "Your IP: ", GetIP4Address());
            //  str.AppendFormat("{0} : {1}\n", "Public IP", this.GetIpAddress());

            return str.ToString();
            //return "Your IP : " + GetIP4Address();
        }



        #endregion
    }
}