using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class ItemSetupModal
    {
        public int ITEM_GROUP_CODE { get; set; }
        public string ITEM_GROUP_NAME { get; set; }
        public string DELETED_FLAG { get; set; }
        public string GROUP_TYPE { get; set; }
        public List<string> ITEM_CODE { get; set; }
       }

    public class ItemCode
    {
        public string ITEM_CODE { get; set; }
        public int ITEM_GROUP_CODE { get; set; }
        public string ITEM_GROUP_NAME { get; set; }
        public string GROUP_TYPE { get; set; }
        public string DELETED_FLAG { get; set; }
    }
}
