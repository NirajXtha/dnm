using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Integration
{
    public class _Settings
    {

        private  string FYear { get; set; }
        private  string LeaveFYear { get; set; }
      

        private int languageID { get; set; }

    

        public static string FiscalYear
        {
            get
            {
                _Settings lm = (_Settings)_Session.GetSession("_Settings");
                if (lm != null)
                    return lm.FYear;
                else
                    return "";
            }
            set
            {
                _Settings lm = (_Settings)_Session.GetSession("_Settings");
                if (lm != null)
                    lm.FYear = value;
                else
                {
                    lm = new _Settings();
                    lm.FYear = value;
                }
                _Session.AddSession("_Settings", lm);
            }
        }

        public static string LeaveFiscalYear
        {
            get
            {
                _Settings lm = (_Settings)_Session.GetSession("_Settings");
                if (lm != null)
                    return lm.LeaveFYear;
                else
                    return "";
            }
            set
            {
                _Settings lm = (_Settings)_Session.GetSession("_Settings");
                if (lm != null)
                    lm.LeaveFYear = value;
                else
                {
                    lm = new _Settings();
                    lm.LeaveFYear = value;
                }
                _Session.AddSession("_Settings", lm);
            }
        }
                
      


    }
}