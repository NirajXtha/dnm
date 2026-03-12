using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IQCQADocumentFinderRepo
    {
        List<QCDocumentFinder> GetDocumentDetails(string formCode, string docVer = "all");
        bool DeleteQCByTransaction(string transactionNo);
    }
}
