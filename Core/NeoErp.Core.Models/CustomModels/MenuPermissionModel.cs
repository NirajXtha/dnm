using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models
{
    public class MenuPermissionModel
    {
        public MenuPermissionModel()
        {
            ChildCount = 0;
            UserGroupID = 0;
            PermissionID = 0;
            FullAccess = false;
            AllowAdd = false;
            AllowEdit = false;
            AllowDelete = false;
            AllowView = false;
            AllowPrint = false;
            IsVisible = false;
        }

        public int ChildCount { get; set; }
        public Nullable<int> UserGroupID { get; set; }
        public Nullable<int> PermissionID { get; set; }
        public bool FullAccess { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowView { get; set; }
        public bool AllowPrint { get; set; }
        public bool IsVisible { get; set; }
    }
}
