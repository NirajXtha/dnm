using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class ImageCategoryModel
    {
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public int CATEGORYID { get; set; }
        public int MAX_ITEMS { get; set; }
    }
    public class ResellerGroupModel
    {
        public int GROUPID { get; set; }
        public string GROUP_CODE { get; set; }
        public string GROUP_EDESC { get; set; }
    }

}
