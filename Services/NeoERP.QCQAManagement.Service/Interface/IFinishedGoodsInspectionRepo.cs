using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IFinishedGoodsInspectionRepo
    {
        List<FinishedGoodsInspection> GetFinishedGoodsInspectionField();
        bool InsertFinishedGoodsInspectionData(FinishedGoodsInspection data);
        FinishedGoodsInspection GetEditFinishedGoodsInspection(string transactionno);
        List<ItemSetup> GetProductWithCategoryFilter(string ProductType);
        FinishedGoodsInspection GetFinishedGoodsInspectionReport(string transactionno);
    }
}
