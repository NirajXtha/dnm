using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeoERP.ProjectManagement.Service.Models
{
    public class Project
    {
        public int ID { get; set; }
        public string PROJECT_NAME { get; set; }
        public DateTime? START_DATE { get; set; }
        public DateTime? END_DATE { get; set; }
        public DateTime? CREATED_DT { get; set; }
        public DateTime? MODIFY_DT { get; set; }
        public string CREATED_BY { get; set; }
        public string MODIFIED_BY { get; set; }
        public List<SubProjectData> SubProjects { get; set; }
    }

    public class SubProjectData
    {
        public int SubProjectId { get; set; }
        public string PROJECT_NAME { get; set; }
        public string SUB_PROJECTNAME { get; set; }
        public string IMAGE_NAME { get; set; }
        public string AREA { get; set; }
        public string BUDGET_PLANNING { get; set; }
        public string PROJECT_MANAGER { get; set; }
        public string CONTRACTOR { get; set; }
        public DateTime START_DT { get; set; }
        public DateTime END_DT { get; set; }
        public string STATUS { get; set; }
        public string Image { get; set; }
        //public HttpPostedFileBase IMAGE_PATH { get; set; }
        //public byte [] IMAGE_BYTES { get; set; }
        public List<MaterialPlanning> MaterialPlanningData { get; set; }
        public List<LabourPlanning> LabourPlanningData { get; set; }
    }
    public class MaterialPlanning
    {
        public int Id { get; set; }
        public string DESCRIPTION { get; set; }
        public string ITEM_NAME { get; set; }
        public int QUANTITY { get; set; }
        public int RATE { get; set; }
        public int AMOUNT { get; set; }
    }
    public class LabourPlanning
    {
        public int Id { get; set; }
        public string DESCRIPTION { get; set; }
        public int QUANTITY { get; set; }
        public int RATE { get; set; }
        public int AMOUNT { get; set; }
    }
}
