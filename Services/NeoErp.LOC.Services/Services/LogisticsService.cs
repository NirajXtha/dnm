using NeoErp.LOC.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.LOC.Services.Models;
using NeoErp.Data;
using NeoErp.Core;

namespace NeoErp.LOC.Services.Services
{
    public class LogisticsService : ILogisticsService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public LogisticsService(IDbContext dbContext, IWorkContext workcontext)
        {
            this._dbContext = dbContext;
            this._workcontext = workcontext;
        }

        public void CreateLogistics(LogisticsModels details, out string sno, out string cinumber)
        {
            if (details.LOGISTIC_CODE == 0)
            {
                var maxlogisticQuery = $@"SELECT COALESCE(MAX(LOGISTIC_CODE)+1,1) FROM LC_LOGISTIC_DETAIL";
                int maxlogisticCode = _dbContext.SqlQuery<int>(maxlogisticQuery).FirstOrDefault();
                //var insertintoLC_LOGISTIC_DETAILS = $@"INSERT INTO LC_LOGISTIC_DETAIL";
                var insertintoLC_LOGISTIC_DETAILS = $@"INSERT INTO LC_LOGISTIC_DETAIL(LOGISTIC_CODE,LC_TRACK_NO,INVOICE_NO,CONTRACTER_CODE,CAGENT_CODE,CONTRACTER_ADDRESS,JOB_ORDER_NO,LOAD_TYPE,CONTRACT_AMOUNT,SHIPPER_NAME,SHIPPER_ADDRESS,CONTRACT_DATE,FROM_LOCATION_CODE,TO_LOCATION_CODE,SHIPMENT_TYPE,  SRC_ETA,SRC_ETD ,SRC_ATA,SRC_ATD,SRC_ETD_DES,DES_ETA,DES_ETD,DES_ATA,DES_ATD,DES_ETD_NEXT_DES,DOC_REC_SUP_DATE,DOC_END_BANK_DATE,DOC_SEND_TOAGENT_DATE,PORT_CLEAR_DATE,POSTAGE_CN,AIR_GROSS_WEIGHT,AIR_CHARGEABLE_WEIGHT,AIR_AWB_NO,AIR_AWB_DATE,SEA_GROSS_WEIGHT,SEA_CBM,SEA_BL_NO,SEA_BL_DATE,SEA_VESSEL_NO,TRAIN_RR_NO,TRAIN_RR_DATE,ROAD_LR_NO,ROAD_LR_DATE,ROAD_TRUCK_NO,ROAD_TRANSPORTER_NAME,ROAD_TRANSPORTER_ADDRESS,CONTAINER_CODE,ISSUING_CARRIER_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REMARKS,AIR_PACK) 
                                                                                VALUES('{maxlogisticCode}',TO_CHAR('{details.LC_TRACK_NO}'),'{details.INVOICE_NO}','{details.CONTRACTER_CODE}','{details.AGENT_CODE}','{details.CONTRACTER_ADDRESS}','{details.JOB_ORDER_NO}', '{details.LOAD_TYPE}',TO_CHAR('{details.CONTRACT_AMOUNT}'),'{details.SHIPPER_NAME}','{details.SHIPPER_ADDRESS}',TO_DATE('{details.CONTRACT_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{details.FROM_LOCATION}','{details.TO_LOCATION}','{details.SHIPMENT_TYPE}',
                                                                                TO_DATE('{details.SRC_ETA}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.SRC_ETD}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.SRC_ATA}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.SRC_ATD}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.SRC_ETD_DES}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.DES_ETA}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.DES_ETD}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.DES_ATA}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.DES_ATD}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.DES_ETD_NEXT_DES}','MM/dd/yyyy HH:MI:SS AM'),

                                                                                TO_DATE('{details.DOC_REC_SUP_DATE}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.DOC_END_BANK_DATE}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.DOC_SEND_TOAGENT_DATE}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                TO_DATE('{details.PORT_CLEAR_DATE}','MM/dd/yyyy HH:MI:SS AM'),
                                                                                '{details.POSTAGE_CN}',
                 '{details.AIR_GROSS_WEIGHT}','{details.AIR_CHARGEABLE_WEIGHT}','{details.AIR_AWB_NO}', TO_DATE('{details.AIR_AWB_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{details.SEA_GROSS_WEIGHT}','{details.SEA_CBM}','{details.SEA_BL_NO}',TO_DATE('{details.SEA_BL_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{details.SEA_VESSEL_NO}','{details.TRAIN_RR_NO}', TO_DATE('{details.TRAIN_RR_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{details.ROAD_LR_NO}',TO_DATE('{details.ROAD_LR_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{details.ROAD_TRUCK_NO}','{details.ROAD_TRANSPORTER_NAME}','{details.ROAD_TRANSPORTER_ADDRESS}','{details.CONTAINER_CODE}','{details.ISSUING_CARRIER_CODE}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}','{details.REMARKS}','{details.AIR_PACK}')";
               var lc_logistic = _dbContext.ExecuteSqlCommand(insertintoLC_LOGISTIC_DETAILS);

                #region INSERT LC_LOGISTIC_CONTAINER


                if ((details.SHIPMENT_TYPE).Trim() != "AIR")
                {

                    var MAX_SNO_QUERY = $@"SELECT COALESCE(MAX(SNO)+1,1) FROM LC_LOGISTIC_CONTAINER 
                                                        WHERE LC_TRACK_NO='{details.LC_TRACK_NO}' AND INVOICE_NO='{details.INVOICE_NO}'";
                    var SNO = _dbContext.SqlQuery<int>(MAX_SNO_QUERY).FirstOrDefault();

                    foreach (var Record in details.LC_LOGISTIC_CONTAINER)
                    {
                        var MAX_LC_LOGISTIC_CON_CODE_QUERY = $@"SELECT COALESCE(MAX(LC_LOGISTIC_CON_CODE)+1,1) FROM LC_LOGISTIC_CONTAINER";
                        int LC_LOGISTIC_CON_CODE = _dbContext.SqlQuery<int>(MAX_LC_LOGISTIC_CON_CODE_QUERY).FirstOrDefault();

                        if (Record.LOAD_TYPE == "--Select Load Type--")
                        {
                            Record.LOAD_TYPE = "";
                        }
                        var companycode = _workcontext.CurrentUserinformation.company_code;
                        var branchcode = _workcontext.CurrentUserinformation.branch_code;

                        var LC_LOGISTIC_CONTAINER_sqlquery = $@"INSERT INTO LC_LOGISTIC_CONTAINER(LC_LOGISTIC_CON_CODE,INVOICE_CONTAINER,CARRIER_NUMBER,LC_TRACK_NO,INVOICE_NO,CONTAINER_CODE,LOAD_TYPE,DETENTION_DAYS,      
                       DEFAULT_AMOUNT,PER_DAY_CHRAGE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,SHIPMENT_TYPE,SNO,LC_LOGISTIC_DETAIL_CODE)
                       VALUES('{LC_LOGISTIC_CON_CODE}','{Record.INVOICE_CONTAINER}','{Record.CARRIER_NUMBER}','{details.LC_TRACK_NO}','{details.INVOICE_NO}','{Record.CONTAINER_CODE}','{Record.LOAD_TYPE}','{Record.DETENTION_DAYS}'
                              ,'{Record.DEFAULT_AMOUNT}','{Record.PER_DAY_CHRAGE}','{companycode}','{branchcode}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{details.SHIPMENT_TYPE}','{SNO}','{maxlogisticCode}') ";
                        _dbContext.ExecuteSqlCommand(LC_LOGISTIC_CONTAINER_sqlquery);

                    }
                }
                #endregion

                bool check = CheckIfLcTrackNoExist(details.LC_TRACK_NO, details.INVOICE_NO);
                if (check == false)
                {
                    insertIntoLc_Logistic(details, maxlogisticCode);
                }

                var maxsnoquery = $@"SELECT COALESCE(MAX(SNO)+1,1) FROM LC_LOGISTIC_DOCS";
                int maxSno = _dbContext.SqlQuery<int>(maxsnoquery).FirstOrDefault();
                string serialno = maxSno.ToString();
                foreach (var item in details.FILE_DATA)
                {
                    var insertintoLC_LOGISTIC_DOCS = $@"INSERT INTO LC_LOGISTIC_DOCS(SNO,LOGISTIC_CODE,LC_TRACK_NO,INVOICE_NO,DOCUMENT_CODE,DOCUMENT_DATE,DOC_ACTION,DOC_PREPARE_DATE,EST_RECIEVED_DATE,RECIEVED_DATE,EST_SUBMIT_DATE,SUBMITTED_DATE,COMPANY_CODE, BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{maxSno}','{maxlogisticCode}','{details.LC_TRACK_NO}','{details.INVOICE_NO}', '{item.DOC_NAME}',TO_DATE('{item.DOC_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{item.DOC_ACTION}',TO_DATE('{item.DOC_PREPARED_DATE}','MM/dd/yyyy HH:MI:SS AM'),TO_DATE('{item.DOC_EST_RECEIVED_DATE}','MM/dd/yyyy HH:MI:SS AM'),TO_DATE('{item.DOC_RECEIVED_DATE}','MM/dd/yyyy HH:MI:SS AM'),TO_DATE('{item.DOC_EST_SUBMITTED_DATE}','MM/dd/yyyy HH:MI:SS AM'),TO_DATE('{item.DOC_SUBMITTED_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
                    var lc_logistic_docs = _dbContext.ExecuteSqlCommand(insertintoLC_LOGISTIC_DOCS);
                    maxSno++;
                }
                sno = serialno;
                cinumber = details.INVOICE_NO;
            }
            else
            {
                //var updateLC_LOGISTIC_DETAILS = "";

                var updateLC_LOGISTIC_DETAILS = $@"UPDATE LC_LOGISTIC_DETAIL  SET AIR_PACK='{details.AIR_PACK}',CAGENT_CODE='{details.AGENT_CODE}', CONTRACTER_CODE = '{details.CONTRACTER_CODE}', CONTRACTER_ADDRESS = '{details.CONTRACTER_ADDRESS}',JOB_ORDER_NO = '{details.JOB_ORDER_NO}', LOAD_TYPE = '{details.LOAD_TYPE}', CONTRACT_AMOUNT = '{details.CONTRACT_AMOUNT}', SHIPPER_NAME = '{details.SHIPPER_NAME}',SHIPPER_ADDRESS='{details.SHIPPER_ADDRESS}', CONTRACT_DATE = TO_DATE('{details.CONTRACT_DATE}','MM/dd/yyyy HH:MI:SS AM'), FROM_LOCATION_CODE = '{details.FROM_LOCATION}',TO_LOCATION_CODE= '{details.TO_LOCATION}',SHIPMENT_TYPE = '{details.SHIPMENT_TYPE}', SRC_ETA = TO_DATE('{details.SRC_ETA}','MM/dd/yyyy HH:MI:SS AM'), SRC_ETD = TO_DATE('{details.SRC_ETD}','MM/dd/yyyy HH:MI:SS AM'), SRC_ATA =TO_DATE('{details.SRC_ATA}','MM/dd/yyyy HH:MI:SS AM'),SRC_ATD =TO_DATE('{details.SRC_ATD}','MM/dd/yyyy HH:MI:SS AM'),SRC_ETD_DES =TO_DATE('{details.SRC_ETD_DES}','MM/dd/yyyy HH:MI:SS AM'),DES_ETA =TO_DATE('{details.DES_ETA}','MM/dd/yyyy HH:MI:SS AM'),DES_ETD =TO_DATE('{details.DES_ETD}','MM/dd/yyyy HH:MI:SS AM'),DES_ATA =TO_DATE('{details.DES_ATA}','MM/dd/yyyy HH:MI:SS AM'),DES_ATD =TO_DATE('{details.DES_ATD}','MM/dd/yyyy HH:MI:SS AM'),DES_ETD_NEXT_DES =TO_DATE('{details.DES_ETD_NEXT_DES}','MM/dd/yyyy HH:MI:SS AM'),
DOC_REC_SUP_DATE =TO_DATE('{details.DOC_REC_SUP_DATE}','MM/dd/yyyy HH:MI:SS AM'),
DOC_END_BANK_DATE =TO_DATE('{details.DOC_END_BANK_DATE}','MM/dd/yyyy HH:MI:SS AM'),
DOC_SEND_TOAGENT_DATE =TO_DATE('{details.DOC_SEND_TOAGENT_DATE}','MM/dd/yyyy HH:MI:SS AM'),
PORT_CLEAR_DATE =TO_DATE('{details.PORT_CLEAR_DATE}','MM/dd/yyyy HH:MI:SS AM'),
POSTAGE_CN ='{details.POSTAGE_CN}',

AIR_GROSS_WEIGHT = '{details.AIR_GROSS_WEIGHT}', AIR_CHARGEABLE_WEIGHT = '{details.AIR_CHARGEABLE_WEIGHT}', AIR_AWB_NO = '{details.AIR_AWB_NO}', AIR_AWB_DATE = TO_DATE('{details.AIR_AWB_DATE}','MM/dd/yyyy HH:MI:SS AM'), SEA_GROSS_WEIGHT = '{details.SEA_GROSS_WEIGHT}', SEA_CBM = '{details.SEA_CBM}',SEA_BL_NO='{details.SEA_BL_NO}',SEA_BL_DATE=TO_DATE('{details.SEA_BL_DATE}','MM/dd/yyyy HH:MI:SS AM'),SEA_VESSEL_NO = '{details.SEA_VESSEL_NO}',TRAIN_RR_NO='{details.TRAIN_RR_NO}',TRAIN_RR_DATE=TO_DATE('{details.TRAIN_RR_DATE}','MM/dd/yyyy HH:MI:SS AM'),ROAD_LR_NO='{details.ROAD_LR_NO}',ROAD_LR_DATE=TO_DATE('{details.ROAD_LR_DATE}','MM/dd/yyyy HH:MI:SS AM'),ROAD_TRUCK_NO='{details.ROAD_TRUCK_NO}',ROAD_TRANSPORTER_NAME='{details.ROAD_TRANSPORTER_NAME}',ROAD_TRANSPORTER_ADDRESS='{details.ROAD_TRANSPORTER_ADDRESS}',CONTAINER_CODE='{details.CONTAINER_CODE}',ISSUING_CARRIER_CODE='{details.ISSUING_CARRIER_CODE}',COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LC_TRACK_NO = '{details.LC_TRACK_NO}' AND INVOICE_NO = '{details.INVOICE_NO}' AND LOGISTIC_CODE = '{details.LOGISTIC_CODE}'";
                var lc_logistic = _dbContext.ExecuteSqlCommand(updateLC_LOGISTIC_DETAILS);

                int maxSno = 0;
                var maxsnoquery = $@"SELECT COALESCE(MAX(SNO)+1,1) FROM LC_LOGISTIC_DOCS";
                maxSno = _dbContext.SqlQuery<int>(maxsnoquery).FirstOrDefault();

                #region INSERT LC_LOGISTIC_CONTAINER


                if (details.SHIPMENT_TYPE != "AIR" || details.SHIPMENT_TYPE != "")
                {
                    foreach (var Record in details.LC_LOGISTIC_CONTAINER)
                    {
                        if (Record.CONTAINER_CODE == null)
                        {
                            Record.CONTAINER_CODE = "";
                        }
                        if (Record.LOAD_TYPE == null || Record.LOAD_TYPE == "--Select Load Type--")
                        {
                            Record.LOAD_TYPE = "";
                        }
                        var updateintoLC_LOGISTIC_CONTAINER = $@"UPDATE LC_LOGISTIC_CONTAINER SET 
                        CONTAINER_CODE ='{Record.CONTAINER_CODE}',
                        LOAD_TYPE ='{Record.LOAD_TYPE}',
                        CARRIER_NUMBER='{Record.CARRIER_NUMBER}',
                        DETENTION_DAYS ='{Record.DETENTION_DAYS}',
                        DEFAULT_AMOUNT ='{Record.DEFAULT_AMOUNT}',
                        PER_DAY_CHRAGE ='{Record.PER_DAY_CHRAGE}'
                        WHERE LC_TRACK_NO = '{details.LC_TRACK_NO}' AND INVOICE_NO = '{details.INVOICE_NO}' AND LC_LOGISTIC_DETAIL_CODE = '{details.LOGISTIC_CODE}' AND LC_LOGISTIC_CON_CODE ='{Record.LC_LOGISTIC_CON_CODE}' AND INVOICE_CONTAINER='{Record.INVOICE_CONTAINER}' ";
                        _dbContext.ExecuteSqlCommand(updateintoLC_LOGISTIC_CONTAINER);
                    }
                }
                #endregion 

                foreach (var item in details.FILE_DATA)
                {
                    var sqlquery = $@"SELECT LOGISTIC_CODE,SNO,LC_TRACK_NO FROM LC_LOGISTIC_DOCS WHERE LOGISTIC_CODE = '{details.LOGISTIC_CODE}' AND LC_TRACK_NO = '{details.LC_TRACK_NO}' AND SNO = '{item.SNO}' AND DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                    var fileExist = _dbContext.SqlQuery<FileUploadModels>(sqlquery).ToList();
                    if (fileExist.Count > 0)
                    {
                        var updateintoLC_LOGISTIC_DOCS = $@"UPDATE LC_LOGISTIC_DOCS SET DOCUMENT_CODE = '{item.DOC_NAME}',DOCUMENT_DATE=TO_DATE('{item.DOC_DATE}','MM/dd/yyyy HH:MI:SS AM'),DOC_ACTION = '{item.DOC_ACTION}',DOC_PREPARE_DATE = TO_DATE('{item.DOC_PREPARED_DATE}','MM/dd/yyyy HH:MI:SS AM'),EST_RECIEVED_DATE=TO_DATE('{item.DOC_EST_RECEIVED_DATE}','MM/dd/yyyy HH:MI:SS AM'),RECIEVED_DATE=TO_DATE('{item.DOC_RECEIVED_DATE}','MM/dd/yyyy HH:MI:SS AM'),EST_SUBMIT_DATE=TO_DATE('{item.DOC_EST_SUBMITTED_DATE}','MM/dd/yyyy HH:MI:SS AM'),SUBMITTED_DATE=TO_DATE('{item.DOC_SUBMITTED_DATE}','MM/dd/yyyy HH:MI:SS AM'),COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}', BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LOGISTIC_CODE = '{details.LOGISTIC_CODE}' AND LC_TRACK_NO = '{details.LC_TRACK_NO}' AND SNO = '{item.SNO}'";
                        var lc_logistic_docs = _dbContext.ExecuteSqlCommand(updateintoLC_LOGISTIC_DOCS);
                    }
                    else
                    {
                        int symbolno = maxSno;
                        var insertintoLC_LOGISTIC_DOCS = $@"INSERT INTO LC_LOGISTIC_DOCS(SNO,LOGISTIC_CODE,LC_TRACK_NO,INVOICE_NO,DOCUMENT_CODE,DOCUMENT_DATE,DOC_ACTION,DOC_PREPARE_DATE,EST_RECIEVED_DATE,RECIEVED_DATE,EST_SUBMIT_DATE,SUBMITTED_DATE,COMPANY_CODE, BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{symbolno}','{details.LOGISTIC_CODE}','{details.LC_TRACK_NO}','{details.INVOICE_NO}', '{item.DOC_NAME}',TO_DATE('{item.DOC_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{item.DOC_ACTION}',TO_DATE('{item.DOC_PREPARED_DATE}','MM/dd/yyyy HH:MI:SS AM'),TO_DATE('{item.DOC_EST_RECEIVED_DATE}','MM/dd/yyyy HH:MI:SS AM'),TO_DATE('{item.DOC_RECEIVED_DATE}','MM/dd/yyyy HH:MI:SS AM'),TO_DATE('{item.DOC_EST_SUBMITTED_DATE}','MM/dd/yyyy HH:MI:SS AM'),TO_DATE('{item.DOC_SUBMITTED_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
                        var lc_logistic_docs = _dbContext.ExecuteSqlCommand(insertintoLC_LOGISTIC_DOCS);
                        symbolno++;
                    }
                }
                sno = maxSno.ToString();
                cinumber = details.INVOICE_NO;
            }
        }

        #region Private functions Started
        private bool CheckIfLcTrackNoExist(int lctrackno, string invoiceno)
        {
            var sqlquery = $@"SELECT LC_TRACK_NO  FROM LC_LOGISTIC WHERE LC_TRACK_NO ='{lctrackno}' AND INVOICE_NO = '{invoiceno}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var locationlist = _dbContext.SqlQuery<int?>(sqlquery).FirstOrDefault();
            if (locationlist == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void insertIntoLc_Logistic(LogisticsModels details, int maxlogisticCode)
        {

            var insertintoLC_LOGISTIC = $@"INSERT INTO LC_LOGISTIC(LC_TRACK_NO,LOGISTIC_CODE,INVOICE_NO,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                                           VALUES('{details.LC_TRACK_NO}','{maxlogisticCode}','{details.INVOICE_NO}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
            var lc_logistic_detail = _dbContext.ExecuteSqlCommand(insertintoLC_LOGISTIC);



        }


        #endregion Private functions


        public List<DocumentModels> GetAllDocuments()
        {
            var sqlquery = $@"SELECT DOCUMENT_CODE,DOCUMENT_EDESC,REMARKS FROM LC_DOCUMENT_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var locationlist = _dbContext.SqlQuery<DocumentModels>(sqlquery).ToList();
            return locationlist;
        }

        public List<LocationModels> GetAllLocations()
        {
            var sqlquery = $@"SELECT LOCATION_CODE,LOCATION_ID,LOCATION_EDESC,MAX_STORING_DAYS,PER_DAY_CHARGE,CURRENCY_CODE FROM LC_LOCATION_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var locationlist = _dbContext.SqlQuery<LocationModels>(sqlquery).ToList();
            return locationlist;
        }


        public List<LC_ContractorModel> GetAllContractor()
        {
            var sqlquery = $@"SELECT TO_CHAR(LC.CONTRACTOR_CODE) AS CONTRACTOR_CODE,LC.CONTRACTOR_EDESC FROM LC_CONTRACTOR LC WHERE LC.DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var contractorList = _dbContext.SqlQuery<LC_ContractorModel>(sqlquery).ToList();
            return contractorList;
        }
        public List<LC_ClearingAgentModel> GetAllClearingAgent()
        {
            var sqlquery = $@"SELECT TO_CHAR(LC.CAGENT_CODE) AS CAGENT_CODE,LC.CAGENT_EDESC FROM LC_CLEARING_AGENT LC WHERE LC.DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var clearingAgentList = _dbContext.SqlQuery<LC_ClearingAgentModel>(sqlquery).ToList();
            return clearingAgentList;
        }

        public List<LcLogisticContainerModel> GetLogisticPlanContainerDetailByShipmentType(string shipmentType, string InvoiceNo, string create_edit, string Logistic_Detail_Code)
        {
            List<LcLogisticContainerModel> LcLogisticContainerModelRecord = new List<LcLogisticContainerModel>();
            var lot_query = $@"SELECT TO_CHAR(L.LOT_NUMBER) AS LOT_NO, LC_TRACK_NO as LC_TRACK_NO  FROM LC_INVOICE_CONTAINER L WHERE INVOICE_CODE='{InvoiceNo}'GROUP BY LOT_NUMBER, LC_TRACK_NO ";
            var Record = _dbContext.SqlQuery<CommercialInvoiceModel>(lot_query).FirstOrDefault();


            var INVOICE_CODE_QUERY = $@"SELECT Distinct TO_CHAR(Invoice_Number) as INVOICE_CODE FROM LC_INVOICE WHERE INVOICE_CODE ='{InvoiceNo}'";
            var INVOICE_NUMBER = _dbContext.SqlQuery<string>(INVOICE_CODE_QUERY).FirstOrDefault();


            if (create_edit == "edit")
            {
                var editsqlquery = $@" SELECT TO_CHAR(SHIPMENT_TYPE) AS SHIPMENT_TYPE,TO_CHAR(C.LC_LOGISTIC_CON_CODE) AS LC_LOGISTIC_CON_CODE,TO_CHAR(INVOICE_NO) AS INVOICE_NO, TO_CHAR(INVOICE_CONTAINER) AS INVOICE_CONTAINER,TO_CHAR(CARRIER_NUMBER) AS CARRIER_NUMBER ,TO_CHAR(C.LC_TRACK_NO) AS LC_TRACK_NO, TO_CHAR(C.CONTAINER_CODE) AS CONTAINER_CODE,TO_CHAR(C.LOAD_TYPE) AS LOAD_TYPE,TO_CHAR(C.DETENTION_DAYS) AS DETENTION_DAYS,TO_CHAR(C.DEFAULT_AMOUNT) AS DEFAULT_AMOUNT, TO_CHAR(PER_DAY_CHRAGE) AS PER_DAY_CHRAGE FROM LC_LOGISTIC_CONTAINER C
                                      WHERE  C.LC_TRACK_NO='{Record.LC_TRACK_NO}' AND C.INVOICE_NO='{INVOICE_NUMBER}' AND  C.LC_LOGISTIC_DETAIL_CODE ='{Logistic_Detail_Code}'AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                LcLogisticContainerModelRecord = _dbContext.SqlQuery<LcLogisticContainerModel>(editsqlquery).ToList();
            }
            else if (create_edit == "create")
            {
                //  var createsqlquery = $@"SELECT  TO_CHAR(C.LOGISTIC_PLAN_CODE) AS LOGISTIC_PLAN_CODE ,  TO_CHAR(C.LC_TRACK_NO) AS LC_TRACK_NO, TO_CHAR(C.CONTAINER_CODE) AS CONTAINER_CODE,C.LOAD_TYPE, '' AS DETENTION_DAYS,'' AS DEFAULT_AMOUNT,'' AS PER_DAY_CHRAGE,'12359' AS INVOICE_NO  FROM LC_INVOICE_CONTAINER C WHERE C.DELETED_FLAG='N' AND shipment_type='{shipmentType}' AND LOT_NO='{Record.LOT_NO}' AND LC_TRACK_NO= '{Record.LC_TRACK_NO}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var createsqlquery = $@"SELECT  TO_CHAR(SHIPMENT_TYPE) AS SHIPMENT_TYPE,TO_CHAR(INVOICE_CODE) AS INVOICE_NO,TO_CHAR(INVOICE_CONTAINER) AS INVOICE_CONTAINER,TO_CHAR(CARRIER_NUMBER) AS CARRIER_NUMBER,TO_CHAR(C.LC_TRACK_NO) AS LC_TRACK_NO, TO_CHAR(C.CARRIER_CODE) AS CONTAINER_CODE,C.LOAD_TYPE, '' AS DETENTION_DAYS,'' AS DEFAULT_AMOUNT,'' AS PER_DAY_CHRAGE FROM LC_INVOICE_CONTAINER C WHERE C.DELETED_FLAG='N' AND LC_TRACK_NO= '{Record.LC_TRACK_NO}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND INVOICE_CODE='{InvoiceNo}'";
                LcLogisticContainerModelRecord = _dbContext.SqlQuery<LcLogisticContainerModel>(createsqlquery).ToList();
            }
            return LcLogisticContainerModelRecord;

        }

        public List<IssuingCarrierModels> GetAllIssuingCarrier()
        {
            var sqlquery = $@"SELECT CARRIER_CODE,CARRIER_EDESC,CARRIER_ADDRESS1,CARRIER_ADDRESS2,CARRIER_PHONE,REMARKS FROM LC_ISSUING_CARRIER WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var iclist = _dbContext.SqlQuery<IssuingCarrierModels>(sqlquery).ToList();
            return iclist;
        }
        public List<ContainerModels> GetAllContainer()
        {
            var sqlquery = $@"SELECT CONTAINER_CODE,CONTAINER_EDESC,CONTAINER_NO,CONTAINER_SIZE,REMARKS FROM LC_CONTAINER WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var clist = _dbContext.SqlQuery<ContainerModels>(sqlquery).ToList();
            return clist;
        }

        public LogisticsModels GetAllLogisticDetails(string lctrackno, string invoiceno)
        {
            var sqlquery = $@"
                            SELECT LC_TRACK_NO, LOGISTIC_CODE,INVOICE_NO,CONSIGNEE_NAME,CONSIGNEE_ADDRESS,ISSUING_CARRIER_CODE,CONTAINER_CODE,NOTIFY_APPLICANT_NAME,NOTIFY_APPLICANT_ADDRESS,LOAD_TYPE,
                            ,EST_DAY,AIR_GROSS_WEIGHT,AIR_CHARGEABLE_WEIGHT,AIR_AWB_NO,AIR_AWB_DATE,SEA_GROSS_WEIGHT,SEA_CBM,SEA_BL_NO,SEA_BL_DATE,SEA_VESSEL_NO,TRAIN_RR_NO,TRAIN_RR_DATE,ROAD_LR_NO,
                            ROAD_LR_DATE,ROAD_TRUCK_NO,ROAD_TRANSPORTER_NAME,ROAD_TRANSPORTER_ADDRESS,REMARKS
                            FROM LC_LOGISTIC_DETAIL WHERE LC_TRACK_NO = '{lctrackno}' AND INVOICE_NO = '{invoiceno}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var logisticlist = _dbContext.SqlQuery<LogisticsModels>(sqlquery).FirstOrDefault();
            return logisticlist;
        }


        public Logisticlocationdetail GetLogisticETAByInvLocationCode(string invoiceno, string locationcode)
        {
            var count = 0;

            Logisticlocationdetail Record = new Logisticlocationdetail();

            var Existsqlquery = $@"SELECT COUNT(*) FROM  lc_logistic_detail WHERE INVOICE_NO IN 
             (select DISTINCT INVOICE_NUMBER from lc_invoice WHERE INVOICE_CODE = '{invoiceno}')  AND FROM_LOCATION_CODE = '{locationcode}'";
            var Existcount = _dbContext.SqlQuery<int>(Existsqlquery).FirstOrDefault();
            if (Existcount > 0)
            {
                Record.Result = "Exist"; 
                return Record;
            }
            var sqlquery = $@"SELECT COUNT(*) FROM  lc_logistic_detail WHERE INVOICE_NO IN (select DISTINCT INVOICE_NUMBER from lc_invoice  WHERE INVOICE_CODE = '{invoiceno}')  AND TO_LOCATION_CODE='{locationcode}'";
            count = _dbContext.SqlQuery<int>(sqlquery).FirstOrDefault();
            if (count > 0)
            {
                var ETAsqlquery = $@"SELECT TO_CHAR(D.DES_ATA,'MM/DD/yyyy') AS DES_ATA,TO_CHAR(D.DES_ATD,'MM/DD/yyyy') AS DES_ATD,TO_CHAR(D.DES_ETA,'MM/DD/yyyy') AS DES_ETA,TO_CHAR(D.DES_ETD,'MM/DD/yyyy') AS DES_ETD,TO_CHAR(D.DES_ETD_NEXT_DES,'MM/DD/yyyy') AS DES_ETD_NEXT_DES  FROM lc_logistic_detail D
                                     WHERE INVOICE_NO IN (select DISTINCT INVOICE_NUMBER from lc_invoice  WHERE INVOICE_CODE = '{invoiceno}')  AND TO_LOCATION_CODE='{locationcode}'";
                Record = _dbContext.SqlQuery<Logisticlocationdetail>(ETAsqlquery).FirstOrDefault();
            }
                return Record;
        }

        public string CheckLogisticTolocation(string invoiceno, string locationcode)
        {
             var Result = "";
            
            var Existsqlquery = $@"SELECT COUNT(*) FROM  lc_logistic_detail WHERE INVOICE_NO IN 
             (select DISTINCT INVOICE_NUMBER from lc_invoice WHERE INVOICE_CODE = '{invoiceno}')  AND TO_LOCATION_CODE = '{locationcode}'";
            var Existcount = _dbContext.SqlQuery<int>(Existsqlquery).FirstOrDefault();
            if (Existcount > 0)
            {
                Result = "Exist";
            }
            return Result;
        }

        public void UploadFiles(string path, int serialno, string lctrackno, string cinumber)
        {
            var updatelcLogistic_Docs = $@"UPDATE LC_LOGISTIC_DOCS SET FILE_URL='{path}' WHERE LC_TRACK_NO='{lctrackno}' AND SNO = '{serialno}' AND INVOICE_NO = '{cinumber}'";
            var updatepurchaseorder = _dbContext.ExecuteSqlCommand(updatelcLogistic_Docs);
        }
        public List<Logistic> GetAllLogistics()
        {
            try
            {
                var sqlquery1 = $@"select LL.LC_TRACK_NO,LL.INVOICE_NO,LL.LOGISTIC_CODE,SA.LOGIN_EDESC AS CREATED_BY,LL.CREATED_DATE FROM LC_LOGISTIC LL
                                LEFT JOIN SC_APPLICATION_USERS SA ON LL.CREATED_BY = SA.USER_NO AND LL.COMPANY_CODE = SA.COMPANY_CODE
                                 WHERE LL.DELETED_FLAG='N' AND LL.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY LL.LC_TRACK_NO ASC";
                var result1 = _dbContext.SqlQuery<Logistic>(sqlquery1).ToList();

                foreach (var items in result1)
                {
                    var sqlquery2 = $@"SELECT LLD.LOGISTIC_CODE,LLD.LC_TRACK_NO,LLD.INVOICE_NO,TO_CHAR(LLD.SRC_ETA) AS SRC_ETA,TO_CHAR(LLD.SRC_ATA) AS SRC_ATA,TO_CHAR(LLD.SRC_ETD) AS SRC_ETD,TO_CHAR(LLD.SRC_ATD) AS SRC_ATD,TO_CHAR(LLD.SRC_ETD_DES) as SRC_ETD_DES,TO_CHAR(LLD.DES_ETA) as DES_ETA,
                           TO_CHAR(LLD.DES_ETD) as DES_ETD,TO_CHAR(LLD.DES_ATA) as DES_ATA,TO_CHAR(LLD.DES_ATD) as DES_ATD,TO_CHAR(LLD.DES_ETD_NEXT_DES) as DES_ETD_NEXT_DES,
                           LC_CON.CONTRACTOR_EDESC AS CONTRACTER_NAME, LLD.CONTRACTER_ADDRESS,
                           LLD.CONTRACT_DATE ,LLD.JOB_ORDER_NO, LLD.LOAD_TYPE,TO_CHAR(COALESCE(LLD.CONTRACT_AMOUNT,0.00)) AS CONTRACT_AMOUNT,LLD.SHIPPER_NAME,LLD.SHIPPER_ADDRESS,LLD.SHIPMENT_TYPE,LLSF.LOCATION_EDESC 
                           AS FROM_LOCATION,LLST.LOCATION_EDESC AS TO_LOCATION,TO_CHAR(COALESCE(LLD.AIR_PACK,0.00)) AS AIR_PACK,LLD.AIR_GROSS_WEIGHT,LLD.AIR_CHARGEABLE_WEIGHT,LLD.AIR_AWB_NO,LLD.AIR_AWB_DATE,LLD.SEA_GROSS_WEIGHT,LLD.SEA_BL_NO,LLD.SEA_CBM,LLD.SEA_BL_DATE,LLD.SEA_VESSEL_NO,
                           LLD.TRAIN_RR_NO,LLD.TRAIN_RR_DATE,LLD.ROAD_LR_NO, TO_CHAR(LLD.ROAD_LR_DATE),LLD.ROAD_TRUCK_NO,LLD.ROAD_TRANSPORTER_NAME,LLD.ROAD_TRANSPORTER_ADDRESS
                           FROM lc_logistic_detail  LLD
                           LEFT JOIN LC_LOGISTIC LL ON LLD.LC_TRACK_NO = LL.LC_TRACK_NO AND LLD.INVOICE_NO = LL.INVOICE_NO
                           LEFT JOIN LC_LOCATION_SETUP LLSF ON LLSF.LOCATION_CODE=LLD.FROM_LOCATION_CODE 
                           LEFT JOIN LC_CONTRACTOR LC_CON ON LC_CON.CONTRACTOR_CODE =LLD.CONTRACTER_CODE
                           JOIN LC_LOCATION_SETUP LLST ON LLST.LOCATION_CODE=LLD.TO_LOCATION_CODE  WHERE LLD.LC_TRACK_NO='{items.LC_TRACK_NO}' AND LLD.INVOICE_NO = '{items.INVOICE_NO}' AND LLD.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                           ORDER BY LLD.SHIPMENT_TYPE";
                    var result2 = _dbContext.SqlQuery<LogisticsModels>(sqlquery2).ToList();
                    items.LOGISTIC_DETAILS.AddRange(result2);

                    var queryforuploadedfiles = $@"SELECT SNO,LC_TRACK_NO,INVOICE_NO,DOCUMENT_CODE,DOCUMENT_DATE,FILE_URL,DOC_ACTION,DOC_PREPARE_DATE,EST_RECIEVED_DATE FROM LC_LOGISTIC_DOCS WHERE LOGISTIC_CODE = '{items.LOGISTIC_CODE}' AND LC_TRACK_NO ='{items.LC_TRACK_NO}' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                    var resultforuploadedfiles = _dbContext.SqlQuery<FileUploadModels>(queryforuploadedfiles).ToList();
                    items.FILE_DATA.AddRange(resultforuploadedfiles);
                }

                return result1;
            }

            catch (Exception ex)
            {
                throw;
            }
        }


        public List<Logistic> getAllLogisticFilter(string invoicenumber)
        {
            List<Logistic> record = new List<Logistic>();
            if (string.IsNullOrEmpty(invoicenumber))
            {
                return record;
            }
            try
            {
                var sqlquery1 = $@"select LL.LC_TRACK_NO,LL.INVOICE_NO,LL.LOGISTIC_CODE,SA.LOGIN_EDESC AS CREATED_BY,LL.CREATED_DATE FROM LC_LOGISTIC LL
                                LEFT JOIN SC_APPLICATION_USERS SA ON LL.CREATED_BY = SA.USER_NO AND LL.COMPANY_CODE = SA.COMPANY_CODE
                                 WHERE LL.DELETED_FLAG='N' AND LL.INVOICE_NO LIKE'%{invoicenumber}%' AND LL.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY LL.LC_TRACK_NO ASC";
                var result1 = _dbContext.SqlQuery<Logistic>(sqlquery1).ToList();

                foreach (var items in result1)
                {
                    var sqlquery2 = $@"SELECT LLD.LOGISTIC_CODE,LLD.LC_TRACK_NO,LLD.INVOICE_NO,LLD.ETA,LLD.ATA,LLD.EDD,LLD.A_DD,LC_CON.CONTRACTOR_EDESC AS CONTRACTER_NAME, LLD.CONTRACTER_ADDRESS,
                    LLD.CONTRACT_DATE ,LLD.JOB_ORDER_NO, LLD.LOAD_TYPE, LLD.CONTRACT_AMOUNT,LLD.SHIPPER_NAME,LLD.SHIPPER_ADDRESS,LLD.SHIPMENT_TYPE,LLSF.LOCATION_EDESC 
                    AS FROM_LOCATION,LLST.LOCATION_EDESC AS TO_LOCATION,TO_CHAR(COALESCE(LLD.AIR_PACK,0.00)) AS AIR_PACK,LLD.AIR_GROSS_WEIGHT,LLD.AIR_CHARGEABLE_WEIGHT,LLD.AIR_AWB_NO,LLD.AIR_AWB_DATE,LLD.SEA_GROSS_WEIGHT,LLD.SEA_BL_NO,LLD.SEA_CBM,LLD.SEA_BL_DATE,LLD.SEA_VESSEL_NO,
                    LLD.TRAIN_RR_NO,LLD.TRAIN_RR_DATE,LLD.ROAD_LR_NO,LLD.ROAD_LR_DATE,LLD.ROAD_TRUCK_NO,LLD.ROAD_TRANSPORTER_NAME,LLD.ROAD_TRANSPORTER_ADDRESS
                      FROM lc_logistic_detail  LLD
                        JOIN LC_LOGISTIC LL ON LLD.LC_TRACK_NO = LL.LC_TRACK_NO AND LLD.INVOICE_NO = LL.INVOICE_NO
                        JOIN LC_LOCATION_SETUP LLSF ON LLSF.LOCATION_CODE=LLD.FROM_LOCATION_CODE 
                       LEFT JOIN LC_CONTRACTOR LC_CON ON LC_CON.CONTRACTOR_CODE =LLD.CONTRACTER_CODE
                        JOIN LC_LOCATION_SETUP LLST ON LLST.LOCATION_CODE=LLD.TO_LOCATION_CODE  WHERE LLD.LC_TRACK_NO='{items.LC_TRACK_NO}' AND LLD.INVOICE_NO = '{items.INVOICE_NO}' AND LLD.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                        ORDER BY LLD.SHIPMENT_TYPE";
                    var result2 = _dbContext.SqlQuery<LogisticsModels>(sqlquery2).ToList();
                    items.LOGISTIC_DETAILS.AddRange(result2);

                    var queryforuploadedfiles = $@"SELECT SNO,LC_TRACK_NO,INVOICE_NO,DOCUMENT_CODE,DOCUMENT_DATE,FILE_URL,DOC_ACTION,DOC_PREPARE_DATE,EST_RECIEVED_DATE FROM LC_LOGISTIC_DOCS WHERE LOGISTIC_CODE = '{items.LOGISTIC_CODE}' AND LC_TRACK_NO ='{items.LC_TRACK_NO}' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                    var resultforuploadedfiles = _dbContext.SqlQuery<FileUploadModels>(queryforuploadedfiles).ToList();
                    items.FILE_DATA.AddRange(resultforuploadedfiles);
                }

                return result1;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        public List<LogisticsModels> GetAlLLogisticShipmentLists(string filter, string invoice)
        {

            var sqlquery = $@"SELECT LLD.LOGISTIC_CODE,LLD.LC_TRACK_NO,LLD.INVOICE_NO,TO_CHAR(LLD.SRC_ETA) AS SRC_ETA,TO_CHAR(LLD.SRC_ATA) AS SRC_ATA,TO_CHAR(LLD.SRC_ETD) AS SRC_ETD,TO_CHAR(LLD.SRC_ATD) AS SRC_ATD,
                            TO_CHAR(LLD.DES_ETA) AS DES_ETA,TO_CHAR(LLD.DES_ETD) AS DES_ETD,TO_CHAR(LLD.DES_ATA) AS DES_ATA,TO_CHAR(LLD.DES_ATD) AS DES_ATD,TO_CHAR(LLD.DES_ETD_NEXT_DES) AS DES_ETD_NEXT_DES,LC_CON.CONTRACTOR_EDESC AS CONTRACTER_NAME, LLD.CONTRACTER_ADDRESS,
                            LLD.CONTRACT_DATE ,LLD.JOB_ORDER_NO, LLD.LOAD_TYPE,TO_CHAR(COALESCE(LLD.CONTRACT_AMOUNT,0.00)) AS CONTRACT_AMOUNT,LLD.SHIPPER_NAME,LLD.SHIPPER_ADDRESS,LLD.SHIPMENT_TYPE,LLSF.LOCATION_EDESC 
                            AS FROM_LOCATION,LLST.LOCATION_EDESC AS TO_LOCATION,LLD.AIR_GROSS_WEIGHT,LLD.AIR_CHARGEABLE_WEIGHT,LLD.AIR_AWB_NO,LLD.AIR_AWB_DATE,LLD.SEA_GROSS_WEIGHT,LLD.SEA_BL_NO,LLD.SEA_CBM,LLD.SEA_BL_DATE,LLD.SEA_VESSEL_NO,
                            LLD.TRAIN_RR_NO,LLD.TRAIN_RR_DATE,LLD.ROAD_LR_NO,TO_CHAR(LLD.ROAD_LR_DATE),LLD.ROAD_TRUCK_NO,LLD.ROAD_TRANSPORTER_NAME,LLD.ROAD_TRANSPORTER_ADDRESS,TO_CHAR(COALESCE(LLD.AIR_PACK,0.00)) AS AIR_PACK 
                            FROM lc_logistic_detail  LLD
                            JOIN LC_LOGISTIC LL ON LLD.LC_TRACK_NO = LL.LC_TRACK_NO AND LLD.INVOICE_NO = LL.INVOICE_NO
                            LEFT JOIN LC_LOCATION_SETUP LLSF ON LLSF.LOCATION_CODE=LLD.FROM_LOCATION_CODE 
                            LEFT JOIN LC_LOCATION_SETUP LLST ON LLST.LOCATION_CODE=LLD.TO_LOCATION_CODE
                            LEFT JOIN LC_CONTRACTOR LC_CON ON LC_CON.CONTRACTOR_CODE =LLD.CONTRACTER_CODE
                            WHERE LLD.LC_TRACK_NO='{filter}' AND LLD.INVOICE_NO = '{invoice}' AND LLD.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                            ORDER BY LLD.SHIPMENT_TYPE,LOGISTIC_CODE";
            var result = _dbContext.SqlQuery<LogisticsModels>(sqlquery).ToList();

            foreach (var item in result)
            {
                LogisticsModels lg = new LogisticsModels();

                var queryforuploadedfiles = $@"SELECT SNO,LC_TRACK_NO,INVOICE_NO,DOCUMENT_CODE,DOCUMENT_DATE,FILE_URL,DOC_ACTION,DOC_PREPARE_DATE,EST_RECIEVED_DATE FROM LC_LOGISTIC_DOCS WHERE LOGISTIC_CODE = '{item.LOGISTIC_CODE}' AND LC_TRACK_NO ='{filter}' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var resultforuploadedfiles = _dbContext.SqlQuery<FileUploadModels>(queryforuploadedfiles).ToList();
                item.FILE_DATA.AddRange(resultforuploadedfiles);
            }

            return result;
        }
        public LogisticsModels GetAllLogisticShipmentDetailsByLogisticCode(string lctrackno, string invoiceno, string logisticcode)
        {
            var sqlquery = $@"select DISTINCT D. LOGISTIC_CODE,D.LC_TRACK_NO,D.INVOICE_NO,D.INVOICE_NO, TO_CHAR(LC.INVOICE_CODE) AS PINVOICE_NO ,COALESCE(CAGENT_CODE,0) as AGENT_CODE,CONTRACTER_CODE,CONTRACTER_ADDRESS,JOB_ORDER_NO,LOAD_TYPE,
                            TO_CHAR(COALESCE(CONTRACT_AMOUNT,0.00)) AS CONTRACT_AMOUNT,CONTRACT_DATE,SHIPPER_NAME,SHIPPER_ADDRESS,CONTRACT_DATE,FROM_LOCATION_CODE,TO_LOCATION_CODE,
                            SHIPMENT_TYPE,
                            TO_CHAR(SRC_ETA,'MM/dd/yyyy') AS SRC_ETA,
                            TO_CHAR(SRC_ETD,'MM/dd/yyyy') AS SRC_ETD,
                            TO_CHAR(SRC_ATA,'MM/dd/yyyy') AS SRC_ATA,
                            TO_CHAR(SRC_ATD,'MM/dd/yyyy') AS SRC_ATD,
                            TO_CHAR(SRC_ETD_DES,'MM/dd/yyyy') AS SRC_ETD_DES,
                            TO_CHAR(DES_ETA,'MM/dd/yyyy') AS DES_ETA,
                            TO_CHAR(DES_ETD,'MM/dd/yyyy') AS DES_ETD,
                            TO_CHAR(DES_ATA,'MM/dd/yyyy') AS DES_ATA,
                            TO_CHAR(DES_ATD,'MM/dd/yyyy') AS DES_ATD,
                            TO_CHAR(DES_ETD_NEXT_DES,'MM/dd/yyyy') AS DES_ETD_NEXT_DES,
                            TO_CHAR(DOC_REC_SUP_DATE,'MM/dd/yyyy') AS DOC_REC_SUP_DATE,
                            TO_CHAR(DOC_END_BANK_DATE,'MM/dd/yyyy') AS DOC_END_BANK_DATE,
                            TO_CHAR(DOC_SEND_TOAGENT_DATE,'MM/dd/yyyy') AS DOC_SEND_TOAGENT_DATE,
                            TO_CHAR(PORT_CLEAR_DATE,'MM/dd/yyyy') AS PORT_CLEAR_DATE,
                            TO_CHAR(POSTAGE_CN) AS POSTAGE_CN,
                            TO_CHAR(COALESCE(AIR_PACK,0.00)) AS AIR_PACK,AIR_GROSS_WEIGHT,AIR_CHARGEABLE_WEIGHT,AIR_AWB_NO,AIR_AWB_DATE,
                          SEA_GROSS_WEIGHT,SEA_CBM,SEA_BL_NO,SEA_BL_DATE,SEA_VESSEL_NO,TRAIN_RR_NO,TRAIN_RR_DATE,ROAD_LR_NO,
                          TO_CHAR(ROAD_LR_DATE,'MM/dd/yyyy') AS ROAD_LR_DATE
,ROAD_TRUCK_NO,ROAD_TRANSPORTER_NAME,ROAD_TRANSPORTER_ADDRESS,CONTAINER_CODE,
                         ISSUING_CARRIER_CODE,REMARKS FROM LC_LOGISTIC_DETAIL D
                         INNER JOIN LC_INVOICE  LC ON LC.LC_TRACK_NO =D.LC_TRACK_NO AND LC.INVOICE_NUMBER =D.INVOICE_NO
                         where D.LOGISTIC_CODE= '{logisticcode}' and D.LC_TRACK_NO= '{lctrackno}' and D.INVOICE_NO='{invoiceno}' AND D.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var detaillist = _dbContext.SqlQuery<LogisticsModels>(sqlquery).FirstOrDefault();
            var queryforuploadedfiles = $@"SELECT SNO,LC_TRACK_NO,INVOICE_NO,DOCUMENT_CODE,DOCUMENT_DATE as DOC_DATE,FILE_URL,DOC_ACTION,DOC_PREPARE_DATE as DOC_PREPARED_DATE,EST_RECIEVED_DATE as DOC_EST_RECEIVED_DATE,RECIEVED_DATE as DOC_RECEIVED_DATE,EST_SUBMIT_DATE as DOC_EST_SUBMITTED_DATE,SUBMITTED_DATE as DOC_SUBMITTED_DATE FROM LC_LOGISTIC_DOCS WHERE LOGISTIC_CODE = '{detaillist.LOGISTIC_CODE}' AND LC_TRACK_NO ='{detaillist.LC_TRACK_NO}' AND DELETED_FLAG= 'N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var resultforuploadedfiles = _dbContext.SqlQuery<FileUploadModels>(queryforuploadedfiles).ToList();
            detaillist.FILE_DATA.AddRange(resultforuploadedfiles);
            return detaillist;

        }

        public void RemoveLogisticImages(RemoveLogisticImages imageremovedetails)
        {
            var updateLC_LOGISTIC_DOCS = $@"UPDATE LC_LOGISTIC_DOCS SET DELETED_FLAG = 'Y' WHERE SNO = '{imageremovedetails.SNO}' AND INVOICE_NO = '{imageremovedetails.INVOICE_NO}' AND LC_TRACK_NO = '{imageremovedetails.LC_TRACK_NO}'";
            _dbContext.ExecuteSqlCommand(updateLC_LOGISTIC_DOCS);

        }

        public List<LogisticsHistoryModel> getAllLogisticsHistoryList(string lctrackno)
        {
            var sqlquery = $@"SELECT ALI.LC_TRACK_NO
	                                ,ALI.INVOICE_NO
	                                ,ALI.SRC_ETA
	                                ,ALI.SRC_ETD
	                                ,ALI.SRC_ATA
	                                ,ALI.SRC_ATD
	                                ,ALI.SRC_ETD_DES
	                                ,ALI.DES_ETA
	                                ,ALI.DES_ETD
	                                ,ALI.DES_ATA
	                                ,ALI.DES_ATD
	                                ,ALI.DES_ETD_NEXT_DES
	                                ,ALI.CREATED_DATE
	                                ,ALI.CREATED_BY
	                                ,SAU.LOGIN_EDESC AS CREATED_BY_EDESC
	                                ,ALI.LAST_MODIFIED_BY
	                                ,SA.LOGIN_EDESC AS LAST_MODIFIED_BY_EDESC
	                                ,ALI.LAST_MODIFIED_DATE AS LAST_MODIFIED_DATE
                                FROM AUD_LC_LOGISTIC_DETAIL ALI
                                LEFT JOIN SC_APPLICATION_USERS SAU ON ALI.CREATED_BY = SAU.USER_NO
	                                AND ALI.COMPANY_CODE = SAU.COMPANY_CODE
                                LEFT JOIN SC_APPLICATION_USERS SA ON ALI.LAST_MODIFIED_BY = SA.USER_NO
	                                AND ALI.COMPANY_CODE = SA.COMPANY_CODE
                                WHERE ALI.LC_TRACK_NO = '{lctrackno}'
	                                AND ALI.DELETED_FLAG = 'N'
	                                AND ALI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                                ORDER BY ALI.LC_TRACK_NO ASC";

            var result = _dbContext.SqlQuery<LogisticsHistoryModel>(sqlquery).ToList();
            return result;
        }
        public List<LogisticsContainerHistoryModel> getAllLogisticsContainerHistoryList(string lctrackno)
        {
            var sqlquery = $@"SELECT ALI.LC_TRACK_NO
	                                ,ALI.INVOICE_NO
	                                ,ALI.CREATED_DATE
	                                ,ALI.CREATED_BY
	                                ,SAU.LOGIN_EDESC AS CREATED_BY_EDESC
	                                --  ,ALI.LAST_MODIFIED_BY
	                                ,SA.LOGIN_EDESC AS LAST_MODIFIED_BY_EDESC
                                --  ,ALI.LAST_MODIFIED_DATE AS LAST_MODIFIED_DATE
                                FROM AUD_LC_LOGISTIC_CONTAINER ALI
                                LEFT JOIN SC_APPLICATION_USERS SAU ON ALI.CREATED_BY = SAU.USER_NO
	                                AND ALI.COMPANY_CODE = SAU.COMPANY_CODE
                                LEFT JOIN SC_APPLICATION_USERS SA ON ALI.COMPANY_CODE = SA.COMPANY_CODE
                                -- ALI.LAST_MODIFIED_BY = SA.USER_NO AND ALI.COMPANY_CODE = SA.COMPANY_CODE
                                WHERE ALI.LC_TRACK_NO = '{lctrackno}'
	                                AND ALI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                                ORDER BY ALI.LC_TRACK_NO ASC";

            var result = _dbContext.SqlQuery<LogisticsContainerHistoryModel>(sqlquery).ToList();
            return result;
        }
        public List<LogisticsDocumentHistoryModel> getAllLogisticsDocumentHistoryList(string lctrackno)
        {
            var sqlquery = $@"SELECT ALI.LC_TRACK_NO
	                                ,ALI.INVOICE_NO
	                                ,ALI.CREATED_DATE
	                                ,ALI.CREATED_BY
	                                ,SAU.LOGIN_EDESC AS CREATED_BY_EDESC
	                                ,ALI.LAST_MODIFIED_BY
	                                ,SA.LOGIN_EDESC AS LAST_MODIFIED_BY_EDESC
	                                ,ALI.LAST_MODIFIED_DATE AS LAST_MODIFIED_DATE
                                FROM AUD_LC_LOGISTIC_DOCS ALI
                                LEFT JOIN SC_APPLICATION_USERS SAU ON ALI.CREATED_BY = SAU.USER_NO
	                                AND ALI.COMPANY_CODE = SAU.COMPANY_CODE
                                LEFT JOIN SC_APPLICATION_USERS SA ON ALI.LAST_MODIFIED_BY = SA.USER_NO
	                                AND ALI.COMPANY_CODE = SA.COMPANY_CODE
                                    WHERE ALI.LC_TRACK_NO = '{lctrackno}'
	                                AND ALI.DELETED_FLAG = 'N'
	                                AND ALI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                                ORDER BY ALI.LC_TRACK_NO ASC";

            var result = _dbContext.SqlQuery<LogisticsDocumentHistoryModel>(sqlquery).ToList();
            return result;
        }

        public bool IsAir(string PinvoiceCode)
        {
            bool result = false;
            var COMPANY_CODE = _workcontext.CurrentUserinformation.company_code;
            var BRANCH_CODE = _workcontext.CurrentUserinformation.branch_code;
            //var sqlquery = $@"SELECT DISTINCT NVL(LI.IS_AIR,'N')  from LC_INVOICE li where  LI.INVOICE_CODE='{PinvoiceCode}' AND COMPANY_CODE='{COMPANY_CODE}' AND BRANCH_CODE='{BRANCH_CODE}'";
            var sqlquery = $@"SELECT DISTINCT NVL(LI.IS_AIR,'N')  from LC_INVOICE li where  LI.INVOICE_CODE='{PinvoiceCode}' AND COMPANY_CODE='{COMPANY_CODE}'";
            var response = _dbContext.SqlQuery<string>(sqlquery).SingleOrDefault();
            if (response =="Y")
            {
                result = true;
            } 
            return result;
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