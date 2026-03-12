using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class DocumentConsumption
    {
        public string IssueNo { get; set; }
        public string ManualNo { get; set; }
        public string UserId { get; set; }
        public string DepartmentId { get; set; }
        public string Date { get; set; }
        public string Miti { get; set; }
        public string EmployeeId { get; set; }

        public virtual List<Users> Users { get; set; }
        public virtual List<Department> Departments { get; set; }
        public virtual List<Employee> Employees { get; set; }
    }

    public class controler
    {
        public string Col { get; set; } = "2";
    }
    public class ConsumptionIssue
    {
        public string Date { get; set; }
        public string Miti { get; set; }
        public string DepartmentId { get; set; }
        public string EmployeeId { get; set; }
        public string IssueId { get; set; }
        public string MannualNo { get; set; }
        public string UserId { get; set; }
        public string GrandTotal { get; set; }
        public List<Issues> Issues { get; set; }
    }

    public class Issues
    {
        //public Location FromLocation { get; set; }
        //public Products ProductDescription { get; set; }
        public string FromLocation { get; set; }
        public string ProductDescription { get; set; }
        public string Unit { get; set; }
        public string Quantity { get; set; }
        public string Rate { get; set; }
        public string Amount { get; set; }
        //public CostCenter CostCenter { get; set; } 
        public string CostCenter { get; set; }
        public string Remark { get; set; }
        public string SN { get; set; }
    }
    public class VoucherRefrence
    {
        public List<checklist> checkList { get; set; }
        public string FormCode { get; set; }
        public string TableName { get; set; }
        public string ModuleCode { get; set; }
        public string ROW { get; set; }
        public string INCLUDE_CHARGE { get; set; }

    }

    public class ProductionRefrence
    {
      
        public string FormCode { get; set; }
        public string TableName { get; set; }
        public string ModuleCode { get; set; }
        public string RoutingName { get; set; }
        public string RoutingCode { get; set; }
        public string Production_Qty { get; set; }

    }

    public class checklist
    {
        public string VOUCHER_NO { get; set; }
        public string SERIAL_NO { get; set; }
        public string TABLE_NAME { get; set; }
        public string ITEM_CODE { get; set; }
        public string MAIN_ITEM_CODE { get; set; }
        public string REF_FORM_CODE { get; set; }
        public string QUANTITY { get; set; }
        public string TOTAL_PRICE { get; set; }
        public string UNIT_PRICE { get; set; }
        public string IS_PARENT_ROW { get; set; }
        public string GATE_NO { get; set; }
    }
    public class Employee
    {
        public string ID { get; set; }
        public string Employee_Name { get; set; }
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }
        public string EMAIL { get; set; }

        public string EPERMANENT_ADDRESS1 { get; set; }
        public string ETEMPORARY_ADDRESS1 { get; set; }
        public string MOBILE { get; set; }
        public string CITIZENSHIP_NO { get; set; }
        public string Type { get; set; }


    }

    public class Department
    {
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string Type { get; set; }
    }

    public class Location
    {
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string Address { get; set; }
        public string Auth_Contact_Person { get; set; }
        public string Telephone_Mobile_No { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Type { get; set; }
    }

    public class Products
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemUnit { get; set; }
        public string ItemGrade { get; set; }
        public string Category { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string Type { get; set; }
        public string MultiItemUnit { get; set; }
    }
    public class CustomerDetail
    {
        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_NAME { get; set; }
        public string SHIPPING_ADDRESS { get; set; }
        public string SHIPPING_CONTACT_NO { get; set; }
      
    }
    public class VECHILES
    {
        public string VEHICLE_CODE { get; set; }
        public string VEHICLE_EDESC { get; set; }
        public string VEHICLE_TYPE { get; set; }
        public string VEHICLE_ID { get; set; }
        public string DRIVER_NAME { get; set; }
        public string DRIVER_LICENCE_NO { get; set; }
        public string DRIVER_MOBILE_NO { get; set; }
        public string VEHICLE_OWNER_NAME { get; set; }
        public string VEHICLE_OWNER_NO { get; set; }
    }
    public class TRANSPORTER
    {
        public string TRANSPORTER_CODE { get; set; }
        public string TRANSPORTER_EDESC { get; set; }

    }
    public class CITY
    {
        public string CITY_CODE { get; set; }
        public string CITY_EDESC { get; set; }

    }

    public class CodeForLog
    {
        public string FORM_CODE { get; set; }
        public string MODULE_CODE { get; set; }
        public string FORM_TYPE { get; set; }
        public string FORM_EDESC { get; set; }
        public string UNIQUE_ID { get; set; }
    }
    public class MuCodeModel
    {
        public string MU_CODE { get; set; }
        public string MU_EDESC { get; set; }
        public string MU_NDESC { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
    }

    public class CategoryModel
    {
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public string CATEGORY_NDESC { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public string CATEGORY_TYPE { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string PREFIX_TEXT { get; set; }
        public string MODIFY_BY { get; set; }
    }
    public class AccountSetup
    {
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string Type { get; set; }
       
    }
    public class TDSCODE
    {
        public string TDS_CODE { get; set; }
        public string TDS_EDESC { get; set; }
        public string TDS_TYPE_CODE { get; set; }
    }
    public class BudgetCenter
    {
        public string BUDGET_CODE { get; set; }
        public string BUDGET_EDESC { get; set; }
    }
    public class AreaSetup
    {
        public string AREA_CODE { get; set; }
        public string AREA_EDESC { get; set; }
    }
    public class PartyType
    {
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string ACC_CODE { get; set; }
    }

    public class PartyRating
    {
        public string PR_CODE { get; set; }
        public string PR_EDESC { get; set; }
    }

    public class CashVoucherReceipt
    {
        public decimal SERIAL_NO { get; set; }
        public string RECEIPT_NAME { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
    }
    public class SubLedger
    {
        public string SUB_CODE { get; set; }
        public string SUB_EDESC { get; set; }
        public string SUB_NDESC { get; set; }
        public string SUB_LEDGER_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string MODIFY_BY { get; set; }
        public string SUBSTITUTE_NAME { get; set; }
        public string LOCK_FLAG { get; set; }
        public string SUB_ID { get; set; }
        public string Type { get; set; }
    }
    public class ChargeSetup
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
    }

    public class CostCenter
    {
        public string BudgetCode { get; set; }
        public string BudgetName { get; set; }
        public string Type { get; set; }
    }

    public class Users
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }

    public class Customers
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string GuestName { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string TPIN_VAT_NO { get; set; }
        public string REGION_CODE { get; set; }
        public string ZONE_CODE { get; set; }
        public string DEALING_PERSON { get; set; }
        public string Type { get; set; }
        public string EMAIL { get; set; }
    }
    public class CustomersSearch
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Type { get; set; }
    }
    public class Suppliers
    {
        public string SUPPLIER_CODE { get; set; }
        public string SUPPLIER_EDESC { get; set; }
        public string REGD_OFFICE_EADDRESS { get; set; }
        public string TEL_MOBILE_NO1 { get; set; }
        public string Type { get; set; }
        public string LINK_SUB_CODE { get; set; }
    }

    public class Branch
    {
        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }
        public string Type { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE_NO { get; set; }
    }

    public class ItemUnit
    {
        public decimal CONVERSION_FACTOR { get; set; }
        public decimal FRACTION { get; set; }
        public int SERIAL_NO { get; set; }
        public string ITEM_CODE { get; set; }
    }

    public class IssueType
    {
        public string ISSUE_TYPE_CODE { get; set; }
        public string ISSUE_TYPE_EDESC { get; set; }
        public string Type { get; set; }
        public string REMARKS { get; set; }

    }
    public class Brand
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
    }
    public class Currency
    {
        public string CURRENCY_CODE { get; set; }
        public string CURRENCY_EDESC { get; set; }
        public decimal EXCHANGE_RATE { get; set; }
        public string Type { get; set; }
    }
    public class PurchaseType
    {
        public string P_TYPE { get; set; }
        public string P_TYPE_EDESC { get; set; }
    }
    public class PaymentMode
    {
        public string PAYMENT_MODE_CODE { get; set; }
        public string PAYMENT_MODE_EDESC { get; set; }

    }
    public class Division
    {
        public string DIVISION_CODE { get; set; }
        public string DIVISION_EDESC { get; set; }
        public string Type { get; set; }
        public string ADDRESS { get; set; }
        public string TELEPHONE_NO { get; set; }
    }
    public class Priority
    {
        public string PRIORITY_CODE { get; set; }
        public string PRIORITY_EDESC { get; set; }
        public string Type { get; set; }
    }
    public class TNC
    {
        public string TNC_CODE { get; set; }
        public string TNC_EDESC { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_TNC_CODE { get; set; }
    }
    public class ItemWiseAttribute
    {
        public string ITEM_CODE { get; set; }
        public int SERIAL_NO { get; set; }
    }
    public class Agent
    {
        public string AGENT_CODE { get; set; }
        public string AGENT_EDESC { get; set; }
        public string AGENT_TYPE { get; set; }
    }
    public class SalesType
    {
        public string SALES_TYPE_CODE { get; set; }
        public string SALES_TYPE_EDESC { get; set; }
        public string Type { get; set; }
    }
    public class DocumentType
    {
        public string TYPE_CODE { get; set; }
        public string TYPE_EDESC { get; set; }
    }
    public class CustomerItemType
    {
        public decimal DISCOUNT_RATE { get; set; }
        public decimal DISCOUNT_PERCENT { get; set; }
    }
    public class RefrenceType
    {
        public string REFERENCE_FLAG { get; set; }
        public string REF_CODE { get; set; }
        public string REF_EDESC { get; set; }
        public string REF_TABLE_NAME { get; set; }
        public string FORM_CODE { get; set; }
    }
    public class ItemRateScheduleSetUp
    {
        public decimal STANDARD_RATE { get; set; }
    }
        public class ChargeOnSales
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public int? PRIORITY_INDEX_NO { get; set; }
        public string VALUE_PERCENT_FLAG { get; set; }
        public double? VALUE_PERCENT_AMOUNT { get; set; }
        public double? CHARGE_AMOUNT { get; set; }
        public double? CALC { get; set; }
        public DateTime APPLY_FROM_DATE { get; set; }
        public DateTime APPLY_TO_DATE { get; set; }
        public string APPLY_ON { get; set; }
        public string CHARGE_APPLY_ON { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string CHARGE_TYPE { get; set; }
        public string IMPACT_ON { get; set; }
        public string APPLY_QUANTITY { get; set; }
        public string SUB_CODE { get; set; }
        public string SERIAL_NO { get; set; }
        public string BUDGET_CODE { get; set; }
        public string GL_FLAG { get; set; }
        public string NON_GL_FLAG { get; set; }
        public string GL { get; set; }
        public string APPORTION_FLAG { get; set; }
        public string APPORTION { get; set; }
        public string ON_ITEM { get; set; }
        public string MANUAL_CALC_CHARGE { get; set; }
        public string SPECIFIC_CHARGE_FLAG { get; set; }

        //AA Added to check whether the charges are active or not 
        public string CHARGE_ACTIVE_FLAG { get; set; }
        public string CHARGE_RATE_FLAG { get; set; }





    }
    public class ChargeOnItem
    {
        public List<ChargeOnItemAmountWise> ChargeOnItemAmountWiseList { get; set; }
        public List<ChargeOnItemQuantityWise> ChargeOnItemQuantityWiseList { get; set; }
    }

    public class ChargeOnItemAmountWise
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public int? PRIORITY_INDEX_NO { get; set; }
        public string VALUE_PERCENT_FLAG { get; set; }
        public double? VALUE_PERCENT_AMOUNT { get; set; }
        public double? CHARGE_AMOUNT { get; set; }
        public DateTime APPLY_FROM_DATE { get; set; }
        public DateTime APPLY_TO_DATE { get; set; }
        public string APPLY_ON { get; set; }
        public string CHARGE_APPLY_ON { get; set; }
        public string ACC_CODE { get; set; }
        public string CHARGE_TYPE { get; set; }
        public string IMPACT_ON { get; set; }
        public string APPLY_QUANTITY { get; set; }
        public string SUB_CODE { get; set; }
        public string BUDEGT_CODE { get; set; }
        public string BUDGET_VAL { get; set; }
        public string GL { get; set; }
        public string APPORTION { get; set; }
        public string CALC { get; set; }

    }
    public class ChargeOnItemQuantityWise
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
        public string CHARGE_TYPE_FLAG { get; set; }
        public int? PRIORITY_INDEX_NO { get; set; }
        public string VALUE_PERCENT_FLAG { get; set; }
        public double? VALUE_PERCENT_AMOUNT { get; set; }
        public double? CHARGE_AMOUNT { get; set; }
        public DateTime APPLY_FROM_DATE { get; set; }
        public DateTime APPLY_TO_DATE { get; set; }
        public string APPLY_ON { get; set; }
        public string CHARGE_APPLY_ON { get; set; }
        public string ACC_CODE { get; set; }
        public string CHARGE_TYPE { get; set; }
        public string IMPACT_ON { get; set; }
        public string APPLY_QUANTITY { get; set; }
        public string SUB_CODE { get; set; }
        public string BUDEGT_CODE { get; set; }
        public string BUDGET_VAL { get; set; }
        public string GL { get; set; }
        public string APPORTION { get; set; }
        public string CALC { get; set; }


    }

    public class TemplateType
    {

        public string TABLE_CODE { get; set; }
        public string TABLE_EDESC { get; set; }

    }
    public class Document
    {

        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }

    }
    public class ChargeCode
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
        public string CHARGE_NDESC { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SYN_ROWID { get; set; }
        public string SPECIFIC_CHARGE_FLAG { get; set; }
        public DateTime? MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
    }

    public class ChargeSetupModel
    {
        public string CHARGE_CODE { get; set; }
        public string CHARGE_EDESC { get; set; }
        public string CHARGE_NDESC { get; set; }
        public string REMARKS { get; set; }
        public string COMPANY_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SPECIFIC_CHARGE_FLAG { get; set; }
    }
    public class ApplicationUser
    {

        public int USER_NO { get; set; }
        public string LOGIN_EDESC { get; set; }

    }
    public class DescriptionDynamicMenu
    {
        public string MENU_NAME { get; set; }
        public string MENU_EDESC { get; set; }

        public string MENU_NO { get; set; }

        public string VIRTUAL_PATH { get; set; }

        public string FULL_PATH { get; set; }

        public string GROUP_SKU_FLAG { get; set; }

        public string ICON_PATH { get; set; }
        public string MODULE_CODE { get; set; }
        public string PRE_MENU_NO { get; set; }
        public List<DynamicMenu> descItems { get; set; }
    }
    public class DynamicMenu
    {
        public string MENU_EDESC { get; set; }

        public string MENU_NO { get; set; }

        public string VIRTUAL_PATH { get; set; }

        public string FULL_PATH { get; set; }

        public string GROUP_SKU_FLAG { get; set; }

        public string ICON_PATH { get; set; }
        public string MODULE_CODE { get; set; }
        public string MODULE_ABBR { get; set; }
        public string COLOR { get; set; }
        public string DESCRIPTION { get; set; }
        public string DASHBOARD_FLAG { get; set; }
        public string PRE_MENU_NO { get; set; }
        public List<DynamicMenu> Items { get; set; }


    }

    public class DynamicMenuForAllModule
    {


        public string MENU_NO { get; set; }
        public string PRE_MENU_NO { get; set; }
        public string MENU_EDESC { get; set; }
        public string MODULE_CODE { get; set; }
        public string VIRTUAL_PATH { get; set; }

        public string FULL_PATH { get; set; }

        public string GROUP_SKU_FLAG { get; set; }

        public string ICON_PATH { get; set; }


        //public string MODULE_ABBR { get; set; }
        //public string COLOR { get; set; }
        //public string DESCRIPTION { get; set; }

        //public string DASHBOARD_FLAG { get; set; }

        //public List<DynamicMenuForAllModule> DynamicMenuList { get; set; } 
        //public List<DynamicMenuForAll> DynamicMenuList { get; set; }   
        //public List<DocumentTemplateMenu> DocumentTemplateList { get; set; }


    }


    public class DocumentTemplateMenu
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        public string PRE_FORM_CODE { get; set; }
        public string MODULE_CODE { get; set; }
        public string TEMPLATE_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string DATE_FORMAT { get; set; }
        public string START_ID_FLAG { get; set; }

    }
    public class DynamicMenuForAll
    {
        public string MENU_EDESC { get; set; }

        public string MENU_NO { get; set; }

        public string VIRTUAL_PATH { get; set; }

        public string FULL_PATH { get; set; }

        public string GROUP_SKU_FLAG { get; set; }

        public string ICON_PATH { get; set; }
        public string MODULE_CODE { get; set; }
        public string MODULE_ABBR { get; set; }
        public string COLOR { get; set; }
        public string DESCRIPTION { get; set; }

        public string DASHBOARD_FLAG { get; set; }
    }

    public class ShippingDetails
    {
        public string VEHICLE_CODE { get; set; }
        public string VEHICLE_OWNER_NAME { get; set; }
        public string VEHICLE_OWNER_NO { get; set; }
        public string DRIVER_NAME { get; set; }
        public string DRIVER_LICENCE_NO { get; set; }
        public string DRIVER_MOBILE_NO { get; set; }
        public string TRANSPORTER_CODE { get; set; }
        public decimal? FREGHT_AMOUNT { get; set; }
        public string START_FORM { get; set; }
        public string DESTINATION { get; set; }
        //public string TRANSPOTRT_INVOICE_NO { get; set; }
        public string CN_NO { get; set; }
        //public DateTime? TRANSPORT_INVOICE_DATE { get; set; }
        //public DateTime? DELIVERY_INVOICE_DATE { get; set; }
        public string TRANSPORT_INVOICE_DATE { get; set; }
        public string DELIVERY_INVOICE_DATE { get; set; }
        public string TRANSPORT_INVOICE_NO { get; set; }
        public decimal? WB_WEIGHT { get; set; }
        public string WB_NO { get; set; }
        //public DateTime? WB_DATE { get; set; }
        public string WB_DATE { get; set; }
        public decimal? FREIGHT_RATE { get; set; }
        public string VEHICLE_NO { get; set; }
        public decimal? LOADING_SLIP_NO { get; set; }
        public string GATE_ENTRY_NO { get; set; }
        //public DateTime? GATE_ENTRY_DATE { get; set; }
        public string GATE_ENTRY_DATE { get; set; }
        public string SHIPPING_TERMS { get; set; }
    }

    public class ShippingDetailsViewModel
    {
        public string VEHICLE_CODE { get; set; }
        public string VEHICLE_OWNER_NAME { get; set; }
        public string VEHICLE_OWNER_NO { get; set; }
        public string DRIVER_NAME { get; set; }
        public string DRIVER_LICENSE_NO { get; set; }
        public string DRIVER_MOBILE_NO { get; set; }
        public string TRANSPORTER_CODE { get; set; }
        public decimal? FREGHT_AMOUNT { get; set; }
        public string START_FORM { get; set; }
        public string DESTINATION { get; set; }
        //public string TRANSPOTRT_INVOICE_NO { get; set; }
        public string CN_NO { get; set; }
        public DateTime? TRANSPORT_INVOICE_DATE { get; set; }
        public DateTime? DELIVERY_INVOICE_DATE { get; set; }

        public string TRANSPORT_INVOICE_NO { get; set; }
        public decimal? WB_WEIGHT { get; set; }
        public string WB_NO { get; set; }
        public DateTime? WB_DATE { get; set; }

        public decimal? FREIGHT_RATE { get; set; }
        public string VEHICLE_NO { get; set; }
        public decimal? LOADING_SLIP_NO { get; set; }
        public string GATE_ENTRY_NO { get; set; }
        public DateTime? GATE_ENTRY_DATE { get; set; }
        //public string GATE_ENTRY_DATE { get; set; }
        public string SHIPPING_TERMS { get; set; }

        public string VEHICLE_EDESC { get; set; }

        public string TRANSPORTER_EDESC { get; set; }
    }
    public class DropZoneFile
    {
        public string FILE_NAME { get; set; }
        public string FORM_CODE { get; set; }
        public string VOUCHER_NO { get; set; }
    }
    public class CUSTOMEROPTION
    {
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
    }
    public class ITEMOPTION
    {
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_ITEM_CODE { get; set; }
        public string PRE_ITEM_CODE { get; set; }
    }
    public class PARTYTYPEOPTION
    {
        public string GROUP_SKU_FLAG { get; set; }
        public string MASTER_PARTY_CODE { get; set; }
        public string PRE_PARTY_CODE { get; set; }
    }
    public class SUBLEDGEROPT
    {
        public string SUB_CODE { get; set; }
        public string ACC_CODE { get; set; }
    }
}