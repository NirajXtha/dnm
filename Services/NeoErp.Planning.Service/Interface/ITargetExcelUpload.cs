using NeoErp.Planning.Service.Models;
using System.Collections.Generic;

namespace NeoErp.Planning.Service.Interface
{
    public interface ITargetExcelUploadRepo
    {
        int InsertTargetExcelUpload(TargetExcelUpload model);
        // Optional for bulk excel upload
        int InsertTargetExcelUploadList(List<TargetExcelUpload> modelList);
        
        // Validation methods
        List<DistributorValidationModel> ValidateDistributors(string companyCode);
        List<ItemValidationModel> ValidateItems(string companyCode);
        List<BranchValidationModel> ValidateBranches(string companyCode);
        List<EmployeeValidationModel> ValidateEmployees(string companyCode);

        // New methods for employee filter, data retrieval, and delete
        List<EmployeeValidationModel> GetEmployeeFilterList(string companyCode, int limit);
        List<TargetExcelUpload> GetEmployeeTargetData(string companyCode, string empCode);
        int DeleteByTargetId(string targetId);
    }
}
