using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IInternalInspectionRepo
    {
        List<FormDetailSetup> GetInternalInspectionList();
        List<INTERNALPRODUCTLIST> GetProductsByProductType(string productType);
        List<ItemSetup> GetVendorDetailsList(string Product);
        //List<ParameterDetails> GetParameterDetailsByItemCode(string productType, string itemCode,string formType);
        List<ParameterDetails> GetParameterDetailsByItemCode(string ProductId);
        bool InsertInternalInspectionData(OnSiteInspection data);
        OnSiteInspection GetEditInternalInspection(string transactionno);
        OnSiteInspection GetInternalInspectionReport(string transactionno);

    }
}
