using NeoErp.Core.Models;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IQCQANumberRepo
    {
        List<FORM_SETUP> GetQCNumberDetails();
        bool InsertQCData(FORM_SETUP formData);
        List<FORM_SETUP> GetQCQAById(string id);
        bool DeleteQCQAId(string id);
    }
}
