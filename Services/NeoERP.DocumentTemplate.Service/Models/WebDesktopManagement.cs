using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class WebDesktopManagement
    {
        public string WEB_DESKTOP_MANAGEMENT_ID   {get; set;}
        public string FORM_CODE                   {get; set;}
        public string NEW_FOLDER_NAME             {get; set;}
        public string PREV_FOLDER_NAME            {get; set;}
        public string TEMPLATE_CODE               {get; set;}
        public string HREF                        {get; set;}
        public string MODULE_CODE                 {get; set;}
        public string FUNCTION_LINK               {get; set;}
        public string FORM_TYPE                   {get; set;}
        public string COLOR                       {get; set;}
        public string ICON_PATH                   {get; set;}
        public string ABBR                        {get; set;}
        public string FORM_EDESC                  {get; set;}
        public string MENU_EDESC                  {get; set;}
        public string MENU_DESC { get; set; }   
        
        public string MENU_NO                     {get; set;}
        public string USER_ID                     {get; set;}
        public string COMPANY_CODE                {get; set;}
        public string BRANCH_CODE                 {get; set;}
        public string CREATED_BY                  {get; set;}
        public string CREATED_DATE                {get; set;}
        public string DELETED_FLAG                { get; set; }
        public string MODIFIED_DATE               { get; set; }
        public string MODIFIED_BY                 { get; set; }
        public string UNIQUE_ID                   { get; set; }
        public string RESET                       { get; set; }
        public List<FOLDER_ORDER> FOLDER_ORDER    { get; set; }
        public bool SideBarMenu                   { get; set; }
        public string SIDEBAR_ID                  { get; set; }
        public string CREATE_FLAG { get; set; }
        public string UPDATE_FLAG { get; set; }
    }

    public class FOLDER_ORDER
    {
       public string FOLDER_ID             { get; set; }
       public string ORDER_NO              { get; set; }
    }
}
