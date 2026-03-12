using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;

namespace NeoErp.Models
{
    public class TrailBalanceGroupJQGridMOdel
    {
        public JQGrid TrailBalanceGroupJQGrid;

        public TrailBalanceGroupJQGridMOdel()
        {
             TrailBalanceGroupJQGrid = new JQGrid
             {
                Columns = new List<JQGridColumn>()
                {
                    new JQGridColumn { DataField = "ACC_CODE" }, 
                    new JQGridColumn { DataField = "GROUP_EDESC" }, 
                    new JQGridColumn { DataField = "SUB_GROUP_EDESC" }, 
                    new JQGridColumn { DataField = "ACC_EDESC" }, 
                    new JQGridColumn { DataField = "DR_AMOUNT" }, 
                    new JQGridColumn { DataField = "CR_AMOUNT" }, 
                }
             };
        }
    }
}