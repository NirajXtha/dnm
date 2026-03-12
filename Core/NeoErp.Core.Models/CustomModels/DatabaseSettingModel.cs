using NeoErp.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models
{
   public class DatabaseSettingModel
    {
        [Display(Name = "Connection Type")]
        public ConnectionType ConType { get; set; }
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }          
    }
}
