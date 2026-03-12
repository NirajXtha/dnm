using AutoMapper;
using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels.SettingsEntities;
using NeoErp.Core.MongoDBRepository.Repository;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using NeoErp.Sales.Modules.Services.Models.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Web.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using NeoErp.Sales.Modules.Services.Models.NetSalesReports;
using NeoErp.Sales.Modules.Services.Models.BillwisePurchaseSummary;
using NeoErp.Sales.Modules.Services.Models.NetSalesReport;
using NeoErp.Sales.Modules.Services.Models.NetPurchaseModels;
using NeoErp.Sales.Modules.Services.Models.PurchaseSummaryReport;
using NeoErp.Sales.Modules.Services.Models.Audit;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class SalesRegister : ReportJsonFilterAbstract, ISalesRegister
    {

        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;
        private ISettingService _setting;
        private readonly IMapper _mapper;
        private ICacheManager _cacheManager;
        public SalesRegister(NeoErpCoreEntity objectEntity, IWorkContext workContext, ISettingService service, IMapper mapper, ICacheManager cacheManager)
        {
            this._objectEntity = objectEntity;
            this._workContext = workContext;
            this._setting = service;
            this._mapper = mapper;
            this._cacheManager = cacheManager;
        }

        /// <summary>
        /// For Mobile Api
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<SalesRegisterCustomerModel> SaleRegisterCustomers(string companyCode, string branchCode)
        {
            string query = $@"SELECT DISTINCT INITCAP(CS.CUSTOMER_EDESC)AS CustomerName, CS.CUSTOMER_CODE AS CustomerCode,
            CS.GROUP_SKU_FLAG, CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N'  
            AND CS.GROUP_SKU_FLAG= 'I'   
            AND CS.COMPANY_CODE IN ({companyCode})";

            if (!string.IsNullOrEmpty(branchCode))
            {
                query += $@" AND BRANCH_CODE IN ({branchCode})";
            }
            query += $@" ORDER BY CS.CUSTOMER_EDESC, MASTER_CUSTOMER_CODE, PRE_CUSTOMER_CODE";

            var salesRegisterCustomers = _objectEntity.SqlQuery<SalesRegisterCustomerModel>(query).ToList();
            return salesRegisterCustomers;
        }

        public List<SalesRegisterCustomerModel> SaleRegisterSuppliers(string companyCode, string branchCode)
        {
            string query = $@"SELECT DISTINCT INITCAP(SUPPLIER_EDESC)AS CUSTOMERNAME, SUPPLIER_CODE AS CUSTOMERCODE,
                GROUP_SKU_FLAG,PRE_SUPPLIER_CODE,MASTER_SUPPLIER_CODE
            FROM IP_SUPPLIER_SETUP
            WHERE DELETED_FLAG = 'N'  
            AND GROUP_SKU_FLAG= 'I'   
            AND COMPANY_CODE IN ('{companyCode}')";

            if (!string.IsNullOrEmpty(branchCode))
            {
                query += $@" AND BRANCH_CODE IN ({branchCode})";
            }
            query += $@" ORDER BY SUPPLIER_EDESC, MASTER_SUPPLIER_CODE, PRE_SUPPLIER_CODE";

            var salesRegisterCustomers = _objectEntity.SqlQuery<SalesRegisterCustomerModel>(query).ToList();
            return salesRegisterCustomers;
        }

        public List<SalesRegisterCustomerModel> SaleRegisterCustomers()
        {

            var companyCode = _workContext.CurrentUserinformation.company_code;
            string query = $@"SELECT DISTINCT INITCAP(CS.CUSTOMER_EDESC)AS CustomerName, CS.CUSTOMER_CODE AS CustomerCode,
            CS.GROUP_SKU_FLAG, CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N'            
            AND CS.COMPANY_CODE = '{companyCode}'
            ORDER BY MASTER_CUSTOMER_CODE, PRE_CUSTOMER_CODE";
            var salesRegisterCustomers = _objectEntity.SqlQuery<SalesRegisterCustomerModel>(query).ToList();
            return salesRegisterCustomers;
        }

        public List<SalesRegisterCustomerModel> SaleRegisterGroupCustomers()
        {
            string query = @"SELECT DISTINCT INITCAP(CS.CUSTOMER_EDESC)AS CustomerName, CS.CUSTOMER_CODE AS CustomerCode,
            CS.GROUP_SKU_FLAG, CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N'
            ORDER BY MASTER_CUSTOMER_CODE, PRE_CUSTOMER_CODE";
            var salesRegisterCustomers = _objectEntity.SqlQuery<SalesRegisterCustomerModel>(query).ToList();
            return salesRegisterCustomers;
        }

        public List<SalesRegisterProductModel> SalesRegisterProductsIndividual()
        {
            string query = @"SELECT DISTINCT LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag
            FROM IP_ITEM_MASTER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND GROUP_SKU_FLAG = 'I'      
            START WITH PRE_ITEM_CODE = '00'
            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
            var productListNodes = _objectEntity.SqlQuery<SalesRegisterProductModel>(query).ToList();
            return productListNodes;
        }

        //       public List<SalesRegisterProductModel> GetDistributorItems(User userInfo)
        //       {
        //           //CheckFlag 
        //           string flagQuery = $@"SELECT PO_DISPLAY_DIST_ITEM FROM DIST_PREFERENCE_SETUP WHERE COMPANY_CODE ='{userInfo.company_code}'";
        //           var result = _objectEntity.SqlQuery<SalesRegisterProductModel>(flagQuery).ToList();
        //           //return result;
        //           var itemflag = result[0].PO_DISPLAY_DIST_ITEM;

        //           if (itemflag == "Y")
        //           {
        //               string query = $@"SELECT DISTINCT IT.ITEM_EDESC AS ItemName, IT.ITEM_CODE AS ItemCode
        // FROM IP_ITEM_MASTER_SETUP IT, DIST_DISTRIBUTOR_ITEM DD
        //WHERE DD.ITEM_CODE = IT.ITEM_CODE AND DD.COMPANY_CODE = IT.COMPANY_CODE ORDER BY IT.ITEM_EDESC ASC";
        //               var product = _objectEntity.SqlQuery<SalesRegisterProductModel>(query).ToList();
        //               return product;
        //           }
        //           else
        //           {

        //               string query = @"SELECT DISTINCT LEVEL, 
        //           INITCAP(ITEM_EDESC) AS ItemName,
        //           ITEM_CODE AS ItemCode,
        //           MASTER_ITEM_CODE AS MasterItemCode, 
        //           PRE_ITEM_CODE AS PreItemCode, 
        //           GROUP_SKU_FLAG AS GroupFlag
        //           FROM IP_ITEM_MASTER_SETUP ims
        //           WHERE ims.DELETED_FLAG = 'N' 
        //           AND GROUP_SKU_FLAG = 'I'      
        //           START WITH PRE_ITEM_CODE = '00'
        //           CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE ORDER BY ItemName ASC";

        //               var product = _objectEntity.SqlQuery<SalesRegisterProductModel>(query).ToList();
        //               return product;
        //           }


        //       }

        public List<SalesRegisterProductModel> SalesRegisterProducts()
        {
            string query = @"SELECT DISTINCT LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag
            FROM IP_ITEM_MASTER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N'             
            START WITH PRE_ITEM_CODE = '00'
            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
            var productListNodes = _objectEntity.SqlQuery<SalesRegisterProductModel>(query).ToList();
            return productListNodes;
        }

        public List<SalesRegisterProductModel> SalesRegisterProducts(User userinfo)
        {

            string query = $@"SELECT DISTINCT 
            --LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag,
             CATEGORY_CODE
            FROM IP_ITEM_MASTER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND ims.COMPANY_CODE = '{userinfo.company_code}'           
            --START WITH PRE_ITEM_CODE = '00'
            --CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
            var productListNodes = _objectEntity.SqlQuery<SalesRegisterProductModel>(query).ToList();
            return productListNodes;
        }
        public List<SalesRegisterProductModel> SalesRegisterProductsByCategory(User userinfo, string category)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = @"SELECT LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag
            FROM IP_ITEM_MASTER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND CATEGORY_CODE = '" + category + @"'
            AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
            AND ims.Branch_Code = '" + userinfo.branch_code + @"'
            --AND GROUP_SKU_FLAG = 'G'
            START WITH PRE_ITEM_CODE = '00'
            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
            var productListNodes = _objectEntity.SqlQuery<SalesRegisterProductModel>(query).ToList();
            return productListNodes;
        }

        public List<CategoryModel> GetSalesRegisterItemCategory()
        {
            string query = @"SELECT DISTINCT CATEGORY_CODE AS CategoryCode, 
                                CATEGORY_EDESC AS CategoryName
                                FROM IP_CATEGORY_CODE
                                WHERE DELETED_FLAG = 'N'";
            var categoryList = _objectEntity.SqlQuery<CategoryModel>(query).ToList();
            return categoryList;
        }
        public List<CategoryModel> GetSalesRegisterItemCategory(User userinfo)
        {

            string query = @"SELECT CATEGORY_CODE AS CategoryCode, 
                                CATEGORY_EDESC AS CategoryName
                                FROM IP_CATEGORY_CODE
                                WHERE DELETED_FLAG = 'N'
                                AND COMPANY_CODE = '" + userinfo.company_code + "' ";
            var categoryList = _objectEntity.SqlQuery<CategoryModel>(query).ToList();
            return categoryList;
        }

        public List<PartyTypeModel> GetSalesRegisterPartyTypes()
        {
            string query = @"SELECT DISTINCT PARTY_TYPE_CODE AS PartyTypeCode,
                                 PARTY_TYPE_EDESC AS PartyTypeName
                                 FROM IP_PARTY_TYPE_CODE
                                 WHERE DELETED_FLAG = 'N' and Party_type_flag='D'";
            var partyTypeList = _objectEntity.SqlQuery<PartyTypeModel>(query).ToList();
            return partyTypeList;
        }
        public List<PartyTypeModel> GetSalesRegisterPartyTypes(User userinfo)
        {
            string query = @"SELECT PARTY_TYPE_CODE AS PartyTypeCode,
                                 PARTY_TYPE_EDESC AS PartyTypeName
                                 FROM IP_PARTY_TYPE_CODE
                                 WHERE DELETED_FLAG = 'N'
                                 AND COMPANY_CODE = '" + userinfo.company_code + "' and Party_type_flag='D'";
            var partyTypeList = _objectEntity.SqlQuery<PartyTypeModel>(query).ToList();
            return partyTypeList;
        }

        public List<AreaTypeModel> GetAreaTypes(User userinfo)
        {
            string query = @"SELECT AREA_CODE,
                                 AREA_EDESC
                                 FROM AREA_SETUP
                                 WHERE DELETED_FLAG = 'N'
                                 AND COMPANY_CODE = '" + userinfo.company_code + "' ";
            var areaTypeList = _objectEntity.SqlQuery<AreaTypeModel>(query).ToList();
            return areaTypeList;
        }

        //Query for the Branch 

        public List<BranchModel> getSalesRegisterBranch()
        {
            string query = @"SELECT BRANCH_CODE AS BranchCode ,
                             BRANCH_EDESC AS  BranchName
                             from FA_BRANCH_SETUP
                             WHERE COMPANY_CODE = '01'
                             AND DELETED_FLAG = 'N' ";
            var BranchList = _objectEntity.SqlQuery<BranchModel>(query).ToList();
            return BranchList;
        }
        public List<BranchModel> getSalesRegisterBranch(User userinfo)
        {
            string query = @"SELECT BRANCH_CODE AS BranchCode ,
                             BRANCH_EDESC AS  BranchName
                             from FA_BRANCH_SETUP
                             WHERE COMPANY_CODE = '" + userinfo.company_code + @"'
                             AND DELETED_FLAG = 'N' ";
            var BranchList = _objectEntity.SqlQuery<BranchModel>(query).ToList();
            return BranchList;
        }
        public List<VoucherModel> SalesRegisterVouchers()
        {
            var companyCode = this._workContext.CurrentUserinformation.company_code;
            string query = $@"SELECT 
                            DISTINCT FS.FORM_CODE VoucherCode, 
                            INITCAP(FS.FORM_EDESC) VoucherName
                            FROM FORM_DETAIL_SETUP DS, FORM_SETUP FS
                            WHERE table_name  IN ( 'SA_SALES_INVOICE', 'SA_SALES_RETURN')                           
                            AND FS.DELETED_FLAG = 'N'
                            AND FS.FORM_CODE = DS.FORM_CODE
                            AND FS.COMPANY_CODE = DS.COMPANY_CODE
                            AND FS.COMPANY_CODE = '{companyCode}'
                            ORDER BY INITCAP(FS.FORM_EDESC)";
            var voucherList = _objectEntity.SqlQuery<VoucherModel>(query).ToList();
            return voucherList;
        }

        public List<VoucherSetupModel> GetAllVoucherNodes()
        {
            var companyCode = this._workContext.CurrentUserinformation.company_code;
            string query = $@"SELECT DISTINCT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(FORM_NDESC) AS VoucherName,
                                        FORM_CODE AS VoucherCode,
                                        MASTER_FORM_CODE AS MasterFormCode, 
                                        PRE_FORM_CODE AS PreFormCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM FORM_SETUP fs
                                        WHERE fs.DELETED_FLAG = 'N'                                         
                                        AND GROUP_SKU_FLAG = 'G' 
                                        AND LEVEL = 1 
                                        AND fs.COMPANY_CODE = '{companyCode}'
                                        START WITH PRE_FORM_CODE = '00'
                                        CONNECT BY PRIOR MASTER_FORM_CODE = PRE_FORM_CODE
                                        ORDER SIBLINGS BY FORM_NDESC";
            var VoucherListNodes = _objectEntity.SqlQuery<VoucherSetupModel>(query).ToList();
            return VoucherListNodes;
        }

        public List<VoucherSetupModel> GetVoucherListByFormCode(string level, string masterSupplierCode)
        {
            var companyCode = this._workContext.CurrentUserinformation.company_code;
            string query = string.Format(@"SELECT DISTINCT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(FORM_NDESC) AS VoucherName,
                                        FORM_CODE AS VoucherCode,
                                        MASTER_FORM_CODE AS MasterFormCode, 
                                        PRE_FORM_CODE AS PreFormCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM FORM_SETUP fs
                                        WHERE fs.DELETED_FLAG = 'N'    
                                        AND fs.COMPANY_CODE = '" + companyCode + @"'                                     
                                        --AND GROUP_SKU_FLAG = 'G'
                                        AND LEVEL = {0} 
                                       -- AND fs.FORM_CODE IN 
                                       --   (
                                       --    SELECT DISTINCT FS.FORM_CODE
                                       --       FROM form_detail_setup DS, FORM_SETUP FS
                                       --      WHERE  FS.COMPANY_CODE = '01'
                                       --      AND FS.DELETED_FLAG = 'N'
                                       --      AND table_name IN  ( 'SA_SALES_INVOICE','SA_SALES_RETURN')
                                       --      AND FS.FORM_CODE = DS.FORM_CODE
                                       --      AND FS.COMPANY_CODE = DS.COMPANY_CODE
                                       --   ) 
                                        START WITH PRE_FORM_CODE = {1}
                                        CONNECT BY PRIOR MASTER_FORM_CODE = PRE_FORM_CODE
                                        ORDER SIBLINGS BY FORM_NDESC", level.ToString(), masterSupplierCode.ToString());
            var voucherListNodes = _objectEntity.SqlQuery<VoucherSetupModel>(query).ToList();
            return voucherListNodes;
        }

        public List<SupplierSetupModel> SupplierAllNodes()
        {
            string query = @"SELECT DISTINCT LEVEL, COMPANY_CODE,DELETED_FLAG,
                             INITCAP(SUPPLIER_EDESC) AS SupplierName,
                             SUPPLIER_CODE AS SupplierCode,
                             MASTER_SUPPLIER_CODE AS MasterSupplierCode, 
                             PRE_SUPPLIER_CODE AS PreSupplierCode, 
                             GROUP_SKU_FLAG AS GroupFlag
                             FROM IP_SUPPLIER_SETUP ims
                             WHERE ims.DELETED_FLAG = 'N'                              
                             AND GROUP_SKU_FLAG = 'G'              
                             START WITH PRE_SUPPLIER_CODE = '00'
                             CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE";
            var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            return SupplierListNodes;
        }

        public List<SupplierSetupModel> SupplierAllNodes(User userinfo)
        {
            string query = @"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                             INITCAP(SUPPLIER_EDESC) AS SupplierName,
                             SUPPLIER_CODE AS SupplierCode,
                             MASTER_SUPPLIER_CODE AS MasterSupplierCode, 
                             PRE_SUPPLIER_CODE AS PreSupplierCode, 
                             GROUP_SKU_FLAG AS GroupFlag
                             FROM IP_SUPPLIER_SETUP ims
                             WHERE ims.DELETED_FLAG = 'N' 
                             AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'                             
                             AND GROUP_SKU_FLAG = 'G'              
                             START WITH PRE_SUPPLIER_CODE = '00'
                             CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE";
            var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            return SupplierListNodes;
        }
        public List<SupplierSetupModel> DealerAllNodes(User userinfo)
        {
            //var query = @"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
            //                INITCAP(PARTY_TYPE_EDESC) AS SupplierName,
            //                PARTY_TYPE_CODE AS SupplierCode,
            //                MASTER_PARTY_CODE AS MasterSupplierCode, 
            //                PRE_PARTY_CODE AS PreSupplierCode, 
            //                GROUP_SKU_FLAG AS GroupFlag
            //                FROM IP_PARTY_TYPE_CODE fs
            //                WHERE fs.DELETED_FLAG = 'N' 
            //                AND fs.COMPANY_CODE = '" + userinfo.company_code + @"'  
            //                --AND GROUP_SKU_FLAG = 'G'              
            //                START WITH PRE_PARTY_CODE = '00'
            //                CONNECT BY PRIOR MASTER_PARTY_CODE = PRE_PARTY_CODE";

            //var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            //return SupplierListNodes;
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = @"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                             INITCAP(PARTY_TYPE_EDESC) AS SupplierName,
                             PARTY_TYPE_CODE AS SupplierCode,
                             MASTER_PARTY_CODE AS MasterSupplierCode, 
                             PRE_PARTY_CODE AS PreSupplierCode, 
                             GROUP_SKU_FLAG AS GroupFlag,
                             (SELECT COUNT(*) FROM IP_PARTY_TYPE_CODE WHERE  PRE_PARTY_CODE = IMS.MASTER_PARTY_CODE) as Childrens
                             FROM IP_PARTY_TYPE_CODE ims
                             WHERE ims.DELETED_FLAG = 'N' 
                             AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
                            -- AND ims.branch_code = '" + userinfo.branch_code + @"'
                             AND GROUP_SKU_FLAG = 'G'
                             AND LEVEL='1'           
                             START WITH PRE_PARTY_CODE = '00'
                             CONNECT BY PRIOR MASTER_PARTY_CODE = PRE_PARTY_CODE";
            var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            return SupplierListNodes;
        }

        public List<SupplierSetupModel> SupplierAllNodesGroup()
        {
            string query = @"SELECT SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                             INITCAP(SUPPLIER_EDESC) AS SupplierName,
                             SUPPLIER_CODE AS SupplierCode,
                             MASTER_SUPPLIER_CODE AS MasterSupplierCode, 
                             PRE_SUPPLIER_CODE AS PreSupplierCode, 
                             GROUP_SKU_FLAG AS GroupFlag,
                             (SELECT COUNT(*) FROM IP_SUPPLIER_SETUP WHERE GROUP_SKU_FLAG='G' AND PRE_SUPPLIER_CODE = IMS.MASTER_SUPPLIER_CODE) as Childrens
                             FROM IP_SUPPLIER_SETUP ims
                             WHERE ims.DELETED_FLAG = 'N'                              
                             AND GROUP_SKU_FLAG = 'G'
                             AND LEVEL='1'           
                             START WITH PRE_SUPPLIER_CODE = '00'
                             CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE";
            var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            return SupplierListNodes;
        }

        public List<SupplierSetupModel> SupplierAllNodesGroup(User userinfo)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = @"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                             INITCAP(SUPPLIER_EDESC) AS SupplierName,
                             SUPPLIER_CODE AS SupplierCode,
                             MASTER_SUPPLIER_CODE AS MasterSupplierCode, 
                             PRE_SUPPLIER_CODE AS PreSupplierCode, 
                             GROUP_SKU_FLAG AS GroupFlag,
                             (SELECT COUNT(*) FROM IP_SUPPLIER_SETUP WHERE GROUP_SKU_FLAG='G' AND PRE_SUPPLIER_CODE = IMS.MASTER_SUPPLIER_CODE) as Childrens
                             FROM IP_SUPPLIER_SETUP ims
                             WHERE ims.DELETED_FLAG = 'N' 
                             AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
                             AND ims.branch_code = '" + userinfo.branch_code + @"'
                             AND GROUP_SKU_FLAG = 'G'
                             AND LEVEL='1'           
                             START WITH PRE_SUPPLIER_CODE = '00'
                             CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE";
            var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            return SupplierListNodes;
        }

        public List<SalesRegisterSupplierModel> SalesRegisterSuppliers()
        {
            string query = @"SELECT DISTINCT LEVEL, COMPANY_CODE,DELETED_FLAG,
                            INITCAP(SUPPLIER_EDESC) AS SupplierName,
                            SUPPLIER_CODE AS SupplierCode,
                            MASTER_SUPPLIER_CODE AS MasterItemCode, 
                            PRE_SUPPLIER_CODE AS PreItemCode, 
                            GROUP_SKU_FLAG AS GroupFlag
                            FROM IP_SUPPLIER_SETUP fs
                            WHERE fs.DELETED_FLAG = 'N'                             
                            --AND GROUP_SKU_FLAG = 'G'              
                            START WITH PRE_SUPPLIER_CODE = '00'
                            CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE";
            var SupplierListNodes = _objectEntity.SqlQuery<SalesRegisterSupplierModel>(query).ToList();
            return SupplierListNodes;
        }

        public List<SalesRegisterSupplierModel> SalesRegisterSuppliers(User userinfo)
        {

            string query = @"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                            INITCAP(SUPPLIER_EDESC) AS SupplierName,
                            SUPPLIER_CODE AS SupplierCode,
                            MASTER_SUPPLIER_CODE AS MasterSuplierCode, 
                            PRE_SUPPLIER_CODE AS PreSuplierCode, 
                            GROUP_SKU_FLAG AS GroupFlag
                            FROM IP_SUPPLIER_SETUP fs
                            WHERE fs.DELETED_FLAG = 'N' 
                            AND fs.COMPANY_CODE = '" + userinfo.company_code + @"'
                            --AND GROUP_SKU_FLAG = 'G'              
                            START WITH PRE_SUPPLIER_CODE = '00'
                            CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE";
            var SupplierListNodes = _objectEntity.SqlQuery<SalesRegisterSupplierModel>(query).ToList();
            return SupplierListNodes;
        }
        public List<SalesRegisterSupplierModel> SalesRegisterDealer(User userinfo)
        {
            var query = @"SELECT DISTINCT LEVEL, COMPANY_CODE,DELETED_FLAG,
                            INITCAP(PARTY_TYPE_EDESC) AS SupplierName,
                            PARTY_TYPE_CODE AS SupplierCode,
                            MASTER_PARTY_CODE AS MasterSupplierCode, 
                            PRE_PARTY_CODE AS PreSupplierCode, 
                            GROUP_SKU_FLAG AS GroupFlag
                            FROM IP_PARTY_TYPE_CODE fs
                            WHERE fs.DELETED_FLAG = 'N' 
                            AND fs.COMPANY_CODE = '" + userinfo.company_code + @"'
                            --AND GROUP_SKU_FLAG = 'G'              
                            START WITH PRE_PARTY_CODE = '00'
                            CONNECT BY PRIOR MASTER_PARTY_CODE = PRE_PARTY_CODE";
            var SupplierListNodes = _objectEntity.SqlQuery<SalesRegisterSupplierModel>(query).ToList();
            return SupplierListNodes;
        }


        public List<SalesRegisterSupplierModel> SalesRegisterGroupSuppliers()
        {
            string query = @"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                            INITCAP(SUPPLIER_EDESC) AS SupplierName,
                            SUPPLIER_CODE AS SupplierCode,
                            MASTER_SUPPLIER_CODE AS MasterItemCode, 
                            PRE_SUPPLIER_CODE AS PreItemCode, 
                            GROUP_SKU_FLAG AS GroupFlag
                            FROM IP_SUPPLIER_SETUP fs
                            WHERE fs.DELETED_FLAG = 'N' 
                            AND fs.COMPANY_CODE = '01'
                            AND GROUP_SKU_FLAG = 'G'              
                            START WITH PRE_SUPPLIER_CODE = '00'
                            CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE";
            var SupplierListNodes = _objectEntity.SqlQuery<SalesRegisterSupplierModel>(query).ToList();
            return SupplierListNodes;
        }

        public List<SalesRegisterSupplierModel> SalesRegisterGroupSuppliers(User userinfo)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = @"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                            INITCAP(SUPPLIER_EDESC) AS SupplierName,
                            SUPPLIER_CODE AS SupplierCode,
                            MASTER_SUPPLIER_CODE AS MasterItemCode, 
                            PRE_SUPPLIER_CODE AS PreItemCode, 
                            GROUP_SKU_FLAG AS GroupFlag
                            FROM IP_SUPPLIER_SETUP fs
                            WHERE fs.DELETED_FLAG = 'N' 
                            AND fs.COMPANY_CODE = '" + userinfo.company_code + @"'
                            AND GROUP_SKU_FLAG = 'G'              
                            START WITH PRE_SUPPLIER_CODE = '00'
                            CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE";
            var SupplierListNodes = _objectEntity.SqlQuery<SalesRegisterSupplierModel>(query).ToList();
            return SupplierListNodes;
        }

        public List<CustomerSetupModel> GetGroupCustomerListByCustomerCode(string level, string masterCustomerCode)
        {
            string query = string.Format(@"SELECT DISTINCT LEVEL,INITCAP(CS.CUSTOMER_EDESC) CUSTOMER_EDESC,GROUP_SKU_FLAG,
            CUSTOMER_CODE, CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE,
            (SELECT COUNT(*) FROM SA_CUSTOMER_SETUP WHERE GROUP_SKU_FLAG='G' AND PRE_CUSTOMER_CODE = CS.MASTER_CUSTOMER_CODE) as Childrens
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N'
            AND GROUP_SKU_FLAG='G'            
            AND LEVEL = {0}
            START WITH PRE_CUSTOMER_CODE = '{1}'
            CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
            ORDER SIBLINGS BY CUSTOMER_EDESC", level.ToString(), masterCustomerCode.ToString());
            var customerListNodes = _objectEntity.SqlQuery<CustomerSetupModel>(query).ToList();
            return customerListNodes;
        }

        public List<CustomerSetupModel> GetCustomerListByCustomerCode(string level, string masterCustomerCode)
        {
            string query = string.Format(@"SELECT DISTINCT LEVEL,INITCAP(CS.CUSTOMER_EDESC) CUSTOMER_EDESC,GROUP_SKU_FLAG,
            CUSTOMER_CODE, CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N'            
            AND LEVEL = {0}
            START WITH PRE_CUSTOMER_CODE = '{1}'
            CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
            ORDER SIBLINGS BY CUSTOMER_EDESC", level.ToString(), masterCustomerCode.ToString());
            var customerListNodes = _objectEntity.SqlQuery<CustomerSetupModel>(query).ToList();
            return customerListNodes;
        }

        public List<CustomerSetupModel> GetCustomerListByCustomerCode(string level, string masterCustomerCode, User userinfo)
        {

            string query = string.Format(@"SELECT LEVEL,INITCAP(CS.CUSTOMER_EDESC) CUSTOMER_EDESC,GROUP_SKU_FLAG,
            CUSTOMER_CODE, CS.MASTER_CUSTOMER_CODE, CS.PRE_CUSTOMER_CODE, CS.BRANCH_CODE,
            (SELECT COUNT(*) FROM SA_CUSTOMER_SETUP WHERE  GROUP_SKU_FLAG='G' AND COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_CUSTOMER_CODE = CS.MASTER_CUSTOMER_CODE) as Childrens
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N'
            AND CS.COMPANY_CODE = '{0}'
            AND LEVEL = {1}
            START WITH PRE_CUSTOMER_CODE = '{2}'
            CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
            ORDER SIBLINGS BY CUSTOMER_EDESC", userinfo.company_code, level.ToString(), masterCustomerCode.ToString());
            var customerListNodes = _objectEntity.SqlQuery<CustomerSetupModel>(query).ToList();
            return customerListNodes;
        }
        public List<CustomerSetupModel> CustomerListAllNodes()
        {
            string query = @"SELECT DISTINCT LEVEL,INITCAP(CS.CUSTOMER_EDESC) CUSTOMER_EDESC,CS.CUSTOMER_CODE,
            CS.GROUP_SKU_FLAG,CS.MASTER_CUSTOMER_CODE,CS.PRE_CUSTOMER_CODE, LEVEL
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N'
            AND GROUP_SKU_FLAG = 'G'
            AND LEVEL = 1
            START WITH PRE_CUSTOMER_CODE = '00'
            CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
            ORDER SIBLINGS BY CUSTOMER_EDESC";
            var customerListNodes = _objectEntity.SqlQuery<CustomerSetupModel>(query).ToList();
            return customerListNodes;
        }
        public List<CustomerSetupModel> CustomerListAllNodes(User userinfo)
        {
            string query = @"SELECT LEVEL,INITCAP(CS.CUSTOMER_EDESC) CUSTOMER_EDESC,CS.CUSTOMER_CODE,
            CS.GROUP_SKU_FLAG,CS.MASTER_CUSTOMER_CODE,CS.PRE_CUSTOMER_CODE, CS.BRANCH_CODE, LEVEL,
            (SELECT COUNT(*) FROM SA_CUSTOMER_SETUP WHERE  GROUP_SKU_FLAG='G' AND COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_CUSTOMER_CODE = CS.MASTER_CUSTOMER_CODE) as Childrens
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N' AND CS.COMPANY_CODE = '" + userinfo.company_code + @"'
            AND GROUP_SKU_FLAG = 'G'
            AND LEVEL = 1
            START WITH PRE_CUSTOMER_CODE = '00'
            CONNECT BY NOCYCLE  PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
            ORDER SIBLINGS BY CUSTOMER_EDESC";
            var customerListNodes = _objectEntity.SqlQuery<CustomerSetupModel>(query).ToList();
            return customerListNodes;
        }



        public List<ConsolidateTree> CompanyListAllNodes(User userinfo, string userNo = null)

        {


            // string query = $@"SELECT INITCAP(CC.COMPANY_EDESC) AS BRANCH_EDESC,CS.BRANCH_CODE,
            //CS.PRE_BRANCH_CODE,CC.ABBR_CODE
            // FROM FA_BRANCH_SETUP CS, COMPANY_SETUP CC
            // WHERE CS.BRANCH_CODE = CC.COMPANY_CODE AND CS.DELETED_FLAG = 'N'
            // AND GROUP_SKU_FLAG = 'G' AND CC.COMPANY_CODE IN (SELECT COMPANY_CODE FROM SC_COMPANY_CONTROL WHERE ACCESS_FLAG='Y' AND USER_NO='{userinfo.User_id}')";

            String query = $@"SELECT CC.COMPANY_CODE BRANCH_CODE, INITCAP(CC.COMPANY_EDESC) AS BRANCH_EDESC,'00' PRE_BRANCH_CODE,CC.ABBR_CODE
                            FROM COMPANY_SETUP CC
                            WHERE  CC.DELETED_FLAG = 'N'
                            AND (CC.COMPANY_CODE IN (SELECT COMPANY_CODE FROM SC_COMPANY_CONTROL WHERE ACCESS_FLAG='Y' AND USER_NO='{userinfo.User_id}')
                            OR  CC.COMPANY_CODE IN (SELECT COMPANY_CODE FROM COMPANY_SETUP WHERE 1 = (SELECT COUNT(DISTINCT USER_NO) FROM SC_APPLICATION_USERS WHERE USER_TYPE <> 'GENERAL' AND USER_NO = '{userinfo.User_id}' )))
                            UNION    
                            SELECT DISTINCT  CS.BRANCH_CODE, INITCAP(CS.BRANCH_EDESC) AS BRANCH_EDESC ,CS.PRE_BRANCH_CODE,CS.ABBR_CODE
                            FROM FA_BRANCH_SETUP CS   
                            WHERE  CS.DELETED_FLAG = 'N'  
                            AND CS.PRE_BRANCH_CODE <> '00'     
                            AND(CS.BRANCH_CODE IN(SELECT BRANCH_CODE FROM SC_BRANCH_CONTROL WHERE ACCESS_FLAG = 'Y' AND USER_NO = '{userinfo.User_id}' )
                            OR CS.BRANCH_CODE IN(SELECT BRANCH_CODE FROM FA_BRANCH_SETUP WHERE 1 = (SELECT COUNT(DISTINCT USER_NO) FROM SC_APPLICATION_USERS WHERE USER_TYPE <> 'GENERAL' AND USER_NO = '{userinfo.User_id}' )))";

            var consolidateListNodes = _objectEntity.SqlQuery<ConsolidateTree>(query).ToList();
            //var accessedData = new List<AccessedControl>();
            //if (userNo != null)
            //{
            //    string accessedQuery = $@"SELECT scc.COMPANY_CODE,sac.USER_NO,sbc.BRANCH_CODE FROM SC_COMPANY_CONTROL scc
            //                              INNER JOIN SC_APPLICATION_USERS sac on scc.USER_NO=sac.USER_NO 
            //                              INNER JOIN SC_BRANCH_CONTROL sbc on sbc.USER_NO = sac.USER_NO WHERE sac.USER_NO={userNo}";
            //    accessedData = _objectEntity.SqlQuery<AccessedControl>(accessedQuery).ToList();

            //    foreach (var cln in consolidateListNodes)
            //    {
            //        if (cln.branch_Code == accessedData.Where(x => x.BRANCH_CODE == cln.branch_Code).Select(x => x.BRANCH_CODE).FirstOrDefault())
            //        {
            //            cln.@checked = true;

            //        }
            //    }
            //}
            return consolidateListNodes;
        }



        public List<ConsolidateTree> branchListByCompanyCode(User userinfo, string company_code)
        {

            string query = @"SELECT INITCAP(CS.BRANCH_EDESC) AS BRANCH_EDESC,CS.BRANCH_CODE,
            CS.PRE_BRANCH_CODE,CC.ABBR_CODE
            FROM FA_BRANCH_SETUP CS, COMPANY_SETUP CC
            WHERE  CS.COMPANY_CODE = CC.COMPANY_CODE
            AND CS.DELETED_FLAG = 'N'
            AND CS.GROUP_SKU_FLAG = 'I' AND CS.COMPANY_CODE = '" + company_code + "'  AND CC.COMPANY_CODE IN (SELECT COMPANY_CODE FROM SC_COMPANY_CONTROL WHERE ACCESS_FLAG='Y' AND USER_NO='" + userinfo.User_id + "')";
            var consolidateListNodes = _objectEntity.SqlQuery<ConsolidateTree>(query).ToList();
            return consolidateListNodes;
        }


        public List<CustomerSetupModel> CustomerGroupListAllNodes()
        {
            string query = @"SELECT DISTINCT LEVEL,INITCAP(CS.CUSTOMER_EDESC) CUSTOMER_EDESC,CS.CUSTOMER_CODE,
            CS.GROUP_SKU_FLAG,CS.MASTER_CUSTOMER_CODE,CS.PRE_CUSTOMER_CODE, CS.BRANCH_CODE, LEVEL,
            (SELECT COUNT(*) FROM SA_CUSTOMER_SETUP WHERE GROUP_SKU_FLAG='G' AND PRE_CUSTOMER_CODE = CS.MASTER_CUSTOMER_CODE) as Childrens
            FROM SA_CUSTOMER_SETUP CS
            WHERE CS.DELETED_FLAG = 'N'
            AND GROUP_SKU_FLAG = 'G'
            AND LEVEL = 1
            START WITH PRE_CUSTOMER_CODE = '00'
            CONNECT BY PRIOR MASTER_CUSTOMER_CODE = PRE_CUSTOMER_CODE
            ORDER SIBLINGS BY CUSTOMER_EDESC";
            var customerListNodes = _objectEntity.SqlQuery<CustomerSetupModel>(query).ToList();
            return customerListNodes;
        }

        public List<ProductSetupModel> ProductListAllNodes()
        {
            string query = @"SELECT DISTINCT LEVEL, 
                 INITCAP(ITEM_EDESC) AS ItemName,
                 ITEM_CODE AS ItemCode,
                 MASTER_ITEM_CODE AS MasterItemCode, 
                 PRE_ITEM_CODE AS PreItemCode, 
                 GROUP_SKU_FLAG AS GroupFlag
                 FROM IP_ITEM_MASTER_SETUP ims
                 WHERE ims.DELETED_FLAG = 'N'                  
                 AND GROUP_SKU_FLAG = 'G'
                 AND LEVEL = 1
                 START WITH PRE_ITEM_CODE = '00'
                 CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
            var productListNodes = _objectEntity.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }

        public List<ProductSetupModel> ProductListAllNodes(User userinfo)
        {

            string query = @"SELECT LEVEL, 
                 INITCAP(ITEM_EDESC) AS ItemName,
                 ITEM_CODE AS ItemCode,
                 MASTER_ITEM_CODE AS MasterItemCode, 
                 PRE_ITEM_CODE AS PreItemCode, 
                 GROUP_SKU_FLAG AS GroupFlag,
                (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
                GROUP_SKU_FLAG='G' AND COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
                 FROM IP_ITEM_MASTER_SETUP ims
                 WHERE ims.DELETED_FLAG = 'N' 
                 AND LEVEL=1
                 AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
                 AND GROUP_SKU_FLAG = 'G'
                 START WITH PRE_ITEM_CODE = '00'
                 CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE";
            var productListNodes = _objectEntity.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }

        public List<ProductSetupModel> GetProductsListByProductCode(string level, string masterProductCode)
        {
            string query = string.Format(@"SELECT DISTINCT LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode,
            GROUP_SKU_FLAG AS GroupFlag
            FROM IP_ITEM_MASTER_SETUP
            WHERE DELETED_FLAG = 'N'             
            AND LEVEL = {0}
            START WITH PRE_ITEM_CODE = '{1}'
            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
            ORDER BY INITCAP(ITEM_EDESC)", level.ToString(), masterProductCode.ToString());
            var productListNodes = _objectEntity.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }

        public List<ProductSetupModel> GetProductsListByProductCode(string level, string masterProductCode, User userinfo)
        {
            string query = string.Format(@"SELECT LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag,
            (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
             COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
            FROM IP_ITEM_MASTER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
            AND LEVEL = {0}
            START WITH PRE_ITEM_CODE = '{1}'
            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
            ORDER SIBLINGS BY ITEM_EDESC", level.ToString(), masterProductCode.ToString());
            var productListNodes = _objectEntity.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }


        public List<ProductSetupModel> GetProductsListWithChild(string level, string masterProductCode, User userinfo)
        {
            string query = string.Format(@"SELECT LEVEL, 
            INITCAP(ITEM_EDESC) AS ItemName,
            ITEM_CODE AS ItemCode,
            MASTER_ITEM_CODE AS MasterItemCode, 
            PRE_ITEM_CODE AS PreItemCode, 
            GROUP_SKU_FLAG AS GroupFlag,
            (SELECT COUNT(*) FROM IP_ITEM_MASTER_SETUP WHERE  
             COMPANY_CODE='" + userinfo.company_code + @"' AND DELETED_FLAG='N' AND PRE_ITEM_CODE = ims.MASTER_ITEM_CODE) as Childrens 
            FROM IP_ITEM_MASTER_SETUP ims
            WHERE ims.DELETED_FLAG = 'N' 
            AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
            --AND LEVEL = {0}
            START WITH PRE_ITEM_CODE = '{1}'
            CONNECT BY PRIOR MASTER_ITEM_CODE = PRE_ITEM_CODE
            ORDER SIBLINGS BY ITEM_EDESC", level.ToString(), masterProductCode.ToString());
            var productListNodes = _objectEntity.SqlQuery<ProductSetupModel>(query).ToList();
            return productListNodes;
        }

        // Supplier query for the SupplierTree
        public List<SupplierSetupModel> GetSupplierListBySupplierCode(string level, string masterSupplierCode)
        {
            string query = string.Format(@"SELECT DISTINCT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(SUPPLIER_EDESC) AS SupplierName,
                                        SUPPLIER_CODE AS SupplierCode,
                                        MASTER_SUPPLIER_CODE AS MasterSupplierCode, 
                                        PRE_SUPPLIER_CODE AS PreSupplierCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM IP_SUPPLIER_SETUP ims
                                        WHERE ims.DELETED_FLAG = 'N'                                         
                                        --AND GROUP_SKU_FLAG = 'G'         
                                        AND LEVEL = {0} 
                                        START WITH PRE_SUPPLIER_CODE = {1}
                                        CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE
                                        /*ORDER SIBLINGS BY SUPPLIER_EDESC*/", level.ToString(), masterSupplierCode.ToString());
            var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            return SupplierListNodes;
        }

        public List<SupplierSetupModel> GetSupplierListBySupplierCode(string level, string masterSupplierCode, User userinfo)
        {
            string query = string.Format(@"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(SUPPLIER_EDESC) AS SupplierName,
                                        SUPPLIER_CODE AS SupplierCode,
                                        MASTER_SUPPLIER_CODE AS MasterSupplierCode, 
                                        PRE_SUPPLIER_CODE AS PreSupplierCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM IP_SUPPLIER_SETUP ims
                                        WHERE ims.DELETED_FLAG = 'N' 
                                        AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
                                        --AND ims.Branch_Code = '" + userinfo.branch_code + @"'
                                        --AND GROUP_SKU_FLAG = 'G'         
                                        AND LEVEL = {0} 
                                        START WITH PRE_SUPPLIER_CODE = {1}
                                        CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE
                                        ORDER SIBLINGS BY SUPPLIER_EDESC", level.ToString(), masterSupplierCode.ToString());
            var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            return SupplierListNodes;
        }
        public List<SupplierSetupModel> GetDealerListBySupplierCode(string level, string masterSupplierCode, User userinfo)
        {
            string query = string.Format(@"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(PARTY_TYPE_EDESC) AS SupplierName,
                                        PARTY_TYPE_CODE AS SupplierCode,
                                        MASTER_PARTY_CODE AS MasterSupplierCode, 
                                        PRE_PARTY_CODE AS PreSupplierCode, 
                                        GROUP_SKU_FLAG AS GroupFlag
                                        FROM IP_PARTY_TYPE_CODE ims
                                        WHERE ims.DELETED_FLAG = 'N' 
                                        AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
                                        --AND ims.Branch_Code = '" + userinfo.branch_code + @"'
                                        --AND GROUP_SKU_FLAG = 'G'         
                                        AND LEVEL = {0} 
                                        START WITH PRE_PARTY_CODE = '{1}'
                                        CONNECT BY PRIOR MASTER_PARTY_CODE = PRE_PARTY_CODE
                                        ORDER SIBLINGS BY PARTY_TYPE_EDESC", level.ToString(), masterSupplierCode.ToString());
            var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            return SupplierListNodes;
        }
        public List<SupplierSetupModel> GetSupplierListBySupplierCodeGroup(string masterSupplierCode)
        {
            string query = string.Format(@"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(SUPPLIER_EDESC) AS SupplierName,
                                        SUPPLIER_CODE AS SupplierCode,
                                        MASTER_SUPPLIER_CODE AS MasterSupplierCode, 
                                        PRE_SUPPLIER_CODE AS PreSupplierCode, 
                                        GROUP_SKU_FLAG AS GroupFlag,
                                        (SELECT COUNT(*) FROM IP_SUPPLIER_SETUP WHERE GROUP_SKU_FLAG='G' AND PRE_SUPPLIER_CODE = IMS.MASTER_SUPPLIER_CODE) as Childrens
                                        FROM IP_SUPPLIER_SETUP ims
                                        WHERE ims.DELETED_FLAG = 'N' 
                                        AND ims.COMPANY_CODE = '01'
                                        AND GROUP_SKU_FLAG = 'G'
                                        START WITH PRE_SUPPLIER_CODE = {0}
                                        CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE
                                        ORDER SIBLINGS BY SUPPLIER_EDESC", masterSupplierCode.ToString());
            var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            return SupplierListNodes;
        }

        public List<SupplierSetupModel> GetSupplierListBySupplierCodeGroup(string masterSupplierCode, User userinfo)
        {
            if (userinfo == null)
            {
                userinfo = new Core.Domain.User();
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";

            }
            else if (string.IsNullOrEmpty(userinfo.company_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            else if (string.IsNullOrEmpty(userinfo.branch_code))
            {
                userinfo.company_code = "01";
                userinfo.branch_code = "01.01";
            }
            string query = string.Format(@"SELECT LEVEL, COMPANY_CODE,DELETED_FLAG,
                                        INITCAP(SUPPLIER_EDESC) AS SupplierName,
                                        SUPPLIER_CODE AS SupplierCode,
                                        MASTER_SUPPLIER_CODE AS MasterSupplierCode, 
                                        PRE_SUPPLIER_CODE AS PreSupplierCode, 
                                        GROUP_SKU_FLAG AS GroupFlag,
                                        (SELECT COUNT(*) FROM IP_SUPPLIER_SETUP WHERE GROUP_SKU_FLAG='G' AND PRE_SUPPLIER_CODE = IMS.MASTER_SUPPLIER_CODE) as Childrens
                                        FROM IP_SUPPLIER_SETUP ims
                                        WHERE ims.DELETED_FLAG = 'N' 
                                        AND ims.COMPANY_CODE = '" + userinfo.company_code + @"'
                                        AND ims.Branch_Code = '" + userinfo.branch_code + @"'
                                        AND GROUP_SKU_FLAG = 'G'
                                        START WITH PRE_SUPPLIER_CODE = {0}
                                        CONNECT BY PRIOR MASTER_SUPPLIER_CODE = PRE_SUPPLIER_CODE
                                        ORDER SIBLINGS BY SUPPLIER_EDESC", masterSupplierCode.ToString());
            var SupplierListNodes = _objectEntity.SqlQuery<SupplierSetupModel>(query).ToList();
            return SupplierListNodes;
        }

        public List<SalesRegisterModel> SaleRegisters()
        {
            string query = @"SELECT A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI, A.SALES_NO, B.CUSTOMER_EDESC, C.ITEM_EDESC, A.MU_CODE, A.CALC_QUANTITY, A.CALC_UNIT_PRICE, A.CALC_TOTAL_PRICE,
            (SELECT SUM(CHARGE_AMOUNT) FROM CHARGE_TRANSACTION WHERE REFERENCE_NO = A.SALES_NO AND CHARGE_CODE = 'DC' and APPLY_ON='D' AND   1 = A.SERIAL_NO) AS DISCOUNT,
            (SELECT CHARGE_AMOUNT FROM CHARGE_TRANSACTION WHERE REFERENCE_NO = A.SALES_NO AND CHARGE_CODE = 'VT' and APPLY_ON='D' AND  1 = A.SERIAL_NO) AS VAT
            FROM SA_SALES_INVOICE A, SA_CUSTOMER_SETUP B, IP_ITEM_MASTER_SETUP C
            WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE
            AND A.COMPANY_CODE = B.COMPANY_CODE
            AND A.ITEM_CODE = C.ITEM_CODE
            AND A.COMPANY_CODE = C.COMPANY_CODE
            AND A.DELETED_FLAG = 'N'
            ORDER BY A.SALES_NO";
            var salesRegisters = _objectEntity.SqlQuery<SalesRegisterModel>(query).ToList();
            foreach (var salesReg in salesRegisters)
            {
                salesReg.CALC_UNIT_PRICE = salesReg.CALC_UNIT_PRICE == null ? 0 : salesReg.CALC_UNIT_PRICE;
                salesReg.CALC_QUANTITY = salesReg.CALC_QUANTITY == null ? 0 : salesReg.CALC_QUANTITY;
                salesReg.CALC_TOTAL_PRICE = salesReg.CALC_TOTAL_PRICE == null ? 0 : salesReg.CALC_TOTAL_PRICE;
                salesReg.DISCOUNT = salesReg.DISCOUNT == null ? 0 : salesReg.DISCOUNT;
                salesReg.VAT = salesReg.VAT == null ? 0 : salesReg.VAT;
                salesReg.NetAmount = salesReg.CALC_TOTAL_PRICE - salesReg.DISCOUNT;
                salesReg.InvoiceAmount = salesReg.NetAmount + salesReg.VAT;
            }

            return salesRegisters;
        }
        public List<SalesRegisterModel> SaleRegistersDateWiseFilter(string formDate, string toDate)
        {
            //if(string.IsNullOrEmpty(formDate))

            string query = @"SELECT A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI, A.SALES_NO, B.CUSTOMER_EDESC, C.ITEM_EDESC, A.MU_CODE, A.CALC_QUANTITY, A.CALC_UNIT_PRICE, A.CALC_TOTAL_PRICE,
            (SELECT SUM(CHARGE_AMOUNT) FROM CHARGE_TRANSACTION WHERE REFERENCE_NO = A.SALES_NO AND CHARGE_CODE = 'DC' and APPLY_ON='D' AND   1 = A.SERIAL_NO) AS DISCOUNT,
            (SELECT CHARGE_AMOUNT FROM CHARGE_TRANSACTION WHERE REFERENCE_NO = A.SALES_NO AND CHARGE_CODE = 'VT' and APPLY_ON='D' AND  1 = A.SERIAL_NO) AS VAT
            FROM SA_SALES_INVOICE A, SA_CUSTOMER_SETUP B, IP_ITEM_MASTER_SETUP C
            WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE
            AND A.COMPANY_CODE = B.COMPANY_CODE
            AND A.ITEM_CODE = C.ITEM_CODE
            AND A.COMPANY_CODE = C.COMPANY_CODE
            AND A.DELETED_FLAG = 'N'";
            if (!string.IsNullOrEmpty(formDate))
                query = query + " and A.SALES_DATE>=TO_DATE('" + formDate + "', 'YYYY-MM-DD') and A.SALES_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            var salesRegisters = _objectEntity.SqlQuery<SalesRegisterModel>(query).ToList();
            foreach (var salesReg in salesRegisters)
            {
                salesReg.CALC_UNIT_PRICE = salesReg.CALC_UNIT_PRICE == null ? 0 : salesReg.CALC_UNIT_PRICE;
                salesReg.CALC_QUANTITY = salesReg.CALC_QUANTITY == null ? 0 : salesReg.CALC_QUANTITY;
                salesReg.CALC_TOTAL_PRICE = salesReg.CALC_TOTAL_PRICE == null ? 0 : salesReg.CALC_TOTAL_PRICE;
                salesReg.DISCOUNT = salesReg.DISCOUNT == null ? 0 : salesReg.DISCOUNT;
                salesReg.VAT = salesReg.VAT == null ? 0 : salesReg.VAT;
                salesReg.NetAmount = salesReg.CALC_TOTAL_PRICE - salesReg.DISCOUNT;
                salesReg.InvoiceAmount = salesReg.NetAmount + salesReg.VAT;
            }

            return salesRegisters;
        }
        public List<SalesRegisterModel> GetSaleRegisters(ReportFiltersModel filters)
        {
            //if(string.IsNullOrEmpty(formDate))

            string query = @"SELECT A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI, A.SALES_NO, B.CUSTOMER_EDESC, C.ITEM_EDESC, A.MU_CODE, Round(A.CALC_QUANTITY/{0},{1}), A.CALC_UNIT_PRICE, A.CALC_TOTAL_PRICE,
                (SELECT SUM(CHARGE_AMOUNT) FROM CHARGE_TRANSACTION WHERE REFERENCE_NO = A.SALES_NO AND CHARGE_CODE = 'DC' and APPLY_ON='D' AND   1 = A.SERIAL_NO) AS DISCOUNT,
                (SELECT CHARGE_AMOUNT FROM CHARGE_TRANSACTION WHERE REFERENCE_NO = A.SALES_NO AND CHARGE_CODE = 'VT' and APPLY_ON='D' AND  1 = A.SERIAL_NO) AS VAT
                FROM SA_SALES_INVOICE A, SA_CUSTOMER_SETUP B, IP_ITEM_MASTER_SETUP C
                WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.ITEM_CODE = C.ITEM_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND A.DELETED_FLAG = 'N'";
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and A.SALES_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and A.SALES_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";
            var salesRegisters = _objectEntity.SqlQuery<SalesRegisterModel>(query).ToList();
            foreach (var salesReg in salesRegisters)
            {
                salesReg.CALC_UNIT_PRICE = salesReg.CALC_UNIT_PRICE == null ? 0 : salesReg.CALC_UNIT_PRICE;
                salesReg.CALC_QUANTITY = salesReg.CALC_QUANTITY == null ? 0 : salesReg.CALC_QUANTITY;
                salesReg.CALC_TOTAL_PRICE = salesReg.CALC_TOTAL_PRICE == null ? 0 : salesReg.CALC_TOTAL_PRICE;
                salesReg.DISCOUNT = salesReg.DISCOUNT == null ? 0 : salesReg.DISCOUNT;
                salesReg.VAT = salesReg.VAT == null ? 0 : salesReg.VAT;
                salesReg.NetAmount = salesReg.CALC_TOTAL_PRICE - salesReg.DISCOUNT;
                salesReg.InvoiceAmount = salesReg.NetAmount + salesReg.VAT;
            }

            return salesRegisters;
        }
        public List<Charges> GetSalesCharges()
        {
            string query = @"SELECT  CT.charge_code, CC.CHARGE_EDESC,CT.CHARGE_AMOUNT as CHARGE_AMOUNT,CT.APPLY_ON,CT.REFERENCE_NO,CT.CHARGE_TYPE_FLAG  FROM CHARGE_TRANSACTION CT, IP_CHARGE_CODE CC
            WHERE 1=1
            AND CT.CHARGE_CODE = CC.CHARGE_CODE
            and form_code in (select form_code from form_detail_setup where table_name = 'SA_SALES_INVOICE') and CT.apply_on='D'";
            var salesRegisters = _objectEntity.SqlQuery<Charges>(query).ToList();
            return salesRegisters;
        }
        public List<Charges> GetSalesCharges(ReportFiltersModel filters)
        {
            var user = this._workContext.CurrentUserinformation;
            // var companyCode = string.Join(",", filters.CompanyFilter);
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{this._workContext.CurrentUserinformation.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            // companyCode = companyCode == "" ? this._workContext.CurrentUserinformation.company_code : companyCode;
            string query = @"SELECT  CT.charge_code||CT.APPLY_ON as charge_code,  CC.CHARGE_EDESC ||'(' || CT.APPLY_ON ||')' as CHARGE_EDESC,Round(NVL(FN_CONVERT_CURRENCY(NVL(CT.CHARGE_AMOUNT,0),'NRS',(SELECT SALES_DATE FROM SA_SALES_INVOICE WHERE SALES_NO = CT.REFERENCE_NO and company_code=ct.company_code AND ROWNUM=1)),0)/{0},{1}) as CHARGE_AMOUNT,CT.APPLY_ON,CT.REFERENCE_NO,CT.CHARGE_TYPE_FLAG  FROM CHARGE_TRANSACTION CT, IP_CHARGE_CODE CC
            WHERE 1=1
            AND CT.CHARGE_CODE = CC.CHARGE_CODE AND  CT.COMPANY_CODE=CC.COMPANY_CODE AND CT.COMPANY_CODE in({2}) and   TRUNC(CT.created_date)>=TRUNC(TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD')) and TRUNC(CT.created_date) <= TRUNC(TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD'))";
            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND CT.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            query = query + " and CT.form_code in (select form_code from form_detail_setup where table_name = 'SA_SALES_INVOICE' and company_code in (" + companyCode + "))";
            //query = query + " and CT.form_code in (select form_code from form_detail_setup where table_name = 'SA_SALES_INVOICE' and company_code=" + companyCode + ")";
            query = string.Format(query, ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter), companyCode);
            var salesRegisters = _objectEntity.SqlQuery<Charges>(query).ToList();
            return salesRegisters;

        }
        public List<Charges> GetSalesItemCharges(ReportFiltersModel filters, string salesNo)
        {
            var user = this._workContext.CurrentUserinformation;
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{this._workContext.CurrentUserinformation.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = @"SELECT DISTINCT  CT.charge_code||CT.APPLY_ON as charge_code,  CC.CHARGE_EDESC ||'(' || CT.APPLY_ON ||')' as CHARGE_EDESC,Round(NVL(FN_CONVERT_CURRENCY(NVL(CT.CHARGE_AMOUNT,0),'NRS',(SELECT SALES_DATE FROM SA_SALES_INVOICE WHERE SALES_NO = CT.REFERENCE_NO AND ROWNUM=1)),0)/{0},{1}) as CHARGE_AMOUNT,CT.APPLY_ON,CT.REFERENCE_NO,CT.CHARGE_TYPE_FLAG,CT.ITEM_CODE  FROM CHARGE_TRANSACTION CT, IP_CHARGE_CODE CC
            WHERE 1=1
            AND CT.CHARGE_CODE = CC.CHARGE_CODE AND CT.COMPANY_CODE in({2}) and   CT.created_date>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and CT.created_date <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";
            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND CT.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }

            query = query + " and CT.REFERENCE_NO='" + salesNo + "' and CT.form_code in (select form_code from form_detail_setup where table_name = 'SA_SALES_INVOICE') and CT.apply_on='I'";
            query = string.Format(query, ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter), companyCode);
            var salesRegisters = _objectEntity.SqlQuery<Charges>(query).ToList();
            return salesRegisters;

        }
        public List<ChargesTitle> GetChargesTitle()
        {
            var user = this._workContext.CurrentUserinformation;
            string query = @"   SELECT distinct CT.charge_code||CT.APPLY_ON as ChargesHeaderNo, CC.CHARGE_EDESC ||'(' || CT.APPLY_ON ||')' as ChargesHeaderTitle,CT.APPLY_ON
          FROM CHARGE_TRANSACTION CT, IP_CHARGE_CODE CC
        WHERE 1=1
        AND CT.CHARGE_CODE = CC.CHARGE_CODE
        and form_code in (select form_code from form_detail_setup where table_name = 'SA_SALES_INVOICE')";
            var chargeTitles = _objectEntity.SqlQuery<ChargesTitle>(query).ToList();
            return chargeTitles;
        }
        public List<ChargesTitle> GetChargesItemTitle()
        {
            var user = this._workContext.CurrentUserinformation;
            string query = @"SELECT distinct CT.charge_code||CT.APPLY_ON as ChargesHeaderNo,  CC.CHARGE_EDESC ||'(' || CT.APPLY_ON ||')' as ChargesHeaderTitle
          FROM CHARGE_TRANSACTION CT, IP_CHARGE_CODE CC
        WHERE 1=1
        AND CT.CHARGE_CODE = CC.CHARGE_CODE
        and form_code in (select form_code from form_detail_setup where table_name = 'SA_SALES_INVOICE') and CT.apply_on='I'";
            var chargeTitles = _objectEntity.SqlQuery<ChargesTitle>(query).ToList();
            return chargeTitles;
        }
        public List<VatRegisterModel> GetVatRegister()
        {
            string query = @"SELECT BS_DATE(SALES_DATE) MITI, INVOICE_NO as InvoiceNo, PARTY_NAME as PartyName, VAT_NO as PANNo, 
                FN_CONVERT_CURRENCY(NVL(GROSS_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE), 
                FN_CONVERT_CURRENCY(NVL(TAXABLE_SALES,1) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) as TaxableSales,
                 FN_CONVERT_CURRENCY(NVL(VAT,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) as VatAmount , 
                 FN_CONVERT_CURRENCY(NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) as NetSales ,
                 FORM_CODE, BRANCH_CODE,CREDIT_DAYS,DELETED_FLAG,SALES_DISCOUNT as Discount, MANUAL_NO,DELETED_FLAG 
                 FROM V$SALES_INVOICE_REPORT3 WHERE SALES_DATE >= '13-Apr-2014' 
                 AND SALES_DATE <= '13-May-2016' AND COMPANY_CODE='01'  AND BRANCH_CODE='01.01'   ORDER BY BS_DATE(SALES_DATE), INVOICE_NO";
            var vatRegisters = _objectEntity.SqlQuery<VatRegisterModel>(query).ToList();
            return vatRegisters;
        }
        public List<VatRegisterModel> GetVatRegisterDateWiseFilter(string formDate, string toDate)
        {
            string query = @"SELECT BS_DATE(SALES_DATE) MITI, INVOICE_NO as InvoiceNo, PARTY_NAME as PartyName, VAT_NO as PANNo, 
            FN_CONVERT_CURRENCY(NVL(GROSS_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE), 
            FN_CONVERT_CURRENCY(NVL(TAXABLE_SALES,1) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) as TaxableSales,
             FN_CONVERT_CURRENCY(NVL(VAT,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) as VatAmount , 
             FN_CONVERT_CURRENCY(NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) as NetSales ,
             FORM_CODE, BRANCH_CODE,CREDIT_DAYS,DELETED_FLAG,SALES_DISCOUNT as Discount, MANUAL_NO,DELETED_FLAG 
             FROM V$SALES_INVOICE_REPORT3 WHERE SALES_DATE >= TO_DATE('" + formDate + "', 'YYYY-MM-DD') AND SALES_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD') AND COMPANY_CODE='01'  AND BRANCH_CODE='01.01'   ORDER BY BS_DATE(SALES_DATE), INVOICE_NO";
            var vatRegisters = _objectEntity.SqlQuery<VatRegisterModel>(query).ToList();
            return vatRegisters;
        }

        public List<VatRegisterModel> GetVatRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            //var companyCode = string.Join(",", filters.CompanyFilter);
            //companyCode = companyCode == "" ? userinfo.company_code : companyCode;
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            string query = string.Format(@"SELECT BS_DATE(SALES_DATE) MITI, INVOICE_NO as InvoiceNo, PARTY_NAME as PartyName, VAT_NO as PANNo, 
            FN_CONVERT_CURRENCY(Round((NVL(GROSS_SALES,0) * NVL(EXCHANGE_RATE,1))/{0},{1}),'NRS', SALES_DATE), 
            FN_CONVERT_CURRENCY(Round((NVL(TAXABLE_SALES,1) * NVL(EXCHANGE_RATE,1))/{2},{3}),'NRS', SALES_DATE) as TaxableSales,
             FN_CONVERT_CURRENCY(Round((NVL(VAT,0) * NVL(EXCHANGE_RATE,1))/{4},{5}),'NRS', SALES_DATE) as VatAmount , 
             FN_CONVERT_CURRENCY(Round((NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1))/{6},{7}),'NRS', SALES_DATE) as NetSales ,

               FN_CONVERT_CURRENCY (NVL (TOTAL_SALES, 0) * NVL (EXCHANGE_RATE, 1),
                              'NRS',
                              SALES_DATE)
         - FN_CONVERT_CURRENCY (
              NVL (CASE WHEN TABLE_NAME = 'SALES_RETURN' THEN -NVL(SALES_DISCOUNT,0)
          ELSE NVL(SALES_DISCOUNT,0) END, 0) * NVL (EXCHANGE_RATE, 1),
              'NRS',
              SALES_DATE)
            EXCISEABLE_SALES,
         FN_CONVERT_CURRENCY (NVL (EXCISE_AMOUNT, 0) * NVL (EXCHANGE_RATE, 1),
                              'NRS',
                              SALES_DATE)
            TaxExempSales,
             FORM_CODE, BRANCH_CODE,CREDIT_DAYS,DELETED_FLAG,Round(CASE WHEN TABLE_NAME = 'SALES_RETURN' THEN -NVL(SALES_DISCOUNT,0)
          ELSE NVL(SALES_DISCOUNT,0) END/{8},{9}) as Discount, NVL(MANUAL_NO,'-') MANUAL_NO,DELETED_FLAG 
             FROM V$SALES_INVOICE_REPORT3 WHERE SALES_DATE >= TO_DATE('{10}', 'YYYY-MM-DD') AND SALES_DATE <= TO_DATE('{11}', 'YYYY-MM-DD') 
             and DELETED_FLAG='N' AND COMPANY_CODE IN(" + companyCode + ")", figureFilter, roundUpFilter, figureFilter, roundUpFilter, figureFilter, roundUpFilter, figureFilter, roundUpFilter, figureFilter, roundUpFilter, filters.FromDate, filters.ToDate);

            if (filters.CustomerFilter.Count > 0)
            {

                var customers = filters.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {

                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", customers[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join("','", customers));

            }

            if (filters.PartyTypeFilter.Count > 0)
            {
                query = query + string.Format(@" AND PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", filters.PartyTypeFilter).ToString());
            }

            if (filters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            if (filters.DivisionFilter.Count > 0)
            {
                query += string.Format(@" AND DIVISION_CODE IN ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
            }
            var min = 0;
            var max = 0;
            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);

            if (!(min == 0 && max == 0))
            {
                query = query + string.Format(@" AND FN_CONVERT_CURRENCY((NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1)),'NRS',SALES_DATE) >={0} AND FN_CONVERT_CURRENCY((NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1)),'NRS',SALES_DATE)<= {1}", min, max);
            }

            query += "ORDER BY BS_DATE(SALES_DATE), INVOICE_NO";

            // var salesRegisters = _objectEntity.SqlQuery<SalesRegisterMasterModel>(query).ToList();


            var vatRegisters = _objectEntity.SqlQuery<VatRegisterModel>(query).ToList();
            return vatRegisters;
        }


        public List<SalesRegisterMasterModel> SaleRegistersMasterDateWiseFilter(string formDate, string toDate)
        {
            string query = @"SELECT A.SALES_DATE as SalesDate ,A.MITI as Miti,A.SALES_NO as InvoiceNumber,A.CUSTOMER_EDESC as CustomerName,A.GROSS_AMOUNT as GrossAmount,NVL( A.DISCOUNT,0) as Discount, NVL(A.GROSS_AMOUNT,0)- NVL(A.DISCOUNT,0) NetAmount,NVL( A.VAT ,0) as VAT,NVL(A.GROSS_AMOUNT,0)- NVL(A.DISCOUNT,0)+NVL(A.VAT,0) InvoiceAmount FROM
                         (SELECT A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI, A.SALES_NO, B.CUSTOMER_EDESC,
                         SUM(NVL(A.CALC_TOTAL_PRICE,0))  GROSS_AMOUNT,
                        (SELECT SUM(CHARGE_AMOUNT) FROM CHARGE_TRANSACTION WHERE REFERENCE_NO = A.SALES_NO AND CHARGE_CODE = 'DC' and APPLY_ON='D' ) AS DISCOUNT,
                        (SELECT CHARGE_AMOUNT FROM CHARGE_TRANSACTION WHERE REFERENCE_NO = A.SALES_NO AND CHARGE_CODE = 'VT' and APPLY_ON='D'  ) AS VAT
                        FROM SA_SALES_INVOICE A, SA_CUSTOMER_SETUP B, IP_ITEM_MASTER_SETUP C
                        WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE
                        AND A.COMPANY_CODE = B.COMPANY_CODE
                        AND A.ITEM_CODE = C.ITEM_CODE
                        AND A.COMPANY_CODE = C.COMPANY_CODE
                        AND A.DELETED_FLAG = 'N'";
            if (!string.IsNullOrEmpty(formDate))
                query = query + " and A.SALES_DATE>=TO_DATE('" + formDate + "', 'YYYY-MM-DD') and A.SALES_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            query = query + "GROUP BY A.SALES_DATE,  A.SALES_NO, B.CUSTOMER_EDESC) A ";
            var salesRegisters = _objectEntity.SqlQuery<SalesRegisterMasterModel>(query).ToList();


            return salesRegisters;
        }
        public List<SalesChildModel> GetSalesItemBySalesId(filterOption filters, string salesId, string ItemCompanyCode)
        {
            if (string.IsNullOrEmpty(salesId))
                throw new Exception();
            var companyCode = string.Join(",", filters.ReportFilters.CompanyFilter);
            companyCode = companyCode == "" ? this._workContext.CurrentUserinformation.company_code : companyCode;

            string query = @"SELECT  C.ITEM_EDESC AS ProductName,C.ITEM_CODE,A.SALES_NO AS INVOICENO,NVL(A.CALC_QUANTITY,0) AS Quanity,A.CALC_UNIT_PRICE AS Rate,A.CALC_TOTAL_PRICE AS GrossAmount
                ,A.MU_CODE AS UNIT
                FROM SA_SALES_INVOICE A, SA_CUSTOMER_SETUP B, IP_ITEM_MASTER_SETUP C
                WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE
                AND A.COMPANY_CODE = B.COMPANY_CODE
                AND A.ITEM_CODE = C.ITEM_CODE
                AND A.COMPANY_CODE = C.COMPANY_CODE
                AND a.COMPANY_CODE IN(" + ItemCompanyCode + ")";


            if (filters.ReportFilters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", filters.ReportFilters.BranchFilter).ToString());
            }
            query += " AND A.DELETED_FLAG = 'N' AND A.SALES_NO='" + salesId + "'";
            return _objectEntity.SqlQuery<SalesChildModel>(query).ToList();



        }
        public List<ChargesMap> GetChargesMapList()
        {
            string query = @"SELECT distinct CT.charge_code as chargeFieldSystemName, CC.CHARGE_EDESC as chargeFieldName,CT.CHARGE_TYPE_FLAG as chargeType
              FROM CHARGE_TRANSACTION CT, IP_CHARGE_CODE CC
            WHERE 1=1
            AND CT.CHARGE_CODE = CC.CHARGE_CODE
            and form_code in (select form_code from form_detail_setup where table_name = 'SA_SALES_INVOICE') and CT.apply_on='D'";
            var chargeTitles = _objectEntity.SqlQuery<ChargesMap>(query).ToList();
            return chargeTitles;
        }

        public List<SalesVatWiseSummaryModel> GetSalesVatWiseSummaryDateWise(string formDate, string toDate)
        {
            string query = @"SELECT  B.CUSTOMER_EDESC as CustomerName ,B.TPIN_VAT_NO as VatNo,A.CUSTOMER_CODE as CustomerId,
             SUM(NVL(A.CALC_TOTAL_PRICE,0))  GrossAmount,SUM(NVL(A.CALC_QUANTITY,0))  Quantity
            FROM SA_SALES_INVOICE A, SA_CUSTOMER_SETUP B 
            WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE
            AND A.COMPANY_CODE = B.COMPANY_CODE
            AND A.DELETED_FLAG = 'N'";
            if (!string.IsNullOrEmpty(formDate))
                query = query + " and A.SALES_DATE>=TO_DATE('" + formDate + "', 'YYYY-MM-DD') and A.SALES_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            query = query + " GROUP BY  B.CUSTOMER_EDESC, A.CUSTOMER_CODE, A.COMPANY_CODE,B.TPIN_VAT_NO";
            var salesRegisters = _objectEntity.SqlQuery<SalesVatWiseSummaryModel>(query).ToList();
            return salesRegisters;
        }

        public List<SalesVatWiseSummaryModel> GetSalesVatWiseSummary(ReportFiltersModel filters)
        {

            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{this._workContext.CurrentUserinformation.company_code}'" : companyCode.Remove(companyCode.Length - 1);


            StringBuilder query = new StringBuilder();
            query.Append("SELECT  B.CUSTOMER_EDESC as CustomerName , NVL( B.TPIN_VAT_NO,'-') as VatNo,A.CUSTOMER_CODE as CustomerId,");
            query.AppendFormat("Round(SUM(FN_CONVERT_CURRENCY(NVL(A.CALC_TOTAL_PRICE,0),'NRS',A.SALES_DATE)/{1}),{0})  as GrossAmount,", ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter), ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter));
            query.AppendFormat("SUM(NVL(A.CALC_QUANTITY,0))/{0} Quantity ", ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter));
            query.AppendFormat(" FROM SA_SALES_INVOICE A, SA_CUSTOMER_SETUP B,IP_ITEM_MASTER_SETUP C WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE AND A.COMPANY_CODE = B.COMPANY_CODE AND A.ITEM_CODE = C.ITEM_CODE AND A.COMPANY_CODE = C.COMPANY_CODE AND A.DELETED_FLAG = 'N' AND A.COMPANY_CODE IN({0}) ", companyCode);
            if (!string.IsNullOrEmpty(filters.FromDate))
                query.AppendFormat(" and A.SALES_DATE>=TO_DATE('{0}', 'YYYY-MM-DD') and A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')", filters.FromDate, filters.ToDate);

            if (filters.CustomerFilter.Count > 0)
            {

                var customers = filters.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {

                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                query = query.AppendFormat(@" AND A.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join("','", customers));
                //query = query.AppendFormat(@" AND A.CUSTOMER_CODE IN ({0}) ", string.Join(",", filters.CustomerFilter).ToString());
            }
            if (filters.ProductFilter.Count > 0)
            {

                var products = filters.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G' )", products[i], companyCode);
                    else
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G' )", products[i], companyCode);

                }
                query = query.AppendFormat(@" AND A.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", productConditionQuery, string.Join("','", products));

                // query = query.AppendFormat(@" AND A.ITEM_CODE IN ({0}) ", string.Join(",", filters.ProductFilter).ToString());
            }
            if (filters.PartyTypeFilter.Count > 0)
            {
                query = query.AppendFormat(@" AND A.PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", filters.PartyTypeFilter).ToString());
            }
            if (filters.AreaTypeFilter.Count > 0)
            {
                query = query.AppendFormat(@" AND A.AREA_CODE IN ('{0}') ", string.Join("','", filters.AreaTypeFilter).ToString());
            }
            if (filters.CategoryFilter.Count > 0)
            {
                query = query.AppendFormat(@" AND C.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            }
            if (filters.EmployeeFilter.Count > 0)
            {
                query = query.AppendFormat(@" AND A.EMPLOYEE_CODE IN ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
            }
            if (filters.AgentFilter.Count > 0)
            {
                query = query.AppendFormat(@" AND A.AGENT_CODE IN ('{0}')", string.Join("','", filters.AgentFilter).ToString());
            }
            if (filters.DivisionFilter.Count > 0)
            {
                query = query.AppendFormat(@" AND A.DIVISION_CODE IN ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
            }
            if (filters.DocumentFilter.Count > 0)
            {
                query = query.AppendFormat(@" AND A.FORM_CODE IN ('{0}')", string.Join("','", filters.DocumentFilter).ToString());
            }
            if (filters.LocationFilter.Count > 0)
            {

                var locations = filters.LocationFilter;
                var locationConditionQuery = string.Empty;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationConditionQuery += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
            }

            if (filters.BranchFilter.Count > 0)
            {
                query = query.AppendFormat(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }

            query.Append(" GROUP BY  B.CUSTOMER_EDESC, A.CUSTOMER_CODE, A.COMPANY_CODE,B.TPIN_VAT_NO");
            var min = 0; var max = 0;
            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query.AppendFormat(@" Having SUM(FN_CONVERT_CURRENCY(NVL(A.CALC_TOTAL_PRICE,0),'NRS',A.SALES_DATE)) >= {0} and SUM(FN_CONVERT_CURRENCY(NVL(A.CALC_TOTAL_PRICE,0),'NRS',A.SALES_DATE)) <= {1}", (decimal)min, (decimal)max);
            var salesRegisters = _objectEntity.SqlQuery<SalesVatWiseSummaryModel>(query.ToString()).ToList();
            return salesRegisters;
        }

        public List<SalesRegisterMasterModel> SaleRegistersMasterDynamicDateWiseFilter(string formDate, string toDate)
        {
            string query = @"SELECT A.SALES_DATE as SalesDate ,A.MITI as Miti,A.SALES_NO as InvoiceNumber,A.CUSTOMER_EDESC as CustomerName,A.GROSS_AMOUNT as GrossAmount FROM
             (SELECT A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI, A.SALES_NO, B.CUSTOMER_EDESC,
             SUM(NVL(A.CALC_TOTAL_PRICE,0))  GROSS_AMOUNT
            FROM SA_SALES_INVOICE A, SA_CUSTOMER_SETUP B, IP_ITEM_MASTER_SETUP C
            WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE
            AND A.COMPANY_CODE = B.COMPANY_CODE
            AND A.ITEM_CODE = C.ITEM_CODE
            AND A.COMPANY_CODE = C.COMPANY_CODE
            AND A.DELETED_FLAG = 'N'";
            if (!string.IsNullOrEmpty(formDate))
                query = query + " and A.SALES_DATE>=TO_DATE('" + formDate + "', 'YYYY-MM-DD') and A.SALES_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            query = query + "GROUP BY A.SALES_DATE,  A.SALES_NO, B.CUSTOMER_EDESC) A ";
            var salesRegisters = _objectEntity.SqlQuery<SalesRegisterMasterModel>(query).ToList();
            return salesRegisters;
        }

        public List<SalesRegisterMasterModel> SaleRegistersMasterDynamic(ReportFiltersModel filters)
        {
            //var companyCode = string.Join(",", filters.CompanyFilter);
            ////companyCode = companyCode == "" ? this._workContext.CurrentUserinformation.company_code : companyCode;
            //if (string.IsNullOrEmpty(companyCode))
            //    companyCode = this._workContext.CurrentUserinformation.company_code;
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{this._workContext.CurrentUserinformation.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = string.Format(@"SELECT 
                                             A.FORM_CODE,
                                            -- A.FROM_LOCATION_CODE,
                                            A.COMPANY_CODE,A.BRANCH_CODE,
                                             to_char( A.SALES_DATE )as SalesDate ,A.MITI as Miti,A.SALES_NO as InvoiceNumber,A.CUSTOMER_EDESC as CustomerName,
                                              Round(NVL(FN_CONVERT_CURRENCY(NVL(A.GROSS_AMOUNT,0),'NRS',A.SALES_DATE),0)/{0},{1}) as GrossAmount 
                                              FROM
                                               (SELECT 
                                                  A.FORM_CODE,A.FROM_LOCATION_CODE,A.COMPANY_CODE,A.BRANCH_CODE,
                                                  A.PARTY_TYPE_CODE, A.CUSTOMER_CODE, 
                                                  A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI, A.SALES_NO, B.CUSTOMER_EDESC,
                                                  SUM(NVL(A.TOTAL_PRICE,0))  GROSS_AMOUNT
                                                  FROM SA_SALES_INVOICE A, SA_CUSTOMER_SETUP B, IP_ITEM_MASTER_SETUP C
                                                  WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE
                                                  AND A.COMPANY_CODE = B.COMPANY_CODE
                                                  AND A.ITEM_CODE = C.ITEM_CODE
                                                  AND A.COMPANY_CODE = C.COMPANY_CODE
                                                  AND A.COMPANY_CODE IN({2})
                                                  AND A.DELETED_FLAG = 'N'",
                                        ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter), companyCode);


            var min = 0;
            var max = 0;


            ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
            {
                query += string.Format(" and NVL(A.CALC_QUANTITY, 0)>={0} and NVL(A.CALC_QUANTITY, 0)<={1}", min, max);

            }

            ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);

            if (!(min == 0 && max == 0))
            {
                query += string.Format(" and NVL(FN_CONVERT_CURRENCY(NVL(A.CALC_UNIT_PRICE,0),'NRS',A.SALES_DATE),0) >={0} and NVL(FN_CONVERT_CURRENCY(NVL(A.CALC_UNIT_PRICE,0),'NRS',A.SALES_DATE),0) <={1}", min, max);

            }

            if (filters.CustomerFilter.Count > 0)
            {
                var customers = filters.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {

                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }
                var customerfilter = string.Empty;
                foreach (var product in customers)
                {
                    customerfilter += $@"'{product}',";
                }
                customerfilter = customerfilter.Remove(customerfilter.Length - 1);
                query = query + string.Format(@" AND A.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0}  OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG = 'I')) ", customerConditionQuery, customerfilter);

            }
            if (filters.ProductFilter.Count > 0)
            {

                var products = filters.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }
                var productfilter = string.Empty;
                foreach (var product in products)
                {
                    productfilter += $@"'{product}',";
                }
                productfilter = productfilter.Remove(productfilter.Length - 1);
                query = query + string.Format(@" AND A.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ({1}) AND GROUP_SKU_FLAG = 'I')) ", productConditionQuery, productfilter);



                // query = query + string.Format(@" AND A.ITEM_CODE IN ({0}) ", string.Join(",", filters.ProductFilter).ToString());
            }
            if (filters.PartyTypeFilter.Count > 0)
            {
                query = query + string.Format(@" AND A.PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", filters.PartyTypeFilter).ToString());
            }
            if (filters.AreaTypeFilter.Count > 0)
            {
                query = query + string.Format(@" AND A.AREA_CODE IN ('{0}') ", string.Join("','", filters.AreaTypeFilter).ToString());
            }
            if (filters.CategoryFilter.Count > 0)
            {
                query = query + string.Format(@" AND C.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            }
            if (filters.EmployeeFilter.Count > 0)
            {
                query = query + string.Format(@" AND  A.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
            }
            if (filters.AgentFilter.Count > 0)
            {
                query = query + string.Format(@" AND  A.AGENT_CODE IN  ('{0}')", string.Join("','", filters.AgentFilter).ToString());
            }
            if (filters.DivisionFilter.Count > 0)
            {
                query = query + string.Format(@" AND A.DIVISION_CODE IN ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
            }
            if (filters.DocumentFilter.Count > 0)
            {
                query = query + string.Format(@" AND  A.FORM_CODE  IN  ('{0}')", string.Join("','", filters.DocumentFilter).ToString());
            }
            if (filters.LocationFilter.Count > 0)
            {

                var locations = filters.LocationFilter;
                var locationConditionQuery = string.Empty;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationConditionQuery += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                var locationsfilter = string.Empty;
                foreach (var product in locations)
                {
                    locationsfilter += $@"'{product}',";
                }
                locationsfilter = locationsfilter.Remove(locationsfilter.Length - 1);
                query = query + string.Format(@" AND A.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, locationsfilter);
            }
            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }


            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and A.SALES_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and A.SALES_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";
            query = query + "GROUP BY A.SALES_DATE,  A.SALES_NO, B.CUSTOMER_EDESC,  A.PARTY_TYPE_CODE, A.CUSTOMER_CODE,A.FORM_CODE,A.COMPANY_CODE,A.BRANCH_CODE";

            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);

            if (!(min == 0 && max == 0))
            {
                query = query + string.Format(@" Having  SUM(NVL(FN_CONVERT_CURRENCY(NVL(A.CALC_TOTAL_PRICE, 0), 'NRS', A.SALES_DATE), 0))/{0} >= {1} and SUM(NVL(FN_CONVERT_CURRENCY(NVL(A.CALC_TOTAL_PRICE, 0), 'NRS', A.SALES_DATE), 0))/{0} <= {2}"
                    , ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), (decimal)min / ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), (decimal)max / ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter));
            }


            query += " ) A ORDER BY A.SALES_DATE ASC ";


            var salesRegisters = _objectEntity.SqlQuery<SalesRegisterMasterModel>(query).ToList();


            return salesRegisters;
        }
        public List<Charges> GetSumChargesDateWise(string formDate, string toDate)
        {
            string Query = @"SELECT CS.CUSTOMER_CODE as CustomerId, CS.CUSTOMER_EDESC ,
                 CT.charge_code as CHARGE_CODE,
                 CC.CHARGE_EDESC as ,
                 SUM (CT.CHARGE_AMOUNT) CHARGE_AMOUNT,
                 CT.APPLY_ON as APPLY_ON ,
                 CT.CHARGE_TYPE_FLAG as CHARGE_TYPE_FLAG
            FROM CHARGE_TRANSACTION CT, IP_CHARGE_CODE CC, SA_SALES_INVOICE SI, SA_CUSTOMER_SETUP CS
           WHERE     1 = 1
                 AND CT.CHARGE_CODE = CC.CHARGE_CODE
                 AND CT.form_code IN (SELECT form_code
                                        FROM form_detail_setup
                                       WHERE table_name = 'SA_SALES_INVOICE')
                 AND CT.apply_on = 'D'
                  AND SI.SALES_NO = CT.REFERENCE_NO
                 AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE";
            if (string.IsNullOrEmpty(formDate))
                Query = Query + "  AND CT.CREATED_DATE >= TO_DATE ('" + formDate + "', 'YYYY-MM-DD') AND CT.CREATED_DATE <= TO_DATE ('" + toDate + "', 'YYYY-MM-DD')";
            Query = Query + " GROUP BY CS.CUSTOMER_CODE,CS.CUSTOMER_EDESC,CT.charge_code,CC.CHARGE_EDESC,CT.APPLY_ON, CT.CHARGE_TYPE_FLAG ORDER BY CS.CUSTOMER_EDESC";

            var charges = _objectEntity.SqlQuery<Charges>(Query).ToList();
            return charges;
        }
        public List<Charges> GetSumCharges(ReportFiltersModel filters)
        {

            // var companyCode = string.Join(",", filters.CompanyFilter);

            //  if (string.IsNullOrEmpty(companyCode))
            //    companyCode = this._workContext.CurrentUserinformation.company_code;

            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{this._workContext.CurrentUserinformation.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            string Query = string.Format(@"SELECT CS.CUSTOMER_CODE as CustomerId, CS.CUSTOMER_EDESC ,
                 CT.charge_code||CT.APPLY_ON as charge_code,  CC.CHARGE_EDESC ||'(' || CT.APPLY_ON ||')' as CHARGE_EDESC,
                 NVL(Round(SUM (NVL(FN_CONVERT_CURRENCY(NVL(CT.CHARGE_AMOUNT,0),'NRS', SI.SALES_DATE),0))/{0},{1}),0) as CHARGE_AMOUNT,
                 CT.APPLY_ON as APPLY_ON ,
                 CT.CHARGE_TYPE_FLAG as CHARGE_TYPE_FLAG
            FROM CHARGE_TRANSACTION CT, IP_CHARGE_CODE CC, SA_SALES_INVOICE SI, SA_CUSTOMER_SETUP CS
           WHERE     1 = 1
   AND SI.SERIAL_NO = 1
                 AND CT.CHARGE_CODE = CC.CHARGE_CODE 
                 AND CT.COMPANY_CODE=CC.COMPANY_CODE
                 AND CT.COMPANY_CODE=SI.COMPANY_CODE
                 AND CT.COMPANY_CODE=CS.COMPANY_CODE
                 AND CT.CHARGE_CODE = CC.CHARGE_CODE AND CT.COMPANY_CODE IN({2})
                 AND CT.form_code IN (SELECT form_code
                                        FROM form_detail_setup
                                       WHERE table_name = 'SA_SALES_INVOICE')
                 AND CT.apply_on = 'D'
                  AND SI.SALES_NO = CT.REFERENCE_NO
                 AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE", ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), ReportFilterHelper.FigureFilterValue(filters.AmountRoundUpFilter), companyCode);
            if (!string.IsNullOrEmpty(filters.FromDate))
                Query = Query + string.Format(@"  AND CT.CREATED_DATE >= TO_DATE ('{0}', 'YYYY-MM-DD') AND CT.CREATED_DATE <= TO_DATE ('{1}', 'YYYY-MM-DD')", filters.FromDate, filters.ToDate);
            Query = Query + " GROUP BY CS.CUSTOMER_CODE,CS.CUSTOMER_EDESC,CT.charge_code,CC.CHARGE_EDESC,CT.APPLY_ON, CT.CHARGE_TYPE_FLAG ORDER BY CS.CUSTOMER_EDESC";

            var charges = _objectEntity.SqlQuery<Charges>(Query).ToList();
            return charges;
        }
        public List<SalesRegistersDetail> GetSalesRegisterDateWise(string formDate, string toDate)
        {
            string query = @"SELECT sales_date as SalesDate,
       bs_date (sales_date) Miti ,
       sales_no as InvoiceNumber,
       INITCAP (CS.CUSTOMER_EDESC) CustomerName,
       INITCAP (IMS.ITEM_EDESC) ItemName,
       INITCAP (ls.location_edesc) LocationName,
       SI.MANUAL_NO as ManualNo,
       SI.REMARKS as REMARKS,
       INITCAP (ES.EMPLOYEE_EDESC) Dealer,
       INITCAP (PTC.PARTY_TYPE_EDESC) PartyType,
       SI.SHIPPING_ADDRESS as ShippingAddress,
       SI.SHIPPING_CONTACT_NO as ShippingContactNo,
       INITCAP (MC.MU_EDESC) Unit ,
       SI.QUANTITY as Quantity,
       SI.UNIT_PRICE as UnitPrice,
       SI.TOTAL_PRICE as TotalPrice
  FROM SA_SALES_INVOICE si,
       IP_ITEM_MASTER_SETUP ims,
       SA_CUSTOMER_SETUP cs,
       IP_LOCATION_SETUP ls,
       IP_MU_CODE mc,
       HR_EMPLOYEE_SETUP es,
       IP_PARTY_TYPE_CODE ptc
WHERE     SI.ITEM_CODE = IMS.ITEM_CODE
       AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
       AND SI.FROM_LOCATION_CODE = LS.LOCATION_CODE
       AND SI.MU_CODE = MC.MU_CODE
  and si.Deleted_flag='N'
      AND SI.EMPLOYEE_CODE = ES.EMPLOYEE_CODE
       AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE(+)";
            if (!string.IsNullOrEmpty(formDate))
                query = query + " and SALES_DATE>=TO_DATE('" + formDate + "', 'YYYY-MM-DD') and SALES_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            var salesRegisters = _objectEntity.SqlQuery<SalesRegistersDetail>(query).ToList();
            return salesRegisters;
        }
        public List<SalesRegistersDetail> GetSalesRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = string.Format(@"SELECT TO_CHAR(sales_date) as SalesDate,
                       bs_date (sales_date) Miti ,
                       sales_no as InvoiceNumber,
                       INITCAP (CS.CUSTOMER_EDESC) CustomerName,
                       INITCAP (IMS.ITEM_EDESC) ItemName,
                       INITCAP (ls.location_edesc) LocationName,
                       NVL(SI.MANUAL_NO,'n/a') as ManualNo,
                       NVL(SI.REMARKS,'n/a') as REMARKS,
                       NVL(INITCAP (ES.EMPLOYEE_EDESC),'n/a') Dealer,
                       NVL(INITCAP (PTC.PARTY_TYPE_EDESC),'n/a') PartyType,
                       SI.SHIPPING_ADDRESS as SHIPPINGCODE,
                       NVL(SI.SHIPPING_CONTACT_NO,'-') as ShippingContactNo,
                       CT.CITY_EDESC as ShippingAddress,
                        ast.AREA_EDESC,
                       INITCAP (SI.MU_CODE) Unit ,
                       Round(NVL(SI.QUANTITY,0)/{0},{1}) as Quantity,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_UNIT_PRICE,0),'NRS',SI.SALES_DATE),0)/{2},{3}) as UnitPrice,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_TOTAL_PRICE,0),'NRS',SI.SALES_DATE),0)/{4},{5}) as TotalPrice
                        FROM SA_SALES_INVOICE si,
                       IP_ITEM_MASTER_SETUP ims,
                       SA_CUSTOMER_SETUP cs,
                       IP_LOCATION_SETUP ls,
                       HR_EMPLOYEE_SETUP es,
                       IP_PARTY_TYPE_CODE ptc,
                             CITY_CODE ct,
                        AREA_SETUP ast
                       WHERE SI.ITEM_CODE = IMS.ITEM_CODE
                        and si.company_code=IMS.company_code
                        and si.AREA_CODE = ast.AREA_CODE
                       and SI.COMPANY_CODE=cs.company_code
                       and SI.company_code=ls.company_code
                          and  SI.SHIPPING_ADDRESS= ct.city_code
                       AND si.COMPANY_CODE IN(" + companyCode + @") AND si.CUSTOMER_CODE = cs.CUSTOMER_CODE"
                     , ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.QuantityRoundUpFilter), ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter)
                        , ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter));
            if (filters.CustomerFilter.Count > 0)
            {

                var customers = filters.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {

                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND SI.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join("','", customers));
            }
            if (filters.ProductFilter.Count > 0)
            {

                var products = filters.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND SI.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", productConditionQuery, string.Join("','", products));

                //query = query + string.Format(@" AND SI.ITEM_CODE IN ({0})", string.Join(",", filters.ProductFilter).ToString());
            }
            if (filters.DocumentFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI. FORM_CODE  IN  ('{0}')", string.Join("','", filters.DocumentFilter).ToString());
            }
            if (filters.PartyTypeFilter.Count > 0)
            {
                query = query + string.Format(@" AND SI.PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", filters.PartyTypeFilter).ToString());
            }
            if (filters.AreaTypeFilter.Count > 0)
            {
                query = query + string.Format(@" AND SI.AREA_CODE IN ('{0}') ", string.Join("','", filters.AreaTypeFilter).ToString());
            }
            if (filters.CategoryFilter.Count > 0)
            {
                query = query + string.Format(@" AND ims.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            }
            if (filters.EmployeeFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
            }
            if (filters.AgentFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.AGENT_CODE IN  ('{0}')", string.Join("','", filters.AgentFilter).ToString());
            }
            if (filters.DivisionFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.DIVISION_CODE IN  ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
            }
            if (filters.LocationFilter.Count > 0)
            {

                var locations = filters.LocationFilter;
                var locationConditionQuery = string.Empty;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%'", locations[i]);
                    else
                    {
                        locationConditionQuery += string.Format(" OR  LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                query = query + string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                //query = query + string.Format(@" AND (SI.FROM_LOCATION_CODE = LS.LOCATION_CODE OR SI.FROM_LOCATION_CODE IN ('{0}'))", string.Join("','", filters.LocationFilter).ToString());
            }
            //if (filters.CompanyFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND si.COMPANY_CODE = cmps.COMPANY_CODE AND SI.COMPANY_CODE IN ('{0}')", string.Join("','", filters.CompanyFilter).ToString());
            //}
            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            //EMPLOYEE_CODE doesn't exist on the NNPL &WLINK database , So We have manually created column of EMPLOYEE_CODE for the both database .
            query = query + @" AND SI.FROM_LOCATION_CODE = LS.LOCATION_CODE
                        and si.Deleted_flag = 'N'
                      AND SI.EMPLOYEE_CODE = ES.EMPLOYEE_CODE(+)
                       AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE(+)";
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and SALES_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and SALES_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";


            int min = 0, max = 0;

            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.SALES_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.SALES_DATE),0)  <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(SI.QUANTITY,0) >= {0} and NVL(SI.QUANTITY,0) <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.SALES_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.SALES_DATE),0) <= {1}", min, max);


            var salesRegisters = _objectEntity.SqlQuery<SalesRegistersDetail>(query).ToList();
            return salesRegisters;
        }
        public List<SalesRegistersDetail> GetSalesRegisterDateWisePaging(string formDate, string toDate, int pageSize, int pageNumber)
        {
            string query = @" select *
                        from (
                                select rownum as rn, a.*
                                from (
                                      SELECT sales_date as SalesDate,
                               bs_date (sales_date) Miti ,
                               sales_no as InvoiceNumber,
                               INITCAP (CS.CUSTOMER_EDESC) CustomerName,
                               INITCAP (IMS.ITEM_EDESC) ItemName,
                               INITCAP (ls.location_edesc) LocationName,
                               SI.MANUAL_NO as ManualNo,
                               SI.REMARKS as REMARKS,
                               INITCAP (ES.EMPLOYEE_EDESC) Dealer,
                               INITCAP (PTC.PARTY_TYPE_EDESC) PartyType,
                               SI.SHIPPING_ADDRESS as ShippingAddress,
                               SI.SHIPPING_CONTACT_NO as ShippingContactNo,
                               INITCAP (MC.MU_EDESC) Unit ,
                               SI.QUANTITY as Quantity,
                               SI.UNIT_PRICE as UnitPrice,
                               SI.TOTAL_PRICE as TotalPrice
                          FROM SA_SALES_INVOICE si,
                               IP_ITEM_MASTER_SETUP ims,
                               SA_CUSTOMER_SETUP cs,
                               IP_LOCATION_SETUP ls,
                               IP_MU_CODE mc,
                               HR_EMPLOYEE_SETUP es,
                               IP_PARTY_TYPE_CODE ptc
                        WHERE     SI.ITEM_CODE = IMS.ITEM_CODE
                               AND SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
                               AND SI.FROM_LOCATION_CODE = LS.LOCATION_CODE
                               AND SI.MU_CODE = MC.MU_CODE
                               and si.Deleted_flag='N'
                               AND SI.EMPLOYEE_CODE = ES.EMPLOYEE_CODE
                               AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE(+)";
            if (!string.IsNullOrEmpty(formDate))
                query = query + " and SALES_DATE>=TO_DATE('" + formDate + "', 'YYYY-MM-DD') and SALES_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            query = query + @") a)
                        where rownum <= " + pageSize + " and rn > (" + pageNumber + " - 1)*" + pageSize + "";
            var salesRegisters = _objectEntity.SqlQuery<SalesRegistersDetail>(query).ToList();
            return salesRegisters;
        }
        public int TotalSalesRegister(string formDate, string toDate)
        {
            string query = @"select count(sales_date) from SA_SALES_INVOICE";
            if (!string.IsNullOrEmpty(formDate))
                query = query + " where SALES_DATE>=TO_DATE('" + formDate + "', 'YYYY-MM-DD') and SALES_DATE <= TO_DATE('" + toDate + "', 'YYYY-MM-DD')";
            var salesRegisters = _objectEntity.SqlQuery<int>(query).First();
            return salesRegisters;
        }
        public List<ChartSalesModel> GetCategorySales()
        {
            string query = @" SELECT IPC.CATEGORY_CODE Code,IPC.CATEGORY_EDESC DESCRIPTION, SUM(SSI.CALC_TOTAL_PRICE) TOTAL , SUM(SSI.CALC_QUANTITY) QUANTITY FROM IP_CATEGORY_CODE  IPC
                                  INNER JOIN  IP_ITEM_MASTER_SETUP IIMS on IPC.CATEGORY_CODE = IIMS.CATEGORY_CODE
                                  INNER JOIN SA_SALES_INVOICE SSI on SSI.ITEM_CODE = IIMS.ITEM_CODE WHERE IPC.DELETED_FLAG = 'N'
                                  GROUP BY IPC.CATEGORY_CODE, IPC.CATEGORY_EDESC";

            var categorySales = _objectEntity.SqlQuery<ChartSalesModel>(query);

            return categorySales.ToList();

        }
        public List<ChartSalesModel> GetAreasSales()
        {
            string query = @"  SELECT * FROM (select A.AREA_CODE Code,A.AREA_EDESC DESCRIPTION, ROUND(SUM(SA.NET_SALES_RATE),2) TOTAL, ROUND(SUM(SA.CALC_QUANTITY),2) QUANTITY from SA_SALES_INVOICE SA,AREA_SETUP A
                                  WHERE A.AREA_CODE=SA.AREA_CODE
                                  AND A.COMPANY_CODE=SA.COMPANY_CODE
                                  AND SA.DELETED_FLAG='N'
                                  GROUP BY A.AREA_CODE,A.AREA_EDESC) AR ORDER BY TOTAL DESC";

            var categorySales = _objectEntity.SqlQuery<ChartSalesModel>(query);

            return categorySales.ToList();

        }


        public List<ChartSalesModel> GetProductSalesByMonth(ReportFiltersModel reportFilters, User userinfo, string dateFormat, string month)
        {
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? this._workContext.CurrentUserinformation.company_code : companyCode;

            string query = string.Empty;
            if (dateFormat == "AD")
            {
                query = string.Format(@"SELECT IIMS.ITEM_CODE Code, IIMS.ITEM_EDESC DESCRIPTION,
                            SUM (nvl(SSI.CALC_TOTAL_PRICE,0))/{0} GrossAmount , 
                           SUM (nvl(SSI.CALC_QUANTITY,0))/{0} QUANTITY FROM
                           IP_ITEM_MASTER_SETUP IIMS 
                          INNER JOIN SA_SALES_INVOICE SSI on SSI.ITEM_CODE = IIMS.ITEM_CODE  and ssi.Company_code=iims.company_code
                          WHERE IIMS.DELETED_FLAG = 'N' AND TO_CHAR(SSI.sales_date, 'YYYYMM') = '{1}'
                          and ssi.company_code IN({2})
                          GROUP BY IIMS.ITEM_CODE, IIMS.ITEM_EDESC", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), month, companyCode);
            }
            else
            {
                query = string.Format(@"SELECT IIMS.ITEM_CODE Code, IIMS.ITEM_EDESC DESCRIPTION,
                            SUM (nvl(SSI.CALC_TOTAL_PRICE,0))/{0} GrossAmount , 
                           SUM (nvl(SSI.CALC_QUANTITY,0))/{0} QUANTITY FROM
                           IP_ITEM_MASTER_SETUP IIMS 
                          INNER JOIN SA_SALES_INVOICE SSI on SSI.ITEM_CODE = IIMS.ITEM_CODE  and ssi.Company_code=iims.company_code
                          WHERE IIMS.DELETED_FLAG = 'N' AND SUBSTR(BS_DATE(SSI.sales_date),6,2) = '{1}'
                          and ssi.company_code IN({2})
                          GROUP BY IIMS.ITEM_CODE, IIMS.ITEM_EDESC", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), month, companyCode);
            }



            var productSales = _objectEntity.SqlQuery<ChartSalesModel>(query);

            return productSales.ToList();
        }



        public List<ChartSalesModel> GetProductSalesByCategory(ReportFiltersModel reportFilters, User userinfo, string categoryCode)
        {
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == null ? this._workContext.CurrentUserinformation.company_code : companyCode;

            string query = string.Format(@"SELECT IIMS.ITEM_CODE Code, IIMS.ITEM_EDESC DESCRIPTION,
                            SUM (nvl(SSI.CALC_TOTAL_PRICE,0))/{0} TOTAL , 
                           SUM (nvl(SSI.CALC_QUANTITY,0))/{0} QUANTITY FROM
                           IP_ITEM_MASTER_SETUP IIMS 
                          INNER JOIN SA_SALES_INVOICE SSI on SSI.ITEM_CODE = IIMS.ITEM_CODE 
                          WHERE IIMS.DELETED_FLAG = 'N' AND IIMS.CATEGORY_CODE= '{1}'
                          and ssi.company_code IN({2})
                          GROUP BY IIMS.ITEM_CODE, IIMS.ITEM_EDESC", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), categoryCode, companyCode);

            var productSales = _objectEntity.SqlQuery<ChartSalesModel>(query);

            return productSales.ToList();
        }

        public List<ChartSalesModel> GetProductSalesByCategory(ReportFiltersModel reportFilters, User userInfo, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {

            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? this._workContext.CurrentUserinformation.company_code : companyCode;

            string Query = string.Format(@"SELECT  IM.ITEM_CODE Code, IM.ITEM_EDESC DESCRIPTION,
                                                    SUM(nvl(SI.CALC_TOTAL_PRICE,0))/{0} TOTAL , 
                                                    SUM(nvl(SI.CALC_QUANTITY,0))/1 QUANTITY 
                                            FROM SA_SALES_INVOICE SI                         
                                            INNER JOIN  IP_ITEM_MASTER_SETUP IM on SI.ITEM_CODE = IM.ITEM_CODE                         
                                            WHERE SI.DELETED_FLAG = 'N' AND IM.DELETED_FLAG= 'N'                           
                                              AND IM.ITEM_CODE = SI.ITEM_CODE  
                                              AND IM.COMPANY_CODE = SI.COMPANY_CODE
                                              AND SI.company_code IN({2}) 
                                              AND IM.CATEGORY_CODE= '{1}'"
                                             , ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), categoryCode, companyCode);


            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }




            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";

            }
            //For Area Filter
            if (reportFilters.AreaTypeFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.AREA_CODE IN ('{0}')", string.Join("','", reportFilters.AreaTypeFilter).ToString());
            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }

            Query += " GROUP BY IM.ITEM_CODE , IM.ITEM_EDESC";

            //  Query += string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            var productSales = _objectEntity.SqlQuery<ChartSalesModel>(Query);

            return productSales.ToList();
        }

        public List<ChartSalesModel> GetStockLevelByCategory(ReportFiltersModel reportFilters, User userInfo, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {

            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var FiscalYearCode = string.Empty;
            if (reportFilters.FiscalYearFilter.Count > 0)
            {
                FiscalYearCode = $"{reportFilters.FiscalYearFilter.FirstOrDefault().DBName}.";
            }

            var query = $@"select ITEM_CODE Code,ITEM_EDESC Description,stock_value Total from(  select ITEM_CODE, ITEM_EDESC,ROUND(SUM({FiscalYearCode}FN_AVG_RATE_INFO (company_code, branch_code ,ITEM_CODE,sysdate) * QTY),2) stock_value from (
                       select VI.ITEM_CODE,IP.ITEM_EDESC, VI.company_code, VI.branch_code, sum(VI.IN_QUANTITY -  VI.OUT_QUANTITY ) QTY
                        from {FiscalYearCode}V$VIRTUAL_STOCK_WIP_LEDGER VI , {FiscalYearCode}IP_ITEM_MASTER_SETUP  IP 
                       WHERE  VI.COMPANY_CODE=IP.COMPANY_CODE
                       AND VI.ITEM_CODE=IP.ITEM_CODE
                       AND IP.GROUP_SKU_FLAG='I'
                       AND VI.DELETED_FLAG='N'
                       AND VI.BRANCH_CODE=IP.BRANCH_CODE
                        AND VI.COMPANY_CODE IN({companyCode})  
                        AND IP.CATEGORY_CODE='{categoryCode}'";
            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = $@"select  DISTINCT item_code from {FiscalYearCode}IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += $"MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from {FiscalYearCode}IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                query += " and VI.ITEM_CODE IN(" + productFilter + ")";
            }





            query += @" group by VI.item_code,IP.ITEM_EDESC, VI.company_code, VI.branch_code) 
                        group by ITEM_CODE, ITEM_EDESC) where stock_value<>0";

            //  Query += string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            var productSales = _objectEntity.SqlQuery<ChartSalesModel>(query);

            return productSales.ToList();
        }
        public List<ChartSalesModel> GetProductSalesByArea(ReportFiltersModel reportFilters, User userInfo, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {

            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? this._workContext.CurrentUserinformation.company_code : companyCode;

            string Query = string.Format($@"SELECT  IM.PARTY_TYPE_CODE Code, IM.PARTY_TYPE_EDESC DESCRIPTION,
                                                    SUM(nvl(SI.CALC_TOTAL_PRICE,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} TOTAL , 
                                                    SUM(nvl(SI.CALC_QUANTITY,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} QUANTITY 
                                            FROM SA_SALES_INVOICE SI                         
                                            INNER JOIN  IP_PARTY_TYPE_CODE IM on SI.party_type_code = IM.PARTY_TYPE_CODE                         
                                            WHERE SI.DELETED_FLAG = 'N' AND IM.DELETED_FLAG= 'N'                           
                                              AND IM.PARTY_TYPE_CODE = SI.PARTY_TYPE_CODE  
                                              AND IM.COMPANY_CODE = SI.COMPANY_CODE
                                              AND SI.company_code IN({companyCode}) 
                                              AND SI.AREA_CODE= '{categoryCode}'");


            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }




            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";

            }
            //FOR AREA FILTER
            if (reportFilters.AreaTypeFilter.Count() > 0)
            {
                Query += " AND SI.AREA_CODE IN (" + string.Join(",", reportFilters.AreaTypeFilter) + ")";

            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }

            Query += " GROUP BY IM.PARTY_TYPE_CODE , IM.PARTY_TYPE_EDESC";

            //  Query += string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            var productSales = _objectEntity.SqlQuery<ChartSalesModel>(Query);

            return productSales.ToList();
        }

        public List<ChartSalesModel> GetProductSalesByAreaEmployee(ReportFiltersModel reportFilters, User userInfo, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {

            companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? this._workContext.CurrentUserinformation.company_code : companyCode;

            string Query = string.Format($@"SELECT  IM.employee_code Code, IM.employee_edesc DESCRIPTION,
                                                    SUM(nvl(SI.CALC_TOTAL_PRICE,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} TOTAL , 
                                                    SUM(nvl(SI.CALC_QUANTITY,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)} QUANTITY 
                                            FROM SA_SALES_INVOICE SI                         
                                            INNER JOIN  HR_EMPLOYEE_SETUP IM on SI.employee_code = IM.employee_code                         
                                            WHERE SI.DELETED_FLAG = 'N' AND IM.DELETED_FLAG= 'N'                           
                                              AND IM.employee_code = SI.employee_code  
                                              AND IM.COMPANY_CODE = SI.COMPANY_CODE
                                              AND SI.company_code IN({companyCode}) 
                                              AND SI.AREA_CODE= '{categoryCode}'");


            //for customer Filter
            //var customerFilter = string.Empty;
            //if (reportFilters.CustomerFilter.Count() > 0)
            //{
            //    customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
            //    //IF CUSTOMER_SKU_FLAG = G
            //    foreach (var item in reportFilters.CustomerFilter)
            //    {
            //        customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
            //    }
            //    customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
            //    //IF CUSTOMER_SKU_FLAG = I                
            //    customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


            //    Query += " and SI.customer_code IN(" + customerFilter + ")";
            //}




            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }




            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";

            }
            //FOR AREA FILTER
            if (reportFilters.AreaTypeFilter.Count() > 0)
            {
                Query += " AND SI.AREA_CODE IN (" + string.Join(",", reportFilters.AreaTypeFilter) + ")";

            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            //if (reportFilters.AgentFilter.Count > 0)
            //{
            //    Query = Query + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            //}
            //string locationFilter = string.Empty;
            //if (reportFilters.LocationFilter.Count > 0)
            //{

            //    var locations = reportFilters.LocationFilter;
            //    for (int i = 0; i < locations.Count; i++)
            //    {

            //        if (i == 0)
            //            locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
            //        else
            //        {
            //            locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
            //        }
            //    }
            //    locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
            //    //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
            //    Query = Query + locationFilter;
            //}

            Query += " GROUP BY  IM.employee_code , IM.employee_edesc order by IM.employee_edesc";

            //  Query += string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));
            var productSales = _objectEntity.SqlQuery<ChartSalesModel>(Query);

            return productSales.ToList();
        }

        public List<ChartSalesModel> GetCategorySales(ReportFiltersModel reportFilters, User userInfo)
        {
            var companyCode = string.Join(",", reportFilters.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            string query = string.Format(@"SELECT  IPC.CATEGORY_CODE Code,
                                    IPC.CATEGORY_EDESC DESCRIPTION, 
                                    SUM(nvl(SSI.CALC_TOTAL_PRICE,0))/{0} TOTAL , 
                                    SUM(nvl(SSI.CALC_QUANTITY,0))/{0} QUANTITY 
                            FROM IP_CATEGORY_CODE  IPC
                            INNER JOIN  IP_ITEM_MASTER_SETUP IIMS on IPC.CATEGORY_CODE = IIMS.CATEGORY_CODE
                            INNER JOIN SA_SALES_INVOICE SSI on SSI.ITEM_CODE = IIMS.ITEM_CODE 
                            WHERE IPC.DELETED_FLAG = 'N'
                            AND SSI.COMPANY_CODE IN({1})
                            GROUP BY IPC.CATEGORY_CODE, IPC.CATEGORY_EDESC", ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter), companyCode);

            var categorySales = _objectEntity.SqlQuery<ChartSalesModel>(query);
            return categorySales.ToList();
        }


        public List<CategoryWiseSalesModel> GetCategoryStockLevel(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            //companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            var FiscalYearCode = string.Empty;
            if (reportFilters.FiscalYearFilter.Count > 0)
            {
                FiscalYearCode += $@"{reportFilters.FiscalYearFilter.FirstOrDefault().DBName}.";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = $@" SELECT CATEGORY_CODE as Code,CATEGORY_EDESC as Description,round(STOCK_VALUE,2) as Total FROM( select CATEGORY_CODE,CATEGORY_EDESC, SUM({FiscalYearCode}FN_AVG_RATE_INFO (company_code, branch_code ,ITEM_CODE,sysdate) * QTY) stock_value from (
                       select VI.ITEM_CODE, VI.company_code, VI.branch_code,CT.CATEGORY_CODE,CT.CATEGORY_EDESC, sum(VI.IN_QUANTITY -  VI.OUT_QUANTITY ) QTY
                        from {FiscalYearCode}V$VIRTUAL_STOCK_WIP_LEDGER VI , {FiscalYearCode}IP_ITEM_MASTER_SETUP  IP 
                       ,{FiscalYearCode}IP_CATEGORY_CODE CT 
                       WHERE  VI.COMPANY_CODE=IP.COMPANY_CODE
                       AND VI.ITEM_CODE=IP.ITEM_CODE
                       AND TRIM(IP.CATEGORY_CODE)= TRIM(CT.CATEGORY_CODE)
                       AND IP.COMPANY_CODE=CT.COMPANY_CODE
                       AND IP.GROUP_SKU_FLAG='I'
                       AND VI.DELETED_FLAG='N'
                       AND VI.COMPANY_CODE=CT.COMPANY_CODE
                       AND VI.BRANCH_CODE=IP.BRANCH_CODE
                        AND VI.COMPANY_CODE IN({companyCode})";



            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = $@"select  DISTINCT item_code from {FiscalYearCode}IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += $"MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from {FiscalYearCode}IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and VI.ITEM_CODE IN(" + productFilter + ")";
            }




            Query += @"  group by CT.CATEGORY_CODE,CT.CATEGORY_EDESC,VI.item_code, VI.company_code, VI.branch_code)    
                       GROUP BY     CATEGORY_CODE,CATEGORY_EDESC) WHERE STOCK_VALUE<>0 ORDER BY CATEGORY_EDESC";


            //  Query = string.Format(Query);

            var categorySales = _objectEntity.SqlQuery<CategoryWiseSalesModel>(Query).ToList();

            return categorySales.ToList();
        }
        public List<ChartSalesModel> GetCategorySales(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            //companyCode = string.Join(",", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            //var companyCode = string.Join(",'", reportFilters.CompanyFilter);
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var FiscalYearCode = string.Empty;
            if (reportFilters.FiscalYearFilter.Count > 0)
            {
                FiscalYearCode += $@"{reportFilters.FiscalYearFilter.FirstOrDefault().DBName}.";
            }
            string Query = $@"SELECT  IPC.CATEGORY_CODE Code,
                                    IPC.CATEGORY_EDESC DESCRIPTION, 
                                    --to_number(round(SUM(NET_SALES_RATE*CALC_QUANTITY),2)) TOTAL , 
                                    --to_number(round(SUM(CALC_UNIT_PRICE*CALC_QUANTITY),2)) TOTAL ,
                                    TO_NUMBER (ROUND (SUM (CALC_UNIT_PRICE * CALC_QUANTITY), 2))-TO_NUMBER (ROUND (SUM (nvl(c.CHARGE_AMOUNT,0)), 2)) TOTAL,
                                    SUM(nvl(SI.CALC_QUANTITY,0)) QUANTITY 
                            FROM {FiscalYearCode}IP_CATEGORY_CODE  IPC                           
                            INNER JOIN  {FiscalYearCode}IP_ITEM_MASTER_SETUP IM on IPC.CATEGORY_CODE = IM.CATEGORY_CODE
                            INNER JOIN {FiscalYearCode}SA_SALES_INVOICE SI on SI.ITEM_CODE = IM.ITEM_CODE  
                            -- added by chandra for get total amount after discount
                             LEFT JOIN charge_transaction c
                            ON     SI.SALES_NO = C.REFERENCE_NO
                               AND SI.FORM_CODE = C.FORM_CODE
                               AND SI.COMPANY_CODE = C.COMPANY_CODE
                               AND CHARGE_CODE IN (SELECT DISTINCT CHARGE_CODE
                                                     FROM IP_CHARGE_CODE
                                                    WHERE SPECIFIC_CHARGE_FLAG = 'D')
                               AND APPLY_ON = 'D'
                               AND c.DELETED_FLAG = 'N'
                            --end added by chandra
                            WHERE IPC.DELETED_FLAG = 'N' AND SI.DELETED_FLAG = 'N' AND IM.DELETED_FLAG= 'N'
                              AND IPC.COMPANY_CODE = SI.COMPANY_CODE
                              AND IM.COMPANY_CODE = IPC.COMPANY_CODE
                              AND IM.ITEM_CODE = SI.ITEM_CODE     
                              AND SI.company_code IN(" + companyCode + ")";

            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = $@"select  DISTINCT(customer_code) from {FiscalYearCode}sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += $"master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from {FiscalYearCode}SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }


            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = $@"select  DISTINCT item_code from {FiscalYearCode}IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += $"MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from {FiscalYearCode}IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }


            //for category Filter
            if (!string.IsNullOrEmpty(categoryCode))
            {
                Query += " and (";
                foreach (var item in categoryCode.Split(','))
                {
                    Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
            }




            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";

            }
            if (reportFilters.AreaTypeFilter.Count > 0)
            {
                Query += string.Format(@" AND SI.AREA_CODE IN ('{0}')", string.Join("','", reportFilters.AreaTypeFilter).ToString());
            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format($"SELECT LOCATION_CODE FROM {FiscalYearCode}IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{locations[i]}%' ");
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }


            Query += " GROUP BY IPC.CATEGORY_CODE, IPC.CATEGORY_EDESC";


            Query = string.Format(Query);

            var categorySales = _objectEntity.SqlQuery<ChartSalesModel>(Query);

            return categorySales.ToList();
        }

        public List<ChartSalesModel> GetAreaSales(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string Query = $@"SELECT  TO_CHAR(IPC.area_code) Code,
                                    IPC.area_edesc DESCRIPTION, 
                                   ROUND(SUM(nvl(SI.NET_SALES_RATE*SI.CALC_QUANTITY,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)},2) TOTAL , 
                                   ROUND( SUM(nvl(SI.CALC_QUANTITY,0))/{ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter)},2) QUANTITY 
                            FROM AREA_SETUP  IPC                           
                            INNER JOIN SA_SALES_INVOICE SI on SI.AREA_CODE = IPC.AREA_CODE            
                            INNER JOIN IP_ITEM_MASTER_SETUP IP on IP.ITEM_CODE=SI.ITEM_CODE                 
                            WHERE IPC.DELETED_FLAG = 'N' AND SI.DELETED_FLAG = 'N' 
                              AND IPC.COMPANY_CODE = SI.COMPANY_CODE
                              AND IP.COMPANY_CODE=SI.COMPANY_CODE
                            --  AND IM.COMPANY_CODE = IPC.COMPANY_CODE
                         --     AND IM.ITEM_CODE = SI.ITEM_CODE     
                              AND SI.company_code IN({companyCode})";

            //for customer Filter
            var customerFilter = string.Empty;
            if (reportFilters.CustomerFilter.Count() > 0)
            {
                customerFilter = @"select  DISTINCT(customer_code) from sa_customer_setup where (";
                //IF CUSTOMER_SKU_FLAG = G
                foreach (var item in reportFilters.CustomerFilter)
                {
                    customerFilter += "master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                customerFilter = customerFilter.Substring(0, customerFilter.Length - 3);
                //IF CUSTOMER_SKU_FLAG = I                
                customerFilter += " or (customer_code in (" + string.Join(",", reportFilters.CustomerFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.customer_code IN(" + customerFilter + ")";
            }


            ////for Product Filter
            var productFilter = string.Empty;
            if (reportFilters.ProductFilter.Count() > 0)
            {
                productFilter = @"select  DISTINCT item_code from IP_ITEM_MASTER_SETUP where (";
                //IF PRODUCT_SKU_FLAG = G
                foreach (var item in reportFilters.ProductFilter)
                {
                    productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE IN(" + companyCode + ")) OR ";
                }
                productFilter = productFilter.Substring(0, productFilter.Length - 3);
                //IF PRODUCT_SKU_FLAG = I                
                productFilter += " or (ITEM_CODE in (" + string.Join(",", reportFilters.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + companyCode + "))) ";


                Query += " and SI.ITEM_CODE IN(" + productFilter + ")";
            }


            //for category Filter
            if (!string.IsNullOrEmpty(categoryCode))
            {
                Query += " and (";
                foreach (var item in categoryCode.Split(','))
                {
                    Query += "IM.CATEGORY_CODE = '" + item + "' OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
            }




            //FOR BRANCH FILTER
            if (reportFilters.BranchFilter.Count() > 0)
            {
                Query += " AND SI.BRANCH_CODE IN (" + string.Join(",", reportFilters.BranchFilter) + ")";

            }
            //FOR AREA FILTER
            if (reportFilters.AreaTypeFilter.Count() > 0)
            {
                Query += " AND SI.AREA_CODE IN (" + string.Join(",", reportFilters.AreaTypeFilter) + ")";

            }
            if (reportFilters.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", reportFilters.EmployeeFilter).ToString());
            }
            if (reportFilters.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND SI.AGENT_CODE IN  ('{0}')", string.Join("','", reportFilters.AgentFilter).ToString());
            }
            string locationFilter = string.Empty;
            if (reportFilters.LocationFilter.Count > 0)
            {

                var locations = reportFilters.LocationFilter;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationFilter += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%' ", locations[i]);
                    else
                    {
                        locationFilter += string.Format(" OR LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                locationFilter = string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationFilter, string.Join("','", locations));
                //query = query.AppendFormat(@" AND A.FROM_LOCATION_CODE IN ('{0}')", string.Join("','", filters.LocationFilter).ToString());
                Query = Query + locationFilter;
            }


            Query += " GROUP BY IPC.AREA_CODE, IPC.AREA_EDESC";


            // Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));

            var categorySales = _objectEntity.SqlQuery<ChartSalesModel>(Query);

            return categorySales.ToList();
        }

        public List<ChartSalesModel> GetNoOfbills(ReportFiltersModel reportFilters, User userInfo, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            string Query = $@"    select BI.BRANCH_CODE CODE,count(MI.voucher_no) TOTAL,BI.BRANCH_EDESC DESCRIPTION
                               from master_transaction MI,FA_BRANCH_SETUP BI
                              where MI.BRANCH_CODE=BI.BRANCH_CODE AND MI.COMPANY_CODE=BI.COMPANY_CODE AND MI.COMPANY_CODE IN ({companyCode}) AND
                               MI.form_code  in
                                (select distinct form_code from form_detail_setup where  table_name='SA_SALES_INVOICE' and deleted_flag='N' and  COMPANY_CODE IN ({companyCode}) )
                               GROUP BY BI.BRANCH_CODE,BI.BRANCH_EDESC";


            // Query = string.Format(Query, ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter));

            var categorySales = _objectEntity.SqlQuery<ChartSalesModel>(Query);

            return categorySales.ToList();
        }

        public IList<CustomerWisePriceListModel> GetCustomerWisePriceList(ReportFiltersModel model, User userInfo)
        {
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            var Query = $@"SELECT IR.CS_CODE as Customer_Code, CS.CUSTOMER_EDESC AS CustomerName,IR.ITEM_CODE,
                               IMS.ITEM_EDESC,
                        Round((NVL(IR.STANDARD_RATE, 0)) / {ReportFilterHelper.FigureFilterValue(model.AmountFigureFilter)},{ReportFilterHelper.RoundUpFilterValue(model.AmountRoundUpFilter)}) Sales_Rate
                              -- IR.STANDARD_RATE as Sales_Rate
                          FROM IP_ITEM_RATE_SCHEDULE_SETUP IR, IP_ITEM_MASTER_SETUP IMS, SA_CUSTOMER_SETUP CS
                        WHERE     IR.ITEM_CODE = IMS.ITEM_CODE AND CS.CUSTOMER_CODE = IR.CS_CODE
                               AND EFFECTIVE_DATE = (SELECT MAX (EFFECTIVE_DATE)
                                                       FROM IP_ITEM_RATE_SCHEDULE_SETUP
                                                      WHERE ITEM_CODE = IMS.ITEM_CODE)
                               AND IR.COMPANY_CODE = IMS.COMPANY_CODE
                               AND IMS.DELETED_FLAG = 'N'
                               AND IMS.COMPANY_CODE IN({companyCode})
                    AND IR.EFFECTIVE_DATE>= TO_DATE('{model.FromDate}', 'YYYY-MON-DD')
                    AND IR.EFFECTIVE_DATE <= TO_DATE('{model.ToDate}',' YYYY-MON-DD') AND IR.COMPANY_CODE IN ({companyCode}) ";

            //for customer Filter
            if (model.CustomerFilter.Count() > 0)
            {
                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {

                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG = 'G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND CS.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CS.CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join(",", customers));
                //Query += " and (";
                ////IF CUSTOMER_SKU_FLAG = G
                //foreach (var item in model.CustomerFilter)
                //{
                //    Query += "cs.master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                //}
                ////IF CUSTOMER_SKU_FLAG = I                
                //Query += "(cs.CUSTOMER_CODE IN ('" + string.Join(",", model.CustomerFilter) + "') AND cs.GROUP_SKU_FLAG = 'I' AND cs.COMPANY_CODE IN(" + companyCode + ") )) ";

                //Query = Query.Substring(0, Query.Length - 1);
            }
            int min = 0, max = 0;
            ReportFilterHelper.RangeFilterValue(model.AmountRangeFilter, out min, out max);

            if (!(min == 0 && max == 0))
            {
                Query += string.Format(@" AND NVL(IR.STANDARD_RATE, 0) >= {0} AND NVL(IR.STANDARD_RATE,0) <= {1}", min, max);
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND CS.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            Query = string.Format(Query, userInfo.company_code, userInfo.branch_code, model.FromDate, model.ToDate);
            var stockData = this._objectEntity.SqlQuery<CustomerWisePriceListModel>(Query).ToList();

            return stockData;
        }


        public IList<ProductWisePriceListModel> GetProductWisePriceList(ReportFiltersModel model, User userInfo)
        {
            //var companyCode = string.Join(",", model.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            //first check in IP_ITEM_RATE_SCHEDULE_SETUP  table
            var Query = @"SELECT IR.ITEM_CODE,IMS.ITEM_EDESC,MAX(IR.STANDARD_RATE) as Sales_Rate
                          FROM IP_ITEM_RATE_SCHEDULE_SETUP IR, IP_ITEM_MASTER_SETUP IMS
                        WHERE     IR.ITEM_CODE = IMS.ITEM_CODE
                               AND EFFECTIVE_DATE = (SELECT MAX (EFFECTIVE_DATE)
                                                       FROM IP_ITEM_RATE_SCHEDULE_SETUP
                                                      WHERE ITEM_CODE = IMS.ITEM_CODE )
                               AND IR.COMPANY_CODE = IMS.COMPANY_CODE
                               AND IMS.DELETED_FLAG = 'N'   
                               AND IMS.COMPANY_CODE IN({0}) 
                               ";

            //AND TRIM(IMS.MASTER_ITEM_CODE) LIKE: V_MIC || '%'      -- WAS COMMENTED IN QUERY
            //AND IMS.ITEM_CODE IN(SELECT ITEM_CODE FROM SA_SALES_INVOICE)


            ////for Product Filter
            if (model.ProductFilter.Count() > 0)
            {
                var products = model.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND IR.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG = 'I')) ", productConditionQuery, string.Join("','", products));
                //Query += " and (";
                ////IF PRODUCT_SKU_FLAG = G
                //foreach (var item in model.ProductFilter)
                //{
                //    Query += "IMS.MASTER_ITEM_CODE like  (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN({0}) ) OR ";
                //}
                ////IF PRODUCT_SKU_FLAG = I                
                //Query += "(IMS.ITEM_CODE IN (" + string.Join(",", model.ProductFilter) + ") AND IMS.GROUP_SKU_FLAG = 'I')) ";

                //Query = Query.Substring(0, Query.Length - 1);
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND IMS.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            Query += @" 
                GROUP BY IR.ITEM_CODE, IMS.ITEM_EDESC";
            Query = string.Format(Query, companyCode, userInfo.branch_code, model.FromDate, model.ToDate);

            var Data = this._objectEntity.SqlQuery<ProductWisePriceListModel>(Query).ToList();


            if (Data.Count() > 0)
            {
                return Data;
            }
            else
            {
                Query = @"SELECT  IR.ITEM_CODE,INITCAP(IMS.ITEM_EDESC) as ITEM_EDESC, IR.SALES_RATE FROM IP_ITEM_RATE_APPLICAT_SETUP  IR, IP_ITEM_MASTER_SETUP IMS
                                WHERE IR.ITEM_CODE = IMS.ITEM_CODE
                                AND APP_DATE = (SELECT MAX(APP_DATE) FROM  IP_ITEM_RATE_APPLICAT_SETUP WHERE ITEM_CODE = IMS.ITEM_CODE)
                                AND IR.COMPANY_CODE = IMS.COMPANY_CODE
                                AND IR.BRANCH_CODE = IMS.BRANCH_CODE
                                 AND IMS.COMPANY_CODE IN({0}) ";


                ////for Product Filter
                if (model.ProductFilter.Count() > 0)
                {

                    var products = model.ProductFilter;
                    var productConditionQuery = string.Empty;
                    for (int i = 0; i < products.Count; i++)
                    {

                        if (i == 0)
                            productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                        else
                        {
                            productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                        }
                    }

                    Query = Query + string.Format(@" AND IR.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG = 'I')) ", productConditionQuery, string.Join("','", products));
                    //Query += " and (";
                    ////IF PRODUCT_SKU_FLAG = G
                    //foreach (var item in model.ProductFilter)
                    //{
                    //    Query += "IMS.MASTER_ITEM_CODE like  (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND COMPANY_CODE IN({0}) ) OR ";
                    //}
                    ////IF PRODUCT_SKU_FLAG = I                
                    //Query += "(IMS.ITEM_CODE IN (" + string.Join(",", model.ProductFilter) + ") AND IMS.GROUP_SKU_FLAG = 'I')) ";

                    //Query = Query.Substring(0, Query.Length - 1);
                }
                if (model.BranchFilter.Count > 0)
                {
                    Query += string.Format(@" AND IMS.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                }

                Query = string.Format(Query, companyCode, userInfo.branch_code, model.FromDate, model.ToDate);
                var newData = this._objectEntity.SqlQuery<ProductWisePriceListModel>(Query).ToList();

                if (newData.Count() > 0)
                    return newData;
                else
                    return Data;
            }

        }



        public IList<CustomerWiseProfileAnalysisModel> GetCustomerWiseProfitAnalysis(ReportFiltersModel model, User userInfo)
        {
            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);




            var Query = @"SELECT CUSTOMER_CODE
            	,CustomerName
            	,ITEM_CODE
            	,Unit
            	,Quantity
            	,Rate
            	,SalesAmount
            	,ITEM_EDESC
            	,UnitCost
            	,LANDED_COST
            	,ROUND((NVL(RATE, 0) - NVL(UnitCost, 0)) * NVL(QUANTITY, 0), 2) GrossProfit
            	,ROUND(((NVL(RATE, 0) - NVL(UnitCost, 0)) / NVL(UnitCost, 1)) * 100, 2)GrossPercent
            FROM (SELECT a.CUSTOMER_CODE, c.CUSTOMER_EDESC as CustomerName, a.ITEM_CODE, a.MU_CODE as Unit,A.COMPANY_CODE
            			,A.BRANCH_CODE
                                             ,SUM(NVL(a.QUANTITY, 0)) as Quantity  ,                                               
                                             Round(SUM(NVL(FN_CONVERT_CURRENCY(NVL(a.TOTAL_PRICE, 0) * NVL(a.EXCHANGE_RATE, 1), 'NRS', a.SALES_DATE), 0)) / DECODE(SUM(NVL(a.QUANTITY, 1)), 0, 1, SUM(NVL(a.QUANTITY, 1)))/ {4},{5}) as Rate ,                                                 
                                             Round(SUM(NVL(FN_CONVERT_CURRENCY(NVL(a.TOTAL_PRICE, 0) * NVL(a.EXCHANGE_RATE, 1), 'NRS', a.SALES_DATE), 0)) / {4},{5}) SalesAmount ,
                                             b.ITEM_EDESC,
                                             FN_UNIT_COST(A.ITEM_CODE, NULL, A.COMPANY_CODE, A.BRANCH_CODE, TO_DATE('{2}', 'YYYY-MM-DD'), TO_DATE('{3}',' YYYY-MM-DD')) UnitCost,
                                             Round(SUM(NVL(a.QUANTITY, 0) * NVL(d.LANDED_COST, 0)) /{4},{5}) LANDED_COST   ,       
                                             Round((SUM(NVL(FN_CONVERT_CURRENCY(NVL(a.TOTAL_PRICE, 0) * NVL(a.EXCHANGE_RATE, 1), 'NRS', a.SALES_DATE), 0)) - SUM(NVL(a.QUANTITY, 0) * NVL(d.LANDED_COST, 0)))/{4},{5}) GrossProfit,
                                             CASE (NVL(d.LANDED_COST, 0)) WHEN  0 THEN 0
                                                 ELSE 
                                                    ROUND(((SUM(NVL(FN_CONVERT_CURRENCY(NVL(a.TOTAL_PRICE, 0) * NVL(a.EXCHANGE_RATE, 1), 'NRS', a.SALES_DATE), 0)) - SUM(NVL(a.QUANTITY, 0) * NVL(d.LANDED_COST, 0))) * 100) / SUM(NVL(a.QUANTITY, 0) * NVL(d.LANDED_COST, 1)), {5})
                                            END GrossPercent
                                     FROM SA_SALES_INVOICE a, IP_ITEM_MASTER_SETUP b, SA_CUSTOMER_SETUP c, MP_ITEM_STD_RATE d
                                     WHERE  a.ITEM_CODE = b.ITEM_CODE AND a.CUSTOMER_CODE = c.CUSTOMER_CODE AND a.COMPANY_CODE = b.COMPANY_CODE
                                            AND a.COMPANY_CODE = c.COMPANY_CODE AND a.ITEM_CODE = d.ITEM_CODE(+) AND a.COMPANY_CODE = d.COMPANY_CODE(+)
                                            AND a.DELETED_FLAG = 'N'
                                            AND a.COMPANY_CODE IN({0})
                                            --AND a.BRANCH_CODE = '{1}'                          
                                            AND a.SALES_DATE >= TO_DATE('{2}', 'YYYY-MM-DD')
                                            AND a.SALES_DATE <= TO_DATE('{3}',' YYYY-MM-DD')";

            //for customer Filter
            if (model.CustomerFilter.Count() > 0)
            {
                Query += " and (";
                foreach (var item in model.CustomerFilter)
                {
                    Query += "c.master_customer_code like  (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '" + item + "' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
            }

            //for item Filter
            if (model.ProductFilter.Count() > 0)
            {
                Query += " and (";
                foreach (var item in model.ProductFilter)
                {
                    Query += "b.MASTER_ITEM_CODE LIKE (Select DISTINCT DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND COMPANY_CODE IN(" + companyCode + ") ) OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
            }


            //for category Filter
            if (model.CategoryFilter.Count() > 0)
            {
                Query += " and (";
                foreach (var item in model.CategoryFilter)
                {
                    Query += "b.CATEGORY_CODE = '" + item + "' OR ";
                }
                Query = Query.Substring(0, Query.Length - 3) + ") ";
            }

            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND a.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }

            Query += @"
                            GROUP BY a.ITEM_CODE, a.MU_CODE, b.ITEM_EDESC,A.COMPANY_CODE
			,A.BRANCH_CODE, c.CUSTOMER_EDESC, a.CUSTOMER_CODE,d.LANDED_COST
                            ORDER BY c.CUSTOMER_EDESC, b.ITEM_EDESC)";

            Query = string.Format(Query, companyCode, userInfo.branch_code, model.FromDate, model.ToDate, ReportFilterHelper.FigureFilterValue(model.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(model.AmountRoundUpFilter));

            var Data = this._objectEntity.SqlQuery<CustomerWiseProfileAnalysisModel>(Query).ToList();
            return Data;

        }

        public List<DynamicMenu> GetDynamicMenu(int userId, int level, string modular_code)
        {
            var dynamicMenu = new List<DynamicMenu>();
            var query = $@"select menu_no,FULL_PATH,VIRTUAL_PATH,MENU_EDESC,GROUP_SKU_FLAG,ICON_PATH,MODULE_CODE
from WEB_MENU_MANAGEMENT where menu_no in (SELECT mc.MENU_NO from WEB_MENU_CONTROL mc 
INNER JOIN WEB_MENU_MANAGEMENT mm on mm.MENU_NO = mc.MENU_NO and mm.group_sku_flag='G'
 WHERE mc.USER_NO ='{userId}' and mm.module_code='{modular_code}' ) and module_code='{modular_code}' order by ORDERBY asc";
            //  string query = "SELECT mc.MENU_NO, mm.VIRTUAL_PATH, mm.MENU_EDESC,mm.GROUP_SKU_FLAG,  mm.ICON_PATH,mm.MODULE_CODE from WEB_MENU_CONTROL mc INNER JOIN WEB_MENU_MANAGEMENT mm on mm.MENU_NO = mc.MENU_NO WHERE mc.USER_NO = " + userId + " and mm.module_code='" + modular_code + "' order by mm.ORDERBY asc";
            try
            {
                dynamicMenu = _objectEntity.SqlQuery<DynamicMenu>(query).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
            return dynamicMenu;
        }

        public List<DynamicMenu> GetChlidMenu(string menuNo, int userid, string module_code)
        {
            var dynamicMenu = new List<DynamicMenu>();
            var query = $@"SELECT  mc.MENU_NO,mm.FULL_PATH, mm.VIRTUAL_PATH, mm.MENU_EDESC,mm.GROUP_SKU_FLAG, mm.DASHBOARD_FLAG,
 mm.ICON_PATH,mm.MODULE_CODE,mm.MODULE_ABBR,mm.COLOR,mm.DESCRIPTION from WEB_MENU_CONTROL mc 
INNER JOIN WEB_MENU_MANAGEMENT mm on mm.MENU_NO = mc.MENU_NO
            WHERE mc.USER_NO ='{userid}' and mm.module_code='{module_code}' and PRE_MENU_NO='{menuNo}' ORDER BY mm.ORDERBY ASC";
            //  string query = "SELECT MENU_NO, VIRTUAL_PATH, MENU_EDESC, GROUP_SKU_FLAG, ICON_PATH,MODULE_CODE,MODULE_ABBR,COLOR,DESCRIPTION  FROM WEB_MENU_MANAGEMENT WHERE PRE_MENU_NO=" + menuNo + " ORDER BY ORDERBY ASC";
            try
            {
                dynamicMenu = _objectEntity.SqlQuery<DynamicMenu>(query).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
            return dynamicMenu;
        }
        public IList<SalesRegisterDetailModel> GetSalesRegisterModelPrivot(ReportFiltersModel model, User userInfo)
        {

            var companyCode = string.Empty;
            foreach (var company in model.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            var Query = string.Format(@"SELECT F.* ,ROUND(TO_NUMBER(F.DISCOUNTED_AMOUNT),2)-F.SPECIAL_DISCOUNT_SCHEME  +  F.EXCISE_DUTY+ F.VAT_AMOUNT TOTAL_BILL_VALUE  FROM (
                                SELECT A.* ,((round(TO_NUMBER(DISCOUNTED_AMOUNT),2)-SPECIAL_DISCOUNT_SCHEME  + EXCISE_DUTY )* NVL((SELECT DISTINCT DECODE(CHARGE_AMOUNT,0,0,13) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='VT'),0)/100 ) VAT_AMOUNT FROM(
                                SELECT F.BRANCH_EDESC, E.DIVISION_EDESC DIVISION_NAME,
                                FN_CHARTAD_MONTH(SUBSTR(A.SALES_DATE,4,3)) EMONTH ,      
                                FN_CHARTBS_MONTH(SUBSTR(BS_DATE(A.SALES_DATE),6,2)) BSMONTH,  BS_DATE(A.SALES_DATE) MITI,    
                                A.SALES_DATE INV_DATE, A.SALES_NO INVOICE,  A.MANUAL_NO,ASM.AREA_EDESC, C.CUSTOMER_EDESC,  
                                B.ITEM_EDESC MODULE, A.QUANTITY,   A.UNIT_PRICE, A.TOTAL_PRICE,
                                (NVL((SELECT DISTINCT NVL(SUM(CHARGE_AMOUNT),0) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='DC'),0)/(SELECT NVL(SUM(CALC_TOTAL_PRICE),0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO) * NVL(A.TOTAL_PRICE,1)) DISCOUNT_AMT,
                                to_char(NVL(A.TOTAL_PRICE,0) - (NVL((SELECT DISTINCT NVL(SUM(CHARGE_AMOUNT),0) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='DC'),0)/(SELECT NVL(SUM(CALC_TOTAL_PRICE),0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO) * NVL(A.TOTAL_PRICE,1))) DISCOUNTED_AMOUNT,
                                (NVL((SELECT DISTINCT NVL(SUM(CHARGE_AMOUNT),0) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='SDD'),0)/(SELECT NVL(SUM(CALC_TOTAL_PRICE),0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO) * NVL(A.TOTAL_PRICE,1)) SPECIAL_DISCOUNT_SCHEME,
                                (SELECT CASE WHEN VALUE_PERCENT_FLAG  = 'Q'
                                    THEN (NVL((SELECT DISTINCT MANUAL_CALC_VALUE FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='ED'),0)*(SELECT NVL(QUANTITY,0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO
                                AND SERIAL_NO = A.SERIAL_NO
                                AND FORM_CODE = A.FORM_CODE))
                                    WHEN VALUE_PERCENT_FLAG = 'V'
                                    THEN (NVL((SELECT DISTINCT NVL(SUM(CHARGE_AMOUNT),0) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='ED'),0)/(SELECT NVL(SUM(CALC_TOTAL_PRICE),0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO) * NVL(A.TOTAL_PRICE,1)
                                )    WHEN VALUE_PERCENT_FLAG = 'P'
                                    THEN (NVL((SELECT DISTINCT NVL(SUM(CHARGE_AMOUNT),0) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='ED'),0)/(SELECT NVL(SUM(CALC_TOTAL_PRICE),0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO) * NVL(A.TOTAL_PRICE,1)
                                )
                                  END AS STATUSTEXT FROM CHARGE_TRANSACTION X, CHARGE_SETUP CS
                                  WHERE X.CHARGE_CODE='ED'
                                  AND X.CHARGE_CODE = CS.CHARGE_CODE
                                  AND X.FORM_CODE = CS.FORM_CODE
                                  AND X.COMPANY_CODE = CS.COMPANY_CODE
                                  AND X.FORM_CODE =A.FORM_CODE
                                  AND X.REFERENCE_NO = A.SALES_NO) EXCISE_DUTY,
                                SUBSTR(FN_FETCH_GROUP_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', B.PRE_ITEM_CODE),1,30) ITEM_GROUP_EDESC,
                                SUBSTR(FN_FETCH_PRE_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', B.PRE_ITEM_CODE),1,30) ITEM_SUBGROUP_EDESC,
                                FN_FETCH_DESC(A.COMPANY_CODE,'IP_CATEGORY_CODE',B.CATEGORY_CODE) CATEGORY_EDESC,
                                G.EMPLOYEE_EDESC MR_NAME, A.BRANCH_CODE, A.FORM_CODE, A.COMPANY_CODE, A.SALES_NO,
                                SUBSTR(FN_FETCH_GROUP_DESC(A.COMPANY_CODE,'SA_CUSTOMER_SETUP', C.PRE_CUSTOMER_CODE),1,30) CUSTOMER_GROUP_EDESC,
                                SUBSTR(FN_FETCH_PRE_DESC(A.COMPANY_CODE,'SA_CUSTOMER_SETUP', C.PRE_CUSTOMER_CODE),1,30) CUSTOMER_SUBGROUP_EDESC, 
                                FN_FETCH_DESC(A.COMPANY_CODE,'IP_PARTY_TYPE_CODE',A.PARTY_TYPE_CODE) DEALER_NAME,
                                S.DESTINATION, FN_FETCH_DESC(S.COMPANY_CODE,'IP_VEHICLE_CODE',S.VEHICLE_CODE) VEHICLE_NAME, S.VEHICLE_OWNER_NAME, S.VEHICLE_OWNER_NO,S.DRIVER_NAME, S.DRIVER_LICENSE_NO, 
                                S.DRIVER_MOBILE_NO, FN_FETCH_DESC(S.COMPANY_CODE,'TRANSPORTER_SETUP',S.TRANSPORTER_CODE) TRANSPORTER_NAME, nvl(S.FREGHT_AMOUNT,0) FREGHT_AMOUNT, nvl(S.WB_WEIGHT,0) WB_WEIGHT, S.WB_NO, S.WB_DATE
                                FROM SA_SALES_INVOICE A, IP_ITEM_MASTER_SETUP B, SA_CUSTOMER_SETUP C, 
                                FA_DIVISION_SETUP E, FA_BRANCH_SETUP F, HR_EMPLOYEE_SETUP G , SHIPPING_TRANSACTION S, AREA_SETUP ASM
                                WHERE A.ITEM_CODE = B.ITEM_CODE(+) AND A.CUSTOMER_CODE = C.CUSTOMER_CODE(+) 
                                AND A.COMPANY_CODE = B.COMPANY_CODE(+) 
                                AND A.DELETED_FLAG='N'
                                AND A.COMPANY_CODE = C.COMPANY_CODE(+)
                                AND A.DIVISION_CODE = E.DIVISION_CODE(+)
                                AND A.BRANCH_CODE = F.BRANCH_CODE(+)
                                AND A.EMPLOYEE_CODE = G.EMPLOYEE_CODE(+)
                                AND A.COMPANY_CODE = G.COMPANY_CODE(+)
                                AND A.COMPANY_CODE = S.COMPANY_CODE(+)
                                AND A.FORM_CODE = S.FORM_CODE (+)
                                AND A.SALES_NO = S.VOUCHER_NO (+)
                                AND A.AREA_CODE = ASM.AREA_CODE (+)
                                AND a.SALES_DATE >= TO_DATE('{0}', 'YYYY-MON-DD')
                                AND a.SALES_DATE <= TO_DATE('{1}',' YYYY-MON-DD')", model.FromDate, model.ToDate, companyCode);

            if (model.CustomerFilter.Count > 0)
            {

                var customers = model.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {

                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND C.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join("','", customers));
            }

            if (model.DocumentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND  A.FORM_CODE  IN  ('{0}')", string.Join("','", model.DocumentFilter).ToString());
            }
            if (model.AreaTypeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND  A.AREA_CODE  IN  ({0})", string.Join(",", model.AreaTypeFilter).ToString());
            }
            if (model.PartyTypeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND A.PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", model.PartyTypeFilter).ToString());
            }
            if (model.CategoryFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND B.CATEGORY_CODE IN ('{0}') ", string.Join("','", model.CategoryFilter).ToString());
            }
            if (model.EmployeeFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND A.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
            }
            if (model.AgentFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND A.AGENT_CODE IN  ('{0}')", string.Join("','", model.AgentFilter).ToString());
            }
            if (model.DivisionFilter.Count > 0)
            {
                Query = Query + string.Format(@" AND A.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
            }
            if (model.LocationFilter.Count > 0)
            {
                var locations = model.LocationFilter;
                var locationConditionQuery = string.Empty;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%'", locations[i]);
                    else
                    {
                        locationConditionQuery += string.Format(" OR  LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                Query = Query + string.Format(@" AND A.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                //query = query + string.Format(@" AND (SI.FROM_LOCATION_CODE = LS.LOCATION_CODE OR SI.FROM_LOCATION_CODE IN ('{0}'))", string.Join("','", filters.LocationFilter).ToString());
            }

            if (model.ProductFilter.Count() > 0)
            {
                var products = model.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G' )", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }

                Query = Query + string.Format(@" AND A.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join("','", products));
                //var productFilter = @"select  DISTINCT TRIM(item_code) from IP_ITEM_MASTER_SETUP where (";
                ////IF PRODUCT_SKU_FLAG = G
                //foreach (var company in model.CompanyFilter)
                //{
                //    foreach (var item in model.ProductFilter)
                //    {
                //        productFilter += "MASTER_ITEM_CODE like  (Select DISTINCT MASTER_ITEM_CODE || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '" + item + "' AND GROUP_SKU_FLAG = 'G' AND DELETED_FLAG= 'N' AND COMPANY_CODE ='" + company + "') OR ";
                //    }
                //}
                //productFilter = productFilter.Substring(0, productFilter.Length - 3);
                ////IF PRODUCT_SKU_FLAG = I                
                //productFilter += " OR (ITEM_CODE in (" + string.Join(",", model.ProductFilter) + ") and group_sku_flag = 'I' AND DELETED_FLAG = 'N' AND COMPANY_CODE IN(" + string.Join(",", model.CompanyFilter) + "))) ";
                //productFilter = " AND A.ITEM_CODE IN(" + productFilter + ")";

                //Query += productFilter;
            }
            if (model.BranchFilter.Count > 0)
            {
                Query += string.Format(@" AND a.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
            }
            Query += $@" AND A.COMPANY_CODE IN({companyCode})) A) F";
            var Data = new List<SalesRegisterDetailModel>();
            try
            {
                Data = this._objectEntity.SqlQuery<SalesRegisterDetailModel>(Query).ToList();
            }
            catch (Exception ex)
            {
                return Data;
            }
            return Data;
        }

        public List<DailySalesTreeList> GetSalesRegisterDailyReport(ReportFiltersModel model, User userInfo)
        {
            var cachedata = new List<DailySalesTreeList>();
            var cachekey = $"NCR-report-{userInfo.company_code}{model.FromDate}-{model.ToDate}";
            if (model.CustomerFilter.Count > 0)
            {
                this._cacheManager.Remove(cachekey);
            }
            if (this._cacheManager.IsSet(cachekey))
            {

                cachedata = this._cacheManager.Get<List<DailySalesTreeList>>(cachekey);
                return cachedata;
            }
            else
            {


                var figureAmountFilter = ReportFilterHelper.FigureFilterValue(model.AmountFigureFilter);
                var roundUpAmountFilter = ReportFilterHelper.RoundUpFilterValue(model.AmountRoundUpFilter);
                var companyCode = string.Empty;
                foreach (var company in model.CompanyFilter)
                {
                    companyCode += $@"'{company}',";
                }
                companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
                var Query = string.Format(@"SELECT F.* ,F.DISCOUNTED_AMOUNT - F.SPECIAL_DISCOUNT_SCHEME  +  F.EXCISE_DUTY+ F.VAT_AMOUNT TOTAL_BILL_VALUE  FROM (
                                SELECT A.* ,((DISCOUNTED_AMOUNT - SPECIAL_DISCOUNT_SCHEME + EXCISE_DUTY )* NVL((SELECT DISTINCT DECODE(CHARGE_AMOUNT,0,0,13) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='VT'),0)/100 ) VAT_AMOUNT FROM(
                                SELECT F.BRANCH_EDESC,
                                FN_CHARTAD_MONTH(SUBSTR(A.SALES_DATE,4,3)) EMONTH ,      
                                FN_CHARTBS_MONTH(SUBSTR(BS_DATE(A.SALES_DATE),6,2)) BSMONTH,  BS_DATE(A.SALES_DATE) MITI,    
                                A.SALES_DATE INV_DATE, A.SALES_NO INVOICE,  A.MANUAL_NO,A.PARTY_TYPE_CODE,C.CUSTOMER_CODE, C.CUSTOMER_EDESC,  
                                B.ITEM_EDESC MODULE,A.QUANTITY,A.UNIT_PRICE,Round(NVL(FN_CONVERT_CURRENCY(NVL(A.TOTAL_PRICE,0),'NRS',A.SALES_DATE),0)/{3},{4}) AS TOTAL_PRICE,
                                (NVL((SELECT DISTINCT NVL(SUM(CHARGE_AMOUNT),0) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='DC'),0)/(SELECT NVL(SUM(CALC_TOTAL_PRICE),0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO) * NVL(A.TOTAL_PRICE,1)) DISCOUNT_AMT,
                                NVL(A.TOTAL_PRICE,0) - (NVL((SELECT DISTINCT NVL(SUM(CHARGE_AMOUNT),0) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='DC'),0)/(SELECT NVL(SUM(CALC_TOTAL_PRICE),0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO) * NVL(A.TOTAL_PRICE,1)) DISCOUNTED_AMOUNT,
                                (NVL((SELECT DISTINCT NVL(SUM(CHARGE_AMOUNT),0) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='SDD'),0)/(SELECT NVL(SUM(CALC_TOTAL_PRICE),0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO) * NVL(A.TOTAL_PRICE,1)) SPECIAL_DISCOUNT_SCHEME,
                                (SELECT CASE WHEN VALUE_PERCENT_FLAG  = 'Q'
                                    THEN (NVL((SELECT DISTINCT MANUAL_CALC_VALUE FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='ED'),0)*(SELECT NVL(QUANTITY,0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO
                                AND SERIAL_NO = A.SERIAL_NO
                                AND FORM_CODE = A.FORM_CODE))
                                    WHEN VALUE_PERCENT_FLAG = 'V'
                                    THEN (NVL((SELECT DISTINCT NVL(SUM(CHARGE_AMOUNT),0) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='ED'),0)/(SELECT NVL(SUM(CALC_TOTAL_PRICE),0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO) * NVL(A.TOTAL_PRICE,1)
                                )    WHEN VALUE_PERCENT_FLAG = 'P'
                                    THEN (NVL((SELECT DISTINCT NVL(SUM(CHARGE_AMOUNT),0) FROM CHARGE_TRANSACTION X
                                WHERE X.REFERENCE_NO = A.SALES_NO
                                AND X.FORM_CODE = A.FORM_CODE
                                AND X.BRANCH_CODE = A.BRANCH_CODE
                                AND X.CHARGE_CODE='ED'),0)/(SELECT NVL(SUM(CALC_TOTAL_PRICE),0) 
                                FROM SA_SALES_INVOICE WHERE COMPANY_CODE = A.COMPANY_CODE 
                                AND SALES_NO = A.SALES_NO) * NVL(A.TOTAL_PRICE,1)
                                )
                                  END AS STATUSTEXT FROM CHARGE_TRANSACTION X, CHARGE_SETUP CS
                                  WHERE X.CHARGE_CODE='ED'
                                  AND X.CHARGE_CODE = CS.CHARGE_CODE
                                  AND X.FORM_CODE = CS.FORM_CODE
                                  AND X.COMPANY_CODE = CS.COMPANY_CODE
                                  AND X.FORM_CODE =A.FORM_CODE
                                  AND X.REFERENCE_NO = A.SALES_NO) EXCISE_DUTY,
                              A.BRANCH_CODE, A.FORM_CODE, A.COMPANY_CODE, A.SALES_NO,
                                C.MASTER_CUSTOMER_CODE as MasterCode,C.PRE_CUSTOMER_CODE as PreCode
                                FROM SA_SALES_INVOICE A, IP_ITEM_MASTER_SETUP B, SA_CUSTOMER_SETUP C,  FA_BRANCH_SETUP F
                                WHERE A.ITEM_CODE = B.ITEM_CODE(+) AND A.CUSTOMER_CODE = C.CUSTOMER_CODE(+) 
                                AND A.COMPANY_CODE = B.COMPANY_CODE(+) 
                                AND A.DELETED_FLAG='N'
                                AND A.COMPANY_CODE = C.COMPANY_CODE(+)
                                AND A.BRANCH_CODE = F.BRANCH_CODE(+)
                                AND a.SALES_DATE >= TO_DATE('{0}', 'YYYY-MON-DD')
                                AND a.SALES_DATE <= TO_DATE('{1}',' YYYY-MON-DD')", model.FromDate, model.ToDate, companyCode, figureAmountFilter, roundUpAmountFilter);

                if (model.CustomerFilter.Count > 0)
                {

                    var customers = model.CustomerFilter;
                    var customerConditionQuery = string.Empty;
                    for (int i = 0; i < customers.Count; i++)
                    {

                        if (i == 0)
                            customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                        else
                        {
                            customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                        }
                    }

                    Query = Query + string.Format(@" AND C.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join("','", customers));
                }

                if (model.DocumentFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND  A.FORM_CODE  IN  ('{0}')", string.Join("','", model.DocumentFilter).ToString());
                }
                if (model.AreaTypeFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND  A.AREA_CODE  IN  ({0})", string.Join(",", model).ToString());
                }
                if (model.PartyTypeFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND A.PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", model.PartyTypeFilter).ToString());
                }
                if (model.CategoryFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND B.CATEGORY_CODE IN ('{0}') ", string.Join("','", model.CategoryFilter).ToString());
                }
                if (model.EmployeeFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND A.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", model.EmployeeFilter).ToString());
                }
                if (model.AgentFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND A.AGENT_CODE IN  ('{0}')", string.Join("','", model.AgentFilter).ToString());
                }
                if (model.DivisionFilter.Count > 0)
                {
                    Query = Query + string.Format(@" AND A.DIVISION_CODE IN ('{0}')", string.Join("','", model.DivisionFilter).ToString());
                }
                if (model.LocationFilter.Count > 0)
                {
                    var locations = model.LocationFilter;
                    var locationConditionQuery = string.Empty;
                    for (int i = 0; i < locations.Count; i++)
                    {

                        if (i == 0)
                            locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%'", locations[i]);
                        else
                        {
                            locationConditionQuery += string.Format(" OR  LOCATION_CODE like '{0}%' ", locations[i]);
                        }
                    }
                    Query = Query + string.Format(@" AND A.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                    //query = query + string.Format(@" AND (SI.FROM_LOCATION_CODE = LS.LOCATION_CODE OR SI.FROM_LOCATION_CODE IN ('{0}'))", string.Join("','", filters.LocationFilter).ToString());
                }

                if (model.ProductFilter.Count() > 0)
                {
                    var products = model.ProductFilter;
                    var productConditionQuery = string.Empty;
                    for (int i = 0; i < products.Count; i++)
                    {

                        if (i == 0)
                            productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G' )", products[i], companyCode);
                        else
                        {
                            productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                        }
                    }

                    Query = Query + string.Format(@" AND A.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I'))", productConditionQuery, string.Join("','", products));

                }
                if (model.BranchFilter.Count > 0)
                {
                    Query += string.Format(@" AND a.BRANCH_CODE IN ('{0}')", string.Join("','", model.BranchFilter).ToString());
                }
                int min = 0, max = 0;
                ReportFilterHelper.RangeFilterValue(model.AmountRangeFilter, out min, out max);

                if (!(min == 0 && max == 0))
                {
                    Query += string.Format(@" AND A.TOTAL_PRICE >= {0} AND A.TOTAL_PRICE <= {1}", min, max);
                }

                Query += $@" AND A.COMPANY_CODE IN({companyCode})  ORDER BY A.AREA_CODE) A) F";
                var Data = this._objectEntity.SqlQuery<SalesRegisterDetailModel>(Query).ToList();

                string querydynamic = string.Format(@"select  * from (SELECT distinct   CUSTOMER_EDESC AS Description,MASTER_CUSTOMER_CODE as MasterCodeWithoutReplace,PRE_CUSTOMER_CODE as PreCodeWithoutReplace,
                                   TO_CHAR(TO_NUMBER(REPLACE(MASTER_CUSTOMER_CODE,'.',''))) as MasterCode, TO_CHAR(TO_NUMBER(CUSTOMER_CODE)) as Code, TO_CHAR(TO_NUMBER(REPLACE(PRE_CUSTOMER_CODE,'.',''))) as PreCode ,group_sku_flag
                                    FROM SA_CUSTOMER_SETUP WHERE  DELETED_FLAG = 'N' )  start with PreCode='0'  CONNECT BY PRIOR MasterCode = PreCode ");


                //querydynamic += string.Format(" AND COMPANY_CODE IN('{0}') ", companyCode);


                var dynamicdata = _objectEntity.SqlQuery<AgeingGroupDataNCR>(querydynamic);

                var dynamicColumnQuery = $@"select v.sub_edesc,s.customer_code,v.party_type_code,sum(v.dr_amount) dramount,
                            FN_FETCH_DESC(v.company_code,'FA_CHART_OF_ACCOUNTS_SETUP',v.acc_code) accountname from V$VIRTUAL_SUB_dealer_LEDGER  v ,
                            sa_customer_setup s 
                            where v.company_code=s.company_code
                            and trim(v.sub_code)=trim(s.link_sub_code)
                            and v.deleted_flag='N'
                            and v.acc_code in('201399','201396','201398','201397','201395','201394')
                            AND v.voucher_date >= TO_DATE('{model.FromDate}', 'YYYY-MON-DD')
                                AND v.voucher_date <= TO_DATE('{model.ToDate}',' YYYY-MON-DD')
                            group by v.sub_edesc,s.customer_code,v.party_type_code,FN_FETCH_DESC(v.company_code,'FA_CHART_OF_ACCOUNTS_SETUP',v.acc_code)";

                var columnsname = this._objectEntity.SqlQuery<DynamicColumns>(dynamicColumnQuery).ToList();

                var finaldata = Data.GroupBy(customer => customer.CUSTOMER_CODE).Select(group => group.First()).ToList();
                foreach (var item in finaldata)
                {
                    Decimal resultt;
                    decimal result;
                    item.QUANTITY = Data.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Sum(x => x.QUANTITY);
                    item.DISCOUNT_AMT = Data.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Sum(x => x.DISCOUNT_AMT);
                    item.DISCOUNTED_AMOUNT = Data.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Sum(x => x.DISCOUNTED_AMOUNT ?? 0);
                    //item.DISCOUNTED_AMOUNT = Data.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Sum(x => decimal.TryParse(x.DISCOUNTED_AMOUNT,out resultt)?resultt:0).ToString();
                    item.Special_Discount_Scheme = Data.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Sum(x => x.Special_Discount_Scheme);
                    item.TOTAL_PRICE = Data.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Sum(x => x.TOTAL_PRICE);
                    item.EXCISE_DUTY = Data.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Sum(x => x.EXCISE_DUTY);
                    item.VAT_AMOUNT = Data.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Sum(x => x.VAT_AMOUNT);
                    item.TOTAL_BILL_VALUE = Data.Where(x => x.CUSTOMER_CODE == item.CUSTOMER_CODE).Sum(x => x.TOTAL_BILL_VALUE);
                    item.GROSS_REALISATION_AMOUNT = item.TOTAL_PRICE - (Decimal.TryParse(item.DISCOUNT_AMT.ToString(), out result) ? result : 0);
                    item.GROSS_REALISATION_PER_QUANTITY = item.GROSS_REALISATION_AMOUNT / item.QUANTITY;
                    var ledgerData = columnsname.Where(x => x.customer_code == item.CUSTOMER_CODE).ToList();
                    if (ledgerData.Count > 0)
                    {
                        if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Target".ToUpper().Trim())))
                        {
                            item.TargetBonus = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Target".ToUpper().Trim())).Sum(x => x.dramount);
                        }
                        if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Monopoly".ToUpper().Trim())))
                        {
                            item.MonopolyBonus = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Monopoly".ToUpper().Trim())).Sum(x => x.dramount);
                        }
                        if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-CMTP Scheme".ToUpper().Trim())))
                        {
                            item.CMTPScheme = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-CMTP Scheme".ToUpper().Trim())).Sum(x => x.dramount);
                        }
                        if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-VPB Scheme".ToUpper().Trim())))
                        {
                            item.VPBScheme = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-VPB Scheme".ToUpper().Trim())).Sum(x => x.dramount);
                        }
                        if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-BG".ToUpper().Trim())))
                        {
                            item.BgBonus = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-BG".ToUpper().Trim())).Sum(x => x.dramount);
                        }
                        if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Collection".ToUpper().Trim())))
                        {
                            item.CollectionBonus = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Collection".ToUpper().Trim())).Sum(x => x.dramount);
                        }
                    }
                }

                foreach (var col in columnsname)
                {
                    if (finaldata.Where(x => x.CUSTOMER_CODE == col.customer_code).FirstOrDefault() == null)
                    {
                        var customerdata = new SalesRegisterDetailModel();
                        var getcustomerdata = dynamicdata.Where(x => x.Code == col.customer_code).FirstOrDefault();
                        if (getcustomerdata == null)
                            continue;

                        customerdata.CUSTOMER_CODE = getcustomerdata.Code;
                        customerdata.CUSTOMER_EDESC = getcustomerdata.Description;
                        customerdata.MasterCode = getcustomerdata.MasterCodeWithoutReplace;
                        customerdata.PreCode = getcustomerdata.PreCodeWithoutReplace;
                        customerdata.QUANTITY = 0;
                        customerdata.DISCOUNT_AMT = 0;
                        customerdata.DISCOUNTED_AMOUNT = 0.0;
                        //customerdata.DISCOUNTED_AMOUNT = "0".ToString();
                        customerdata.Special_Discount_Scheme = 0;
                        customerdata.TOTAL_PRICE = 0;
                        customerdata.EXCISE_DUTY = 0;
                        customerdata.VAT_AMOUNT = 0;
                        customerdata.TOTAL_BILL_VALUE = 0;
                        Decimal result;
                        customerdata.GROSS_REALISATION_AMOUNT = customerdata.TOTAL_PRICE - (Decimal.TryParse(customerdata.DISCOUNT_AMT.ToString(), out result) ? result : 0);
                        customerdata.GROSS_REALISATION_PER_QUANTITY = customerdata.QUANTITY > 0 ? customerdata.GROSS_REALISATION_AMOUNT / customerdata.QUANTITY : customerdata.GROSS_REALISATION_AMOUNT / 1;
                        var ledgerData = columnsname.Where(x => x.customer_code == col.customer_code).ToList();
                        if (ledgerData.Count > 0)
                        {
                            if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Target".ToUpper().Trim())))
                            {
                                customerdata.TargetBonus = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Target".ToUpper().Trim())).Sum(x => x.dramount);
                            }
                            if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Monopoly".ToUpper().Trim())))
                            {
                                customerdata.MonopolyBonus = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Monopoly".ToUpper().Trim())).Sum(x => x.dramount);
                            }
                            if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-CMTP Scheme".ToUpper().Trim())))
                            {
                                customerdata.CMTPScheme = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-CMTP Scheme".ToUpper().Trim())).Sum(x => x.dramount);
                            }
                            if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-VPB Scheme".ToUpper().Trim())))
                            {
                                customerdata.VPBScheme = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-VPB Scheme".ToUpper().Trim())).Sum(x => x.dramount);
                            }
                            if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-BG".ToUpper().Trim())))
                            {
                                customerdata.BgBonus = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-BG".ToUpper().Trim())).Sum(x => x.dramount);
                            }
                            if (ledgerData.Any(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Collection".ToUpper().Trim())))
                            {
                                customerdata.CollectionBonus = ledgerData.Where(x => x.accountname.ToUpper().Trim().Equals("Sales Commission-Collection".ToUpper().Trim())).Sum(x => x.dramount);
                            }
                        }
                        finaldata.Add(customerdata);
                    }
                }

                List<DailySalesTreeList> newTotalList = new List<DailySalesTreeList>();
                try
                {
                    foreach (var groupItem in dynamicdata)
                    {
                        var dataSales = new DailySalesTreeList();

                        if (groupItem.PreCode == "0")
                        {
                            dataSales.parentId = null;
                        }
                        else
                        {
                            dataSales.parentId = groupItem.PreCode.ToString();
                        }

                        dataSales.Description = groupItem.Description;
                        if (groupItem.group_sku_flag == "G")
                        {
                            dataSales.Id = groupItem.MasterCode.ToString();
                            dataSales.QUANTITY = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.QUANTITY);
                            if (dataSales.QUANTITY == 0)
                                continue;
                            decimal result;
                            decimal test;
                            decimal DISCOUNTED_AMOUNT;
                            decimal Special_Discount_Scheme;
                            decimal exciseduty;
                            var masterCode = groupItem.MasterCodeWithoutReplace?.ToString() ?? "";
                            dataSales.DISCOUNT_AMT = finaldata.Where(x => x.PreCode.StartsWith(masterCode)).Sum(x => x.DISCOUNT_AMT ?? 0);

                            // dataSales.DISCOUNT_AMT = decimal.TryParse(finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.DISCOUNT_AMT).ToString(),out result)?result:0;
                            dataSales.DISCOUNTED_AMOUNT = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.DISCOUNTED_AMOUNT);
                            //dataSales.DISCOUNTED_AMOUNT =decimal.TryParse(finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => decimal.TryParse(x.DISCOUNTED_AMOUNT,out test)?test:0).ToString(),out DISCOUNTED_AMOUNT) ? DISCOUNTED_AMOUNT : 0;
                            dataSales.Special_Discount_Scheme = decimal.TryParse(finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.Special_Discount_Scheme).ToString(), out Special_Discount_Scheme) ? Special_Discount_Scheme : 0;
                            dataSales.TOTAL_PRICE = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.TOTAL_PRICE);
                            dataSales.EXCISE_DUTY = decimal.TryParse(finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.EXCISE_DUTY).ToString(), out exciseduty) ? exciseduty : 0;
                            dataSales.VAT_AMOUNT = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.VAT_AMOUNT);
                            dataSales.TOTAL_BILL_VALUE = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.TOTAL_BILL_VALUE);
                            dataSales.GROSS_REALISATION_AMOUNT = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.GROSS_REALISATION_AMOUNT);
                            dataSales.GROSS_REALISATION_PER_QUANTITY = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.GROSS_REALISATION_PER_QUANTITY);
                            dataSales.TargetBonus = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.TargetBonus);
                            dataSales.CollectionBonus = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.CollectionBonus);
                            dataSales.MonopolyBonus = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.MonopolyBonus);
                            dataSales.BgBonus = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.BgBonus);
                            dataSales.CMTPScheme = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.CMTPScheme);
                            dataSales.VPBScheme = finaldata.Where(x => x.PreCode.StartsWith(groupItem.MasterCodeWithoutReplace.ToString())).Sum(x => x.VPBScheme);
                            dataSales.TotalBonus = dataSales.TargetBonus + dataSales.CollectionBonus + dataSales.MonopolyBonus + dataSales.BgBonus + dataSales.CMTPScheme + dataSales.VPBScheme;
                            dataSales.TotalBonusPerQty = dataSales.QUANTITY > 0 ? dataSales.TotalBonus / dataSales.QUANTITY : dataSales.TotalBonus / 1;
                            dataSales.NCRAmount = dataSales.GROSS_REALISATION_AMOUNT - dataSales.TotalBonus;
                            dataSales.NCRPerQty = dataSales.QUANTITY > 0 ? dataSales.NCRAmount / dataSales.QUANTITY : dataSales.NCRAmount;
                        }
                        else
                        {
                            dataSales.Id = groupItem.Code.ToString();
                            dataSales.QUANTITY = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.QUANTITY);
                            if (dataSales.QUANTITY == 0)
                                continue;
                            decimal customerDiscount;
                            decimal disAmount;
                            decimal cusDiscounted;
                            decimal specialDiscount;
                            decimal custExcise;
                            var sumDiscountAmt = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.DISCOUNT_AMT ?? 0.0); // Ensure we sum using a default value of type double
                            dataSales.DISCOUNT_AMT = (double?)(decimal)sumDiscountAmt; // Explicitly cast to decimal
                            //dataSales.DISCOUNT_AMT = decimal.TryParse(finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.DISCOUNT_AMT).ToString(),out customerDiscount)?customerDiscount:0;
                            dataSales.DISCOUNTED_AMOUNT = finaldata.Where(x => x.CUSTOMER_CODE == groupItem.Code.ToString()).Sum(x => x.DISCOUNTED_AMOUNT);
                            // dataSales.DISCOUNTED_AMOUNT = decimal.TryParse(finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => decimal.TryParse(x.DISCOUNTED_AMOUNT,out disAmount)?disAmount:0).ToString(),out cusDiscounted)?customerDiscount:0;
                            dataSales.Special_Discount_Scheme = decimal.TryParse(finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.Special_Discount_Scheme).ToString(), out specialDiscount) ? specialDiscount : 0;
                            dataSales.TOTAL_PRICE = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.TOTAL_PRICE);
                            dataSales.EXCISE_DUTY = decimal.TryParse(finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.EXCISE_DUTY).ToString(), out custExcise) ? custExcise : 0;
                            dataSales.VAT_AMOUNT = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.VAT_AMOUNT);
                            dataSales.TOTAL_BILL_VALUE = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.TOTAL_BILL_VALUE);
                            dataSales.GROSS_REALISATION_AMOUNT = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.GROSS_REALISATION_AMOUNT);
                            dataSales.GROSS_REALISATION_PER_QUANTITY = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.GROSS_REALISATION_PER_QUANTITY);
                            dataSales.TargetBonus = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.TargetBonus);
                            dataSales.CollectionBonus = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.CollectionBonus);
                            dataSales.MonopolyBonus = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.MonopolyBonus);
                            dataSales.BgBonus = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.BgBonus);
                            dataSales.CMTPScheme = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.CMTPScheme);
                            dataSales.VPBScheme = finaldata.Where(x => x.CUSTOMER_CODE.Equals(groupItem.Code.ToString())).Sum(x => x.VPBScheme);
                            dataSales.TotalBonus = dataSales.TargetBonus + dataSales.CollectionBonus + dataSales.MonopolyBonus + dataSales.BgBonus + dataSales.CMTPScheme + dataSales.VPBScheme;
                            dataSales.TotalBonusPerQty = dataSales.QUANTITY > 0 ? dataSales.TotalBonus / dataSales.QUANTITY : dataSales.TotalBonus;
                            dataSales.NCRAmount = dataSales.GROSS_REALISATION_AMOUNT - dataSales.TotalBonus;
                            dataSales.NCRPerQty = dataSales.QUANTITY > 0 ? dataSales.NCRAmount / dataSales.QUANTITY : dataSales.NCRAmount;
                        }
                        newTotalList.Add(dataSales);


                    }
                }
                catch (Exception ex)
                {

                }
                if (newTotalList.Count > 0)
                {

                    var TotalColumndata = new DailySalesTreeList();
                    TotalColumndata.Description = "Total";
                    TotalColumndata.parentId = null;
                    TotalColumndata.Id = "9856985";
                    TotalColumndata.QUANTITY = newTotalList.Where(x => x.parentId == null).Sum(x => x.QUANTITY);

                    TotalColumndata.DISCOUNT_AMT = newTotalList.Where(x => x.parentId == null).Sum(x => x.DISCOUNT_AMT);
                    TotalColumndata.DISCOUNTED_AMOUNT = newTotalList.Where(x => x.parentId == null).Sum(x => x.DISCOUNTED_AMOUNT);
                    TotalColumndata.Special_Discount_Scheme = newTotalList.Where(x => x.parentId == null).Sum(x => x.Special_Discount_Scheme);
                    TotalColumndata.TOTAL_PRICE = newTotalList.Where(x => x.parentId == null).Sum(x => x.TOTAL_PRICE);
                    TotalColumndata.EXCISE_DUTY = newTotalList.Where(x => x.parentId == null).Sum(x => x.EXCISE_DUTY);
                    TotalColumndata.VAT_AMOUNT = newTotalList.Where(x => x.parentId == null).Sum(x => x.VAT_AMOUNT);
                    TotalColumndata.TOTAL_BILL_VALUE = newTotalList.Where(x => x.parentId == null).Sum(x => x.TOTAL_BILL_VALUE);
                    TotalColumndata.GROSS_REALISATION_AMOUNT = newTotalList.Where(x => x.parentId == null).Sum(x => x.GROSS_REALISATION_AMOUNT);
                    TotalColumndata.GROSS_REALISATION_PER_QUANTITY = newTotalList.Where(x => x.parentId == null).Sum(x => x.GROSS_REALISATION_PER_QUANTITY);
                    TotalColumndata.TargetBonus = newTotalList.Where(x => x.parentId == null).Sum(x => x.TargetBonus);
                    TotalColumndata.CollectionBonus = newTotalList.Where(x => x.parentId == null).Sum(x => x.CollectionBonus);
                    TotalColumndata.MonopolyBonus = newTotalList.Where(x => x.parentId == null).Sum(x => x.MonopolyBonus);
                    TotalColumndata.BgBonus = newTotalList.Where(x => x.parentId == null).Sum(x => x.BgBonus);
                    TotalColumndata.CMTPScheme = newTotalList.Where(x => x.parentId == null).Sum(x => x.CMTPScheme);
                    TotalColumndata.VPBScheme = newTotalList.Where(x => x.parentId == null).Sum(x => x.VPBScheme);
                    TotalColumndata.TotalBonus = newTotalList.Where(x => x.parentId == null).Sum(x => x.TotalBonus);
                    TotalColumndata.TotalBonusPerQty = newTotalList.Where(x => x.parentId == null).Sum(x => x.TotalBonusPerQty);
                    TotalColumndata.NCRAmount = newTotalList.Where(x => x.parentId == null).Sum(x => x.NCRAmount);
                    TotalColumndata.NCRPerQty = newTotalList.Where(x => x.parentId == null).Sum(x => x.NCRPerQty);
                    newTotalList.Add(TotalColumndata);
                }
                this._cacheManager.Set(cachekey, newTotalList, 20);
                return newTotalList;

            }

            //  return newTotalList;
        }

        public IEnumerable<GoodsReceiptNotesDetailModel> GetGoodsReceiptNotesData(ReportFiltersModel reportFilters, User userInfo, bool liveData)
        {

            //var companyCode = string.Join(", ", reportFilters.CompanyFilter);
            //companyCode = companyCode == "" ? userInfo.company_code : companyCode;
            var companyCode = string.Empty;
            foreach (var company in reportFilters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);


            IEnumerable<GoodsReceiptNotesDetailModel> data = Enumerable.Empty<GoodsReceiptNotesDetailModel>();
            var data1 = new List<GoodsReceiptNotesDetailModel>();

            string path = System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/GoodsReceiptNotesReport.json");
            IMongoRepository<GoodsReceiptNotesDetailModelMongo> _ItemRepo = new MongoRepository<GoodsReceiptNotesDetailModelMongo>();
            if (liveData == false)
            {


                var datatest = _ItemRepo.ToList();
                foreach (var d in datatest)
                {
                    var mongodb = new GoodsReceiptNotesDetailModel();
                    mongodb.Id = Convert.ToInt16(d.Id);
                    mongodb.MRR_DATE = d.MRR_DATE;
                    mongodb.BRANCH_CODE = d.BRANCH_CODE;
                    mongodb.BRANCH_EDESC = d.BRANCH_EDESC;
                    mongodb.MRR_NO = d.MRR_NO;
                    mongodb.MANUAL_NO = d.MANUAL_NO;
                    mongodb.SUPPLIER_CODE = d.SUPPLIER_CODE;
                    mongodb.SUPPLIER_NAME = d.SUPPLIER_NAME;
                    mongodb.SUPPLIER_INV_NO = d.SUPPLIER_INV_NO;
                    mongodb.SUPPLIER_MRR_NO = d.SUPPLIER_MRR_NO;
                    mongodb.SUPPLIER_INV_DATE = d.SUPPLIER_INV_DATE;
                    mongodb.PP_NO = d.PP_NO;
                    mongodb.REMARKS = d.REMARKS;
                    mongodb.CURRENCY_CODE = d.CURRENCY_CODE;
                    mongodb.EXCHANGE_RATE = d.EXCHANGE_RATE;
                    mongodb.LOCATION_EDESC = d.LOCATION_EDESC;
                    mongodb.ITEM_CODE = d.ITEM_CODE;
                    mongodb.ITEM_EDESC = d.ITEM_EDESC;
                    //mongodb.QUANTITY = d.QUANTITY;
                    //mongodb.UNIT_PRICE = d.UNIT_PRICE;
                    //mongodb.TOTAL_PRICE = d.TOTAL_PRICE;
                    mongodb.QUANTITY = d.QUANTITY != null ? (int)d.QUANTITY : 0;
                    mongodb.UNIT_PRICE = d.UNIT_PRICE != null ? (int)d.UNIT_PRICE : 0;
                    mongodb.TOTAL_PRICE = d.TOTAL_PRICE != null ? (int)d.TOTAL_PRICE : 0;

                    mongodb.FORM_EDESC = d.FORM_EDESC;
                    mongodb.ITEM_GROUP_EDESC = d.ITEM_GROUP_EDESC;
                    mongodb.ITEM_SUBGROUP_EDESC = d.ITEM_SUBGROUP_EDESC;
                    mongodb.CATEGORY_CODE = d.CATEGORY_CODE;
                    mongodb.CATEGORY_EDESC = d.CATEGORY_EDESC;
                    mongodb.COMPANY_CODE = d.COMPANY_CODE;
                    mongodb.COMPANY_EDESC = d.COMPANY_EDESC;
                    mongodb.BRANCH_CODE = d.BRANCH_CODE;
                    mongodb.BRANCH_EDESC = d.BRANCH_EDESC;
                    data1.Add(mongodb);
                    //  var acc = _mapper.Map<GoodsReceiptNotesDetailModelMongo>(d);
                    ///Mongodata.Add(_mapper.Map<GoodsReceiptNotesDetailModelMongo>(d));
                }
                data = data1.AsEnumerable<GoodsReceiptNotesDetailModel>();
                // var data1=   _mapper.Map<GoodsReceiptNotesDetailModel>(datatest.FirstOrDefault());
                //using (StreamReader r = new StreamReader(path))
                //{
                //    string jsonData = r.ReadToEnd();
                //    data = Newtonsoft.Json.JsonConvert.DeserializeObject<EnumerableQuery<GoodsReceiptNotesDetailModel>>(jsonData);
                //}
            }
            else
            {
                var Query = $@"SELECT ROWNUM Id, A.MRR_NO,TO_DATE(A.MRR_DATE) MRR_DATE,A.MANUAL_NO,A.SUPPLIER_CODE , B.SUPPLIER_EDESC SUPPLIER_NAME ,A.SUPPLIER_INV_NO,
                            A.SUPPLIER_MRR_NO, TO_DATE(A.SUPPLIER_INV_DATE) SUPPLIER_INV_DATE , 
                            A.PP_NO, A.REMARKS,  A.CURRENCY_CODE, A.EXCHANGE_RATE ,C.LOCATION_EDESC,D.ITEM_CODE,
                            D.ITEM_EDESC , A.QUANTITY ,A.UNIT_PRICE, A.TOTAL_PRICE, E.FORM_EDESC,
                            SUBSTR(FN_FETCH_GROUP_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', D.PRE_ITEM_CODE),1,100) ITEM_GROUP_EDESC,
                            SUBSTR(FN_FETCH_PRE_DESC(A.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', D.PRE_ITEM_CODE),1,100) ITEM_SUBGROUP_EDESC,
                            D.CATEGORY_CODE,FN_FETCH_DESC(A.COMPANY_CODE,'IP_CATEGORY_CODE',D.CATEGORY_CODE) CATEGORY_EDESC,
                            A.COMPANY_CODE,FN_FETCH_DESC(A.COMPANY_CODE,'COMPANY_SETUP',A.COMPANY_CODE) COMPANY_EDESC,
                            A.BRANCH_CODE,FN_FETCH_DESC(A.COMPANY_CODE,'FA_BRANCH_SETUP',A.BRANCH_CODE) BRANCH_EDESC
                            FROM IP_PURCHASE_MRR A , IP_SUPPLIER_SETUP B, IP_LOCATION_SETUP C, IP_ITEM_MASTER_SETUP D , FORM_SETUP E
                            WHERE A.SUPPLIER_CODE = B.SUPPLIER_CODE
                            AND A.COMPANY_CODE = B.COMPANY_CODE
                            AND A.TO_LOCATION_CODE = C.LOCATION_CODE (+)
                            AND A.COMPANY_CODE = C.COMPANY_CODE (+)
                            AND A.ITEM_CODE = D.ITEM_CODE
                            AND A.COMPANY_CODE = D.COMPANY_CODE
                            AND A.FORM_CODE = E.FORM_CODE
                            AND A.COMPANY_CODE = E.COMPANY_CODE";
                data = this._objectEntity.SqlQuery<GoodsReceiptNotesDetailModel>(Query).ToList();
                List<GoodsReceiptNotesDetailModelMongo> Mongodata = new List<GoodsReceiptNotesDetailModelMongo>();
                foreach (var d in data)
                {
                    var mongodb = new GoodsReceiptNotesDetailModelMongo();
                    mongodb.Id = d.Id.ToString();
                    mongodb.MRR_DATE = d.MRR_DATE;
                    mongodb.BRANCH_CODE = d.BRANCH_CODE;
                    mongodb.BRANCH_EDESC = d.BRANCH_EDESC;
                    mongodb.MRR_NO = d.MRR_NO;
                    mongodb.MANUAL_NO = d.MANUAL_NO;
                    mongodb.SUPPLIER_CODE = d.SUPPLIER_CODE;
                    mongodb.SUPPLIER_NAME = d.SUPPLIER_NAME;
                    mongodb.SUPPLIER_INV_NO = d.SUPPLIER_INV_NO;
                    mongodb.SUPPLIER_MRR_NO = d.SUPPLIER_MRR_NO;
                    mongodb.SUPPLIER_INV_DATE = d.SUPPLIER_INV_DATE;
                    mongodb.PP_NO = d.PP_NO;
                    mongodb.REMARKS = d.REMARKS;
                    mongodb.CURRENCY_CODE = d.CURRENCY_CODE;
                    mongodb.EXCHANGE_RATE = d.EXCHANGE_RATE;
                    mongodb.LOCATION_EDESC = d.LOCATION_EDESC;
                    mongodb.ITEM_CODE = d.ITEM_CODE;
                    mongodb.ITEM_EDESC = d.ITEM_EDESC;
                    mongodb.QUANTITY = d.QUANTITY;
                    mongodb.UNIT_PRICE = d.UNIT_PRICE;
                    mongodb.TOTAL_PRICE = d.TOTAL_PRICE;
                    mongodb.FORM_EDESC = d.FORM_EDESC;
                    mongodb.ITEM_GROUP_EDESC = d.ITEM_GROUP_EDESC;
                    mongodb.ITEM_SUBGROUP_EDESC = d.ITEM_SUBGROUP_EDESC;
                    mongodb.CATEGORY_CODE = d.CATEGORY_CODE;
                    mongodb.CATEGORY_EDESC = d.CATEGORY_EDESC;
                    mongodb.COMPANY_CODE = d.COMPANY_CODE;
                    mongodb.COMPANY_EDESC = d.COMPANY_EDESC;
                    mongodb.BRANCH_CODE = d.BRANCH_CODE;
                    mongodb.BRANCH_EDESC = d.BRANCH_EDESC;
                    Mongodata.Add(mongodb);
                    //  var acc = _mapper.Map<GoodsReceiptNotesDetailModelMongo>(d);
                    ///Mongodata.Add(_mapper.Map<GoodsReceiptNotesDetailModelMongo>(d));
                }
                //var mongodata=  _mapper.Map<List<GoodsReceiptNotesDetailModel>,List<GoodsReceiptNotesDetailModelMongo>>(data.ToList());
                _ItemRepo.Drop();
                _ItemRepo.AddMany(Mongodata);

                //save to jsonfile
                //  string file = System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/GoodsReceiptNotesReport.json");
                //  string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                // File.WriteAllText(file, json);

                //save property               
                //  var fs = File.GetAccessControl(file);
                //  var sid = fs.GetOwner(typeof(SecurityIdentifier));                
                //var ntAccount = sid.Translate(typeof(NTAccount)).ToString();

                //var setting = new GoodsReceiptNotesReportJsonSetting()
                //{
                //     SID = sid.ToString(),
                //    CreatedBy = ntAccount,
                //   CreatedDate = File.GetCreationTime(file),
                //    ModifyDate = File.GetLastWriteTime(file)
                //  };               
                // _setting.SaveSetting(setting);

            }



            //****************************
            //CONDITIONS FITLER START HERE
            //****************************

            //companyFilter
            data = data.Where(x => x.COMPANY_CODE.Contains(companyCode));

            //branchFilter
            if (reportFilters.BranchFilter.Count() > 0)
            {
                data = data.Where(x => reportFilters.BranchFilter.Contains(x.BRANCH_CODE));
            }


            //supplierFilter
            if (reportFilters.SupplierFilter.Count() > 0)
            {
                var selectedSupplier = getSelectedSuplierFromJsonData(_objectEntity, reportFilters);
                data = data.Where(x => selectedSupplier.Contains(x.SUPPLIER_CODE));

            }

            //productFilter
            if (reportFilters.ProductFilter.Count() > 0)
            {
                var selectedItems = getSelectedProductFromJsonData(_objectEntity, reportFilters);
                data = data.Where(x => selectedItems.Contains(x.ITEM_CODE));

            }

            //categoryFilter
            if (reportFilters.CategoryFilter.Count() > 0)
            {
                data = data.Where(x => reportFilters.CategoryFilter.Contains(x.CATEGORY_CODE));
            }




            //range filter
            int min = 0, max = 0;
            ReportFilterHelper.RangeFilterValue(reportFilters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
            {
                data = data.Where(x => x.TOTAL_PRICE >= min && x.TOTAL_PRICE <= max);
                data = data.Where(x => x.QUANTITY >= min && x.QUANTITY <= max);
            }




            //dateFilter
            if (!string.IsNullOrEmpty(reportFilters.FromDate))
            {
                DateTime fromDate = Convert.ToDateTime(reportFilters.FromDate);
                data = data.Where(x => x.MRR_DATE >= fromDate);
            }
            if (!string.IsNullOrEmpty(reportFilters.ToDate))
            {
                DateTime toDate = Convert.ToDateTime(reportFilters.ToDate);
                data = data.Where(x => x.MRR_DATE <= toDate);
            }

            //amountFormat
            var temp = ReportFilterHelper.FigureFilterValue(reportFilters.AmountFigureFilter);


            ////****************************
            ////CONDITIONS FITLER END HERE
            ////****************************



            return data;
        }

        public List<DynamicColumnForNCR> GetDynamiColumns()
        {

            List<DynamicColumnForNCR> staticColumns = new List<DynamicColumnForNCR>();
            staticColumns.Add(new DynamicColumnForNCR { Name = "TargetBonus" });
            staticColumns.Add(new DynamicColumnForNCR { Name = "CollectionBonus" });
            staticColumns.Add(new DynamicColumnForNCR { Name = "MonopolyBonus" });
            staticColumns.Add(new DynamicColumnForNCR { Name = "BgBonus" });
            staticColumns.Add(new DynamicColumnForNCR { Name = "CMTPScheme" });
            staticColumns.Add(new DynamicColumnForNCR { Name = "VPBScheme" });

            return staticColumns;
        }


        public List<MaterializeModel> GetMaterializeReprot(ReportFiltersModel filters, User userInfo, bool sync = false)
        {

            var figureAmountFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter);
            var roundUpAmountFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter);
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userInfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = string.Format($@"SELECT FISCAL_YEAR,
            BILL_NO,
            CUSTOMER_NAME,
            CUSTOMER_PAN,
            BS_DATE (BILL_DATE) BILL_DATE,
to_char(BILL_DATE) BILL_DATEAD,
            AMOUNT,
            DISCOUNT,
            TAXABLE_AMOUNT,
             --  DECODE (IS_BILL_ACTIVE, 'Y', TAX_AMOUNT,0) TAX_AMOUNT,
             TAX_AMOUNT,
            TOTAL_AMOUNT,
            SYNC_WITH_IRD,
            IS_BILL_PRINTED,
            IS_BILL_ACTIVE,
            PRINTED_TIME,
            ENTERED_BY,
            PRINTED_BY,
            IS_REAL_TIME,
            COMPANY_CODE,
            BRANCH_CODE,
            FORM_CODE,
(select table_name from form_detail_setup where form_code=v.form_code and company_code=v.company_code and rownum=1)  as TableName
       FROM V_IRD_CBMS_VAT_REPORT v
      WHERE COMPANY_CODE  IN ({companyCode}) ");
            if (filters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND BRANCH_CODE  IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }

            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " AND TO_DATE(BILL_DATE, 'DD/MM/RRRR')>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') AND TO_DATE(BILL_DATE, 'DD/MM/RRRR') <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";

            if (sync)
            {
                query = query + "  and  SYNC_WITH_IRD='N' ";
            }
            else
            {
                query = query + "  ORDER BY FISCAL_YEAR, BILL_DATE, BILL_NO ";

            }


            var materializeData = _objectEntity.SqlQuery<MaterializeModel>(query).ToList();
            if (sync)
            {

                foreach (var data in materializeData.Where(x => x.SYNC_WITH_IRD == "N"))
                {
                    SynFunctionIRD(data);
                }

            }

            return materializeData;
        }

        public string SynFunctionIRD(MaterializeModel Model)
        {
            using (var client = new HttpClient())
            {
                //FiscalYear
                var username = ConfigurationManager.AppSettings["username"];
                var password = ConfigurationManager.AppSettings["password"];
                var seller_pan = ConfigurationManager.AppSettings["seller_pan"];
                var fiscal_year = ConfigurationManager.AppSettings["FiscalYear"];
                var IRDUrl = ConfigurationManager.AppSettings["IRDUrl"];
                var invoice_date = Convert.ToDateTime(Model.BILL_DATEAD).ToString("yyyy.MM.dd");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                IRDModel billViewModel = new IRDModel
                {
                    username = username,
                    password = password,
                    seller_pan = seller_pan,
                    buyer_pan = Model.CUSTOMER_PAN,
                    buyer_name = Model.CUSTOMER_NAME,
                    fiscal_year = fiscal_year,
                    invoice_number = Model.BILL_NO,
                    invoice_date = invoice_date,
                    total_sales = Model.TAXABLE_AMOUNT ?? 0,
                    taxable_sales_vat = Model.TAXABLE_AMOUNT ?? 0,
                    vat = Model.TAX_AMOUNT ?? 0,
                    excisable_amount = 0,
                    excise = 0,
                    taxable_sales_hst = 0,
                    hst = 0,
                    amount_for_esf = 0,
                    esf = 0,
                    export_sales = 0,
                    tax_exempted_sales = 0,
                    isrealtime = true,
                    datetimeClient = DateTime.Now
                };
                //if(Model.TableName.ToUpper()=="SA_SALES_RETURN")
                //{
                //    IRDUrl = ConfigurationManager.AppSettings["IRDUrlReturn"];
                //}
                var response = client.PostAsJsonAsync(IRDUrl, billViewModel).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync();
                    if (result.Result.ToString() == "100")
                    {
                        SaveIRDLog(Model, "API credentials do not match");
                        return "API credentials do not match";
                    }
                    else if (result.Result.ToString() == "101")
                    {
                        SaveIRDLog(Model, "bill already exists");
                        ifIsRealTimeFalse(Model);
                        return "bill already exists";
                    }
                    else if (result.Result.ToString() == "102")
                    {
                        SaveIRDLog(Model, "exception while saving bill details");
                        return "exception while saving bill details";
                    }
                    else if (result.Result.ToString() == "103")
                    {
                        SaveIRDLog(Model, "Unknown exceptions");
                        //ifIsRealTimeFalse(Model);
                        return "Unknown exceptions";
                    }
                    else if (result.Result.ToString() == "104")
                    {
                        SaveIRDLog(Model, "model invalid");
                        // ifIsRealTimeFalse(Model);
                        return "model invalid";
                    }
                    else if (result.Result.ToString() == "200")
                    {
                        SaveIRDLog(Model, "Success");
                        ifIsRealTimeFalse(Model);
                        return result.Status.ToString();
                    }
                    else
                    {
                        SaveIRDLog(Model, "Error NOt Define");
                        ifIsRealTimeFalse(Model);
                        return result.Status.ToString();
                    }
                }
                else
                {
                    var result = response.Content.ReadAsStringAsync();
                    SaveIRDLog(Model, result.Result.ToString());
                    ifIsRealTimeFalse(Model);
                    return "Error";
                }
            }
        }

        public void ifIsRealTimeFalse(MaterializeModel Record)
        {

            try
            {
                var updatMasterTransactionQuery = $@"UPDATE MASTER_TRANSACTION SET IS_SYNC_WITH_IRD='{"Y"}', IS_REAL_TIME='{"N"}' WHERE VOUCHER_NO='{Record.BILL_NO}' AND FORM_CODE='{Record.FORM_CODE}'";
                _objectEntity.ExecuteSqlCommand(updatMasterTransactionQuery);

            }
            catch (Exception)
            {
                throw;
            }


        }
        public void SaveIRDLog(MaterializeModel Record, string Message)
        {

            try
            {
                var updatMasterTransactionQuery = $@"insert into IRD_LOG(VOUCHER_NO,MESSAGE,FORM_CODE,CREATED_DATE) values ('{Record.BILL_NO}','{Message}','{Record.FORM_CODE}',sysdate)";
                _objectEntity.ExecuteSqlCommand(updatMasterTransactionQuery);

            }
            catch (Exception)
            {
                throw;
            }


        }
        public List<MaterializedViewMasterModel> MaterializedViewReport(ReportFiltersModel filters)
        {
            //var companyCode = string.Join(",", filters.CompanyFilter);List<MaterializedViewMasterModel>
            ////companyCode = companyCode == "" ? this._workContext.CurrentUserinformation.company_code : companyCode;
            //if (string.IsNullOrEmpty(companyCode))
            //    companyCode = this._workContext.CurrentUserinformation.company_code;
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{this._workContext.CurrentUserinformation.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            //string query = string.Format(@"SELECT 
            //                                 A.FORM_CODE,
            //                                -- A.FROM_LOCATION_CODE,
            //                                A.COMPANY_CODE,A.BRANCH_CODE,
            //                                  A.SALES_DATE as SalesDate ,A.MITI as Miti,A.SALES_NO as InvoiceNumber,A.CUSTOMER_EDESC as CustomerName,
            //                                  Round(NVL(FN_CONVERT_CURRENCY(NVL(A.GROSS_AMOUNT,0),'NRS',A.SALES_DATE),0)/{0},{1}) as GrossAmount 
            //                                  FROM
            //                                   (SELECT 
            //                                      A.FORM_CODE,A.FROM_LOCATION_CODE,A.COMPANY_CODE,A.BRANCH_CODE,
            //                                      A.PARTY_TYPE_CODE, A.CUSTOMER_CODE, 
            //                                      A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI, A.SALES_NO, B.CUSTOMER_EDESC,
            //                                      SUM(NVL(A.TOTAL_PRICE,0))  GROSS_AMOUNT
            //                                      FROM SA_SALES_INVOICE A, SA_CUSTOMER_SETUP B, IP_ITEM_MASTER_SETUP C
            //                                      WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE
            //                                      AND A.COMPANY_CODE = B.COMPANY_CODE
            //                                      AND A.ITEM_CODE = C.ITEM_CODE
            //                                      AND A.COMPANY_CODE = C.COMPANY_CODE
            //                                      AND A.COMPANY_CODE IN({2})
            //                                      AND A.DELETED_FLAG = 'N'",
            //                            ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter), ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter), companyCode);

            string query = string.Format($@"SELECT FISCAL_YEAR,
         BILL_NO,
         CUSTOMER_NAME,
         CUSTOMER_PAN,
         BS_DATE (BILL_DATE) BILL_DATE,
         AMOUNT,
         DISCOUNT,
         TAXABLE_AMOUNT,
         TAX_AMOUNT,
         TOTAL_AMOUNT,
         SYNC_WITH_IRD,
         IS_BILL_PRINTED,
         IS_BILL_ACTIVE,
         PRINTED_TIME,
         ENTERED_BY,
         PRINTED_BY,
         IS_REAL_TIME,
         COMPANY_CODE,
         BRANCH_CODE,
         FORM_CODE
    FROM V_IRD_CBMS_VAT_REPORT A WHERE A.COMPANY_CODE IN({companyCode})  ");

            //if (filters.CustomerFilter.Count > 0)
            //{
            //    var customers = filters.CustomerFilter;
            //    var customerConditionQuery = string.Empty;
            //    for (int i = 0; i < customers.Count; i++)
            //    {

            //        if (i == 0)
            //            customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
            //        else
            //        {
            //            customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
            //        }
            //    }
            //    var customerfilter = string.Empty;
            //    foreach (var product in customers)
            //    {
            //        customerfilter += $@"'{product}',";
            //    }
            //    customerfilter = customerfilter.Remove(customerfilter.Length - 1);
            //    query = query + string.Format(@" AND A.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0}  OR (CUSTOMER_CODE IN ({1}) AND GROUP_SKU_FLAG = 'I')) ", customerConditionQuery, customerfilter);

            //}
            if (filters.DocumentFilter.Count > 0)
            {
                query = query + string.Format(@" AND  A.FORM_CODE  IN  ('{0}')", string.Join("','", filters.DocumentFilter).ToString());
            }

            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and TO_CHAR (TO_DATE (BILL_DATE, 'DD-MM-YYYY')) >=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and TO_CHAR (TO_DATE (BILL_DATE, 'DD-MM-YYYY'))  <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";
            //query = query + " GROUP BY A.SALES_DATE,  A.SALES_NO, B.CUSTOMER_EDESC,  A.PARTY_TYPE_CODE, A.CUSTOMER_CODE,A.FORM_CODE,A.COMPANY_CODE,A.BRANCH_CODE";




            query += " ORDER BY FISCAL_YEAR, BILL_DATE, BILL_NO ";


            var salesRegisters = _objectEntity.SqlQuery<MaterializedViewMasterModel>(query).ToList();


            return salesRegisters;
        }
        public List<VatRegistrationIRDMasterModel> VatRegisterIRDReport(ReportFiltersModel filters)
        {
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{this._workContext.CurrentUserinformation.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            string query = string.Format($@"SELECT BS_DATE (SALES_DATE) MITI,INVOICE_NO,PARTY_NAME,VAT_NO,
         FN_CONVERT_CURRENCY (NVL (GROSS_SALES, 0) * NVL (EXCHANGE_RATE, 1),'NRS',SALES_DATE)GROSS_SALES,
         FN_CONVERT_CURRENCY (NVL (TAXABLE_SALES, 1) * NVL (EXCHANGE_RATE, 1),'NRS',SALES_DATE)TAXABLE_SALES,
         FN_CONVERT_CURRENCY (NVL (VAT, 0) * NVL (EXCHANGE_RATE, 1),'NRS',SALES_DATE)VAT,
         FN_CONVERT_CURRENCY (NVL (TOTAL_SALES, 0) * NVL (EXCHANGE_RATE, 1),'NRS',SALES_DATE)TOTAL_SALES,
         FORM_CODE,
         BRANCH_CODE,
         CREDIT_DAYS,
         DELETED_FLAG,
         SALES_DISCOUNT,
         MANUAL_NO,
         0 ZERO_RATE_EXPORT
        FROM V$SALES_INVOICE_REPORT3
       WHERE     COMPANY_CODE IN({companyCode})  ");


            if (filters.DocumentFilter.Count > 0)
            {
                query = query + string.Format(@" AND  FORM_CODE  IN  ('{0}')", string.Join("','", filters.DocumentFilter).ToString());
            }

            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and TO_CHAR (TO_DATE (SALES_DATE, 'DD-MM-YYYY')) >=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and TO_CHAR (TO_DATE (SALES_DATE, 'DD-MM-YYYY'))  <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";





            query += " ORDER BY BS_DATE (SALES_DATE), INVOICE_NO ";


            var salesRegisters = _objectEntity.SqlQuery<VatRegistrationIRDMasterModel>(query).ToList();


            return salesRegisters;
        }
        public List<PurchaseReturnRegistersDetail> GetPurchaseReturnRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = string.Format(@"SELECT RETURN_date as ReturnDate,
                        bs_date (RETURN_date) Miti ,
                       RETURN_no as InvoiceNumber,
                       INITCAP (IMS.ITEM_EDESC) ItemName,
                       INITCAP (ls.location_edesc) LocationName,
                       SI.MANUAL_NO as ManualNo,
                       SI.REMARKS as REMARKS,
                        INITCAP (PTC.SUPPLIER_EDESC) SUPPLIERNAME,
                       INITCAP (SI.MU_CODE) Unit ,
                       Round(NVL(SI.QUANTITY,0)/{0},{1}) as Quantity,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_UNIT_PRICE,0),'NRS',SI.Return_DATE),0)/{2},{3}) as UnitPrice,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_TOTAL_PRICE,0),'NRS',SI.Return_DATE),0)/{4},{5}) as TotalPrice
                        FROM IP_PURCHASE_RETURN si,
                       IP_ITEM_MASTER_SETUP ims,
                       IP_LOCATION_SETUP ls,
                       ip_supplier_setup ptc
                       WHERE SI.ITEM_CODE = IMS.ITEM_CODE
                        and si.company_code=IMS.company_code
                        --and si.AREA_CODE = ast.AREA_CODE
                       --and SI.COMPANY_CODE=cs.company_code
                       and SI.company_code=ls.company_code
                          --and  SI.SHIPPING_ADDRESS= ct.city_code
                       AND si.COMPANY_CODE IN(" + companyCode + @") "
                     , ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.QuantityRoundUpFilter), ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter)
                        , ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter));
            //if (filters.CustomerFilter.Count > 0)
            //{

            //    var customers = filters.CustomerFilter;
            //    var customerConditionQuery = string.Empty;
            //    for (int i = 0; i < customers.Count; i++)
            //    {

            //        if (i == 0)
            //            customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
            //        else
            //        {
            //            customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
            //        }
            //    }

            //    query = query + string.Format(@" AND SI.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join("','", customers));
            //}
            if (filters.ProductFilter.Count > 0)
            {

                var products = filters.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND SI.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", productConditionQuery, string.Join("','", products));

                //query = query + string.Format(@" AND SI.ITEM_CODE IN ({0})", string.Join(",", filters.ProductFilter).ToString());
            }
            //if (filters.DocumentFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND  SI. FORM_CODE  IN  ('{0}')", string.Join("','", filters.DocumentFilter).ToString());
            //}
            //if (filters.PartyTypeFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND SI.PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", filters.PartyTypeFilter).ToString());
            //}
            //if (filters.AreaTypeFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND SI.AREA_CODE IN ('{0}') ", string.Join("','", filters.AreaTypeFilter).ToString());
            //}
            if (filters.CategoryFilter.Count > 0)
            {
                query = query + string.Format(@" AND ims.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            }
            if (filters.EmployeeFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
            }
            if (filters.AgentFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.AGENT_CODE IN  ('{0}')", string.Join("','", filters.AgentFilter).ToString());
            }
            if (filters.DivisionFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.DIVISION_CODE IN  ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
            }
            if (filters.LocationFilter.Count > 0)
            {

                var locations = filters.LocationFilter;
                var locationConditionQuery = string.Empty;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%'", locations[i]);
                    else
                    {
                        locationConditionQuery += string.Format(" OR  LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                query = query + string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                //query = query + string.Format(@" AND (SI.FROM_LOCATION_CODE = LS.LOCATION_CODE OR SI.FROM_LOCATION_CODE IN ('{0}'))", string.Join("','", filters.LocationFilter).ToString());
            }
            //if (filters.CompanyFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND si.COMPANY_CODE = cmps.COMPANY_CODE AND SI.COMPANY_CODE IN ('{0}')", string.Join("','", filters.CompanyFilter).ToString());
            //}
            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            //EMPLOYEE_CODE doesn't exist on the NNPL &WLINK database , So We have manually created column of EMPLOYEE_CODE for the both database .
            query = query + @" AND SI.FROM_LOCATION_CODE = LS.LOCATION_CODE
                        and si.Deleted_flag = 'N'
                     -- AND SI.SUPPLIER_CODE = ES.SUPPLIER_CODE(+)
                       AND SI.SUPPLIER_CODE = PTC.SUPPLIER_CODE(+)";
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and Return_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and Return_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";


            int min = 0, max = 0;

            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.SALES_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.SALES_DATE),0)  <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(SI.QUANTITY,0) >= {0} and NVL(SI.QUANTITY,0) <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.SALES_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.SALES_DATE),0) <= {1}", min, max);


            var salesRegisters = _objectEntity.SqlQuery<PurchaseReturnRegistersDetail>(query).ToList();
            return salesRegisters;
        }
        public List<SalesRegistersDetail> GetAgentWiseSalesRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = string.Format(@"SELECT sales_date as SalesDate,
                       bs_date (sales_date) Miti ,
                       sales_no as InvoiceNumber,
                       INITCAP (CS.CUSTOMER_EDESC) CustomerName,
                       INITCAP (IMS.ITEM_EDESC) ItemName,
                       INITCAP (ls.location_edesc) LocationName,
                       SI.MANUAL_NO as ManualNo,
                       SI.REMARKS as REMARKS,
                       --INITCAP (ES.EMPLOYEE_EDESC) Dealer,
                       INITCAP (PTC.PARTY_TYPE_EDESC) PartyType,
                       SI.SHIPPING_ADDRESS as SHIPPINGCODE,
                       SI.SHIPPING_CONTACT_NO as ShippingContactNo,
                       --CT.CITY_EDESC as ShippingAddress,
                       -- ast.AREA_EDESC,
                        ags.AGENT_EDESC,
                        iss.Brand_name BRAND_EDESC,
                       INITCAP (SI.MU_CODE) Unit ,
                       Round(NVL(SI.QUANTITY,0)/{0},{1}) as Quantity,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_UNIT_PRICE,0),'NRS',SI.SALES_DATE),0)/{2},{3}) as UnitPrice,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_TOTAL_PRICE,0),'NRS',SI.SALES_DATE),0)/{4},{5}) as TotalPrice
                        FROM SA_SALES_INVOICE si,
                       IP_ITEM_MASTER_SETUP ims,
                       SA_CUSTOMER_SETUP cs,
                       IP_LOCATION_SETUP ls,
                       --HR_EMPLOYEE_SETUP es,
                       IP_PARTY_TYPE_CODE ptc,
                        AGENT_SETUP ags,
                        IP_ITEM_SPEC_SETUP iss
                           --  CITY_CODE ct,
                       -- AREA_SETUP ast
                       WHERE SI.ITEM_CODE = IMS.ITEM_CODE
                        and si.company_code=IMS.company_code
                       -- and si.AREA_CODE = ast.AREA_CODE
                       and SI.COMPANY_CODE=cs.company_code
                       and SI.company_code=ls.company_code
                       and si.agent_code=ags.agent_code and si.company_code=ags.company_code
                        and si.item_code=iss.item_code and si.company_code=iss.company_code
                        --  and  SI.SHIPPING_ADDRESS= ct.city_code
                       AND si.COMPANY_CODE IN(" + companyCode + @") AND si.CUSTOMER_CODE = cs.CUSTOMER_CODE"
                     , ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.QuantityRoundUpFilter), ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter)
                        , ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter));
            if (filters.CustomerFilter.Count > 0)
            {

                var customers = filters.CustomerFilter;
                var customerConditionQuery = string.Empty;
                for (int i = 0; i < customers.Count; i++)
                {

                    if (i == 0)
                        customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    else
                    {
                        customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND SI.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join("','", customers));
            }
            if (filters.ProductFilter.Count > 0)
            {

                var products = filters.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND SI.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", productConditionQuery, string.Join("','", products));

                //query = query + string.Format(@" AND SI.ITEM_CODE IN ({0})", string.Join(",", filters.ProductFilter).ToString());
            }
            if (filters.DocumentFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI. FORM_CODE  IN  ('{0}')", string.Join("','", filters.DocumentFilter).ToString());
            }
            if (filters.PartyTypeFilter.Count > 0)
            {
                query = query + string.Format(@" AND SI.PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", filters.PartyTypeFilter).ToString());
            }
            //if (filters.AreaTypeFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND SI.AREA_CODE IN ('{0}') ", string.Join("','", filters.AreaTypeFilter).ToString());
            //}
            if (filters.CategoryFilter.Count > 0)
            {
                query = query + string.Format(@" AND ims.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            }
            //if (filters.EmployeeFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND  SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
            //}
            if (filters.AgentFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.AGENT_CODE IN  ('{0}')", string.Join("','", filters.AgentFilter).ToString());
            }
            if (filters.DivisionFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.DIVISION_CODE IN  ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
            }
            if (filters.LocationFilter.Count > 0)
            {

                var locations = filters.LocationFilter;
                var locationConditionQuery = string.Empty;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%'", locations[i]);
                    else
                    {
                        locationConditionQuery += string.Format(" OR  LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                query = query + string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                //query = query + string.Format(@" AND (SI.FROM_LOCATION_CODE = LS.LOCATION_CODE OR SI.FROM_LOCATION_CODE IN ('{0}'))", string.Join("','", filters.LocationFilter).ToString());
            }
            //if (filters.CompanyFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND si.COMPANY_CODE = cmps.COMPANY_CODE AND SI.COMPANY_CODE IN ('{0}')", string.Join("','", filters.CompanyFilter).ToString());
            //}
            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            //EMPLOYEE_CODE doesn't exist on the NNPL &WLINK database , So We have manually created column of EMPLOYEE_CODE for the both database .
            query = query + @" AND SI.FROM_LOCATION_CODE = LS.LOCATION_CODE
                        and si.Deleted_flag = 'N'
                     -- AND SI.EMPLOYEE_CODE = ES.EMPLOYEE_CODE(+)
                       AND SI.PARTY_TYPE_CODE = PTC.PARTY_TYPE_CODE(+)";
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and SALES_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and SALES_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";


            int min = 0, max = 0;

            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.SALES_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.SALES_DATE),0)  <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(SI.QUANTITY,0) >= {0} and NVL(SI.QUANTITY,0) <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.SALES_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.SALES_DATE),0) <= {1}", min, max);


            var salesRegisters = _objectEntity.SqlQuery<SalesRegistersDetail>(query).ToList();
            foreach (var item in salesRegisters)
            {
                item.AGENT_EDESC = item.AGENT_EDESC == null ? "" : item.AGENT_EDESC;
                if (item.AGENT_EDESC.Split('-').Count() > 1)
                {
                    item.AGENT_CODE = item.AGENT_EDESC.Split('-')[0];
                    item.AGENT_EDESC = item.AGENT_EDESC.Split('-')[1];
                }
                else
                {
                    item.AGENT_CODE = "-";
                }
            }
            return salesRegisters;
        }
        public List<PurchaseReturnRegistersDetail> GetPurchaseRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = string.Format(@"SELECT INVOICE_DATE as ReturnDate,
                        bs_date (INVOICE_DATE) Miti ,
                       INVOICE_NO as InvoiceNumber,
                       INITCAP (IMS.ITEM_EDESC) ItemName,
                       INITCAP (ls.location_edesc) LocationName,
                       SI.MANUAL_NO as ManualNo,
                       SI.REMARKS as REMARKS,
                        INITCAP (PTC.SUPPLIER_EDESC) SUPPLIERNAME,
                       INITCAP (SI.MU_CODE) Unit ,
                       Round(NVL(SI.QUANTITY,0)/{0},{1}) as Quantity,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_UNIT_PRICE,0),'NRS',SI.INVOICE_DATE),0)/{2},{3}) as UnitPrice,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_TOTAL_PRICE,0),'NRS',SI.INVOICE_DATE),0)/{4},{5}) as TotalPrice
                        FROM IP_PURCHASE_INVOICE si,
                       IP_ITEM_MASTER_SETUP ims,
                       IP_LOCATION_SETUP ls,
                       ip_supplier_setup ptc
                       WHERE SI.ITEM_CODE = IMS.ITEM_CODE
                        and si.company_code=IMS.company_code
                        --and si.AREA_CODE = ast.AREA_CODE
                       --and SI.COMPANY_CODE=cs.company_code
                       and SI.company_code=ls.company_code
                          --and  SI.SHIPPING_ADDRESS= ct.city_code
                       AND si.COMPANY_CODE IN(" + companyCode + @") "
                     , ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.QuantityRoundUpFilter), ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter)
                        , ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter));
            //if (filters.CustomerFilter.Count > 0)
            //{

            //    var customers = filters.CustomerFilter;
            //    var customerConditionQuery = string.Empty;
            //    for (int i = 0; i < customers.Count; i++)
            //    {

            //        if (i == 0)
            //            customerConditionQuery += string.Format("MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%' from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
            //        else
            //        {
            //            customerConditionQuery += string.Format(" OR  MASTER_CUSTOMER_CODE like (Select DISTINCT(MASTER_CUSTOMER_CODE) || '%'  from SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", customers[i], companyCode);
            //        }
            //    }

            //    query = query + string.Format(@" AND SI.CUSTOMER_CODE IN (SELECT DISTINCT(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE  {0} OR (CUSTOMER_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", customerConditionQuery, string.Join("','", customers));
            //}
            if (filters.ProductFilter.Count > 0)
            {

                var products = filters.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND SI.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", productConditionQuery, string.Join("','", products));

                //query = query + string.Format(@" AND SI.ITEM_CODE IN ({0})", string.Join(",", filters.ProductFilter).ToString());
            }
            //if (filters.DocumentFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND  SI. FORM_CODE  IN  ('{0}')", string.Join("','", filters.DocumentFilter).ToString());
            //}
            //if (filters.PartyTypeFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND SI.PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", filters.PartyTypeFilter).ToString());
            //}
            //if (filters.AreaTypeFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND SI.AREA_CODE IN ('{0}') ", string.Join("','", filters.AreaTypeFilter).ToString());
            //}
            if (filters.CategoryFilter.Count > 0)
            {
                query = query + string.Format(@" AND ims.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            }
            if (filters.EmployeeFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
            }
            if (filters.AgentFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.AGENT_CODE IN  ('{0}')", string.Join("','", filters.AgentFilter).ToString());
            }
            if (filters.DivisionFilter.Count > 0)
            {
                query = query + string.Format(@" AND  SI.DIVISION_CODE IN  ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
            }
            if (filters.LocationFilter.Count > 0)
            {

                var locations = filters.LocationFilter;
                var locationConditionQuery = string.Empty;
                for (int i = 0; i < locations.Count; i++)
                {

                    if (i == 0)
                        locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%'", locations[i]);
                    else
                    {
                        locationConditionQuery += string.Format(" OR  LOCATION_CODE like '{0}%' ", locations[i]);
                    }
                }
                query = query + string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
                //query = query + string.Format(@" AND (SI.FROM_LOCATION_CODE = LS.LOCATION_CODE OR SI.FROM_LOCATION_CODE IN ('{0}'))", string.Join("','", filters.LocationFilter).ToString());
            }
            //if (filters.CompanyFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND si.COMPANY_CODE = cmps.COMPANY_CODE AND SI.COMPANY_CODE IN ('{0}')", string.Join("','", filters.CompanyFilter).ToString());
            //}
            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            //EMPLOYEE_CODE doesn't exist on the NNPL &WLINK database , So We have manually created column of EMPLOYEE_CODE for the both database .
            query = query + @" AND SI.TO_LOCATION_CODE = LS.LOCATION_CODE
                        and si.Deleted_flag = 'N'
                     -- AND SI.SUPPLIER_CODE = ES.SUPPLIER_CODE(+)
                       AND SI.SUPPLIER_CODE = PTC.SUPPLIER_CODE(+)";
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and INVOICE_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and INVOICE_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";


            int min = 0, max = 0;

            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.SALES_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.SALES_DATE),0)  <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(SI.QUANTITY,0) >= {0} and NVL(SI.QUANTITY,0) <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.SALES_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.SALES_DATE),0) <= {1}", min, max);


            var salesRegisters = _objectEntity.SqlQuery<PurchaseReturnRegistersDetail>(query).ToList();
            return salesRegisters;
        }
        public List<PurchasePendingDetailModel> GetPurchasePendingReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = string.Format(@"SELECT TO_CHAR(ORDER_DATE) ORDER_DATE,
                        bs_date (ORDER_DATE) Miti ,
                       ORDER_NO ,
                        MANUAL_NO,
                       INITCAP (IMS.ITEM_EDESC) ITEM_EDESC,
                       --INITCAP (ls.location_edesc) LocationName,
                       SI.MANUAL_NO ,
                       SI.REMARKS ,
                        INITCAP (PTC.SUPPLIER_EDESC) SUPPLIER_EDESC,
                       INITCAP (SI.MU_CODE) MU_CODE ,
                       Round(NVL(SI.QUANTITY,0)/{0},{1}) as QUANTITY,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_UNIT_PRICE,0),'NRS',SI.ORDER_DATE),0)/{2},{3}) as UNIT_PRICE,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_TOTAL_PRICE,0),'NRS',SI.ORDER_DATE),0)/{4},{5}) as TOTAL_PRICE
                        FROM IP_PURCHASE_ORDER si,
                       IP_ITEM_MASTER_SETUP ims,
                      -- IP_LOCATION_SETUP ls,
                       ip_supplier_setup ptc
                       WHERE SI.ITEM_CODE = IMS.ITEM_CODE
                        and si.company_code=IMS.company_code
                        and si.supplier_code=ptc.supplier_code 
                        and si.company_code=ptc.company_code
                        and si.ORDER_NO NOT IN (SELECT REFERENCE_NO FROM REFERENCE_DETAIL)
                       AND si.COMPANY_CODE IN(" + companyCode + @") "
                     , ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.QuantityRoundUpFilter), ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter)
                        , ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter));
            if (filters.ProductFilter.Count > 0)
            {

                var products = filters.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND SI.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", productConditionQuery, string.Join("','", products));


            }

            //if (filters.CategoryFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND ims.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            //}
            //if (filters.EmployeeFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND  SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
            //}
            //if (filters.AgentFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND  SI.AGENT_CODE IN  ('{0}')", string.Join("','", filters.AgentFilter).ToString());
            //}
            //if (filters.DivisionFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND  SI.DIVISION_CODE IN  ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
            //}
            //if (filters.LocationFilter.Count > 0)
            //{

            //    var locations = filters.LocationFilter;
            //    var locationConditionQuery = string.Empty;
            //    for (int i = 0; i < locations.Count; i++)
            //    {

            //        if (i == 0)
            //            locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%'", locations[i]);
            //        else
            //        {
            //            locationConditionQuery += string.Format(" OR  LOCATION_CODE like '{0}%' ", locations[i]);
            //        }
            //    }
            //    query = query + string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
            //    //query = query + string.Format(@" AND (SI.FROM_LOCATION_CODE = LS.LOCATION_CODE OR SI.FROM_LOCATION_CODE IN ('{0}'))", string.Join("','", filters.LocationFilter).ToString());
            //}
            if (filters.CompanyFilter.Count > 0)
            {
                query = query + string.Format(@" AND si.COMPANY_CODE = cmps.COMPANY_CODE AND SI.COMPANY_CODE IN ('{0}')", string.Join("','", filters.CompanyFilter).ToString());
            }
            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            //EMPLOYEE_CODE doesn't exist on the NNPL &WLINK database , So We have manually created column of EMPLOYEE_CODE for the both database .
            //query = query + @" AND SI.TO_LOCATION_CODE = LS.LOCATION_CODE
            //            and si.Deleted_flag = 'N'
            //         -- AND SI.SUPPLIER_CODE = ES.SUPPLIER_CODE(+)
            //           AND SI.SUPPLIER_CODE = PTC.SUPPLIER_CODE(+)";
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and ORDER_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and ORDER_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";


            int min = 0, max = 0;

            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.ORDER_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.ORDER_DATE),0)  <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(SI.QUANTITY,0) >= {0} and NVL(SI.QUANTITY,0) <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.ORDER_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.ORDER_DATE),0) <= {1}", min, max);


            var salesRegisters = _objectEntity.SqlQuery<PurchasePendingDetailModel>(query).ToList();
            return salesRegisters;
        }
        public List<PurchasePendingDetailModel> GetPurchaseOrderReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = string.Format(@"SELECT TO_CHAR(ORDER_DATE) ORDER_DATE,
                        bs_date (ORDER_DATE) Miti ,
                       ORDER_NO ,
                        MANUAL_NO,
                       INITCAP (IMS.ITEM_EDESC) ITEM_EDESC,
                       --INITCAP (ls.location_edesc) LocationName,
                       SI.MANUAL_NO ,
                       SI.REMARKS ,
                        INITCAP (PTC.SUPPLIER_EDESC) SUPPLIER_EDESC,
                       INITCAP (SI.MU_CODE) MU_CODE ,
                       Round(NVL(SI.QUANTITY,0)/{0},{1}) as QUANTITY,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_UNIT_PRICE,0),'NRS',SI.ORDER_DATE),0)/{2},{3}) as UNIT_PRICE,
                       Round(NVL(FN_CONVERT_CURRENCY(NVL(SI.CALC_TOTAL_PRICE,0),'NRS',SI.ORDER_DATE),0)/{4},{5}) as TOTAL_PRICE
                        FROM IP_PURCHASE_ORDER si,
                       IP_ITEM_MASTER_SETUP ims,
                      -- IP_LOCATION_SETUP ls,
                       ip_supplier_setup ptc
                       WHERE SI.ITEM_CODE = IMS.ITEM_CODE
                        and si.company_code=IMS.company_code
                        and si.supplier_code=ptc.supplier_code 
                        and si.company_code=ptc.company_code
                        and si.ORDER_NO  IN (SELECT REFERENCE_NO FROM REFERENCE_DETAIL)
                       AND si.COMPANY_CODE IN(" + companyCode + @") "
                     , ReportFilterHelper.FigureFilterValue(filters.QuantityFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.QuantityRoundUpFilter), ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter)
                        , ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter),
                        ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter));
            if (filters.ProductFilter.Count > 0)
            {

                var products = filters.ProductFilter;
                var productConditionQuery = string.Empty;
                for (int i = 0; i < products.Count; i++)
                {

                    if (i == 0)
                        productConditionQuery += string.Format("MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%' from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    else
                    {
                        productConditionQuery += string.Format(" OR  MASTER_ITEM_CODE like (Select DISTINCT(MASTER_ITEM_CODE) || '%'  from IP_ITEM_MASTER_SETUP WHERE ITEM_CODE = '{0}' AND COMPANY_CODE IN({1}) AND GROUP_SKU_FLAG='G')", products[i], companyCode);
                    }
                }

                query = query + string.Format(@" AND SI.ITEM_CODE IN (SELECT DISTINCT(ITEM_CODE) FROM IP_ITEM_MASTER_SETUP WHERE {0} OR (ITEM_CODE IN ('{1}') AND GROUP_SKU_FLAG='I')) ", productConditionQuery, string.Join("','", products));


            }

            //if (filters.CategoryFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND ims.CATEGORY_CODE IN ('{0}') ", string.Join("','", filters.CategoryFilter).ToString());
            //}
            //if (filters.EmployeeFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND  SI.EMPLOYEE_CODE IN  ('{0}')", string.Join("','", filters.EmployeeFilter).ToString());
            //}
            //if (filters.AgentFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND  SI.AGENT_CODE IN  ('{0}')", string.Join("','", filters.AgentFilter).ToString());
            //}
            //if (filters.DivisionFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND  SI.DIVISION_CODE IN  ('{0}')", string.Join("','", filters.DivisionFilter).ToString());
            //}
            //if (filters.LocationFilter.Count > 0)
            //{

            //    var locations = filters.LocationFilter;
            //    var locationConditionQuery = string.Empty;
            //    for (int i = 0; i < locations.Count; i++)
            //    {

            //        if (i == 0)
            //            locationConditionQuery += string.Format("SELECT LOCATION_CODE FROM IP_LOCATION_SETUP WHERE LOCATION_CODE LIKE '{0}%'", locations[i]);
            //        else
            //        {
            //            locationConditionQuery += string.Format(" OR  LOCATION_CODE like '{0}%' ", locations[i]);
            //        }
            //    }
            //    query = query + string.Format(@" AND SI.FROM_LOCATION_CODE IN ({0} OR LOCATION_CODE IN ('{1}'))", locationConditionQuery, string.Join("','", locations));
            //    //query = query + string.Format(@" AND (SI.FROM_LOCATION_CODE = LS.LOCATION_CODE OR SI.FROM_LOCATION_CODE IN ('{0}'))", string.Join("','", filters.LocationFilter).ToString());
            //}
            if (filters.CompanyFilter.Count > 0)
            {
                query = query + string.Format(@" AND si.COMPANY_CODE = cmps.COMPANY_CODE AND SI.COMPANY_CODE IN ('{0}')", string.Join("','", filters.CompanyFilter).ToString());
            }
            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND SI.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            //EMPLOYEE_CODE doesn't exist on the NNPL &WLINK database , So We have manually created column of EMPLOYEE_CODE for the both database .
            //query = query + @" AND SI.TO_LOCATION_CODE = LS.LOCATION_CODE
            //            and si.Deleted_flag = 'N'
            //         -- AND SI.SUPPLIER_CODE = ES.SUPPLIER_CODE(+)
            //           AND SI.SUPPLIER_CODE = PTC.SUPPLIER_CODE(+)";
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and ORDER_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and ORDER_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";


            int min = 0, max = 0;

            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.ORDER_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.ORDER_DATE),0)  <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(SI.QUANTITY,0) >= {0} and NVL(SI.QUANTITY,0) <= {1}", min, max);

            ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);
            if (!(min == 0 && max == 0))
                query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.ORDER_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.ORDER_DATE),0) <= {1}", min, max);


            var salesRegisters = _objectEntity.SqlQuery<PurchasePendingDetailModel>(query).ToList();
            return salesRegisters;
        }
        public List<PurchaseVatRegistrationDetailModel> GetPurchaseVatRegisterReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }
            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);
            string query = $@"SELECT BS_DATE (INVOICE_DATE) MITI,
                                 INVOICE_NO,
                                 PARTY_NAME,
                                 VAT_NO,
                                 FN_CONVERT_CURRENCY (NVL (GROSS_PURCHASE, 0) * NVL (EXCHANGE_RATE, 1),
                                                      'NRS',
                                                      INVOICE_DATE)
                                    GROSS_AMOUNT,
                                 FN_CONVERT_CURRENCY (
                                    NVL (TAXABLE_PURCHASE, 0) * NVL (EXCHANGE_RATE, 1),
                                    'NRS',
                                    INVOICE_DATE)
                                    TAXABLE_AMOUNT,
                                 FN_CONVERT_CURRENCY (NVL (VAT, 0) * NVL (EXCHANGE_RATE, 1),
                                                      'NRS',
                                                      INVOICE_DATE)
                                    VAT_AMOUNT,
                                 --  FN_CONVERT_CURRENCY (NVL (TOTAL_PURCHASE, 0) * NVL (EXCHANGE_RATE, 1),
                                 --    'NRS',
                                 --     INVOICE_DATE)TOTAL_AMOUNT,
                                 NVL (
                                    FN_CONVERT_CURRENCY (NVL (VAT, 0) * NVL (EXCHANGE_RATE, 1),
                                                         'NRS',
                                                         INVOICE_DATE),
                                    0)
                                 + NVL (
                                      FN_CONVERT_CURRENCY (
                                         NVL (TAXABLE_PURCHASE, 0) * NVL (EXCHANGE_RATE, 1),
                                         'NRS',
                                         INVOICE_DATE),
                                      0)
                                    TOTAL_AMOUNT,
                                 FORM_CODE,
                                 P_TYPE,
                                 MANUAL_NO,
                                 BS_DATE (VOUCHER_DATE) VOUCHER_DATE,
                                 TO_CHAR(INVOICE_DATE)INVOICE_DATE,
                                 TABLE_NAME,
                                 SUPPLIER_CODE,
                                 CASE
                                    WHEN TABLE_NAME = 'IP_PURCHASE_INVOICE'
                                    THEN
                                       (SELECT LISTAGG (ACC_EDESC, ',')
                                                  WITHIN GROUP (ORDER BY ACC_EDESC)
                                          FROM FA_DOUBLE_VOUCHER X, FA_CHART_OF_ACCOUNTS_SETUP Y
                                         WHERE     X.MANUAL_NO = A.MANUAL_NO
                                               AND X.COMPANY_CODE = A.COMPANY_CODE
                                               AND X.TRANSACTION_TYPE = 'DR'
                                               AND X.COMPANY_CODE = Y.COMPANY_CODE
                                               AND X.ACC_CODE = Y.ACC_CODE
                                               AND Y.ACC_NATURE = 'SB')
                                    ELSE
                                       (SELECT LISTAGG (ACC_EDESC, ',')
                                                  WITHIN GROUP (ORDER BY ACC_EDESC)
                                          FROM FA_DOUBLE_VOUCHER X, FA_CHART_OF_ACCOUNTS_SETUP Y
                                         WHERE     X.MANUAL_NO = A.INVOICE_NO
                                               AND X.COMPANY_CODE = A.COMPANY_CODE
                                               AND X.TRANSACTION_TYPE = 'CR'
                                               AND X.COMPANY_CODE = Y.COMPANY_CODE
                                               AND X.ACC_CODE = Y.ACC_CODE
                                               AND Y.ACC_NATURE = 'SB')
                                 END
                                    ACC_INT_VOUCHER
                            FROM V$PURCHASE_INVOICE_REPORT3 A
                           WHERE 1=1 ";



            if (filters.CompanyFilter.Count > 0)
            {
                query = query + string.Format(@"  AND A.COMPANY_CODE IN ('{0}')", string.Join("','", filters.CompanyFilter).ToString());
            }
            if (filters.BranchFilter.Count > 0)
            {
                query = query + string.Format(@" AND A.BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            //EMPLOYEE_CODE doesn't exist on the NNPL &WLINK database , So We have manually created column of EMPLOYEE_CODE for the both database .
            //query = query + @" AND SI.TO_LOCATION_CODE = LS.LOCATION_CODE
            //            and si.Deleted_flag = 'N'
            //         -- AND SI.SUPPLIER_CODE = ES.SUPPLIER_CODE(+)
            //           AND SI.SUPPLIER_CODE = PTC.SUPPLIER_CODE(+)";
            if (!string.IsNullOrEmpty(filters.FromDate))
                query = query + " and A.INVOICE_DATE>=TO_DATE('" + filters.FromDate + "', 'YYYY-MM-DD') and A.INVOICE_DATE <= TO_DATE('" + filters.ToDate + "', 'YYYY-MM-DD')";

            query += " ORDER BY BS_DATE (INVOICE_DATE), MANUAL_NO, INVOICE_NO";


            //int min = 0, max = 0;

            //ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);
            //if (!(min == 0 && max == 0))
            //    query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.ORDER_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.TOTAL_PRICE,0),'NRS',SI.ORDER_DATE),0)  <= {1}", min, max);

            //ReportFilterHelper.RangeFilterValue(filters.QuantityRangeFilter, out min, out max);
            //if (!(min == 0 && max == 0))
            //    query = query + string.Format(@" and NVL(SI.QUANTITY,0) >= {0} and NVL(SI.QUANTITY,0) <= {1}", min, max);

            //ReportFilterHelper.RangeFilterValue(filters.RateRangeFilter, out min, out max);
            //if (!(min == 0 && max == 0))
            //    query = query + string.Format(@" and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.ORDER_DATE),0) >= {0} and NVL(FN_CONVERT_CURRENCY(NVL(SI.UNIT_PRICE,0),'NRS',SI.ORDER_DATE),0) <= {1}", min, max);


            var salesRegisters = _objectEntity.SqlQuery<PurchaseVatRegistrationDetailModel>(query).ToList();
            return salesRegisters;
        }
        #region NewReports
        public List<SalesExciseRegisterModel> SalesExciseRegister(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            //var companyCode = string.Join(",", filters.CompanyFilter);
            //companyCode = companyCode == "" ? userinfo.company_code : companyCode;
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            string query = string.Format(@"SELECT BS_DATE(SALES_DATE) MITI
	                                                ,INVOICE_NO AS InvoiceNo
	                                                ,PARTY_NAME AS PartyName
	                                                ,VAT_NO AS PANNo
	                                                ,FORM_CODE
	                                                ,BRANCH_CODE
	                                                ,TO_CHAR(CREDIT_DAYS) CREDIT_DAYS
	                                                ,DELETED_FLAG
	                                                ,Round(CASE 
			                                            WHEN TABLE_NAME = 'SALES_RETURN'
				                                            THEN - NVL(SALES_DISCOUNT, 0)
			                                            ELSE NVL(SALES_DISCOUNT, 0)
			                                            END / 1, 2) AS Discount
	                                                ,NVL(MANUAL_NO,'n/a') MANUAL_NO
	                                                ,DELETED_FLAG
                                                    ,QUANTITY 
	                                                ,FN_CONVERT_CURRENCY(Round((NVL(TOTAL_SALES, 0) * NVL(EXCHANGE_RATE, 1)) / 1, 2), 'NRS', SALES_DATE) AS NetSales
	                                                ,FN_CONVERT_CURRENCY(NVL(EXCISE_AMOUNT, 0) * NVL(EXCHANGE_RATE, 1), 'NRS', SALES_DATE) TaxExempSales
                                                FROM V$SALES_INVOICE_REPORT3 WHERE SALES_DATE >= TO_DATE('{10}', 'YYYY-MM-DD') AND SALES_DATE <= TO_DATE('{11}', 'YYYY-MM-DD') 
             and DELETED_FLAG='N'AND EXCISE_AMOUNT <> 0 AND TABLE_NAME IN ('SALES','SALES_RETURN') AND COMPANY_CODE IN(" + companyCode + ")", figureFilter, roundUpFilter, figureFilter, roundUpFilter, figureFilter, roundUpFilter, figureFilter, roundUpFilter, figureFilter, roundUpFilter, filters.FromDate, filters.ToDate);


            if (filters.PartyTypeFilter.Count > 0)
            {
                query = query + string.Format(@" AND PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", filters.PartyTypeFilter).ToString());
            }

            if (filters.BranchFilter.Count > 0)
            {
                query += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            }
            var min = 0;
            var max = 0;
            ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);

            if (!(min == 0 && max == 0))
            {
                query = query + string.Format(@" AND FN_CONVERT_CURRENCY((NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1)),'NRS',SALES_DATE) >={0} AND FN_CONVERT_CURRENCY((NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1)),'NRS',SALES_DATE)<= {1}", min, max);
            }

            query += "ORDER BY BS_DATE(SALES_DATE), INVOICE_NO";

            var salesExciseRegisterList = _objectEntity.SqlQuery<SalesExciseRegisterModel>(query).ToList();
            return salesExciseRegisterList;
        }

        public List<AuditTrailModel> AuditTrailReport(ReportFiltersModel filters, NeoErp.Core.Domain.User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            var companyCode = string.Empty;
            foreach (var company in filters.CompanyFilter)
            {
                companyCode += $@"'{company}',";
            }

            companyCode = companyCode == "" ? $@"'{userinfo.company_code}'" : companyCode.Remove(companyCode.Length - 1);

            string query = string.Format(@"SELECT LOG_ID
	                                                ,LOG_USER
	                                                ,BS_DATE(TRUNC(LOG_DATE)) LOG_DATE
	                                                ,LOG_MESSAGE
                                                FROM LOG_DOC_TEMPLATE WHERE LOG_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND LOG_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                  ", filters.FromDate, filters.ToDate);


            //if (filters.PartyTypeFilter.Count > 0)
            //{
            //    query = query + string.Format(@" AND PARTY_TYPE_CODE IN ('{0}') ", string.Join("','", filters.PartyTypeFilter).ToString());
            //}

            //if (filters.BranchFilter.Count > 0)
            //{
            //    query += string.Format(@" AND BRANCH_CODE IN ('{0}')", string.Join("','", filters.BranchFilter).ToString());
            //}
            //var min = 0;
            //var max = 0;
            //ReportFilterHelper.RangeFilterValue(filters.AmountRangeFilter, out min, out max);

            //if (!(min == 0 && max == 0))
            //{
            //    query = query + string.Format(@" AND FN_CONVERT_CURRENCY((NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1)),'NRS',SALES_DATE) >={0} AND FN_CONVERT_CURRENCY((NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1)),'NRS',SALES_DATE)<= {1}", min, max);
            //}

            query += "ORDER BY LOG_DATE DESC";

            var auditTrailList = _objectEntity.SqlQuery<AuditTrailModel>(query).ToList();
            return auditTrailList;
        }

        public List<ProductWiseSalesSummaryModel> GetProductWiseSalesSummary(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                              : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                 ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                 : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT * FROM (  
                                    SELECT ITEM_CODE, ITEM_EDESC, UNIT  
                                    , SUM(QUANTITY) QUANTITY, SUM(TOTAL_PRICE) / SUM(QUANTITY) UNIT_PRICE, SUM(TOTAL_PRICE) TOTAL_PRICE, SUM(SECOND_QUANTITY) SECOND_QUANTITY, SUM(THIRD_QUANTITY) THIRD_QUANTITY, SUM(FREE_QTY)  FREE_QTY, SUM(ROLL_QTY) ROLL_QTY , SUM(EXCISE_DUTY) EXCISE_DUTY, SUM(EXCISE_DUTYII) EXCISE_DUTYII, SUM(LINE_DISCOUNT) LINE_DISCOUNT, SUM(DISCOUNT) DISCOUNT, SUM(CASH_DISCOUNT) CASH_DISCOUNT, SUM(SPECIAL_DISCOUNT) SPECIAL_DISCOUNT  
                                    , SUM(YEARLY_DISCOUNT) YEARLY_DISCOUNT,SUM(BILL_DISCOUNT) BILL_DISCOUNT, SUM(LUX_TAX) LUX_TAX, SUM(INSURANCE) INSURANCE, SUM(FTD) FTD, SUM(SLET) SLET, SUM(LFT) LFT, SUM(FLT) FLT, SUM(TAXABLE_TOTAL_PRICE) TAXABLE_TOTAL_PRICE , SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE, SUM(INVOICE_TOTAL_PRICE) INVOICE_TOTAL_PRICE FROM (  
                                    SELECT ITEM_CODE, ITEM_EDESC  
                                    , UNIT, QUANTITY, UNIT_PRICE, TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, FREE_QTY, ROLL_QTY, EXCISE_DUTY, EXCISE_DUTYII, LINE_DISCOUNT, DISCOUNT, CASH_DISCOUNT, SPECIAL_DISCOUNT, YEARLY_DISCOUNT, BILL_DISCOUNT, LUX_TAX, INSURANCE, FTD, SLET, LFT, FLT, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) +LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT ) TAXABLE_TOTAL_PRICE  
                                    , CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + LUX_TAX + INSURANCE+ FTD + SLET + LFT + FLT )  * .13,2) ELSE 0 END VAT_TOTAL_PRICE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT  - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT  - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) )  * .13,2) ELSE 0 END ) INVOICE_TOTAL_PRICE  
                                    FROM (  
                                    SELECT A.FORM_CODE, A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI 
                                  , FN_GET_SECOND_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.SALES_NO, A.SERIAL_NO) ORDER_NO, A.SALES_NO ,  
                                    B.PARTY_TYPE_CODE, B.PARTY_TYPE_EDESC,F.CUSTOMER_CODE, F.CUSTOMER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO PAN_NO, C.ITEM_CODE, C.ITEM_EDESC, A.MU_CODE UNIT,  
                                    a.QUANTITY , a.UNIT_PRICE, a.TOTAL_PRICE, A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.FREE_QTY, A.ROLL_QTY  
                                    , CASE WHEN EXCISE_AMOUNT IS NOT NULL AND NVL(EXCISE_AMOUNT,0) > 0 THEN  
                                     NVL(EXCISE_AMOUNT,0)  ELSE  
                                     ROUND((NVL((SELECT SUM(NVL(X.CHARGE_AMOUNT,0)) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTY  
                                    , CASE WHEN EXCISE_AMOUNTII IS NOT NULL AND EXCISE_AMOUNTII > 0 THEN  
                                     NVL(EXCISE_AMOUNTII,0)  ELSE  
                                     ROUND((NVL((SELECT SUM(NVL(X.CHARGE_AMOUNT,0)) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNTII,0)) > 0 THEN 0 ELSE 1 END FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTYII  
                                    , ROUND(NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND ITEM_CODE = A.ITEM_CODE AND SERIAL_NO = A.SERIAL_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG IN ('D','C','Y','B') AND COMPANY_CODE = '01')  AND APPLY_ON IN ('I')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0),2)  LINE_DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2)  DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) CASH_DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) BILL_DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LUX_TAX  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'I' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) INSURANCE  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'W' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FTD  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'X' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SLET  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'R' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LFT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Z' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FLT  
                                    , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01') AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                     NVL(VAT_AMOUNT,0)  ELSE  
                                    ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01')  AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) + SUM(NVL(EXCISE_AMOUNT,0)) + SUM(NVL(EXCISE_AMOUNTII,2))  END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE+NVL(EXCISE_AMOUNT,0)+NVL(EXCISE_AMOUNTII,0)),2) END VAT_TOTAL_PRICE  
                                    ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION  
                                    ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO  
                                    ,E.SALES_TYPE_EDESC SALES_TYPE, FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS  
                                    ,G.ITEM_SIZE UPC, G.TYPE CATEGORY, G.GRADE SHELF_LIFE,  G.SIZE_LENGHT WEIGHT, G.THICKNESS UOM, G.REMARKS STATUS, (SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE = A.EMPLOYEE_CODE AND COMPANY_CODE = A.COMPANY_CODE) EMPLOYEE_EDESC, (SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL, g.BRAND_NAME  
                                    FROM SA_SALES_INVOICE A, IP_PARTY_TYPE_CODE B, IP_ITEM_MASTER_SETUP C, SHIPPING_TRANSACTION D, SA_SALES_TYPE E, SA_CUSTOMER_SETUP F, IP_ITEM_SPEC_SETUP G  
                                    WHERE A.PARTY_TYPE_CODE = B.PARTY_TYPE_CODE (+)  
                                    AND A.COMPANY_CODE = B.COMPANY_CODE (+)  
                                    AND A.ITEM_CODE = C.ITEM_CODE  
                                    AND A.COMPANY_CODE = C.COMPANY_CODE  
                                    AND A.CUSTOMER_CODE = F.CUSTOMER_CODE  
                                    AND A.COMPANY_CODE = F.COMPANY_CODE  
                                    AND A.SALES_NO = D.VOUCHER_NO (+)  
                                    AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                    AND A.SALES_TYPE_CODE = E.SALES_TYPE_CODE (+)  
                                    AND A.COMPANY_CODE = E.COMPANY_CODE (+)  
                                    AND C.ITEM_CODE = G.ITEM_CODE (+)  
                                    AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                    AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                    AND A.COMPANY_CODE IN ({2})
                                    AND A.BRANCH_CODE IN ({3})
                                    AND A.DELETED_FLAG = 'N'  
                                    ORDER BY SALES_NO, A.SERIAL_NO  
                                    )  
                                    )  
                                    GROUP BY ITEM_CODE, ITEM_EDESC, UNIT  
                                    ) ORDER BY ITEM_EDESC",
                                    filters.FromDate,
                                    filters.ToDate,
                                    companyCode,
                                    branchCode);
            var productSales = _objectEntity.SqlQuery<ProductWiseSalesSummaryModel>(query).ToList();
            return productSales;
        }


        public List<DateWiseSalesDetailsModel> GetDateWiseSalesDetails(ReportFiltersModel filters, User userinfo)
        {
            filters.FromDate = "2024-Jul-16";
            filters.ToDate = "2025-Jul-15";
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT SALES_DATE, MITI, ORDER_NO, SALES_NO, PARTY_TYPE_CODE, PARTY_TYPE_EDESC, CUSTOMER_CODE, CUSTOMER_EDESC, ADDRESS, PAN_NO, ITEM_CODE, ITEM_EDESC  
                                , UNIT, QUANTITY, UNIT_PRICE, TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, FREE_QTY, ROLL_QTY, EXCISE_DUTY, EXCISE_DUTYII, DIS_PER, LINE_DISCOUNT , DISCOUNT, CASH_DISCOUNT, SPECIAL_DISCOUNT, YEARLY_DISCOUNT, BILL_DISCOUNT, LUX_TAX, INSURANCE, FTD, SLET, LFT, FLT, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) +LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT ) TAXABLE_TOTAL_PRICE  
                                , CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT )  * .13,2) ELSE 0 END VAT_TOTAL_PRICE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT  - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT  - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) )  * .13,2) ELSE 0 END ) INVOICE_TOTAL_PRICE, VEHICLE_NO, DESTINATION  
                                ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, SALES_TYPE, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , UPC, CATEGORY, SHELF_LIFE,  WEIGHT, UOM, STATUS, EMPLOYEE_EDESC, FISCAL, FORM_CODE, BRAND_NAME , PAYMENT_MODE, PRIORITY_CODE   FROM (  
                                SELECT A.FORM_CODE, A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI , FN_GET_SECOND_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.SALES_NO, A.SERIAL_NO) ORDER_NO, A.SALES_NO ,  
                                B.PARTY_TYPE_CODE, B.PARTY_TYPE_EDESC,F.CUSTOMER_CODE, F.CUSTOMER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO PAN_NO, C.ITEM_CODE, C.ITEM_EDESC, A.MU_CODE UNIT,  
                                a.QUANTITY , a.UNIT_PRICE, a.TOTAL_PRICE, A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.ROLL_QTY, A.FREE_QTY  
                                , CASE WHEN EXCISE_AMOUNT IS NOT NULL AND NVL(EXCISE_AMOUNT,0) > 0 THEN  
                                 NVL(EXCISE_AMOUNT,0)  ELSE  
                                 ROUND((NVL((SELECT SUM(NVL(X.CHARGE_AMOUNT,0)) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTY  
                                , CASE WHEN EXCISE_AMOUNTII IS NOT NULL AND EXCISE_AMOUNTII > 0 THEN  
                                 NVL(EXCISE_AMOUNTII,0)  ELSE  
                                 ROUND((NVL((SELECT SUM(NVL(X.CHARGE_AMOUNT,0)) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNTII,0)) > 0 THEN 0 ELSE 1 END FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTYII  
                                , ROUND(NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND ITEM_CODE = A.ITEM_CODE AND SERIAL_NO = A.SERIAL_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG IN ('D','C','Y','B') AND COMPANY_CODE = '01')  AND APPLY_ON IN ('I')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0),2)  LINE_DISCOUNT  
                                ,(SELECT MANUAL_CALC_VALUE FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE AND ROWNUM  = 1) DIS_PER  
                                , CASE WHEN DISCOUNT_AMOUNT IS NOT NULL AND DISCOUNT_AMOUNT > 0  
                                 AND ROUND(NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND ITEM_CODE = A.ITEM_CODE AND SERIAL_NO = A.SERIAL_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG IN ('D','C','Y','B') AND COMPANY_CODE = '01')  AND APPLY_ON IN ('I')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0),2) = 0 THEN  
                                 NVL(DISCOUNT_AMOUNT,0)  ELSE  
                                 ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2)  END DISCOUNT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) CASH_DISCOUNT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) BILL_DISCOUNT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LUX_TAX  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'I' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) INSURANCE  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'W' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FTD  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'X' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SLET  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'R' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LFT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Z' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FLT  
                                , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01') AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                 NVL(VAT_AMOUNT,0)  ELSE  
                                ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01')  AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE  
                                ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION  
                                ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO  
                                ,E.SALES_TYPE_EDESC SALES_TYPE, FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS  
                                ,G.ITEM_SIZE UPC, G.TYPE CATEGORY, G.GRADE SHELF_LIFE,  G.SIZE_LENGHT WEIGHT, G.THICKNESS UOM, G.REMARKS STATUS, (SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE = A.EMPLOYEE_CODE AND COMPANY_CODE = A.COMPANY_CODE) EMPLOYEE_EDESC, (SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL, g.BRAND_NAME, A.PAYMENT_MODE, (SELECT PRIORITY_EDESC FROM IP_PRIORITY_CODE WHERE PRIORITY_CODE = A.PRIORITY_CODE AND COMPANY_CODE = A.COMPANY_CODE)  PRIORITY_CODE  
                                FROM SA_SALES_INVOICE A, IP_PARTY_TYPE_CODE B, IP_ITEM_MASTER_SETUP C, SHIPPING_TRANSACTION D, SA_SALES_TYPE E, SA_CUSTOMER_SETUP F, IP_ITEM_SPEC_SETUP G  
                                WHERE A.PARTY_TYPE_CODE = B.PARTY_TYPE_CODE (+)  
                                AND A.COMPANY_CODE = B.COMPANY_CODE (+)  
                                AND A.ITEM_CODE = C.ITEM_CODE  
                                AND A.COMPANY_CODE = C.COMPANY_CODE  
                                AND A.CUSTOMER_CODE = F.CUSTOMER_CODE  
                                AND A.COMPANY_CODE = F.COMPANY_CODE  
                                AND A.SALES_NO = D.VOUCHER_NO (+)  
                                AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                AND A.SALES_TYPE_CODE = E.SALES_TYPE_CODE (+)  
                                AND A.COMPANY_CODE = E.COMPANY_CODE (+)  
                                AND C.ITEM_CODE = G.ITEM_CODE (+)  
                                AND C.COMPANY_CODE = G.COMPANY_CODE (+)  
                                AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                AND A.COMPANY_CODE IN ({2})
                                AND A.BRANCH_CODE IN ({3})
                                AND A.DELETED_FLAG = 'N'  
                                ORDER BY SALES_NO, A.SERIAL_NO  
                                )  
                                ORDER BY SALES_NO",
                                            filters.FromDate,
                                            filters.ToDate,
                                            companyCode,
                                            branchCode);
            var dateWiswSales = _objectEntity.SqlQuery<DateWiseSalesDetailsModel>(query).ToList();
            return dateWiswSales;


        }

        public List<DateWiseSalesReturnDetailsModel> GetDateWiseSalesReturnDetails(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT RETURN_DATE, MITI, ORDER_NO, RETURN_NO, PARTY_TYPE_CODE, PARTY_TYPE_EDESC, CUSTOMER_CODE, CUSTOMER_EDESC, ADDRESS, PAN_NO, ITEM_CODE, ITEM_EDESC  
                                            , UNIT, QUANTITY, UNIT_PRICE, TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, FREE_QTY, ROLL_QTY, EXCISE_DUTY, EXCISE_DUTYII, LINE_DISCOUNT, DIS_PER, DISCOUNT, CASH_DISCOUNT, SPECIAL_DISCOUNT, YEARLY_DISCOUNT, BILL_DISCOUNT, LUX_TAX, INSURANCE, FTD, SLET, LFT, FLT, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) +LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT ) TAXABLE_TOTAL_PRICE  
                                            , CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT )  * .13,2) ELSE 0 END VAT_TOTAL_PRICE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT  - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT  - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) )  * .13,2) ELSE 0 END ) INVOICE_TOTAL_PRICE, VEHICLE_NO, DESTINATION  
                                            ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, SALES_TYPE, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , UPC, CATEGORY, SHELF_LIFE,  WEIGHT, UOM, STATUS, EMPLOYEE_EDESC, FISCAL, FORM_CODE, BRAND_NAME , PAYMENT_MODE   FROM (  
                                            SELECT A.FORM_CODE, A.RETURN_DATE, BS_DATE(A.RETURN_DATE) MITI , FN_GET_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.RETURN_NO, A.SERIAL_NO) ORDER_NO, A.RETURN_NO ,  
                                            B.PARTY_TYPE_CODE, B.PARTY_TYPE_EDESC,F.CUSTOMER_CODE, F.CUSTOMER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO PAN_NO, C.ITEM_CODE, C.ITEM_EDESC, A.MU_CODE UNIT,  
                                            a.QUANTITY , a.UNIT_PRICE, a.TOTAL_PRICE, A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.ROLL_QTY, A.FREE_QTY  
                                            , CASE WHEN EXCISE_AMOUNT IS NOT NULL AND NVL(EXCISE_AMOUNT,0) > 0 THEN  
                                             NVL(EXCISE_AMOUNT,0)  ELSE  
                                             ROUND((NVL((SELECT SUM(NVL(X.CHARGE_AMOUNT,0)) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTY  
                                            , CASE WHEN EXCISE_AMOUNTII IS NOT NULL AND EXCISE_AMOUNTII > 0 THEN  
                                             NVL(EXCISE_AMOUNTII,0)  ELSE  
                                             ROUND((NVL((SELECT SUM(NVL(X.CHARGE_AMOUNT,0)) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNTII,0)) > 0 THEN 0 ELSE 1 END FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTYII  
                                            , ROUND(NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND ITEM_CODE = A.ITEM_CODE AND SERIAL_NO = A.SERIAL_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG IN ('D','C','Y','B') AND COMPANY_CODE = '01')  AND APPLY_ON IN ('I')  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0),2)  LINE_DISCOUNT  
                                            ,(SELECT MANUAL_CALC_VALUE FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE AND ROWNUM  = 1) DIS_PER  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2)  DISCOUNT  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) CASH_DISCOUNT  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) BILL_DISCOUNT  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LUX_TAX  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'I' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) INSURANCE  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'W' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FTD  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'X' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SLET  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'R' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LFT  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Z' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FLT  
                                            , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01') AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                             NVL(VAT_AMOUNT,0)  ELSE  
                                            ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01')  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE)+ sum(NVL(EXCISE_AMOUNT,0)) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE+NVL(EXCISE_AMOUNT,0)),2) END VAT_TOTAL_PRICE  
                                            ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION  
                                            ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO  
                                            ,E.SALES_TYPE_EDESC SALES_TYPE, FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS  
                                            ,G.ITEM_SIZE UPC, G.TYPE CATEGORY, G.GRADE SHELF_LIFE,  G.SIZE_LENGHT WEIGHT, G.THICKNESS UOM, G.REMARKS STATUS, (SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE = A.EMPLOYEE_CODE AND COMPANY_CODE = A.COMPANY_CODE) EMPLOYEE_EDESC, (SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL, g.BRAND_NAME, A.PAYMENT_MODE  
                                            FROM SA_SALES_RETURN A, IP_PARTY_TYPE_CODE B, IP_ITEM_MASTER_SETUP C, SHIPPING_TRANSACTION D, SA_SALES_TYPE E, SA_CUSTOMER_SETUP F, IP_ITEM_SPEC_SETUP G  
                                            WHERE A.PARTY_TYPE_CODE = B.PARTY_TYPE_CODE (+)  
                                            AND A.COMPANY_CODE = B.COMPANY_CODE (+)  
                                            AND A.ITEM_CODE = C.ITEM_CODE  
                                            AND A.COMPANY_CODE = C.COMPANY_CODE  
                                            AND A.CUSTOMER_CODE = F.CUSTOMER_CODE  
                                            AND A.COMPANY_CODE = F.COMPANY_CODE  
                                            AND A.RETURN_NO = D.VOUCHER_NO (+)  
                                            AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                            AND A.SALES_TYPE_CODE = E.SALES_TYPE_CODE (+)  
                                            AND A.COMPANY_CODE = E.COMPANY_CODE (+)  
                                            AND C.ITEM_CODE = G.ITEM_CODE (+)  
                                            AND C.COMPANY_CODE = G.COMPANY_CODE (+)  
                                            AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})
                                            AND A.DELETED_FLAG = 'N'  
                                            ORDER BY RETURN_NO, A.SERIAL_NO  
                                            )  
                                            ORDER BY RETURN_NO  
                                            ",
                                          filters.FromDate,
                                          filters.ToDate,
                                          companyCode,
                                          branchCode);
            var dateWiswSales = _objectEntity.SqlQuery<DateWiseSalesReturnDetailsModel>(query).ToList();
            return dateWiswSales;

        }

        public List<BillwiseSalesSummaryModel> GetBillwiseSalesSummary(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT SALES_DATE, MITI, ORDER_NO, SALES_NO, MANUAL_NO, PARTY_TYPE_CODE, PARTY_TYPE_EDESC, CUSTOMER_CODE, CUSTOMER_EDESC, ADDRESS, PAN_NO, DIS_PER  
                                    , SUM(QUANTITY) QUANTITY, SUM(NVL(SECOND_QUANTITY,0)) SECOND_QUANTITY, SUM(NVL(THIRD_QUANTITY,0)) THIRD_QUANTITY, SUM(NVL(FREE_QTY,0)) FREE_QTY, SUM(NVL(ROLL_QTY,0))ROLL_QTY,  SUM(TOTAL_PRICE) TOTAL_PRICE, SUM(EXCISE_DUTY) EXCISE_DUTY , SUM(EXCISE_DUTYII) EXCISE_DUTYII , SUM(LINE_DISCOUNT) LINE_DISCOUNT, SUM(DISCOUNT) DISCOUNT, SUM(CASH_DISCOUNT) CASH_DISCOUNT, SUM(SPECIAL_DISCOUNT) SPECIAL_DISCOUNT, SUM(YEARLY_DISCOUNT) YEARLY_DISCOUNT, SUM(BILL_DISCOUNT) BILL_DISCOUNT, SUM(LUX_TAX) LUX_TAX, SUM(INSURANCE) INSURANCE,  SUM(FTD) FTD , SUM(SLET) SLET, SUM(LFT) LFT, SUM(FLT) FLT,  (SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) - (SUM(LINE_DISCOUNT) + SUM(DISCOUNT) + SUM(CASH_DISCOUNT) + SUM(SPECIAL_DISCOUNT) + SUM(YEARLY_DISCOUNT) + SUM(BILL_DISCOUNT)) + SUM(LUX_TAX ) +SUM(INSURANCE) + SUM(FTD) + SUM(SLET) + SUM(LFT) + SUM(FLT)) TAXABLE_TOTAL_PRICE  
                                    , SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE ,(SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) + SUM(LUX_TAX) + SUM(INSURANCE) + SUM(FTD) + SUM(SLET) + SUM(LFT) + SUM(FLT) - (SUM(LINE_DISCOUNT) + SUM(DISCOUNT) + SUM(CASH_DISCOUNT) + SUM(SPECIAL_DISCOUNT) + SUM(YEARLY_DISCOUNT) + SUM(BILL_DISCOUNT)) + SUM(VAT_TOTAL_PRICE)) INVOICE_TOTAL_PRICE , VEHICLE_NO, DESTINATION  
                                    ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, SALES_TYPE, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , EMPLOYEE_EDESC, FISCAL, FORM_CODE, PAYMENT_MODE, PRIORITY_CODE FROM (  
                                    SELECT A.FORM_CODE, A.MANUAL_NO, A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI , FN_GET_SECOND_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.SALES_NO, 1) ORDER_NO, A.SALES_NO ,  
                                    B.PARTY_TYPE_CODE, B.PARTY_TYPE_EDESC,F.CUSTOMER_CODE, F.CUSTOMER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO PAN_NO,  
                                    a.QUANTITY , a.TOTAL_PRICE, A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.FREE_QTY, A.ROLL_QTY  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) EXCISE_DUTY  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) EXCISE_DUTYII  
                                    , ROUND(NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND ITEM_CODE = A.ITEM_CODE AND SERIAL_NO = A.SERIAL_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG IN ('D','C','Y','B') AND COMPANY_CODE = '01')  AND APPLY_ON IN ('I')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0),2)  LINE_DISCOUNT  
                                    ,(SELECT MANUAL_CALC_VALUE FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE AND ROWNUM  = 1) DIS_PER  
                                    , ROUND((NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE = '01')  AND APPLY_ON IN ('D')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2)  DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) CASH_DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) BILL_DISCOUNT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LUX_TAX  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'I' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) INSURANCE  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'W' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FTD  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'X' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SLET  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'R' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LFT  
                                    ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Z' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FLT  
                                    , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01') AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                     NVL(VAT_AMOUNT,0)  ELSE  
                                    ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                    WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                    AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE  
                                    ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION  
                                    ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO  
                                    ,E.SALES_TYPE_EDESC SALES_TYPE, FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS  
                                    ,(SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE = A.EMPLOYEE_CODE AND COMPANY_CODE = A.COMPANY_CODE) EMPLOYEE_EDESC, (SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL, A.PAYMENT_MODE, (SELECT PRIORITY_EDESC FROM IP_PRIORITY_CODE WHERE PRIORITY_CODE = A.PRIORITY_CODE AND COMPANY_CODE = A.COMPANY_CODE)  PRIORITY_CODE    
                                    FROM SA_SALES_INVOICE A, IP_PARTY_TYPE_CODE B, SHIPPING_TRANSACTION D, SA_SALES_TYPE E, SA_CUSTOMER_SETUP F    
                                    WHERE A.PARTY_TYPE_CODE = B.PARTY_TYPE_CODE (+)  
                                    AND A.COMPANY_CODE = B.COMPANY_CODE (+)  
                                    AND A.CUSTOMER_CODE = F.CUSTOMER_CODE  
                                    AND A.COMPANY_CODE = F.COMPANY_CODE  
                                    AND A.SALES_NO = D.VOUCHER_NO (+)  
                                    AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                    AND A.SALES_TYPE_CODE = E.SALES_TYPE_CODE (+)  
                                    AND A.COMPANY_CODE = E.COMPANY_CODE (+)  
                                    AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                    AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                    AND A.COMPANY_CODE IN ({2})
                                    AND A.BRANCH_CODE IN ({3})
                                    AND A.DELETED_FLAG = 'N'  
                                    ORDER BY SALES_NO, A.SERIAL_NO  
                                    )  
                                    GROUP BY SALES_DATE, MITI, ORDER_NO, SALES_NO, MANUAL_NO, PARTY_TYPE_CODE, PARTY_TYPE_EDESC,
                                     CUSTOMER_CODE, CUSTOMER_EDESC, ADDRESS, PAN_NO, VEHICLE_NO, DESTINATION, DRIVER_NAME, 
                                     DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, 
                                     GATE_ENTRY_NO, LOADING_SLIP_NO, SALES_TYPE, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, 
                                     SHIPPING_TERMS , EMPLOYEE_EDESC, FISCAL, FORM_CODE, PAYMENT_MODE, DIS_PER,
                                     PRIORITY_CODE  ORDER BY SALES_NO",
                                                                       filters.FromDate,
                                                                       filters.ToDate,
                                                                       companyCode,
                                                                       branchCode);
            var billwiseSummary = _objectEntity.SqlQuery<BillwiseSalesSummaryModel>(query).ToList();
            return billwiseSummary;
        }

        public List<BillwiseSalesReturnSummaryModel> GetBillwiseSalesReturnSummary(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();

            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";

            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";

            string query = string.Format(@"SELECT RETURN_DATE, MITI, ORDER_NO, RETURN_NO, MANUAL_NO, PARTY_TYPE_CODE, PARTY_TYPE_EDESC, CUSTOMER_CODE, CUSTOMER_EDESC, ADDRESS, PAN_NO  
                                        , SUM(QUANTITY) QUANTITY,  SUM(NVL(SECOND_QUANTITY,0)) SECOND_QUANTITY, SUM(NVL(THIRD_QUANTITY,0)) THIRD_QUANTITY, SUM(NVL(FREE_QTY,0)) FREE_QTY, SUM(NVL(ROLL_QTY,0)) ROLL_QTY, SUM(TOTAL_PRICE) TOTAL_PRICE, SUM(EXCISE_DUTY) EXCISE_DUTY , SUM(EXCISE_DUTYII) EXCISE_DUTYII, SUM(LINE_DISCOUNT) LINE_DISCOUNT ,SUM(DISCOUNT) DISCOUNT, SUM(CASH_DISCOUNT) CASH_DISCOUNT, SUM(SPECIAL_DISCOUNT) SPECIAL_DISCOUNT, SUM(YEARLY_DISCOUNT) YEARLY_DISCOUNT, SUM(BILL_DISCOUNT) BILL_DISCOUNT, SUM(LUX_TAX) LUX_TAX , SUM(INSURANCE) INSURANCE,  SUM(FTD) FTD , SUM(SLET) SLET, SUM(LFT) LFT, SUM(FLT) FLT,(SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) - (SUM(LINE_DISCOUNT) + SUM(DISCOUNT) + SUM(CASH_DISCOUNT) + SUM(SPECIAL_DISCOUNT) + SUM(YEARLY_DISCOUNT) + SUM(BILL_DISCOUNT)) + SUM(LUX_TAX) + SUM(INSURANCE) + SUM(FTD) + SUM(SLET) + SUM(LFT) + SUM(FLT) ) TAXABLE_TOTAL_PRICE  
                                        , SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE ,(SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) + SUM(LUX_TAX) + SUM(INSURANCE) + SUM(FTD) + SUM(SLET) + SUM(LFT) + SUM(FLT) - (SUM(LINE_DISCOUNT) + SUM(DISCOUNT) + SUM(CASH_DISCOUNT) + SUM(SPECIAL_DISCOUNT) + SUM(YEARLY_DISCOUNT) + SUM(BILL_DISCOUNT)) + SUM(VAT_TOTAL_PRICE)) INVOICE_TOTAL_PRICE , VEHICLE_NO, DESTINATION  
                                        ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, SALES_TYPE, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , EMPLOYEE_EDESC, FISCAL, FORM_CODE FROM (  
                                        SELECT A.FORM_CODE, A.MANUAL_NO, A.RETURN_DATE, BS_DATE(A.RETURN_DATE) MITI , FN_GET_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.RETURN_NO, 1) ORDER_NO, A.RETURN_NO ,  
                                        B.PARTY_TYPE_CODE, B.PARTY_TYPE_EDESC,F.CUSTOMER_CODE, F.CUSTOMER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO PAN_NO,  
                                        a.QUANTITY , a.TOTAL_PRICE, A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.FREE_QTY,  A.ROLL_QTY  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) EXCISE_DUTY  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) EXCISE_DUTYII  
                                        , ROUND(NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND ITEM_CODE = A.ITEM_CODE AND SERIAL_NO = A.SERIAL_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG IN ('D','C','Y','B') AND COMPANY_CODE = '01')  AND APPLY_ON IN ('I')  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0),2)  LINE_DISCOUNT  
                                        , ROUND((NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE = '01')  AND APPLY_ON IN ('D')  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2)  DISCOUNT  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) CASH_DISCOUNT  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE = '01') AND APPLY_ON IN ('D')  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) BILL_DISCOUNT  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LUX_TAX  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'I' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) INSURANCE  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'W' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FTD  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'X' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SLET  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'R' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LFT  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Z' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FLT  
                                        , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01') AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                         NVL(VAT_AMOUNT,0)  ELSE  
                                        ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01') AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE  
                                        ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION  
                                        ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO  
                                        ,E.SALES_TYPE_EDESC SALES_TYPE, FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS  
                                        ,(SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE = A.EMPLOYEE_CODE AND COMPANY_CODE = A.COMPANY_CODE) EMPLOYEE_EDESC, (SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL   
                                        FROM SA_SALES_RETURN A, IP_PARTY_TYPE_CODE B, SHIPPING_TRANSACTION D, SA_SALES_TYPE E, SA_CUSTOMER_SETUP F    
                                        WHERE A.PARTY_TYPE_CODE = B.PARTY_TYPE_CODE (+)  
                                        AND A.COMPANY_CODE = B.COMPANY_CODE (+)  
                                        AND A.CUSTOMER_CODE = F.CUSTOMER_CODE  
                                        AND A.COMPANY_CODE = F.COMPANY_CODE  
                                        AND A.RETURN_NO = D.VOUCHER_NO (+)  
                                        AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                        AND A.SALES_TYPE_CODE = E.SALES_TYPE_CODE (+)  
                                        AND A.COMPANY_CODE = E.COMPANY_CODE (+)  
                                        AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                        AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                        AND A.COMPANY_CODE IN ({2})
                                        AND A.BRANCH_CODE IN ({3})
                                        AND A.DELETED_FLAG = 'N'  
                                        ORDER BY RETURN_NO, A.SERIAL_NO  
                                        )  
                                        GROUP BY RETURN_DATE, MITI, ORDER_NO, RETURN_NO, MANUAL_NO, PARTY_TYPE_CODE, PARTY_TYPE_EDESC,
                                         CUSTOMER_CODE, CUSTOMER_EDESC, ADDRESS, PAN_NO, VEHICLE_NO, DESTINATION, DRIVER_NAME, DRIVER_MOBILE_NO, 
                                         DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, SALES_TYPE, 
                                         TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , EMPLOYEE_EDESC, FISCAL, FORM_CODE  ORDER BY RETURN_NO",
                                                                                                              filters.FromDate,
                                                                                                              filters.ToDate,
                                                                                                              companyCode,
                                                                                                              branchCode);
            var billwiseReturn = _objectEntity.SqlQuery<BillwiseSalesReturnSummaryModel>(query).ToList();
            return billwiseReturn;



        }

        public dynamic GetMonthwiseCustomerProductNetSales(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT B.CUSTOMER_CODE, B.CUSTOMER_EDESC, B.MASTER_CUSTOMER_CODE , B.PRE_CUSTOMER_CODE , B.GROUP_SKU_FLAG, (LENGTH(B.MASTER_CUSTOMER_CODE) - LENGTH(REPLACE(B.MASTER_CUSTOMER_CODE,'.',''))) ROWLEV,  A.* FROM (  
                                            SELECT * FROM (  
                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE CUS_CODE, ITEM_CODE, ITEM_EDESC, INDEX_MU_CODE, MTH,   
                                            SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE FROM (   
                                            SELECT MTH,A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, D.ITEM_EDESC,  D.INDEX_MU_CODE INDEX_MU_CODE, CASE WHEN E.BRAND_NAME IS NULL THEN D.ITEM_EDESC ELSE E.BRAND_NAME END BRAND_NAME,  SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE  FROM (   
                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE,  A.ITEM_CODE, SUBSTR(BS_DATE(SALES_DATE),6,2) MTH, SUM(NVL(A.QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE   FROM SA_SALES_INVOICE A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.CUSTOMER_CODE,  A.ITEM_CODE , A.COMPANY_CODE, SALES_DATE  
                                            Union All   
                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(RETURN_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY,0))  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE   FROM SA_SALES_RETURN A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3}) 
                                            GROUP BY  A.CUSTOMER_CODE, A.ITEM_CODE , A.COMPANY_CODE, RETURN_DATE  
                                            Union All   
                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE  FROM SA_DEBIT_NOTE A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.CUSTOMER_CODE, A.ITEM_CODE , A.COMPANY_CODE, VOUCHER_DATE  
                                            Union All   
                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE  FROM SA_CREDIT_NOTE A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.CUSTOMER_CODE, A.ITEM_CODE , A.COMPANY_CODE, VOUCHER_DATE  
                                            ORDER BY CUSTOMER_CODE) A,  SA_CUSTOMER_SETUP B ,  IP_ITEM_MASTER_SETUP D, IP_ITEM_SPEC_SETUP E   
                                            WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE   
                                            AND  A.ITEM_CODE = D.ITEM_CODE   
                                            AND A.COMPANY_CODE = D.COMPANY_CODE   
                                            AND A.COMPANY_CODE = B.COMPANY_CODE   
                                            AND D.GROUP_SKU_FLAG = 'I'  
                                            AND D.ITEM_CODE = E.ITEM_CODE (+)  
                                            AND D.COMPANY_CODE = E.COMPANY_CODE (+)  
                                            AND A.COMPANY_CODE IN ({2})
                                            GROUP BY   A.CUSTOMER_CODE,  A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE, E.BRAND_NAME, A.COMPANY_CODE  , MTH  
                                            ) A ORDER BY CUSTOMER_CODE, ITEM_EDESC  
                                            )  
                                         PIVOT  
                                            (SUM(NET_SALES_QTY) QTY, SUM(NET_SALES_VALUE) VALUE  
                                            FOR MTH IN ('04' AS S,
                                                '05' AS B,
                                                '06' AS A,
                                                '07' AS K,
                                                '08' AS M,
                                                '09' AS P,
                                                '10' AS Mg,
                                                '11' AS F,
                                                '12' AS C,
                                                '01' AS Bh,
                                                '02' AS J,
                                                '03' AS Aa)  
                                            )  
                                            ) 

                                            A, SA_CUSTOMER_SETUP B  
                                            WHERE  B.CUSTOMER_CODE = A.CUS_CODE (+)   
                                            AND B.COMPANY_CODE = A.COMPANY_CODE (+)   
                                            
                                            AND B.COMPANY_CODE IN ({2}) 
                                            ORDER BY B.MASTER_CUSTOMER_CODE, B.PRE_CUSTOMER_CODE", FromDate, ToDate, companyCode, branchCode);
            var monthwise = _objectEntity.SqlQuery<MonthwiseCustProdNetSalesModel>(query).ToList();
            var groupedData = monthwise;

            foreach (var item in groupedData)
            {
                if (item.MASTER_CUSTOMER_CODE == "01" || item.MASTER_CUSTOMER_CODE == "02")
                {
                    item.PRE_CUSTOMER_CODE = "";
                }

            }
            // return groupedData;
            List<dynamic> stockData = new List<dynamic>();
            var itemRows = groupedData.Where(r => r.GROUP_SKU_FLAG == "I").ToList();

            foreach (var row in groupedData)
            {
                double total1 = 0;  // S_QTY
                double total2 = 0;  // S_VALUE
                double total3 = 0;  // B_QTY
                double total4 = 0;  // B_VALUE
                double total5 = 0;  // A_QTY
                double total6 = 0;  // A_VALUE
                double total7 = 0;  // K_QTY
                double total8 = 0;  // K_VALUE
                double total9 = 0;  // M_QTY
                double total10 = 0; // M_VALUE
                double total11 = 0; // P_QTY
                double total12 = 0; // P_VALUE
                double total13 = 0; // Mg_QTY
                double total14 = 0; // Mg_VALUE
                double total15 = 0; // F_QTY
                double total16 = 0; // F_VALUE
                double total17 = 0; // C_QTY
                double total18 = 0; // C_VALUE
                double total19 = 0; // Bh_QTY
                double total20 = 0; // Bh_VALUE
                double total21 = 0; // J_VALUE
                double total22 = 0; // J_VALUE
                double total23 = 0; // Aa_VALUE
                double total24 = 0; // Aa_VALUE

                if (row.GROUP_SKU_FLAG == "G")
                {
                    string masterCode = row.MASTER_CUSTOMER_CODE;
                    foreach (var z in itemRows)
                    {
                        if (!string.IsNullOrEmpty(z.MASTER_CUSTOMER_CODE) &&
                            z.MASTER_CUSTOMER_CODE.StartsWith(masterCode))
                        {
                            total1 += z.S_QTY ?? 0;
                            total2 += z.S_VALUE ?? 0;
                            total3 += z.B_QTY ?? 0;
                            total4 += z.B_VALUE ?? 0;
                            total5 += z.A_QTY ?? 0;
                            total6 += z.A_VALUE ?? 0;
                            total7 += z.K_QTY ?? 0;
                            total8 += z.K_VALUE ?? 0;
                            total9 += z.M_QTY ?? 0;
                            total10 += z.M_VALUE ?? 0;
                            total11 += z.P_QTY ?? 0;
                            total12 += z.P_VALUE ?? 0;
                            total13 += z.Mg_QTY ?? 0;
                            total14 += z.Mg_VALUE ?? 0;
                            total15 += z.F_QTY ?? 0;
                            total16 += z.F_VALUE ?? 0;
                            total17 += z.C_QTY ?? 0;
                            total18 += z.C_VALUE ?? 0;
                            total19 += z.Bh_QTY ?? 0;
                            total20 += z.Bh_VALUE ?? 0;
                            total21 += z.J_QTY ?? 0;
                            total22 += z.J_VALUE ?? 0;
                            total23 += z.Aa_QTY ?? 0;
                            total24 += z.Aa_VALUE ?? 0;
                        }
                    }
                }
                else
                {
                    total1 = row.S_QTY ?? 0;
                    total2 = row.S_VALUE ?? 0;
                    total3 = row.B_QTY ?? 0;
                    total4 = row.B_VALUE ?? 0;
                    total5 = row.A_QTY ?? 0;
                    total6 = row.A_VALUE ?? 0;
                    total7 = row.K_QTY ?? 0;
                    total8 = row.K_VALUE ?? 0;
                    total9 = row.M_QTY ?? 0;
                    total10 = row.M_VALUE ?? 0;
                    total11 = row.P_QTY ?? 0;
                    total12 = row.P_VALUE ?? 0;
                    total13 = row.Mg_QTY ?? 0;
                    total14 = row.Mg_VALUE ?? 0;
                    total15 = row.F_QTY ?? 0;
                    total16 = row.F_VALUE ?? 0;
                    total17 = row.C_QTY ?? 0;
                    total18 = row.C_VALUE ?? 0;
                    total19 = row.Bh_QTY ?? 0;
                    total20 = row.Bh_VALUE ?? 0;
                    total21 = row.J_QTY ?? 0;
                    total22 = row.J_VALUE ?? 0;
                    total23 = row.Aa_QTY ?? 0;
                    total24 = row.Aa_VALUE ?? 0;
                }

                // Add only if any total is non-zero
                if (
                    total1 != 0 || total2 != 0 || total3 != 0 || total4 != 0 ||
                    total5 != 0 || total6 != 0 || total7 != 0 || total8 != 0 ||
                    total9 != 0 || total10 != 0 || total11 != 0 || total12 != 0 ||
                    total13 != 0 || total14 != 0 || total15 != 0 || total16 != 0 ||
                    total17 != 0 || total18 != 0 || total19 != 0 || total20 != 0 ||
                    total21 != 0 || total22 != 0 || total23 != 0 || total24 != 0
                )
                {
                    stockData.Add(new
                    {
                        CUSTOMER_CODE = row.CUSTOMER_CODE,
                        CUS_CODE = row.CUS_CODE,
                        CUSTOMER_EDESC = row.CUSTOMER_EDESC,
                        ITEM_CODE = row.ITEM_CODE,
                        ITEM_EDESC = row.ITEM_EDESC,
                        INDEX_MU_CODE = row.INDEX_MU_CODE,
                        GROUP_SKU_FLAG = row.GROUP_SKU_FLAG,
                        MASTER_CUSTOMER_CODE = row.MASTER_CUSTOMER_CODE,
                        PRE_CUSTOMER_CODE = row.PRE_CUSTOMER_CODE,

                        S_QTY = total1,
                        S_VALUE = total2,
                        B_QTY = total3,
                        B_VALUE = total4,
                        A_QTY = total5,
                        A_VALUE = total6,
                        K_QTY = total7,
                        K_VALUE = total8,
                        M_QTY = total9,
                        M_VALUE = total10,
                        P_QTY = total11,
                        P_VALUE = total12,
                        Mg_QTY = total13,
                        Mg_VALUE = total14,
                        F_QTY = total15,
                        F_VALUE = total16,
                        C_QTY = total17,
                        C_VALUE = total18,
                        Bh_QTY = total19,
                        Bh_VALUE = total20,
                        J_QTY = total21,
                        J_VALUE = total22,
                        Aa_QTY = total23,
                        Aa_VALUE = total24
                    });
                }
            }

            //// ────────────────────────────────────────────────────────────────────────────
            //// 3. Grand Total row
            //// ────────────────────────────────────────────────────────────────────────────
            if (stockData.Any())
            {
                var grand = new
                {
                    CUSTOMER_CODE = "",
                    CUS_CODE = "",
                    CUSTOMER_EDESC = "Grand Total",
                    ITEM_CODE = "",
                    ITEM_EDESC = "",
                    INDEX_MU_CODE = "",
                    MASTER_CUSTOMER_CODE = "",
                    PRE_CUSTOMER_CODE = "",
                    GROUP_SKU_FLAG = "",

                    S_QTY = itemRows.Sum(x => x.S_QTY ?? 0),
                    S_VALUE = itemRows.Sum(x => x.S_VALUE ?? 0),

                    B_QTY = itemRows.Sum(x => x.B_QTY ?? 0),
                    B_VALUE = itemRows.Sum(x => x.B_VALUE ?? 0),

                    A_QTY = itemRows.Sum(x => x.A_QTY ?? 0),
                    A_VALUE = itemRows.Sum(x => x.A_VALUE ?? 0),

                    K_QTY = itemRows.Sum(x => x.K_QTY ?? 0),
                    K_VALUE = itemRows.Sum(x => x.K_VALUE ?? 0),

                    M_QTY = itemRows.Sum(x => x.M_QTY ?? 0),
                    M_VALUE = itemRows.Sum(x => x.M_VALUE ?? 0),

                    P_QTY = itemRows.Sum(x => x.P_QTY ?? 0),
                    P_VALUE = itemRows.Sum(x => x.P_VALUE ?? 0),

                    Mg_QTY = itemRows.Sum(x => x.Mg_QTY ?? 0),
                    Mg_VALUE = itemRows.Sum(x => x.Mg_VALUE ?? 0),

                    F_QTY = itemRows.Sum(x => x.F_QTY ?? 0),
                    F_VALUE = itemRows.Sum(x => x.F_VALUE ?? 0),

                    C_QTY = itemRows.Sum(x => x.C_QTY ?? 0),
                    C_VALUE = itemRows.Sum(x => x.C_VALUE ?? 0),

                    Bh_QTY = itemRows.Sum(x => x.Bh_QTY ?? 0),
                    Bh_VALUE = itemRows.Sum(x => x.Bh_VALUE ?? 0),

                    J_QTY = itemRows.Sum(x => x.J_QTY ?? 0),
                    J_VALUE = itemRows.Sum(x => x.J_VALUE ?? 0),

                    Aa_QTY = itemRows.Sum(x => x.Aa_QTY ?? 0),
                    Aa_VALUE = itemRows.Sum(x => x.Aa_VALUE ?? 0)
                };

                stockData.Add(grand);
            }
            return stockData;


        }


        public dynamic GetGroupwiseStockSummary(dynamic data, User userinfo)
        {
            string method = string.IsNullOrWhiteSpace((string)data?.valuationMethod) ? "FIFO" : (string)data.valuationMethod;

            string multiUnits = data?.multiUnit;
            string purchaseOption = data?.purchaseOption;
            string salesOption = data?.salesOption;
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";


            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            //// Optional: Convert to DateTime if needed
            string query = string.Format(@"SELECT b.ITEM_CODE, b.ITEM_EDESC, b.PRODUCT_CODE, b.INDEX_MU_CODE, b.MASTER_ITEM_CODE, b.PRE_ITEM_CODE, B.GROUP_SKU_FLAG, B.SERVICE_ITEM_FLAG, (LENGTH(B.MASTER_ITEM_CODE) - LENGTH(REPLACE(B.MASTER_ITEM_CODE,'.',''))) ROWLEV,   
                                                                OPEN_QTY, OPEN_AMT, PURCHASE_QTY, PURCHASE_AMT, PURCHASE_RET_QTY, PURCHASE_RET_AMT,  
                                                                SALES_QTY, SALES_AMT, SALES_RET_QTY, SALES_RET_AMT, SALES_NET_AMT, SI_AMT, SR_AMT, SRE_AMT, DN_AMT, CN_AMT,  
                                                                STK_REC_QTY, STK_REC_AMT, STK_TRANS_QTY, STK_TRANS_AMT, ST_TRNS_QTY, ST_TRNS_AMT, GOODS_ISS_QTY, GOODS_ISS_AMT, CIR_QTY, CIR_AMT, PRO_ISS_QTY, PRO_ISS_AMT, PRO_REC_QTY, PRO_REC_AMT,  
                                                                EXP_QTY, EXP_AMT, DA_EXP_QTY, DA_EXP_AMT, SAM_QTY, SAM_AMT, QC_QTY, QC_AMT, ROUND(OPEN_QTY + PURCHASE_QTY - PURCHASE_RET_QTY - SALES_QTY + SALES_RET_QTY + STK_REC_QTY - STK_TRANS_QTY - ST_TRNS_QTY - PRO_ISS_QTY + PRO_REC_QTY - GOODS_ISS_QTY - DA_EXP_QTY - SAM_QTY - QC_QTY + CIR_QTY,3) * DECODE(SERVICE_ITEM_FLAG,'Y',0,1)  CLOSING_QTY,  
                                                                ROUND(OPEN_AMT + PURCHASE_AMT  - PURCHASE_RET_AMT - SALES_AMT + SALES_RET_AMT + STK_REC_AMT - STK_TRANS_AMT - ST_TRNS_AMT - PRO_ISS_AMT + PRO_REC_AMT  - GOODS_ISS_AMT - DA_EXP_AMT - SAM_AMT - QC_AMT + CIR_AMT ,2) * DECODE(SERVICE_ITEM_FLAG,'Y',0,1) CLOSING_AMT  
                                                                FROM (SELECT ITEM_CODE,  
                                                                NVL(OPENING_IQ, 0) - NVL(OPENING_OQ, 0) OPEN_QTY,  
                                                                ROUND(NVL(OPENING_TIP, 0) - NVL(OPENING_TOP, 0), 2) OPEN_AMT,  
                                                                NVL(PURCHASE_IQ, 0) - NVL(PURCHASE_OQ, 0) PURCHASE_QTY,  
                                                                ROUND(NVL(PURCHASE_TIP, 0) - NVL(PURCHASE_TOP, 0), 2) PURCHASE_AMT,  
                                                                NVL(PURCHASE_RET_OQ, 0) - NVL(PURCHASE_RET_IQ, 0) PURCHASE_RET_QTY,  
                                                                ROUND(NVL(PURCHASE_RET_TOP, 0) - NVL(PURCHASE_RET_TIP, 0), 2) PURCHASE_RET_AMT,  
                                                                NVL(SALES_OQ, 0) - NVL(SALES_IQ, 0) SALES_QTY,  
                                                                ROUND(NVL(SALES_TOP, 0) - NVL(SALES_TIP, 0), 2) SALES_AMT,  
                                                                NVL(SALES_RETURN_IQ, 0) - NVL(SALES_RETURN_OQ, 0) SALES_RET_QTY,  
                                                                ROUND(NVL(SALES_RETURN_TIP, 0) - NVL(SALES_RETURN_TOP, 0), 2) SALES_RET_AMT,  
                                                                NVL(STK_REC_IQ, 0) - NVL(STK_REC_OQ, 0) STK_REC_QTY,  
                                                                ROUND(NVL(STK_REC_TIP, 0) - NVL(STK_REC_TOP, 0), 2) STK_REC_AMT,  
                                                                NVL(STK_TRANS_OQ, 0) - NVL(STK_TRANS_IQ, 0) STK_TRANS_QTY,  
                                                                ROUND(NVL(STK_TRANS_TOP, 0) - NVL(STK_TRANS_TIP, 0), 2) STK_TRANS_AMT,  
                                                                NVL(ST_TRNS_OQ, 0) - NVL(ST_TRNS_IQ, 0) ST_TRNS_QTY,  
                                                                ROUND(NVL(ST_TRNS_TOP, 0) - NVL(ST_TRNS_TIP, 0), 2) ST_TRNS_AMT,  
                                                                NVL(GOODS_ISS_OQ, 0) + NVL(GOODS_ISS_C_OQ, 0) - NVL(GOODS_ISS_IQ, 0) GOODS_ISS_QTY,  
                                                                ROUND(NVL(GOODS_ISS_TOP, 0) + NVL(GOODS_ISS_C_TOP, 0) - NVL(GOODS_ISS_TIP, 0), 2) GOODS_ISS_AMT,  
                                                                NVL(CIR_IQ, 0) - NVL(CIR_OQ, 0) CIR_QTY,  
                                                                ROUND(NVL(CIR_TIP, 0) - NVL(CIR_TOP, 0), 2) CIR_AMT,  
                                                                NVL(PRO_ISS_OQ, 0) - NVL(PRO_ISS_IQ, 0)  PRO_ISS_QTY,  
                                                                ROUND(NVL(PRO_ISS_TOP, 0) - NVL(PRO_ISS_TIP, 0), 2) PRO_ISS_AMT,  
                                                                NVL(PRO_REC_IQ, 0) - NVL(PRO_REC_OQ, 0) PRO_REC_QTY,  
                                                                ROUND(NVL(PRO_REC_TIP, 0) - NVL(PRO_REC_TOP, 0), 2) PRO_REC_AMT,  
                                                                ROUND(NVL(SALES_TNP, 0), 2) SALES_NET_AMT,  
                                                                ROUND(NVL(SI_TOP, 0)- NVL(SI_TIP, 0), 2) SI_AMT,  
                                                                ROUND(NVL(SR_TIP, 0)- NVL(SR_TOP, 0), 2) SR_AMT,  
                                                                ROUND(NVL(SRE_TIP, 0)- NVL(SRE_TOP, 0), 2) SRE_AMT,  
                                                                ROUND(NVL(D_NOTE_TIP, 0)- NVL(D_NOTE_TOP, 0), 2) DN_AMT,  
                                                                ROUND(NVL(C_NOTE_TIP, 0)- NVL(C_NOTE_TOP, 0), 2) CN_AMT,  
                                                                NVL(DA_EXP_OQ, 0) - NVL(DA_EXP_IQ, 0)  DA_EXP_QTY,  
                                                                ROUND(NVL(DA_EXP_TOP, 0) - NVL(DA_EXP_TIP, 0), 2) DA_EXP_AMT,  
                                                                NVL(SAM_OQ, 0) - NVL(SAM_IQ, 0)  SAM_QTY,  
                                                                ROUND(NVL(SAM_TOP, 0) - NVL(SAM_TIP, 0), 2) SAM_AMT,  
                                                                NVL(QC_OQ, 0) - NVL(QC_IQ, 0)  QC_QTY,  
                                                                ROUND(NVL(QC_TOP, 0) - NVL(QC_TIP, 0), 2) QC_AMT,  
                                                                NVL(S_EXP_IQ, 0) - NVL(S_EXP_OQ, 0) EXP_QTY,  
                                                                ROUND(NVL(S_EXP_TIP, 0) - NVL(S_EXP_TOP, 0), 2) EXP_AMT  
                                                                FROM (  
                                                                SELECT PD.* FROM (  
                                                                SELECT * FROM (  
                                                                SELECT  A.ITEM_CODE ,  
                                                                IN_QUANTITY, IN_QUANTITY * IN_UNIT_PRICE TOTALINPRICE, OUT_QUANTITY, OUT_QUANTITY * OUT_UNIT_PRICE  TOTALOUTPRICE, 0 TOTALNETPRICE, SOURCE SOURCE_TABLE  
                                                                FROM V_VALUE_STOCK_LEDGER A,  IP_ITEM_MASTER_SETUP B  
                                                                WHERE VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')    
                                                                AND METHOD = 'FIFO'  
                                                                AND A.ITEM_CODE = B.ITEM_CODE AND A.COMPANY_CODE = B.COMPANY_CODE AND A.MU_CODE= B.INDEX_MU_CODE AND A.COMPANY_CODE IN ({2}) AND A.BRANCH_CODE IN ({3}) UNION ALL  
                                                                SELECT  A.ITEM_CODE ,  QUANTITY IN_QUANTITY, ROUND((QUANTITY * CALC_UNIT_PRICE) * EXCHANGE_RATE,2) TOTALINPRICE, 0 OUT_QUANTITY, 0  TOTALOUTPRICE, 0 TOTALNETPRICE, 'IP_PURCHASE_INVOICE' SOURCE_TABLE   
                                                                FROM IP_PURCHASE_INVOICE A  
                                                                WHERE INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                AND INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')  
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})  
                                                                 AND A.DELETED_FLAG = 'N'  
                                                                UNION ALL  
                                                                SELECT  A.ITEM_CODE , 0 IN_QUANTITY, 0 TOTALINPRICE, QUANTITY OUT_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_SALES_INVOICE_V' SOURCE_TABLE   
                                                                FROM SA_SALES_INVOICE A  
                                                                WHERE  SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')  
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})   
                                                                AND A.DELETED_FLAG = 'N'  
                                                                UNION ALL  
                                                                SELECT  A.ITEM_CODE , 0 IN_QUANTITY, (NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_DEBIT_NOTE' SOURCE_TABLE   
                                                                FROM SA_DEBIT_NOTE A  
                                                                WHERE VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')  
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})  
                                                                AND A.DELETED_FLAG = 'N'  
                                                                UNION ALL  
                                                                SELECT  A.ITEM_CODE , 0 IN_QUANTITY, (NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_CREDIT_NOTE' SOURCE_TABLE   
                                                                FROM SA_CREDIT_NOTE A  
                                                                WHERE VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')  
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})  
                                                                AND A.DELETED_FLAG = 'N'  
                                                                Union All  
                                                                SELECT  A.ITEM_CODE , QUANTITY IN_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALINPRICE, 0 OUT_QUANTITY, 0  TOTALOUTPRICE, 0 TOTALNETPRICE, 'SALES_RETURN' SOURCE_TABLE  
                                                                FROM SA_SALES_RETURN A  
                                                                WHERE RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                AND RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')  
                                                                AND (SALES_TYPE_CODE IS NULL OR (SALES_TYPE_CODE, COMPANY_CODE) IN (SELECT SALES_TYPE_CODE, COMPANY_CODE FROM SA_SALES_TYPE WHERE EXP_FLAG = 'N'))  
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})   
                                                                AND A.DELETED_FLAG = 'N'  
                                                                Union All  
                                                                SELECT  A.ITEM_CODE , QUANTITY IN_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALINPRICE, 0 OUT_QUANTITY, 0  TOTALOUTPRICE, 0 TOTALNETPRICE, 'RET_EXP' SOURCE_TABLE  
                                                                FROM SA_SALES_RETURN A  
                                                                WHERE RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                AND RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                                AND (SALES_TYPE_CODE, COMPANY_CODE) IN (SELECT SALES_TYPE_CODE, COMPANY_CODE FROM SA_SALES_TYPE WHERE EXP_FLAG = 'Y')  
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})  
                                                                AND A.DELETED_FLAG = 'N'  
                                                                UNION ALL  
                                                                SELECT b.ITEM_CODE ,   
                                                                OPEN_QTY + PURCHASE_QTY - PURCHASE_RET_QTY - SALES_QTY + SALES_RET_QTY + STK_REC_QTY  - STK_TRANS_QTY - ST_TRNS_QTY - PRO_ISS_QTY + PRO_REC_QTY - GOODS_ISS_QTY - DA_EXP_QTY - SAM_QTY - QC_QTY + CIR_QTY IN_QUANTITY,  
                                                                OPEN_AMT + PURCHASE_AMT - PURCHASE_RET_AMT  - SALES_AMT + SALES_RET_AMT + STK_REC_AMT  - STK_TRANS_AMT - ST_TRNS_AMT - PRO_ISS_AMT + PRO_REC_AMT - GOODS_ISS_AMT - DA_EXP_AMT - SAM_AMT - QC_AMT + CIR_AMT - DN_AMT + CN_AMT TOTALINPRICE,  
                                                                0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'OPENING_BALANCE' SOURCE_TABLE  
                                                                FROM (SELECT ITEM_CODE,  
                                                                NVL(OPENING_IQ, 0) - NVL(OPENING_OQ, 0) OPEN_QTY,  
                                                                ROUND(NVL(OPENING_TIP, 0) - NVL(OPENING_TOP, 0), 2) OPEN_AMT,  
                                                                NVL(PURCHASE_IQ, 0) - NVL(PURCHASE_OQ, 0) PURCHASE_QTY,  
                                                                ROUND(NVL(PURCHASE_TIP, 0) - NVL(PURCHASE_TOP, 0), 2) PURCHASE_AMT,  
                                                                NVL(PURCHASE_RET_OQ, 0) - NVL(PURCHASE_RET_IQ, 0) PURCHASE_RET_QTY,  
                                                                ROUND(NVL(PURCHASE_RET_TOP, 0) - NVL(PURCHASE_RET_TIP, 0), 2) PURCHASE_RET_AMT,  
                                                                NVL(SALES_OQ, 0) - NVL(SALES_IQ, 0)  SALES_QTY,  
                                                                ROUND(NVL(SALES_TOP, 0)- NVL(SALES_TIP, 0), 2) SALES_AMT,  
                                                                NVL(SALES_RETURN_IQ, 0) - NVL(SALES_RETURN_OQ, 0) SALES_RET_QTY,  
                                                                ROUND(NVL(SALES_RETURN_TIP, 0) - NVL(SALES_RETURN_TOP, 0), 2) SALES_RET_AMT,  
                                                                NVL(STK_REC_IQ, 0) - NVL(STK_REC_OQ, 0) STK_REC_QTY,  
                                                                ROUND(NVL(STK_REC_TIP, 0) - NVL(STK_REC_TOP, 0), 2) STK_REC_AMT,  
                                                                NVL(STK_TRANS_OQ, 0) - NVL(STK_TRANS_IQ, 0) STK_TRANS_QTY,  
                                                                ROUND(NVL(STK_TRANS_TOP, 0) - NVL(STK_TRANS_TIP, 0), 2) STK_TRANS_AMT,  
                                                                NVL(ST_TRNS_OQ, 0) - NVL(ST_TRNS_IQ, 0) ST_TRNS_QTY,  
                                                                ROUND(NVL(ST_TRNS_TOP, 0)- NVL(ST_TRNS_TIP, 0), 2) ST_TRNS_AMT,  
                                                                NVL(GOODS_ISS_OQ, 0) + NVL(GOODS_ISS_C_OQ, 0) - NVL(GOODS_ISS_IQ, 0) GOODS_ISS_QTY,  
                                                                ROUND(NVL(GOODS_ISS_TOP, 0) + NVL(GOODS_ISS_C_TOP, 0) - NVL(GOODS_ISS_TIP, 0), 2) GOODS_ISS_AMT,  
                                                                NVL(CIR_IQ, 0) - NVL(CIR_OQ, 0) CIR_QTY,  
                                                                ROUND(NVL(CIR_TIP, 0)- NVL(CIR_TOP, 0), 2) CIR_AMT,  
                                                                NVL(PRO_ISS_OQ, 0) - NVL(PRO_ISS_IQ, 0) PRO_ISS_QTY,  
                                                                ROUND(NVL(PRO_ISS_TOP, 0)- NVL(PRO_ISS_TIP, 0), 2) PRO_ISS_AMT,  
                                                                NVL(PRO_REC_IQ, 0) - NVL(PRO_REC_OQ, 0) PRO_REC_QTY,  
                                                                ROUND(NVL(PRO_REC_TIP, 0) - NVL(PRO_REC_TOP, 0), 2) PRO_REC_AMT,  
                                                                ROUND(NVL(SALES_TNP, 0) , 2) SALES_NET_AMT,  
                                                                ROUND(NVL(SI_TOP, 0)- NVL(SI_TIP, 0), 2) SI_AMT,  
                                                                ROUND(NVL(SR_TIP, 0)- NVL(SR_TOP, 0), 2) SR_AMT,  
                                                                ROUND(NVL(SRE_TIP, 0)- NVL(SRE_TOP, 0), 2) SRE_AMT,  
                                                                ROUND(NVL(D_NOTE_TIP, 0)- NVL(D_NOTE_TOP, 0), 2) DN_AMT,  
                                                                ROUND(NVL(C_NOTE_TIP, 0)- NVL(C_NOTE_TOP, 0), 2) CN_AMT,  
                                                                NVL(DA_EXP_OQ, 0) - NVL(DA_EXP_IQ, 0)  DA_EXP_QTY,  
                                                                ROUND(NVL(DA_EXP_TOP, 0) - NVL(DA_EXP_TIP, 0), 2) DA_EXP_AMT,  
                                                                NVL(SAM_OQ, 0) - NVL(SAM_IQ, 0)  SAM_QTY,  
                                                                ROUND(NVL(SAM_TOP, 0) - NVL(SAM_TIP, 0), 2) SAM_AMT,  
                                                                NVL(QC_OQ, 0) - NVL(QC_IQ, 0)  QC_QTY,  
                                                                ROUND(NVL(QC_TOP, 0) - NVL(QC_TIP, 0), 2) QC_AMT,  
                                                                NVL(S_EXP_IQ, 0) - NVL(S_EXP_OQ, 0) EXP_QTY,  
                                                                ROUND(NVL(S_EXP_TIP, 0) - NVL(S_EXP_TOP, 0), 2) EXP_AMT  
                                                                FROM (  
                                                                SELECT PD.* FROM (  
                                                                SELECT * FROM (  
                                                                SELECT  A.ITEM_CODE ,  
                                                                IN_QUANTITY, IN_QUANTITY * IN_UNIT_PRICE TOTALINPRICE, OUT_QUANTITY, OUT_QUANTITY * OUT_UNIT_PRICE  TOTALOUTPRICE, 0 TOTALNETPRICE, SOURCE SOURCE_TABLE  
                                                                FROM V_VALUE_STOCK_LEDGER A, IP_ITEM_MASTER_SETUP B   
                                                                WHERE VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD')  
                                                                 AND METHOD = 'FIFO'  
                                                                AND A.ITEM_CODE = B.ITEM_CODE AND A.COMPANY_CODE = B.COMPANY_CODE AND A.MU_CODE= B.INDEX_MU_CODE AND A.COMPANY_CODE IN ({2}) AND A.BRANCH_CODE IN ({3}) UNION ALL  
                                                                SELECT  A.ITEM_CODE , QUANTITY IN_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'SALES_RETURN' SOURCE_TABLE  
                                                                FROM SA_SALES_RETURN A  
                                                                WHERE RETURN_DATE < TO_DATE('{0}', 'YYYY-MM-DD')  
                                                                AND (SALES_TYPE_CODE IS NULL OR (SALES_TYPE_CODE, COMPANY_CODE) IN (SELECT SALES_TYPE_CODE, COMPANY_CODE FROM SA_SALES_TYPE WHERE EXP_FLAG = 'N'))  
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3}) 
                                                                AND A.DELETED_FLAG = 'N'  
                                                                UNION ALL  
                                                                SELECT  A.ITEM_CODE , QUANTITY IN_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'RET_EXP' SOURCE_TABLE  
                                                                FROM SA_SALES_RETURN A  
                                                                WHERE RETURN_DATE < TO_DATE('{0}', 'YYYY-MM-DD')  
                                                                AND (SALES_TYPE_CODE, COMPANY_CODE) IN (SELECT SALES_TYPE_CODE, COMPANY_CODE FROM SA_SALES_TYPE WHERE EXP_FLAG = 'Y')  
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3}) 
                                                                AND A.DELETED_FLAG = 'N'  
                                                                UNION ALL  
                                                                SELECT  A.ITEM_CODE , 0 IN_QUANTITY, 0 TOTALINPRICE, QUANTITY OUT_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0)  TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_SALES_INVOICE_V' SOURCE_TABLE  
                                                                FROM SA_SALES_INVOICE A  
                                                                WHERE SALES_DATE < TO_DATE('{0}', 'YYYY-MM-DD')  
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})  
                                                                AND A.DELETED_FLAG = 'N'  
                                                                UNION ALL  
                                                                SELECT  A.ITEM_CODE , 0 IN_QUANTITY, TOTAL_PRICE TOTALINPRICE, 0 OUT_QUANTITY, 0  TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_DEBIT_NOTE' SOURCE_TABLE  
                                                                FROM SA_DEBIT_NOTE A  
                                                                WHERE VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD')  
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3}) 
                                                                AND A.DELETED_FLAG = 'N'  
                                                                UNION ALL  
                                                                SELECT  A.ITEM_CODE , 0 IN_QUANTITY, TOTAL_PRICE  TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_CREDIT_NOTE' SOURCE_TABLE  
                                                                FROM SA_CREDIT_NOTE A  
                                                                WHERE VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})  
                                                                AND A.DELETED_FLAG = 'N'  
                                                                )  
                                                                PIVOT  
                                                                (  
                                                                SUM(IN_QUANTITY) AS IQ,  
                                                                   SUM(TOTALINPRICE) AS TIP,  
                                                                   SUM(OUT_QUANTITY) AS OQ,  
                                                                   SUM(TOTALOUTPRICE) As TOP,  
                                                                   SUM(TOTALNETPRICE) As TNP  
                                                                FOR  
                                                                   (SOURCE_TABLE)  
                                                                   IN('OPENING_BALANCE' AS OPENING, 'IP_PURCHASE_MRR' AS PURCHASE, 'IP_PURCHASE_RETURN' AS PURCHASE_RET, 'SA_SALES_CHALAN' AS SALES, 'SA_SALES_RETURN' AS SALES_RETURN, 'SA_SALES_INVOICE_V' AS SI,   'SALES_RETURN' AS SR, 'RET_EXP' AS SRE,  
                                                                   'IP_ADVICE_MRR' AS STK_REC, 'IP_ADVICE_ISSUE' AS STK_TRANS, 'IP_TRANSFER_ISSUE' AS ST_TRNS, 'IP_GOODS_ISSUE_CHARGE' AS GOODS_ISS_C, 'IP_GOODS_ISSUE' AS GOODS_ISS, 'IP_GOODS_ISSUE_RETURN' AS CIR, 'IP_PRODUCTION_ISSUE' AS PRO_ISS, 'IP_PRODUCTION_MRR' AS PRO_REC, 'SA_DEBIT_NOTE' AS D_NOTE ,'SA_CREDIT_NOTE' AS C_NOTE, 'SA_SALES_RETURN_EXP' AS S_EXP, 'IP_DAMAGE_ISSUE' AS DA_EXP ,'IP_SAMPLE_ISSUE' SAM ,'IP_QC_ISSUE' QC   
                                                                   )  
                                                                )) PD  
                                                                )) aa, IP_ITEM_MASTER_SETUP B  
                                                                WHERE B.ITEM_CODE = aa.ITEM_CODE (+)  
                                                                 AND B.COMPANY_CODE IN ({2})  
                                                                 AND B.DELETED_FLAG = 'N'  
                                                                )  
                                                                PIVOT  
                                                                (  
                                                                SUM(IN_QUANTITY) AS IQ,  
                                                                   SUM(TOTALINPRICE) AS TIP,  
                                                                   SUM(OUT_QUANTITY) AS OQ,  
                                                                   SUM(TOTALOUTPRICE) As TOP,  
                                                                   SUM(TOTALNETPRICE) As TNP  
                                                                FOR  
                                                                   (SOURCE_TABLE)  
                                                                   IN('OPENING_BALANCE' AS OPENING, 'IP_PURCHASE_MRR' AS PURCHASE, 'IP_PURCHASE_RETURN' AS PURCHASE_RET, 'SA_SALES_CHALAN' AS SALES, 'SA_SALES_RETURN' AS SALES_RETURN, 'SA_SALES_INVOICE_V' AS SI,   'SALES_RETURN' AS SR, 'RET_EXP' AS SRE,  
                                                                   'IP_ADVICE_MRR' AS STK_REC, 'IP_ADVICE_ISSUE' AS STK_TRANS, 'IP_TRANSFER_ISSUE' AS ST_TRNS, 'IP_GOODS_ISSUE_CHARGE' AS GOODS_ISS_C, 'IP_GOODS_ISSUE' AS GOODS_ISS, 'IP_GOODS_ISSUE_RETURN' AS CIR, 'IP_PRODUCTION_ISSUE' AS PRO_ISS, 'IP_PRODUCTION_MRR' AS PRO_REC, 'SA_DEBIT_NOTE' AS D_NOTE ,'SA_CREDIT_NOTE' AS C_NOTE, 'SA_SALES_RETURN_EXP' AS S_EXP, 'IP_DAMAGE_ISSUE' AS DA_EXP ,'IP_SAMPLE_ISSUE' SAM ,'IP_QC_ISSUE' QC    
                                                                   )  
                                                                )) PD  
                                                                )) aa, IP_ITEM_MASTER_SETUP B  
                                                                WHERE B.ITEM_CODE = aa.ITEM_CODE (+)  
                                                                 AND B.COMPANY_CODE IN ({2}) 
                                                                 AND B.DELETED_FLAG = 'N'  
                                                                ORDER BY b.MASTER_ITEM_CODE, b.PRE_ITEM_CODE, b.ITEM_EDESC", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<GroupwiseStockSummary>(query).ToList();

            var groupedData = monthwise;
            var allowedCodes = new HashSet<string>
                {
                    "01", "02", "03", "04", "05", "06", "07","08","09",
                    "001", "002", "003", "004", "005", "006", "007", "009","10"
                };

            foreach (var item in groupedData)
            {
                if (allowedCodes.Contains(item.MASTER_ITEM_CODE))
                {
                    item.PRE_ITEM_CODE = "";
                }
            }
            var itemRows = groupedData.Where(r => r.GROUP_SKU_FLAG == "I").ToList();

            List<dynamic> stockData = new List<dynamic>();

            foreach (var row in groupedData)
            {
                double total1 = 0;  // OPEN_QTY
                double total2 = 0;  // OPEN_AMT

                double total3 = 0;  // PURCHASE_QTY
                double total4 = 0;  // PURCHASE_AMT
                double total5 = 0;  // PURCHASE_RET_QTY
                double total6 = 0;  // PURCHASE_RET_AMT

                double total7 = 0;  // SALES_QTY
                double total8 = 0;  // SALES_AMT
                double total9 = 0;  // SALES_RET_QTY
                double total10 = 0; // SALES_RET_AMT
                double total11 = 0; // SALES_NET_AMT

                double total12 = 0; // SI_AMT
                double total13 = 0; // SR_AMT
                double total14 = 0; // SRE_AMT
                double total15 = 0; // DN_AMT
                double total16 = 0; // CN_AMT

                double total17 = 0; // STK_REC_QTY
                double total18 = 0; // STK_REC_AMT

                double total19 = 0; // STK_TRANS_QTY
                double total20 = 0; // STK_TRANS_AMT
                double total21 = 0; // ST_TRNS_QTY
                double total22 = 0; // ST_TRNS_AMT

                double total23 = 0; // GOODS_ISS_QTY
                double total24 = 0; // GOODS_ISS_AMT

                double total25 = 0; // CIR_QTY
                double total26 = 0; // CIR_AMT

                double total27 = 0; // PRO_ISS_QTY
                double total28 = 0; // PRO_ISS_AMT

                double total29 = 0; // PRO_REC_QTY
                double total30 = 0; // PRO_REC_AMT

                double total31 = 0; // EXP_QTY
                double total32 = 0; // EXP_AMT

                double total33 = 0; // DA_EXP_QTY
                double total34 = 0; // DA_EXP_AMT

                double total35 = 0; // SAM_QTY
                double total36 = 0; // SAM_AMT

                double total37 = 0; // QC_QTY
                double total38 = 0; // QC_AMT

                double total39 = 0; // CLOSING_QTY
                double total40 = 0; // CLOSING_AMT
                if (row.GROUP_SKU_FLAG == "G")
                {
                    string masterCode = row.MASTER_ITEM_CODE;
                    foreach (var z in itemRows)
                    {
                        if (!string.IsNullOrEmpty(z.MASTER_ITEM_CODE) &&
                            z.MASTER_ITEM_CODE.StartsWith(masterCode))
                        {
                            total1 += z.OPEN_QTY ?? 0;
                            total2 += z.OPEN_AMT ?? 0;

                            total3 += z.PURCHASE_QTY ?? 0;
                            total4 += z.PURCHASE_AMT ?? 0;
                            total5 += z.PURCHASE_RET_QTY ?? 0;
                            total6 += z.PURCHASE_RET_AMT ?? 0;

                            total7 += z.SALES_QTY ?? 0;
                            total8 += z.SALES_AMT ?? 0;
                            total9 += z.SALES_RET_QTY ?? 0;
                            total10 += z.SALES_RET_AMT ?? 0;
                            total11 += z.SALES_NET_AMT ?? 0;

                            total12 += z.SI_AMT ?? 0;
                            total13 += z.SR_AMT ?? 0;
                            total14 += z.SRE_AMT ?? 0;
                            total15 += z.DN_AMT ?? 0;
                            total16 += z.CN_AMT ?? 0;

                            total17 += z.STK_REC_QTY ?? 0;
                            total18 += z.STK_REC_AMT ?? 0;

                            total19 += z.STK_TRANS_QTY ?? 0;
                            total20 += z.STK_TRANS_AMT ?? 0;
                            total21 += z.ST_TRNS_QTY ?? 0;
                            total22 += z.ST_TRNS_AMT ?? 0;

                            total23 += z.GOODS_ISS_QTY ?? 0;
                            total24 += z.GOODS_ISS_AMT ?? 0;

                            total25 += z.CIR_QTY ?? 0;
                            total26 += z.CIR_AMT ?? 0;

                            total27 += z.PRO_ISS_QTY ?? 0;
                            total28 += z.PRO_ISS_AMT ?? 0;

                            total29 += z.PRO_REC_QTY ?? 0;
                            total30 += z.PRO_REC_AMT ?? 0;

                            total31 += z.EXP_QTY ?? 0;
                            total32 += z.EXP_AMT ?? 0;

                            total33 += z.DA_EXP_QTY ?? 0;
                            total34 += z.DA_EXP_AMT ?? 0;

                            total35 += z.SAM_QTY ?? 0;
                            total36 += z.SAM_AMT ?? 0;

                            total37 += z.QC_QTY ?? 0;
                            total38 += z.QC_AMT ?? 0;

                            total39 += z.CLOSING_QTY ?? 0;
                            total40 += z.CLOSING_AMT ?? 0;

                        }
                    }
                }
                else
                {
                    total1 = row.OPEN_QTY ?? 0;
                    total2 = row.OPEN_AMT ?? 0;

                    total3 = row.PURCHASE_QTY ?? 0;
                    total4 = row.PURCHASE_AMT ?? 0;
                    total5 = row.PURCHASE_RET_QTY ?? 0;
                    total6 = row.PURCHASE_RET_AMT ?? 0;

                    total7 = row.SALES_QTY ?? 0;
                    total8 = row.SALES_AMT ?? 0;
                    total9 = row.SALES_RET_QTY ?? 0;
                    total10 = row.SALES_RET_AMT ?? 0;
                    total11 = row.SALES_NET_AMT ?? 0;

                    total12 = row.SI_AMT ?? 0;
                    total13 = row.SR_AMT ?? 0;
                    total14 = row.SRE_AMT ?? 0;
                    total15 = row.DN_AMT ?? 0;
                    total16 = row.CN_AMT ?? 0;

                    total17 = row.STK_REC_QTY ?? 0;
                    total18 = row.STK_REC_AMT ?? 0;

                    total19 = row.STK_TRANS_QTY ?? 0;
                    total20 = row.STK_TRANS_AMT ?? 0;
                    total21 = row.ST_TRNS_QTY ?? 0;
                    total22 = row.ST_TRNS_AMT ?? 0;

                    total23 = row.GOODS_ISS_QTY ?? 0;
                    total24 = row.GOODS_ISS_AMT ?? 0;

                    total25 = row.CIR_QTY ?? 0;
                    total26 = row.CIR_AMT ?? 0;

                    total27 = row.PRO_ISS_QTY ?? 0;
                    total28 = row.PRO_ISS_AMT ?? 0;

                    total29 = row.PRO_REC_QTY ?? 0;
                    total30 = row.PRO_REC_AMT ?? 0;

                    total31 = row.EXP_QTY ?? 0;
                    total32 = row.EXP_AMT ?? 0;

                    total33 = row.DA_EXP_QTY ?? 0;
                    total34 = row.DA_EXP_AMT ?? 0;

                    total35 = row.SAM_QTY ?? 0;
                    total36 = row.SAM_AMT ?? 0;

                    total37 = row.QC_QTY ?? 0;
                    total38 = row.QC_AMT ?? 0;

                    total39 = row.CLOSING_QTY ?? 0;
                    total40 = row.CLOSING_AMT ?? 0;

                }
                var totals = new[]
                {
                    total1, total2, total3, total4, total5, total6, total7, total8, total9, total10,
                    total11, total12, total13, total14, total15, total16, total17, total18, total19, total20,
                    total21, total22, total23, total24, total25, total26, total27, total28, total29, total30,
                    total31, total32, total33, total34, total35, total36, total37, total38, total39, total40
                };
                if (totals.Any(t => t != 0))
                {
                    stockData.Add(new
                    {
                        ITEM_CODE = row.ITEM_CODE,
                        ITEM_EDESC = row.ITEM_EDESC,
                        INDEX_MU_CODE = row.INDEX_MU_CODE,
                        PRODUCT_CODE = row.PRODUCT_CODE,
                        MASTER_ITEM_CODE = row.MASTER_ITEM_CODE,
                        PRE_ITEM_CODE = row.PRE_ITEM_CODE == "00" ? " " : row.PRE_ITEM_CODE,

                        OPEN_QTY = total1,
                        OPEN_AMT = total2,

                        PURCHASE_QTY = total3,
                        PURCHASE_AMT = total4,
                        PURCHASE_RET_QTY = total5,
                        PURCHASE_RET_AMT = total6,

                        SALES_QTY = total7,
                        SALES_AMT = total8,
                        SALES_RET_QTY = total9,
                        SALES_RET_AMT = total10,
                        SALES_NET_AMT = total11,

                        SI_AMT = total12,
                        SR_AMT = total13,
                        SRE_AMT = total14,
                        DN_AMT = total15,
                        CN_AMT = total16,

                        STK_REC_QTY = total17,
                        STK_REC_AMT = total18,

                        STK_TRANS_QTY = total19,
                        STK_TRANS_AMT = total20,
                        ST_TRNS_QTY = total21,
                        ST_TRNS_AMT = total22,

                        GOODS_ISS_QTY = total23,
                        GOODS_ISS_AMT = total24,

                        CIR_QTY = total25,
                        CIR_AMT = total26,

                        PRO_ISS_QTY = total27,
                        PRO_ISS_AMT = total28,

                        PRO_REC_QTY = total29,
                        PRO_REC_AMT = total30,

                        EXP_QTY = total31,
                        EXP_AMT = total32,

                        DA_EXP_QTY = total33,
                        DA_EXP_AMT = total34,

                        SAM_QTY = total35,
                        SAM_AMT = total36,

                        QC_QTY = total37,
                        QC_AMT = total38,

                        CLOSING_QTY = total39,
                        CLOSING_AMT = total40
                    });
                }



            }
            //comment without Grand Total

            if (stockData.Any())
            {
                var grandTotal = new
                {
                    ITEM_CODE = "",
                    ITEM_EDESC = "Grand Total",
                    INDEX_MU_CODE = "",
                    PRODUCT_CODE = "",
                    MASTER_ITEM_CODE = "",
                    PRE_ITEM_CODE = "",

                    OPEN_QTY = itemRows.Sum(x => x.OPEN_QTY ?? 0),
                    OPEN_AMT = itemRows.Sum(x => x.OPEN_AMT ?? 0),

                    PURCHASE_QTY = itemRows.Sum(x => x.PURCHASE_QTY ?? 0),
                    PURCHASE_AMT = itemRows.Sum(x => x.PURCHASE_AMT ?? 0),
                    PURCHASE_RET_QTY = itemRows.Sum(x => x.PURCHASE_RET_QTY ?? 0),
                    PURCHASE_RET_AMT = itemRows.Sum(x => x.PURCHASE_RET_AMT ?? 0),

                    SALES_QTY = itemRows.Sum(x => x.SALES_QTY ?? 0),
                    SALES_AMT = itemRows.Sum(x => x.SALES_AMT ?? 0),
                    SALES_RET_QTY = itemRows.Sum(x => x.SALES_RET_QTY ?? 0),
                    SALES_RET_AMT = itemRows.Sum(x => x.SALES_RET_AMT ?? 0),
                    SALES_NET_AMT = itemRows.Sum(x => x.SALES_NET_AMT ?? 0),

                    SI_AMT = itemRows.Sum(x => x.SI_AMT ?? 0),
                    SR_AMT = itemRows.Sum(x => x.SR_AMT ?? 0),
                    SRE_AMT = itemRows.Sum(x => x.SRE_AMT ?? 0),
                    DN_AMT = itemRows.Sum(x => x.DN_AMT ?? 0),
                    CN_AMT = itemRows.Sum(x => x.CN_AMT ?? 0),

                    STK_REC_QTY = itemRows.Sum(x => x.STK_REC_QTY ?? 0),
                    STK_REC_AMT = itemRows.Sum(x => x.STK_REC_AMT ?? 0),

                    STK_TRANS_QTY = itemRows.Sum(x => x.STK_TRANS_QTY ?? 0),
                    STK_TRANS_AMT = itemRows.Sum(x => x.STK_TRANS_AMT ?? 0),
                    ST_TRNS_QTY = itemRows.Sum(x => x.ST_TRNS_QTY ?? 0),
                    ST_TRNS_AMT = itemRows.Sum(x => x.ST_TRNS_AMT ?? 0),

                    GOODS_ISS_QTY = itemRows.Sum(x => x.GOODS_ISS_QTY ?? 0),
                    GOODS_ISS_AMT = itemRows.Sum(x => x.GOODS_ISS_AMT ?? 0),

                    CIR_QTY = itemRows.Sum(x => x.CIR_QTY ?? 0),
                    CIR_AMT = itemRows.Sum(x => x.CIR_AMT ?? 0),

                    PRO_ISS_QTY = itemRows.Sum(x => x.PRO_ISS_QTY ?? 0),
                    PRO_ISS_AMT = itemRows.Sum(x => x.PRO_ISS_AMT ?? 0),

                    PRO_REC_QTY = itemRows.Sum(x => x.PRO_REC_QTY ?? 0),
                    PRO_REC_AMT = itemRows.Sum(x => x.PRO_REC_AMT ?? 0),

                    EXP_QTY = itemRows.Sum(x => x.EXP_QTY ?? 0),
                    EXP_AMT = itemRows.Sum(x => x.EXP_AMT ?? 0),

                    DA_EXP_QTY = itemRows.Sum(x => x.DA_EXP_QTY ?? 0),
                    DA_EXP_AMT = itemRows.Sum(x => x.DA_EXP_AMT ?? 0),

                    SAM_QTY = itemRows.Sum(x => x.SAM_QTY ?? 0),
                    SAM_AMT = itemRows.Sum(x => x.SAM_AMT ?? 0),

                    QC_QTY = itemRows.Sum(x => x.QC_QTY ?? 0),
                    QC_AMT = itemRows.Sum(x => x.QC_AMT ?? 0),

                    CLOSING_QTY = itemRows.Sum(x => x.CLOSING_QTY ?? 0),
                    CLOSING_AMT = itemRows.Sum(x => x.CLOSING_AMT ?? 0)
                };

                stockData.Add(grandTotal);
            }
            return stockData;
        }


        public List<BranchwiseEmployeeNetSalesModel> BranchwiseEmployeeNetSales(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";

            // --- Form Code Filter (works at both root or inside filters) ---
            var formCodeFilter = filters?.FormCodeFilter as IEnumerable<object>;
            // ?? data?.FormCodeFilter as IEnumerable<object>;

            var rateColumn = formCodeFilter != null && formCodeFilter.Any()
                ? formCodeFilter.FirstOrDefault()?.ToString()
                : null;
            if (string.IsNullOrEmpty(rateColumn))
            {
                rateColumn = "NET_GROSS_RATE"; // Set a default if it's missing
            }



            string query = string.Format(@"SELECT B.EMPLOYEE_CODE, B.EMPLOYEE_EDESC, B.MASTER_EMPLOYEE_CODE MASTER_CODE, B.PRE_EMPLOYEE_CODE PRE_CODE, B.GROUP_SKU_FLAG GROUP_FLAG, (LENGTH(B.MASTER_EMPLOYEE_CODE) - LENGTH(REPLACE(B.MASTER_EMPLOYEE_CODE,'.',''))) ROWLEV,  A.* FROM (  
                                                        SELECT * FROM (  
                                                        SELECT A.COMPANY_CODE, EMPLOYEE_CODE EMP_CODE, BRANCH_CODE,   
                                                        SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE FROM (   
                                                        SELECT A.BRANCH_CODE ,A.COMPANY_CODE, A.EMPLOYEE_CODE, SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE  FROM (   
                                                        SELECT A.COMPANY_CODE, A.EMPLOYEE_CODE, A.BRANCH_CODE , SUM(NVL(A.QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY*A.{4},0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE   FROM SA_SALES_INVOICE A   
                                                        WHERE A.DELETED_FLAG = 'N'   
                                                        AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                        AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                        AND A.COMPANY_CODE IN ({2})
                                                        AND A.BRANCH_CODE IN ({3}) 
                                                        GROUP BY  A.EMPLOYEE_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                                        Union All   
                                                        SELECT A.COMPANY_CODE, A.EMPLOYEE_CODE, A.BRANCH_CODE, 0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY,0))  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.{4},0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE   FROM SA_SALES_RETURN A   
                                                        WHERE A.DELETED_FLAG = 'N'   
                                                        AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                        AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                        AND A.COMPANY_CODE IN ({2})
                                                        AND A.BRANCH_CODE IN ({3})  
                                                        GROUP BY  A.EMPLOYEE_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                                        Union All   
                                                        SELECT A.COMPANY_CODE, A.EMPLOYEE_CODE, A.BRANCH_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.{4},0)) DEBIT_VALUE, 0 CREDIT_VALUE  FROM SA_DEBIT_NOTE A   
                                                        WHERE A.DELETED_FLAG = 'N'   
                                                        AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                        AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                        AND A.COMPANY_CODE IN ({2})
                                                        AND A.BRANCH_CODE IN ({3}) 
                                                        GROUP BY  A.EMPLOYEE_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                                        Union All   
                                                        SELECT A.COMPANY_CODE, A.EMPLOYEE_CODE, A.BRANCH_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.{4},0)) CREDIT_VALUE  FROM SA_CREDIT_NOTE A   
                                                        WHERE A.DELETED_FLAG = 'N'   
                                                        AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                        AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                        AND A.COMPANY_CODE IN ({2})
                                                        AND A.BRANCH_CODE IN ({3}) 
                                                        GROUP BY A.EMPLOYEE_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                                        ) A   
                                                        WHERE 1 = 1   
                                                        GROUP BY   A.EMPLOYEE_CODE, A.COMPANY_CODE  , A.BRANCH_CODE  
                                                        ) A  
                                                        )                        
                                                         PIVOT  
                                                         (SUM(NET_SALES_QTY) QTY, SUM(NET_SALES_VALUE) VALUE  
                                                          FOR BRANCH_CODE IN ({3}  AS Total)
                                                          )  
                                                          )                      

                                                        A, HR_EMPLOYEE_SETUP B  
                                                        WHERE  B.EMPLOYEE_CODE = A.EMP_CODE  
                                                        AND B.COMPANY_CODE = A.COMPANY_CODE  
                                                        AND A.COMPANY_CODE IN ({2}) 
                                                        ORDER BY B.MASTER_EMPLOYEE_CODE, B.EMPLOYEE_EDESC",
                                                                       filters.FromDate,
                                                                       filters.ToDate,
                                                                       companyCode,
                                                                       branchCode, rateColumn);
            var branch = _objectEntity.SqlQuery<BranchwiseEmployeeNetSalesModel>(query).ToList();
            return branch;
        }

        public List<VatSalesRegisterNewModel> GetVatSalesRegisterNew(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT BS_DATE(SALES_DATE) MITI, INVOICE_NO, PARTY_NAME, VAT_NO, 
                                         FN_CONVERT_CURRENCY((NVL(TAXABLE_SALES,0) + NVL(VAT,0)) * NVL(EXCHANGE_RATE,1), 'NRS', SALES_DATE) GROSS_SALES,  
                                        FN_CONVERT_CURRENCY(NVL(TAXABLE_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) TAXABLE_SALES,  
                                        FN_CONVERT_CURRENCY(NVL(VAT,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) VAT,  
                                        FN_CONVERT_CURRENCY(NVL(TAX_EXEMPTED_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) TAX_EXEMPTED_SALES,  
                                        FN_CONVERT_CURRENCY(NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) TOTAL_SALES,FORM_CODE, BRANCH_CODE,CREDIT_DAYS,DELETED_FLAG,SALES_DISCOUNT, MANUAL_NO  
                                        FROM V$SALES_INVOICE_REPORT3  
                                        WHERE SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                        AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                        AND COMPANY_CODE IN ({2})
                                        AND BRANCH_CODE IN ({3})  
                                        AND GROSS_SALES >= 0  
                                         ORDER BY BS_DATE(SALES_DATE), INVOICE_NO",
                                                                  filters.FromDate,
                                                                  filters.ToDate,
                                                                  companyCode,
                                                                  branchCode);
            var vatRegister = _objectEntity.SqlQuery<VatSalesRegisterNewModel>(query).ToList();
            return vatRegister;
        }
        //
        public List<VatSalesRegisterNewModel> GetVatSalesReturnRegisterNew(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT BS_DATE(SALES_DATE) MITI,NET_SALES, INVOICE_NO, PARTY_NAME, VAT_NO, FN_CONVERT_CURRENCY(NVL(GROSS_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) GROSS_SALES ,  
                                            FN_CONVERT_CURRENCY(NVL(TAXABLE_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) TAXABLE_SALES,  
                                            FN_CONVERT_CURRENCY(NVL(VAT,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) VAT,  
                                            FN_CONVERT_CURRENCY(NVL(TAX_EXEMPTED_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) TAX_EXEMPTED_SALES,  
                                            FN_CONVERT_CURRENCY(NVL(TOTAL_SALES,0) * NVL(EXCHANGE_RATE,1),'NRS', SALES_DATE) TOTAL_SALES,FORM_CODE, BRANCH_CODE,CREDIT_DAYS,DELETED_FLAG,SALES_DISCOUNT, MANUAL_NO  
                                            FROM  V$SALES_INVOICE_REPORT3   
                                            WHERE SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND COMPANY_CODE IN ({2})
                                            AND BRANCH_CODE IN ({3})   
                                            AND TAXABLE_SALES < 0  
                                             ORDER BY BS_DATE(SALES_DATE), INVOICE_NO",
                                                                  filters.FromDate,
                                                                  filters.ToDate,
                                                                  companyCode,
                                                                  branchCode);
            var vatRegister = _objectEntity.SqlQuery<VatSalesRegisterNewModel>(query).ToList();
            return vatRegister;
        }

        public List<DateWiseSalesDetailsModel> GetDateWiseSalesDetailsJewellery(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT SALES_DATE, MITI, ORDER_NO, SALES_NO, PARTY_TYPE_CODE, PARTY_TYPE_EDESC, CUSTOMER_CODE, CUSTOMER_EDESC, ADDRESS, PAN_NO, ITEM_CODE, ITEM_EDESC 
                                ,PURITY,GROSS_WEIGHT,LESS_STONE,NET_WEIGHT,WASTAGE,TOTAL_WEIGHT,AMOUNT,MAKING,STONE_WEIGHT,STONE_AMOUNT,DIAMOND_CARAT,DIAMOND_AMOUNT,REMARKS,NET_AMOUNT,VAT_AMOUNT
                                , UNIT, QUANTITY, UNIT_PRICE, TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, FREE_QTY, ROLL_QTY, EXCISE_DUTY, EXCISE_DUTYII, DIS_PER, LINE_DISCOUNT , DISCOUNT, CASH_DISCOUNT, SPECIAL_DISCOUNT, YEARLY_DISCOUNT, BILL_DISCOUNT, LUX_TAX, INSURANCE, FTD, SLET, LFT, FLT, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) +LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT ) TAXABLE_TOTAL_PRICE  
                                , CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT ) + VAT_AMOUNT) ELSE 0 END VAT_TOTAL_PRICE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT  - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + INSURANCE + FTD + SLET + LFT + FLT  - (LINE_DISCOUNT + DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) ) + VAT_AMOUNT) ELSE 0 END ) INVOICE_TOTAL_PRICE, VEHICLE_NO, DESTINATION  
                                ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, SALES_TYPE, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , UPC, CATEGORY, SHELF_LIFE,  WEIGHT, UOM, STATUS, EMPLOYEE_EDESC, FISCAL, FORM_CODE, BRAND_NAME , PAYMENT_MODE, PRIORITY_CODE   FROM (  
                                SELECT A.FORM_CODE, A.SALES_DATE, BS_DATE(A.SALES_DATE) MITI , FN_GET_SECOND_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.SALES_NO, A.SERIAL_NO) ORDER_NO, A.SALES_NO ,  
                                B.PARTY_TYPE_CODE, B.PARTY_TYPE_EDESC,F.CUSTOMER_CODE, F.CUSTOMER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO PAN_NO, C.ITEM_CODE, C.ITEM_EDESC, A.MU_CODE UNIT,
                                A.PURITY,A.GROSS_WEIGHT,A.LESS_STONE,A.NET_WEIGHT,A.WASTAGE,A.TOTAL_WEIGHT,A.AMOUNT,A.MAKING,A.STONE_WEIGHT,A.STONE_AMOUNT,A.DIAMOND_CARAT,A.DIAMOND_AMOUNT,A.REMARKS,A.VAT_AMOUNT,A.NET_AMOUNT,

                                a.QUANTITY , a.UNIT_PRICE, a.TOTAL_PRICE, A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.ROLL_QTY, A.FREE_QTY  
                                , CASE WHEN EXCISE_AMOUNT IS NOT NULL AND NVL(EXCISE_AMOUNT,0) > 0 THEN  
                                 NVL(EXCISE_AMOUNT,0)  ELSE  
                                 ROUND((NVL((SELECT SUM(NVL(X.CHARGE_AMOUNT,0)) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTY  
                                , CASE WHEN EXCISE_AMOUNTII IS NOT NULL AND EXCISE_AMOUNTII > 0 THEN  
                                 NVL(EXCISE_AMOUNTII,0)  ELSE  
                                 ROUND((NVL((SELECT SUM(NVL(X.CHARGE_AMOUNT,0)) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNTII,0)) > 0 THEN 0 ELSE 1 END FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTYII  
                                , ROUND(NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND ITEM_CODE = A.ITEM_CODE AND SERIAL_NO = A.SERIAL_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG IN ('D','C','Y','B') AND COMPANY_CODE IN ({2}))  AND APPLY_ON IN ('I')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0),2)  LINE_DISCOUNT  
                                ,(SELECT MANUAL_CALC_VALUE FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2})) AND APPLY_ON IN ('D')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE AND ROWNUM  = 1) DIS_PER  
                                , CASE WHEN DISCOUNT_AMOUNT IS NOT NULL AND DISCOUNT_AMOUNT > 0  
                                 AND ROUND(NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND ITEM_CODE = A.ITEM_CODE AND SERIAL_NO = A.SERIAL_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG IN ('D','C','Y','B') AND COMPANY_CODE IN ({2}))  AND APPLY_ON IN ('I')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0),2) = 0 THEN  
                                 NVL(DISCOUNT_AMOUNT,0)  ELSE  
                                 ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2})) AND APPLY_ON IN ('D')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2)  END DISCOUNT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2})) AND APPLY_ON IN ('D')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) CASH_DISCOUNT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2})) AND APPLY_ON IN ('D')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2})) AND APPLY_ON IN ('D')  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) BILL_DISCOUNT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LUX_TAX  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'I' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) INSURANCE  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'W' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FTD  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'X' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) SLET  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'R' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) LFT  
                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Z' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * CALC_TOTAL_PRICE,2) FLT  
                                , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = '01') AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                 NVL(VAT_AMOUNT,0)  ELSE  
                                ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                WHERE X.REFERENCE_NO = A.SALES_NO AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(CALC_TOTAL_PRICE) = 0 THEN 1 ELSE SUM(CALC_TOTAL_PRICE) END  FROM SA_SALES_INVOICE_JEWEL WHERE SALES_NO = A.SALES_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE  
                                ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION  
                                ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO  
                                ,E.SALES_TYPE_EDESC SALES_TYPE, FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS  
                                ,G.ITEM_SIZE UPC, G.TYPE CATEGORY, G.GRADE SHELF_LIFE,  G.SIZE_LENGHT WEIGHT, G.THICKNESS UOM, G.REMARKS STATUS, (SELECT EMPLOYEE_EDESC FROM HR_EMPLOYEE_SETUP WHERE EMPLOYEE_CODE = A.EMPLOYEE_CODE AND COMPANY_CODE = A.COMPANY_CODE) EMPLOYEE_EDESC, (SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL, g.BRAND_NAME, A.PAYMENT_MODE, (SELECT PRIORITY_EDESC FROM IP_PRIORITY_CODE WHERE PRIORITY_CODE = A.PRIORITY_CODE AND COMPANY_CODE = A.COMPANY_CODE)  PRIORITY_CODE  
                                FROM SA_SALES_INVOICE_JEWEL A, IP_PARTY_TYPE_CODE B, IP_ITEM_MASTER_SETUP C, SHIPPING_TRANSACTION D, SA_SALES_TYPE E, SA_CUSTOMER_SETUP F, IP_ITEM_SPEC_SETUP G  
                                WHERE A.PARTY_TYPE_CODE = B.PARTY_TYPE_CODE (+)  
                                AND A.COMPANY_CODE = B.COMPANY_CODE (+)  
                                AND A.ITEM_CODE = C.ITEM_CODE  
                                AND A.COMPANY_CODE = C.COMPANY_CODE  
                                AND A.CUSTOMER_CODE = F.CUSTOMER_CODE  
                                AND A.COMPANY_CODE = F.COMPANY_CODE  
                                AND A.SALES_NO = D.VOUCHER_NO (+)  
                                AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                AND A.SALES_TYPE_CODE = E.SALES_TYPE_CODE (+)  
                                AND A.COMPANY_CODE = E.COMPANY_CODE (+)  
                                AND C.ITEM_CODE = G.ITEM_CODE (+)  
                                AND C.COMPANY_CODE = G.COMPANY_CODE (+)  
                                AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                AND A.COMPANY_CODE IN ({2})
                                AND A.BRANCH_CODE IN ({3})
                                AND A.DELETED_FLAG = 'N'  
                                ORDER BY SALES_NO, A.SERIAL_NO  
                                )  
                                ORDER BY SALES_NO",
                                            filters.FromDate,
                                            filters.ToDate,
                                            companyCode,
                                            branchCode);
            var dateWiswSales = _objectEntity.SqlQuery<DateWiseSalesDetailsModel>(query).ToList();
            return dateWiswSales;
        }


        public List<VatPurchaseRegisterNewModel> GetVatPurchaseRegisterNew(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();

            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";

            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT INVOICE_DATE, MITI, INVOICE_NO,SUPPLIER_INV_NO, SUPPLIER_INV_DATE, PP_NO, SUPPLIER_CODE, SUPPLIER_EDESC, TPIN_VAT_NO, P_TYPE, VEHICLE_PUR_FLAG, FORM_CODE, DELETED_FLAG, ACC_INT_VOUCHER , SUM(INVOICE_TOTAL_PRICE) INVOICE_TOTAL_PRICE, SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE  
                                            ,SUM(TAXFREE_PURCHASE) TAXFREE_PURCHASE  
                                            ,SUM(TAXABLE_PURCHASE) TAXABLE_PURCHASE  
                                            ,SUM(TAXABLE_VAT) TAXABLE_VAT  
                                            ,SUM(TAXABLE_PURCHASE_IMPORT) TAXABLE_PURCHASE_IMPORT  
                                            ,SUM(TAXABLE_VAT_IMPORT) TAXABLE_VAT_IMPORT  
                                            ,SUM(TAX_PUR_IMP_CAP) TAX_PUR_IMP_CAP  
                                            ,SUM(TAX_VAT_IMP_CAP) TAX_VAT_IMP_CAP  
                                            FROM (  
                                            SELECT INVOICE_DATE, MITI, INVOICE_NO,SUPPLIER_INV_NO, SUPPLIER_INV_DATE, PP_NO, SUPPLIER_CODE, SUPPLIER_EDESC, TPIN_VAT_NO, P_TYPE, VEHICLE_PUR_FLAG, FORM_CODE, DELETED_FLAG , INVOICE_TOTAL_PRICE, VAT_TOTAL_PRICE  
                                            ,CASE WHEN VAT_TOTAL_PRICE = 0 THEN INVOICE_TOTAL_PRICE ELSE 0 END TAXFREE_PURCHASE  
                                            ,CASE WHEN (P_TYPE = 'LOC' Or P_TYPE = 'ADM' Or P_TYPE = 'CLE') AND VAT_TOTAL_PRICE > 0 THEN INVOICE_TOTAL_PRICE ELSE 0 END  TAXABLE_PURCHASE  
                                            ,CASE WHEN (P_TYPE = 'LOC' Or P_TYPE = 'ADM' Or P_TYPE = 'CLE') AND VAT_TOTAL_PRICE > 0 THEN VAT_TOTAL_PRICE ELSE 0 END  TAXABLE_VAT  
                                            ,CASE WHEN (P_TYPE = 'IMP' ) AND VAT_TOTAL_PRICE > 0 THEN INVOICE_TOTAL_PRICE ELSE 0 END  TAXABLE_PURCHASE_IMPORT  
                                            ,CASE WHEN (P_TYPE = 'IMP' ) AND VAT_TOTAL_PRICE > 0 THEN VAT_TOTAL_PRICE ELSE 0 END  TAXABLE_VAT_IMPORT  
                                            ,CASE WHEN (P_TYPE = 'CPE' ) AND VAT_TOTAL_PRICE > 0 THEN INVOICE_TOTAL_PRICE ELSE 0 END  TAX_PUR_IMP_CAP  
                                            ,CASE WHEN (P_TYPE = 'CPE' ) AND VAT_TOTAL_PRICE > 0 THEN VAT_TOTAL_PRICE ELSE 0 END  TAX_VAT_IMP_CAP  
                                            ,ACC_INT_VOUCHER  
                                            FROM (  
                                            SELECT INVOICE_DATE, MITI, INVOICE_NO,SUPPLIER_INV_NO, SUPPLIER_INV_DATE, PP_NO, SUPPLIER_CODE, SUPPLIER_EDESC, TPIN_VAT_NO, P_TYPE  
                                            ,  TOTAL_PRICE * EXCHANGE_RATE TOTAL_PUR_VALUE, EXCISE_DUTY, EXCISE_DUTYII, DISCOUNT, CASH_DISCOUNT, SPECIAL_DISCOUNT, YEARLY_DISCOUNT,BILL_DISCOUNT, LUX_TAX , INSURANCE, ADD_CHARGE, ((TOTAL_PRICE * EXCHANGE_RATE)+ EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) +LUX_TAX + INSURANCE + ADD_CHARGE ) TAXABLE_TOTAL_PRICE  
                                            , CASE WHEN VAT_TOTAL_PRICE > 0 AND UNIT_PRICE > 0  THEN ROUND(((TOTAL_PRICE * EXCHANGE_RATE)+ EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + LUX_TAX + INSURANCE + ADD_CHARGE )  * (NON_VAT* .13) ,2) WHEN VAT_TOTAL_PRICE > 0 AND UNIT_PRICE = 0 THEN VAT_TOTAL_PRICE  ELSE 0 END VAT_TOTAL_PRICE  
                                            , ((TOTAL_PRICE * EXCHANGE_RATE) + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + INSURANCE + ADD_CHARGE - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) ) INVOICE_TOTAL_PRICE, VEHICLE_PUR_FLAG, FORM_CODE, DELETED_FLAG, ACC_INT_VOUCHER  
                                            FROM (  
                                            SELECT A.FORM_CODE, A.INVOICE_DATE, BS_DATE(A.INVOICE_DATE) MITI , A.PP_NO, A.SUPPLIER_INV_NO, A.SUPPLIER_INV_DATE, A.INVOICE_NO ,  
                                            F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO,A.P_TYPE,  
                                             a.UNIT_PRICE, a.TOTAL_PRICE, A.EXCHANGE_RATE, A.CURRENCY_CODE, A.TOTAL_PRICE * A.EXCHANGE_RATE TOTAL_IN_NRS, A.CALC_TOTAL_PRICE * A.EXCHANGE_RATE LANDED_IN_NRS  
                                            , CASE WHEN EXCISE_AMOUNT IS NOT NULL AND EXCISE_AMOUNT > 0 THEN  
                                             NVL(EXCISE_AMOUNT,0)  ELSE  
                                             ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTY  
                                            , CASE WHEN EXCISE_AMOUNTII IS NOT NULL AND EXCISE_AMOUNTII > 0 THEN  
                                             NVL(EXCISE_AMOUNTII,0)  ELSE  
                                             ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNTII,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTYII  
                                            , CASE WHEN DISCOUNT_AMOUNT IS NOT NULL AND DISCOUNT_AMOUNT > 0 THEN  
                                            NVL(DISCOUNT_AMOUNT,0)  ELSE  
                                            ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT * X.EXCHANGE_RATE) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(DISCOUNT_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT* X.EXCHANGE_RATE FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) CASH_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) BILL_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) LUX_TAX  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'I' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) INSURANCE  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE  FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'A' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) ADD_CHARGE  
                                            , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN  (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = A.COMPANY_CODE) AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                            NVL(VAT_AMOUNT,0)  ELSE  
                                            ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = X.COMPANY_CODE)  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE * EXCHANGE_RATE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE * EXCHANGE_RATE),2) END VAT_TOTAL_PRICE, 'N' VEHICLE_PUR_FLAG, A.DELETED_FLAG,  DECODE(C.NON_VAT_FLAG,'N',1,0) NON_VAT   
                                            ,(SELECT LISTAGG (ACC_EDESC,',') WITHIN GROUP (ORDER BY ACC_EDESC) FROM FA_DOUBLE_VOUCHER X, FA_CHART_OF_ACCOUNTS_SETUP Y WHERE (X.MANUAL_NO =A.INVOICE_NO OR X.VOUCHER_NO =A.INVOICE_NO) AND X.COMPANY_CODE = A.COMPANY_CODE AND X.TRANSACTION_TYPE = 'DR' AND X.COMPANY_CODE = Y.COMPANY_CODE AND X.ACC_CODE = Y.ACC_CODE AND Y.ACC_NATURE <> 'LB'  )  ACC_INT_VOUCHER FROM IP_PURCHASE_INVOICE A, IP_SUPPLIER_SETUP F, IP_ITEM_MASTER_SETUP C  
                                            Where A.SUPPLIER_CODE = F.SUPPLIER_CODE  
                                            AND A.COMPANY_CODE = F.COMPANY_CODE  
                                            AND A.ITEM_CODE = C.ITEM_CODE  
                                            AND A.COMPANY_CODE = C.COMPANY_CODE  
                                            AND A.DELETED_FLAG = 'N' 
                                            AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            AND (INVOICE_NO,P_TYPE) NOT IN (SELECT REFERENCE_NO,'IMP' FROM FINANCIAL_REFERENCE_DETAIL V WHERE COMPANY_CODE IN ({2})  
                                            AND FORM_CODE IN (SELECT FORM_CODE FROM FORM_SETUP WHERE PURCHASE_EXPENSES_FLAG = 'Y' AND COMPANY_CODE IN ({2}))  
                                            AND VOUCHER_NO IN (SELECT VOUCHER_NO FROM FA_DOUBLE_VOUCHER R,  FA_CHART_OF_ACCOUNTS_SETUP S WHERE R.TRANSACTION_TYPE = 'DR'  
                                            AND R.VOUCHER_NO =  V.VOUCHER_NO AND R.ACC_CODE = S.ACC_CODE AND S.ACC_NATURE IN ('LB','PV')  
                                            AND R.COMPANY_CODE= '01' ))  
                                            Union All  
                                            SELECT A.FORM_CODE, A.INVOICE_DATE INVOICE_DATE, BS_DATE(A.INVOICE_DATE) MITI , A.PP_NO, A.MANUAL_NO SUPPLIER_INV_NO, A.INVOICE_DATE SUPPLIER_INV_DATE, A.INVOICE_NO ,  
                                            F.SUPPLIER_CODE, CASE WHEN P_TYPE = 'CLE' THEN (SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE COMPANY_CODE IN ({2}) AND CUSTOMER_CODE = A.CS_CODE) ELSE F.SUPPLIER_EDESC END SUPPLIER_EDESC, CASE WHEN P_TYPE = 'CLE' THEN (SELECT REGD_OFFICE_EADDRESS FROM SA_CUSTOMER_SETUP WHERE COMPANY_CODE = '01' AND CUSTOMER_CODE = A.CS_CODE) ELSE F.REGD_OFFICE_EADDRESS END ADDRESS, CASE WHEN P_TYPE = 'CLE' THEN (SELECT TPIN_VAT_NO FROM SA_CUSTOMER_SETUP WHERE COMPANY_CODE = '01' AND CUSTOMER_CODE = A.CS_CODE) ELSE F.TPIN_VAT_NO END TPIN_VAT_NO , A.P_TYPE,  
                                             0 UNIT_PRICE, TAXABLE_AMOUNT, A.EXCHANGE_RATE, A.CURRENCY_CODE, TAXABLE_AMOUNT TOTAL_IN_NRS, TAXABLE_AMOUNT LANDED_IN_NRS  
                                            , 0 EXCISE_DUTY, 0 EXCISE_DUTYII  
                                            , 0 DISCOUNT  
                                            ,0 CASH_DISCOUNT  
                                            ,0 SPECIAL_DISCOUNT  
                                            ,0 YEARLY_DISCOUNT  
                                            ,0  BILL_DISCOUNT  
                                            ,0 LUX_TAX, 0 INSURANCE, 0 ADD_CHARGE  
                                            , VAT_AMOUNT VAT_TOTAL_PRICE, VEHICLE_PUR_FLAG, A.DELETED_FLAG, 1 NON_VAT  
                                            ,(SELECT LISTAGG (ACC_EDESC,',') WITHIN GROUP (ORDER BY ACC_EDESC) FROM FA_DOUBLE_VOUCHER X, FA_CHART_OF_ACCOUNTS_SETUP Y WHERE X.VOUCHER_NO =A.INVOICE_NO AND X.COMPANY_CODE = A.COMPANY_CODE AND X.TRANSACTION_TYPE = 'DR' AND X.COMPANY_CODE = Y.COMPANY_CODE AND X.ACC_CODE = Y.ACC_CODE AND Y.ACC_NATURE <> 'LB'  )  ACC_INT_VOUCHER FROM FA_DC_VAT_INVOICE  A, IP_SUPPLIER_SETUP F  
                                            Where a.CS_CODE = f.SUPPLIER_CODE (+)  
                                            AND A.DOC_TYPE = 'P'  
                                            AND A.FORM_CODE NOT IN (SELECT FORM_CODE FROM FA_PAY_ORDER WHERE COMPANY_CODE IN ({2}))  
                                            AND A.FORM_CODE NOT IN (SELECT FORM_CODE FROM FA_JOB_ORDER WHERE COMPANY_CODE IN ({2})) AND A.COMPANY_CODE = F.COMPANY_CODE (+)  
                                              AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                              AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                              AND A.COMPANY_CODE IN ({2})
                                              AND A.BRANCH_CODE IN ({3}) 
                                            AND A.TAXABLE_AMOUNT >= 0  
                                            AND A.DELETED_FLAG = 'N'  
                                            ORDER BY INVOICE_NO  
                                            ) )  
                                            ) GROUP BY invoice_date , MITI, INVOICE_NO, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, PP_NO, SUPPLIER_CODE, SUPPLIER_EDESC, TPIN_VAT_NO, P_TYPE, VEHICLE_PUR_FLAG, FORM_CODE, DELETED_FLAG,ACC_INT_VOUCHER  
                                            ORDER BY MITI, INVOICE_NO", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var vatPurchase = _objectEntity.SqlQuery<VatPurchaseRegisterNewModel>(query).ToList();
            return vatPurchase;
        }

        public List<VatPurchaseRegisterNewModel> GetVatPurchaseReturnRegisterNew(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();

            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";

            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT INVOICE_DATE, MITI, INVOICE_NO,SUPPLIER_INV_NO, SUPPLIER_INV_DATE, PP_NO, SUPPLIER_CODE, SUPPLIER_EDESC, TPIN_VAT_NO, P_TYPE, VEHICLE_PUR_FLAG, FORM_CODE, DELETED_FLAG, ACC_INT_VOUCHER  , SUM(INVOICE_TOTAL_PRICE) INVOICE_TOTAL_PRICE, SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE  
                                            ,SUM(TAXFREE_PURCHASE) TAXFREE_PURCHASE  
                                            ,SUM(TAXABLE_PURCHASE) TAXABLE_PURCHASE  
                                            ,SUM(TAXABLE_VAT) TAXABLE_VAT  
                                            ,SUM(TAXABLE_PURCHASE_IMPORT) TAXABLE_PURCHASE_IMPORT  
                                            ,SUM(TAXABLE_VAT_IMPORT) TAXABLE_VAT_IMPORT  
                                            ,SUM(TAX_PUR_IMP_CAP) TAX_PUR_IMP_CAP  
                                            ,SUM(TAX_VAT_IMP_CAP) TAX_VAT_IMP_CAP  
                                            FROM (  
                                            SELECT INVOICE_DATE, MITI, INVOICE_NO,SUPPLIER_INV_NO, SUPPLIER_INV_DATE, PP_NO, SUPPLIER_CODE, SUPPLIER_EDESC, TPIN_VAT_NO, P_TYPE, VEHICLE_PUR_FLAG, FORM_CODE, INVOICE_TOTAL_PRICE, VAT_TOTAL_PRICE, DELETED_FLAG, ACC_INT_VOUCHER   
                                            ,CASE WHEN VAT_TOTAL_PRICE = 0 THEN INVOICE_TOTAL_PRICE ELSE 0 END TAXFREE_PURCHASE  
                                            ,CASE WHEN (P_TYPE = 'LOC' Or P_TYPE = 'ADM' Or P_TYPE = 'CLE') AND VAT_TOTAL_PRICE > 0 THEN INVOICE_TOTAL_PRICE ELSE 0 END  TAXABLE_PURCHASE  
                                            ,CASE WHEN (P_TYPE = 'LOC' Or P_TYPE = 'ADM' Or P_TYPE = 'CLE') AND VAT_TOTAL_PRICE > 0 THEN VAT_TOTAL_PRICE ELSE 0 END  TAXABLE_VAT  
                                            ,CASE WHEN (P_TYPE = 'IMP' ) AND VAT_TOTAL_PRICE > 0 THEN INVOICE_TOTAL_PRICE ELSE 0 END  TAXABLE_PURCHASE_IMPORT  
                                            ,CASE WHEN (P_TYPE = 'IMP' ) AND VAT_TOTAL_PRICE > 0 THEN VAT_TOTAL_PRICE ELSE 0 END  TAXABLE_VAT_IMPORT  
                                            ,CASE WHEN (P_TYPE = 'CPE' ) AND VAT_TOTAL_PRICE > 0 THEN INVOICE_TOTAL_PRICE ELSE 0 END  TAX_PUR_IMP_CAP  
                                            ,CASE WHEN (P_TYPE = 'CPE' ) AND VAT_TOTAL_PRICE > 0 THEN VAT_TOTAL_PRICE ELSE 0 END  TAX_VAT_IMP_CAP  
                                            FROM (  
                                            SELECT INVOICE_DATE, MITI, INVOICE_NO,SUPPLIER_INV_NO, SUPPLIER_INV_DATE, PP_NO, SUPPLIER_CODE, SUPPLIER_EDESC, TPIN_VAT_NO, P_TYPE  
                                            , TOTAL_PRICE * EXCHANGE_RATE TOTAL_PUR_VALUE, EXCISE_DUTY, EXCISE_DUTYII, DISCOUNT, CASH_DISCOUNT, SPECIAL_DISCOUNT, YEARLY_DISCOUNT,BILL_DISCOUNT, LUX_TAX + INSURANCE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) +LUX_TAX + INSURANCE ) TAXABLE_TOTAL_PRICE  
                                            , CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + LUX_TAX + INSURANCE )  * .13,2) ELSE 0 END VAT_TOTAL_PRICE  
                                            , (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + INSURANCE - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT)) INVOICE_TOTAL_PRICE, VEHICLE_PUR_FLAG, FORM_CODE, DELETED_FLAG, ACC_INT_VOUCHER   
                                            FROM (  
                                            SELECT A.FORM_CODE, A.RETURN_DATE INVOICE_DATE, BS_DATE(A.RETURN_DATE) MITI , A.PP_NO, A.SUPPLIER_INV_NO, A.SUPPLIER_INV_DATE, A.RETURN_NO INVOICE_NO ,  
                                            F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO,A.P_TYPE,  
                                             a.UNIT_PRICE, a.TOTAL_PRICE, A.EXCHANGE_RATE, A.CURRENCY_CODE, A.TOTAL_PRICE * A.EXCHANGE_RATE TOTAL_IN_NRS, A.CALC_TOTAL_PRICE * A.EXCHANGE_RATE LANDED_IN_NRS  
                                            , CASE WHEN EXCISE_AMOUNT IS NOT NULL AND EXCISE_AMOUNT > 0 THEN  
                                             NVL(EXCISE_AMOUNT,0)  ELSE  
                                             ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E'  AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTY  
                                            , CASE WHEN EXCISE_AMOUNTII IS NOT NULL AND EXCISE_AMOUNTII > 0 THEN  
                                             NVL(EXCISE_AMOUNTII,0)  ELSE  
                                             ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNTII,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTYII  
                                            , CASE WHEN DISCOUNT_AMOUNT IS NOT NULL AND DISCOUNT_AMOUNT > 0 THEN  
                                            NVL(DISCOUNT_AMOUNT,0)  ELSE  
                                            ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(DISCOUNT_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE) END DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) CASH_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) BILL_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) LUX_TAX  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'I' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) INSURANCE  
                                            , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN  (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = A.COMPANY_CODE) AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                            NVL(VAT_AMOUNT,0)  ELSE  
                                            ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = X.COMPANY_CODE)  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE, 'N' VEHICLE_PUR_FLAG, A.DELETED_FLAG  
                                            ,(SELECT LISTAGG (ACC_EDESC,',') WITHIN GROUP (ORDER BY ACC_EDESC) FROM FA_DOUBLE_VOUCHER X, FA_CHART_OF_ACCOUNTS_SETUP Y WHERE (X.MANUAL_NO =A.RETURN_NO OR X.VOUCHER_NO =A.RETURN_NO) AND X.COMPANY_CODE = A.COMPANY_CODE AND X.TRANSACTION_TYPE = 'CR' AND X.COMPANY_CODE = Y.COMPANY_CODE AND X.ACC_CODE = Y.ACC_CODE AND Y.ACC_NATURE <> 'LB'  )  ACC_INT_VOUCHER  
                                            FROM IP_PURCHASE_RETURN A, IP_SUPPLIER_SETUP F  
                                            Where A.SUPPLIER_CODE = F.SUPPLIER_CODE  
                                            AND A.COMPANY_CODE = F.COMPANY_CODE  
                                            AND A.COMPANY_CODE IN ({2})  
                                            AND A.BRANCH_CODE IN ({3}) 
                                            AND A.DELETED_FLAG = 'N'  
                                            AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND RETURN_NO NOT IN (SELECT REFERENCE_NO FROM FINANCIAL_REFERENCE_DETAIL V WHERE COMPANY_CODE IN ({2}) 
                                            AND FORM_CODE IN (SELECT FORM_CODE FROM FORM_SETUP WHERE PURCHASE_EXPENSES_FLAG = 'Y' AND COMPANY_CODE IN ({2}))  
                                            AND VOUCHER_NO IN (SELECT VOUCHER_NO FROM FA_DOUBLE_VOUCHER R,  FA_CHART_OF_ACCOUNTS_SETUP S WHERE R.TRANSACTION_TYPE = 'DR'  
                                            AND R.VOUCHER_NO =  V.VOUCHER_NO AND R.COMPANY_CODE = V.COMPANY_CODE AND R.ACC_CODE = S.ACC_CODE AND S.ACC_NATURE IN ('LB','PV')  
                                            AND R.COMPANY_CODE IN ({2}) ))  
                                            Union All  
                                            SELECT A.FORM_CODE, A.INVOICE_DATE INVOICE_DATE, BS_DATE(A.INVOICE_DATE) MITI , A.PP_NO, A.MANUAL_NO SUPPLIER_INV_NO, A.INVOICE_DATE SUPPLIER_INV_DATE, A.INVOICE_NO ,  
                                            F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO,A.P_TYPE,  
                                             0 UNIT_PRICE, ABS(TAXABLE_AMOUNT) TAXABLE_AMOUNT, A.EXCHANGE_RATE, A.CURRENCY_CODE, ABS(TAXABLE_AMOUNT) TOTAL_IN_NRS, ABS(TAXABLE_AMOUNT) LANDED_IN_NRS  
                                            , 0 EXCISE_DUTY, 0 EXCISE_DUTYII  
                                            , 0 DISCOUNT  
                                            ,0 CASH_DISCOUNT  
                                            ,0 SPECIAL_DISCOUNT  
                                            ,0 YEARLY_DISCOUNT  
                                            ,0  BILL_DISCOUNT  
                                            ,0 LUX_TAX, 0 INSURANCE  
                                            , ABS(VAT_AMOUNT) VAT_TOTAL_PRICE, VEHICLE_PUR_FLAG, A.DELETED_FLAG  
                                            ,(SELECT LISTAGG (ACC_EDESC,',') WITHIN GROUP (ORDER BY ACC_EDESC) FROM FA_DOUBLE_VOUCHER X, FA_CHART_OF_ACCOUNTS_SETUP Y WHERE X.VOUCHER_NO =A.INVOICE_NO AND X.COMPANY_CODE = A.COMPANY_CODE AND X.TRANSACTION_TYPE = 'CR' AND X.COMPANY_CODE = Y.COMPANY_CODE AND X.ACC_CODE = Y.ACC_CODE AND Y.ACC_NATURE <> 'LB'  )  ACC_INT_VOUCHER  
                                            FROM FA_DC_VAT_INVOICE  A, IP_SUPPLIER_SETUP F  
                                            Where a.CS_CODE = f.SUPPLIER_CODE  
                                            AND A.DOC_TYPE = 'P'  
                                            AND A.FORM_CODE NOT IN (SELECT FORM_CODE FROM FA_PAY_ORDER WHERE COMPANY_CODE IN ({2}))  
                                            AND A.FORM_CODE NOT IN (SELECT FORM_CODE FROM FA_JOB_ORDER WHERE COMPANY_CODE IN ({2}))  
                                            AND A.COMPANY_CODE = F.COMPANY_CODE  
                                            AND A.COMPANY_CODE IN ({2})  
                                            AND A.BRANCH_CODE IN ({3})  
                                            AND A.TAXABLE_AMOUNT < 0  
                                            AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')   
                                            AND A.DELETED_FLAG = 'N'  
                                            ORDER BY INVOICE_NO  
                                            ) )  
                                            ) GROUP BY invoice_date , MITI, INVOICE_NO, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, PP_NO, SUPPLIER_CODE, SUPPLIER_EDESC, TPIN_VAT_NO, P_TYPE, VEHICLE_PUR_FLAG, FORM_CODE, DELETED_FLAG, ACC_INT_VOUCHER   
                                            ORDER BY MITI, INVOICE_NO", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var vatPurchaseReturn = _objectEntity.SqlQuery<VatPurchaseRegisterNewModel>(query).ToList();
            return vatPurchaseReturn;
        }

        public dynamic GetAllItems(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";
            string query = string.Format(@"
                SELECT ITEM_CODE, ITEM_EDESC 
                FROM IP_ITEM_MASTER_SETUP 
                WHERE DELETED_FLAG = 'N' 
                  AND COMPANY_CODE IN ({0})  
                  AND BRANCH_CODE IN ({1})", companyCode, branchCode);

            var item = _objectEntity.SqlQuery<Item>(query).ToList();
            return item;
        }

        public List<DatewiseNetSalesModels> GetDatewiseNetSalesReport(ReportFiltersModel filters, User userinfo)
        {
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT SALES_DATE ITEM_EDESC, BS_DATE(SALES_DATE) INDEX_MU_CODE, SALES_QTY, SALES_VALUE, SALES_RET_QTY, SALES_RET_VALUE, DEBIT_VALUE, CREDIT_VALUE,  FREE_QTY, SALES_QTY - SALES_RET_QTY + FREE_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE FROM (  
                                                            SELECT A.SALES_DATE,  SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE, SUM(FREE_QTY) FREE_QTY FROM (  
                                                            SELECT A.SALES_DATE, SUM(QUANTITY) SALES_QTY, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, NVL(SUM(NVL(A.FREE_QTY,0)),0) FREE_QTY  FROM SA_SALES_INVOICE A  
                                                            WHERE A.DELETED_FLAG = 'N'  
                                                            AND TRUNC(A.SALES_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                            AND TRUNC(A.SALES_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                            AND A.COMPANY_CODE IN ({2})
                                                            AND A.BRANCH_CODE IN ({3}) 
                                                            GROUP BY  SALES_DATE    
                                                            Union All  
                                                            SELECT A.RETURN_DATE SALES_DATE,  0 SALES_QTY, 0 SALES_VALUE, SUM(QUANTITY)  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, NVL(SUM(NVL(A.FREE_QTY * -1,0)),0) FREE_QTY  FROM SA_SALES_RETURN A  
                                                            WHERE A.DELETED_FLAG = 'N'  
                                                            AND TRUNC(A.RETURN_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                            AND TRUNC(A.RETURN_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                            AND A.COMPANY_CODE IN ({2})
                                                            AND A.BRANCH_CODE IN ({3}) 
                                                              GROUP BY  RETURN_DATE    
                                                            Union All  
                                                            SELECT A.VOUCHER_DATE SALES_DATE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE, 0 FREE_QTY  FROM SA_DEBIT_NOTE A  
                                                            WHERE A.DELETED_FLAG = 'N'  
                                                            AND TRUNC(A.VOUCHER_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                            AND TRUNC(A.VOUCHER_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                            AND A.COMPANY_CODE IN ({2})
                                                            AND A.BRANCH_CODE IN ({3}) 
                                                            GROUP BY A.VOUCHER_DATE  
                                                            Union All  
                                                            SELECT A.VOUCHER_DATE SALES_DATE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE, 0 FREE_QTY  FROM SA_CREDIT_NOTE A  
                                                            WHERE A.DELETED_FLAG = 'N'  
                                                            AND TRUNC(A.VOUCHER_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                            AND TRUNC(A.VOUCHER_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                            AND A.COMPANY_CODE IN ({2})
                                                            AND A.BRANCH_CODE IN ({3})  
                                                            GROUP BY  A.VOUCHER_DATE  
                                                            ORDER BY SALES_DATE) A   
                                                            GROUP BY SALES_DATE  
                                                            ) ORDER BY SALES_DATE ", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var datewise = _objectEntity.SqlQuery<DatewiseNetSalesModels>(query).ToList();
            return datewise;
        }

        public List<ProductwiseNetSalesModel> GetProductwiseNetSalesReport(ReportFiltersModel filters, User userinfo)
        {
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                              : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@" SELECT ITEM_EDESC,  INDEX_MU_CODE, SALES_ROLL_QTY, SALES_QTY, SALES_VALUE, DEBIT_VALUE, CREDIT_VALUE, FREE_QTY, SALES_RET_ROLL_QTY, SALES_RET_QTY, SALES_RET_VALUE, SALES_ROLL_QTY - SALES_RET_ROLL_QTY NET_ROLL_QTY , SALES_QTY - SALES_RET_QTY + FREE_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE FROM (  
                                            SELECT D.ITEM_EDESC, INDEX_MU_CODE INDEX_MU_CODE, SUM(A.SALES_ROLL_QTY) SALES_ROLL_QTY, SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(SALES_RET_ROLL_QTY) SALES_RET_ROLL_QTY, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE, SUM(FREE_QTY) FREE_QTY  FROM (  
                                            SELECT  A.ITEM_CODE, SUM(NVL(A.ROLL_QTY,0)) SALES_ROLL_QTY, SUM(NVL(QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_ROLL_QTY, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, NVL(SUM(NVL(A.FREE_QTY,0)),0) FREE_QTY   FROM SA_SALES_INVOICE A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND TRUNC(A.SALES_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND TRUNC(A.SALES_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})   
                                            GROUP BY  A.ITEM_CODE  
                                            Union All  
                                            SELECT A.ITEM_CODE, 0 SALES_ROLL_QTY, 0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.ROLL_QTY,0)) SALES_RET_ROLL_QTY, SUM(NVL(QUANTITY,0))  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE , NVL(SUM(NVL(A.FREE_QTY * -1,0)),0) FREE_QTY  FROM SA_SALES_RETURN A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND TRUNC(A.RETURN_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND TRUNC(A.RETURN_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.ITEM_CODE  
                                            Union All 
                                            SELECT A.ITEM_CODE, 0 SALES_ROLL_QTY, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_ROLL_QTY, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE, 0 FREE_QTY   FROM SA_DEBIT_NOTE A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND TRUNC(A.VOUCHER_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND TRUNC(A.VOUCHER_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3}) 
                                            GROUP BY  A.ITEM_CODE  
                                            Union All  
                                            SELECT A.ITEM_CODE, 0 SALES_ROLL_QTY, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_ROLL_QTY, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE, 0 FREE_QTY   FROM SA_CREDIT_NOTE A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND TRUNC(A.VOUCHER_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND TRUNC(A.VOUCHER_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY A.ITEM_CODE  
                                            ORDER BY 1 ) A,  IP_ITEM_MASTER_SETUP D  
                                            Where  A.ITEM_CODE = D.ITEM_CODE  
                                            AND D.COMPANY_CODE = '01'   
                                            GROUP BY  A.ITEM_CODE,  D.ITEM_EDESC, D.INDEX_MU_CODE   
                                            ) ORDER BY ITEM_EDESC", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var datewise = _objectEntity.SqlQuery<ProductwiseNetSalesModel>(query).ToList();
            return datewise;
        }

        public dynamic GetBranchwiseNetSalesReport(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";


            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";


            // --- Form Code Filter (works at both root or inside filters) ---
            var formCodeFilter = filters?.FormCodeFilter as IEnumerable<object>
                                 ?? data?.FormCodeFilter as IEnumerable<object>;

            var rateColumn = formCodeFilter != null && formCodeFilter.Any()
                ? formCodeFilter.FirstOrDefault()?.ToString()
                : null;
            if (string.IsNullOrEmpty(rateColumn))
            {
                rateColumn = "NET_GROSS_RATE"; // Set a default if it's missing
            }
            List<string> branchCodes = new List<string> { branchCode };

            // Generate the pivot aliases like: '01.01' AS "BRANCH_01_01" 

            //var pivotInClause = string.Join(", ", branchCodes.Select(b => $"{b} AS \"BRANCH_{b.Replace(".", "_")}\""));
            var pivotInClause = string.Join(", ", branchCodes.Select(b => $"{b} AS \"BRANCH\""));
            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");


            string query = string.Format(@"SELECT B.ITEM_CODE, B.ITEM_EDESC, B.MASTER_ITEM_CODE , B.INDEX_MU_CODE INDEX_MU_CODE, B.PRE_ITEM_CODE , B.GROUP_SKU_FLAG , (LENGTH(B.MASTER_ITEM_CODE) - LENGTH(REPLACE(B.MASTER_ITEM_CODE,'.',''))) ROWLEV,  A.* FROM (  
                                            SELECT * FROM (  
                                            SELECT A.COMPANY_CODE, ITEM_CODE IT_CODE, BRANCH_CODE,   
                                            SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE, FREE_QTY FROM (   
                                            SELECT A.BRANCH_CODE ,A.COMPANY_CODE, A.ITEM_CODE, SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE, SUM(NVL(FREE_QTY,0)) FREE_QTY  FROM (   
                                            SELECT A.COMPANY_CODE, A.ITEM_CODE, A.BRANCH_CODE , SUM(NVL(A.QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY*A.{5},0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, SUM(NVL(FREE_QTY,0)) FREE_QTY   FROM SA_SALES_INVOICE A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})
                                            GROUP BY  A.ITEM_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                            Union All   
                                            SELECT A.COMPANY_CODE, A.ITEM_CODE, A.BRANCH_CODE, 0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY,0))  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.{5},0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, SUM(NVL(FREE_QTY,0))*-1 FREE_QTY   FROM SA_SALES_RETURN A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3}) 
                                            GROUP BY  A.ITEM_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                            Union All   
                                            SELECT A.COMPANY_CODE, A.ITEM_CODE, A.BRANCH_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.{5},0)) DEBIT_VALUE, 0 CREDIT_VALUE, 0 FREE_QTY  FROM SA_DEBIT_NOTE A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3}) 
                                            GROUP BY  A.ITEM_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                            Union All   
                                            SELECT A.COMPANY_CODE, A.ITEM_CODE, A.BRANCH_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.{5},0)) CREDIT_VALUE, 0 FREE_QTY  FROM SA_CREDIT_NOTE A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3}) 
                                            GROUP BY A.ITEM_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                            ) A   
                                            WHERE 1 = 1   
                                            GROUP BY   A.ITEM_CODE, A.COMPANY_CODE  , A.BRANCH_CODE  
                                            ) A  
                                            )  
                                            PIVOT (
                                                    SUM(NET_SALES_QTY) AS QTY,
                                                     SUM(NET_SALES_VALUE) AS VALUE,
                                                    SUM(FREE_QTY) AS FREE
                                                     FOR BRANCH_CODE IN ({4})
                                              ) 
                                            ) A, IP_ITEM_MASTER_SETUP B  
                                            WHERE  B.ITEM_CODE = A.IT_CODE (+)   
                                            AND B.COMPANY_CODE = A.COMPANY_CODE (+)   
                                            AND B.COMPANY_CODE IN ({2})  
                                            ORDER BY B.MASTER_ITEM_CODE, B.ITEM_EDESC", FromDate, ToDate, companyCode, branchCode, pivotInClause, rateColumn);
            var datewise = _objectEntity.SqlQuery<BranchItemStockModel>(query).ToList();
            var groupedData = datewise;

            // Set PRE_CUSTOMER_CODE = null where CUSTOMER_CODE == "1"
            foreach (var item in groupedData)
            {
                if (item.MASTER_ITEM_CODE == "01" || item.MASTER_ITEM_CODE == "02")
                {
                    item.PRE_ITEM_CODE = "";
                }

            }
            //foreach (var item in groupedData)
            //{
            //    if (item.MASTER_ITEM_CODE != null && item.MASTER_ITEM_CODE =="01" && item.MASTER_ITEM_CODE =="002") // e.g. 002, 0025
            //    {
            //        item.PRE_ITEM_CODE ="";
            //    }
            //}

            //return groupedData;

            var itemRows = groupedData.Where(r => r.GROUP_SKU_FLAG == "I").ToList();
            List<dynamic> stockData = new List<dynamic>();

            foreach (var row in groupedData)
            {
                // ... [original 40 totals]
                double totalBranchQty = 0;     // BRANCH_QTY
                double totalBranchValue = 0;   // BRANCH_VALUE
                double totalBranchFree = 0;    // BRANCH_FREE

                if (row.GROUP_SKU_FLAG == "G")
                {
                    string masterCode = row.MASTER_ITEM_CODE;
                    foreach (var z in itemRows)
                    {
                        if (!string.IsNullOrEmpty(z.MASTER_ITEM_CODE) && z.MASTER_ITEM_CODE.StartsWith(masterCode))
                        {
                            // ... [original totals aggregation]
                            totalBranchQty += z.BRANCH_QTY ?? 0;
                            totalBranchValue += z.BRANCH_VALUE ?? 0;
                            totalBranchFree += z.BRANCH_FREE ?? 0;
                        }
                    }
                }
                else
                {
                    // ... [original single row assignment]
                    totalBranchQty = row.BRANCH_QTY ?? 0;
                    totalBranchValue = row.BRANCH_VALUE ?? 0;
                    totalBranchFree = row.BRANCH_FREE ?? 0;
                }

                if (totalBranchQty != 0 || totalBranchValue != 0 || totalBranchFree != 0)
                {
                    stockData.Add(new
                    {
                        ITEM_CODE = row.ITEM_CODE,
                        ITEM_EDESC = row.ITEM_EDESC,
                        INDEX_MU_CODE = row.INDEX_MU_CODE,
                        MASTER_ITEM_CODE = row.MASTER_ITEM_CODE,
                        PRE_ITEM_CODE = row.PRE_ITEM_CODE == "00" ? " " : row.PRE_ITEM_CODE,
                        // ... [existing fields]
                        BRANCH_QTY = totalBranchQty,
                        BRANCH_VALUE = totalBranchValue,
                        BRANCH_FREE = totalBranchFree
                    });
                }
            }

            if (stockData.Any())
            {
                var grandTotal = new
                {
                    ITEM_CODE = "",
                    ITEM_EDESC = "Grand Total",
                    INDEX_MU_CODE = "",
                    MASTER_ITEM_CODE = "",
                    PRE_ITEM_CODE = "",
                    // ... [existing grand total fields]
                    BRANCH_QTY = itemRows.Sum(x => x.BRANCH_QTY ?? 0),
                    BRANCH_VALUE = itemRows.Sum(x => x.BRANCH_VALUE ?? 0),
                    BRANCH_FREE = itemRows.Sum(x => x.BRANCH_FREE ?? 0)
                };

                stockData.Add(grandTotal);
            }

            return stockData;

        }

        public dynamic GetMonthwiseProductNetSales(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            List<string> branchCodes = new List<string> { branchCode }; // Can contain more than one

            // Generate the pivot aliases like: '01.01' AS "BRANCH_01_01"

            //var pivotInClause = string.Join(", ", branchCodes.Select(b => $"{b} AS \"BRANCH_{b.Replace(".", "_")}\""));
            var pivotInClause = string.Join(", ", branchCodes.Select(b => $"{b} AS \"BRANCH\""));
            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");


            string query = string.Format(@"SELECT B.ITEM_CODE, B.ITEM_EDESC, B.MASTER_ITEM_CODE , B.PRE_ITEM_CODE, B.GROUP_SKU_FLAG , (LENGTH(B.MASTER_ITEM_CODE) - LENGTH(REPLACE(B.MASTER_ITEM_CODE,'.',''))) ROWLEV,  A.* FROM (  
                                                    SELECT * FROM (  
                                                    SELECT A.COMPANY_CODE, ITEM_CODE, ITEM_NAME, INDEX_MU_CODE, MTH,   
                                                    SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE FROM (   
                                                    SELECT MTH, A.COMPANY_CODE, A.ITEM_CODE, D.ITEM_EDESC ITEM_NAME,  D.INDEX_MU_CODE INDEX_MU_CODE , CASE WHEN E.BRAND_NAME IS NULL THEN D.ITEM_EDESC ELSE E.BRAND_NAME END BRAND_NAME,  SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE  FROM (   
                                                    SELECT A.COMPANY_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(SALES_DATE),6,2) MTH, SUM(NVL(A.QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE   FROM SA_SALES_INVOICE A   
                                                    WHERE A.DELETED_FLAG = 'N'   
                                                    AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3})   
                                                    GROUP BY  A.ITEM_CODE , A.COMPANY_CODE, SALES_DATE  
                                                    Union All   
                                                    SELECT A.COMPANY_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(RETURN_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY,0))  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE   FROM SA_SALES_RETURN A   
                                                    WHERE A.DELETED_FLAG = 'N'   
                                                    AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3})   
                                                    GROUP BY  A.ITEM_CODE , A.COMPANY_CODE, RETURN_DATE  
                                                    Union All   
                                                    SELECT A.COMPANY_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE  FROM SA_DEBIT_NOTE A   
                                                    WHERE A.DELETED_FLAG = 'N'   
                                                    AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3})
                                                    GROUP BY  A.ITEM_CODE , A.COMPANY_CODE, VOUCHER_DATE  
                                                    Union All   
                                                    SELECT A.COMPANY_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE  FROM SA_CREDIT_NOTE A   
                                                    WHERE A.DELETED_FLAG = 'N'   
                                                    AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3})
                                                    GROUP BY  A.ITEM_CODE , A.COMPANY_CODE, VOUCHER_DATE  
                                                    ) A,  IP_ITEM_MASTER_SETUP D, IP_ITEM_SPEC_SETUP E   
                                                    WHERE  D.ITEM_CODE = A.ITEM_CODE   
                                                    AND D.COMPANY_CODE = A.COMPANY_CODE  
                                                    AND D.ITEM_CODE = E.ITEM_CODE (+)  
                                                    AND D.COMPANY_CODE = E.COMPANY_CODE (+)  
                                                    AND A.COMPANY_CODE IN ({2})  
                                                    GROUP BY   A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE, E.BRAND_NAME, A.COMPANY_CODE  , MTH  
                                                    ) A   
                                                    ) 
                                                     PIVOT  
                                                        (SUM(NET_SALES_QTY) QTY, SUM(NET_SALES_VALUE) VALUE
                                                         FOR MTH IN ('04' AS S,
                                                          '05' AS B,
                                                          '06' AS A,
                                                          '07' AS K,
                                                          '08' AS M,
                                                          '09' AS P,
                                                          '10' AS Mg,
                                                          '11' AS F,
                                                          '12' AS C,
                                                          '01' AS Bh,
                                                          '02' AS J,
                                                          '03' AS Aa
                                                               )  
                                                          ) 
                                                           )   
                                                     A, IP_ITEM_MASTER_SETUP B  
                                                    WHERE  B.ITEM_CODE = A.ITEM_CODE (+)   
                                                    AND B.COMPANY_CODE = A.COMPANY_CODE (+)   
                                                    AND B.COMPANY_CODE IN ({2})  
                                                    ORDER BY B.MASTER_ITEM_CODE, B.ITEM_EDESC  
                                                    ", FromDate, ToDate, companyCode, branchCode);
            var datewise = _objectEntity.SqlQuery<MonthwiseSalesCustomerNetSales>(query).ToList();
            var groupedData = datewise;

            // Set PRE_CUSTOMER_CODE = null where CUSTOMER_CODE == "1"
            //foreach (var item in groupedData)
            //{
            //    if (item.MASTER_ITEM_CODE == "01")
            //    {
            //        item.PRE_ITEM_CODE = "";
            //    }
            //}
            foreach (var item in groupedData)
            {
                if (item.MASTER_ITEM_CODE == "01" || item.MASTER_ITEM_CODE == "02")
                {
                    item.PRE_ITEM_CODE = "";
                }

            }
            // return groupedData;
            List<dynamic> stockData = new List<dynamic>();
            var itemRows = groupedData.Where(r => r.GROUP_SKU_FLAG == "I").ToList();

            foreach (var row in groupedData)
            {
                double total1 = 0;  // S_QTY
                double total2 = 0;  // S_VALUE
                double total3 = 0;  // B_QTY
                double total4 = 0;  // B_VALUE
                double total5 = 0;  // A_QTY
                double total6 = 0;  // A_VALUE
                double total7 = 0;  // K_QTY
                double total8 = 0;  // K_VALUE
                double total9 = 0;  // M_QTY
                double total10 = 0; // M_VALUE
                double total11 = 0; // P_QTY
                double total12 = 0; // P_VALUE
                double total13 = 0; // Mg_QTY
                double total14 = 0; // Mg_VALUE
                double total15 = 0; // F_QTY
                double total16 = 0; // F_VALUE
                double total17 = 0; // C_QTY
                double total18 = 0; // C_VALUE
                double total19 = 0; // Bh_QTY
                double total20 = 0; // Bh_VALUE
                double total21 = 0; // J_VALUE
                double total22 = 0; // J_VALUE
                double total23 = 0; // Aa_VALUE
                double total24 = 0; // Aa_VALUE

                if (row.GROUP_SKU_FLAG == "G")
                {
                    string masterCode = row.MASTER_ITEM_CODE;
                    foreach (var z in itemRows)
                    {
                        if (!string.IsNullOrEmpty(z.MASTER_ITEM_CODE) &&
                            z.MASTER_ITEM_CODE.StartsWith(masterCode))
                        {
                            total1 += z.S_QTY ?? 0;
                            total2 += z.S_VALUE ?? 0;
                            total3 += z.B_QTY ?? 0;
                            total4 += z.B_VALUE ?? 0;
                            total5 += z.A_QTY ?? 0;
                            total6 += z.A_VALUE ?? 0;
                            total7 += z.K_QTY ?? 0;
                            total8 += z.K_VALUE ?? 0;
                            total9 += z.M_QTY ?? 0;
                            total10 += z.M_VALUE ?? 0;
                            total11 += z.P_QTY ?? 0;
                            total12 += z.P_VALUE ?? 0;
                            total13 += z.Mg_QTY ?? 0;
                            total14 += z.Mg_VALUE ?? 0;
                            total15 += z.F_QTY ?? 0;
                            total16 += z.F_VALUE ?? 0;
                            total17 += z.C_QTY ?? 0;
                            total18 += z.C_VALUE ?? 0;
                            total19 += z.Bh_QTY ?? 0;
                            total20 += z.Bh_VALUE ?? 0;
                            total21 += z.J_QTY ?? 0;
                            total22 += z.J_VALUE ?? 0;
                            total23 += z.Aa_QTY ?? 0;
                            total24 += z.Aa_VALUE ?? 0;
                        }
                    }
                }
                else
                {
                    total1 = row.S_QTY ?? 0;
                    total2 = row.S_VALUE ?? 0;
                    total3 = row.B_QTY ?? 0;
                    total4 = row.B_VALUE ?? 0;
                    total5 = row.A_QTY ?? 0;
                    total6 = row.A_VALUE ?? 0;
                    total7 = row.K_QTY ?? 0;
                    total8 = row.K_VALUE ?? 0;
                    total9 = row.M_QTY ?? 0;
                    total10 = row.M_VALUE ?? 0;
                    total11 = row.P_QTY ?? 0;
                    total12 = row.P_VALUE ?? 0;
                    total13 = row.Mg_QTY ?? 0;
                    total14 = row.Mg_VALUE ?? 0;
                    total15 = row.F_QTY ?? 0;
                    total16 = row.F_VALUE ?? 0;
                    total17 = row.C_QTY ?? 0;
                    total18 = row.C_VALUE ?? 0;
                    total19 = row.Bh_QTY ?? 0;
                    total20 = row.Bh_VALUE ?? 0;
                    total21 = row.J_QTY ?? 0;
                    total22 = row.J_VALUE ?? 0;
                    total23 = row.Aa_QTY ?? 0;
                    total24 = row.Aa_VALUE ?? 0;
                }

                // Add only if any total is non-zero
                if (
                    total1 != 0 || total2 != 0 || total3 != 0 || total4 != 0 ||
                    total5 != 0 || total6 != 0 || total7 != 0 || total8 != 0 ||
                    total9 != 0 || total10 != 0 || total11 != 0 || total12 != 0 ||
                    total13 != 0 || total14 != 0 || total15 != 0 || total16 != 0 ||
                    total17 != 0 || total18 != 0 || total19 != 0 || total20 != 0 ||
                    total21 != 0 || total22 != 0 || total23 != 0 || total24 != 0
                )
                {
                    stockData.Add(new
                    {
                        ITEM_CODE = row.ITEM_CODE,
                        ITEM_EDESC = row.ITEM_EDESC,
                        MASTER_ITEM_CODE = row.MASTER_ITEM_CODE,
                        PRE_ITEM_CODE = row.PRE_ITEM_CODE == "00" ? "" : row.PRE_ITEM_CODE,

                        S_QTY = total1,
                        S_VALUE = total2,
                        B_QTY = total3,
                        B_VALUE = total4,
                        A_QTY = total5,
                        A_VALUE = total6,
                        K_QTY = total7,
                        K_VALUE = total8,
                        M_QTY = total9,
                        M_VALUE = total10,
                        P_QTY = total11,
                        P_VALUE = total12,
                        Mg_QTY = total13,
                        Mg_VALUE = total14,
                        F_QTY = total15,
                        F_VALUE = total16,
                        C_QTY = total17,
                        C_VALUE = total18,
                        Bh_QTY = total19,
                        Bh_VALUE = total20,
                        J_QTY = total21,
                        J_VALUE = total22,
                        Aa_QTY = total23,
                        Aa_VALUE = total24
                    });
                }
            }

            //// ────────────────────────────────────────────────────────────────────────────
            //// 3. Grand Total row
            //// ────────────────────────────────────────────────────────────────────────────
            if (stockData.Any())
            {
                var grand = new
                {
                    ITEM_CODE = "",
                    ITEM_EDESC = "Grand Total",
                    MASTER_ITEM_CODE = "",
                    PRE_ITEM_CODE = "",

                    S_QTY = itemRows.Sum(x => x.S_QTY ?? 0),
                    S_VALUE = itemRows.Sum(x => x.S_VALUE ?? 0),

                    B_QTY = itemRows.Sum(x => x.B_QTY ?? 0),
                    B_VALUE = itemRows.Sum(x => x.B_VALUE ?? 0),

                    A_QTY = itemRows.Sum(x => x.A_QTY ?? 0),
                    A_VALUE = itemRows.Sum(x => x.A_VALUE ?? 0),

                    K_QTY = itemRows.Sum(x => x.K_QTY ?? 0),
                    K_VALUE = itemRows.Sum(x => x.K_VALUE ?? 0),

                    M_QTY = itemRows.Sum(x => x.M_QTY ?? 0),
                    M_VALUE = itemRows.Sum(x => x.M_VALUE ?? 0),

                    P_QTY = itemRows.Sum(x => x.P_QTY ?? 0),
                    P_VALUE = itemRows.Sum(x => x.P_VALUE ?? 0),

                    Mg_QTY = itemRows.Sum(x => x.Mg_QTY ?? 0),
                    Mg_VALUE = itemRows.Sum(x => x.Mg_VALUE ?? 0),

                    F_QTY = itemRows.Sum(x => x.F_QTY ?? 0),
                    F_VALUE = itemRows.Sum(x => x.F_VALUE ?? 0),

                    C_QTY = itemRows.Sum(x => x.C_QTY ?? 0),
                    C_VALUE = itemRows.Sum(x => x.C_VALUE ?? 0),

                    Bh_QTY = itemRows.Sum(x => x.Bh_QTY ?? 0),
                    Bh_VALUE = itemRows.Sum(x => x.Bh_VALUE ?? 0),

                    J_QTY = itemRows.Sum(x => x.J_QTY ?? 0),
                    J_VALUE = itemRows.Sum(x => x.J_VALUE ?? 0),

                    Aa_QTY = itemRows.Sum(x => x.Aa_QTY ?? 0),
                    Aa_VALUE = itemRows.Sum(x => x.Aa_VALUE ?? 0)
                };

                stockData.Add(grand);
            }
            return stockData;

        }

        //public dynamic GetMonthwiseCustomerSalesCollection(dynamic data, User userinfo)  List<MonthwiseCustomerSalesCollection> GetMonthwiseCustomerSalesCollection
        public dynamic GetMonthwiseCustomerSalesCollection(dynamic data, User userinfo)
        {
            var filters = data?.filter;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            List<string> branchCodes = new List<string> { branchCode }; // Can contain more than one

            // Generate the pivot aliases like: '01.01' AS "BRANCH_01_01"

            //var pivotInClause = string.Join(", ", branchCodes.Select(b => $"{b} AS \"BRANCH_{b.Replace(".", "_")}\""));
            //var pivotInClause = string.Join(", ", branchCodes.Select(b => $"{b} AS \"BRANCH\""));
            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");


            string query = string.Format(@"SELECT B.CUSTOMER_CODE, B.CUSTOMER_EDESC, B.MASTER_CUSTOMER_CODE, B.PRE_CUSTOMER_CODE, B.GROUP_SKU_FLAG, (LENGTH(B.MASTER_CUSTOMER_CODE) - LENGTH(REPLACE(B.MASTER_CUSTOMER_CODE,'.',''))) ROWLEV,  A.* FROM (  
                                            SELECT * FROM (  
                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE CUS_CODE, MTH,   
                                            SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE, COLLECTION FROM (   
                                            SELECT MTH,A.COMPANY_CODE, A.CUSTOMER_CODE, SUM(A.SALES_VALUE) SALES_VALUE,  SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE , SUM(COLLECTION)  COLLECTION   FROM (   
                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE,  SUBSTR(BS_DATE(SALES_DATE),6,2) MTH, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE , 0 COLLECTION    FROM SA_SALES_INVOICE A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.CUSTOMER_CODE, A.COMPANY_CODE, SALES_DATE  
                                            Union All   
                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, SUBSTR(BS_DATE(RETURN_DATE),6,2) MTH,  0 SALES_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE , 0 COLLECTION    FROM SA_SALES_RETURN A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.CUSTOMER_CODE, A.COMPANY_CODE, RETURN_DATE  
                                            Union All   
                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH,  0 SALES_VALUE, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE, 0 COLLECTION   FROM SA_DEBIT_NOTE A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.CUSTOMER_CODE, A.COMPANY_CODE, VOUCHER_DATE  
                                            Union All   
                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH, 0 SALES_VALUE, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE, 0 COLLECTION    FROM SA_CREDIT_NOTE A   
                                            WHERE A.DELETED_FLAG = 'N'   
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.CUSTOMER_CODE, A.COMPANY_CODE, VOUCHER_DATE  
                                            Union All   
                                            SELECT A.COMPANY_CODE, REPLACE(A.SUB_CODE,'C','') CUSTOMER_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH,  0 SALES_VALUE, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, SUM(NVL(A.CR_AMOUNT,0)) COLLECTION  FROM V$VIRTUAL_SUB_LEDGER a  
                                            WHERE A.DELETED_FLAG = 'N'     
                                            AND A.FORM_CODE NOT IN (SELECT FORM_CODE FROM FORM_SETUP WHERE COMPANY_CODE IN ({2}) AND FORM_TYPE = 'PV' )  
                                            AND A.SUB_CODE LIKE 'C%'  
                                            AND A.FORM_CODE != '0'  
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.SUB_CODE, A.COMPANY_CODE, VOUCHER_DATE  
                                            ) A,  SA_CUSTOMER_SETUP B   
                                            WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE   
                                            AND A.COMPANY_CODE = B.COMPANY_CODE   
                                            AND A.COMPANY_CODE IN ({2})  
                                            GROUP BY   A.CUSTOMER_CODE, A.COMPANY_CODE  , MTH  
                                            ) A ORDER BY CUSTOMER_CODE  
                                            )  
                                            PIVOT  
                                              (SUM(NET_SALES_VALUE) VALUE, SUM(COLLECTION) COLLECTION
                                               FOR MTH IN ('04' AS S,
                                               '05' AS B,
                                               '06' AS A,
                                               '07' AS K,
                                               '08' AS M,
                                               '09' AS P,
                                               '10' AS Mg,
                                               '11' AS F,
                                               '12' AS C,
                                               '01' AS Bh,
                                               '02' AS J,
                                               '03' AS Aa)  
                                            ) 
                                            )   
                                             A, SA_CUSTOMER_SETUP B  
                                            WHERE  B.CUSTOMER_CODE = A.CUS_CODE (+)   
                                            AND B.COMPANY_CODE = A.COMPANY_CODE (+)   
                                            AND B.COMPANY_CODE IN ({2}) 
                                            ORDER BY B.MASTER_CUSTOMER_CODE, B.CUSTOMER_EDESC", FromDate, ToDate, companyCode, branchCode);
            var datewise = _objectEntity.SqlQuery<MonthwiseCustomerSalesCollection>(query).ToList();
            var groupedData = datewise;

            // Set PRE_CUSTOMER_CODE = null where CUSTOMER_CODE == "1"
            //foreach (var item in groupedData)
            //{
            //    if (item.MASTER_CUSTOMER_CODE == "01")
            //    {
            //        item.PRE_CUSTOMER_CODE = "";
            //    }
            //}
            foreach (var item in groupedData)
            {
                if (item.MASTER_CUSTOMER_CODE == "01" || item.MASTER_CUSTOMER_CODE == "02")
                {
                    item.PRE_CUSTOMER_CODE = "";
                }

            }
            //return groupedData;
            List<dynamic> stockData = new List<dynamic>();
            var itemRows = groupedData.Where(r => r.GROUP_SKU_FLAG == "I").ToList();

            foreach (var row in groupedData)
            {
                double total1 = 0;  // S_QTY
                double total2 = 0;  // S_VALUE
                double total3 = 0;  // B_QTY
                double total4 = 0;  // B_VALUE
                double total5 = 0;  // A_QTY
                double total6 = 0;  // A_VALUE
                double total7 = 0;  // K_QTY
                double total8 = 0;  // K_VALUE
                double total9 = 0;  // M_QTY
                double total10 = 0; // M_VALUE
                double total11 = 0; // P_QTY
                double total12 = 0; // P_VALUE
                double total13 = 0; // Mg_QTY
                double total14 = 0; // Mg_VALUE
                double total15 = 0; // F_QTY
                double total16 = 0; // F_VALUE
                double total17 = 0; // C_QTY
                double total18 = 0; // C_VALUE
                double total19 = 0; // Bh_QTY
                double total20 = 0; // Bh_VALUE
                double total21 = 0; // J_VALUE
                double total22 = 0; // J_VALUE
                double total23 = 0; // Aa_VALUE
                double total24 = 0; // Aa_VALUE

                if (row.GROUP_SKU_FLAG == "G")
                {
                    string masterCode = row.MASTER_CUSTOMER_CODE;
                    foreach (var z in itemRows)
                    {
                        if (!string.IsNullOrEmpty(z.MASTER_CUSTOMER_CODE) &&
                            z.MASTER_CUSTOMER_CODE.StartsWith(masterCode))
                        {
                            total1 += z.S_COLLECTION ?? 0;
                            total2 += z.S_VALUE ?? 0;
                            total3 += z.B_COLLECTION ?? 0;
                            total4 += z.B_VALUE ?? 0;
                            total5 += z.A_COLLECTION ?? 0;
                            total6 += z.A_VALUE ?? 0;
                            total7 += z.K_COLLECTION ?? 0;
                            total8 += z.K_VALUE ?? 0;
                            total9 += z.M_COLLECTION ?? 0;
                            total10 += z.M_VALUE ?? 0;
                            total11 += z.P_COLLECTION ?? 0;
                            total12 += z.P_VALUE ?? 0;
                            total13 += z.Mg_COLLECTION ?? 0;
                            total14 += z.Mg_VALUE ?? 0;
                            total15 += z.F_COLLECTION ?? 0;
                            total16 += z.F_VALUE ?? 0;
                            total17 += z.C_COLLECTION ?? 0;
                            total18 += z.C_VALUE ?? 0;
                            total19 += z.Bh_COLLECTION ?? 0;
                            total20 += z.Bh_VALUE ?? 0;
                            total21 += z.J_COLLECTION ?? 0;
                            total22 += z.J_VALUE ?? 0;
                            total23 += z.Aa_COLLECTION ?? 0;
                            total24 += z.Aa_VALUE ?? 0;
                        }
                    }
                }
                else
                {
                    total1 = row.S_COLLECTION ?? 0;
                    total2 = row.S_VALUE ?? 0;
                    total3 = row.B_COLLECTION ?? 0;
                    total4 = row.B_VALUE ?? 0;
                    total5 = row.A_COLLECTION ?? 0;
                    total6 = row.A_VALUE ?? 0;
                    total7 = row.K_COLLECTION ?? 0;
                    total8 = row.K_VALUE ?? 0;
                    total9 = row.M_COLLECTION ?? 0;
                    total10 = row.M_VALUE ?? 0;
                    total11 = row.P_COLLECTION ?? 0;
                    total12 = row.P_VALUE ?? 0;
                    total13 = row.Mg_COLLECTION ?? 0;
                    total14 = row.Mg_VALUE ?? 0;
                    total15 = row.F_COLLECTION ?? 0;
                    total16 = row.F_VALUE ?? 0;
                    total17 = row.C_COLLECTION ?? 0;
                    total18 = row.C_VALUE ?? 0;
                    total19 = row.Bh_COLLECTION ?? 0;
                    total20 = row.Bh_VALUE ?? 0;
                    total21 = row.J_COLLECTION ?? 0;
                    total22 = row.J_VALUE ?? 0;
                    total23 = row.Aa_COLLECTION ?? 0;
                    total24 = row.Aa_VALUE ?? 0;
                }

                // Add only if any total is non-zero
                if (
                    total1 != 0 || total2 != 0 || total3 != 0 || total4 != 0 ||
                    total5 != 0 || total6 != 0 || total7 != 0 || total8 != 0 ||
                    total9 != 0 || total10 != 0 || total11 != 0 || total12 != 0 ||
                    total13 != 0 || total14 != 0 || total15 != 0 || total16 != 0 ||
                    total17 != 0 || total18 != 0 || total19 != 0 || total20 != 0 ||
                    total21 != 0 || total22 != 0 || total23 != 0 || total24 != 0
                )
                {
                    stockData.Add(new
                    {
                        CUSTOMER_CODE = row.CUSTOMER_CODE,
                        CUSTOMER_EDESC = row.CUSTOMER_EDESC,
                        MASTER_CUSTOMER_CODE = row.MASTER_CUSTOMER_CODE,
                        PRE_CUSTOMER_CODE = row.PRE_CUSTOMER_CODE,

                        S_COLLECTION = total1,
                        S_VALUE = total2,
                        B_COLLECTION = total3,
                        B_VALUE = total4,
                        A_COLLECTION = total5,
                        A_VALUE = total6,
                        K_COLLECTION = total7,
                        K_VALUE = total8,
                        M_COLLECTION = total9,
                        M_VALUE = total10,
                        P_COLLECTION = total11,
                        P_VALUE = total12,
                        Mg_COLLECTION = total13,
                        Mg_VALUE = total14,
                        F_COLLECTION = total15,
                        F_VALUE = total16,
                        C_COLLECTION = total17,
                        C_VALUE = total18,
                        Bh_COLLECTION = total19,
                        Bh_VALUE = total20,
                        J_COLLECTION = total21,
                        J_VALUE = total22,
                        Aa_COLLECTION = total23,
                        Aa_VALUE = total24
                    });
                }
            }

            //// ────────────────────────────────────────────────────────────────────────────
            //// 3. Grand Total row
            //// ────────────────────────────────────────────────────────────────────────────
            if (stockData.Any())
            {
                var grand = new
                {
                    CUSTOMER_CODE = "",
                    CUSTOMER_EDESC = "Grand Total",
                    MASTER_CUSTOMER_CODE = "",
                    PRE_CUSTOMER_CODE = "",

                    S_COLLECTION = itemRows.Sum(x => x.S_COLLECTION ?? 0),
                    S_VALUE = itemRows.Sum(x => x.S_VALUE ?? 0),

                    B_COLLECTION = itemRows.Sum(x => x.B_COLLECTION ?? 0),
                    B_VALUE = itemRows.Sum(x => x.B_VALUE ?? 0),

                    A_COLLECTION = itemRows.Sum(x => x.A_COLLECTION ?? 0),
                    A_VALUE = itemRows.Sum(x => x.A_VALUE ?? 0),

                    K_COLLECTION = itemRows.Sum(x => x.K_COLLECTION ?? 0),
                    K_VALUE = itemRows.Sum(x => x.K_VALUE ?? 0),

                    M_COLLECTION = itemRows.Sum(x => x.M_COLLECTION ?? 0),
                    M_VALUE = itemRows.Sum(x => x.M_VALUE ?? 0),

                    P_COLLECTION = itemRows.Sum(x => x.P_COLLECTION ?? 0),
                    P_VALUE = itemRows.Sum(x => x.P_VALUE ?? 0),

                    Mg_COLLECTION = itemRows.Sum(x => x.Mg_COLLECTION ?? 0),
                    Mg_VALUE = itemRows.Sum(x => x.Mg_VALUE ?? 0),

                    F_COLLECTION = itemRows.Sum(x => x.F_COLLECTION ?? 0),
                    F_VALUE = itemRows.Sum(x => x.F_VALUE ?? 0),

                    C_COLLECTION = itemRows.Sum(x => x.C_COLLECTION ?? 0),
                    C_VALUE = itemRows.Sum(x => x.C_VALUE ?? 0),

                    Bh_COLLECTION = itemRows.Sum(x => x.Bh_COLLECTION ?? 0),
                    Bh_VALUE = itemRows.Sum(x => x.Bh_VALUE ?? 0),

                    J_COLLECTION = itemRows.Sum(x => x.J_COLLECTION ?? 0),
                    J_VALUE = itemRows.Sum(x => x.J_VALUE ?? 0),

                    Aa_COLLECTION = itemRows.Sum(x => x.Aa_COLLECTION ?? 0),
                    Aa_VALUE = itemRows.Sum(x => x.Aa_VALUE ?? 0)
                };

                stockData.Add(grand);
            }
            return stockData;

        }

        public dynamic GetBranchwiseCustomerProductNetSales(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            List<string> branchCodes = new List<string> { branchCode }; // Can contain more than one

            // Generate the pivot aliases like: '01.01' AS "BRANCH_01_01"

            //var pivotInClause = string.Join(", ", branchCodes.Select(b => $"{b} AS \"BRANCH_{b.Replace(".", "_")}\""));
            var pivotInClause = string.Join(", ", branchCodes.Select(b => $"{b} AS \"BRANCH\""));
            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT B.CUSTOMER_CODE, B.CUSTOMER_EDESC, B.MASTER_CUSTOMER_CODE , B.PRE_CUSTOMER_CODE, B.GROUP_SKU_FLAG, (LENGTH(B.MASTER_CUSTOMER_CODE) - LENGTH(REPLACE(B.MASTER_CUSTOMER_CODE,'.',''))) ROWLEV,  A.* FROM (  
                                    SELECT * FROM (  
                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE CUS_CODE, ITEM_CODE, ITEM_EDESC, INDEX_MU_CODE, BRANCH_CODE,   
                                    SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE FROM (   
                                    SELECT A.BRANCH_CODE ,A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE INDEX_MU_CODE,  SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE  FROM (   
                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE,  A.ITEM_CODE, A.BRANCH_CODE , SUM(NVL(A.QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE   FROM SA_SALES_INVOICE A   
                                    WHERE A.DELETED_FLAG = 'N'   
                                    AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                    AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                    AND A.COMPANY_CODE IN ({2})
                                    AND A.BRANCH_CODE IN ({3})    
                                    GROUP BY  A.CUSTOMER_CODE,  A.ITEM_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                    Union All   
                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, A.BRANCH_CODE, 0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY,0))  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE   FROM SA_SALES_RETURN A   
                                    WHERE A.DELETED_FLAG = 'N'   
                                    AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                    AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                    AND A.COMPANY_CODE IN ({2})
                                    AND A.BRANCH_CODE IN ({3})  
                                    GROUP BY  A.CUSTOMER_CODE, A.ITEM_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                    Union All   
                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, A.BRANCH_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE  FROM SA_DEBIT_NOTE A   
                                    WHERE A.DELETED_FLAG = 'N'   
                                    AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                    AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                    AND A.COMPANY_CODE IN ({2})
                                    AND A.BRANCH_CODE IN ({3})  
                                    GROUP BY  A.CUSTOMER_CODE, A.ITEM_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                    Union All   
                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, A.BRANCH_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE  FROM SA_CREDIT_NOTE A   
                                    WHERE A.DELETED_FLAG = 'N'   
                                    AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                    AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                    AND A.COMPANY_CODE IN ({2})
                                    AND A.BRANCH_CODE IN ({3})  
                                    GROUP BY  A.CUSTOMER_CODE, A.ITEM_CODE , A.COMPANY_CODE, A.BRANCH_CODE  
                                    ORDER BY CUSTOMER_CODE) A,  SA_CUSTOMER_SETUP B ,  IP_ITEM_MASTER_SETUP D   
                                    WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE   
                                    AND  A.ITEM_CODE = D.ITEM_CODE   
                                    AND A.COMPANY_CODE = B.COMPANY_CODE   
                                    AND A.COMPANY_CODE = D.COMPANY_CODE   
                                    AND A.COMPANY_CODE IN ({2})  
                                    GROUP BY   A.CUSTOMER_CODE,  A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE, A.COMPANY_CODE  , A.BRANCH_CODE  
                                    ) A ORDER BY CUSTOMER_CODE, ITEM_EDESC  
                                    )  
                                    PIVOT (
                                         SUM(NET_SALES_QTY) AS QTY,
                                         SUM(NET_SALES_VALUE) AS VALUE
                                         FOR BRANCH_CODE IN ({4})
                                        ) 
                                        ) 
                                     A, SA_CUSTOMER_SETUP B  
                                    WHERE  B.CUSTOMER_CODE = A.CUS_CODE (+)   
                                    AND B.COMPANY_CODE = A.COMPANY_CODE (+)   
                                    AND B.COMPANY_CODE IN ({2})  
                                    ORDER BY B.MASTER_CUSTOMER_CODE, B.CUSTOMER_EDESC, A.ITEM_EDESC ", FromDate, ToDate, companyCode, branchCode, pivotInClause);
            var datewise = _objectEntity.SqlQuery<BranchItemStockModel>(query).ToList();
            var groupedData = datewise;

            foreach (var item in groupedData)
            {
                if (item.MASTER_CUSTOMER_CODE == "01" || item.MASTER_CUSTOMER_CODE == "02")
                {
                    item.PRE_CUSTOMER_CODE = "";
                }

            }

            //return groupedData;
            var itemRows = groupedData.Where(r => r.GROUP_SKU_FLAG == "I").ToList();
            List<dynamic> stockData = new List<dynamic>();

            foreach (var row in groupedData)
            {
                // ... [original 40 totals]
                double totalBranchQty = 0;     // BRANCH_QTY
                double totalBranchValue = 0;   // BRANCH_VALUE

                if (row.GROUP_SKU_FLAG == "G")
                {
                    string masterCode = row.MASTER_CUSTOMER_CODE;
                    foreach (var z in itemRows)
                    {
                        if (!string.IsNullOrEmpty(z.MASTER_CUSTOMER_CODE) && z.MASTER_CUSTOMER_CODE.StartsWith(masterCode))
                        {
                            // ... [original totals aggregation]
                            totalBranchQty += z.BRANCH_QTY ?? 0;
                            totalBranchValue += z.BRANCH_VALUE ?? 0;
                        }
                    }
                }
                else
                {
                    // ... [original single row assignment]
                    totalBranchQty = row.BRANCH_QTY ?? 0;
                    totalBranchValue = row.BRANCH_VALUE ?? 0;
                }

                if (totalBranchQty != 0 || totalBranchValue != 0)
                {
                    stockData.Add(new
                    {
                        CUSTOMER_CODE = row.CUSTOMER_CODE,
                        CUS_CODE = row.CUS_CODE,
                        CUSTOMER_EDESC = row.CUSTOMER_EDESC,
                        ITEM_CODE = row.ITEM_CODE,
                        ITEM_EDESC = row.ITEM_EDESC,
                        INDEX_MU_CODE = row.INDEX_MU_CODE,
                        GROUP_SKU_FLAG = row.GROUP_SKU_FLAG,
                        MASTER_CUSTOMER_CODE = row.MASTER_CUSTOMER_CODE,
                        PRE_CUSTOMER_CODE = row.PRE_CUSTOMER_CODE,
                        //PRE_CUSTOMER_CODE = row.PRE_CUSTOMER_CODE == "00" ? null : row.PRE_ITEM_CODE,
                        // ... [existing fields]
                        BRANCH_QTY = totalBranchQty,
                        BRANCH_VALUE = totalBranchValue,
                    });
                }
            }

            if (stockData.Any())
            {
                var grandTotal = new
                {
                    CUSTOMER_CODE = "",
                    CUS_CODE = "",
                    CUSTOMER_EDESC = "Grand Total",
                    ITEM_CODE = "",
                    ITEM_EDESC = "",
                    INDEX_MU_CODE = "",
                    MASTER_CUSTOMER_CODE = "",
                    PRE_CUSTOMER_CODE = "",
                    GROUP_SKU_FLAG = "",
                    // ... [existing grand total fields]
                    BRANCH_QTY = itemRows.Sum(x => x.BRANCH_QTY ?? 0),
                    BRANCH_VALUE = itemRows.Sum(x => x.BRANCH_VALUE ?? 0),
                };

                stockData.Add(grandTotal);
            }

            return stockData;
        }

        public dynamic GetMonthwiseSalesTypeCustomerNetSales(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT 
                                            B.CUSTOMER_CODE,
                                            B.CUSTOMER_EDESC,
                                            A.SALES_TYPE_CODE,
                                            B.MASTER_CUSTOMER_CODE,
                                            B.PRE_CUSTOMER_CODE ,
                                            B.GROUP_SKU_FLAG,
                                            (LENGTH(B.MASTER_CUSTOMER_CODE) - LENGTH(REPLACE(B.MASTER_CUSTOMER_CODE, '.', ''))) ROWLEV,
                                            A.*
                                        FROM (
                                            SELECT *
                                            FROM (
                                                SELECT 
                                                    A.COMPANY_CODE,
                                                    A.CUSTOMER_CODE CUS_CODE,
			                                        A.SALES_TYPE_CODE,
                                                    ITEM_CODE,
                                                    ITEM_EDESC,
                                                    INDEX_MU_CODE,
                                                    MTH,
                                                    SALES_QTY - SALES_RET_QTY NET_SALES_QTY,
                                                    SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE
                                                FROM (
                                                    SELECT 
                                                        MTH,
                                                        A.COMPANY_CODE,
                                                        A.CUSTOMER_CODE,
			                                            A.SALES_TYPE_CODE,
                                                        A.ITEM_CODE,
                                                        D.ITEM_EDESC,
                                                        D.INDEX_MU_CODE,
                                                        CASE 
                                                            WHEN E.BRAND_NAME IS NULL THEN D.ITEM_EDESC
                                                            ELSE E.BRAND_NAME
                                                        END BRAND_NAME,
                                                        SUM(A.SALES_QTY) SALES_QTY,
                                                        SUM(A.SALES_VALUE) SALES_VALUE,
                                                        SUM(A.SALES_RET_QTY) SALES_RET_QTY,
                                                        SUM(A.SALES_RET_VALUE) SALES_RET_VALUE,
                                                        SUM(DEBIT_VALUE) DEBIT_VALUE,
                                                        SUM(CREDIT_VALUE) CREDIT_VALUE
                                                    FROM (
                                                        -- Sales Invoice
                                                        SELECT 
                                                            A.COMPANY_CODE,
                                                            A.CUSTOMER_CODE,
					                                        A.SALES_TYPE_CODE,
                                                            A.ITEM_CODE,
                                                            SUBSTR(BS_DATE(SALES_DATE), 6, 2) MTH,
                                                            SUM(NVL(A.QUANTITY, 0)) SALES_QTY,
                                                            SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) SALES_VALUE,
                                                            0 SALES_RET_QTY,
                                                            0 SALES_RET_VALUE,
                                                            0 DEBIT_VALUE,
                                                            0 CREDIT_VALUE
                                                        FROM SA_SALES_INVOICE A
                                                        WHERE A.DELETED_FLAG = 'N'
                  
                                                            AND A.SALES_TYPE_CODE IN ('01', '02', '03', '04', '05', '06', '07', '08','09')
                                                            AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                            AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                            AND A.COMPANY_CODE IN ({2})
                                                            AND A.BRANCH_CODE IN ({3})  
                                                        GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE, SALES_DATE,A.SALES_TYPE_CODE

                                                        UNION ALL

                                                        -- Sales Return
                                                        SELECT 
                                                            A.COMPANY_CODE,
                                                            A.CUSTOMER_CODE,
					                                        A.SALES_TYPE_CODE,
                                                            A.ITEM_CODE,
                                                            SUBSTR(BS_DATE(RETURN_DATE), 6, 2) MTH,
                                                            0 SALES_QTY,
                                                            0 SALES_VALUE,
                                                            SUM(NVL(A.QUANTITY, 0)) SALES_RET_QTY,
                                                            SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) SALES_RET_VALUE,
                                                            0 DEBIT_VALUE,
                                                            0 CREDIT_VALUE
                                                        FROM SA_SALES_RETURN A
                                                        WHERE A.DELETED_FLAG = 'N'
                                                            AND A.SALES_TYPE_CODE IN ('01', '02', '03', '04', '05', '06', '07', '08','09')
                                                            AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                            AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                            AND A.COMPANY_CODE IN ({2})
                                                            AND A.BRANCH_CODE IN ({3})
                                                        GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE,A.SALES_TYPE_CODE, RETURN_DATE

                                                        UNION ALL

                                                        -- Debit Note
                                                        SELECT 
                                                            A.COMPANY_CODE,
                                                            A.CUSTOMER_CODE,
					                                        A.SALES_TYPE_CODE,
                                                            A.ITEM_CODE,
                                                            SUBSTR(BS_DATE(VOUCHER_DATE), 6, 2) MTH,
                                                            0 SALES_QTY,
                                                            0 SALES_VALUE,
                                                            0 SALES_RET_QTY,
                                                            0 SALES_RET_VALUE,
                                                            SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) DEBIT_VALUE,
                                                            0 CREDIT_VALUE
                                                        FROM SA_DEBIT_NOTE A
                                                        WHERE A.DELETED_FLAG = 'N'
                    
                                                            AND A.SALES_TYPE_CODE IN ('01', '02', '03', '04', '05', '06', '07', '08','09')
                                                             AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                             AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                             AND A.COMPANY_CODE IN ({2})
                                                             AND A.BRANCH_CODE IN ({3})  
                                                        GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE,A.SALES_TYPE_CODE, VOUCHER_DATE

                                                        UNION ALL

                                                        -- Credit Note
                                                        SELECT 
                                                            A.COMPANY_CODE,
                                                            A.CUSTOMER_CODE,
					                                        A.SALES_TYPE_CODE,
                                                            A.ITEM_CODE,
                                                            SUBSTR(BS_DATE(VOUCHER_DATE), 6, 2) MTH,
                                                            0 SALES_QTY,
                                                            0 SALES_VALUE,
                                                            0 SALES_RET_QTY,
                                                            0 SALES_RET_VALUE,
                                                            0 DEBIT_VALUE,
                                                            SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) CREDIT_VALUE
                                                        FROM SA_CREDIT_NOTE A
                                                        WHERE A.DELETED_FLAG = 'N'
                   
                                                            AND A.SALES_TYPE_CODE IN ('01', '02', '03', '04', '05', '06', '07', '08','09')
                                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                            AND A.COMPANY_CODE IN ({2})
                                                            AND A.BRANCH_CODE IN ({3})  
                                                        GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE,A.SALES_TYPE_CODE, VOUCHER_DATE
                                                    ) A,
                                                    SA_CUSTOMER_SETUP B,
                                                    IP_ITEM_MASTER_SETUP D,
                                                    IP_ITEM_SPEC_SETUP E
                                                    WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE
                                                        AND A.ITEM_CODE = D.ITEM_CODE
                                                        AND A.COMPANY_CODE = D.COMPANY_CODE
                                                        AND A.COMPANY_CODE = B.COMPANY_CODE
                                                        AND D.GROUP_SKU_FLAG = 'I'
                                                        AND D.ITEM_CODE = E.ITEM_CODE(+)
                                                        AND D.COMPANY_CODE = E.COMPANY_CODE(+)
                                                        AND A.COMPANY_CODE IN ({2})
                                                    GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE,A.SALES_TYPE_CODE, E.BRAND_NAME, A.COMPANY_CODE, MTH
                                                ) A
                                                ORDER BY CUSTOMER_CODE, ITEM_EDESC
                                            ) 
	
                                           PIVOT  
                                             (SUM(NET_SALES_QTY) QTY, SUM(NET_SALES_VALUE) VALUE  
                                              FOR MTH IN ('04' AS S,
                                                '05' AS B,
                                                '06' AS A,
                                                '07' AS K,
                                                '08' AS M,
                                                '09' AS P,
                                                '10' AS Mg,
                                                '11' AS F,
                                                '12' AS C,
                                                '01' AS Bh,
                                                '02' AS J,
                                                '03' AS Aa)
		                                        )  
                                                ) 
                                         A,
                                        SA_CUSTOMER_SETUP B
                                        WHERE B.CUSTOMER_CODE = A.CUS_CODE(+)
                                            AND B.COMPANY_CODE = A.COMPANY_CODE(+)
                                            AND B.COMPANY_CODE IN ({2})
                                        ORDER BY B.MASTER_CUSTOMER_CODE,A.SALES_TYPE_CODE, B.CUSTOMER_EDESC, A.ITEM_EDESC", FromDate, ToDate, companyCode, branchCode);
            var monthwise = _objectEntity.SqlQuery<MonthwiseCustProdNetSalesModel>(query).ToList();
            var groupedData = monthwise;

            foreach (var item in groupedData)
            {
                if (item.MASTER_CUSTOMER_CODE == "01" || item.MASTER_CUSTOMER_CODE == "02")
                {
                    item.PRE_CUSTOMER_CODE = "";
                }

            }

            // Define the list of sales type codes you want to add
            var salesTypeCodes = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09" };

            // Create a new list to hold the processed data
            var processedData = new List<MonthwiseCustProdNetSalesModel>();

            // Iterate through the original data from the database
            foreach (var item in groupedData)
            {
                // Check if the item's SALES_TYPE_CODE is null or empty.
                // This condition identifies the group records you added via UNION ALL.
                if (string.IsNullOrEmpty(item.SALES_TYPE_CODE))
                {
                    // For each missing sales type, create a new, duplicated record.
                    foreach (var salesTypeCode in salesTypeCodes)
                    {
                        // Create a copy of the original item
                        var newItem = new MonthwiseCustProdNetSalesModel
                        {
                            CUSTOMER_CODE = item.CUSTOMER_CODE,
                            CUSTOMER_EDESC = item.CUSTOMER_EDESC,
                            MASTER_CUSTOMER_CODE = item.MASTER_CUSTOMER_CODE,
                            PRE_CUSTOMER_CODE = item.PRE_CUSTOMER_CODE,
                            GROUP_SKU_FLAG = item.GROUP_SKU_FLAG,
                            ROWLEV = item.ROWLEV,
                            // Assign the sales type code to the new item
                            SALES_TYPE_CODE = salesTypeCode,
                            // All pivoted values should be 0 or null for these new records
                            // since they are just for structuring the hierarchy.
                            // You may need to copy other properties if your model has more.
                            // Example for pivoted columns:
                            //S_QTY = 0,
                            //S_VALUE = 0,
                            //B_QTY = 0,
                            //B_VALUE = 0,
                            // ... and so on for all 12 months
                        };
                        processedData.Add(newItem);
                    }
                }
                else
                {
                    // For items that already have a sales type code, add them directly to the list.
                    processedData.Add(item);
                }
            }

            // The final groupedData list now contains all the necessary entries.
            groupedData = processedData;





            // return groupedData;
            List<dynamic> stockData = new List<dynamic>();
            var itemRows = groupedData.Where(r => r.GROUP_SKU_FLAG == "I").ToList();

            foreach (var row in groupedData)
            {
                double total1 = 0;  // S_QTY
                double total2 = 0;  // S_VALUE
                double total3 = 0;  // B_QTY
                double total4 = 0;  // B_VALUE
                double total5 = 0;  // A_QTY
                double total6 = 0;  // A_VALUE
                double total7 = 0;  // K_QTY
                double total8 = 0;  // K_VALUE
                double total9 = 0;  // M_QTY
                double total10 = 0; // M_VALUE
                double total11 = 0; // P_QTY
                double total12 = 0; // P_VALUE
                double total13 = 0; // Mg_QTY
                double total14 = 0; // Mg_VALUE
                double total15 = 0; // F_QTY
                double total16 = 0; // F_VALUE
                double total17 = 0; // C_QTY
                double total18 = 0; // C_VALUE
                double total19 = 0; // Bh_QTY
                double total20 = 0; // Bh_VALUE
                double total21 = 0; // J_VALUE
                double total22 = 0; // J_VALUE
                double total23 = 0; // Aa_VALUE
                double total24 = 0; // Aa_VALUE

                if (row.GROUP_SKU_FLAG == "G")
                {
                    string masterCode = row.MASTER_CUSTOMER_CODE;
                    foreach (var z in itemRows)
                    {
                        if (!string.IsNullOrEmpty(z.MASTER_CUSTOMER_CODE) &&
                            z.MASTER_CUSTOMER_CODE.StartsWith(masterCode))
                        {
                            total1 += z.S_QTY ?? 0;
                            total2 += z.S_VALUE ?? 0;
                            total3 += z.B_QTY ?? 0;
                            total4 += z.B_VALUE ?? 0;
                            total5 += z.A_QTY ?? 0;
                            total6 += z.A_VALUE ?? 0;
                            total7 += z.K_QTY ?? 0;
                            total8 += z.K_VALUE ?? 0;
                            total9 += z.M_QTY ?? 0;
                            total10 += z.M_VALUE ?? 0;
                            total11 += z.P_QTY ?? 0;
                            total12 += z.P_VALUE ?? 0;
                            total13 += z.Mg_QTY ?? 0;
                            total14 += z.Mg_VALUE ?? 0;
                            total15 += z.F_QTY ?? 0;
                            total16 += z.F_VALUE ?? 0;
                            total17 += z.C_QTY ?? 0;
                            total18 += z.C_VALUE ?? 0;
                            total19 += z.Bh_QTY ?? 0;
                            total20 += z.Bh_VALUE ?? 0;
                            total21 += z.J_QTY ?? 0;
                            total22 += z.J_VALUE ?? 0;
                            total23 += z.Aa_QTY ?? 0;
                            total24 += z.Aa_VALUE ?? 0;
                        }
                    }
                }
                else
                {
                    total1 = row.S_QTY ?? 0;
                    total2 = row.S_VALUE ?? 0;
                    total3 = row.B_QTY ?? 0;
                    total4 = row.B_VALUE ?? 0;
                    total5 = row.A_QTY ?? 0;
                    total6 = row.A_VALUE ?? 0;
                    total7 = row.K_QTY ?? 0;
                    total8 = row.K_VALUE ?? 0;
                    total9 = row.M_QTY ?? 0;
                    total10 = row.M_VALUE ?? 0;
                    total11 = row.P_QTY ?? 0;
                    total12 = row.P_VALUE ?? 0;
                    total13 = row.Mg_QTY ?? 0;
                    total14 = row.Mg_VALUE ?? 0;
                    total15 = row.F_QTY ?? 0;
                    total16 = row.F_VALUE ?? 0;
                    total17 = row.C_QTY ?? 0;
                    total18 = row.C_VALUE ?? 0;
                    total19 = row.Bh_QTY ?? 0;
                    total20 = row.Bh_VALUE ?? 0;
                    total21 = row.J_QTY ?? 0;
                    total22 = row.J_VALUE ?? 0;
                    total23 = row.Aa_QTY ?? 0;
                    total24 = row.Aa_VALUE ?? 0;
                }

                // Add only if any total is non-zero
                if (
                    total1 != 0 || total2 != 0 || total3 != 0 || total4 != 0 ||
                    total5 != 0 || total6 != 0 || total7 != 0 || total8 != 0 ||
                    total9 != 0 || total10 != 0 || total11 != 0 || total12 != 0 ||
                    total13 != 0 || total14 != 0 || total15 != 0 || total16 != 0 ||
                    total17 != 0 || total18 != 0 || total19 != 0 || total20 != 0 ||
                    total21 != 0 || total22 != 0 || total23 != 0 || total24 != 0
                )
                {
                    stockData.Add(new
                    {
                        CUSTOMER_CODE = row.CUSTOMER_CODE,
                        CUS_CODE = row.CUS_CODE,
                        SALES_TYPE_CODE = row.SALES_TYPE_CODE,
                        CUSTOMER_EDESC = row.CUSTOMER_EDESC,
                        ITEM_CODE = row.ITEM_CODE,
                        ITEM_EDESC = row.ITEM_EDESC,
                        INDEX_MU_CODE = row.INDEX_MU_CODE,
                        GROUP_SKU_FLAG = row.GROUP_SKU_FLAG,
                        MASTER_CUSTOMER_CODE = row.MASTER_CUSTOMER_CODE,
                        PRE_CUSTOMER_CODE = row.PRE_CUSTOMER_CODE,

                        S_QTY = total1,
                        S_VALUE = total2,
                        B_QTY = total3,
                        B_VALUE = total4,
                        A_QTY = total5,
                        A_VALUE = total6,
                        K_QTY = total7,
                        K_VALUE = total8,
                        M_QTY = total9,
                        M_VALUE = total10,
                        P_QTY = total11,
                        P_VALUE = total12,
                        Mg_QTY = total13,
                        Mg_VALUE = total14,
                        F_QTY = total15,
                        F_VALUE = total16,
                        C_QTY = total17,
                        C_VALUE = total18,
                        Bh_QTY = total19,
                        Bh_VALUE = total20,
                        J_QTY = total21,
                        J_VALUE = total22,
                        Aa_QTY = total23,
                        Aa_VALUE = total24
                    });
                }
            }

            //// ────────────────────────────────────────────────────────────────────────────
            //// 3. Grand Total row
            //// ────────────────────────────────────────────────────────────────────────────
            if (stockData.Any())
            {
                var grand = new
                {
                    CUSTOMER_CODE = "",
                    CUS_CODE = "",
                    SALES_TYPE_CODE = "",
                    CUSTOMER_EDESC = "Grand Total",
                    ITEM_CODE = "",
                    ITEM_EDESC = "",
                    INDEX_MU_CODE = "",
                    MASTER_CUSTOMER_CODE = "",
                    PRE_CUSTOMER_CODE = "",
                    GROUP_SKU_FLAG = "",

                    S_QTY = itemRows.Sum(x => x.S_QTY ?? 0),
                    S_VALUE = itemRows.Sum(x => x.S_VALUE ?? 0),

                    B_QTY = itemRows.Sum(x => x.B_QTY ?? 0),
                    B_VALUE = itemRows.Sum(x => x.B_VALUE ?? 0),

                    A_QTY = itemRows.Sum(x => x.A_QTY ?? 0),
                    A_VALUE = itemRows.Sum(x => x.A_VALUE ?? 0),

                    K_QTY = itemRows.Sum(x => x.K_QTY ?? 0),
                    K_VALUE = itemRows.Sum(x => x.K_VALUE ?? 0),

                    M_QTY = itemRows.Sum(x => x.M_QTY ?? 0),
                    M_VALUE = itemRows.Sum(x => x.M_VALUE ?? 0),

                    P_QTY = itemRows.Sum(x => x.P_QTY ?? 0),
                    P_VALUE = itemRows.Sum(x => x.P_VALUE ?? 0),

                    Mg_QTY = itemRows.Sum(x => x.Mg_QTY ?? 0),
                    Mg_VALUE = itemRows.Sum(x => x.Mg_VALUE ?? 0),

                    F_QTY = itemRows.Sum(x => x.F_QTY ?? 0),
                    F_VALUE = itemRows.Sum(x => x.F_VALUE ?? 0),

                    C_QTY = itemRows.Sum(x => x.C_QTY ?? 0),
                    C_VALUE = itemRows.Sum(x => x.C_VALUE ?? 0),

                    Bh_QTY = itemRows.Sum(x => x.Bh_QTY ?? 0),
                    Bh_VALUE = itemRows.Sum(x => x.Bh_VALUE ?? 0),

                    J_QTY = itemRows.Sum(x => x.J_QTY ?? 0),
                    J_VALUE = itemRows.Sum(x => x.J_VALUE ?? 0),

                    Aa_QTY = itemRows.Sum(x => x.Aa_QTY ?? 0),
                    Aa_VALUE = itemRows.Sum(x => x.Aa_VALUE ?? 0)
                };

                stockData.Add(grand);
            }
            return stockData;

        }

        public dynamic GetMonthwiseEmployeeCustomerNetSales(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"WITH EmployeeCodes AS (
                                            SELECT EMPLOYEE_CODE, EMPLOYEE_EDESC
                                            FROM HR_EMPLOYEE_SETUP
                                            WHERE DELETED_FLAG = 'N'
                                              AND EMPLOYEE_CODE IN (
                                                  SELECT EMPLOYEE_CODE
                                                  FROM SA_SALES_INVOICE
                                                  WHERE DELETED_FLAG = 'N'
                                                    AND COMPANY_CODE IN ({2})
                                                    AND BRANCH_CODE IN ({3})
                                                    AND SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD')
                                                    AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                              )
                                              AND COMPANY_CODE IN ({2})
                                        )
                                        SELECT B.CUSTOMER_CODE, B.CUSTOMER_EDESC, B.MASTER_CUSTOMER_CODE, B.PRE_CUSTOMER_CODE, B.GROUP_SKU_FLAG,
                                               (LENGTH(B.MASTER_CUSTOMER_CODE) - LENGTH(REPLACE(B.MASTER_CUSTOMER_CODE, '.', ''))) ROWLEV,
                                               EC.EMPLOYEE_EDESC, 
                                               A.*
                                        FROM (
                                            SELECT *
                                            FROM (
                                                SELECT A.COMPANY_CODE, A.CUSTOMER_CODE CUS_CODE, A.EMPLOYEE_CODE, ITEM_CODE, ITEM_EDESC, INDEX_MU_CODE, MTH,
                                                       SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE
                                                FROM (
                                                    SELECT MTH, A.COMPANY_CODE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE INDEX_MU_CODE,
                                                           CASE WHEN E.BRAND_NAME IS NULL THEN D.ITEM_EDESC ELSE E.BRAND_NAME END BRAND_NAME,
                                                           SUM(A.SALES_QTY) SALES_QTY, SUM(A.SALES_VALUE) SALES_VALUE,
                                                           SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE,
                                                           SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE
                                                    FROM (
                                                        SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(SALES_DATE), 6, 2) MTH,
                                                               SUM(NVL(A.QUANTITY, 0)) SALES_QTY, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) SALES_VALUE,
                                                               0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE
                                                        FROM SA_SALES_INVOICE A
                                                        WHERE A.DELETED_FLAG = 'N' AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                          AND A.COMPANY_CODE IN ({2}) AND A.BRANCH_CODE IN ({3}) AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes)
                                                        GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE, A.EMPLOYEE_CODE, SALES_DATE
                                                        UNION ALL
                                                        SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(RETURN_DATE), 6, 2) MTH,
                                                               0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY, 0)) SALES_RET_QTY, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) SALES_RET_VALUE,
                                                               0 DEBIT_VALUE, 0 CREDIT_VALUE
                                                        FROM SA_SALES_RETURN A
                                                        WHERE A.DELETED_FLAG = 'N' AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                          AND A.COMPANY_CODE IN ({2}) AND A.BRANCH_CODE IN ({3}) AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes)
                                                        GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE, A.EMPLOYEE_CODE, RETURN_DATE
                                                        UNION ALL
                                                        SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(VOUCHER_DATE), 6, 2) MTH,
                                                               0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) DEBIT_VALUE,
                                                               0 CREDIT_VALUE
                                                        FROM SA_DEBIT_NOTE A
                                                        WHERE A.DELETED_FLAG = 'N' AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                          AND A.COMPANY_CODE IN ({2}) AND A.BRANCH_CODE IN ({3}) AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes)
                                                        GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE, A.EMPLOYEE_CODE, VOUCHER_DATE
                                                        UNION ALL
                                                        SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(VOUCHER_DATE), 6, 2) MTH,
                                                               0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE,
                                                               SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) CREDIT_VALUE
                                                        FROM SA_CREDIT_NOTE A
                                                        WHERE A.DELETED_FLAG = 'N' AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                          AND A.COMPANY_CODE IN ({2}) AND A.BRANCH_CODE IN ({3}) AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes)
                                                        GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE, A.EMPLOYEE_CODE, VOUCHER_DATE
                                                    ) A, SA_CUSTOMER_SETUP B, IP_ITEM_MASTER_SETUP D, IP_ITEM_SPEC_SETUP E
                                                    WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE AND A.ITEM_CODE = D.ITEM_CODE AND A.COMPANY_CODE = D.COMPANY_CODE
                                                      AND A.COMPANY_CODE = B.COMPANY_CODE AND D.GROUP_SKU_FLAG = 'I' AND D.ITEM_CODE = E.ITEM_CODE(+)
                                                      AND D.COMPANY_CODE = E.COMPANY_CODE(+) AND A.COMPANY_CODE IN ({2})
                                                    GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.EMPLOYEE_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE, E.BRAND_NAME, A.COMPANY_CODE, MTH
                                                ) A
                                                ORDER BY CUSTOMER_CODE, ITEM_EDESC
                                            )
                                            PIVOT (
                                                SUM(NET_SALES_QTY) QTY, SUM(NET_SALES_VALUE) VALUE
                                                FOR MTH IN ('04' AS S, '05' AS B, '06' AS A, '07' AS K, '08' AS M, '09' AS P, '10' AS Mg, '11' AS F, '12' AS C, '01' AS Bh, '02' AS J, '03' AS Aa)
                                            )
                                        ) A
                                        INNER JOIN SA_CUSTOMER_SETUP B ON B.CUSTOMER_CODE = A.CUS_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                        INNER JOIN EmployeeCodes EC ON A.EMPLOYEE_CODE = EC.EMPLOYEE_CODE
                                        WHERE B.COMPANY_CODE IN ({2})
                                        ORDER BY EC.EMPLOYEE_CODE", FromDate, ToDate, companyCode, branchCode);
            //string query = string.Format(@"WITH EmployeeCodes AS (
            //                                        SELECT EMPLOYEE_CODE,EMPLOYEE_EDESC
            //                                        FROM HR_EMPLOYEE_SETUP
            //                                        WHERE DELETED_FLAG = 'N'
            //                                        AND EMPLOYEE_CODE IN (
            //                                            SELECT EMPLOYEE_CODE
            //                                            FROM SA_SALES_INVOICE
            //                                            WHERE DELETED_FLAG = 'N'
            //                                            AND COMPANY_CODE IN ({2})
            //                                            AND BRANCH_CODE IN ({3})
            //                                            AND SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
            //                                            AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
            //                                        )
            //                                        AND COMPANY_CODE IN ({2})
            //                                    )
            //                                    SELECT B.CUSTOMER_CODE, B.CUSTOMER_EDESC, B.MASTER_CUSTOMER_CODE, B.PRE_CUSTOMER_CODE, B.GROUP_SKU_FLAG, (LENGTH(B.MASTER_CUSTOMER_CODE) - LENGTH(REPLACE(B.MASTER_CUSTOMER_CODE,'.',''))) ROWLEV, A.*
            //                                    FROM (
            //                                        SELECT *
            //                                        FROM (
            //                                            SELECT A.COMPANY_CODE, A.CUSTOMER_CODE CUS_CODE, ITEM_CODE, ITEM_EDESC, INDEX_MU_CODE, MTH,
            //                                            SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE
            //                                            FROM (
            //                                                SELECT MTH, A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE INDEX_MU_CODE,
            //                                                CASE WHEN E.BRAND_NAME IS NULL THEN D.ITEM_EDESC ELSE E.BRAND_NAME END BRAND_NAME,
            //                                                SUM(A.SALES_QTY) SALES_QTY, SUM(A.SALES_VALUE) SALES_VALUE,
            //                                                SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE,
            //                                                SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE
            //                                                FROM (
            //                                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(SALES_DATE),6,2) MTH,
            //                                                    SUM(NVL(A.QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE
            //                                                    FROM SA_SALES_INVOICE A
            //                                                    WHERE A.DELETED_FLAG = 'N'
            //                                        AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
            //                                                    AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
            //                                                    AND A.COMPANY_CODE IN ({2})
            //                                                    AND A.BRANCH_CODE IN ({3}) 
            //                                                    AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes)  -- DYNAMICALLY APPLIES ALL EMPLOYEE CODES
            //                                                    GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE, SALES_DATE
            //                                                    Union All
            //                                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(RETURN_DATE),6,2) MTH,
            //                                                    0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY,0)) SALES_RET_QTY, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE
            //                                                    FROM SA_SALES_RETURN A
            //                                                    WHERE A.DELETED_FLAG = 'N' 
            //                                        AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
            //                                                    AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
            //                                                    AND A.COMPANY_CODE IN ({2})
            //                                                    AND A.BRANCH_CODE IN ({3})  
            //                                                    AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes)  -- DYNAMICALLY APPLIES ALL EMPLOYEE CODES
            //                                                    GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE, RETURN_DATE
            //                                                    Union All
            //                                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH,
            //                                                    0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE
            //                                                    FROM SA_DEBIT_NOTE A
            //                                                    WHERE A.DELETED_FLAG = 'N' 
            //                                        AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
            //                                                    AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
            //                                                    AND A.COMPANY_CODE IN ({2})
            //                                                    AND A.BRANCH_CODE IN ({3}) 
            //                                                    AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes)  -- DYNAMICALLY APPLIES ALL EMPLOYEE CODES
            //                                                    GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE, VOUCHER_DATE
            //                                                    Union All
            //                                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH,
            //                                                    0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE
            //                                                    FROM SA_CREDIT_NOTE A
            //                                                    WHERE A.DELETED_FLAG = 'N' 
            //                                        AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
            //                                                    AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
            //                                                    AND A.COMPANY_CODE IN ({2})
            //                                                    AND A.BRANCH_CODE IN ({3}) 
            //                                                    AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes)  -- DYNAMICALLY APPLIES ALL EMPLOYEE CODES
            //                                                    GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, A.COMPANY_CODE, VOUCHER_DATE
            //                                                ) A,
            //                                                SA_CUSTOMER_SETUP B, IP_ITEM_MASTER_SETUP D, IP_ITEM_SPEC_SETUP E
            //                                                WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE AND A.ITEM_CODE = D.ITEM_CODE AND A.COMPANY_CODE = D.COMPANY_CODE
            //                                                AND A.COMPANY_CODE = B.COMPANY_CODE AND D.GROUP_SKU_FLAG = 'I' AND D.ITEM_CODE = E.ITEM_CODE (+)
            //                                                AND D.COMPANY_CODE = E.COMPANY_CODE (+) AND A.COMPANY_CODE IN ({2})
            //                                                GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE, E.BRAND_NAME, A.COMPANY_CODE, MTH
            //                                            ) A ORDER BY CUSTOMER_CODE, ITEM_EDESC
            //                                        )
            //                                        PIVOT  
            //                                         (SUM(NET_SALES_QTY) QTY, SUM(NET_SALES_VALUE) VALUE  
            //                                          FOR MTH IN ('04' AS S,
            //                                            '05' AS B,
            //                                            '06' AS A,
            //                                            '07' AS K,
            //                                            '08' AS M,
            //                                            '09' AS P,
            //                                            '10' AS Mg,
            //                                            '11' AS F,
            //                                            '12' AS C,
            //                                            '01' AS Bh,
            //                                            '02' AS J,
            //                                            '03' AS Aa)
            //                                     )  
            //                                       ) 
            //                                     A, SA_CUSTOMER_SETUP B
            //                                    WHERE B.CUSTOMER_CODE = A.CUS_CODE AND B.COMPANY_CODE = A.COMPANY_CODE AND B.COMPANY_CODE IN ({2})
            //                                    ORDER BY B.MASTER_CUSTOMER_CODE, B.CUSTOMER_EDESC, A.ITEM_EDESC", FromDate, ToDate, companyCode, branchCode);
            var monthwise = _objectEntity.SqlQuery<MonthwiseCustProdNetSalesModel>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
            //            var finalData = new List<MonthwiseCustProdNetSalesModel>();

            //            int employeeIndex = 1;
            //            foreach (var group in groupedData.GroupBy(x => x.EMPLOYEE_EDESC))
            //            {
            //                // Create the Employee (level 1) parent row
            //                string employeeId = "EMP-" + employeeIndex.ToString("D3");
            //                var employeeParentRow = group.First();
            //                employeeParentRow.MASTER_CUSTOMER_CODE = employeeId;
            //                employeeParentRow.PRE_CUSTOMER_CODE = null; // Root node, no parent
            //                employeeParentRow.GROUP_SKU_FLAG = "E"; // 'E' for Employee
            //                finalData.Add(employeeParentRow);

            //                int customerIndex = 1;
            //                foreach (var customerGroup in group.GroupBy(x => x.CUSTOMER_EDESC))
            //                {
            //                    // Create the Customer (level 2) parent row
            //                    string customerId = employeeId + "-CUS-" + customerIndex.ToString("D2");
            //                    var customerParentRow = customerGroup.First();
            //                    customerParentRow.MASTER_CUSTOMER_CODE = customerId;
            //                    customerParentRow.PRE_CUSTOMER_CODE = employeeId; // Parent is the employee
            //                    customerParentRow.GROUP_SKU_FLAG = "C"; // 'C' for Customer
            //                    finalData.Add(customerParentRow);

            //                    int productIndex = 1;
            //                    foreach (var product in customerGroup)
            //                    {
            //                        // Assign codes to the Product (level 3) leaf row
            //                        string productId = customerId + "-PROD-" + productIndex.ToString("D2");
            //                        product.MASTER_CUSTOMER_CODE = productId;
            //                        product.PRE_CUSTOMER_CODE = customerId; // Parent is the customer
            //                        product.GROUP_SKU_FLAG = "I"; // 'I' for Item/Product
            //                        finalData.Add(product);
            //                        productIndex++;
            //                    }
            //                    customerIndex++;
            //                }
            //                employeeIndex++;
            //            }

            //            List<dynamic> stockData = new List<dynamic>();
            //var itemRows = groupedData.Where(r => r.GROUP_SKU_FLAG == "I").ToList();

            //foreach (var row in groupedData)
            //{
            //    double total1 = 0;  // S_QTY
            //    double total2 = 0;  // S_VALUE
            //    double total3 = 0;  // B_QTY
            //    double total4 = 0;  // B_VALUE
            //    double total5 = 0;  // A_QTY
            //    double total6 = 0;  // A_VALUE
            //    double total7 = 0;  // K_QTY
            //    double total8 = 0;  // K_VALUE
            //    double total9 = 0;  // M_QTY
            //    double total10 = 0; // M_VALUE
            //    double total11 = 0; // P_QTY
            //    double total12 = 0; // P_VALUE
            //    double total13 = 0; // Mg_QTY
            //    double total14 = 0; // Mg_VALUE
            //    double total15 = 0; // F_QTY
            //    double total16 = 0; // F_VALUE
            //    double total17 = 0; // C_QTY
            //    double total18 = 0; // C_VALUE
            //    double total19 = 0; // Bh_QTY
            //    double total20 = 0; // Bh_VALUE
            //    double total21 = 0; // J_VALUE
            //    double total22 = 0; // J_VALUE
            //    double total23 = 0; // Aa_VALUE
            //    double total24 = 0; // Aa_VALUE
            //  if (row.GROUP_SKU_FLAG == "I")
            //  {
            //        total1 = row.S_QTY ?? 0;
            //        total2 = row.S_VALUE ?? 0;
            //        total3 = row.B_QTY ?? 0;
            //        total4 = row.B_VALUE ?? 0;
            //        total5 = row.A_QTY ?? 0;
            //        total6 = row.A_VALUE ?? 0;
            //        total7 = row.K_QTY ?? 0;
            //        total8 = row.K_VALUE ?? 0;
            //        total9 = row.M_QTY ?? 0;
            //        total10 = row.M_VALUE ?? 0;
            //        total11 = row.P_QTY ?? 0;
            //        total12 = row.P_VALUE ?? 0;
            //        total13 = row.Mg_QTY ?? 0;
            //        total14 = row.Mg_VALUE ?? 0;
            //        total15 = row.F_QTY ?? 0;
            //        total16 = row.F_VALUE ?? 0;
            //        total17 = row.C_QTY ?? 0;
            //        total18 = row.C_VALUE ?? 0;
            //        total19 = row.Bh_QTY ?? 0;
            //        total20 = row.Bh_VALUE ?? 0;
            //        total21 = row.J_QTY ?? 0;
            //        total22 = row.J_VALUE ?? 0;
            //        total23 = row.Aa_QTY ?? 0;
            //        total24 = row.Aa_VALUE ?? 0;
            //   }

            //    // Add only if any total is non-zero
            //    if (
            //        total1 != 0 || total2 != 0 || total3 != 0 || total4 != 0 ||
            //        total5 != 0 || total6 != 0 || total7 != 0 || total8 != 0 ||
            //        total9 != 0 || total10 != 0 || total11 != 0 || total12 != 0 ||
            //        total13 != 0 || total14 != 0 || total15 != 0 || total16 != 0 ||
            //        total17 != 0 || total18 != 0 || total19 != 0 || total20 != 0 ||
            //        total21 != 0 || total22 != 0 || total23 != 0 || total24 != 0
            //    )
            //    {
            //        stockData.Add(new
            //        {
            //            CUSTOMER_CODE = row.CUSTOMER_CODE,
            //            CUS_CODE = row.CUS_CODE, 
            //            CUSTOMER_EDESC = row.CUSTOMER_EDESC,
            //            EMPLOYEE_EDESC = row.EMPLOYEE_EDESC,
            //            EMPLOYEE_CODE = row.EMPLOYEE_CODE,
            //            ITEM_CODE = row.ITEM_CODE,
            //            ITEM_EDESC = row.ITEM_EDESC,
            //            INDEX_MU_CODE = row.INDEX_MU_CODE,
            //            GROUP_SKU_FLAG = row.GROUP_SKU_FLAG,
            //            MASTER_CUSTOMER_CODE = row.MASTER_CUSTOMER_CODE,
            //            PRE_CUSTOMER_CODE = row.PRE_CUSTOMER_CODE,

            //            S_QTY = total1,
            //            S_VALUE = total2,
            //            B_QTY = total3,
            //            B_VALUE = total4,
            //            A_QTY = total5,
            //            A_VALUE = total6,
            //            K_QTY = total7,
            //            K_VALUE = total8,
            //            M_QTY = total9,
            //            M_VALUE = total10,
            //            P_QTY = total11,
            //            P_VALUE = total12,
            //            Mg_QTY = total13,
            //            Mg_VALUE = total14,
            //            F_QTY = total15,
            //            F_VALUE = total16,
            //            C_QTY = total17,
            //            C_VALUE = total18,
            //            Bh_QTY = total19,
            //            Bh_VALUE = total20,
            //            J_QTY = total21,
            //            J_VALUE = total22,
            //            Aa_QTY = total23,
            //            Aa_VALUE = total24
            //        });
            //    }
            //}

            ////// ────────────────────────────────────────────────────────────────────────────
            ////// 3. Grand Total row
            ////// ────────────────────────────────────────────────────────────────────────────
            //if (stockData.Any())
            //{
            //    var grand = new
            //    {
            //        CUSTOMER_CODE = "",
            //        CUS_CODE = "",
            //        CUSTOMER_EDESC = "Grand Total",
            //        EMPLOYEE_EDESC = "",
            //        EMPLOYEE_CODE = "",
            //        ITEM_CODE = "",
            //        ITEM_EDESC = "",
            //        INDEX_MU_CODE = "",
            //        MASTER_CUSTOMER_CODE = "",
            //        PRE_CUSTOMER_CODE = "",
            //        GROUP_SKU_FLAG = "",

            //        S_QTY = itemRows.Sum(x => x.S_QTY ?? 0),
            //        S_VALUE = itemRows.Sum(x => x.S_VALUE ?? 0),

            //        B_QTY = itemRows.Sum(x => x.B_QTY ?? 0),
            //        B_VALUE = itemRows.Sum(x => x.B_VALUE ?? 0),

            //        A_QTY = itemRows.Sum(x => x.A_QTY ?? 0),
            //        A_VALUE = itemRows.Sum(x => x.A_VALUE ?? 0),

            //        K_QTY = itemRows.Sum(x => x.K_QTY ?? 0),
            //        K_VALUE = itemRows.Sum(x => x.K_VALUE ?? 0),

            //        M_QTY = itemRows.Sum(x => x.M_QTY ?? 0),
            //        M_VALUE = itemRows.Sum(x => x.M_VALUE ?? 0),

            //        P_QTY = itemRows.Sum(x => x.P_QTY ?? 0),
            //        P_VALUE = itemRows.Sum(x => x.P_VALUE ?? 0),

            //        Mg_QTY = itemRows.Sum(x => x.Mg_QTY ?? 0),
            //        Mg_VALUE = itemRows.Sum(x => x.Mg_VALUE ?? 0),

            //        F_QTY = itemRows.Sum(x => x.F_QTY ?? 0),
            //        F_VALUE = itemRows.Sum(x => x.F_VALUE ?? 0),

            //        C_QTY = itemRows.Sum(x => x.C_QTY ?? 0),
            //        C_VALUE = itemRows.Sum(x => x.C_VALUE ?? 0),

            //        Bh_QTY = itemRows.Sum(x => x.Bh_QTY ?? 0),
            //        Bh_VALUE = itemRows.Sum(x => x.Bh_VALUE ?? 0),

            //        J_QTY = itemRows.Sum(x => x.J_QTY ?? 0),
            //        J_VALUE = itemRows.Sum(x => x.J_VALUE ?? 0),

            //        Aa_QTY = itemRows.Sum(x => x.Aa_QTY ?? 0),
            //        Aa_VALUE = itemRows.Sum(x => x.Aa_VALUE ?? 0)
            //    };

            //    stockData.Add(grand);
            //}
            //return stockData;
        }

        public dynamic GetMonthwiseEmployeeProductNetSales(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"WITH EmployeeCodes AS (
                                            SELECT EMPLOYEE_CODE, EMPLOYEE_EDESC
                                            FROM HR_EMPLOYEE_SETUP
                                            WHERE DELETED_FLAG = 'N'
                                              AND EMPLOYEE_CODE IN (
                                                  SELECT EMPLOYEE_CODE
                                                  FROM SA_SALES_INVOICE
                                                  WHERE DELETED_FLAG = 'N'
                                                    AND COMPANY_CODE IN ({2})
                                                    AND BRANCH_CODE IN ({3})
                                                    AND SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD')
                                                    AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                              )
                                              AND COMPANY_CODE IN ({2})
                                        )
                                        SELECT *
                                        FROM
                                        (
                                            SELECT
                                                A.COMPANY_CODE,
                                                A.CUSTOMER_CODE CUS_CODE,
                                                A.ITEM_CODE,
                                                A.ITEM_EDESC,
                                                A.INDEX_MU_CODE,
                                                A.EMPLOYEE_CODE,
                                                A.EMPLOYEE_EDESC,
                                                A.MTH,
                                                (A.SALES_QTY - A.SALES_RET_QTY) AS NET_SALES_QTY,
                                                (A.SALES_VALUE - A.SALES_RET_VALUE + A.DEBIT_VALUE - A.CREDIT_VALUE) AS NET_SALES_VALUE
                                            FROM (
                                                SELECT
                                                    MTH,
                                                    A.COMPANY_CODE,
                                                    A.CUSTOMER_CODE,
                                                    A.ITEM_CODE,
                                                    D.ITEM_EDESC,
                                                    D.INDEX_MU_CODE,
                                                    A.EMPLOYEE_CODE,
                                                    A.EMPLOYEE_EDESC,
                                                    SUM(A.SALES_QTY) AS SALES_QTY,
                                                    SUM(A.SALES_VALUE) AS SALES_VALUE,
                                                    SUM(A.SALES_RET_QTY) AS SALES_RET_QTY,
                                                    SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE,
                                                    SUM(DEBIT_VALUE) AS DEBIT_VALUE,
                                                    SUM(CREDIT_VALUE) AS CREDIT_VALUE
                                                FROM (
                                                    SELECT
                                                        A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, A.EMPLOYEE_CODE, E.EMPLOYEE_EDESC,
                                                        SUBSTR(BS_DATE(SALES_DATE), 6, 2) AS MTH,
                                                        SUM(NVL(A.QUANTITY, 0)) AS SALES_QTY,
                                                        SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_VALUE,
                                                        0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE, 0 AS DEBIT_VALUE, 0 AS CREDIT_VALUE
                                                    FROM SA_SALES_INVOICE A
                                                    JOIN HR_EMPLOYEE_SETUP E ON A.EMPLOYEE_CODE = E.EMPLOYEE_CODE AND A.COMPANY_CODE = E.COMPANY_CODE
                                                    WHERE A.DELETED_FLAG = 'N' 
            
                                                    AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes) 
                                                    AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3})
                                                    GROUP BY A.ITEM_CODE, A.COMPANY_CODE, SALES_DATE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, E.EMPLOYEE_EDESC

                                                    UNION ALL
                                                    SELECT
                                                        A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, A.EMPLOYEE_CODE, E.EMPLOYEE_EDESC,
                                                        SUBSTR(BS_DATE(RETURN_DATE), 6, 2) AS MTH,
                                                        0 AS SALES_QTY, 0 AS SALES_VALUE,
                                                        SUM(NVL(A.QUANTITY, 0)) AS SALES_RET_QTY,
                                                        SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS SALES_RET_VALUE,
                                                        0 AS DEBIT_VALUE, 0 AS CREDIT_VALUE
                                                    FROM SA_SALES_RETURN A
                                                    JOIN HR_EMPLOYEE_SETUP E ON A.EMPLOYEE_CODE = E.EMPLOYEE_CODE AND A.COMPANY_CODE = E.COMPANY_CODE
                                                    WHERE A.DELETED_FLAG = 'N' 
                                                    AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes) 
                                                    AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3})
                                                    GROUP BY A.ITEM_CODE, A.COMPANY_CODE, RETURN_DATE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, E.EMPLOYEE_EDESC

                                                    UNION ALL
                                                    SELECT
                                                        A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, A.EMPLOYEE_CODE, E.EMPLOYEE_EDESC,
                                                        SUBSTR(BS_DATE(VOUCHER_DATE), 6, 2) AS MTH,
                                                        0 AS SALES_QTY, 0 AS SALES_VALUE, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE,
                                                        SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS DEBIT_VALUE, 0 AS CREDIT_VALUE
                                                    FROM SA_DEBIT_NOTE A
                                                    JOIN HR_EMPLOYEE_SETUP E ON A.EMPLOYEE_CODE = E.EMPLOYEE_CODE AND A.COMPANY_CODE = E.COMPANY_CODE
                                                    WHERE A.DELETED_FLAG = 'N' 
                                                    AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes) 
                                                    AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3})
                                                    GROUP BY A.ITEM_CODE, A.COMPANY_CODE, VOUCHER_DATE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, E.EMPLOYEE_EDESC

                                                    UNION ALL
                                                    SELECT
                                                        A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, A.EMPLOYEE_CODE, E.EMPLOYEE_EDESC,
                                                        SUBSTR(BS_DATE(VOUCHER_DATE), 6, 2) AS MTH,
                                                        0 AS SALES_QTY, 0 AS SALES_VALUE, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE, 0 AS DEBIT_VALUE,
                                                        SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE, 0)) AS CREDIT_VALUE
                                                    FROM SA_CREDIT_NOTE A
                                                    JOIN HR_EMPLOYEE_SETUP E ON A.EMPLOYEE_CODE = E.EMPLOYEE_CODE AND A.COMPANY_CODE = E.COMPANY_CODE
                                                    WHERE A.DELETED_FLAG = 'N' 
                                                    AND A.EMPLOYEE_CODE IN (SELECT EMPLOYEE_CODE FROM EmployeeCodes)
                                                    AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3})
                                                    GROUP BY A.ITEM_CODE, A.COMPANY_CODE, VOUCHER_DATE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, E.EMPLOYEE_EDESC

                                                ) A, IP_ITEM_MASTER_SETUP D
                                                WHERE A.ITEM_CODE = D.ITEM_CODE
                                                  AND A.COMPANY_CODE = D.COMPANY_CODE
                                                  AND D.GROUP_SKU_FLAG = 'I'
                                                GROUP BY A.CUSTOMER_CODE, A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE, A.COMPANY_CODE, MTH, A.EMPLOYEE_CODE, A.EMPLOYEE_EDESC
                                            ) A
                                        )
                                        PIVOT (
                                            SUM(NET_SALES_QTY) QTY,
                                            SUM(NET_SALES_VALUE) VALUE
                                            FOR MTH IN ('04' as S, '05' as B, '06' as A, '07' as K, '08' as M, '09' as P, '10' as Mg, '11' as F, '12' as C, '01' as Bh, '02' as J, '03' as Aa)
                                        )
                                        ORDER BY ITEM_EDESC", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<MonthwiseCustProdNetSalesModel>(query).ToList();
            var groupedData = monthwise;
            return groupedData;

        }

        public dynamic GetMonthwiseRegionProductSales(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT * FROM (  
                                                SELECT A.COMPANY_CODE, REGION_EDESC,  ITEM_CODE, ITEM_EDESC, INDEX_MU_CODE, MTH,   
                                                SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE, FREE_QTY , SALES_QTY - SALES_RET_QTY + FREE_QTY NET_QTY FROM (   
                                                SELECT MTH,A.COMPANY_CODE, F.REGION_EDESC, A.ITEM_CODE, D.ITEM_EDESC,  D.INDEX_MU_CODE INDEX_MU_CODE, CASE WHEN E.BRAND_NAME IS NULL THEN D.ITEM_EDESC ELSE E.BRAND_NAME END BRAND_NAME,  SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE, SUM(NVL(FREE_QTY,0)) FREE_QTY  FROM (   
                                                SELECT A.COMPANY_CODE, A.CUSTOMER_CODE,  A.ITEM_CODE, SUBSTR(BS_DATE(SALES_DATE),6,2) MTH, SUM(NVL(A.QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE , SUM(NVL(FREE_QTY,0)) FREE_QTY   FROM SA_SALES_INVOICE A   
                                                WHERE A.DELETED_FLAG = 'N'   
                                                 AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                 AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                 AND A.COMPANY_CODE IN ({2})
                                                 AND A.BRANCH_CODE IN ({3})  
                                                GROUP BY  A.CUSTOMER_CODE,  A.ITEM_CODE , A.COMPANY_CODE, SALES_DATE  
                                                Union All   
                                                SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(RETURN_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY,0))  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE , SUM(NVL(FREE_QTY,0)) * -1 FREE_QTY  FROM SA_SALES_RETURN A   
                                                WHERE A.DELETED_FLAG = 'N'   
                                                 AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                 AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                 AND A.COMPANY_CODE IN ({2})
                                                 AND A.BRANCH_CODE IN ({3})  
                                                GROUP BY  A.CUSTOMER_CODE, A.ITEM_CODE , A.COMPANY_CODE, RETURN_DATE  
                                                Union All   
                                                SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE, 0 FREE_QTY  FROM SA_DEBIT_NOTE A   
                                                WHERE A.DELETED_FLAG = 'N'   
                                                 AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                 AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                 AND A.COMPANY_CODE IN ({2})
                                                 AND A.BRANCH_CODE IN ({3})  
                                                GROUP BY  A.CUSTOMER_CODE, A.ITEM_CODE , A.COMPANY_CODE, VOUCHER_DATE  
                                                Union All   
                                                SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.ITEM_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE, 0 FREE_QTY  FROM SA_CREDIT_NOTE A   
                                                WHERE A.DELETED_FLAG = 'N'   
                                                 AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                 AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                 AND A.COMPANY_CODE IN ({2})
                                                 AND A.BRANCH_CODE IN ({3})  
                                                GROUP BY  A.CUSTOMER_CODE, A.ITEM_CODE , A.COMPANY_CODE, VOUCHER_DATE  
                                                ORDER BY CUSTOMER_CODE) A,  SA_CUSTOMER_SETUP B ,  IP_ITEM_MASTER_SETUP D, IP_ITEM_SPEC_SETUP E, REGION_CODE F   
                                                WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE   
                                                AND  A.ITEM_CODE = D.ITEM_CODE   
                                                AND A.COMPANY_CODE = D.COMPANY_CODE   
                                                AND A.COMPANY_CODE = B.COMPANY_CODE   
                                                AND D.GROUP_SKU_FLAG = 'I'  
                                                AND D.ITEM_CODE = E.ITEM_CODE (+)  
                                                AND D.COMPANY_CODE = E.COMPANY_CODE (+)  
                                                AND B.REGION_CODE = F.REGION_CODE  
                                                AND A.COMPANY_CODE IN ({2})  
                                                GROUP BY    F.REGION_EDESC, A.ITEM_CODE, D.ITEM_EDESC, D.INDEX_MU_CODE, E.BRAND_NAME, A.COMPANY_CODE  , MTH  
                                                ) A ORDER BY ITEM_EDESC  
                                                )  
                                                PIVOT  
                                                (SUM(NET_SALES_QTY) QTY, SUM(NET_SALES_VALUE) VALUE , SUM(FREE_QTY),  SUM(NET_QTY) NET_QTY  
                                                FOR MTH IN ('04' as S, '05' as B, '06' as A, '07' as K, '08' as M, '09' as P, '10' as Mg, '11' as F, '12' as C, '01' as Bh, '02' as J, '03' as Aa) 
                                                )  
                                                ORDER BY REGION_EDESC, ITEM_EDESC  ", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<MonthwiseCustProdNetSalesModel>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
        }

        public dynamic GetMonthwiseRegionEmployeeNetSales(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT * FROM (  
                                                    SELECT A.COMPANY_CODE, REGION_EDESC,  EMPLOYEE_CODE, EMPLOYEE_EDESC, '' INDEX_MU_CODE, MTH,   
                                                    SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE, FREE_QTY, SALES_QTY - SALES_RET_QTY + FREE_QTY NET_QTY FROM (   
                                                    SELECT MTH,A.COMPANY_CODE, A.CUSTOMER_CODE, F.REGION_EDESC, A.EMPLOYEE_CODE, D.EMPLOYEE_EDESC,  '' INDEX_MU_CODE, SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE,  SUM(NVL(FREE_QTY,0)) FREE_QTY  FROM (   
                                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE,  A.EMPLOYEE_CODE, SUBSTR(BS_DATE(SALES_DATE),6,2) MTH, SUM(NVL(A.QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY * A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, SUM(NVL(FREE_QTY,0)) FREE_QTY   FROM SA_SALES_INVOICE A   
                                                    WHERE A.DELETED_FLAG = 'N'   
                                                    AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3}) 
                                                    GROUP BY  A.CUSTOMER_CODE,  A.EMPLOYEE_CODE , A.COMPANY_CODE, SALES_DATE  
                                                    Union All   
                                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, SUBSTR(BS_DATE(RETURN_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY,0))  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE , SUM(NVL(FREE_QTY,0)) * -1 FREE_QTY  FROM SA_SALES_RETURN A   
                                                    WHERE A.DELETED_FLAG = 'N'   
                                                    AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3}) 
                                                    GROUP BY  A.CUSTOMER_CODE, A.EMPLOYEE_CODE , A.COMPANY_CODE, RETURN_DATE  
                                                    Union All   
                                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE, 0 FREE_QTY  FROM SA_DEBIT_NOTE A   
                                                    WHERE A.DELETED_FLAG = 'N'   
                                                    AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3})  
                                                    GROUP BY  A.CUSTOMER_CODE, A.EMPLOYEE_CODE , A.COMPANY_CODE, VOUCHER_DATE  
                                                    Union All   
                                                    SELECT A.COMPANY_CODE, A.CUSTOMER_CODE, A.EMPLOYEE_CODE, SUBSTR(BS_DATE(VOUCHER_DATE),6,2) MTH, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE, 0 FREE_QTY  FROM SA_CREDIT_NOTE A   
                                                    WHERE A.DELETED_FLAG = 'N'   
                                                    AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                    AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                    AND A.COMPANY_CODE IN ({2})
                                                    AND A.BRANCH_CODE IN ({3})
                                                    GROUP BY  A.CUSTOMER_CODE, A.EMPLOYEE_CODE , A.COMPANY_CODE, VOUCHER_DATE  
                                                    ORDER BY CUSTOMER_CODE) A,  SA_CUSTOMER_SETUP B ,  HR_EMPLOYEE_SETUP D, REGION_CODE F   
                                                    WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE   
                                                    AND  A.EMPLOYEE_CODE = D.EMPLOYEE_CODE   
                                                    AND A.COMPANY_CODE = D.COMPANY_CODE   
                                                    AND A.COMPANY_CODE = B.COMPANY_CODE   
                                                    AND D.GROUP_SKU_FLAG = 'I'  
                                                    AND B.REGION_CODE = F.REGION_CODE  
                                                    AND A.COMPANY_CODE IN ({2})  
                                                    GROUP BY   A.CUSTOMER_CODE, F.REGION_EDESC, A.EMPLOYEE_CODE, D.EMPLOYEE_EDESC,  A.COMPANY_CODE  , MTH  
                                                    ) A ORDER BY CUSTOMER_CODE, EMPLOYEE_EDESC  
                                                    )  
                                                    PIVOT  
                                                    (SUM(NET_SALES_QTY) QTY, SUM(NET_SALES_VALUE) VALUE, SUM(FREE_QTY) FREE_QTY,  SUM(NET_QTY) NET_QTY  
                                                    FOR MTH IN ('04' as S, '05' as B, '06' as A, '07' as K, '08' as M, '09' as P, '10' as Mg, '11' as F, '12' as C, '01' as Bh, '02' as J, '03' as Aa)  
                                                    )  
                                                    ORDER BY REGION_EDESC, EMPLOYEE_EDESC  
                                                    ", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<MonthwiseCustProdNetSalesModel>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
        }

        public dynamic GetBrandwiseEmployeeCustomerNetSales(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";
            string FromDate = "2024-Jul-17";
            string ToDate = "2025-Jul-17";
            branchCode = "01.01";
            companyCode = "01";
            //string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            //string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            // Step 1: Query for the list of dynamic brand names
            string brandQuery = string.Format(@"SELECT DISTINCT SUBSTR(TRIM(BRAND_NAME), 1, 26) AS BRANDNAME
                                                FROM IP_ITEM_SPEC_SETUP
                                                WHERE COMPANY_CODE IN ({0})
                                                  AND DELETED_FLAG = 'N'
                                                  AND EXISTS (
                                                      SELECT 1
                                                      FROM SA_SALES_INVOICE
                                                      WHERE DELETED_FLAG = 'N'
                                                        AND COMPANY_CODE IN ({0})
                                                        AND BRANCH_CODE IN ({1})
                                                        AND SALES_DATE >= TO_DATE('{2}', 'YYYY-MM-DD')
                                                        AND SALES_DATE <= TO_DATE('{3}', 'YYYY-MM-DD')
                                                        AND IP_ITEM_SPEC_SETUP.ITEM_CODE = SA_SALES_INVOICE.ITEM_CODE
                                                  )
                                                ORDER BY BRANDNAME", companyCode, branchCode, FromDate, ToDate);
            var brandNamesList = _objectEntity.SqlQuery<string>(brandQuery).ToList();
            // FIX: Ensure the list itself is not null and then filter out any null elements within it.
            if (brandNamesList == null)
            {
                brandNamesList = new List<string>();
            }
            // Add 'NA' if it's not present
            if (!brandNamesList.Contains("NA"))
            {
                brandNamesList.Add("NA");
            }
            // FIX: Filter out nulls before selecting and formatting the strings
            var pivotBrandNames = string.Join(", ", brandNamesList.Where(b => b != null).Select(b => $"'{b.Replace("'", "''")}'"));
            string finalQuery = string.Format(@"WITH EmployeeCodes AS (
                                                SELECT EMPLOYEE_CODE, EMPLOYEE_EDESC
                                                FROM HR_EMPLOYEE_SETUP
                                                WHERE DELETED_FLAG = 'N'
                                                  AND EXISTS (
                                                      SELECT 1
                                                      FROM SA_SALES_INVOICE
                                                      WHERE DELETED_FLAG = 'N'
                                                        AND COMPANY_CODE IN ({2})
                                                        AND BRANCH_CODE IN ({3})
                                                        AND SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD')
                                                        AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                        AND SA_SALES_INVOICE.EMPLOYEE_CODE = HR_EMPLOYEE_SETUP.EMPLOYEE_CODE
                                                  )
                                                  AND COMPANY_CODE IN ({2})
                                            ),
                                            SalesData AS (
                                                SELECT
                                                    A.COMPANY_CODE,
                                                    A.CUSTOMER_CODE AS CUS_CODE,
                                                    B.CUSTOMER_EDESC,
                                                    A.EMPLOYEE_CODE,
                                                    C.EMPLOYEE_EDESC,
                                                    CASE WHEN E.BRAND_NAME IS NULL THEN 'NA' ELSE SUBSTR(TRIM(E.BRAND_NAME), 1, 26) END AS BRAND_NAME,
                                                    SUM(A.NET_SALES_QTY) AS NET_SALES_QTY,
                                                    SUM(A.NET_SALES_VALUE) AS NET_SALES_VALUE
                                                FROM (
                                                    -- Union all your sales/return/debit/credit transactions
                                                    SELECT COMPANY_CODE, CUSTOMER_CODE, ITEM_CODE, EMPLOYEE_CODE,
                                                        NVL(QUANTITY, 0) AS NET_SALES_QTY,
                                                        NVL(QUANTITY * NET_GROSS_RATE, 0) AS NET_SALES_VALUE
                                                    FROM SA_SALES_INVOICE
                                                    WHERE DELETED_FLAG = 'N'
                                                      AND SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                      AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3})

                                                    UNION ALL

                                                    SELECT COMPANY_CODE, CUSTOMER_CODE, ITEM_CODE, EMPLOYEE_CODE,
                                                        -NVL(QUANTITY, 0),
                                                        -NVL(QUANTITY * NET_GROSS_RATE, 0)
                                                    FROM SA_SALES_RETURN
                                                    WHERE DELETED_FLAG = 'N'
                                                      AND RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                      AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3})

                                                    UNION ALL

                                                    SELECT COMPANY_CODE, CUSTOMER_CODE, ITEM_CODE, EMPLOYEE_CODE,
                                                        0,
                                                        NVL(QUANTITY * NET_GROSS_RATE, 0)
                                                    FROM SA_DEBIT_NOTE
                                                    WHERE DELETED_FLAG = 'N'
                                                      AND VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                      AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3})

                                                    UNION ALL

                                                    SELECT COMPANY_CODE, CUSTOMER_CODE, ITEM_CODE, EMPLOYEE_CODE,
                                                        0,
                                                        -NVL(QUANTITY * NET_GROSS_RATE, 0)
                                                    FROM SA_CREDIT_NOTE
                                                    WHERE DELETED_FLAG = 'N'
                                                      AND VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                      AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3})
                                                ) A
                                                JOIN SA_CUSTOMER_SETUP B ON A.CUSTOMER_CODE = B.CUSTOMER_CODE AND A.COMPANY_CODE = B.COMPANY_CODE
                                                JOIN IP_ITEM_MASTER_SETUP D ON A.ITEM_CODE = D.ITEM_CODE AND A.COMPANY_CODE = D.COMPANY_CODE AND D.GROUP_SKU_FLAG = 'I'
                                                LEFT JOIN IP_ITEM_SPEC_SETUP E ON D.ITEM_CODE = E.ITEM_CODE AND D.COMPANY_CODE = E.COMPANY_CODE
                                                JOIN EmployeeCodes C ON A.EMPLOYEE_CODE = C.EMPLOYEE_CODE
                                                WHERE A.COMPANY_CODE IN ({2})
                                                GROUP BY A.COMPANY_CODE, A.CUSTOMER_CODE, B.CUSTOMER_EDESC, A.EMPLOYEE_CODE, C.EMPLOYEE_EDESC, CASE WHEN E.BRAND_NAME IS NULL THEN 'NA' ELSE SUBSTR(TRIM(E.BRAND_NAME), 1, 26) END
                                            )
                                            SELECT * FROM SalesData
                                            PIVOT (
                                                SUM(NET_SALES_QTY) AS Q,
                                                SUM(NET_SALES_VALUE) AS V
                                                FOR BRAND_NAME IN({4})
                                            )
                                            ORDER BY EMPLOYEE_EDESC", FromDate, ToDate, companyCode, branchCode, pivotBrandNames);


            var monthwise = _objectEntity.SqlQuery<EmployeeBrandwiseSales>(finalQuery).ToList();
            return monthwise;
        }

        public List<BillwisePurchaseSummaryViewModel> GetBillwisePurchaseSummary(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();

            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";

            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";

            string query = string.Format(@"SELECT INVOICE_DATE, MITI, ORDER_NO, INVOICE_NO, MANUAL_NO, SUPPLIER_CODE, SUPPLIER_EDESC, ADDRESS, TPIN_VAT_NO  
                                                                , SUM(QUANTITY) QUANTITY, SUM(TOTAL_PRICE) TOTAL_PRICE, SUM(NVL(SECOND_QUANTITY,0)) SECOND_QUANTITY, SUM(NVL(THIRD_QUANTITY,0)) THIRD_QUANTITY, SUM(NVL(ROLL_QTY,0))ROLL_QTY  , SUM(EXCISE_DUTY) EXCISE_DUTY , SUM(DISCOUNT) DISCOUNT, SUM(CASH_DISCOUNT) CASH_DISCOUNT, SUM(SPECIAL_DISCOUNT) SPECIAL_DISCOUNT, SUM(YEARLY_DISCOUNT) YEARLY_DISCOUNT, SUM(BILL_DISCOUNT) BILL_DISCOUNT, SUM(LUX_TAX) LUX_TAX, SUM(ADD_CHARGE) ADD_CHARGE, (SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) - (SUM(DISCOUNT) + SUM(CASH_DISCOUNT) + SUM(SPECIAL_DISCOUNT) + SUM(YEARLY_DISCOUNT) + SUM(BILL_DISCOUNT)) + SUM(LUX_TAX) + SUM(ADD_CHARGE)) TAXABLE_TOTAL_PRICE  
                                                                , SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE ,(SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) + SUM(LUX_TAX) + SUM(ADD_CHARGE) - SUM(DISCOUNT) - SUM(CASH_DISCOUNT) + SUM(VAT_TOTAL_PRICE)) INVOICE_TOTAL_PRICE, SUM(TOTAL_IN_NRS) TOTAL_IN_NRS, SUM(LANDED_IN_NRS) LANDED_IN_NRS , VEHICLE_NO, DESTINATION  
                                                                ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , FISCAL, FORM_CODE, EXCHANGE_RATE, CURRENCY_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, PP_NO  FROM (  
                                                                SELECT A.FORM_CODE, A.MANUAL_NO, A.INVOICE_DATE, BS_DATE(A.INVOICE_DATE) MITI , FN_GET_SECOND_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.INVOICE_NO, 1) ORDER_NO, A.INVOICE_NO ,  
                                                                F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO,  
                                                                a.QUANTITY , a.TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, ROLL_QTY, A.EXCHANGE_RATE, A.CURRENCY_CODE, A.TOTAL_PRICE * A.EXCHANGE_RATE TOTAL_IN_NRS, A.CALC_TOTAL_PRICE * A.EXCHANGE_RATE LANDED_IN_NRS, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, (SELECT PP_NO FROM FA_PP_DETAIL_TRANSACTION WHERE REFERENCE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE AND FORM_CODE = A.FORM_CODE AND ROWNUM = 1) PP_NO    
                                                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) EXCISE_DUTY  
                                                                ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) EXCISE_DUTYII  
                                                                , ROUND((NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2}))   AND APPLY_ON = 'D'  
                                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2)  DISCOUNT  
                                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) CASH_DISCOUNT  
                                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) BILL_DISCOUNT  
                                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) LUX_TAX  
                                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE  FROM CHARGE_TRANSACTION X  
                                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'A' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) ADD_CHARGE  
                                                                , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2})) AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                                                 NVL(VAT_AMOUNT,0)  ELSE  
                                                                ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE  
                                                                ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION  
                                                                ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO  
                                                                ,FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS  
                                                                ,(SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL   
                                                                FROM IP_PURCHASE_INVOICE A, SHIPPING_TRANSACTION D, IP_SUPPLIER_SETUP F    
                                                                WHERE A.SUPPLIER_CODE = F.SUPPLIER_CODE  
                                                                AND A.COMPANY_CODE = F.COMPANY_CODE  
                                                                AND A.INVOICE_NO = D.VOUCHER_NO (+)  
                                                                AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                                                AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                                AND A.COMPANY_CODE IN ({2})
                                                                AND A.BRANCH_CODE IN ({3})   
                                                                AND A.DELETED_FLAG = 'N'  
                                                                ORDER BY INVOICE_NO, A.SERIAL_NO  
                                                                )  
                                                                GROUP BY INVOICE_DATE, MITI, ORDER_NO, INVOICE_NO, MANUAL_NO, SUPPLIER_CODE, SUPPLIER_EDESC, ADDRESS, TPIN_VAT_NO, VEHICLE_NO, DESTINATION, DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , FISCAL, FORM_CODE, EXCHANGE_RATE, CURRENCY_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, PP_NO   ORDER BY INVOICE_NO  

                                                                ", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var billwiseReturn = _objectEntity.SqlQuery<BillwisePurchaseSummaryViewModel>(query).ToList();
            return billwiseReturn;

        }

        public List<BillwisePurchaseSummaryViewModel> GetBillwisePurchaseReturnSummary(ReportFiltersModel filters, User userinfo)
        {

            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";

            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";

            string query = string.Format(@"SELECT RETURN_DATE, MITI, ORDER_NO, RETURN_NO, MANUAL_NO, SUPPLIER_CODE, SUPPLIER_EDESC, ADDRESS, TPIN_VAT_NO  
                                            , SUM(QUANTITY) QUANTITY, SUM(TOTAL_PRICE) TOTAL_PRICE, SUM(NVL(SECOND_QUANTITY,0)) SECOND_QUANTITY, SUM(NVL(THIRD_QUANTITY,0)) THIRD_QUANTITY, SUM(NVL(ROLL_QTY,0))ROLL_QTY , SUM(EXCISE_DUTY) EXCISE_DUTY , SUM(EXCISE_DUTYII) EXCISE_DUTYII, SUM(DISCOUNT) DISCOUNT, SUM(CASH_DISCOUNT) CASH_DISCOUNT, SUM(SPECIAL_DISCOUNT) SPECIAL_DISCOUNT, SUM(YEARLY_DISCOUNT) YEARLY_DISCOUNT, SUM(BILL_DISCOUNT) BILL_DISCOUNT, SUM(LUX_TAX) LUX_TAX, (SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) - (SUM(DISCOUNT) + SUM(CASH_DISCOUNT) + SUM(SPECIAL_DISCOUNT) + SUM(YEARLY_DISCOUNT) + SUM(BILL_DISCOUNT)) + SUM(LUX_TAX )) TAXABLE_TOTAL_PRICE  
                                            , SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE ,(SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) + SUM(LUX_TAX) - SUM(DISCOUNT) - SUM(CASH_DISCOUNT) + SUM(VAT_TOTAL_PRICE)) INVOICE_TOTAL_PRICE , VEHICLE_NO, DESTINATION  
                                            ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS ,FISCAL, FORM_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, RETURN_TYPE FROM (  
                                            SELECT A.FORM_CODE, A.MANUAL_NO, A.RETURN_DATE, BS_DATE(A.RETURN_DATE) MITI , FN_GET_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.RETURN_NO, 1) ORDER_NO, A.RETURN_NO ,  
                                            F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO,  
                                            a.QUANTITY , a.TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, ROLL_QTY, SUPPLIER_INV_NO, SUPPLIER_INV_DATE  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) EXCISE_DUTY  
                                            ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) EXCISE_DUTYII  
                                            , ROUND((NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2)  DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) CASH_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) BILL_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) LUX_TAX  
                                            , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2}))  AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                             NVL(VAT_AMOUNT,0)  ELSE  
                                            ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE  
                                            ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION  
                                            ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO  
                                            , FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS  
                                            ,(SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL , A.RETURN_TYPE  
                                            FROM IP_PURCHASE_RETURN A, SHIPPING_TRANSACTION D, IP_SUPPLIER_SETUP F    
                                            WHERE A.SUPPLIER_CODE = F.SUPPLIER_CODE  
                                            AND A.COMPANY_CODE = F.COMPANY_CODE  
                                            AND A.RETURN_NO = D.VOUCHER_NO (+)  
                                            AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                            AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})   
                                            AND A.DELETED_FLAG = 'N'  
                                            ORDER BY RETURN_NO, A.SERIAL_NO  
                                            )  
                                            GROUP BY RETURN_DATE, MITI, ORDER_NO, RETURN_NO, MANUAL_NO, SUPPLIER_CODE, SUPPLIER_EDESC, ADDRESS, 
                                            TPIN_VAT_NO, VEHICLE_NO, DESTINATION, DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, 
                                            FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, TRANSPORTER_NAME, CN_NO,
                                            TRANSPORT_INVOICE_NO, SHIPPING_TERMS , FISCAL, FORM_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, RETURN_TYPE  ORDER BY RETURN_NO", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var billwiseReturn = _objectEntity.SqlQuery<BillwisePurchaseSummaryViewModel>(query).ToList();
            return billwiseReturn;

        }

        public List<ProductwisePurchaseSummaryViewModel> GetProductwisePurchaseSummary(ReportFiltersModel filters, User userinfo)
        {
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                           : $"'{userinfo.company_code}'";

            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";

            string query = string.Format(@"SELECT * FROM (SELECT ITEM_CODE, ITEM_EDESC, PRODUCT_CODE, UNIT   
                                                , SUM(QUANTITY) QUANTITY, SUM(TOTAL_PRICE) / SUM(QUANTITY) UNIT_PRICE, SUM(TOTAL_PRICE) TOTAL_PRICE, SUM(SECOND_QUANTITY) SECOND_QUANTITY, SUM(THIRD_QUANTITY) THIRD_QUANTITY, SUM(ROLL_QTY) ROLL_QTY , SUM(EXCISE_DUTY) EXCISE_DUTY, SUM(EXCISE_DUTYII) EXCISE_DUTYII, SUM(DISCOUNT) DISCOUNT, SUM(CASH_DISCOUNT) CASH_DISCOUNT, SUM(SPECIAL_DISCOUNT) SPECIAL_DISCOUNT  
                                                , SUM(YEARLY_DISCOUNT) YEARLY_DISCOUNT,SUM(BILL_DISCOUNT) BILL_DISCOUNT, SUM(LUX_TAX) LUX_TAX, SUM(ADD_CHARGE) ADD_CHARGE, SUM(TAXABLE_TOTAL_PRICE) TAXABLE_TOTAL_PRICE , SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE, SUM(INVOICE_TOTAL_PRICE) INVOICE_TOTAL_PRICE, SUM(LANDED_IN_NRS)LANDED_IN_NRS FROM (  
                                                SELECT ITEM_CODE, ITEM_EDESC, PRODUCT_CODE  
                                                , UNIT, QUANTITY, UNIT_PRICE, TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, ROLL_QTY, EXCISE_DUTY, EXCISE_DUTYII, DISCOUNT, CASH_DISCOUNT, SPECIAL_DISCOUNT, YEARLY_DISCOUNT, BILL_DISCOUNT, LUX_TAX, ADD_CHARGE ,  (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) +LUX_TAX + ADD_CHARGE  ) TAXABLE_TOTAL_PRICE  
                                                , CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + LUX_TAX + ADD_CHARGE ) * (NON_VAT * .13),2) ELSE 0 END VAT_TOTAL_PRICE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + ADD_CHARGE - DISCOUNT - CASH_DISCOUNT + CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + ADD_CHARGE - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) ) * (NON_VAT * .13),2) ELSE 0 END ) INVOICE_TOTAL_PRICE, LANDED_IN_NRS   
                                                FROM (  
                                                SELECT A.FORM_CODE, A.INVOICE_DATE, A.INVOICE_NO ,  
                                                H.SUPPLIER_EDESC LC_NAME ,F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO, C.ITEM_CODE, C.ITEM_EDESC, c.PRODUCT_CODE, A.MU_CODE UNIT,  
                                                a.QUANTITY , a.UNIT_PRICE, a.TOTAL_PRICE * A.EXCHANGE_RATE TOTAL_PRICE , A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.ROLL_QTY, A.EXCHANGE_RATE, A.CURRENCY_CODE, A.TOTAL_PRICE * A.EXCHANGE_RATE TOTAL_IN_NRS, A.CALC_TOTAL_PRICE * A.EXCHANGE_RATE LANDED_IN_NRS  
                                                , CASE WHEN EXCISE_AMOUNT IS NOT NULL AND EXCISE_AMOUNT > 0 THEN  
                                                 NVL(EXCISE_AMOUNT,0)   ELSE  
                                                 ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END * EXCHANGE_RATE EXCISE_DUTY  
                                                , CASE WHEN EXCISE_AMOUNTII IS NOT NULL AND EXCISE_AMOUNTII > 0 THEN  
                                                 NVL(EXCISE_AMOUNTII,0)  ELSE  
                                                 ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNTII,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END * EXCHANGE_RATE EXCISE_DUTYII  
                                                , CASE WHEN DISCOUNT_AMOUNT IS NOT NULL AND DISCOUNT_AMOUNT > 0 THEN  
                                                 NVL(DISCOUNT_AMOUNT,0)  ELSE  
                                                ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(DISCOUNT_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END * EXCHANGE_RATE DISCOUNT  
                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE CASH_DISCOUNT  
                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE SPECIAL_DISCOUNT  
                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE YEARLY_DISCOUNT  
                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE BILL_DISCOUNT  
                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE LUX_TAX  
                                                ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE  FROM CHARGE_TRANSACTION X  
                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'A' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE ADD_CHARGE  
                                                , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2})) AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                                 NVL(VAT_AMOUNT,0)  ELSE  
                                                ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                                WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = X.COMPANY_CODE) AND APPLY_ON = 'D'  
                                                AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE, DECODE(C.NON_VAT_FLAG,'N',1,0) * EXCHANGE_RATE NON_VAT  
                                                FROM IP_PURCHASE_INVOICE A, IP_ITEM_MASTER_SETUP C, SHIPPING_TRANSACTION D, IP_SUPPLIER_SETUP F, IP_ITEM_SPEC_SETUP G, IP_SUPPLIER_SETUP H   
                                                WHERE A.ITEM_CODE = C.ITEM_CODE  
                                                AND A.COMPANY_CODE = C.COMPANY_CODE  
                                                AND A.SUPPLIER_CODE = F.SUPPLIER_CODE  
                                                AND A.COMPANY_CODE = F.COMPANY_CODE  
                                                AND A.INVOICE_NO = D.VOUCHER_NO (+)  
                                                AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                                AND C.ITEM_CODE = G.ITEM_CODE (+)  
                                                AND C.COMPANY_CODE = G.COMPANY_CODE (+)  
                                                AND A.LC_NO = H.SUPPLIER_CODE (+)  
                                                AND A.COMPANY_CODE = H.COMPANY_CODE (+)  
                                                AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                                AND A.COMPANY_CODE IN ({2})
                                                AND A.BRANCH_CODE IN ({3})    
                                                AND A.DELETED_FLAG = 'N'  
                                                ORDER BY INVOICE_NO, A.SERIAL_NO  
                                                )  
                                                )  
                                                GROUP BY ITEM_CODE, ITEM_EDESC, PRODUCT_CODE, UNIT  
                                                ) ORDER BY ITEM_EDESC  ", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var prductwise = _objectEntity.SqlQuery<ProductwisePurchaseSummaryViewModel>(query).ToList();
            return prductwise;
        }

        public List<BillwisePurchaseSummaryViewModel> GetDatewisePurchaseDetails(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();

            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";

            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";

            string query = string.Format(@"SELECT INVOICE_DATE, MITI, ORDER_NO, INVOICE_NO, LC_NAME, SUPPLIER_CODE, SUPPLIER_EDESC, ADDRESS, TPIN_VAT_NO, ITEM_CODE, ITEM_EDESC, PRODUCT_CODE  
                                            , UNIT, QUANTITY, UNIT_PRICE, TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, ROLL_QTY , EXCISE_DUTY, EXCISE_DUTYII, DISCOUNT, CASH_DISCOUNT, SPECIAL_DISCOUNT, YEARLY_DISCOUNT,BILL_DISCOUNT, LUX_TAX, ADD_CHARGE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) +LUX_TAX + ADD_CHARGE ) TAXABLE_TOTAL_PRICE  
                                            , CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + LUX_TAX + ADD_CHARGE )  * (NON_VAT * .13),2) ELSE 0 END VAT_TOTAL_PRICE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + ADD_CHARGE - DISCOUNT - CASH_DISCOUNT + CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + ADD_CHARGE - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) ) * (NON_VAT * .13),2) ELSE 0 END ) INVOICE_TOTAL_PRICE, VEHICLE_NO, DESTINATION  
                                            ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , UPC, CATEGORY, SHELF_LIFE,  WEIGHT, UOM, STATUS,  FISCAL, FORM_CODE, BRAND_NAME, NON_VAT , EXCHANGE_RATE, CURRENCY_CODE, TOTAL_IN_NRS , LANDED_IN_NRS , SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, PP_NO, HS_CODE, HSN  FROM (  
                                            SELECT A.FORM_CODE, A.INVOICE_DATE, BS_DATE(A.INVOICE_DATE) MITI , FN_GET_SECOND_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.INVOICE_NO, A.SERIAL_NO) ORDER_NO, A.INVOICE_NO ,  
                                            H.SUPPLIER_EDESC LC_NAME ,F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO, C.ITEM_CODE, C.ITEM_EDESC, c.PRODUCT_CODE, A.MU_CODE UNIT,  
                                            a.QUANTITY , a.UNIT_PRICE, a.TOTAL_PRICE, A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.ROLL_QTY, A.EXCHANGE_RATE, A.CURRENCY_CODE, (A.TOTAL_PRICE * A.EXCHANGE_RATE)  TOTAL_IN_NRS, (A.CALC_TOTAL_PRICE * A.EXCHANGE_RATE) LANDED_IN_NRS, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, (SELECT PP_NO FROM FA_PP_DETAIL_TRANSACTION WHERE REFERENCE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE AND FORM_CODE = A.FORM_CODE AND ROWNUM = 1) PP_NO  
                                            , CASE WHEN EXCISE_AMOUNT IS NOT NULL AND EXCISE_AMOUNT > 0 THEN  
                                             NVL(EXCISE_AMOUNT,0)  ELSE  
                                             ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTY  
                                            , CASE WHEN EXCISE_AMOUNTII IS NOT NULL AND EXCISE_AMOUNTII > 0 THEN  
                                             NVL(EXCISE_AMOUNTII,0)  ELSE  
                                             ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNTII,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTYII  
                                            , CASE WHEN DISCOUNT_AMOUNT IS NOT NULL AND DISCOUNT_AMOUNT > 0 THEN  
                                             NVL(DISCOUNT_AMOUNT,0)  ELSE  
                                            ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) *  (SELECT CASE WHEN SUM(NVL(DISCOUNT_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) CASH_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) BILL_DISCOUNT  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) LUX_TAX  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE  FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'A' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) ADD_CHARGE  
                                            , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2})) AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                             NVL(VAT_AMOUNT,0)  ELSE  
                                            ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = X.COMPANY_CODE) AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE  
                                            ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION  
                                            ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO  
                                            , FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS  
                                            ,G.ITEM_SIZE UPC, G.TYPE CATEGORY, G.GRADE SHELF_LIFE,  G.SIZE_LENGHT WEIGHT, G.THICKNESS UOM, G.REMARKS STATUS, (SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL, g.BRAND_NAME, DECODE(C.NON_VAT_FLAG,'N',1,0) NON_VAT, SERIAL_NO, C.HS_CODE, A.HSN   
                                            FROM IP_PURCHASE_INVOICE A, IP_ITEM_MASTER_SETUP C, SHIPPING_TRANSACTION D, IP_SUPPLIER_SETUP F, IP_ITEM_SPEC_SETUP G, IP_SUPPLIER_SETUP H   
                                            WHERE A.ITEM_CODE = C.ITEM_CODE  
                                            AND A.COMPANY_CODE = C.COMPANY_CODE  
                                            AND A.SUPPLIER_CODE = F.SUPPLIER_CODE  
                                            AND A.COMPANY_CODE = F.COMPANY_CODE  
                                            AND A.INVOICE_NO = D.VOUCHER_NO (+)  
                                            AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                            AND C.ITEM_CODE = G.ITEM_CODE (+)  
                                            AND C.COMPANY_CODE = G.COMPANY_CODE (+)  
                                            AND A.LC_NO = H.SUPPLIER_CODE (+)  
                                            AND A.COMPANY_CODE = H.COMPANY_CODE (+)  
                                            AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})   
                                            AND A.DELETED_FLAG = 'N'  
                                            ORDER BY INVOICE_NO, A.SERIAL_NO  
                                            )  
                                            ORDER BY INVOICE_DATE, INVOICE_NO, SERIAL_NO", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var datewise = _objectEntity.SqlQuery<BillwisePurchaseSummaryViewModel>(query).ToList();
            return datewise;
        }

        public List<BillwisePurchaseSummaryViewModel> GetPurchaseLandedCost(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();

            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";

            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";

            string query = string.Format(@"SELECT a.*,  D1  +  D2  +  D3  +  D4  +  D5  +  D6  +  D7  +  D8 AS TotalD FROM (SELECT A.FORM_CODE, A.INVOICE_DATE, BS_DATE(A.INVOICE_DATE) MITI , FN_GET_SECOND_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.INVOICE_NO, A.SERIAL_NO) ORDER_NO, A.INVOICE_NO ,  
                                             G.SUPPLIER_EDESC LC_NAME ,F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO, C.ITEM_CODE, C.ITEM_EDESC, c.PRODUCT_CODE, A.MU_CODE UNIT ,  
                                            a.QUANTITY , a.UNIT_PRICE * EXCHANGE_RATE UNIT_PRICE , a.TOTAL_PRICE * EXCHANGE_RATE TOTAL_PRICE, A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.ROLL_QTY, A.EXCHANGE_RATE, A.CURRENCY_CODE, A.TOTAL_PRICE * A.EXCHANGE_RATE TOTAL_IN_NRS, A.CALC_TOTAL_PRICE * A.EXCHANGE_RATE LANDED_IN_NRS, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, PP_NO  
                                            ,ROUND(((SELECT SUM(DR_AMOUNT * EXCHANGE_RATE) DR_AMOUNT FROM V$VIRTUAL_GENERAL_LEDGER1 X, FINANCIAL_REFERENCE_DETAIL Y  
                                            WHERE X.ACC_CODE IN (  
                                            SELECT DISTINCT ACC_CODE FROM CHARGE_SETUP  
                                            WHERE CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE  
                                            WHERE SPECIFIC_CHARGE_FLAG IN ('E')))  
                                            AND X.VOUCHER_NO = Y.VOUCHER_NO  
                                            AND X.FORM_CODE = Y.FORM_CODE  
                                            AND X.COMPANY_CODE = Y.COMPANY_CODE  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE  
                                            AND Y.REFERENCE_NO = A.INVOICE_NO 
                                            AND X.TRANSACTION_TYPE = 'DR' AND X.FORM_CODE IN (SELECT FORM_CODE FROM FORM_SETUP WHERE PURCHASE_EXPENSES_FLAG = 'Y')) / NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2)  EXCISE_AMT_E  
                                            ,ROUND(((SELECT SUM(DR_AMOUNT * EXCHANGE_RATE) DR_AMOUNT FROM V$VIRTUAL_GENERAL_LEDGER1 X, FINANCIAL_REFERENCE_DETAIL Y  
                                            WHERE X.ACC_CODE IN (  
                                            SELECT DISTINCT ACC_CODE FROM CHARGE_SETUP  
                                            WHERE CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE  
                                            WHERE SPECIFIC_CHARGE_FLAG IN ('F')))  
                                            AND X.VOUCHER_NO = Y.VOUCHER_NO  
                                            AND X.FORM_CODE = Y.FORM_CODE  
                                            AND X.COMPANY_CODE = Y.COMPANY_CODE  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE  
                                            AND Y.REFERENCE_NO = A.INVOICE_NO 
                                            AND X.TRANSACTION_TYPE = 'DR' AND X.FORM_CODE IN (SELECT FORM_CODE FROM FORM_SETUP WHERE PURCHASE_EXPENSES_FLAG = 'Y')) / NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2)  EXCISE_AMT_F  
                                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'A' AND COMPANY_CODE = X.COMPANY_CODE) AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2)  EXCISE_AMT_II  
                                            , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2})) AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                             NVL(VAT_AMOUNT,0)  ELSE  
                                            ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = X.COMPANY_CODE) AND APPLY_ON = 'D'  
                                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE AND ITEM_CODE IN (SELECT ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE NON_VAT_FLAG = 'N' AND COMPANY_CODE IN ({2}))  ),1)) * (TOTAL_PRICE),2) * DECODE(C.NON_VAT_FLAG,'N',1,0)  END VAT_TOTAL_PRICE  
                                            ,ROUND((NVL(D1_CHARGE_AMOUNT,0)),2) D1  
                                            ,ROUND((NVL(D2_CHARGE_AMOUNT,0)),2) D2  
                                            ,ROUND((NVL(D3_CHARGE_AMOUNT,0)),2) D3  
                                            ,ROUND((NVL(D4_CHARGE_AMOUNT,0)),2) D4  
                                            ,ROUND((NVL(D5_CHARGE_AMOUNT,0)),2) D5  
                                            ,ROUND((NVL(D6_CHARGE_AMOUNT,0)),2) D6  
                                            ,ROUND((NVL(D7_CHARGE_AMOUNT,0)),2) D7  
                                            ,ROUND((NVL(D8_CHARGE_AMOUNT,0)),2) D8  
                                             FROM IP_PURCHASE_INVOICE A,   
                                            (SELECT * FROM (  
                                            SELECT REFERENCE_NO VOUCHER_NO, SERIAL_NO, FORM_CODE, ITEM_CODE, COMPANY_CODE, CHARGE_CODE, CASE WHEN CHARGE_TYPE_FLAG = 'A' THEN 1 ELSE -1 END * CHARGE_AMOUNT * EXCHANGE_RATE CHARGE_AMOUNT  FROM CHARGE_TRANSACTION A  
                                            WHERE APPLY_ON IN ('I','D')  
                                            AND TABLE_NAME = 'IP_PURCHASE_INVOICE'  
                                            )  
                                            Pivot  
                                            (SUM(CHARGE_AMOUNT)CHARGE_AMOUNT  
                                            FOR CHARGE_CODE IN ('ID' D1, 'BC' D2, 'CL' D3, 'CD' D4, 'FI' D5, 'FL' D6, 'IS' D7, 'OE' D8)  
                                            )) B, IP_ITEM_MASTER_SETUP C, IP_SUPPLIER_SETUP F, IP_SUPPLIER_SETUP G   
                                            WHERE A.ITEM_CODE = C.ITEM_CODE  
                                            AND A.COMPANY_CODE = C.COMPANY_CODE  
                                            AND A.SUPPLIER_CODE = F.SUPPLIER_CODE  
                                            AND A.COMPANY_CODE = F.COMPANY_CODE  
                                            AND a.INVOICE_NO = b.VOUCHER_NO (+)  
                                            AND A.FORM_CODE = B.FORM_CODE (+)  
                                            AND A.COMPANY_CODE = B.COMPANY_CODE (+)  
                                            AND A.ITEM_CODE = B.ITEM_CODE (+)  
                                            AND A.SERIAL_NO = B.SERIAL_NO (+)  
                                            AND A.LC_NO = G.SUPPLIER_CODE (+)  
                                            AND A.COMPANY_CODE = G.COMPANY_CODE (+)  
                                            AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})
                                            AND A.DELETED_FLAG = 'N'  
                                            ORDER BY INVOICE_DATE, INVOICE_NO, A.SERIAL_NO  
                                            ) a  
                                            ORDER BY INVOICE_DATE, INVOICE_NO", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var landedCost = _objectEntity.SqlQuery<BillwisePurchaseSummaryViewModel>(query).ToList();
            return landedCost;
        }

        public dynamic GetCustomerwiseOrderTracking(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT DISTINCT A.CUSTOMER_CODE, C.CUSTOMER_EDESC, A.ORDER_DATE , BS_DATE(A.ORDER_DATE) MITI , A.ORDER_NO,A.EMPLOYEE_CODE, E.EMPLOYEE_EDESC, A.ITEM_CODE,  
                                                D.ITEM_EDESC, A.MU_CODE, A.QUANTITY, A.SECOND_QUANTITY, A.UNIT_PRICE, A.TOTAL_PRICE,  
                                                X.CHALAN_DATE, X.CHALAN_NO, X.CHALAN_QTY, X.CHALAN_ALT_QTY, X.SALES_DATE,  
                                                X.SALES_NO, X.SALES_QTY, X.SALES_ALT_QTY, X.SALES_VALUE, X.COMPANY_CODE  FROM  
                                                (SELECT S.CUSTOMER_CODE, R.COMPANY_CODE, R.REFERENCE_NO, R.REFERENCE_SERIAL_NO,  
                                                S.CHALAN_DATE CHALAN_DATE, S.CHALAN_NO CHALAN_NO,  
                                                S.ITEM_CODE, S.QUANTITY CHALAN_QTY, S.SECOND_QUANTITY  CHALAN_ALT_QTY , T.SALES_DATE, 
                                                T.SALES_NO, T.SALES_QTY, T.SALES_ALT_QTY, T.SALES_VALUE FROM  
                                                (SELECT T.CUSTOMER_CODE, T.COMPANY_CODE, T.SALES_DATE SALES_DATE, T.SALES_NO SALES_NO,  
                                                R.REFERENCE_SERIAL_NO, T.ITEM_CODE, T.QUANTITY SALES_QTY, T.SECOND_QUANTITY  SALES_ALT_QTY, T.TOTAL_PRICE SALES_VALUE,  
                                                R.REFERENCE_NO, R.REFERENCE_FORM_CODE FROM  
                                                SA_SALES_INVOICE T, REFERENCE_DETAIL R WHERE T.COMPANY_CODE = R.COMPANY_CODE  
                                                AND T.ITEM_CODE = R.REFERENCE_ITEM_CODE AND T.SALES_NO = R.VOUCHER_NO  
                                                AND T.SERIAL_NO = R.SERIAL_NO AND T.DELETED_FLAG = 'N' ) T,  
                                                SA_SALES_CHALAN S, REFERENCE_DETAIL R WHERE S.COMPANY_CODE = R.COMPANY_CODE  
                                                AND S.ITEM_CODE = R.REFERENCE_ITEM_CODE AND S.CHALAN_NO = R.VOUCHER_NO  
                                                AND S.SERIAL_NO = R.SERIAL_NO AND S.DELETED_FLAG = 'N'  
                                                AND S.FORM_CODE = T.REFERENCE_FORM_CODE(+) AND S.SERIAL_NO = T.REFERENCE_SERIAL_NO(+)  
                                                AND S.CUSTOMER_CODE = T.CUSTOMER_CODE(+) AND S.ITEM_CODE = T.ITEM_CODE(+)  
                                                AND S.CHALAN_NO = T.REFERENCE_NO(+)  
                                                AND S.COMPANY_CODE = T.COMPANY_CODE(+)) X, V$SALES_ORDER_ANALYSIS A, SA_CUSTOMER_SETUP C, IP_ITEM_MASTER_SETUP D, HR_EMPLOYEE_SETUP E  
                                                WHERE A.CUSTOMER_CODE = C.CUSTOMER_CODE(+)  
                                                AND A.ITEM_CODE = D.ITEM_CODE(+) AND A.DELETED_FLAG = 'N'  
                                                AND A.COMPANY_CODE = C.COMPANY_CODE(+) AND A.COMPANY_CODE = D.COMPANY_CODE(+)  
                                                AND A.ORDER_NO = X.REFERENCE_NO(+) AND A.ITEM_CODE = X.ITEM_CODE(+)  
                                                AND A.COMPANY_CODE = X.COMPANY_CODE(+)  
                                                AND A.SERIAL_NO =  X.REFERENCE_SERIAL_NO (+)  
                                                AND A.EMPLOYEE_CODE = E.EMPLOYEE_CODE (+) AND A.COMPANY_CODE = E.COMPANY_CODE(+)  
                                                AND A.ORDER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                AND A.ORDER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                AND A.COMPANY_CODE IN ({2})
                                                AND A.BRANCH_CODE IN ({3})  
                                                ORDER BY C.CUSTOMER_EDESC, A.ORDER_DATE, A.ORDER_NO, D.ITEM_EDESC,  X.CHALAN_NO, X.SALES_NO", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<CustomerwiseOrderTrackingModel>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
        }

        public dynamic GetGroupProductwiseOrderPending(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT A.ITEM_CODE, A.ITEM_EDESC, A.INDEX_MU_CODE UNIT, A.MASTER_ITEM_CODE, A.PRE_ITEM_CODE, (LENGTH(A.MASTER_ITEM_CODE) - LENGTH(REPLACE(A.MASTER_ITEM_CODE,'.',''))) ROWLEV , A.GROUP_SKU_FLAG, DATA.* FROM (  
                                            SELECT A.ITEM_CODE, A.MU_CODE, A.COMPANY_CODE, SUM(ORDER_QTY) ORDER_QTY, SUM(DUE_QTY) DUE_QTY FROM V$SALES_ORDER_ANALYSIS A, IP_ITEM_MASTER_SETUP B, SA_CUSTOMER_SETUP C  
                                            WHERE A.CUSTOMER_CODE = C.CUSTOMER_CODE  
                                            AND A.COMPANY_CODE = C.COMPANY_CODE  
                                            AND A.ITEM_CODE = B.ITEM_CODE  
                                            AND A.COMPANY_CODE = B.COMPANY_CODE  
                                            AND A.DELETED_FLAG = 'N'  
                                            AND A.ORDER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.ORDER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})   
                                            GROUP BY A.ITEM_CODE, A.MU_CODE, A.COMPANY_CODE ) DATA, IP_ITEM_MASTER_SETUP A  
                                            WHERE A.ITEM_CODE = DATA.ITEM_CODE (+)  
                                            AND A.COMPANY_CODE = DATA.COMPANY_CODE (+)  
                                            AND A.COMPANY_CODE IN ({2})  
                                            ORDER BY A.MASTER_ITEM_CODE, A.ITEM_EDESC  ", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<CustomerwiseOrderTrackingModel>(query).ToList();
            var groupedData = monthwise;

            foreach (var item in groupedData)
            {
                if (item.MASTER_ITEM_CODE == "001" || item.MASTER_ITEM_CODE == "002" || item.MASTER_ITEM_CODE == "003" || item.MASTER_ITEM_CODE == "004" || item.MASTER_ITEM_CODE == "005"
                   || item.MASTER_ITEM_CODE == "006" || item.MASTER_ITEM_CODE == "007" || item.MASTER_ITEM_CODE == "008" || item.MASTER_ITEM_CODE == "009")
                {
                    item.PRE_ITEM_CODE = "";
                }

            }
            //return groupedData;
            List<dynamic> stockData = new List<dynamic>();
            var itemRows = groupedData.Where(r => r.GROUP_SKU_FLAG == "I").ToList();

            foreach (var row in groupedData)
            {
                double total1 = 0;  // S_QTY
                double total2 = 0;  // S_VALUE
                if (row.GROUP_SKU_FLAG == "G")
                {
                    string masterCode = row.MASTER_ITEM_CODE;
                    foreach (var z in itemRows)
                    {
                        if (!string.IsNullOrEmpty(z.MASTER_ITEM_CODE) &&
                            z.MASTER_ITEM_CODE.StartsWith(masterCode))
                        {
                            total1 += z.ORDER_QTY ?? 0;
                            total2 += z.DUE_QTY ?? 0;
                        }
                    }
                }
                else
                {
                    total1 = row.ORDER_QTY ?? 0;
                    total2 = row.DUE_QTY ?? 0;
                }
                // Add only if any total is non-zero
                if (total1 != 0 || total2 != 0)
                {
                    stockData.Add(new
                    {
                        ITEM_CODE = row.ITEM_CODE,
                        ITEM_EDESC = row.ITEM_EDESC,
                        MASTER_ITEM_CODE = row.MASTER_ITEM_CODE,
                        PRE_ITEM_CODE = row.PRE_ITEM_CODE,
                        GROUP_SKU_FLAG = row.GROUP_SKU_FLAG,
                        ORDER_QTY = total1,
                        DUE_QTY = total2,
                        UNIT = row.UNIT,
                    });
                }
            }
            return stockData;
        }

        public List<SalesAboveOneLakhModel> GetSalesAboveOneLakh(ReportFiltersModel filters, User userinfo)
        {
            var figureFilter = ReportFilterHelper.FigureFilterValue(filters.AmountFigureFilter).ToString();
            var roundUpFilter = ReportFilterHelper.RoundUpFilterValue(filters.AmountRoundUpFilter).ToString();

            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                               : $"'{userinfo.company_code}'";

            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";

            string query = string.Format(@"SELECT  PAN,  (SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE TPIN_VAT_NO = PAN AND COMPANY_CODE = '01' AND DELETED_FLAG='N' AND ROWNUM =1)NAME_OF_TAXPAYER , TRADE_NAME_TYPE, PURCHASE_SALES, SUM(EXEMPTED_AMOUNT) EXEMPTED_AMOUNT , SUM(TAXABLE_AMOUNT) TAXABLE_AMOUNT,'' REMARKS  FROM (  
                                            SELECT B.CUSTOMER_CODE,  B.TPIN_VAT_NO PAN, B.CUSTOMER_EDESC NAME_OF_TAXPAYER , 'E' TRADE_NAME_TYPE,'S' PURCHASE_SALES, EXEMPT_AMOUNT EXEMPTED_AMOUNT,  TAXABLE_AMOUNT   TAXABLE_AMOUNT FROM (  
                                            SELECT VOUCHER_NO, VOUCHER_DATE, COMPANY_CODE, PARTY_CODE, (EXEMPT_AMOUNT*EXCHANGE_RATE) EXEMPT_AMOUNT, (TAXABLE_AMOUNT*EXCHANGE_RATE) TAXABLE_AMOUNT  
                                            FROM V$SALES_TAX_REPORT A  
                                            Where 1 = 1  
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})    
                                            ) A, SA_CUSTOMER_SETUP B  
                                            Where a.PARTY_CODE = b.CUSTOMER_CODE  
                                            AND A.COMPANY_CODE = B.COMPANY_CODE  
                                            AND B.TPIN_VAT_NO IS NOT NULL  
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')  
                                            ) HAVING SUM(TAXABLE_AMOUNT+EXEMPTED_AMOUNT) >= 100000  
                                            GROUP BY  PAN, TRADE_NAME_TYPE, PURCHASE_SALES   
                                            ORDER BY 2", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var datewise = _objectEntity.SqlQuery<SalesAboveOneLakhModel>(query).ToList();
            return datewise;
        }

        public dynamic GetProductGroupwiseNetSales(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT ITEM_EDESC, PRODUCT_CODE, INDEX_MU_CODE, ITEM_GROUP_EDESC , SALES_QTY, SALES_VALUE, SALES_RET_QTY, SALES_RET_VALUE, DEBIT_VALUE, CREDIT_VALUE, FREE_QTY, SALES_QTY - SALES_RET_QTY + FREE_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE  NET_SALES_VALUE FROM (  
                                            SELECT D.ITEM_EDESC,D.PRODUCT_CODE, E.CUSTOMER_EDESC ,INDEX_MU_CODE INDEX_MU_CODE, SUBSTR(FN_FETCH_PRE_DESC(D.COMPANY_CODE,'IP_ITEM_MASTER_SETUP', D.PRE_ITEM_CODE),1,100) ITEM_GROUP_EDESC,  SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE, SUM(FREE_QTY) FREE_QTY  FROM (  
                                            SELECT A.ITEM_CODE, A.CUSTOMER_CODE, SUM(NVL(QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, NVL(SUM(NVL(A.FREE_QTY,0)),0) FREE_QTY    FROM SA_SALES_INVOICE A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND TRUNC(A.SALES_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND TRUNC(A.SALES_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})   
                                            GROUP BY  A.CUSTOMER_CODE,  A.ITEM_CODE  
                                            Union All  
                                            SELECT A.ITEM_CODE, A.CUSTOMER_CODE,  0 SALES_QTY, 0 SALES_VALUE, SUM(QUANTITY)  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, NVL(SUM(NVL(A.FREE_QTY -1,0)),0) FREE_QTY    FROM SA_SALES_RETURN A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND TRUNC(A.RETURN_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND TRUNC(A.RETURN_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})   
                                            GROUP BY  A.CUSTOMER_CODE,   A.ITEM_CODE  
                                            Union All  
                                            SELECT A.ITEM_CODE, A.CUSTOMER_CODE,  0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE, 0 FREE_QTY     FROM SA_DEBIT_NOTE A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND TRUNC(A.VOUCHER_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND TRUNC(A.VOUCHER_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})
                                            GROUP BY  A.CUSTOMER_CODE,   A.ITEM_CODE  
                                            Union All  
                                            SELECT A.ITEM_CODE, A.CUSTOMER_CODE,  0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE, 0 FREE_QTY   FROM SA_CREDIT_NOTE A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND TRUNC(A.VOUCHER_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND TRUNC(A.VOUCHER_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3}) 
                                            GROUP BY  A.CUSTOMER_CODE,   A.ITEM_CODE  
                                            ORDER BY 1 ) A,  IP_ITEM_MASTER_SETUP D , SA_CUSTOMER_SETUP E  
                                            Where  A.ITEM_CODE = D.ITEM_CODE  
                                            AND a.CUSTOMER_CODE = E.CUSTOMER_CODE  
                                            AND D.COMPANY_CODE = E.COMPANY_CODE  
                                            AND D.COMPANY_CODE IN ({2})   
                                            GROUP BY  A.ITEM_CODE,  D.ITEM_EDESC, D.INDEX_MU_CODE, D.PRODUCT_CODE , D.COMPANY_CODE, D.PRE_ITEM_CODE   
                                            ) ORDER BY ITEM_GROUP_EDESC,  ITEM_EDESC", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<ProductGroupwiseNetSales>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
        }

        public dynamic GetCustomerwiseNetSalesRep(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");

            string query = string.Format(@"SELECT
                                                                T.CUSTOMER_EDESC,
                                                                T.TPIN_VAT_NO,
                                                                T.ITEM_EDESC,
                                                                T.PRODUCT_CODE,
                                                                T.INDEX_MU_CODE,
                                                                NVL(B.OPENING, 0) AS OPENING,
                                                                NVL(B.BALA, 0) AS BALA,
                                                                T.SALES_QTY,
                                                                T.SALES_VALUE,
                                                                T.SALES_RET_QTY,
                                                                T.SALES_RET_VALUE,
                                                                T.DEBIT_VALUE,
                                                                T.CREDIT_VALUE,
                                                                T.FREE_QTY,
                                                                (T.SALES_QTY - T.SALES_RET_QTY + T.FREE_QTY) AS NET_SALES_QTY,
                                                                (T.SALES_VALUE - T.SALES_RET_VALUE + T.DEBIT_VALUE - T.CREDIT_VALUE) AS NET_SALES_VALUE
                                                            FROM
                                                                ( -- T: Main Sales/Returns/Notes Aggregation (Date format corrected for best practice)
                                                                    SELECT
                                                                        B.CUSTOMER_CODE,
                                                                        B.ACC_CODE,
                                                                        B.CUSTOMER_EDESC,
                                                                        B.TPIN_VAT_NO,
                                                                        D.ITEM_EDESC,
                                                                        D.PRODUCT_CODE,
                                                                        D.INDEX_MU_CODE,
                                                                        SUM(A.SALES_QTY) AS SALES_QTY,
                                                                        SUM(A.SALES_VALUE) AS SALES_VALUE,
                                                                        SUM(A.SALES_RET_QTY) AS SALES_RET_QTY,
                                                                        SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE,
                                                                        SUM(A.DEBIT_VALUE) AS DEBIT_VALUE,
                                                                        SUM(A.CREDIT_VALUE) AS CREDIT_VALUE,
                                                                        SUM(A.FREE_QTY) AS FREE_QTY
                                                                    FROM
                                                                        ( -- A: Combined Transaction Data using UNION ALL
                                                                            SELECT
                                                                                CUSTOMER_CODE, ITEM_CODE, QUANTITY AS SALES_QTY, NVL(QUANTITY * NET_GROSS_RATE, 0) AS SALES_VALUE,
                                                                                0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE, 0 AS DEBIT_VALUE, 0 AS CREDIT_VALUE, NVL(FREE_QTY, 0) AS FREE_QTY, SALES_DATE AS TRANSACTION_DATE
                                                                            FROM
                                                                                SA_SALES_INVOICE
                                                                            WHERE
                                                                                DELETED_FLAG = 'N'
                                                                               AND SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                               AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                                               AND COMPANY_CODE IN ({2})
                                                                               AND BRANCH_CODE IN ({3}) -- Removed TRUNC() and used exclusive end date

                                                                            UNION ALL

                                                                            SELECT
                                                                                CUSTOMER_CODE, ITEM_CODE, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                                                                                QUANTITY AS SALES_RET_QTY, NVL(QUANTITY * NET_GROSS_RATE, 0) AS SALES_RET_VALUE,
                                                                                0 AS DEBIT_VALUE, 0 AS CREDIT_VALUE, NVL(FREE_QTY * -1, 0) AS FREE_QTY, RETURN_DATE AS TRANSACTION_DATE
                                                                            FROM
                                                                                SA_SALES_RETURN
                                                                            WHERE
                                                                                DELETED_FLAG = 'N'
                                                                               AND RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                               AND RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                                               AND COMPANY_CODE IN ({2})
                                                                               AND BRANCH_CODE IN ({3})
                                                                            UNION ALL

                                                                            SELECT
                                                                                CUSTOMER_CODE, ITEM_CODE, 0 AS SALES_QTY, 0 AS SALES_VALUE, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE,
                                                                                NVL(QUANTITY * NET_GROSS_RATE, 0) AS DEBIT_VALUE, 0 AS CREDIT_VALUE, 0 AS FREE_QTY, VOUCHER_DATE AS TRANSACTION_DATE
                                                                            FROM
                                                                                SA_DEBIT_NOTE
                                                                            WHERE
                                                                                DELETED_FLAG = 'N'
                                                                                AND VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                                AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                                                AND COMPANY_CODE IN ({2})
                                                                                AND BRANCH_CODE IN ({3})
                                                                            UNION ALL

                                                                            SELECT
                                                                                CUSTOMER_CODE, ITEM_CODE, 0 AS SALES_QTY, 0 AS SALES_VALUE, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE,
                                                                                0 AS DEBIT_VALUE, NVL(QUANTITY * NET_GROSS_RATE, 0) AS CREDIT_VALUE, 0 AS FREE_QTY, VOUCHER_DATE AS TRANSACTION_DATE
                                                                            FROM
                                                                                SA_CREDIT_NOTE
                                                                            WHERE
                                                                                DELETED_FLAG = 'N'
                                                                               AND VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                               AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                                               AND COMPANY_CODE IN ({2})
                                                                               AND BRANCH_CODE IN ({3})
                                                                        ) A
                                                                        JOIN SA_CUSTOMER_SETUP B ON A.CUSTOMER_CODE = B.CUSTOMER_CODE
                                                                        JOIN IP_ITEM_MASTER_SETUP D ON A.ITEM_CODE = D.ITEM_CODE AND B.COMPANY_CODE = D.COMPANY_CODE
                                                                    WHERE
                                                                        B.COMPANY_CODE IN ({2})
                                                                    GROUP BY
                                                                        B.CUSTOMER_CODE, B.ACC_CODE, B.CUSTOMER_EDESC, B.TPIN_VAT_NO, D.ITEM_EDESC, D.PRODUCT_CODE, D.INDEX_MU_CODE
                                                                ) T
                                                            LEFT JOIN
                                                                ( -- B: Non-Correlated Balance Calculation (Major Performance Gain)
                                                                    SELECT
                                                                        SUB_CODE,
                                                                        ACC_CODE,
                                                                        -- Opening Balance: Up to (but not including) 16-Jul-2024 OR special VOUCHER_NO = '0'
                                                                        SUM(CASE WHEN VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD') OR VOUCHER_NO = '0' THEN NVL(DR_AMOUNT, 0) - NVL(CR_AMOUNT, 0) ELSE 0 END) AS OPENING,
                                                                        -- Closing Balance: Up to and including 15-Jul-2025
                                                                        SUM(CASE WHEN  VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') THEN NVL(DR_AMOUNT, 0) - NVL(CR_AMOUNT, 0) ELSE 0 END) AS BALA
                                                                    FROM
                                                                        V$VIRTUAL_SUB_LEDGER
                                                                    WHERE
                                                                        DELETED_FLAG = 'N'
                                                                        AND COMPANY_CODE IN ({2})
                                                                        AND SUB_CODE LIKE 'C%' -- Optimistic filter based on 'C'||A.CUSTOMER_CODE
                                                                    GROUP BY
                                                                        SUB_CODE, ACC_CODE
                                                                ) B ON B.SUB_CODE = 'C' || T.CUSTOMER_CODE AND B.ACC_CODE = T.ACC_CODE
                                                            ORDER BY
                                                                T.CUSTOMER_EDESC,
                                                                T.ITEM_EDESC", FromDate, ToDate, companyCode, branchCode);
            var monthwise = _objectEntity.SqlQuery<CustomerwiseNetSalesModel>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
        }
        public dynamic GetProductCustomerwiseNetSalesReport(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT
                                                A.CUSTOMER_EDESC,
                                                A.TPIN_VAT_NO,
                                                A.ITEM_EDESC,
                                                A.PRODUCT_CODE,
                                                A.INDEX_MU_CODE,
                                                NVL(L.OPENING, 0) AS OPENING,
                                                NVL(L.BALA, 0) AS BALA,
                                                A.SALES_QTY,
                                                A.SALES_VALUE,
                                                A.SALES_RET_QTY,
                                                A.SALES_RET_VALUE,
                                                A.DEBIT_VALUE,
                                                A.CREDIT_VALUE,
                                                A.FREE_QTY,
                                                (A.SALES_QTY - A.SALES_RET_QTY + A.FREE_QTY) AS NET_SALES_QTY,
                                                (A.SALES_VALUE - A.SALES_RET_VALUE + A.DEBIT_VALUE - A.CREDIT_VALUE) AS NET_SALES_VALUE
                                            FROM
                                                (
                                                    -- Main block for sales/return/notes aggregation (A)
                                                    SELECT
                                                        B.CUSTOMER_CODE,
                                                        B.ACC_CODE,
                                                        B.CUSTOMER_EDESC,
                                                        B.TPIN_VAT_NO,
                                                        D.ITEM_EDESC,
                                                        D.PRODUCT_CODE,
                                                        D.INDEX_MU_CODE,
                                                        SUM(T.SALES_QTY) AS SALES_QTY,
                                                        SUM(T.SALES_VALUE) AS SALES_VALUE,
                                                        SUM(T.SALES_RET_QTY) AS SALES_RET_QTY,
                                                        SUM(T.SALES_RET_VALUE) AS SALES_RET_VALUE,
                                                        SUM(T.DEBIT_VALUE) AS DEBIT_VALUE,
                                                        SUM(T.CREDIT_VALUE) AS CREDIT_VALUE,
                                                        SUM(T.FREE_QTY) AS FREE_QTY
                                                    FROM
                                                        (
                                                            -- Consolidated data using UNION ALL
                                                            SELECT
                                                                CUSTOMER_CODE,
                                                                ITEM_CODE,
                                                                QUANTITY AS SALES_QTY,
                                                                NVL(QUANTITY * NET_GROSS_RATE, 0) AS SALES_VALUE,
                                                                0 AS SALES_RET_QTY,
                                                                0 AS SALES_RET_VALUE,
                                                                0 AS DEBIT_VALUE,
                                                                0 AS CREDIT_VALUE,
                                                                NVL(FREE_QTY, 0) AS FREE_QTY
                                                            FROM
                                                                SA_SALES_INVOICE
                                                            WHERE
                                                                DELETED_FLAG = 'N'
                                                                AND SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD')
                                                                AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})

                                                            UNION ALL

                                                            SELECT
                                                                CUSTOMER_CODE,
                                                                ITEM_CODE,
                                                                0 AS SALES_QTY,
                                                                0 AS SALES_VALUE,
                                                                QUANTITY AS SALES_RET_QTY,
                                                                NVL(QUANTITY * NET_GROSS_RATE, 0) AS SALES_RET_VALUE,
                                                                0 AS DEBIT_VALUE,
                                                                0 AS CREDIT_VALUE,
                                                                NVL(FREE_QTY * -1, 0) AS FREE_QTY
                                                            FROM
                                                                SA_SALES_RETURN
                                                            WHERE
                                                                DELETED_FLAG = 'N'
                                                                AND RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD')
                                                                AND RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})

                                                            UNION ALL

                                                            SELECT
                                                                CUSTOMER_CODE,
                                                                ITEM_CODE,
                                                                0 AS SALES_QTY,
                                                                0 AS SALES_VALUE,
                                                                0 AS SALES_RET_QTY,
                                                                0 AS SALES_RET_VALUE,
                                                                NVL(QUANTITY * NET_GROSS_RATE, 0) AS DEBIT_VALUE,
                                                                0 AS CREDIT_VALUE,
                                                                0 AS FREE_QTY
                                                            FROM
                                                                SA_DEBIT_NOTE
                                                            WHERE
                                                                DELETED_FLAG = 'N'
                                                                AND VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD')
                                                                AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})

                                                            UNION ALL

                                                            SELECT
                                                                CUSTOMER_CODE,
                                                                ITEM_CODE,
                                                                0 AS SALES_QTY,
                                                                0 AS SALES_VALUE,
                                                                0 AS SALES_RET_QTY,
                                                                0 AS SALES_RET_VALUE,
                                                                0 AS DEBIT_VALUE,
                                                                NVL(QUANTITY * NET_GROSS_RATE, 0) AS CREDIT_VALUE,
                                                                0 AS FREE_QTY
                                                            FROM
                                                                SA_CREDIT_NOTE
                                                            WHERE
                                                                DELETED_FLAG = 'N'
                                                                AND VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD')
                                                                AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                                                AND COMPANY_CODE IN ({2})
                                                                AND BRANCH_CODE IN ({3})
                                                        ) T
                                                        JOIN SA_CUSTOMER_SETUP B ON T.CUSTOMER_CODE = B.CUSTOMER_CODE
                                                        JOIN IP_ITEM_MASTER_SETUP D ON T.ITEM_CODE = D.ITEM_CODE
                                                        AND B.COMPANY_CODE = D.COMPANY_CODE
                                                    WHERE
                                                        B.COMPANY_CODE IN ({2})
                                                    GROUP BY
                                                        B.CUSTOMER_CODE,
                                                        B.ACC_CODE,
                                                        B.CUSTOMER_EDESC,
                                                        B.TPIN_VAT_NO,
                                                        D.ITEM_EDESC,
                                                        D.PRODUCT_CODE,
                                                        D.INDEX_MU_CODE
                                                ) A
                                                LEFT JOIN (
                                                    -- Calculate OPENING and BALA once per customer/account (L)
                                                    SELECT
                                                        SUBSTR(SUB_CODE, 2) AS CUSTOMER_CODE,
                                                        ACC_CODE,
                                                        SUM(
                                                            CASE
                                                                WHEN VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD') OR VOUCHER_NO = '0' THEN NVL(DR_AMOUNT, 0) - NVL(CR_AMOUNT, 0)
                                                                ELSE 0
                                                            END
                                                        ) AS OPENING,
                                                        SUM(
                                                            CASE
                                                                WHEN VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') THEN NVL(DR_AMOUNT, 0) - NVL(CR_AMOUNT, 0)
                                                                ELSE 0
                                                            END
                                                        ) AS BALA
                                                    FROM
                                                        V$VIRTUAL_SUB_LEDGER
                                                    WHERE
                                                        SUB_CODE LIKE 'C%'
                                                        AND DELETED_FLAG = 'N'
                                                        AND COMPANY_CODE IN ({2})
                                                        AND (
                                                            VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD') OR VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') OR VOUCHER_NO = '0'
                                                        )
                                                    GROUP BY
                                                        SUBSTR(SUB_CODE, 2),
                                                        ACC_CODE
                                                ) L ON 'C' || A.CUSTOMER_CODE = 'C' || L.CUSTOMER_CODE AND A.ACC_CODE = L.ACC_CODE
                                            ORDER BY
                                                ITEM_EDESC,
                                                CUSTOMER_EDESC", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<ProductGroupwiseNetSales>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
        }

        public dynamic GetCustomerBrandwiseNetSalesReport(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT
                                                    T.CUSTOMER_EDESC,
                                                    T.TPIN_VAT_NO,
                                                    T.ITEM_EDESC,
                                                    T.PRODUCT_CODE,
                                                    T.INDEX_MU_CODE,
                                                    NVL(B.OPENING, 0) AS OPENING,
                                                    NVL(B.BALA, 0) AS BALA,
                                                    T.SALES_QTY,
                                                    T.SALES_VALUE,
                                                    T.SALES_RET_QTY,
                                                    T.SALES_RET_VALUE,
                                                    T.DEBIT_VALUE,
                                                    T.CREDIT_VALUE,
                                                    T.FREE_QTY,
                                                    (T.SALES_QTY - T.SALES_RET_QTY + T.FREE_QTY) AS NET_SALES_QTY,
                                                    (T.SALES_VALUE - T.SALES_RET_VALUE + T.DEBIT_VALUE - T.CREDIT_VALUE) AS NET_SALES_VALUE
                                                FROM
                                                    ( -- T: Main Sales/Returns/Notes Aggregation and Joins
                                                        SELECT
                                                            B.CUSTOMER_CODE,
                                                            B.ACC_CODE,
                                                            B.CUSTOMER_EDESC,
                                                            B.TPIN_VAT_NO,
                                                            NVL(E.BRAND_NAME,'NA') AS ITEM_EDESC,
                                                            '' AS PRODUCT_CODE,
                                                            D.INDEX_MU_CODE,
                                                            SUM(A.SALES_QTY) AS SALES_QTY,
                                                            SUM(A.SALES_VALUE) AS SALES_VALUE,
                                                            SUM(A.SALES_RET_QTY) AS SALES_RET_QTY,
                                                            SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE,
                                                            SUM(A.DEBIT_VALUE) AS DEBIT_VALUE,
                                                            SUM(A.CREDIT_VALUE) AS CREDIT_VALUE,
                                                            SUM(A.FREE_QTY) AS FREE_QTY
                                                        FROM
                                                            ( -- A: Combined Transaction Data using UNION ALL
                                                                SELECT
                                                                    CUSTOMER_CODE, ITEM_CODE, QUANTITY AS SALES_QTY, NVL(QUANTITY * NET_GROSS_RATE, 0) AS SALES_VALUE,
                                                                    0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE, 0 AS DEBIT_VALUE, 0 AS CREDIT_VALUE, NVL(FREE_QTY, 0) AS FREE_QTY
                                                                FROM
                                                                    SA_SALES_INVOICE
                                                                WHERE
                                                                    DELETED_FLAG = 'N'
                                                                   AND SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                   AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                                   AND COMPANY_CODE IN ({2})
                                                                   AND BRANCH_CODE IN ({3})

                                                                UNION ALL

                                                                SELECT
                                                                    CUSTOMER_CODE, ITEM_CODE, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                                                                    QUANTITY AS SALES_RET_QTY, NVL(QUANTITY * NET_GROSS_RATE, 0) AS SALES_RET_VALUE,
                                                                    0 AS DEBIT_VALUE, 0 AS CREDIT_VALUE, NVL(FREE_QTY * -1, 0) AS FREE_QTY
                                                                FROM
                                                                    SA_SALES_RETURN
                                                                WHERE
                                                                    DELETED_FLAG = 'N'
                                                                  AND RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                  AND RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                                  AND COMPANY_CODE IN ({2})
                                                                  AND BRANCH_CODE IN ({3})

                                                                UNION ALL

                                                                SELECT
                                                                    CUSTOMER_CODE, ITEM_CODE, 0 AS SALES_QTY, 0 AS SALES_VALUE, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE,
                                                                    NVL(QUANTITY * NET_GROSS_RATE, 0) AS DEBIT_VALUE, 0 AS CREDIT_VALUE, 0 AS FREE_QTY
                                                                FROM
                                                                    SA_DEBIT_NOTE
                                                                WHERE
                                                                    DELETED_FLAG = 'N'
                                                                    AND VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                    AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                                    AND COMPANY_CODE IN ({2})
                                                                    AND BRANCH_CODE IN ({3})


                                                                UNION ALL

                                                                SELECT
                                                                    CUSTOMER_CODE, ITEM_CODE, 0 AS SALES_QTY, 0 AS SALES_VALUE, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE,
                                                                    0 AS DEBIT_VALUE, NVL(QUANTITY * NET_GROSS_RATE, 0) AS CREDIT_VALUE, 0 AS FREE_QTY
                                                                FROM
                                                                    SA_CREDIT_NOTE
                                                                WHERE
                                                                    DELETED_FLAG = 'N'
                                                                  AND VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                                  AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                                  AND COMPANY_CODE IN ({2})
                                                                  AND BRANCH_CODE IN ({3})

                                                            ) A
                                                            JOIN SA_CUSTOMER_SETUP B ON A.CUSTOMER_CODE = B.CUSTOMER_CODE AND B.COMPANY_CODE IN ({2})
                                                            JOIN IP_ITEM_MASTER_SETUP D ON A.ITEM_CODE = D.ITEM_CODE AND B.COMPANY_CODE = D.COMPANY_CODE
                                                            LEFT JOIN IP_ITEM_SPEC_SETUP E ON D.ITEM_CODE = E.ITEM_CODE AND D.COMPANY_CODE = E.COMPANY_CODE -- Standard LEFT JOIN syntax
                                                        GROUP BY
                                                            B.CUSTOMER_CODE, B.ACC_CODE, B.CUSTOMER_EDESC, B.TPIN_VAT_NO, NVL(E.BRAND_NAME,'NA'), D.INDEX_MU_CODE
                                                    ) T
                                                LEFT JOIN
                                                    ( -- B: Non-Correlated Balance Calculation (Major Performance Gain)
                                                        SELECT
                                                            SUB_CODE,
                                                            ACC_CODE,
                                                            -- Opening Balance: Up to (but not including) 16-Jul-2024 OR special VOUCHER_NO = '0'
                                                            SUM(CASE WHEN VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD') OR VOUCHER_NO = '0' THEN NVL(DR_AMOUNT, 0) - NVL(CR_AMOUNT, 0) ELSE 0 END) AS OPENING,
                                                            -- Closing Balance: Up to and including 15-Jul-2025
                                                            SUM(CASE WHEN  VOUCHER_DATE <= TO_DATE('{0}', 'YYYY-MM-DD') THEN NVL(DR_AMOUNT, 0) - NVL(CR_AMOUNT, 0) ELSE 0 END) AS BALA
                                                        FROM
                                                            V$VIRTUAL_SUB_LEDGER
                                                        WHERE
                                                            DELETED_FLAG = 'N'
                                                            AND COMPANY_CODE IN ({2})
                                                            AND ( VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD') OR  VOUCHER_DATE <= TO_DATE('{0}', 'YYYY-MM-DD') OR VOUCHER_NO = '0') -- Restrict scan range
                                                        GROUP BY
                                                            SUB_CODE, ACC_CODE
                                                    ) B ON B.SUB_CODE = 'C' || T.CUSTOMER_CODE AND B.ACC_CODE = T.ACC_CODE
                                                ORDER BY
                                                    T.CUSTOMER_EDESC,
                                                    T.ITEM_EDESC", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<CustomerwiseNetSalesModel>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
        }

        public dynamic GetEmployeeCustomerwiseNetSalesReport(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");

            string query = string.Format(@"SELECT
                                            T.CUSTOMER_EDESC,
                                            T.TPIN_VAT_NO,
                                            T.EMPLOYEE_EDESC,
                                            NVL(B.OPENING, 0) AS OPENING,
                                            NVL(B.BALA, 0) AS BALA,
                                            T.SALES_QTY,
                                            T.SALES_VALUE,
                                            T.SALES_RET_QTY,
                                            T.SALES_RET_VALUE,
                                            T.DEBIT_VALUE,
                                            T.CREDIT_VALUE,
                                            T.FREE_QTY,
                                            (T.SALES_QTY - T.SALES_RET_QTY + T.FREE_QTY) AS NET_SALES_QTY,
                                            (T.SALES_VALUE - T.SALES_RET_VALUE + T.DEBIT_VALUE - T.CREDIT_VALUE) AS NET_SALES_VALUE
                                        FROM
                                            ( -- T: Main Sales/Returns/Notes Aggregation and Joins
                                                SELECT
                                                    B.CUSTOMER_CODE,
                                                    B.ACC_CODE,
                                                    B.CUSTOMER_EDESC,
                                                    B.TPIN_VAT_NO,
                                                    D.EMPLOYEE_EDESC,
                                                    SUM(A.SALES_QTY) AS SALES_QTY,
                                                    SUM(A.SALES_VALUE) AS SALES_VALUE,
                                                    SUM(A.SALES_RET_QTY) AS SALES_RET_QTY,
                                                    SUM(A.SALES_RET_VALUE) AS SALES_RET_VALUE,
                                                    SUM(A.DEBIT_VALUE) AS DEBIT_VALUE,
                                                    SUM(A.CREDIT_VALUE) AS CREDIT_VALUE,
                                                    SUM(A.FREE_QTY) AS FREE_QTY
                                                FROM
                                                    ( -- A: Combined Transaction Data using UNION ALL
                                                        SELECT
                                                            CUSTOMER_CODE, EMPLOYEE_CODE, QUANTITY AS SALES_QTY, NVL(QUANTITY * NET_GROSS_RATE, 0) AS SALES_VALUE,
                                                            0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE, 0 AS DEBIT_VALUE, 0 AS CREDIT_VALUE, NVL(FREE_QTY, 0) AS FREE_QTY
                                                        FROM
                                                            SA_SALES_INVOICE
                                                        WHERE
                                                            DELETED_FLAG = 'N'
                                                            AND SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                            AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                            AND COMPANY_CODE IN ({2})
                                                            AND BRANCH_CODE IN ({3})
                                                        UNION ALL

                                                        SELECT
                                                            CUSTOMER_CODE, EMPLOYEE_CODE, 0 AS SALES_QTY, 0 AS SALES_VALUE,
                                                            QUANTITY AS SALES_RET_QTY, NVL(QUANTITY * NET_GROSS_RATE, 0) AS SALES_RET_VALUE,
                                                            0 AS DEBIT_VALUE, 0 AS CREDIT_VALUE, NVL(FREE_QTY * -1, 0) AS FREE_QTY
                                                        FROM
                                                            SA_SALES_RETURN
                                                        WHERE
                                                            DELETED_FLAG = 'N'
                                                           AND RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                           AND RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                           AND COMPANY_CODE IN ({2})
                                                           AND BRANCH_CODE IN ({3})
                                                        UNION ALL

                                                        SELECT
                                                            CUSTOMER_CODE, EMPLOYEE_CODE, 0 AS SALES_QTY, 0 AS SALES_VALUE, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE,
                                                            NVL(QUANTITY * NET_GROSS_RATE, 0) AS DEBIT_VALUE, 0 AS CREDIT_VALUE, 0 AS FREE_QTY
                                                        FROM
                                                            SA_DEBIT_NOTE
                                                        WHERE
                                                            DELETED_FLAG = 'N'
                                                          AND VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                          AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                          AND COMPANY_CODE IN ({2})
                                                          AND BRANCH_CODE IN ({3})
                                                        UNION ALL

                                                        SELECT
                                                            CUSTOMER_CODE, EMPLOYEE_CODE, 0 AS SALES_QTY, 0 AS SALES_VALUE, 0 AS SALES_RET_QTY, 0 AS SALES_RET_VALUE,
                                                            0 AS DEBIT_VALUE, NVL(QUANTITY * NET_GROSS_RATE, 0) AS CREDIT_VALUE, 0 AS FREE_QTY
                                                        FROM
                                                            SA_CREDIT_NOTE
                                                        WHERE
                                                            DELETED_FLAG = 'N'
                                                           AND VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                           AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                           AND COMPANY_CODE IN ({2})
                                                           AND BRANCH_CODE IN ({3})
                                                    ) A
                                                    JOIN SA_CUSTOMER_SETUP B ON A.CUSTOMER_CODE = B.CUSTOMER_CODE AND B.COMPANY_CODE = '01'
                                                    JOIN HR_EMPLOYEE_SETUP D ON A.EMPLOYEE_CODE = D.EMPLOYEE_CODE AND B.COMPANY_CODE = D.COMPANY_CODE
                                                GROUP BY
                                                    B.CUSTOMER_CODE, B.ACC_CODE, B.CUSTOMER_EDESC, B.TPIN_VAT_NO, D.EMPLOYEE_EDESC
                                            ) T
                                        LEFT JOIN
                                            ( -- B: Non-Correlated Balance Calculation (Major Performance Gain)
                                                SELECT
                                                    SUB_CODE,
                                                    ACC_CODE,
                                                    -- Opening Balance: Up to (but not including) 16-Jul-2024 OR special VOUCHER_NO = '0'
                                                    SUM(CASE WHEN VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD') OR VOUCHER_NO = '0' THEN NVL(DR_AMOUNT, 0) - NVL(CR_AMOUNT, 0) ELSE 0 END) AS OPENING,
                                                    -- Closing Balance: Up to and including 15-Jul-2025
                                                    SUM(CASE WHEN VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') THEN NVL(DR_AMOUNT, 0) - NVL(CR_AMOUNT, 0) ELSE 0 END) AS BALA
                                                FROM
                                                    V$VIRTUAL_SUB_LEDGER
                                                WHERE
                                                    DELETED_FLAG = 'N'
                                                    AND COMPANY_CODE = '01'
                                                    AND (VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD') OR VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') OR VOUCHER_NO = '0') -- Restrict scan range
                                                GROUP BY
                                                    SUB_CODE, ACC_CODE
                                            ) B ON B.SUB_CODE = 'C' || T.CUSTOMER_CODE AND B.ACC_CODE = T.ACC_CODE
                                        ORDER BY
                                            T.EMPLOYEE_EDESC,
                                            T.CUSTOMER_EDESC", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<CustomerwiseNetSalesModel>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
        }

        public dynamic GetEmployeeProductwiseNetSalesReport(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT ITEM_EDESC, PRODUCT_CODE, INDEX_MU_CODE, EMPLOYEE_EDESC  
                                                , SALES_QTY, SALES_VALUE, SALES_RET_QTY, SALES_RET_VALUE, DEBIT_VALUE, CREDIT_VALUE ,FREE_QTY, SALES_QTY - SALES_RET_QTY + FREE_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE FROM (  
                                                SELECT B.ITEM_CODE, B.ITEM_EDESC, B.PRODUCT_CODE, B.INDEX_MU_CODE, D.EMPLOYEE_EDESC, SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE, SUM(FREE_QTY) FREE_QTY  FROM (  
                                                SELECT A.ITEM_CODE, A.EMPLOYEE_CODE, SUM(QUANTITY) SALES_QTY, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, NVL(SUM(NVL(A.FREE_QTY,0)),0) FREE_QTY    FROM SA_SALES_INVOICE A  
                                                WHERE A.DELETED_FLAG = 'N'  
                                                AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                AND A.COMPANY_CODE IN ({2})
                                                AND A.BRANCH_CODE IN ({3}) 
                                                GROUP BY  A.ITEM_CODE,  A.EMPLOYEE_CODE  
                                                Union All  
                                                SELECT A.ITEM_CODE, A.EMPLOYEE_CODE, 0 SALES_QTY, 0 SALES_VALUE, SUM(QUANTITY)  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, NVL(SUM(NVL(A.FREE_QTY * -1,0)),0) FREE_QTY    FROM SA_SALES_RETURN A  
                                                WHERE A.DELETED_FLAG = 'N'  
                                                AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                AND A.COMPANY_CODE IN ({2})
                                                AND A.BRANCH_CODE IN ({3})  
                                                GROUP BY  A.ITEM_CODE, A.EMPLOYEE_CODE  
                                                Union All  
                                                SELECT A.ITEM_CODE, A.EMPLOYEE_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE, 0 FREE_QTY  FROM SA_DEBIT_NOTE A  
                                                WHERE A.DELETED_FLAG = 'N'  
                                                AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                AND A.COMPANY_CODE IN ({2})
                                                AND A.BRANCH_CODE IN ({3})
                                                GROUP BY  A.ITEM_CODE, A.EMPLOYEE_CODE  
                                                Union All  
                                                SELECT A.ITEM_CODE, A.EMPLOYEE_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE, 0 FREE_QTY  FROM SA_CREDIT_NOTE A  
                                                WHERE A.DELETED_FLAG = 'N'  
                                                AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                                AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                                AND A.COMPANY_CODE IN ({2})
                                                AND A.BRANCH_CODE IN ({3}) 
                                                GROUP BY  A.ITEM_CODE, A.EMPLOYEE_CODE  
                                                ORDER BY ITEM_CODE) A,  IP_ITEM_MASTER_SETUP B ,  HR_EMPLOYEE_SETUP D  
                                                WHERE A.ITEM_CODE = B.ITEM_CODE  
                                                AND  A.EMPLOYEE_CODE = D.EMPLOYEE_CODE  
                                                AND B.COMPANY_CODE = D.COMPANY_CODE  
                                                AND D.COMPANY_CODE IN ({2})  
                                                GROUP BY   A.ITEM_CODE,  A.EMPLOYEE_CODE, B.ITEM_EDESC, B.PRODUCT_CODE, B.INDEX_MU_CODE, D.EMPLOYEE_EDESC  
                                                ) A ORDER BY EMPLOYEE_EDESC, ITEM_EDESC", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<CustomerwiseNetSalesModel>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
        }

        public List<ProductwiseNetSalesModel> GetDealerNetSalesReport(ReportFiltersModel filters, User userinfo)
        {
            var companyCode = filters.CompanyFilter?.Any() == true ? string.Join(",", filters.CompanyFilter.Select(c => $"'{c}'"))
                              : $"'{userinfo.company_code}'";
            var branchCode = filters.BranchFilter?.Any() == true
                  ? string.Join(",", filters.BranchFilter.Select(b => $"'{b}'"))
                  : $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT CUSTOMER_EDESC, ITEM_EDESC, PRODUCT_CODE,  TPIN_VAT_NO, INDEX_MU_CODE, SALES_QTY, SALES_VALUE, DEBIT_VALUE, CREDIT_VALUE, FREE_QTY, SALES_RET_QTY, SALES_RET_VALUE, SALES_QTY - SALES_RET_QTY + FREE_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE FROM (  
                                            SELECT B.PARTY_TYPE_EDESC CUSTOMER_EDESC, D.ITEM_EDESC, D.PRODUCT_CODE, INDEX_MU_CODE INDEX_MU_CODE, B.PAN_NO TPIN_VAT_NO,  SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE, SUM(FREE_QTY) FREE_QTY FROM (  
                                            SELECT A.PARTY_TYPE_CODE,  A.ITEM_CODE, SUM(QUANTITY) SALES_QTY, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, NVL(SUM(NVL(A.FREE_QTY,0)),0) FREE_QTY  FROM SA_SALES_INVOICE A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND A.SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.PARTY_TYPE_CODE,  A.ITEM_CODE  
                                            Union All  
                                            SELECT A.PARTY_TYPE_CODE, A.ITEM_CODE, 0 SALES_QTY, 0 SALES_VALUE, SUM(QUANTITY)  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE, NVL(SUM(NVL(A.FREE_QTY * -1,0)),0) FREE_QTY  FROM SA_SALES_RETURN A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3}) 
                                            GROUP BY  A.PARTY_TYPE_CODE, A.ITEM_CODE  
                                            Union All  
                                            SELECT  A.PARTY_TYPE_CODE, A.ITEM_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE, 0 FREE_QTY  FROM SA_DEBIT_NOTE A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3}) 
                                            GROUP BY  A.PARTY_TYPE_CODE, A.ITEM_CODE  
                                            Union All  
                                            SELECT A.PARTY_TYPE_CODE, A.ITEM_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE, 0 FREE_QTY  FROM SA_CREDIT_NOTE A  
                                            WHERE A.DELETED_FLAG = 'N'  
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})  
                                            GROUP BY  A.PARTY_TYPE_CODE, A.ITEM_CODE  
                                            ORDER BY PARTY_TYPE_CODE) A,  IP_PARTY_TYPE_CODE B,  IP_ITEM_MASTER_SETUP D Where a.PARTY_TYPE_CODE = b.PARTY_TYPE_CODE AND A.ITEM_CODE = D.ITEM_CODE AND B.COMPANY_CODE = D.COMPANY_CODE AND D.COMPANY_CODE IN ({2})  
                                            GROUP BY A.PARTY_TYPE_CODE,  A.ITEM_CODE, B.PARTY_TYPE_EDESC, D.ITEM_EDESC, D.INDEX_MU_CODE , B.PAN_NO, D.PRODUCT_CODE ) ORDER BY CUSTOMER_EDESC, ITEM_EDESC 
                                            ", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var datewise = _objectEntity.SqlQuery<ProductwiseNetSalesModel>(query).ToList();
            return datewise;
        }

        public List<ProductwiseNetPurchaseViewModel> GetProductwiseNetPurchaseReport(ReportFiltersModel filters, User userinfo)
        {
            var companyCode = $"'{userinfo.company_code}'";
            var branchCode = $"'{userinfo.branch_code}'";



            string query = string.Format(@"SELECT ITEM_EDESC, PRODUCT_CODE, INDEX_MU_CODE, SALES_ROLL_QTY, SALES_QTY AS PURCHASE_QTY, SALES_VALUE AS PURCHASE_VALUE, SALES_RET_ROLL_QTY,
                                             SALES_RET_QTY AS PURCHASE_RET_QTY, SALES_RET_VALUE AS PURCHASE_RET_VALUE , SALES_ROLL_QTY - SALES_RET_ROLL_QTY NET_ROLL_QTY , 
                                             SALES_QTY - SALES_RET_QTY NET_SALES_QTY , SALES_VALUE - SALES_RET_VALUE NET_SALES_VALUE  FROM ( SELECT D.ITEM_EDESC, D.PRODUCT_CODE,  D.INDEX_MU_CODE, 
                                             SUM(A.SALES_ROLL_QTY) SALES_ROLL_QTY, SUM(A.SALES_QTY) SALES_QTY ,  SUM(A.SALES_VALUE) SALES_VALUE, 
                                             SUM(SALES_RET_ROLL_QTY) SALES_RET_ROLL_QTY, 
                                             SUM(A.SALES_RET_QTY) SALES_RET_QTY, 
                                             SUM(A.SALES_RET_VALUE) SALES_RET_VALUE FROM ( SELECT A.COMPANY_CODE, A.BRANCH_CODE,
                                             A.ITEM_CODE, SUM(NVL(A.ROLL_QTY,0)) SALES_ROLL_QTY, SUM(NVL(A.QUANTITY,0)) SALES_QTY, 
                                             SUM(NVL(A.QUANTITY*A.CALC_UNIT_PRICE*EXCHANGE_RATE,0)) SALES_VALUE,
                                             0 SALES_RET_ROLL_QTY, 0 SALES_RET_QTY, 0 SALES_RET_VALUE  
                                             FROM IP_PURCHASE_INVOICE A WHERE 
                                             A.DELETED_FLAG = 'N'
                                             AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                             AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                             AND A.COMPANY_CODE IN ({2})
                                             AND A.BRANCH_CODE IN ({3}) 
                                             GROUP BY  A.COMPANY_CODE, A.BRANCH_CODE,  A.ITEM_CODE 
                                             Union All 
                                             SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.ITEM_CODE, 0 SALES_ROLL_QTY, 0 SALES_QTY, 0 SALES_VALUE, 
                                             SUM(NVL(A.ROLL_QTY,0)) SALES_RET_ROLL_QTY, SUM(NVL(A.QUANTITY,0))  SALES_RET_QTY, 
                                             SUM(NVL(A.QUANTITY*A.CALC_UNIT_PRICE*EXCHANGE_RATE,0)) SALES_RET_VALUE  FROM IP_PURCHASE_RETURN A 
                                             WHERE A.DELETED_FLAG = 'N' 
                                             AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                             AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                             AND A.COMPANY_CODE IN ({2})
                                             AND A.BRANCH_CODE IN ({3})
                                             GROUP BY  A.COMPANY_CODE, A.BRANCH_CODE,  A.ITEM_CODE ORDER BY 1) A,
                                             IP_ITEM_MASTER_SETUP D WHERE A.COMPANY_CODE = D.COMPANY_CODE 
                                             AND  A.ITEM_CODE = D.ITEM_CODE AND A.COMPANY_CODE = D.COMPANY_CODE GROUP BY  A.ITEM_CODE,  D.ITEM_EDESC, D.INDEX_MU_CODE, 
                                             D.PRODUCT_CODE  )ORDER BY   ITEM_EDESC", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var purchase = _objectEntity.SqlQuery<ProductwiseNetPurchaseViewModel>(query).ToList();
            return purchase;
        }

        public List<ProductwiseNetPurchaseViewModel> GetDatewiseNetPurchaseReport(ReportFiltersModel filters, User userinfo)
        {
            var companyCode = $"'{userinfo.company_code}'";
            var branchCode = $"'{userinfo.branch_code}'";



            string query = string.Format(@"SELECT SALES_DATE AS EDATE, BS_DATE(SALES_DATE) AS NDATE, SALES_QTY AS PURCHASE_QTY, SALES_VALUE AS PURCHASE_VALUE, SALES_RET_QTY AS PURCHASE_RET_QTY,
                                             SALES_RET_VALUE AS PURCHASE_RET_VALUE, SALES_QTY - SALES_RET_QTY NET_SALES_QTY, SALES_VALUE - SALES_RET_VALUE NET_SALES_VALUE 
                                             FROM ( SELECT A.INVOICE_DATE SALES_DATE,  SUM(A.SALES_QTY) SALES_QTY,  SUM(A.SALES_VALUE) SALES_VALUE, 
                                             SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE FROM ( SELECT A.COMPANY_CODE, A.BRANCH_CODE, 
                                             A.INVOICE_DATE, SUM(NVL(A.QUANTITY,0)) SALES_QTY, SUM(NVL(A.QUANTITY*A.CALC_UNIT_PRICE*EXCHANGE_RATE,0)) SALES_VALUE,
                                             0 SALES_RET_QTY, 0 SALES_RET_VALUE  FROM IP_PURCHASE_INVOICE A
                                             WHERE A.DELETED_FLAG = 'N'
                                             AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                             AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                             AND A.COMPANY_CODE IN ({2})
                                             AND A.BRANCH_CODE IN ({3})
                                             GROUP BY  A.COMPANY_CODE, A.BRANCH_CODE, A.INVOICE_DATE   
                                             Union All
                                             SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.RETURN_DATE SALES_DATE,  0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY,0))  SALES_RET_QTY, 
                                             SUM(NVL(A.QUANTITY*A.CALC_UNIT_PRICE*EXCHANGE_RATE,0)) SALES_RET_VALUE  FROM IP_PURCHASE_RETURN A WHERE A.DELETED_FLAG = 'N' 
                                             AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                             AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                             AND A.COMPANY_CODE IN ({2})
                                             AND A.BRANCH_CODE IN ({3})
                                             GROUP BY  A.COMPANY_CODE, A.BRANCH_CODE, A.RETURN_DATE  ORDER BY INVOICE_DATE) A 
                                             GROUP BY A.INVOICE_DATE )ORDER BY SALES_DATE 
                                            ", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var datewise = _objectEntity.SqlQuery<ProductwiseNetPurchaseViewModel>(query).ToList();
            return datewise;
        }

        public List<SalesAboveOneLakhModel> GetPurchaseAboveOneLakh(ReportFiltersModel filters, User userinfo)
        {
            var companyCode = $"'{userinfo.company_code}'";
            var branchCode = $"'{userinfo.branch_code}'";

            string query = string.Format(@"SELECT  PAN,  NAME_OF_TAXPAYER , TRADE_NAME_TYPE, PURCHASE_SALES, SUM(EXEMPTED_AMOUNT) EXEMPTED_AMOUNT , SUM(TAXABLE_AMOUNT) TAXABLE_AMOUNT,'' REMARKS  FROM (  
                                            SELECT B.SUPPLIER_CODE,  B.TPIN_VAT_NO PAN, B.SUPPLIER_EDESC NAME_OF_TAXPAYER , 'E' TRADE_NAME_TYPE,'P' PURCHASE_SALES, EXEMPT_AMOUNT EXEMPTED_AMOUNT,  TAXABLE_AMOUNT   TAXABLE_AMOUNT FROM (  
                                            SELECT VOUCHER_NO, VOUCHER_DATE, COMPANY_CODE, PARTY_CODE, EXEMPT_AMOUNT *EXCHANGE_RATE EXEMPT_AMOUNT, ((TAXABLE_AMOUNT*EXCHANGE_RATE))  TAXABLE_AMOUNT  
                                            FROM V$PURCHASE_TAX_REPORT A  
                                            Where 1 = 1  
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})
                                            AND P_TYPE = 'LOC') A, IP_SUPPLIER_SETUP B  
                                            Where a.PARTY_CODE = b.SUPPLIER_CODE  
                                            AND A.COMPANY_CODE = B.COMPANY_CODE  
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')   
                                            ) HAVING SUM(TAXABLE_AMOUNT + EXEMPTED_AMOUNT) >= 100000  
                                            GROUP BY  PAN,  NAME_OF_TAXPAYER , TRADE_NAME_TYPE, PURCHASE_SALES  
                                            ORDER BY NAME_OF_TAXPAYER", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var datewise = _objectEntity.SqlQuery<SalesAboveOneLakhModel>(query).ToList();
            return datewise;
        }

        public dynamic GetSupplierwiseNetPurchaseReport(dynamic data, User userinfo)
        {
            var filters = data?.filters;
            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
                : $"'{userinfo.company_code}'";

            // Safely cast BranchFilter to List<string>
            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data.toADdate.ToString("yyyy-MM-dd");
            string query = string.Format(@"SELECT (SELECT SUM(NVL(CR_AMOUNT,0)- NVL(DR_AMOUNT,0)) FROM V$VIRTUAL_SUB_LEDGER 
                                            WHERE COMPANY_CODE IN ({2})
                                            AND DELETED_FLAG = 'N'  
                                            AND SUB_CODE = 'S'||A.SUPPLIER_CODE AND ACC_CODE = A.ACC_CODE AND (VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD') OR VOUCHER_NO = '0' )) OPENING ,
                                            (SELECT SUM(NVL(CR_AMOUNT,0)- NVL(DR_AMOUNT,0)) FROM V$VIRTUAL_SUB_LEDGER WHERE 
                                            COMPANY_CODE IN ({2}) 
                                            AND DELETED_FLAG = 'N'  AND SUB_CODE = 'S'||A.SUPPLIER_CODE AND ACC_CODE = A.ACC_CODE 
                                            AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') ) BALANCE ,
                                             CUSTOMER_EDESC, TPIN_VAT_NO, ITEM_EDESC, PRODUCT_CODE, INDEX_MU_CODE, SALES_QTY, SALES_VALUE, SALES_RET_QTY, 
                                             SALES_RET_VALUE, SALES_QTY - SALES_RET_QTY NET_SALES_QTY, 
                                             SALES_VALUE - SALES_RET_VALUE NET_SALES_VALUE FROM ( SELECT B.SUPPLIER_CODE, B.SUPPLIER_EDESC CUSTOMER_EDESC, 
                                             B.TPIN_VAT_NO, B.ACC_CODE, D.ITEM_EDESC, D.PRODUCT_CODE, D.INDEX_MU_CODE,  SUM(A.SALES_QTY) SALES_QTY, 
                                             SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY,
                                             SUM(A.SALES_RET_VALUE) SALES_RET_VALUE FROM ( SELECT A.COMPANY_CODE, A.BRANCH_CODE, 
                                             A.SUPPLIER_CODE,  A.ITEM_CODE, SUM(NVL(A.QUANTITY,0)) SALES_QTY,
                                             SUM(NVL(A.QUANTITY*A.CALC_UNIT_PRICE*EXCHANGE_RATE,0)) SALES_VALUE,
                                             0 SALES_RET_QTY, 0 SALES_RET_VALUE  FROM IP_PURCHASE_INVOICE A WHERE A.DELETED_FLAG = 'N' 
                                            AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3}) 
                                             GROUP BY  A.COMPANY_CODE, A.BRANCH_CODE,A.SUPPLIER_CODE,  A.ITEM_CODE
                                             Union All
                                             SELECT A.COMPANY_CODE, A.BRANCH_CODE, A.SUPPLIER_CODE, 
                                             A.ITEM_CODE, 0 SALES_QTY, 0 SALES_VALUE, SUM(NVL(A.QUANTITY,0))  SALES_RET_QTY,
                                             SUM(NVL(A.QUANTITY*A.CALC_UNIT_PRICE*EXCHANGE_RATE,0)) SALES_RET_VALUE  FROM IP_PURCHASE_RETURN A WHERE A.DELETED_FLAG = 'N'
                                            AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3}) 
                                             GROUP BY  A.COMPANY_CODE, A.BRANCH_CODE, A.SUPPLIER_CODE, A.ITEM_CODE ORDER BY SUPPLIER_CODE) A, 
                                             IP_SUPPLIER_SETUP B ,  IP_ITEM_MASTER_SETUP D Where a.SUPPLIER_CODE = b.SUPPLIER_CODE AND A.COMPANY_CODE = B.COMPANY_CODE 
                                             AND  A.ITEM_CODE = D.ITEM_CODE AND A.COMPANY_CODE = D.COMPANY_CODE GROUP BY   A.SUPPLIER_CODE, 
                                             A.ITEM_CODE, B.SUPPLIER_EDESC, D.ITEM_EDESC, D.PRODUCT_CODE, D.INDEX_MU_CODE, B.SUPPLIER_CODE,
                                             B.ACC_CODE, B.TPIN_VAT_NO  ) A ORDER BY CUSTOMER_EDESC, ITEM_EDESC", FromDate, ToDate, companyCode, branchCode);

            var monthwise = _objectEntity.SqlQuery<CustomerwiseNetSalesModel>(query).ToList();
            var groupedData = monthwise;
            return groupedData;
        }

        public List<DatewisePurchaseDetailViewModel> GetDatewisePurchaseDetailReport(ReportFiltersModel filters, User userinfo)
        {
            string formCodeFilterSql = string.Empty;

            // Check if the List exists AND has items.
            if (filters.FormCodeFilter != null && filters.FormCodeFilter.Any())
            {
                // 1. Join the list items into a single, comma-separated string (e.g., "408,415")
                string commaSeparatedCodes = string.Join(",", filters.FormCodeFilter);

                // 2. Format the string for SQL IN clause: "'408','415'"
                string codesInQuotes = $"'{commaSeparatedCodes.Replace(",", "','")}'";

                // 3. Create the full SQL WHERE clause fragment
                formCodeFilterSql = $" AND A.FORM_CODE IN ({codesInQuotes})";
            }

            var companyCode = $"'{userinfo.company_code}'";
            var branchCode = $"'{userinfo.branch_code}'";

            // --- Define the Base Query (Part 1) ---
            // Stop the string literal BEFORE the insertion point and before the date range filters ({0} and {1}).
            string baseQuery = @"SELECT INVOICE_DATE, MITI, ORDER_NO, INVOICE_NO, LC_NAME, SUPPLIER_CODE, SUPPLIER_EDESC, ADDRESS, TPIN_VAT_NO, ITEM_CODE, ITEM_EDESC, PRODUCT_CODE
                         , UNIT, QUANTITY, UNIT_PRICE, TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, ROLL_QTY , EXCISE_DUTY, EXCISE_DUTYII, DISCOUNT, CASH_DISCOUNT, SPECIAL_DISCOUNT, YEARLY_DISCOUNT,BILL_DISCOUNT, LUX_TAX, ADD_CHARGE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) +LUX_TAX + ADD_CHARGE ) TAXABLE_TOTAL_PRICE
                         , CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + LUX_TAX + ADD_CHARGE ) * (NON_VAT * .13),2) ELSE 0 END VAT_TOTAL_PRICE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + ADD_CHARGE - DISCOUNT - CASH_DISCOUNT + CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + ADD_CHARGE - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) ) * (NON_VAT * .13),2) ELSE 0 END ) INVOICE_TOTAL_PRICE, VEHICLE_NO, DESTINATION
                         ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , UPC, CATEGORY, SHELF_LIFE, WEIGHT, UOM, STATUS, FISCAL, FORM_CODE, BRAND_NAME, NON_VAT , EXCHANGE_RATE, CURRENCY_CODE, TOTAL_IN_NRS , LANDED_IN_NRS , SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, PP_NO, HS_CODE, HSN
                         FROM (
                             SELECT A.FORM_CODE, A.INVOICE_DATE, BS_DATE(A.INVOICE_DATE) MITI , FN_GET_SECOND_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.INVOICE_NO, A.SERIAL_NO) ORDER_NO, A.INVOICE_NO ,
                             H.SUPPLIER_EDESC LC_NAME ,F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO, C.ITEM_CODE, C.ITEM_EDESC, c.PRODUCT_CODE, A.MU_CODE UNIT,
                             a.QUANTITY , a.UNIT_PRICE, a.TOTAL_PRICE, A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.ROLL_QTY, A.EXCHANGE_RATE, A.CURRENCY_CODE, (A.TOTAL_PRICE * A.EXCHANGE_RATE) TOTAL_IN_NRS, (A.CALC_TOTAL_PRICE * A.EXCHANGE_RATE) LANDED_IN_NRS, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, (SELECT PP_NO FROM FA_PP_DETAIL_TRANSACTION WHERE REFERENCE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE AND FORM_CODE = A.FORM_CODE AND ROWNUM = 1) PP_NO
                             , CASE WHEN EXCISE_AMOUNT IS NOT NULL AND EXCISE_AMOUNT > 0 THEN NVL(EXCISE_AMOUNT,0) ELSE ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTY
                             , CASE WHEN EXCISE_AMOUNTII IS NOT NULL AND EXCISE_AMOUNTII > 0 THEN NVL(EXCISE_AMOUNTII,0) ELSE ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNTII,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END EXCISE_DUTYII
                             , CASE WHEN DISCOUNT_AMOUNT IS NOT NULL AND DISCOUNT_AMOUNT > 0 THEN NVL(DISCOUNT_AMOUNT,0) ELSE ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * (SELECT CASE WHEN SUM(NVL(DISCOUNT_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END DISCOUNT
                             ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) CASH_DISCOUNT
                             ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) SPECIAL_DISCOUNT
                             ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) YEARLY_DISCOUNT
                             ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) BILL_DISCOUNT
                             ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) LUX_TAX
                             ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'A' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) ADD_CHARGE
                             , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2})) AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN NVL(VAT_AMOUNT,0) ELSE ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = X.COMPANY_CODE) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE
                             ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION
                             ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO
                             , FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS
                             ,G.ITEM_SIZE UPC, G.TYPE CATEGORY, G.GRADE SHELF_LIFE, G.SIZE_LENGHT WEIGHT, G.THICKNESS UOM, G.REMARKS STATUS, (SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL, g.BRAND_NAME, DECODE(C.NON_VAT_FLAG,'N',1,0) NON_VAT, SERIAL_NO, C.HS_CODE, A.HSN
                             FROM IP_PURCHASE_INVOICE A, IP_ITEM_MASTER_SETUP C, SHIPPING_TRANSACTION D, IP_SUPPLIER_SETUP F, IP_ITEM_SPEC_SETUP G, IP_SUPPLIER_SETUP H
                             WHERE A.ITEM_CODE = C.ITEM_CODE
                             AND A.COMPANY_CODE = C.COMPANY_CODE
                             AND A.SUPPLIER_CODE = F.SUPPLIER_CODE
                             AND A.COMPANY_CODE = F.COMPANY_CODE
                             AND A.INVOICE_NO = D.VOUCHER_NO (+)
                             AND A.COMPANY_CODE = D.COMPANY_CODE (+)
                             AND C.ITEM_CODE = G.ITEM_CODE (+)
                             AND C.COMPANY_CODE = G.COMPANY_CODE (+)
                             AND A.LC_NO = H.SUPPLIER_CODE (+)
                             AND A.COMPANY_CODE = H.COMPANY_CODE (+)
                             -- >>> INSERTION POINT <<< 
                             AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD')
                             AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                             AND A.COMPANY_CODE IN ({2})
                             AND A.BRANCH_CODE IN ({3})
                             AND A.DELETED_FLAG = 'N'"; // End of the base query string literal

            // --- Construct the Final Query (Concatenation) ---
            // The final query is the base string + the dynamic filter + the closing clauses.
            string finalQuery = string.Format(baseQuery + formCodeFilterSql + @"
                                    ORDER BY INVOICE_NO, A.SERIAL_NO
                                )
                                ORDER BY INVOICE_DATE, INVOICE_NO, SERIAL_NO",
                                        filters.FromDate, filters.ToDate, companyCode, branchCode);

            // --- Execute the Query ---
            var purchase = _objectEntity.SqlQuery<DatewisePurchaseDetailViewModel>(finalQuery).ToList();
            var groupedData = purchase;
            return groupedData;
        }
        public List<DatewisePurchaseDetailViewModel> GetBillwisePurchaseSummaryReport(ReportFiltersModel filters, User userinfo)
        {
            // --- C# OPTIMIZATION: Dynamic Filter Construction (Corrected) ---
            string formCodeFilterSql = string.Empty;

            if (filters.FormCodeFilter != null && filters.FormCodeFilter.Any())
            {
                // 1. Join the list items (e.g., ["408", "415"] -> "408,415")
                string commaSeparatedCodes = string.Join(",", filters.FormCodeFilter);

                // 2. Format the string for SQL IN clause (e.g., "408,415" -> "'408','415'")
                string codesInQuotes = $"'{commaSeparatedCodes.Replace(",", "','")}'";

                // 3. Create the full SQL WHERE clause fragment
                formCodeFilterSql = $" AND A.FORM_CODE IN ({codesInQuotes})";
            }

            // Prepare fixed parameters, using string interpolation for cleaner quotes
            var companyCode = $"'{userinfo.company_code}'";
            var branchCode = $"'{userinfo.branch_code}'";

            string baseQueryPart1 = @"SELECT INVOICE_DATE, MITI, ORDER_NO, INVOICE_NO, MANUAL_NO,LC_NAME, SUPPLIER_CODE, SUPPLIER_EDESC, ADDRESS, TPIN_VAT_NO
                                 , SUM(QUANTITY) QUANTITY, SUM(TOTAL_PRICE) TOTAL_PRICE, SUM(NVL(SECOND_QUANTITY,0)) SECOND_QUANTITY, SUM(NVL(THIRD_QUANTITY,0)) THIRD_QUANTITY, SUM(NVL(ROLL_QTY,0))ROLL_QTY , SUM(EXCISE_DUTY) EXCISE_DUTY , SUM(DISCOUNT) DISCOUNT, SUM(CASH_DISCOUNT) CASH_DISCOUNT, SUM(SPECIAL_DISCOUNT) SPECIAL_DISCOUNT, SUM(YEARLY_DISCOUNT) YEARLY_DISCOUNT, SUM(BILL_DISCOUNT) BILL_DISCOUNT, SUM(LUX_TAX) LUX_TAX, SUM(ADD_CHARGE) ADD_CHARGE, (SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) - (SUM(DISCOUNT) + SUM(CASH_DISCOUNT) + SUM(SPECIAL_DISCOUNT) + SUM(YEARLY_DISCOUNT) + SUM(BILL_DISCOUNT)) + SUM(LUX_TAX) + SUM(ADD_CHARGE)) TAXABLE_TOTAL_PRICE
                                 , SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE ,(SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) + SUM(LUX_TAX) + SUM(ADD_CHARGE) - SUM(DISCOUNT) - SUM(CASH_DISCOUNT) + SUM(VAT_TOTAL_PRICE)) INVOICE_TOTAL_PRICE, SUM(TOTAL_IN_NRS) TOTAL_IN_NRS, SUM(LANDED_IN_NRS) LANDED_IN_NRS , VEHICLE_NO, DESTINATION
                                 ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , FISCAL, FORM_CODE, EXCHANGE_RATE, CURRENCY_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, PP_NO FROM (
                                 SELECT A.FORM_CODE, A.MANUAL_NO, A.INVOICE_DATE, BS_DATE(A.INVOICE_DATE) MITI , FN_GET_SECOND_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.INVOICE_NO, 1) ORDER_NO, A.INVOICE_NO ,
                                 H.SUPPLIER_EDESC LC_NAME ,F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO,
                                 a.QUANTITY , a.TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, ROLL_QTY, A.EXCHANGE_RATE, A.CURRENCY_CODE, A.TOTAL_PRICE * A.EXCHANGE_RATE TOTAL_IN_NRS, A.CALC_TOTAL_PRICE * A.EXCHANGE_RATE LANDED_IN_NRS, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, (SELECT PP_NO FROM FA_PP_DETAIL_TRANSACTION WHERE REFERENCE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE AND FORM_CODE = A.FORM_CODE AND ROWNUM = 1) PP_NO
                                 ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) EXCISE_DUTY
                                 ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) EXCISE_DUTYII
                                 , ROUND((NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) DISCOUNT
                                 ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) CASH_DISCOUNT
                                 ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) SPECIAL_DISCOUNT
                                 ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) YEARLY_DISCOUNT
                                 ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) BILL_DISCOUNT
                                 ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) LUX_TAX
                                 ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'A' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) ADD_CHARGE
                                 , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2})) AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN
                                    NVL(VAT_AMOUNT,0)  ELSE
                                 ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE
                                 ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION
                                 ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO
                                 ,FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS
                                 ,(SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL
                                 FROM IP_PURCHASE_INVOICE A, SHIPPING_TRANSACTION D, IP_SUPPLIER_SETUP F, IP_SUPPLIER_SETUP H
                                 WHERE A.SUPPLIER_CODE = F.SUPPLIER_CODE
                                 AND A.COMPANY_CODE = F.COMPANY_CODE
                                 AND A.INVOICE_NO = D.VOUCHER_NO (+)
                                 AND A.COMPANY_CODE = D.COMPANY_CODE (+)
                                 AND A.LC_NO = H.SUPPLIER_CODE (+)
                                 AND A.COMPANY_CODE = H.COMPANY_CODE (+)
                                 -- END OF baseQueryPart1 HERE
                                 ";

            // --- Part 2: Final Query Assembly ---
            // Concatenate baseQueryPart1, the dynamic filter, and the rest of the fixed WHERE/GROUP BY clauses.
            string finalQuery = string.Format(baseQueryPart1 + formCodeFilterSql + @"
                                 AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD')
                                 AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                                 AND A.COMPANY_CODE IN ({2})
                                 AND A.BRANCH_CODE IN ({3})
                                 AND A.DELETED_FLAG = 'N'
                                 ORDER BY INVOICE_NO, A.SERIAL_NO
                                 )
                                 GROUP BY INVOICE_DATE, MITI, ORDER_NO, INVOICE_NO, MANUAL_NO, LC_NAME, SUPPLIER_CODE,
                                 SUPPLIER_EDESC, ADDRESS, TPIN_VAT_NO, VEHICLE_NO, DESTINATION, DRIVER_NAME, DRIVER_MOBILE_NO,
                                 DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO,
                                 TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , FISCAL, FORM_CODE, EXCHANGE_RATE,
                                 CURRENCY_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, PP_NO  ORDER BY INVOICE_NO",
                                         filters.FromDate, filters.ToDate, companyCode, branchCode);

            var purchase = _objectEntity.SqlQuery<DatewisePurchaseDetailViewModel>(finalQuery).ToList();
            return purchase;
        }

        public List<DatewisePurchaseDetailViewModel> GetProductwisePurchaseSummaryReport(ReportFiltersModel filters, User userinfo)
        {
            // --- 1. Form Code Filter Logic ---
            string formCodeFilterSql = string.Empty;

            if (filters.FormCodeFilter != null && filters.FormCodeFilter.Any())
            {
                // 1. Join the list items (e.g., ["408", "415"] -> "408,415")
                string commaSeparatedCodes = string.Join(",", filters.FormCodeFilter);

                // 2. Format the string for SQL IN clause (e.g., "408,415" -> "'408','415'")
                string codesInQuotes = $"'{commaSeparatedCodes.Replace(",", "','")}'";

                // 3. Create the full SQL WHERE clause fragment, starting with AND
                formCodeFilterSql = $" AND A.FORM_CODE IN ({codesInQuotes})";
            }
            // ------------------------------------

            var companyCode = $"'{userinfo.company_code}'";
            var branchCode = $"'{userinfo.branch_code}'";

            string baseQueryPart1 = @"SELECT * FROM ( 
                            SELECT ITEM_CODE, ITEM_EDESC, PRODUCT_CODE, UNIT 
                            , SUM(QUANTITY) QUANTITY, SUM(TOTAL_PRICE) / SUM(QUANTITY) UNIT_PRICE, SUM(TOTAL_PRICE) TOTAL_PRICE, SUM(SECOND_QUANTITY) SECOND_QUANTITY, SUM(THIRD_QUANTITY) THIRD_QUANTITY, SUM(ROLL_QTY) ROLL_QTY , SUM(EXCISE_DUTY) EXCISE_DUTY, SUM(EXCISE_DUTYII) EXCISE_DUTYII, SUM(DISCOUNT) DISCOUNT, SUM(CASH_DISCOUNT) CASH_DISCOUNT, SUM(SPECIAL_DISCOUNT) SPECIAL_DISCOUNT 
                            , SUM(YEARLY_DISCOUNT) YEARLY_DISCOUNT,SUM(BILL_DISCOUNT) BILL_DISCOUNT, SUM(LUX_TAX) LUX_TAX, SUM(ADD_CHARGE) ADD_CHARGE, SUM(TAXABLE_TOTAL_PRICE) TAXABLE_TOTAL_PRICE , SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE, SUM(INVOICE_TOTAL_PRICE) INVOICE_TOTAL_PRICE, SUM(LANDED_IN_NRS)LANDED_IN_NRS FROM ( 
                            SELECT ITEM_CODE, ITEM_EDESC, PRODUCT_CODE 
                            , UNIT, QUANTITY, UNIT_PRICE, TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, ROLL_QTY, EXCISE_DUTY, EXCISE_DUTYII, DISCOUNT, CASH_DISCOUNT, SPECIAL_DISCOUNT, YEARLY_DISCOUNT, BILL_DISCOUNT, LUX_TAX, ADD_CHARGE ,  (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) +LUX_TAX + ADD_CHARGE  ) TAXABLE_TOTAL_PRICE 
                            , CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) + LUX_TAX + ADD_CHARGE ) * (NON_VAT * .13),2) ELSE 0 END VAT_TOTAL_PRICE, (TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + ADD_CHARGE - DISCOUNT - CASH_DISCOUNT + CASE WHEN VAT_TOTAL_PRICE > 0 THEN ROUND((TOTAL_PRICE + EXCISE_DUTY + EXCISE_DUTYII + LUX_TAX + ADD_CHARGE - (DISCOUNT + CASH_DISCOUNT + SPECIAL_DISCOUNT + YEARLY_DISCOUNT + BILL_DISCOUNT) ) * (NON_VAT * .13),2) ELSE 0 END ) INVOICE_TOTAL_PRICE, LANDED_IN_NRS  
                            FROM ( 
                            SELECT A.FORM_CODE, A.INVOICE_DATE, A.INVOICE_NO , 
                            H.SUPPLIER_EDESC LC_NAME ,F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO, C.ITEM_CODE, C.ITEM_EDESC, c.PRODUCT_CODE, A.MU_CODE UNIT, 
                            a.QUANTITY , a.UNIT_PRICE, a.TOTAL_PRICE * A.EXCHANGE_RATE TOTAL_PRICE , A.SECOND_QUANTITY, A.THIRD_QUANTITY, A.ROLL_QTY, A.EXCHANGE_RATE, A.CURRENCY_CODE, A.TOTAL_PRICE * A.EXCHANGE_RATE TOTAL_IN_NRS, A.CALC_TOTAL_PRICE * A.EXCHANGE_RATE LANDED_IN_NRS 
                            , CASE WHEN EXCISE_AMOUNT IS NOT NULL AND EXCISE_AMOUNT > 0 THEN 
                             NVL(EXCISE_AMOUNT,0)  ELSE 
                             ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X 
                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' 
                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END * EXCHANGE_RATE EXCISE_DUTY 
                            , CASE WHEN EXCISE_AMOUNTII IS NOT NULL AND EXCISE_AMOUNTII > 0 THEN 
                             NVL(EXCISE_AMOUNTII,0)  ELSE 
                             ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X 
                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2})) AND APPLY_ON = 'D' 
                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * (SELECT CASE WHEN SUM(NVL(EXCISE_AMOUNTII,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END * EXCHANGE_RATE EXCISE_DUTYII 
                            , CASE WHEN DISCOUNT_AMOUNT IS NOT NULL AND DISCOUNT_AMOUNT > 0 THEN 
                             NVL(DISCOUNT_AMOUNT,0)  ELSE 
                             ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X 
                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D' 
                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * (SELECT CASE WHEN SUM(NVL(DISCOUNT_AMOUNT,0)) > 0 THEN 0 ELSE 1 END FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE) END * EXCHANGE_RATE DISCOUNT 
                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X 
                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D' 
                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE CASH_DISCOUNT 
                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X 
                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D' 
                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE SPECIAL_DISCOUNT 
                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X 
                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D' 
                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE YEARLY_DISCOUNT 
                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X 
                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D' 
                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE BILL_DISCOUNT 
                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X 
                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D' 
                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE LUX_TAX 
                            ,ROUND((NVL((SELECT X.CHARGE_AMOUNT * X.EXCHANGE_RATE  FROM CHARGE_TRANSACTION X 
                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'A' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D' 
                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE * EXCHANGE_RATE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) * EXCHANGE_RATE ADD_CHARGE 
                            , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2})) AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN 
                             NVL(VAT_AMOUNT,0)  ELSE 
                            ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X 
                            WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = X.COMPANY_CODE) AND APPLY_ON = 'D' 
                            AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE, DECODE(C.NON_VAT_FLAG,'N',1,0) * EXCHANGE_RATE NON_VAT 
                            FROM IP_PURCHASE_INVOICE A, IP_ITEM_MASTER_SETUP C, SHIPPING_TRANSACTION D, IP_SUPPLIER_SETUP F, IP_ITEM_SPEC_SETUP G, IP_SUPPLIER_SETUP H  
                            WHERE A.ITEM_CODE = C.ITEM_CODE 
                            AND A.COMPANY_CODE = C.COMPANY_CODE 
                            AND A.SUPPLIER_CODE = F.SUPPLIER_CODE 
                            AND A.COMPANY_CODE = F.COMPANY_CODE 
                            AND A.INVOICE_NO = D.VOUCHER_NO (+) 
                            AND A.COMPANY_CODE = D.COMPANY_CODE (+) 
                            AND C.ITEM_CODE = G.ITEM_CODE (+) 
                            AND C.COMPANY_CODE = G.COMPANY_CODE (+) 
                            AND A.LC_NO = H.SUPPLIER_CODE (+)
                             AND A.COMPANY_CODE = H.COMPANY_CODE(+)"; // <-- End of fixed WHERE conditions

            string finalQuery = string.Format(baseQueryPart1 + formCodeFilterSql + @" 
                            AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                            AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') 
                            AND A.COMPANY_CODE IN ({2})
                            AND A.BRANCH_CODE IN ({3})  
                           AND A.DELETED_FLAG = 'N' 
                          ORDER BY INVOICE_NO, A.SERIAL_NO 
                            ) 
                            )  GROUP BY ITEM_CODE, ITEM_EDESC, PRODUCT_CODE, UNIT 
                            ) ORDER BY ITEM_EDESC", filters.FromDate, filters.ToDate, companyCode, branchCode);

            // --- 3. Execute Query ---
            var purchase = _objectEntity.SqlQuery<DatewisePurchaseDetailViewModel>(finalQuery).ToList();

            return purchase;
        }

        public List<DatewisePurchaseDetailViewModel> GetBillwisePurchaseReturnSummaryReport(ReportFiltersModel filters, User userinfo)
        {
            string formCodeFilterSql = string.Empty;

            if (filters.FormCodeFilter != null && filters.FormCodeFilter.Any())
            {
                // Ensures each code is quoted for safe SQL IN clause: '408','415'
                var quotedCodes = filters.FormCodeFilter.Select(code => $"'{code}'");
                string codesInQuotes = string.Join(",", quotedCodes);

                // The resulting string will start with ' AND '
                formCodeFilterSql = $" AND A.FORM_CODE IN ({codesInQuotes})";
            }
            var companyCode = $"'{userinfo.company_code}'";
            var branchCode = $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT RETURN_DATE, MITI, ORDER_NO, RETURN_NO, MANUAL_NO, LC_NAME, SUPPLIER_CODE, SUPPLIER_EDESC, ADDRESS, TPIN_VAT_NO  
                                        , SUM(QUANTITY) QUANTITY, SUM(TOTAL_PRICE) TOTAL_PRICE, SUM(NVL(SECOND_QUANTITY,0)) SECOND_QUANTITY, SUM(NVL(THIRD_QUANTITY,0)) THIRD_QUANTITY, SUM(NVL(ROLL_QTY,0))ROLL_QTY , SUM(EXCISE_DUTY) EXCISE_DUTY , SUM(EXCISE_DUTYII) EXCISE_DUTYII, SUM(DISCOUNT) DISCOUNT, SUM(CASH_DISCOUNT) CASH_DISCOUNT, SUM(SPECIAL_DISCOUNT) SPECIAL_DISCOUNT, SUM(YEARLY_DISCOUNT) YEARLY_DISCOUNT, SUM(BILL_DISCOUNT) BILL_DISCOUNT, SUM(LUX_TAX) LUX_TAX, (SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) - (SUM(DISCOUNT) + SUM(CASH_DISCOUNT) + SUM(SPECIAL_DISCOUNT) + SUM(YEARLY_DISCOUNT) + SUM(BILL_DISCOUNT)) + SUM(LUX_TAX )) TAXABLE_TOTAL_PRICE  
                                        , SUM(VAT_TOTAL_PRICE) VAT_TOTAL_PRICE ,(SUM(TOTAL_PRICE) + SUM(EXCISE_DUTY) + SUM(EXCISE_DUTYII) + SUM(LUX_TAX) - SUM(DISCOUNT) - SUM(CASH_DISCOUNT) + SUM(VAT_TOTAL_PRICE)) INVOICE_TOTAL_PRICE , VEHICLE_NO, DESTINATION  
                                        ,DRIVER_NAME, DRIVER_MOBILE_NO, DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO, GATE_ENTRY_NO, LOADING_SLIP_NO, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS ,FISCAL, FORM_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, RETURN_TYPE FROM (  
                                        SELECT A.FORM_CODE, A.MANUAL_NO, A.RETURN_DATE, BS_DATE(A.RETURN_DATE) MITI , FN_GET_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.RETURN_NO, 1) ORDER_NO, A.RETURN_NO ,  
                                        H.SUPPLIER_EDESC LC_NAME ,F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO,  
                                        a.QUANTITY , a.TOTAL_PRICE, SECOND_QUANTITY, THIRD_QUANTITY, ROLL_QTY, SUPPLIER_INV_NO, SUPPLIER_INV_DATE  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'E' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) EXCISE_DUTY  
                                        ,ROUND((NVL((SELECT SUM(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'F' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) EXCISE_DUTYII  
                                        , ROUND((NVL((SELECT sum(X.CHARGE_AMOUNT) FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'D' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2)  DISCOUNT  
                                        ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'C' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) CASH_DISCOUNT  
                                        ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'S' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) SPECIAL_DISCOUNT  
                                        ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'Y' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) YEARLY_DISCOUNT  
                                        ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'B' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) BILL_DISCOUNT  
                                        ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'L' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * TOTAL_PRICE,2) LUX_TAX  
                                        , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2}))  AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                                         NVL(VAT_AMOUNT,0)  ELSE  
                                        ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                                        WHERE X.REFERENCE_NO = A.RETURN_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({2}))  AND APPLY_ON = 'D'  
                                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_RETURN WHERE RETURN_NO = A.RETURN_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2) END VAT_TOTAL_PRICE  
                                        ,FN_FETCH_DESC(A.COMPANY_CODE, 'IP_VEHICLE_CODE', D.VEHICLE_CODE) VEHICLE_NO , D.DESTINATION  
                                        ,D.DRIVER_NAME, D.DRIVER_MOBILE_NO, D.DRIVER_LICENSE_NO, D.FREIGHT_RATE, D.FREGHT_AMOUNT, D.WB_WEIGHT, D.WB_NO, D.GATE_ENTRY_NO, D.LOADING_SLIP_NO  
                                        , FN_FETCH_DESC(A.COMPANY_CODE, 'TRANSPORTER_SETUP','TRANSPORTER_CODE') TRANSPORTER_NAME, D.CN_NO, D.TRANSPORT_INVOICE_NO, D.SHIPPING_TERMS  
                                        ,(SELECT FISCAL_YEAR FROM PREFERENCE_SETUP WHERE COMPANY_CODE = A.COMPANY_CODE AND ROWNUM = 1) FISCAL , A.RETURN_TYPE  
                                        FROM IP_PURCHASE_RETURN A, SHIPPING_TRANSACTION D, IP_SUPPLIER_SETUP F ,IP_SUPPLIER_SETUP H   
                                        WHERE A.SUPPLIER_CODE = F.SUPPLIER_CODE  
                                        AND A.COMPANY_CODE = F.COMPANY_CODE  
                                        AND A.RETURN_NO = D.VOUCHER_NO (+)  
                                        AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
                                        AND A.LC_NO = H.SUPPLIER_CODE (+)  
                                        AND A.COMPANY_CODE = H.COMPANY_CODE (+)  
                                        AND A.RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                        AND A.RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
                                        AND A.COMPANY_CODE IN ({2})
                                        AND A.BRANCH_CODE IN ({3})  
                                          {4} 
                                        AND A.DELETED_FLAG = 'N'  
                                        ORDER BY RETURN_NO, A.SERIAL_NO  
                                        )  
                                        GROUP BY RETURN_DATE, MITI, ORDER_NO, RETURN_NO, MANUAL_NO, LC_NAME, SUPPLIER_CODE,
                                    SUPPLIER_EDESC, ADDRESS, TPIN_VAT_NO, VEHICLE_NO, DESTINATION, DRIVER_NAME, DRIVER_MOBILE_NO,
                                    DRIVER_LICENSE_NO, FREIGHT_RATE, FREGHT_AMOUNT, WB_WEIGHT, WB_NO,
                                    GATE_ENTRY_NO, LOADING_SLIP_NO, TRANSPORTER_NAME, CN_NO, TRANSPORT_INVOICE_NO, SHIPPING_TERMS , 
                                    FISCAL, FORM_CODE, SUPPLIER_INV_NO, SUPPLIER_INV_DATE, RETURN_TYPE  ORDER BY RETURN_NO", filters.FromDate, filters.ToDate, companyCode, branchCode, formCodeFilterSql);

            var purchase = _objectEntity.SqlQuery<DatewisePurchaseDetailViewModel>(query).ToList();
            return purchase;
        }

        public List<DatewisePurchaseDetailViewModel> GetPurchaseLandedCostSummaryReport(ReportFiltersModel filters, User userinfo)
        {
            // --- 1. Dynamic Filter Construction ---
            string formCodeFilterSql = string.Empty;

            if (filters.FormCodeFilter != null && filters.FormCodeFilter.Any())
            {
                // Enclose each item in single quotes and join them
                var quotedCodes = filters.FormCodeFilter.Select(code => $"'{code}'");
                string codesInQuotes = string.Join(",", quotedCodes);
                formCodeFilterSql = $" AND A.FORM_CODE IN ({codesInQuotes})";
            }

            // Prepare quoted single-value parameters for SQL IN clauses
            // (Note: Using parameterized queries is safer than string concatenation for user input)
            var companyCode = $"'{userinfo.company_code}'";
            var branchCode = $"'{userinfo.branch_code}'";

            // --- 2. Base SQL Query (Needs internal formatting for {2}) ---
            // The placeholder {2} is used inside the subquery for COMPANY_CODE in the base query string.
            // We must replace it NOW to prevent a conflict with the final string.Format call.
            string baseQueryTemplate = @"SELECT a.*,  D1 + D2 + D3 + D4 + D5 + D6 + D7 + D8 as TOTAL_LANDED_COST
                    FROM (SELECT A.FORM_CODE, A.INVOICE_DATE, BS_DATE(A.INVOICE_DATE) MITI , 
                        FN_GET_SECOND_REFERENCE_NO(A.COMPANY_CODE, A.BRANCH_CODE, A.FORM_CODE, A.INVOICE_NO, A.SERIAL_NO) ORDER_NO, A.INVOICE_NO ,  
                        G.SUPPLIER_EDESC LC_NAME ,F.SUPPLIER_CODE, F.SUPPLIER_EDESC, F.REGD_OFFICE_EADDRESS ADDRESS, F.TPIN_VAT_NO, C.ITEM_CODE, 
                        C.ITEM_EDESC, c.PRODUCT_CODE, A.MU_CODE UNIT ,  
                        a.QUANTITY , a.UNIT_PRICE * EXCHANGE_RATE UNIT_PRICE , a.TOTAL_PRICE * EXCHANGE_RATE TOTAL_PRICE, A.SECOND_QUANTITY, A.THIRD_QUANTITY, 
                        A.ROLL_QTY, A.EXCHANGE_RATE, A.CURRENCY_CODE, A.TOTAL_PRICE * A.EXCHANGE_RATE TOTAL_IN_NRS, A.CALC_TOTAL_PRICE * A.EXCHANGE_RATE LANDED_IN_NRS, 
                        SUPPLIER_INV_NO, SUPPLIER_INV_DATE, DUE_DATE, PP_NO  
                        ,ROUND(((SELECT SUM(DR_AMOUNT * EXCHANGE_RATE) DR_AMOUNT FROM V$VIRTUAL_GENERAL_LEDGER1 X, FINANCIAL_REFERENCE_DETAIL Y  
                        WHERE X.ACC_CODE IN (  
                        SELECT DISTINCT ACC_CODE FROM CHARGE_SETUP  
                        WHERE CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE  
                        WHERE SPECIFIC_CHARGE_FLAG IN ('E')))  
                        AND X.VOUCHER_NO = Y.VOUCHER_NO  
                        AND X.FORM_CODE = Y.FORM_CODE  
                        AND X.COMPANY_CODE = Y.COMPANY_CODE  
                        AND X.COMPANY_CODE = A.COMPANY_CODE  
                        AND Y.REFERENCE_NO = A.INVOICE_NO 
                        AND X.TRANSACTION_TYPE = 'DR' AND X.FORM_CODE IN (SELECT FORM_CODE FROM FORM_SETUP WHERE PURCHASE_EXPENSES_FLAG = 'Y')) / NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2)  EXCISE_AMT_E  
                        ,ROUND(((SELECT SUM(DR_AMOUNT * EXCHANGE_RATE) DR_AMOUNT FROM V$VIRTUAL_GENERAL_LEDGER1 X, FINANCIAL_REFERENCE_DETAIL Y  
                        WHERE X.ACC_CODE IN (  
                        SELECT DISTINCT ACC_CODE FROM CHARGE_SETUP  
                        WHERE CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE  
                        WHERE SPECIFIC_CHARGE_FLAG IN ('F')))  
                        AND X.VOUCHER_NO = Y.VOUCHER_NO  
                        AND X.FORM_CODE = Y.FORM_CODE  
                        AND X.COMPANY_CODE = Y.COMPANY_CODE  
                        AND X.COMPANY_CODE = A.COMPANY_CODE  
                        AND Y.REFERENCE_NO = A.INVOICE_NO 
                        AND X.TRANSACTION_TYPE = 'DR' AND X.FORM_CODE IN (SELECT FORM_CODE FROM FORM_SETUP WHERE PURCHASE_EXPENSES_FLAG = 'Y')) / NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  
                        FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2)  EXCISE_AMT_F  
                        ,ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                        WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'A' AND COMPANY_CODE = X.COMPANY_CODE) AND APPLY_ON = 'D'  
                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE),1)) * (TOTAL_PRICE),2)  EXCISE_AMT_II  
                        , CASE WHEN (SELECT COUNT(*) FROM CHARGE_SETUP WHERE CALC_ITEM_BASED_ON = 'Y' AND CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE IN ({0})) AND FORM_CODE = A.FORM_CODE AND COMPANY_CODE = A.COMPANY_CODE) != 0 THEN  
                        NVL(VAT_AMOUNT,0)  ELSE  
                        ROUND((NVL((SELECT X.CHARGE_AMOUNT FROM CHARGE_TRANSACTION X  
                        WHERE X.REFERENCE_NO = A.INVOICE_NO AND X.CHARGE_CODE IN (SELECT CHARGE_CODE FROM IP_CHARGE_CODE WHERE SPECIFIC_CHARGE_FLAG = 'V' AND COMPANY_CODE = X.COMPANY_CODE) AND APPLY_ON = 'D'  
                        AND X.COMPANY_CODE = A.COMPANY_CODE),0)/ NVL((SELECT CASE WHEN SUM(TOTAL_PRICE) = 0 THEN 1 ELSE SUM(TOTAL_PRICE) END  FROM IP_PURCHASE_INVOICE WHERE INVOICE_NO = A.INVOICE_NO AND COMPANY_CODE = A.COMPANY_CODE AND ITEM_CODE IN (SELECT ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE NON_VAT_FLAG = 'N' AND COMPANY_CODE IN ({0}))  ),1)) * (TOTAL_PRICE),2) * DECODE(C.NON_VAT_FLAG,'N',1,0)  END VAT_TOTAL_PRICE  
                        ,ROUND((NVL(D1_CHARGE_AMOUNT,0)),2) D1  
                        ,ROUND((NVL(D2_CHARGE_AMOUNT,0)),2) D2  
                        ,ROUND((NVL(D3_CHARGE_AMOUNT,0)),2) D3  
                        ,ROUND((NVL(D4_CHARGE_AMOUNT,0)),2) D4  
                        ,ROUND((NVL(D5_CHARGE_AMOUNT,0)),2) D5  
                        ,ROUND((NVL(D6_CHARGE_AMOUNT,0)),2) D6  
                        ,ROUND((NVL(D7_CHARGE_AMOUNT,0)),2) D7  
                        ,ROUND((NVL(D8_CHARGE_AMOUNT,0)),2) D8  
                        FROM IP_PURCHASE_INVOICE A,   
                        (SELECT * FROM (  
                        SELECT REFERENCE_NO VOUCHER_NO, SERIAL_NO, FORM_CODE, ITEM_CODE, COMPANY_CODE, CHARGE_CODE, CASE WHEN CHARGE_TYPE_FLAG = 'A' THEN 1 ELSE -1 END * CHARGE_AMOUNT * EXCHANGE_RATE CHARGE_AMOUNT  FROM CHARGE_TRANSACTION A  
                        WHERE APPLY_ON IN ('I','D')  
                        AND TABLE_NAME = 'IP_PURCHASE_INVOICE'  
                        )  
                        Pivot  
                        (SUM(CHARGE_AMOUNT)CHARGE_AMOUNT  
                        FOR CHARGE_CODE IN ('ID' D1, 'BC' D2, 'CL' D3, 'CD' D4, 'FI' D5, 'FL' D6, 'IS' D7, 'OE' D8)  
                        )) B, IP_ITEM_MASTER_SETUP C, IP_SUPPLIER_SETUP F, IP_SUPPLIER_SETUP G   
                        WHERE A.ITEM_CODE = C.ITEM_CODE  
                        AND A.COMPANY_CODE = C.COMPANY_CODE  
                        AND A.SUPPLIER_CODE = F.SUPPLIER_CODE  
                        AND A.COMPANY_CODE = F.COMPANY_CODE  
                        AND a.INVOICE_NO = b.VOUCHER_NO (+)  
                        AND A.FORM_CODE = B.FORM_CODE (+)  
                        AND A.COMPANY_CODE = B.COMPANY_CODE (+)  
                        AND A.ITEM_CODE = B.ITEM_CODE (+)  
                        AND A.SERIAL_NO = B.SERIAL_NO (+)  
                        AND A.LC_NO = G.SUPPLIER_CODE (+)  
                        AND A.COMPANY_CODE = G.COMPANY_CODE (+)";

            // Replace the internal {0} in the base query template with the actual companyCode.
            // This is the source of the common error (placeholder conflict).
            string query = string.Format(baseQueryTemplate, companyCode);


            // --- 3. Final Query Construction and Formatting ---
            // Now, we use {0} and {1} for the dates, and {2} and {3} for the remaining filters.
            string finalQuery = string.Format(query + formCodeFilterSql + @"
                        AND A.INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                        AND A.INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')       
                        AND A.COMPANY_CODE IN ({2})
                        AND A.BRANCH_CODE IN ({3})  
                        AND A.DELETED_FLAG = 'N'  
                        ORDER BY INVOICE_DATE, INVOICE_NO, A.SERIAL_NO  
                        ) a  
                        ORDER BY INVOICE_DATE, INVOICE_NO ",
                                filters.FromDate, filters.ToDate, companyCode, branchCode);

            // --- 4. Execution ---
            var purchase = _objectEntity.SqlQuery<DatewisePurchaseDetailViewModel>(finalQuery).ToList();

            return purchase;
        }
        public List<FormCodeData> GetFormCodeByCompany(ReportFiltersModel filters, User userinfo)
        {
            var companyCode = $"'{userinfo.company_code}'";
            var branchCode = $"'{userinfo.branch_code}'";
            string query = string.Format(@"SELECT DISTINCT
                                            FS.FORM_CODE,
                                            FS.FORM_EDESC
                                        FROM
                                            FORM_SETUP FS
                                        INNER JOIN
                                            FORM_DETAIL_SETUP FDS 
                                            ON FS.FORM_CODE = FDS.FORM_CODE
                                            AND FS.COMPANY_CODE = FDS.COMPANY_CODE
                                        WHERE
                                            FDS.TABLE_NAME = 'IP_PURCHASE_INVOICE'
                                            AND FS.COMPANY_CODE IN ({0})", companyCode);
            var purchaseInv = _objectEntity.SqlQuery<FormCodeData>(query).ToList();
            return purchaseInv;
        }

        public List<SalesAboveOneLakhModel> PurchaseAboveOneLakhReport(ReportFiltersModel filters, User userinfo)
        {
            var companyCode = $"'{userinfo.company_code}'";
            var branchCode = $"'{userinfo.branch_code}'";

            string query = string.Format(@"SELECT  PAN,  NAME_OF_TAXPAYER , TRADE_NAME_TYPE, PURCHASE_SALES, SUM(EXEMPTED_AMOUNT) EXEMPTED_AMOUNT , SUM(TAXABLE_AMOUNT) TAXABLE_AMOUNT,'' REMARKS  FROM (  
                                            SELECT B.SUPPLIER_CODE,  B.TPIN_VAT_NO PAN, B.SUPPLIER_EDESC NAME_OF_TAXPAYER , 'E' TRADE_NAME_TYPE,'P' PURCHASE_SALES, EXEMPT_AMOUNT EXEMPTED_AMOUNT,  TAXABLE_AMOUNT   TAXABLE_AMOUNT FROM (  
                                            SELECT VOUCHER_NO, VOUCHER_DATE, COMPANY_CODE, PARTY_CODE, EXEMPT_AMOUNT *EXCHANGE_RATE EXEMPT_AMOUNT, ((TAXABLE_AMOUNT*EXCHANGE_RATE))  TAXABLE_AMOUNT  
                                            FROM V$PURCHASE_TAX_REPORT A  
                                            Where 1 = 1  
                                            AND A.COMPANY_CODE IN ({2})
                                            AND A.BRANCH_CODE IN ({3})
                                            AND P_TYPE IN ('LOC', 'IMP', 'ADM', 'CPE')) A, IP_SUPPLIER_SETUP B  
                                            Where a.PARTY_CODE = b.SUPPLIER_CODE  
                                            AND A.COMPANY_CODE = B.COMPANY_CODE  
                                            AND A.VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') 
                                            AND A.VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')   
                                            ) HAVING SUM(TAXABLE_AMOUNT + EXEMPTED_AMOUNT) >= 100000  
                                            GROUP BY  PAN,  NAME_OF_TAXPAYER , TRADE_NAME_TYPE, PURCHASE_SALES  
                                            ORDER BY NAME_OF_TAXPAYER", filters.FromDate, filters.ToDate, companyCode, branchCode);
            var datewise = _objectEntity.SqlQuery<SalesAboveOneLakhModel>(query).ToList();
            return datewise;
        }

        public List<AuditTransactionModel> GetAuditTrasactionReport(ReportFiltersModel filters, User userinfo)
        {
            var companyCode = userinfo.company_code;
            var branchCode = userinfo.branch_code;
            DateTime fromDate = Convert.ToDateTime(filters.FromDate);
            DateTime toDate = Convert.ToDateTime(filters.ToDate);
            var toDatePlusOne = toDate.AddDays(1);
            string query = string.Format(@"SELECT *
                                            FROM AUDIT_SA_IP_TRANSACTION
                                            WHERE COMPANY_CODE = '{0}'
                                              AND BRANCH_CODE = '{1}'
                                              AND CREATED_DATE >= TO_DATE('{2}', 'YYYY-MM-DD')
                                              AND CREATED_DATE <  TO_DATE('{3}', 'YYYY-MM-DD')
                                            ORDER BY CREATED_DATE DESC",
                                                    companyCode,
                                                    branchCode,
                                                    fromDate.ToString("yyyy-MM-dd"),
                                                    toDatePlusOne.ToString("yyyy-MM-dd")
            );
            var purchase = _objectEntity.SqlQuery<AuditTransactionModel>(query).ToList();
            return purchase;
        }
        public dynamic GetGroupstockSummaryReport(dynamic data, User userinfo)
        {
            string method = string.IsNullOrWhiteSpace((string)data?.valuationMethod)
                ? "FIFO" : (string)data.valuationMethod;
            string multiUnits = data?.multiUnit;
            string purchaseOption = data?.purchaseOption;
            string salesOption = data?.salesOption;

            var filters = data?.filters;

            var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
            var companyCode = companyFilter != null && companyFilter.Any()
                ? string.Join(",", companyFilter.Select(c => $"'{c}'"))
                : $"'{userinfo.company_code}'";

            var branchFilter = filters?.BranchFilter as IEnumerable<object>;
            var branchCode = branchFilter != null && branchFilter.Any()
                ? string.Join(",", branchFilter.Select(b => $"'{b}'"))
                : $"'{userinfo.branch_code}'";

            string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
            string ToDate = data?.toADdate.ToString("yyyy-MM-dd");
            // 1. Unify the source of data
            IEnumerable<string> locations = null;
            if (data?.multiLocation is IEnumerable<object> list && list.Any())
                locations = list.Select(l => l?.ToString()).Where(s => !string.IsNullOrEmpty(s));
            else if (data?.multiLocation is string str && !string.IsNullOrWhiteSpace(str))
                locations = str.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s));
            string locCondBoth = "";  
            string locCondFrom = "";  
            string locCondTo = "";    

            if (locations != null && locations.Any())
            {
                string formatted = string.Join(",", locations.Select(loc => $"'{loc}'"));

                locCondBoth = $" AND (A.FROM_LOCATION_CODE IN ({formatted}) OR A.TO_LOCATION_CODE IN ({formatted})) ";
                locCondFrom = $" AND A.FROM_LOCATION_CODE IN ({formatted}) ";
                locCondTo = $" AND A.TO_LOCATION_CODE IN ({formatted}) ";
            }


            // ── Item filter ────────────────────────────────────────────────────────
            // TWO filter types from the frontend (matching desktop app strategy):
            //   selectedMasterCodes = MASTER_ITEM_CODEs of G-type group nodes selected
            //                         → generates LIKE 'code%' to capture all children
            //   selectedItems       = ITEM_CODEs of I-type leaf nodes selected
            //                         → generates direct IN ('id1','id2',...)

            var masterCodesRaw = data?.selectedMasterCodes as Newtonsoft.Json.Linq.JArray;
            List<string> masterCodesList = masterCodesRaw != null
                ? masterCodesRaw.Select(t => t.ToString()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
                : new List<string>();

            var selectedItemsRaw = data?.selectedItems as Newtonsoft.Json.Linq.JArray;
            List<string> selectedItemsList = selectedItemsRaw != null
                ? selectedItemsRaw.Select(t => t.ToString()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
                : new List<string>();

            // Category filter via MASTER_ITEM_CODE LIKE pattern (legacy/direct)
            string masterItemCodePattern = (string)data?.masterItemCode;

            string innerItemCodeFilter;
            string outerItemCodeFilter;

            if (masterCodesList.Count > 0 || selectedItemsList.Count > 0)
            {
                // Build the combined filter parts
                var innerParts = new List<string>();
                var outerParts = new List<string>();

                // G-type groups: use LIKE 'MASTER_ITEM_CODE%' to capture all children at any depth
                foreach (var mc in masterCodesList)
                {
                    // Escape any single quotes in the code for safety
                    var safeMc = mc.Replace("'", "''");
                    innerParts.Add(
                        $"A.ITEM_CODE IN (SELECT ITEM_CODE FROM IP_ITEM_MASTER_SETUP " +
                        $"WHERE COMPANY_CODE IN ({companyCode}) AND DELETED_FLAG = 'N' " +
                        $"AND MASTER_ITEM_CODE LIKE '{safeMc}%')");
                    outerParts.Add($"B.MASTER_ITEM_CODE LIKE '{safeMc}%'");
                }

                //// I-type leaf items: direct ITEM_CODE IN (...)
                //if (selectedItemsList.Count > 0)
                //{
                //    string itemList = string.Join(",", selectedItemsList.Select(i => $"'{i}'"));
                //    innerParts.Add($"A.ITEM_CODE IN ({itemList})");
                //    outerParts.Add($"B.ITEM_CODE IN ({itemList})");
                //}

                // Combine with OR (an item matches if caught by any LIKE or any IN)
                innerItemCodeFilter = $"AND ({string.Join(" OR ", innerParts)})";
                outerItemCodeFilter = $"AND ({string.Join(" OR ", outerParts)})";
            }
            else if (!string.IsNullOrEmpty(masterItemCodePattern))
            {
                // Legacy: category-level filter via a single MASTER_ITEM_CODE LIKE pattern
                innerItemCodeFilter =
                    $"AND A.ITEM_CODE IN (SELECT ITEM_CODE FROM IP_ITEM_MASTER_SETUP " +
                    $"WHERE COMPANY_CODE IN ({companyCode}) AND DELETED_FLAG = 'N' " +
                    $"AND MASTER_ITEM_CODE LIKE '{masterItemCodePattern}')";
                outerItemCodeFilter = $"AND (B.MASTER_ITEM_CODE LIKE '{masterItemCodePattern}')";
            }
            else
            {
                // No filter — return all items for the company
                innerItemCodeFilter = "";
                outerItemCodeFilter = "";
            }

                        string query = string.Format(@"SELECT b.ITEM_CODE, b.ITEM_EDESC, b.PRODUCT_CODE, b.INDEX_MU_CODE, b.MASTER_ITEM_CODE, b.PRE_ITEM_CODE, B.GROUP_SKU_FLAG, B.SERVICE_ITEM_FLAG, (LENGTH(B.MASTER_ITEM_CODE) - LENGTH(REPLACE(B.MASTER_ITEM_CODE,'.',''))) ROWLEV,   
                           OPEN_QTY, OPEN_AMT, PURCHASE_QTY, PURCHASE_AMT, PURCHASE_RET_QTY, PURCHASE_RET_AMT,  
                           SALES_QTY, SALES_AMT, SALES_RET_QTY, SALES_RET_AMT, SALES_NET_AMT, SI_AMT, SR_AMT, SRE_AMT, DN_AMT, CN_AMT,  
                           STK_REC_QTY, STK_REC_AMT, STK_TRANS_QTY, STK_TRANS_AMT, ST_TRNS_QTY, ST_TRNS_AMT, GOODS_ISS_QTY, GOODS_ISS_AMT, CIR_QTY, CIR_AMT, PRO_ISS_QTY, PRO_ISS_AMT, PRO_REC_QTY, PRO_REC_AMT,  
                           EXP_QTY, EXP_AMT, DA_EXP_QTY, DA_EXP_AMT, SAM_QTY, SAM_AMT, QC_QTY, QC_AMT,
                           ROUND(OPEN_QTY + PURCHASE_QTY - PURCHASE_RET_QTY - SALES_QTY + SALES_RET_QTY + STK_REC_QTY - STK_TRANS_QTY - ST_TRNS_QTY - PRO_ISS_QTY + PRO_REC_QTY - GOODS_ISS_QTY - DA_EXP_QTY - SAM_QTY - QC_QTY + CIR_QTY,3) * DECODE(SERVICE_ITEM_FLAG,'Y',0,1) CLOSING_QTY,  
                           ROUND(OPEN_AMT + PURCHASE_AMT - PURCHASE_RET_AMT - SALES_AMT + SALES_RET_AMT + STK_REC_AMT - STK_TRANS_AMT - ST_TRNS_AMT - PRO_ISS_AMT + PRO_REC_AMT - GOODS_ISS_AMT - DA_EXP_AMT - SAM_AMT - QC_AMT + CIR_AMT,2) * DECODE(SERVICE_ITEM_FLAG,'Y',0,1) CLOSING_AMT  
                           FROM (SELECT ITEM_CODE,  
                           NVL(OPENING_IQ, 0) - NVL(OPENING_OQ, 0) OPEN_QTY,  
                           ROUND(NVL(OPENING_TIP, 0) - NVL(OPENING_TOP, 0), 2) OPEN_AMT,  
                           NVL(PURCHASE_IQ, 0) - NVL(PURCHASE_OQ, 0) PURCHASE_QTY,  
                           ROUND(NVL(PURCHASE_TIP, 0) - NVL(PURCHASE_TOP, 0), 2) PURCHASE_AMT,  
                           NVL(PURCHASE_RET_OQ, 0) - NVL(PURCHASE_RET_IQ, 0) PURCHASE_RET_QTY,  
                           ROUND(NVL(PURCHASE_RET_TOP, 0) - NVL(PURCHASE_RET_TIP, 0), 2) PURCHASE_RET_AMT,  
                           NVL(SALES_OQ, 0) - NVL(SALES_IQ, 0) SALES_QTY,  
                           ROUND(NVL(SALES_TOP, 0) - NVL(SALES_TIP, 0), 2) SALES_AMT,  
                           NVL(SALES_RETURN_IQ, 0) - NVL(SALES_RETURN_OQ, 0) SALES_RET_QTY,  
                           ROUND(NVL(SALES_RETURN_TIP, 0) - NVL(SALES_RETURN_TOP, 0), 2) SALES_RET_AMT,  
                           NVL(STK_REC_IQ, 0) - NVL(STK_REC_OQ, 0) STK_REC_QTY,  
                           ROUND(NVL(STK_REC_TIP, 0) - NVL(STK_REC_TOP, 0), 2) STK_REC_AMT,  
                           NVL(STK_TRANS_OQ, 0) - NVL(STK_TRANS_IQ, 0) STK_TRANS_QTY,  
                           ROUND(NVL(STK_TRANS_TOP, 0) - NVL(STK_TRANS_TIP, 0), 2) STK_TRANS_AMT,  
                           NVL(ST_TRNS_OQ, 0) - NVL(ST_TRNS_IQ, 0) ST_TRNS_QTY,  
                           ROUND(NVL(ST_TRNS_TOP, 0) - NVL(ST_TRNS_TIP, 0), 2) ST_TRNS_AMT,  
                           NVL(GOODS_ISS_OQ, 0) + NVL(GOODS_ISS_C_OQ, 0) - NVL(GOODS_ISS_IQ, 0) GOODS_ISS_QTY,  
                           ROUND(NVL(GOODS_ISS_TOP, 0) + NVL(GOODS_ISS_C_TOP, 0) - NVL(GOODS_ISS_TIP, 0), 2) GOODS_ISS_AMT,  
                           NVL(CIR_IQ, 0) - NVL(CIR_OQ, 0) CIR_QTY,  
                           ROUND(NVL(CIR_TIP, 0) - NVL(CIR_TOP, 0), 2) CIR_AMT,  
                           NVL(PRO_ISS_OQ, 0) - NVL(PRO_ISS_IQ, 0) PRO_ISS_QTY,  
                           ROUND(NVL(PRO_ISS_TOP, 0) - NVL(PRO_ISS_TIP, 0), 2) PRO_ISS_AMT,  
                           NVL(PRO_REC_IQ, 0) - NVL(PRO_REC_OQ, 0) PRO_REC_QTY,  
                           ROUND(NVL(PRO_REC_TIP, 0) - NVL(PRO_REC_TOP, 0), 2) PRO_REC_AMT,  
                           ROUND(NVL(SALES_TNP, 0), 2) SALES_NET_AMT,  
                           ROUND(NVL(SI_TOP, 0)- NVL(SI_TIP, 0), 2) SI_AMT,  
                           ROUND(NVL(SR_TIP, 0)- NVL(SR_TOP, 0), 2) SR_AMT,  
                           ROUND(NVL(SRE_TIP, 0)- NVL(SRE_TOP, 0), 2) SRE_AMT,  
                           ROUND(NVL(D_NOTE_TIP, 0)- NVL(D_NOTE_TOP, 0), 2) DN_AMT,  
                           ROUND(NVL(C_NOTE_TIP, 0)- NVL(C_NOTE_TOP, 0), 2) CN_AMT,  
                           NVL(DA_EXP_OQ, 0) - NVL(DA_EXP_IQ, 0) DA_EXP_QTY,  
                           ROUND(NVL(DA_EXP_TOP, 0) - NVL(DA_EXP_TIP, 0), 2) DA_EXP_AMT,  
                           NVL(SAM_OQ, 0) - NVL(SAM_IQ, 0) SAM_QTY,  
                           ROUND(NVL(SAM_TOP, 0) - NVL(SAM_TIP, 0), 2) SAM_AMT,  
                           NVL(QC_OQ, 0) - NVL(QC_IQ, 0) QC_QTY,  
                           ROUND(NVL(QC_TOP, 0) - NVL(QC_TIP, 0), 2) QC_AMT,  
                           NVL(S_EXP_IQ, 0) - NVL(S_EXP_OQ, 0) EXP_QTY,  
                           ROUND(NVL(S_EXP_TIP, 0) - NVL(S_EXP_TOP, 0), 2) EXP_AMT  
                           FROM (  
                           SELECT PD.* FROM (  
                           SELECT * FROM (  
                           SELECT A.ITEM_CODE,
                           IN_QUANTITY, IN_QUANTITY * IN_UNIT_PRICE TOTALINPRICE, OUT_QUANTITY, OUT_QUANTITY * OUT_UNIT_PRICE TOTALOUTPRICE, 0 TOTALNETPRICE, SOURCE SOURCE_TABLE  
                           FROM V_VALUE_STOCK_LEDGER A, IP_ITEM_MASTER_SETUP B  
                           WHERE VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                           AND METHOD = 'FIFO'  
                           AND A.ITEM_CODE = B.ITEM_CODE AND A.COMPANY_CODE = B.COMPANY_CODE AND A.MU_CODE = B.INDEX_MU_CODE
                           AND A.COMPANY_CODE IN ({2}) AND A.BRANCH_CODE IN ({3})
                           {4}
                           {8}
                           UNION ALL  
                           SELECT A.ITEM_CODE, QUANTITY IN_QUANTITY, ROUND((QUANTITY * CALC_UNIT_PRICE) * EXCHANGE_RATE,2) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'IP_PURCHASE_INVOICE' SOURCE_TABLE   
                           FROM IP_PURCHASE_INVOICE A  
                           WHERE INVOICE_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND INVOICE_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
                           {7}
                           UNION ALL  
                           SELECT A.ITEM_CODE, 0 IN_QUANTITY, 0 TOTALINPRICE, QUANTITY OUT_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_SALES_INVOICE_V' SOURCE_TABLE   
                           FROM SA_SALES_INVOICE A  
                           WHERE SALES_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND SALES_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
	                          {6}
                           UNION ALL  
                           SELECT A.ITEM_CODE, 0 IN_QUANTITY, (NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_DEBIT_NOTE' SOURCE_TABLE   
                           FROM SA_DEBIT_NOTE A  
                           WHERE VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
	                          {6}
                           UNION ALL  
                           SELECT A.ITEM_CODE, 0 IN_QUANTITY, (NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_CREDIT_NOTE' SOURCE_TABLE   
                           FROM SA_CREDIT_NOTE A  
                           WHERE VOUCHER_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
	                          {6}
                           UNION ALL  
                           SELECT A.ITEM_CODE, QUANTITY IN_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'SALES_RETURN' SOURCE_TABLE  
                           FROM SA_SALES_RETURN A  
                           WHERE RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                           AND (SALES_TYPE_CODE IS NULL OR (SALES_TYPE_CODE, COMPANY_CODE) IN (SELECT SALES_TYPE_CODE, COMPANY_CODE FROM SA_SALES_TYPE WHERE EXP_FLAG = 'N'))  
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
	                          {7}
                           UNION ALL  
                           SELECT A.ITEM_CODE, QUANTITY IN_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'RET_EXP' SOURCE_TABLE  
                           FROM SA_SALES_RETURN A  
                           WHERE RETURN_DATE >= TO_DATE('{0}', 'YYYY-MM-DD') AND RETURN_DATE <= TO_DATE('{1}', 'YYYY-MM-DD')
                           AND (SALES_TYPE_CODE, COMPANY_CODE) IN (SELECT SALES_TYPE_CODE, COMPANY_CODE FROM SA_SALES_TYPE WHERE EXP_FLAG = 'Y')  
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
	                          {7}
                           UNION ALL  
                           SELECT b.ITEM_CODE,   
                           OPEN_QTY + PURCHASE_QTY - PURCHASE_RET_QTY - SALES_QTY + SALES_RET_QTY + STK_REC_QTY - STK_TRANS_QTY - ST_TRNS_QTY - PRO_ISS_QTY + PRO_REC_QTY - GOODS_ISS_QTY - DA_EXP_QTY - SAM_QTY - QC_QTY + CIR_QTY IN_QUANTITY,  
                           OPEN_AMT + PURCHASE_AMT - PURCHASE_RET_AMT - SALES_AMT + SALES_RET_AMT + STK_REC_AMT - STK_TRANS_AMT - ST_TRNS_AMT - PRO_ISS_AMT + PRO_REC_AMT - GOODS_ISS_AMT - DA_EXP_AMT - SAM_AMT - QC_AMT + CIR_AMT - DN_AMT + CN_AMT TOTALINPRICE,  
                           0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'OPENING_BALANCE' SOURCE_TABLE  
                           FROM (SELECT ITEM_CODE,  
                           NVL(OPENING_IQ, 0) - NVL(OPENING_OQ, 0) OPEN_QTY,  
                           ROUND(NVL(OPENING_TIP, 0) - NVL(OPENING_TOP, 0), 2) OPEN_AMT,  
                           NVL(PURCHASE_IQ, 0) - NVL(PURCHASE_OQ, 0) PURCHASE_QTY,  
                           ROUND(NVL(PURCHASE_TIP, 0) - NVL(PURCHASE_TOP, 0), 2) PURCHASE_AMT,  
                           NVL(PURCHASE_RET_OQ, 0) - NVL(PURCHASE_RET_IQ, 0) PURCHASE_RET_QTY,  
                           ROUND(NVL(PURCHASE_RET_TOP, 0) - NVL(PURCHASE_RET_TIP, 0), 2) PURCHASE_RET_AMT,  
                           NVL(SALES_OQ, 0) - NVL(SALES_IQ, 0) SALES_QTY,  
                           ROUND(NVL(SALES_TOP, 0)- NVL(SALES_TIP, 0), 2) SALES_AMT,  
                           NVL(SALES_RETURN_IQ, 0) - NVL(SALES_RETURN_OQ, 0) SALES_RET_QTY,  
                           ROUND(NVL(SALES_RETURN_TIP, 0) - NVL(SALES_RETURN_TOP, 0), 2) SALES_RET_AMT,  
                           NVL(STK_REC_IQ, 0) - NVL(STK_REC_OQ, 0) STK_REC_QTY,  
                           ROUND(NVL(STK_REC_TIP, 0) - NVL(STK_REC_TOP, 0), 2) STK_REC_AMT,  
                           NVL(STK_TRANS_OQ, 0) - NVL(STK_TRANS_IQ, 0) STK_TRANS_QTY,  
                           ROUND(NVL(STK_TRANS_TOP, 0) - NVL(STK_TRANS_TIP, 0), 2) STK_TRANS_AMT,  
                           NVL(ST_TRNS_OQ, 0) - NVL(ST_TRNS_IQ, 0) ST_TRNS_QTY,  
                           ROUND(NVL(ST_TRNS_TOP, 0)- NVL(ST_TRNS_TIP, 0), 2) ST_TRNS_AMT,  
                           NVL(GOODS_ISS_OQ, 0) + NVL(GOODS_ISS_C_OQ, 0) - NVL(GOODS_ISS_IQ, 0) GOODS_ISS_QTY,  
                           ROUND(NVL(GOODS_ISS_TOP, 0) + NVL(GOODS_ISS_C_TOP, 0) - NVL(GOODS_ISS_TIP, 0), 2) GOODS_ISS_AMT,  
                           NVL(CIR_IQ, 0) - NVL(CIR_OQ, 0) CIR_QTY,  
                           ROUND(NVL(CIR_TIP, 0)- NVL(CIR_TOP, 0), 2) CIR_AMT,  
                           NVL(PRO_ISS_OQ, 0) - NVL(PRO_ISS_IQ, 0) PRO_ISS_QTY,  
                           ROUND(NVL(PRO_ISS_TOP, 0)- NVL(PRO_ISS_TIP, 0), 2) PRO_ISS_AMT,  
                           NVL(PRO_REC_IQ, 0) - NVL(PRO_REC_OQ, 0) PRO_REC_QTY,  
                           ROUND(NVL(PRO_REC_TIP, 0) - NVL(PRO_REC_TOP, 0), 2) PRO_REC_AMT,  
                           ROUND(NVL(SALES_TNP, 0), 2) SALES_NET_AMT,  
                           ROUND(NVL(SI_TOP, 0)- NVL(SI_TIP, 0), 2) SI_AMT,  
                           ROUND(NVL(SR_TIP, 0)- NVL(SR_TOP, 0), 2) SR_AMT,  
                           ROUND(NVL(SRE_TIP, 0)- NVL(SRE_TOP, 0), 2) SRE_AMT,  
                           ROUND(NVL(D_NOTE_TIP, 0)- NVL(D_NOTE_TOP, 0), 2) DN_AMT,  
                           ROUND(NVL(C_NOTE_TIP, 0)- NVL(C_NOTE_TOP, 0), 2) CN_AMT,  
                           NVL(DA_EXP_OQ, 0) - NVL(DA_EXP_IQ, 0) DA_EXP_QTY,  
                           ROUND(NVL(DA_EXP_TOP, 0) - NVL(DA_EXP_TIP, 0), 2) DA_EXP_AMT,  
                           NVL(SAM_OQ, 0) - NVL(SAM_IQ, 0) SAM_QTY,  
                           ROUND(NVL(SAM_TOP, 0) - NVL(SAM_TIP, 0), 2) SAM_AMT,  
                           NVL(QC_OQ, 0) - NVL(QC_IQ, 0) QC_QTY,  
                           ROUND(NVL(QC_TOP, 0) - NVL(QC_TIP, 0), 2) QC_AMT,  
                           NVL(S_EXP_IQ, 0) - NVL(S_EXP_OQ, 0) EXP_QTY,  
                           ROUND(NVL(S_EXP_TIP, 0) - NVL(S_EXP_TOP, 0), 2) EXP_AMT  
                           FROM (  
                           SELECT PD.* FROM (  
                           SELECT * FROM (  
                           SELECT A.ITEM_CODE,
                           IN_QUANTITY, IN_QUANTITY * IN_UNIT_PRICE TOTALINPRICE, OUT_QUANTITY, OUT_QUANTITY * OUT_UNIT_PRICE TOTALOUTPRICE, 0 TOTALNETPRICE, SOURCE SOURCE_TABLE  
                           FROM V_VALUE_STOCK_LEDGER A, IP_ITEM_MASTER_SETUP B   
                           WHERE VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD')
                           AND METHOD = 'FIFO'  
                           AND A.ITEM_CODE = B.ITEM_CODE AND A.COMPANY_CODE = B.COMPANY_CODE AND A.MU_CODE = B.INDEX_MU_CODE
                           AND A.COMPANY_CODE IN ({2}) AND A.BRANCH_CODE IN ({3})
                           {4}
	                        {8}
                           UNION ALL  
                           SELECT A.ITEM_CODE, QUANTITY IN_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'SALES_RETURN' SOURCE_TABLE  
                           FROM SA_SALES_RETURN A  
                           WHERE RETURN_DATE < TO_DATE('{0}', 'YYYY-MM-DD')
                           AND (SALES_TYPE_CODE IS NULL OR (SALES_TYPE_CODE, COMPANY_CODE) IN (SELECT SALES_TYPE_CODE, COMPANY_CODE FROM SA_SALES_TYPE WHERE EXP_FLAG = 'N'))  
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
	                          {7}
                           UNION ALL  
                           SELECT A.ITEM_CODE, QUANTITY IN_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'RET_EXP' SOURCE_TABLE  
                           FROM SA_SALES_RETURN A  
                           WHERE RETURN_DATE < TO_DATE('{0}', 'YYYY-MM-DD')
                           AND (SALES_TYPE_CODE, COMPANY_CODE) IN (SELECT SALES_TYPE_CODE, COMPANY_CODE FROM SA_SALES_TYPE WHERE EXP_FLAG = 'Y')  
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
	                          {7}
                           UNION ALL  
                           SELECT A.ITEM_CODE, 0 IN_QUANTITY, 0 TOTALINPRICE, QUANTITY OUT_QUANTITY, CALC_TOTAL_PRICE - NVL(BD_AMT,0) TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_SALES_INVOICE_V' SOURCE_TABLE  
                           FROM SA_SALES_INVOICE A  
                           WHERE SALES_DATE < TO_DATE('{0}', 'YYYY-MM-DD')
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
	                          {6}
                           UNION ALL  
                           SELECT A.ITEM_CODE, 0 IN_QUANTITY, TOTAL_PRICE TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_DEBIT_NOTE' SOURCE_TABLE  
                           FROM SA_DEBIT_NOTE A  
                           WHERE VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD')
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
	                          {6}
                           UNION ALL  
                           SELECT A.ITEM_CODE, 0 IN_QUANTITY, TOTAL_PRICE TOTALINPRICE, 0 OUT_QUANTITY, 0 TOTALOUTPRICE, 0 TOTALNETPRICE, 'SA_CREDIT_NOTE' SOURCE_TABLE  
                           FROM SA_CREDIT_NOTE A  
                           WHERE VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD')
                           AND COMPANY_CODE IN ({2}) AND BRANCH_CODE IN ({3}) AND DELETED_FLAG = 'N'
                           {4}
	                          {6}
                           )  
                           PIVOT (  
                           SUM(IN_QUANTITY) AS IQ, SUM(TOTALINPRICE) AS TIP, SUM(OUT_QUANTITY) AS OQ, SUM(TOTALOUTPRICE) AS TOP, SUM(TOTALNETPRICE) AS TNP  
                           FOR (SOURCE_TABLE)  
                           IN('OPENING_BALANCE' AS OPENING, 'IP_PURCHASE_MRR' AS PURCHASE, 'IP_PURCHASE_RETURN' AS PURCHASE_RET, 'SA_SALES_INVOICE' AS SALES, 'SA_SALES_RETURN' AS SALES_RETURN, 'SA_SALES_INVOICE_V' AS SI, 'SALES_RETURN' AS SR, 'RET_EXP' AS SRE,  
                           'IP_ADVICE_MRR' AS STK_REC, 'IP_ADVICE_ISSUE' AS STK_TRANS, 'IP_TRANSFER_ISSUE' AS ST_TRNS, 'IP_GOODS_ISSUE_CHARGE' AS GOODS_ISS_C, 'IP_GOODS_ISSUE' AS GOODS_ISS, 'IP_GOODS_ISSUE_RETURN' AS CIR, 'IP_PRODUCTION_ISSUE' AS PRO_ISS, 'IP_PRODUCTION_MRR' AS PRO_REC, 'SA_DEBIT_NOTE' AS D_NOTE, 'SA_CREDIT_NOTE' AS C_NOTE, 'SA_SALES_RETURN_EXP' AS S_EXP, 'IP_DAMAGE_ISSUE' AS DA_EXP, 'IP_SAMPLE_ISSUE' SAM, 'IP_QC_ISSUE' QC   
                           )  
                           )) PD  
                           )) aa, IP_ITEM_MASTER_SETUP B  
                           WHERE B.ITEM_CODE = aa.ITEM_CODE (+)
                           AND B.COMPANY_CODE IN ({2}) AND B.DELETED_FLAG = 'N'
                           {5}
                           )  
                           PIVOT (  
                           SUM(IN_QUANTITY) AS IQ, SUM(TOTALINPRICE) AS TIP, SUM(OUT_QUANTITY) AS OQ, SUM(TOTALOUTPRICE) AS TOP, SUM(TOTALNETPRICE) AS TNP  
                           FOR (SOURCE_TABLE)  
                           IN('OPENING_BALANCE' AS OPENING, 'IP_PURCHASE_MRR' AS PURCHASE, 'IP_PURCHASE_RETURN' AS PURCHASE_RET, 'SA_SALES_INVOICE' AS SALES, 'SA_SALES_RETURN' AS SALES_RETURN, 'SA_SALES_INVOICE_V' AS SI, 'SALES_RETURN' AS SR, 'RET_EXP' AS SRE,  
                           'IP_ADVICE_MRR' AS STK_REC, 'IP_ADVICE_ISSUE' AS STK_TRANS, 'IP_TRANSFER_ISSUE' AS ST_TRNS, 'IP_GOODS_ISSUE_CHARGE' AS GOODS_ISS_C, 'IP_GOODS_ISSUE' AS GOODS_ISS, 'IP_GOODS_ISSUE_RETURN' AS CIR, 'IP_PRODUCTION_ISSUE' AS PRO_ISS, 'IP_PRODUCTION_MRR' AS PRO_REC, 'SA_DEBIT_NOTE' AS D_NOTE, 'SA_CREDIT_NOTE' AS C_NOTE, 'SA_SALES_RETURN_EXP' AS S_EXP, 'IP_DAMAGE_ISSUE' AS DA_EXP, 'IP_SAMPLE_ISSUE' SAM, 'IP_QC_ISSUE' QC    
                           )  
                           )) PD  
                           )) aa, IP_ITEM_MASTER_SETUP B  
                           WHERE B.ITEM_CODE = aa.ITEM_CODE (+)
                           AND B.COMPANY_CODE IN ({2}) AND B.DELETED_FLAG = 'N'
                           {5}
                           ORDER BY b.MASTER_ITEM_CODE, b.PRE_ITEM_CODE, b.ITEM_EDESC",
                              FromDate, ToDate, companyCode, branchCode,
                              innerItemCodeFilter, outerItemCodeFilter, locCondFrom, locCondTo, locCondBoth);

                                    var monthwise = _objectEntity.SqlQuery<GroupwiseStockSummary>(query).ToList();


            var groupedData = monthwise;

            // PRE_ITEM_CODE references the MASTER_ITEM_CODE of the parent row (not ITEM_CODE).
            // Build a set of all MASTER_ITEM_CODEs present in this result so we can detect
            // which rows have no visible parent → those become root nodes (PRE_ITEM_CODE = null).
            // Kendo TreeList requires NULL (not "") to identify root nodes.
            var existingMasterCodes = new HashSet<string>(
                groupedData.Select(r => r.MASTER_ITEM_CODE?.ToString() ?? ""),
                StringComparer.OrdinalIgnoreCase);

            foreach (var item in groupedData)
            {
                string parent = item.PRE_ITEM_CODE?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(parent) || !existingMasterCodes.Contains(parent))
                {
                    item.PRE_ITEM_CODE = null;   // null = root node for Kendo TreeList
                }
            }
            var itemRows = groupedData.Where(r => r.GROUP_SKU_FLAG == "I").ToList();

            List<dynamic> stockData = new List<dynamic>();

            foreach (var row in groupedData)
            {
                double total1 = 0;  // OPEN_QTY
                double total2 = 0;  // OPEN_AMT

                double total3 = 0;  // PURCHASE_QTY
                double total4 = 0;  // PURCHASE_AMT
                double total5 = 0;  // PURCHASE_RET_QTY
                double total6 = 0;  // PURCHASE_RET_AMT

                double total7 = 0;  // SALES_QTY
                double total8 = 0;  // SALES_AMT
                double total9 = 0;  // SALES_RET_QTY
                double total10 = 0; // SALES_RET_AMT
                double total11 = 0; // SALES_NET_AMT

                double total12 = 0; // SI_AMT
                double total13 = 0; // SR_AMT
                double total14 = 0; // SRE_AMT
                double total15 = 0; // DN_AMT
                double total16 = 0; // CN_AMT

                double total17 = 0; // STK_REC_QTY
                double total18 = 0; // STK_REC_AMT

                double total19 = 0; // STK_TRANS_QTY
                double total20 = 0; // STK_TRANS_AMT
                double total21 = 0; // ST_TRNS_QTY
                double total22 = 0; // ST_TRNS_AMT

                double total23 = 0; // GOODS_ISS_QTY
                double total24 = 0; // GOODS_ISS_AMT

                double total25 = 0; // CIR_QTY
                double total26 = 0; // CIR_AMT

                double total27 = 0; // PRO_ISS_QTY
                double total28 = 0; // PRO_ISS_AMT

                double total29 = 0; // PRO_REC_QTY
                double total30 = 0; // PRO_REC_AMT

                double total31 = 0; // EXP_QTY
                double total32 = 0; // EXP_AMT

                double total33 = 0; // DA_EXP_QTY
                double total34 = 0; // DA_EXP_AMT

                double total35 = 0; // SAM_QTY
                double total36 = 0; // SAM_AMT

                double total37 = 0; // QC_QTY
                double total38 = 0; // QC_AMT

                double total39 = 0; // CLOSING_QTY
                double total40 = 0; // CLOSING_AMT
                if (row.GROUP_SKU_FLAG == "G")
                {
                    string masterCode = row.MASTER_ITEM_CODE;
                    foreach (var z in itemRows)
                    {
                        if (!string.IsNullOrEmpty(z.MASTER_ITEM_CODE) &&
                            z.MASTER_ITEM_CODE.StartsWith(masterCode))
                        {
                            total1 += z.OPEN_QTY ?? 0;
                            total2 += z.OPEN_AMT ?? 0;

                            total3 += z.PURCHASE_QTY ?? 0;
                            total4 += z.PURCHASE_AMT ?? 0;
                            total5 += z.PURCHASE_RET_QTY ?? 0;
                            total6 += z.PURCHASE_RET_AMT ?? 0;

                            total7 += z.SALES_QTY ?? 0;
                            total8 += z.SALES_AMT ?? 0;
                            total9 += z.SALES_RET_QTY ?? 0;
                            total10 += z.SALES_RET_AMT ?? 0;
                            total11 += z.SALES_NET_AMT ?? 0;

                            total12 += z.SI_AMT ?? 0;
                            total13 += z.SR_AMT ?? 0;
                            total14 += z.SRE_AMT ?? 0;
                            total15 += z.DN_AMT ?? 0;
                            total16 += z.CN_AMT ?? 0;

                            total17 += z.STK_REC_QTY ?? 0;
                            total18 += z.STK_REC_AMT ?? 0;

                            total19 += z.STK_TRANS_QTY ?? 0;
                            total20 += z.STK_TRANS_AMT ?? 0;
                            total21 += z.ST_TRNS_QTY ?? 0;
                            total22 += z.ST_TRNS_AMT ?? 0;

                            total23 += z.GOODS_ISS_QTY ?? 0;
                            total24 += z.GOODS_ISS_AMT ?? 0;

                            total25 += z.CIR_QTY ?? 0;
                            total26 += z.CIR_AMT ?? 0;

                            total27 += z.PRO_ISS_QTY ?? 0;
                            total28 += z.PRO_ISS_AMT ?? 0;

                            total29 += z.PRO_REC_QTY ?? 0;
                            total30 += z.PRO_REC_AMT ?? 0;

                            total31 += z.EXP_QTY ?? 0;
                            total32 += z.EXP_AMT ?? 0;

                            total33 += z.DA_EXP_QTY ?? 0;
                            total34 += z.DA_EXP_AMT ?? 0;

                            total35 += z.SAM_QTY ?? 0;
                            total36 += z.SAM_AMT ?? 0;

                            total37 += z.QC_QTY ?? 0;
                            total38 += z.QC_AMT ?? 0;

                            total39 += z.CLOSING_QTY ?? 0;
                            total40 += z.CLOSING_AMT ?? 0;

                        }
                    }
                }
                else
                {
                    total1 = row.OPEN_QTY ?? 0;
                    total2 = row.OPEN_AMT ?? 0;

                    total3 = row.PURCHASE_QTY ?? 0;
                    total4 = row.PURCHASE_AMT ?? 0;
                    total5 = row.PURCHASE_RET_QTY ?? 0;
                    total6 = row.PURCHASE_RET_AMT ?? 0;

                    total7 = row.SALES_QTY ?? 0;
                    total8 = row.SALES_AMT ?? 0;
                    total9 = row.SALES_RET_QTY ?? 0;
                    total10 = row.SALES_RET_AMT ?? 0;
                    total11 = row.SALES_NET_AMT ?? 0;

                    total12 = row.SI_AMT ?? 0;
                    total13 = row.SR_AMT ?? 0;
                    total14 = row.SRE_AMT ?? 0;
                    total15 = row.DN_AMT ?? 0;
                    total16 = row.CN_AMT ?? 0;

                    total17 = row.STK_REC_QTY ?? 0;
                    total18 = row.STK_REC_AMT ?? 0;

                    total19 = row.STK_TRANS_QTY ?? 0;
                    total20 = row.STK_TRANS_AMT ?? 0;
                    total21 = row.ST_TRNS_QTY ?? 0;
                    total22 = row.ST_TRNS_AMT ?? 0;

                    total23 = row.GOODS_ISS_QTY ?? 0;
                    total24 = row.GOODS_ISS_AMT ?? 0;

                    total25 = row.CIR_QTY ?? 0;
                    total26 = row.CIR_AMT ?? 0;

                    total27 = row.PRO_ISS_QTY ?? 0;
                    total28 = row.PRO_ISS_AMT ?? 0;

                    total29 = row.PRO_REC_QTY ?? 0;
                    total30 = row.PRO_REC_AMT ?? 0;

                    total31 = row.EXP_QTY ?? 0;
                    total32 = row.EXP_AMT ?? 0;

                    total33 = row.DA_EXP_QTY ?? 0;
                    total34 = row.DA_EXP_AMT ?? 0;

                    total35 = row.SAM_QTY ?? 0;
                    total36 = row.SAM_AMT ?? 0;

                    total37 = row.QC_QTY ?? 0;
                    total38 = row.QC_AMT ?? 0;

                    total39 = row.CLOSING_QTY ?? 0;
                    total40 = row.CLOSING_AMT ?? 0;

                }
                var totals = new[]
                {
             total1, total2, total3, total4, total5, total6, total7, total8, total9, total10,
             total11, total12, total13, total14, total15, total16, total17, total18, total19, total20,
             total21, total22, total23, total24, total25, total26, total27, total28, total29, total30,
             total31, total32, total33, total34, total35, total36, total37, total38, total39, total40
         };
                if (totals.Any(t => t != 0))
                {
                    stockData.Add(new
                    {
                        ITEM_CODE = row.ITEM_CODE,
                        ITEM_EDESC = row.ITEM_EDESC,
                        INDEX_MU_CODE = row.INDEX_MU_CODE,
                        PRODUCT_CODE = row.PRODUCT_CODE,
                        MASTER_ITEM_CODE = row.MASTER_ITEM_CODE,
                        PRE_ITEM_CODE = row.PRE_ITEM_CODE == "00" ? " " : row.PRE_ITEM_CODE,

                        OPEN_QTY = total1,
                        OPEN_AMT = total2,

                        PURCHASE_QTY = total3,
                        PURCHASE_AMT = total4,
                        PURCHASE_RET_QTY = total5,
                        PURCHASE_RET_AMT = total6,

                        SALES_QTY = total7,
                        SALES_AMT = total8,
                        SALES_RET_QTY = total9,
                        SALES_RET_AMT = total10,
                        SALES_NET_AMT = total11,

                        SI_AMT = total12,
                        SR_AMT = total13,
                        SRE_AMT = total14,
                        DN_AMT = total15,
                        CN_AMT = total16,

                        STK_REC_QTY = total17,
                        STK_REC_AMT = total18,

                        STK_TRANS_QTY = total19,
                        STK_TRANS_AMT = total20,
                        ST_TRNS_QTY = total21,
                        ST_TRNS_AMT = total22,

                        GOODS_ISS_QTY = total23,
                        GOODS_ISS_AMT = total24,

                        CIR_QTY = total25,
                        CIR_AMT = total26,

                        PRO_ISS_QTY = total27,
                        PRO_ISS_AMT = total28,

                        PRO_REC_QTY = total29,
                        PRO_REC_AMT = total30,

                        EXP_QTY = total31,
                        EXP_AMT = total32,

                        DA_EXP_QTY = total33,
                        DA_EXP_AMT = total34,

                        SAM_QTY = total35,
                        SAM_AMT = total36,

                        QC_QTY = total37,
                        QC_AMT = total38,

                        CLOSING_QTY = total39,
                        CLOSING_AMT = total40
                    });
                }

            }
            //comment without Grand Total

            if (stockData.Any())
            {
                var grandTotal = new
                {
                    ITEM_CODE = "",
                    ITEM_EDESC = "Grand Total",
                    INDEX_MU_CODE = "",
                    PRODUCT_CODE = "",
                    MASTER_ITEM_CODE = "",
                    PRE_ITEM_CODE = "",

                    OPEN_QTY = itemRows.Sum(x => x.OPEN_QTY ?? 0),
                    OPEN_AMT = itemRows.Sum(x => x.OPEN_AMT ?? 0),

                    PURCHASE_QTY = itemRows.Sum(x => x.PURCHASE_QTY ?? 0),
                    PURCHASE_AMT = itemRows.Sum(x => x.PURCHASE_AMT ?? 0),
                    PURCHASE_RET_QTY = itemRows.Sum(x => x.PURCHASE_RET_QTY ?? 0),
                    PURCHASE_RET_AMT = itemRows.Sum(x => x.PURCHASE_RET_AMT ?? 0),

                    SALES_QTY = itemRows.Sum(x => x.SALES_QTY ?? 0),
                    SALES_AMT = itemRows.Sum(x => x.SALES_AMT ?? 0),
                    SALES_RET_QTY = itemRows.Sum(x => x.SALES_RET_QTY ?? 0),
                    SALES_RET_AMT = itemRows.Sum(x => x.SALES_RET_AMT ?? 0),
                    SALES_NET_AMT = itemRows.Sum(x => x.SALES_NET_AMT ?? 0),

                    SI_AMT = itemRows.Sum(x => x.SI_AMT ?? 0),
                    SR_AMT = itemRows.Sum(x => x.SR_AMT ?? 0),
                    SRE_AMT = itemRows.Sum(x => x.SRE_AMT ?? 0),
                    DN_AMT = itemRows.Sum(x => x.DN_AMT ?? 0),
                    CN_AMT = itemRows.Sum(x => x.CN_AMT ?? 0),

                    STK_REC_QTY = itemRows.Sum(x => x.STK_REC_QTY ?? 0),
                    STK_REC_AMT = itemRows.Sum(x => x.STK_REC_AMT ?? 0),

                    STK_TRANS_QTY = itemRows.Sum(x => x.STK_TRANS_QTY ?? 0),
                    STK_TRANS_AMT = itemRows.Sum(x => x.STK_TRANS_AMT ?? 0),
                    ST_TRNS_QTY = itemRows.Sum(x => x.ST_TRNS_QTY ?? 0),
                    ST_TRNS_AMT = itemRows.Sum(x => x.ST_TRNS_AMT ?? 0),

                    GOODS_ISS_QTY = itemRows.Sum(x => x.GOODS_ISS_QTY ?? 0),
                    GOODS_ISS_AMT = itemRows.Sum(x => x.GOODS_ISS_AMT ?? 0),

                    CIR_QTY = itemRows.Sum(x => x.CIR_QTY ?? 0),
                    CIR_AMT = itemRows.Sum(x => x.CIR_AMT ?? 0),

                    PRO_ISS_QTY = itemRows.Sum(x => x.PRO_ISS_QTY ?? 0),
                    PRO_ISS_AMT = itemRows.Sum(x => x.PRO_ISS_AMT ?? 0),

                    PRO_REC_QTY = itemRows.Sum(x => x.PRO_REC_QTY ?? 0),
                    PRO_REC_AMT = itemRows.Sum(x => x.PRO_REC_AMT ?? 0),

                    EXP_QTY = itemRows.Sum(x => x.EXP_QTY ?? 0),
                    EXP_AMT = itemRows.Sum(x => x.EXP_AMT ?? 0),

                    DA_EXP_QTY = itemRows.Sum(x => x.DA_EXP_QTY ?? 0),
                    DA_EXP_AMT = itemRows.Sum(x => x.DA_EXP_AMT ?? 0),

                    SAM_QTY = itemRows.Sum(x => x.SAM_QTY ?? 0),
                    SAM_AMT = itemRows.Sum(x => x.SAM_AMT ?? 0),

                    QC_QTY = itemRows.Sum(x => x.QC_QTY ?? 0),
                    QC_AMT = itemRows.Sum(x => x.QC_AMT ?? 0),

                    CLOSING_QTY = itemRows.Sum(x => x.CLOSING_QTY ?? 0),
                    CLOSING_AMT = itemRows.Sum(x => x.CLOSING_AMT ?? 0)
                };

                stockData.Add(grandTotal);
            }
            return stockData;
        }

        public List<AllItem> GetAllItemByGroup(ReportFiltersModel filters, User userinfo)
        {
            var branchCode = $"'{userinfo.branch_code}'";
            var companyCode = $"'{userinfo.company_code}'";
            string query = string.Format(@"SELECT DISTINCT 
                ITEM_CODE, 
                ITEM_EDESC,
                MASTER_ITEM_CODE,
                PRE_ITEM_CODE,
                GROUP_SKU_FLAG
             FROM IP_ITEM_MASTER_SETUP 
             WHERE  COMPANY_CODE IN ({0})
             AND BRANCH_CODE  IN ({1})
             AND DELETED_FLAG = 'N'
             ORDER BY MASTER_ITEM_CODE", companyCode, branchCode);
            var itemlist = _objectEntity.SqlQuery<AllItem>(query).ToList();
            return itemlist;
        }

        public List<AllLocation> GetAllLocation(ReportFiltersModel filters, User userinfo)
        {
            var branchCode = $"'{userinfo.branch_code}'";
            var companyCode = $"'{userinfo.company_code}'";
            string query = string.Format(@"SELECT DISTINCT 
                LOCATION_CODE, 
                LOCATION_EDESC,
                GROUP_SKU_FLAG
             FROM IP_LOCATION_SETUP 
             WHERE  COMPANY_CODE IN ({0})
             AND BRANCH_CODE  IN ({1})
             AND DELETED_FLAG = 'N'
             ORDER BY LOCATION_EDESC", companyCode, branchCode);
            var location = _objectEntity.SqlQuery<AllLocation>(query).ToList();
            return location;
        }




        //public dynamic GetProductCustomerwiseNetSalesReport(dynamic data, User userinfo)
        //{
        //    var filters = data?.filters;
        //    var companyFilter = filters?.CompanyFilter as IEnumerable<object>;
        //    var companyCode = companyFilter != null && companyFilter.Any()
        //        ? string.Join(",", companyFilter.Select(c => $"'{c.ToString()}'"))
        //        : $"'{userinfo.company_code}'";

        //    // Safely cast BranchFilter to List<string>
        //    var branchFilter = filters?.BranchFilter as IEnumerable<object>;
        //    var branchCode = branchFilter != null && branchFilter.Any()
        //        ? string.Join(",", branchFilter.Select(b => $"'{b.ToString()}'"))
        //        : $"'{userinfo.branch_code}'";

        //    string FromDate = data?.fromADdate.ToString("yyyy-MM-dd");
        //    string ToDate = data.toADdate.ToString("yyyy-MM-dd");
        //    string query = string.Format(@"SELECT CUSTOMER_EDESC, TPIN_VAT_NO, ITEM_EDESC, PRODUCT_CODE, INDEX_MU_CODE, (SELECT SUM(NVL(DR_AMOUNT,0) - NVL(CR_AMOUNT,0)) 
        //                                    FROM V$VIRTUAL_SUB_LEDGER WHERE SUB_CODE = 'C'||A.CUSTOMER_CODE AND DELETED_FLAG = 'N' AND ACC_CODE = A.ACC_CODE 
        //                                    AND COMPANY_CODE IN ({2}) 
        //                                    AND (VOUCHER_DATE < TO_DATE('{0}', 'YYYY-MM-DD') OR VOUCHER_NO = '0') ) OPENING ,
        //                                    (SELECT SUM(NVL(DR_AMOUNT,0) - NVL(CR_AMOUNT,0)) FROM V$VIRTUAL_SUB_LEDGER WHERE SUB_CODE = 'C'||A.CUSTOMER_CODE AND DELETED_FLAG = 'N' AND ACC_CODE = A.ACC_CODE 
        //                                    AND COMPANY_CODE IN ({2}) AND VOUCHER_DATE <= TO_DATE('{1}', 'YYYY-MM-DD') ) BALA, SALES_QTY, SALES_VALUE, SALES_RET_QTY, SALES_RET_VALUE, DEBIT_VALUE, CREDIT_VALUE, FREE_QTY, SALES_QTY - SALES_RET_QTY + FREE_QTY NET_SALES_QTY, 
        //                                    SALES_VALUE - SALES_RET_VALUE + DEBIT_VALUE - CREDIT_VALUE NET_SALES_VALUE FROM ( SELECT B.CUSTOMER_CODE, B.ACC_CODE, B.CUSTOMER_EDESC, B.TPIN_VAT_NO, D.ITEM_EDESC, D.PRODUCT_CODE, INDEX_MU_CODE INDEX_MU_CODE,  SUM(A.SALES_QTY) SALES_QTY,
        //                                      SUM(A.SALES_VALUE) SALES_VALUE, SUM(A.SALES_RET_QTY) SALES_RET_QTY, SUM(A.SALES_RET_VALUE) SALES_RET_VALUE, SUM(DEBIT_VALUE) DEBIT_VALUE, SUM(CREDIT_VALUE) CREDIT_VALUE, SUM(FREE_QTY) FREE_QTY  FROM ( SELECT A.CUSTOMER_CODE,  A.ITEM_CODE, SUM(QUANTITY) SALES_QTY,
        //                                      SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, 0 DEBIT_VALUE, 0 CREDIT_VALUE , NVL(SUM(NVL(A.FREE_QTY,0)),0) FREE_QTY FROM SA_SALES_INVOICE A WHERE A.DELETED_FLAG = 'N' 
        //                                    AND TRUNC(A.SALES_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
        //                                    AND TRUNC(A.SALES_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
        //                                    AND A.COMPANY_CODE IN ({2})
        //                                    AND A.BRANCH_CODE IN ({3}) 
        //                                    GROUP BY  A.CUSTOMER_CODE,  A.ITEM_CODE Union All SELECT A.CUSTOMER_CODE, A.ITEM_CODE, 0 SALES_QTY, 0 SALES_VALUE, SUM(QUANTITY)  SALES_RET_QTY,  SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) SALES_RET_VALUE, 
        //                                    0 DEBIT_VALUE, 0 CREDIT_VALUE, NVL(SUM(NVL(A.FREE_QTY * -1,0)),0) FREE_QTY  FROM SA_SALES_RETURN A WHERE A.DELETED_FLAG = 'N'
        //                                    AND TRUNC(A.RETURN_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
        //                                    AND TRUNC(A.RETURN_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
        //                                    AND A.COMPANY_CODE IN ({2})
        //                                    AND A.BRANCH_CODE IN ({3}) 
        //                                    GROUP BY  A.CUSTOMER_CODE, A.ITEM_CODE
        //                                    Union All  
        //                                    SELECT A.CUSTOMER_CODE,  A.ITEM_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY, 0 SALES_RET_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) DEBIT_VALUE, 0 CREDIT_VALUE, 0 FREE_QTY  FROM SA_DEBIT_NOTE A  
        //                                    WHERE A.DELETED_FLAG = 'N'  
        //                                    AND TRUNC(A.VOUCHER_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
        //                                    AND TRUNC(A.VOUCHER_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
        //                                    AND A.COMPANY_CODE IN ({2})
        //                                    AND A.BRANCH_CODE IN ({3})   
        //                                    GROUP BY A.CUSTOMER_CODE,  A.ITEM_CODE 
        //                                    Union All  
        //                                    SELECT A.CUSTOMER_CODE,  A.ITEM_CODE, 0 SALES_QTY, 0 SALES_VALUE, 0 SALES_RET_QTY,  0 SALES_RET_VALUE, 0 DEBIT_VALUE, SUM(NVL(A.QUANTITY*A.NET_GROSS_RATE,0)) CREDIT_VALUE, 0 FREE_QTY  FROM SA_CREDIT_NOTE A  
        //                                    WHERE A.DELETED_FLAG = 'N'  
        //                                    AND TRUNC(A.VOUCHER_DATE) >= TO_DATE('{0}', 'YYYY-MM-DD') 
        //                                    AND TRUNC(A.VOUCHER_DATE) <= TO_DATE('{1}', 'YYYY-MM-DD')                                                 
        //                                    AND A.COMPANY_CODE IN ({2})
        //                                    AND A.BRANCH_CODE IN ({3})   
        //                                    GROUP BY  A.CUSTOMER_CODE,  A.ITEM_CODE  
        //                                    ORDER BY CUSTOMER_CODE) A,  SA_CUSTOMER_SETUP B ,  IP_ITEM_MASTER_SETUP D WHERE A.CUSTOMER_CODE = B.CUSTOMER_CODE AND  A.ITEM_CODE = D.ITEM_CODE AND B.COMPANY_CODE = D.COMPANY_CODE AND D.COMPANY_CODE IN ({2})   
        //                                    GROUP BY   A.CUSTOMER_CODE,  A.ITEM_CODE, B.CUSTOMER_EDESC, B.TPIN_VAT_NO, D.ITEM_EDESC, D.INDEX_MU_CODE, B.CUSTOMER_CODE, B.ACC_CODE, D.PRODUCT_CODE  ) A ORDER BY ITEM_EDESC, CUSTOMER_EDESC", FromDate, ToDate, companyCode, branchCode);

        //    var monthwise = _objectEntity.SqlQuery<ProductGroupwiseNetSales>(query).ToList();
        //    var groupedData = monthwise;
        //    return groupedData;
        //}



        #endregion


    }
}
