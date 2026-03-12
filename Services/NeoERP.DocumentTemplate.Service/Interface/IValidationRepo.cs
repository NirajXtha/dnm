using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface IValidationRepo
    {
        bool ValidateCreditLimiCustomerWise(string formcode,string customercode,decimal totalamount, bool isConsolidated,out decimal DrCrTotal,out decimal actualbalance,string companycode);
    }
}
