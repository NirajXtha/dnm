using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.LOC.Services.Models
{
    public class LogisticsModels
    {

        public LogisticsModels()
        {
            FILE_DATA = new List<FileUploadModels>();

        }
        public int LC_TRACK_NO { get; set; }
        public int SNO { get; set; }
        public string PINVOICE_NO { get; set; }

        //public DateTime? ETA { get; set; }
        //public DateTime? ATA { get; set; }
        //public DateTime? EDD { get; set; }
        //public DateTime? A_DD { get; set; }

        public string SRC_ETA { get; set; }
        public string SRC_ETD { get; set; }
        public string SRC_ATA { get; set; }
        public string SRC_ATD { get; set; }
        public string SRC_ETD_DES { get; set; }
        public string DES_ETA { get; set; }
        public string DES_ETD { get; set; }
        public string DES_ATA { get; set; }
        public string DES_ATD { get; set; }
        public string DES_ETD_NEXT_DES { get; set; }

        public string DOC_REC_SUP_DATE { get; set; }
        public string DOC_END_BANK_DATE { get; set; }
        public string DOC_SEND_TOAGENT_DATE { get; set; }
        public string PORT_CLEAR_DATE { get; set; }
        public string POSTAGE_CN { get; set; }



        public int LOGISTIC_CODE { get; set; }
        public string CONTRACTER_NAME { get; set; }
        public string CONTRACTER_CODE { get; set; }
        public string AGENT_CODE { get; set; }
        public string CONTRACTER_ADDRESS { get; set; }
        public string CONTRACT_AMOUNT { get; set; }
        public DateTime? CONTRACT_DATE { get; set; }
        public string JOB_ORDER_NO { get; set; }
        public string SHIPPER_NAME { get; set; }
        public string SHIPPER_ADDRESS { get; set; }
        public string FROM_LOCATION { get; set; }
        public string TO_LOCATION { get; set; }
        public int FROM_LOCATION_CODE { get; set; }
        public int TO_LOCATION_CODE { get; set; }
        public string SHIPMENT_TYPE { get; set; }
        public string SHIPMENT_STATUS { get; set; }
        public string AIR_AWB_NO { get; set; }
        public string SEA_BL_NO { get; set; }
        public DateTime? SEA_BL_DATE { get; set; }
        public string TRAIN_RR_NO { get; set; }
        public DateTime? TRAIN_RR_DATE { get; set; }
        public string ROAD_LR_NO { get; set; }
        public string ROAD_LR_DATE { get; set; }
        public string CONSIGNEE_NAME { get; set; }
        public string CONSIGNEE_ADDRESS { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public string NOTIFY_APPLICANT_NAME { get; set; }
        public string NOTIFY_APPLICANT_ADDRESS { get; set; }
        public string INVOICE_NO { get; set; }
        public string LOAD_TYPE { get; set; }

        public int? SEA_GROSS_WEIGHT { get; set; }
        public int? SEA_CBM { get; set; }
        public decimal? AIR_GROSS_WEIGHT { get; set; }
        public int? AIR_CHARGEABLE_WEIGHT { get; set; }
        public int? CONTAINER_CODE { get; set; }
        public int? ISSUING_CARRIER_CODE { get; set; }
        public DateTime? AIR_AWB_DATE { get; set; }
        public int EST_DAY { get; set; }
        public string SEA_VESSEL_NO { get; set; }
        public string ROAD_TRUCK_NO { get; set; }
        public string ROAD_TRANSPORTER_NAME { get; set; }
        public string ROAD_TRANSPORTER_ADDRESS { get; set; }
        public string REMARKS { get; set; }
        public string DETAIL_ENTRY_FLAG { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public int? RECEIVED_QUANTITY { get; set; }
        public string AIR_FROM_LOCATION { get; set; }
        public string CARRIER_EDESC { get; set; }
        public string CONTAINER_EDESC { get; set; }
        public string AIR_PACK { get; set; }
        public List<FileUploadModels> FILE_DATA { get; set; }

        public List<LC_LOGISTIC_CONTAINER> LC_LOGISTIC_CONTAINER { get; set; }
    }


    public class LC_LOGISTIC_CONTAINER
    {
        public string LC_LOGISTIC_CON_CODE { get; set; }
        public string LOGISTIC_PLAN_CODE { get; set; }
        public string INVOICE_CONTAINER { get; set; }
        //public string LOGISTIC_CODE { get; set; }
        public string LC_TRACK_NO { get; set; }
        public string INVOICE_NO { get; set; }
        public string CONTAINER_CODE { get; set; }
        public string LOAD_TYPE { get; set; }
        public string DETENTION_DAYS { get; set; }
        public string DEFAULT_AMOUNT { get; set; }
        public string PER_DAY_CHRAGE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY { get; set; }
        public string LAST_MODIFIED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CARRIER_NUMBER { get; set; }
    }
    public class FileUploadModels
    {
        public int SNO { get; set; }
        public string DOC_NAME { get; set; }
        public string DOC_ACTION { get; set; }
        public string FILE_URL { get; set; }
        public DateTime? DOC_DATE { get; set; }
        public DateTime? DOC_PREPARED_DATE { get; set; }
        public DateTime? DOC_EST_RECEIVED_DATE { get; set; }
        public DateTime? DOC_RECEIVED_DATE { get; set; }
        public DateTime? DOC_EST_SUBMITTED_DATE { get; set; }
        public DateTime? DOC_SUBMITTED_DATE { get; set; }
        public string DOCUMENT_CODE { get; set; }
    }

    public class FileLIstModels
    {
        List<string> LogisticCode { get; set; }
    }

    public class LogisticItemModels
    {
        public DateTime INVOICE_DATE { get; set; }
        public string INVOICE_NUMBER { get; set; }
        public string LC_NUMBER { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public decimal QUANTITY { get; set; }
        public decimal AMOUNT { get; set; }
        public int LC_TRACK_NO { get; set; }

    }

    public class Logistic
    {
        public Logistic()
        {
            LOGISTIC_DETAILS = new List<LogisticsModels>();
            FILE_DATA = new List<FileUploadModels>();
        }
        public int LC_TRACK_NO { get; set; }
        public string INVOICE_NO { get; set; }
        public int LOGISTIC_CODE { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public List<LogisticsModels> LOGISTIC_DETAILS { get; set; }
        public List<FileUploadModels> FILE_DATA { get; set; }
       
    }
    public class Logisticlocationdetail {
       
        public string DES_ETA { get; set; }
        public string DES_ETD { get; set; }
        public string DES_ATA { get; set; }
        public string DES_ATD { get; set; }
        public string DES_ETD_NEXT_DES { get; set; }

        //public DateTime DES_ETA { get; set; }
        //public DateTime DES_ETD { get; set; }
        //public DateTime DES_ATA { get; set; }
        //public DateTime DES_ATD { get; set; }
        //public DateTime DES_ETD_NEXT_DES { get; set; }
        public string Result { get; set; }
    }
    public class LcLogisticContainerModel
    {
        public string LC_LOGISTIC_CON_CODE { get; set; }
        // public string LOGISTIC_PLAN_CODE{ get; set; }
        public string LC_TRACK_NO { get; set; }
        public string INVOICE_NO { get; set; }
        public string CONTAINER_CODE { get; set; }
        public string LOAD_TYPE { get; set; }
        public string DETENTION_DAYS { get; set; }
        public string DEFAULT_AMOUNT { get; set; }
        public string PER_DAY_CHRAGE { get; set; }
        public string INVOICE_CONTAINER { get; set; }
        public string CARRIER_NUMBER { get; set; }
        public string SHIPMENT_TYPE { get; set; }
    }
    public class RemoveLogisticImages
    {
        public int SNO { get; set; }
        public int LC_TRACK_NO { get; set; }
        public string INVOICE_NO { get; set; }
    }

    public class LogisticsHistoryModel
    {
        public int LOGISTIC_CODE { get; set; }
        public int LC_TRACK_NO { get; set; }

        public int SNO { get; set; }
        public DateTime? INVOICE_DATE { get; set; }
        public string INVOICE_NO { get; set; }
        public string CONTRACTER_CODE { get; set; }
        public string CONTRACTER_ADDRESS { get; set; }
        public string JOB_ORDER_NO { get; set; }
        public string LOAD_TYPE { get; set; }
        public decimal? CONTRACT_AMOUNT { get; set; }
        public string SHIPPER_NAME { get; set; }
        public string SHIPPER_ADDRESS { get; set; }
        public DateTime? CONTRACT_DATE { get; set; }
        public int FROM_LOCATION_CODE { get; set; }
        public int TO_LOCATION_CODE { get; set; }
        public string SHIPMENT_TYPE { get; set; }
        public DateTime? SRC_ETA { get; set; }
        public DateTime? SRC_ETD { get; set; }
        public DateTime? SRC_ATA { get; set; }
        public DateTime? SRC_ATD { get; set; }
        public DateTime? SRC_ETD_DES { get; set; }
        public DateTime? DES_ETA { get; set; }
        public DateTime? DES_ETD { get; set; }
        public DateTime? DES_ATA { get; set; }
        public DateTime? DES_ATD { get; set; }
        public DateTime? DES_ETD_NEXT_DES { get; set; }
        public decimal? AIR_GROSS_WEIGHT { get; set; }
        public decimal? AIR_CHARGEABLE_WEIGHT { get; set; }

        public string AIR_AWB_NO { get; set; }
        public decimal? AIR_AWB_DATE { get; set; }
        public decimal? SEA_GROSS_WEIGHT { get; set; }
        public decimal? SEA_CBM { get; set; }
        public string SEA_BL_NO { get; set; }

        public DateTime? SEA_BL_DATE { get; set; }
        public string SEA_VESSEL_NO { get; set; }
        public string TRAIN_RR_NO { get; set; }

        public DateTime? TRAIN_RR_DATE { get; set; }
        public string ROAD_LR_NO { get; set; }
        public DateTime? ROAD_LR_DATE { get; set; }
        public string ROAD_TRUCK_NO { get; set; }
        public string ROAD_TRANSPORTER_NAME { get; set; }

        public int CONTAINER_CODE { get; set; }
        public int ISSUING_CARRIER_CODE { get; set; }
        public string REMARKS { get; set; }
        public int CAGENT_CODE { get; set; }
        public int AIR_PACK { get; set; }
        public DateTime? DOC_REC_SUP_DATE { get; set; }
        public DateTime? DOC_END_BANK_DATE { get; set; }
        public DateTime? DOC_SEND_TOAGENT_DATE { get; set; }
        public int POSTAGE_CN { get; set; }
        public DateTime? PORT_CLEAR_DATE { get; set; }


        public string CREATED_BY_EDESC { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY_EDESC { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }

    }

    public class LogisticsDocumentHistoryModel
    {
        public int LOGISTIC_CODE { get; set; }
        public int SNO { get; set; }
        public int LC_TRACK_NO { get; set; }
        public string INVOICE_NO { get; set; }
        public string DOCUMENT_CODE { get; set; }

        public DateTime? DOCUMENT_DATE { get; set; }
        public string FILE_URL { get; set; }
        public string DOC_ACTION { get; set; }
        public DateTime? DOC_PREPARE_DATE { get; set; }
        public DateTime? EST_RECIEVED_DATE { get; set; }
        public DateTime? RECIEVED_DATE { get; set; }
        public DateTime? EST_SUBMIT_DATE { get; set; }
        public DateTime? SUBMITTED_DATE { get; set; }
        public string CREATED_BY_EDESC { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY_EDESC { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }

    }
    public class LogisticsContainerHistoryModel
    {
        public int LC_LOGISTIC_CON_CODE { get; set; }
        public int INVOICE_CONTAINER { get; set; }
        public int LC_TRACK_NO { get; set; }
        public string INVOICE_NO { get; set; }
        public int CONTAINER_CODE { get; set; }
        public string LOAD_TYPE { get; set; }
        public int DETENTION_DAYS { get; set; }
        public decimal? DEFAULT_AMOUNT { get; set; }
        public decimal? PER_DAY_CHRAGE { get; set; }
        public string SHIPMENT_TYPE { get; set; }
        public int LC_LOGISTIC_DETAIL_CODE { get; set; }
        public int SNO { get; set; }
        public string CARRIER_NUMBER { get; set; }
        public string CREATED_BY_EDESC { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string LAST_MODIFIED_BY_EDESC { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }


    }
}