using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;

namespace NeoErp.Models
{
    public class CustomerJQGridModel
    {
        public JQGrid CustomerGrid;
        public JQGrid CustomerGridDetails;
        public JQGrid CustomerGridVoucher;
        public JQGrid SubLedger;

        public CustomerJQGridModel()
        {
            CustomerGrid = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                { 
                    new JQGridColumn { DataField = "ACC_CODE", DataType=typeof(string) }, 
                    new JQGridColumn { DataField = "ACC_EDESC", DataType=typeof(string) }, 
                    new JQGridColumn { DataField = "DR_AMOUNT", DataType=typeof(decimal) }, 
                    new JQGridColumn { DataField = "CR_AMOUNT", DataType=typeof(decimal) }, 
                }
            };
            CustomerGridDetails = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                { 
                    new JQGridColumn { DataField = "Count", DataType=typeof(string) },
                    new JQGridColumn { DataField = "VoucherDate", DataType=typeof(string) }, 
                    new JQGridColumn { DataField = "VoucherNO", DataType=typeof(string) }, 
                    new JQGridColumn { DataField = "Particulars", DataType=typeof(string) }, 
                    new JQGridColumn { DataField = "DR_AMOUNT", DataType=typeof(decimal) }, 
                    new JQGridColumn { DataField = "CR_AMOUNT", DataType=typeof(decimal) }, 
                }
            };
            CustomerGridVoucher = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                { 
                    new JQGridColumn { DataField = "VoucherNO", DataType=typeof(string), Width=100 }, 
                    new JQGridColumn { DataField = "SNo", DataType=typeof(string), Width=100 }, 
                    new JQGridColumn { DataField = "Particulars", DataType=typeof(string), Width=500 }, 
                    new JQGridColumn { DataField = "Niration", DataType=typeof(string), Width=500 }, 
                    new JQGridColumn { DataField = "DRAMOUNT", DataType=typeof(decimal), Width=100 }, 
                    new JQGridColumn { DataField = "CRAMOUNT", DataType=typeof(decimal), Width=100 },
                    new JQGridColumn { DataField = "POSTED_BY", DataType=typeof(string) },
                    new JQGridColumn { DataField = "CREATED_BY", DataType=typeof(string) },  
                }
            };
            SubLedger = new JQGrid
            {
                Columns = new List<JQGridColumn>()
                { 
                    new JQGridColumn { DataField = "Sub_Code", DataType=typeof(string), Width=100 }, 
                    new JQGridColumn { DataField = "Sub_EDesc", DataType=typeof(string), Width=500 }, 
                    new JQGridColumn { DataField = "DR_AMOUNT", DataType=typeof(decimal), Width=100 }, 
                    new JQGridColumn { DataField = "CR_AMOUNT", DataType=typeof(decimal), Width=100 },
                }
            };
        }
    }
}