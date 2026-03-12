using NeoErp.Core.Domain;
using NeoERP.ProjectManagement.Service.Models;
using System.Collections.Generic;

namespace NeoERP.ProjectManagement.Service.Interface
{
    public interface IFormProjectRepo
    {
        bool InsertProjectData(Project projectData);
        List<Project> ListAllProjects();
        bool DeleteProjects(int id);

        List<Project> GetProjectById(int id);

        List<Employee> GetAllEmployees();
        List<ProjectCount> GetProjectCount();
        List<ProjectCount> CountBarGraph();
        List<SubProjectData> GetCompletedProject();
        List<SubProjectData> GetStartedProject();
        List<SubProjectData> GetSubProjectList();
        List<EntryReport> GetConsumptionTransDetailByFormCode(string formCode, string docVer = "all");
        List<EntryReport> GetRequisitionTransDetailByFormCode(string formCode, string docVer = "all");
        List<EntryReport> GetRequisitionPendingTransByFormCode(string formCode, string docVer = "all");
        List<EntryReport> GetPurchaseTransByFormCode(string formCode, string docVer = "all");

        List<SupplierModel> GetAllSupplier();
        List<PriceSetupModel> ListAllItemWithName();
    }
}
