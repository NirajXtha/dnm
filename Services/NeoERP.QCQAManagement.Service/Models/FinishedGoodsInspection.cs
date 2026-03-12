using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class FinishedGoodsInspection
    {
        public string Plant_Id { get; set; }
        public string PARAMETER_ID { get; set; }
        public string PARAMETERS { get; set; }
        public string Batch_No { get; set; }
        public string REFERENCE_NO { get; set; }
        public string FINISH_GOODS_INSP_NO { get; set; }
        public string ITEM_CODE { get; set; }
        public string PACK_CONDITION { get; set; }
        public string PACK_CONDITION_NO { get; set; }
        public string PACK_COND_REMARKS { get; set; }
        public string LABEL_ACCURACY { get; set; }
        public string LABEL_ACCURACY_NO { get; set; }
        public string LABEL_ACC_REMARKS { get; set; }
        public string PRODUCT_APPEARANCE { get; set; }
        public string PRODUCT_APPEARANCE_NO { get; set; }
        public string PRODUCT_APP_REMARKS { get; set; }
        public string DIMENSIONS { get; set; }
        public string DIMENSIONS_NO { get; set; }
        public string DIMENSIONS_REMARKS { get; set; }
        public string COMPLIANCE_CERTIFICATES { get; set; }
        public string COMPLIANCE_CERTIFICATES_NO { get; set; }
        public string COMP_CERT_REMARKS { get; set; }
        public string VENDOR_TEST { get; set; }
        public string VENDOR_TEST_NO { get; set; }
        public string VENDOR_TEST_REMARKS { get; set; }
        public string SAMPLING_METHOD { get; set; }
        public string SAMP_METHOD_REMARKS { get; set; }
        public string SAMPLE_SIZE { get; set; }
        public string SAMP_SIZE_REMARKS { get; set; }
        public string NUMBER_PASSED { get; set; }
        public string NUMBER_PASSED_NO { get; set; }
        public string NUMBER_PASSED_REMARKS { get; set; }
        public string DEFECT_TYPE { get; set; }
        public string DEFECT_TYPE_REMARKS { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string ACTION_TAKEN_REMARKS { get; set; }
        public string REMARKS { get; set; }
        public string FINAL_REMARKS { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string GRN_NO { get; set; }
        public DateTime MFG_DATE { get; set; }
        public DateTime EXP_DATE { get; set; }
        public string QUANTITY { get; set; }
        public string VENDOR_NAME { get; set; }
        public string COMPANY_CODE { get; set; }
        public string FORM_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public DateTime RECEIPT_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public int SERIAL_NO { get; set; }
        public List<FinishedGoodsInspectionDetails> FinishedGoodsInspectionDetailsList { get; set; }
    }
    public class FinishedGoodsInspectionDetails
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string COLUMN_HEADER { get; set; }
        public string PACK_CONDITION { get; set; }
        public string PACK_CONDITION_NO { get; set; }
        public string PACK_COND_REMARKS { get; set; }
        public string LABEL_ACCURACY { get; set; }
        public string LABEL_ACCURACY_NO { get; set; }
        public string LABEL_ACC_REMARKS { get; set; }
        public string PRODUCT_APPEARANCE { get; set; }
        public string PRODUCT_APPEARANCE_NO { get; set; }
        public string PRODUCT_APP_REMARKS { get; set; }
        public string DIMENSIONS { get; set; }
        public string DIMENSIONS_NO { get; set; }
        public string DIMENSIONS_REMARKS { get; set; }
        public string COMPLIANCE_CERTIFICATES { get; set; }
        public string COMPLIANCE_CERTIFICATES_NO { get; set; }
        public string COMP_CERT_REMARKS { get; set; }
        public string VENDOR_TEST { get; set; }
        public string VENDOR_TEST_NO { get; set; }
        public string VENDOR_TEST_REMARKS { get; set; }
        public string SAMPLING_METHOD { get; set; }
        public string SAMP_METHOD_REMARKS { get; set; }
        public string SAMPLE_SIZE { get; set; }
        public string SAMP_SIZE_REMARKS { get; set; }
        public string NUMBER_PASSED { get; set; }
        public string NUMBER_PASSED_NO { get; set; }
        public string NUMBER_PASSED_REMARKS { get; set; }
        public string DEFECT_TYPE { get; set; }
        public string DEFECT_TYPE_REMARKS { get; set; }
        public string ACTION_TAKEN { get; set; }
        public string ACTION_TAKEN_REMARKS { get; set; }
        public string REMARKS { get; set; }
        public string FINAL_REMARKS { get; set; }
    }
}
