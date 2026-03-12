using NeoERP.QCQAManagement.Service.Models;
using NeoERP.QCQAManagement.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IParameterRepo
    {
        List<SubMenu> GetSubMenuList();
        List<QCQASubMenu> GetQCQADetailByFormCode(string formCode, string docVer = "All");
        List<ParameterSetup> MasterItemList();
        List<Items> GetGroupMaterialLists();
        List<Items> GetChildItems(string masterItemCode);
        List<Products> GetProductDetails(string masterItemCode);
        List<Items> GetSpecDetailsByItemID(string itemCode);
        bool InsertParameterData(Items itemList);
    }
}
