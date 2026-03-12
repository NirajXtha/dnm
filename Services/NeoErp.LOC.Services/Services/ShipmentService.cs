using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.LOC.Services.Models;
using NeoErp.Data;
using NeoErp.Core;

namespace NeoErp.LOC.Services.Services
{
    public class ShipmentService : IShipmentService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public ShipmentService(IDbContext dbContext, IWorkContext workcontext)
        {
            _dbContext = dbContext;
            _workcontext = workcontext;
        }

        public void createShipment(ShipmentModels sidetails)
        {

            var insertShipment = "";
            sidetails.AWB_DATE = (sidetails.AWB_DATE == null ? DateTime.Now : sidetails.AWB_DATE);
            sidetails.RR_DATE = (sidetails.AWB_DATE == null ? DateTime.Now : sidetails.AWB_DATE);
            sidetails.LR_DATE = (sidetails.AWB_DATE == null ? DateTime.Now : sidetails.AWB_DATE);
            sidetails.BL_DATE = (sidetails.AWB_DATE == null ? DateTime.Now : sidetails.AWB_DATE);
            string staticColumns = "LC_TRACK_NO,INVOICE_NO,FROM_LOCATION,TO_LOCATION,SHIPMENT_TYPE,LOAD_TYPE,SHIPMENT_STATUS,CONTAINER_SIZE,CONTAINER_NO,EST_DAY,SHIPPER_NAME,SHIPPER_ADDRESS,CONSIGNEE_NAME,CONSIGNEE_ADDRESS,ISSUING_CARRIER_NAME,ISSUING_CARRIER_ADDRESS,NOTIFY_APPLICANT_NAME,NOTIFY_APPLICANT_ADDRESS,REMARKS";
            string staticValues = $@"'{sidetails.LC_TRACK_NO}','{sidetails.INVOICE_NO}','{sidetails.FROM_LOCATION}','{sidetails.TO_LOCATION}','{sidetails.SHIPMENT_TYPE}','{sidetails.LOAD_TYPE}','{sidetails.SHIPMENT_STATUS}','{sidetails.CONTAINER_SIZE}','{sidetails.CONTAINER_NO}','{sidetails.EST_DAY}','{sidetails.SHIPPER_NAME}','{sidetails.SHIPPER_ADDRESS}','{sidetails.CONSIGNEE_NAME}','{sidetails.CONSIGNEE_ADDRESS}','{sidetails.ISSUING_CARRIER_NAME}','{sidetails.ISSUING_CARRIER_ADDRESS}','{sidetails.NOTIFY_APPLICANT_NAME}','{sidetails.NOTIFY_APPLICANT_ADDRESS}','{sidetails.REMARKS}'";
            foreach (var item in sidetails.ITEMLIST)
            {
                if (sidetails.SHIPMENT_TYPE == "AIR")
                {
                    insertShipment = $@"INSERT INTO LC_SHIPMENT ({staticColumns},AWB_NO,AWB_DATE,GROSS_WEIGHT_AIR,CHARGEABLE_WEIGHT_AIR,SNO,ITEM_CODE,QUANTITY)VALUES({staticValues},'{sidetails.AWB_NO}',TO_DATE('{sidetails.AWB_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{sidetails.GROSS_WEIGHT_AIR}','{sidetails.CHARGEABLE_WEIGHT_AIR}','{item.SNO}','{item.ITEM_CODE}','{item.RECEIVED_QUANTITY}')";
                }
                else if (sidetails.SHIPMENT_TYPE == "SEA")
                {
                    insertShipment = $@"INSERT INTO LC_SHIPMENT ({staticColumns},BL_NO,BL_DATE,GROSS_WEIGHT_SEA,CBM_SEA,VESSEL_NO,SNO,ITEM_CODE,QUANTITY)VALUES({staticValues},'{sidetails.BL_NO}',TO_DATE('{sidetails.BL_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{sidetails.GROSS_WEIGHT_SEA}','{sidetails.CBM_SEA}','{sidetails.VESSEL_NO}','{item.SNO}','{item.ITEM_CODE}','{item.RECEIVED_QUANTITY}')";
                }
                else if (sidetails.SHIPMENT_TYPE == "ROAD")
                {
                    insertShipment = $@"INSERT INTO LC_SHIPMENT ({staticColumns},LR_NO,LR_DATE,TRUCK_NO,TRANSPORTER_NAME,TRANSPORTER_ADDRESS,SNO,ITEM_CODE,QUANTITY)VALUES({staticValues},{sidetails.LR_NO},TO_DATE('{sidetails.LR_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{sidetails.TRUCK_NO}','{sidetails.TRANSPORTER_NAME}','{sidetails.TRANSPORTER_ADDRESS}',{item.SNO},{item.ITEM_CODE},{item.RECEIVED_QUANTITY})";
                }
                else if (sidetails.SHIPMENT_TYPE == "TRAIN")
                {
                    insertShipment = $@"INSERT INTO LC_SHIPMENT ({staticColumns},RR_NO,RR_DATE,SNO,ITEM_CODE,QUANTITY)VALUES({staticValues},'{sidetails.RR_NO}',TO_DATE('{sidetails.RR_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{item.SNO}','{item.ITEM_CODE}','{item.RECEIVED_QUANTITY}')";
                }
                var insertship = _dbContext.ExecuteSqlCommand(insertShipment);
            }
          

            #region commented lines
            //var updatetPO = $@"UPDATE LC_SHIPMENT SET SHIPMENT_STATUS = '{sidetails.SHIPMENT_STATUS}',FROM_LOCATION='{sidetails.FROM_LOCATION}',TO_LOCATION='{sidetails.TO_LOCATION}',INVOICE_NO='{sidetails.INVOICE_NO}',SHIPMENT_TYPE='{sidetails.SHIPMENT_TYPE}',LOAD_TYPE='{sidetails.LOAD_TYPE}', 
            //                CONTAINER_SIZE = '{sidetails.CONTAINER_SIZE}',CONTAINER_NO='{sidetails.CONTAINER_NO}',GROSS_WEIGHT_SEA='{sidetails.GROSS_WEIGHT_SEA}', CBM_SEA='{sidetails.CBM_SEA}',GROSS_WEIGHT_AIR='{sidetails.GROSS_WEIGHT_AIR}',CHARGEABLE_WEIGHT_AIR='{sidetails.CHARGEABLE_WEIGHT_AIR}',AWB_NO='{sidetails.AWB_NO}',AWB_DATE=TO_DATE('{sidetails.AWB_DATE}','MM/dd/yyyy HH:MI:SS AM'),BL_NO='{sidetails.BL_NO}',BL_DATE=TO_DATE('{sidetails.BL_DATE}','MM/dd/yyyy HH:MI:SS AM'),RR_NO='{sidetails.RR_NO}',RR_DATE=TO_DATE('{sidetails.RR_DATE}','MM/dd/yyyy HH:MI:SS AM'),LR_NO='{sidetails.LR_NO}',LR_DATE=TO_DATE('{sidetails.LR_DATE}','MM/dd/yyyy HH:MI:SS AM'),EST_DAY='{sidetails.EST_DAY}',VESSEL_NO='{sidetails.VESSEL_NO}',TRUCK_NO='{sidetails.TRUCK_NO}',TRANSPORTER_NAME='{sidetails.TRANSPORTER_NAME}',TRANSPORTER_ADDRESS='{sidetails.TRANSPORTER_ADDRESS}',SHIPPER_NAME='{sidetails.SHIPPER_NAME}',SHIPPER_ADDRESS='{sidetails.SHIPPER_ADDRESS}',CONSIGNEE_NAME='{sidetails.CONSIGNEE_NAME}',CONSIGNEE_ADDRESS='{sidetails.CONSIGNEE_ADDRESS}',ISSUING_CARRIER_NAME='{sidetails.ISSUING_CARRIER_NAME}',ISSUING_CARRIER_ADDRESS='{sidetails.ISSUING_CARRIER_ADDRESS}',NOTIFY_APPLICANT_NAME='{sidetails.NOTIFY_APPLICANT_NAME}',NOTIFY_APPLICANT_ADDRESS='{sidetails.NOTIFY_APPLICANT_ADDRESS}',DETAIL_ENTRY_FLAG='Y',REMARKS='{sidetails.REMARKS}',COMPANY_CODE='{_workcontext.CurrentUserinformation
            //                   .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
            //                   .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LC_TRACK_NO = '{sidetails.LC_TRACK_NO}'";
            //var updatepurchaseorder = _dbContext.ExecuteSqlCommand(updatetPO);
            //}
            //else
            //{
            //        sidetails.AWB_DATE = (sidetails.AWB_DATE == null ? DateTime.Now : sidetails.AWB_DATE);
            //        sidetails.RR_DATE = (sidetails.AWB_DATE == null ? DateTime.Now : sidetails.AWB_DATE);
            //        sidetails.LR_DATE = (sidetails.AWB_DATE == null ? DateTime.Now : sidetails.AWB_DATE);
            //        sidetails.BL_DATE = (sidetails.AWB_DATE == null ? DateTime.Now : sidetails.AWB_DATE);
            //        var updatetPO = $@"UPDATE LC_SHIPMENT SET SHIPMENT_STATUS = '{sidetails.SHIPMENT_STATUS}',FROM_LOCATION='{sidetails.FROM_LOCATION}',TO_LOCATION='{sidetails.TO_LOCATION}',INVOICE_NO='{sidetails.INVOICE_NO}',SHIPMENT_TYPE='{sidetails.SHIPMENT_TYPE}',LOAD_TYPE='{sidetails.LOAD_TYPE}', 
            //                        CONTAINER_SIZE = '{sidetails.CONTAINER_SIZE}',CONTAINER_NO='{sidetails.CONTAINER_NO}',GROSS_WEIGHT_SEA='{sidetails.GROSS_WEIGHT_SEA}', CBM_SEA='{sidetails.CBM_SEA}',GROSS_WEIGHT_AIR='{sidetails.GROSS_WEIGHT_AIR}',CHARGEABLE_WEIGHT_AIR='{sidetails.CHARGEABLE_WEIGHT_AIR}',AWB_NO='{sidetails.AWB_NO}',
            //AWB_DATE=TO_DATE('{sidetails.AWB_DATE.Value.ToShortDateString()}','MM/dd/yyyy'),BL_NO='{sidetails.BL_NO}',BL_DATE=TO_DATE('{sidetails.BL_DATE.Value.ToShortDateString()}','MM/dd/yyyy'),RR_NO='{sidetails.RR_NO}',RR_DATE=TO_DATE('{sidetails.RR_DATE.Value.ToShortDateString()}','MM/dd/yyyy'),LR_NO='{sidetails.LR_NO}',LR_DATE=TO_DATE('{sidetails.LR_DATE.Value.ToShortDateString()}','MM/dd/yyyy'),EST_DAY='{sidetails.EST_DAY}',VESSEL_NO='{sidetails.VESSEL_NO}',TRUCK_NO='{sidetails.TRUCK_NO}',TRANSPORTER_NAME='{sidetails.TRANSPORTER_NAME}',TRANSPORTER_ADDRESS='{sidetails.TRANSPORTER_ADDRESS}',SHIPPER_NAME='{sidetails.SHIPPER_NAME}',SHIPPER_ADDRESS='{sidetails.SHIPPER_ADDRESS}',CONSIGNEE_NAME='{sidetails.CONSIGNEE_NAME}',CONSIGNEE_ADDRESS='{sidetails.CONSIGNEE_ADDRESS}',ISSUING_CARRIER_NAME='{sidetails.ISSUING_CARRIER_NAME}',ISSUING_CARRIER_ADDRESS='{sidetails.ISSUING_CARRIER_ADDRESS}',NOTIFY_APPLICANT_NAME='{sidetails.NOTIFY_APPLICANT_NAME}',NOTIFY_APPLICANT_ADDRESS='{sidetails.NOTIFY_APPLICANT_ADDRESS}',REMARKS='{sidetails.REMARKS}',COMPANY_CODE='{_workcontext.CurrentUserinformation
            //                          .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
            //                          .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE SNO = '{sidetails.SNO}'";
            //        var updatepurchaseorder = _dbContext.ExecuteSqlCommand(updatetPO);
            #endregion





        }

        public List<CommercialInvoiceModel> GetAllLcIpPurchaseOrder(string filter)
        {

            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select DISTINCT INVOICE_CODE,INVOICE_NUMBER,LC_TRACK_NO,CREATED_DATE from LC_INVOICE
                                where UPPER(INVOICE_NUMBER) like '%{ filter.ToUpperInvariant()}%' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY TO_DATE(CREATED_DATE) DESC ";
            var result = _dbContext.SqlQuery<CommercialInvoiceModel>(sqlquery).ToList();
            return result;

        }
        public List<CommercialInvoiceModel> GetAllLcIpPurchaseOrderfilter(string filter)
        {

            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }
           
            var sqlquery = $@"select DISTINCT INVOICE_CODE,INVOICE_NUMBER,LC_TRACK_NO,CREATED_DATE from LC_INVOICE LC
                                where UPPER(INVOICE_NUMBER) like '%{ filter.ToUpperInvariant()}%' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' 
                                AND LC.INVOICE_NUMBER IN (select L.INVOICE_NO from lc_logistic L)
                                ORDER BY TO_DATE(CREATED_DATE) DESC ";
            var result = _dbContext.SqlQuery<CommercialInvoiceModel>(sqlquery).ToList();
            return result;

        }
        

        public void getAllShipmentList()
        {
            throw new NotImplementedException();
        }

        public ShipmentModels getShipmentBySno(int sno, string lctrackno)
        {
            var sqlquery = $@"SELECT LC_TRACK_NO,SNO,ITEM_CODE,QUANTITY,SHIPMENT_STATUS,FROM_LOCATION,TO_LOCATION,INVOICE_NO,SHIPMENT_TYPE,LOAD_TYPE,CONTAINER_SIZE,CONTAINER_NO,
                        GROSS_WEIGHT_SEA,CBM_SEA,GROSS_WEIGHT_AIR,CHARGEABLE_WEIGHT_AIR,AWB_NO,AWB_DATE,BL_NO,BL_DATE,RR_NO,RR_DATE,LR_NO,LR_DATE,EST_DAY,VESSEL_NO,TRUCK_NO,
                        TRANSPORTER_NAME,TRANSPORTER_ADDRESS,SHIPPER_NAME,SHIPPER_ADDRESS,CONSIGNEE_NAME,CONSIGNEE_ADDRESS,ISSUING_CARRIER_NAME,ISSUING_CARRIER_ADDRESS,
                        NOTIFY_APPLICANT_NAME,NOTIFY_APPLICANT_ADDRESS,REMARKS FROM LC_SHIPMENT
                                WHERE SNO = '{sno}' AND LC_TRACK_NO = '{lctrackno}'";

            var result = _dbContext.SqlQuery<ShipmentModels>(sqlquery).FirstOrDefault();
            return result;


        }

        public List<LogisticItemModels> getShipmentlistbyTrackNo(string lcnumber)
        {
            var sqlquery = $@"SELECT LI.LC_TRACK_NO,LI.INVOICE_DATE,LI.INVOICE_NUMBER,LI.LC_NUMBER,LI.ITEM_CODE,IMS.ITEM_EDESC,LI.QUANTITY,LI.AMOUNT FROM 
                LC_INVOICE LI JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE 
                WHERE LI.INVOICE_CODE = '{lcnumber}' AND LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";

            var result = _dbContext.SqlQuery<LogisticItemModels>(sqlquery).ToList();
            return result;
        }

       

    }
}