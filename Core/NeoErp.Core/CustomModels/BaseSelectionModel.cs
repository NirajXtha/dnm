using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NeoErp.Core.Integration;

namespace NeoErp.Core.Models
{   
    public class BaseSelectionModel
    {
        protected BaseSelectionModel(double? days)
        {
            if(days!=null)
                DateFrom = string.Format("{0:dd/MM/yyyy}", DateTime.UtcNow.Date.AddDays(-(double)days));
            DateTo = string.Format("{0:dd/MM/yyyy}", DateTime.UtcNow.Date);
            //DateType = "_Settings._S.setting.PrimaryDateType";
        }

        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int? DateType { get; set; } 
    }

}