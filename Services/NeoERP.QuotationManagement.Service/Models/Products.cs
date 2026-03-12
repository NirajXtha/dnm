using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QuotationManagement.Service.Models
{
    public class Products
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemUnit { get; set; }
        public string Category { get; set; }
        public string SPECIFICATION { get; set; }
        public string BRAND_NAME { get; set; }
        public string INTERFACE { get; set; }
        public string TYPE { get; set; }
        public string LAMINATION { get; set; }
        public string ITEM_SIZE { get; set; }
        public string THICKNESS { get; set; }
        public string COLOR { get; set; }
        public string GRADE { get; set; }
        public int SIZE_LENGHT { get; set; }
        public int SIZE_WIDTH { get; set; } 

    }
}
