using NeoErp.Core.Services.CommonSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Settings
{
    public class GoodsReceiptNotesReportJsonSetting : ISettings
    {
        public string SID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifyDate { get; set; }
       
        
    }
}
