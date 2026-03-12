using NeoErp.Transaction.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Transaction.Service.Services.BankSetup
{
    public interface IBankSetupService
    {
        string SaveBank(BankSetupModel model);
        BankSetupModel GetBank(int BankCode);
        string UpdateBank(BankSetupModel model);
        string DeleteBank(int BankCode);
        List<BankSetupModel> GetAllBanks();
        string SaveLimit(BankLimitModel model);
        string UpdateLimit(BankLimitModel model);
        string RenewByid(int Id, DateTime date);
        List<BankLimitModel> GetHistory(int transNo, int sn, string type);
        List<BankLimitModel> GetLimitList(string limitType = "B");
        BankLimitModel GetLimitByTransactionNumber(int transactionNumber, string limitType);
        string SaveCategory(LoanCategoryModel cat);
        List<LoanCategoryModel> GetLoanCategories();
        LoanCategoryModel GetLoanCategory(int id);
        string DeleteCategory(int Id);
    }
}
