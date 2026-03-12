using NeoErp.Core.Models;
using NeoErp.Models;
using NeoErp.Services;
using NeoErp.Services.UserService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace NeoErp.Controllers.Api
{
    public class FormSetupApiController : ApiController
    {

        private NeoErp.Core.Services.IFormSetup _formSetup;
        private IUserService _userService;
        public FormSetupApiController(NeoErp.Core.Services.IFormSetup formSetup, IUserService userService)
        {
            this._formSetup = formSetup;
            this._userService = userService;
        }

        [HttpGet]
        public HttpResponseMessage GetFormTreeStructureList(string moduleId)
        {
            try
            {
                var result = _formSetup.GetFormTreeStructureList(moduleId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Error occured while processing the request - " + ex.Message, TYPE = "error" });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetFormListByPreFormCode(string preFormCode, string moduleId = "04")
        {
            try
            {
                var result = _formSetup.GetFormListByPreFormCode(preFormCode, moduleId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetAllFormList()
        {
            try
            {
                var result = _formSetup.GetMasterFormList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetNextFormCode()
        {
            try
            {
                var result = _formSetup.GetNextFormCode();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetAllModuleList()
        {
            try
            {
                var result = _formSetup.GetAllModuleList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetAllBranchList(string form_code)
        {
            try
            {
                var result = _formSetup.GetAllBranchList(form_code);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpPost]
        public HttpResponseMessage InsertFormSetupGroupedItem([FromBody] NeoErp.Core.Models.FormSetupGroupEntryModel model)
        {
            try
            {
                bool? result = null;
                if (model.IS_EDIT)
                {
                    result = _formSetup.UpdateFormGroupItem(model);
                }
                else
                {
                    result = _formSetup.InsertFormGroupItem(model);
                }


                if (result != null && result == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        MESSAGE = model.IS_EDIT ? "Form item updated successfully" : "Form inserted successfully.",
                        TYPE = "success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        MESSAGE = "Failed to insert form record.",
                        TYPE = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }




        [HttpGet]
        public HttpResponseMessage GetInitialInformationTabInfo(int form_code, bool is_duplicate = false)
        {
            try
            {
                var result = _formSetup.GetForEditFormSetupData(form_code.ToString(), is_duplicate);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpPost]
        public HttpResponseMessage UpdateFormSetupGridItemInformationTabData([FromBody] FormSetupAddOrDuplicateStepModel model)
        {
            try
            {
                bool? result = null;

                result = _formSetup.UpdateFormSetupGridItem(model);


                if (result != null && result == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        MESSAGE = "Updated successfully",
                        TYPE = "success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        MESSAGE = "Failed to insert form record.",
                        TYPE = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetTransactionTableList()
        {
            try
            {
                var result = _formSetup.FormattingTab_GetTransactionTableList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }





        [HttpGet]
        public HttpResponseMessage GetSubLedgerList(string searchText = "")
        {
            try
            {
                var result = _formSetup.FormattingTab_GetSubLedgerList(searchText);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetFormDetailSetupList(string form_code)
        {
            try
            {
                var result = _formSetup.GetFormDetailSetupList(form_code);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetUnmappedColumnList(string form_code)
        {
            try
            {
                var result = _formSetup.FormattingTab_GetUnmappedColumnList(form_code);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while fetching table name list - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetDataForFormattingTab(string form_code)
        {
            try
            {
                var result = _formSetup.FormattingTab_GetEditData(form_code);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCustomerList()
        {
            try
            {
                var result = _formSetup.GetCustomersList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetItemMasterLookupList()
        {
            try
            {
                var result = _formSetup.GetItemMasterList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while fetching item master list - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpPost]
        public HttpResponseMessage UpdateFormatingTabInfo([FromBody] FormSetupFormattingTabModel model)
        {
            try
            {
                bool? result = null;

                result = _formSetup.UpdateFormSetupFormattingTabInfo(model);

                if (result != null && result == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        MESSAGE = "Updated successfully",
                        TYPE = "success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        MESSAGE = "Failed to insert form record.",
                        TYPE = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        #region Start For Formatting Tab.

        [HttpGet]
        public HttpResponseMessage GetEmployeeList()
        {
            try
            {
                var result = _formSetup.GetEmployeeList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetCurrencyList()
        {
            try
            {
                var result = _formSetup.GetCurrencyList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetCurrencyCodeDESCValue(string code)
        {
            try
            {
                var result = _formSetup.GetCurrencyDesc(code);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }



        [HttpGet]
        public HttpResponseMessage GetMUList()
        {
            try
            {
                var result = _formSetup.GetMUList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSalesTypeList()
        {
            try
            {
                var result = _formSetup.GetSalesTypeList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetPriorityList()
        {
            try
            {
                var result = _formSetup.GetPriorityList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetCustomerTreeStructureList()
        {
            try
            {
                var result = _formSetup.GetCustomerTreeStructureList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }





        [HttpGet]
        public HttpResponseMessage GetCustomerDetailListByPreCustCode(string preCustomerCode)
        {
            try
            {
                var result = _formSetup.GetCustomerListByPreCode(preCustomerCode);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetCustomerListBySearch(string searchText)
        {
            try
            {
                var result = _formSetup.GetCustomerListBySearch(searchText);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }



        [HttpGet]
        public HttpResponseMessage GetCustomerDetailByCustomerCode(string customerCode)
        {
            try
            {
                var result = _formSetup.GetCustomerDetailByCode(customerCode);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetItemGroupList()
        {
            try
            {
                var result = _formSetup.GetItemGroupList();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetItemDetailListByPreItemCode(string preItemCode)
        {
            try
            {
                var result = _formSetup.GetItemListByPreCode(preItemCode);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetItemDetailByItemCode(string itemCode)
        {
            try
            {
                var result = _formSetup.GetItemDetailByItemCode(itemCode);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetEmployeeTreeStructureList()
        {
            try
            {
                var result = _formSetup.GetEmployeeGroupedTreeData();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    SUCCESS = false
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetEmployeeListByPreCode(string preEmployeeCode)
        {
            try
            {
                var result = _formSetup.GetEmployeeListByPreCode(preEmployeeCode);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    SUCCESS = false
                });
            }
        }

        #endregion End region for Formatting Tab



        #region Start For Reference Tab.


        [HttpGet]
        public HttpResponseMessage GetFormSetupReferenceTabInfo(string form_code)
        {
            try
            {
                var result = _formSetup.GetReferenceTabData(form_code);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }



        [HttpGet]
        public HttpResponseMessage GetTransactionTableListWithoutVoucher()
        {
            try
            {
                var result = _formSetup.GetTransactionTableListWithoutVoucher();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetListBasedOnTableName(string table_name)
        {
            try
            {
                var result = _formSetup.GetQuotationFormList(table_name);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateReferenceTabInfo([FromBody] FormSetupReferenceTabInfoModel model)
        {
            try
            {

                var result = _formSetup.UpdateFormReferenceTabData(model);

                if (result != null && result == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        MESSAGE = "Updated successfully",
                        TYPE = "success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        MESSAGE = "Failed to insert form record.",
                        TYPE = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage GetBranchListData(string formCode)
        {
            try
            {
                var result = _formSetup.GetBranchListByFormCode(formCode);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }



        [HttpGet]
        public HttpResponseMessage GetInvoiceToBeMatchedList()
        {
            try
            {
                var result = _formSetup.GetFormListForInvoiceToBeMatched();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }



        [HttpGet]
        public HttpResponseMessage GetAccountList(string searchText)
        {
            try
            {
                var result = _formSetup.GetAccountList(searchText);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }

        #endregion End For Reference Tab.


        #region Start For Numbering Tab.

        [HttpGet]
        public HttpResponseMessage GetFormSetupNumberingTabInfo(string form_code)
        {
            try
            {
                var result = _formSetup.GetNumberingTabData(form_code);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }


        [HttpPost]
        public HttpResponseMessage UpdateNumberingTabInfo([FromBody] FormSetupNumberingTabInfoModel model)
        {
            try
            {
                var result = _formSetup.UpdateFormNumberingTabData(model);
                if (result != null && result == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        MESSAGE = "Updated successfully",
                        TYPE = "success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        MESSAGE = "Failed to insert form record.",
                        TYPE = "error"
                    });
                }
            }
            catch (Exception ex)
            {

                var exceptionType = ex.GetType().Name;

                if (exceptionType == "ValidationException")
                {
                    return Request.CreateResponse((HttpStatusCode)422, new
                    {
                        MESSAGE = ex.Message,
                        TYPE = "validation"
                    });
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }

        #endregion End For Reference Tab.



        #region Start For Charge Setup Tab.

        [HttpGet]
        public HttpResponseMessage ChargeSetupGetAccountList(string searchText)
        {
            try
            {
                var result = _formSetup.ChargeSetupGetAccountList(searchText);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }


        [HttpGet]
        public HttpResponseMessage ChargeSetupGetChargeList(string form_code)
        {
            try
            {
                var result = _formSetup.GetChargeCodeList(form_code);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetChargeSetup(string formCode, string chargeCode)
        {
            try
            {
                var result = _formSetup.GetChargeSetupData(formCode, chargeCode);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }


        [HttpPost]
        public HttpResponseMessage UpdateChargeSetupTabInfo([FromBody] ChargeSetupDataModel model)
        {
            try
            {
                bool? result = null;

                result = _formSetup.UpdateChargeSetupData(model);

                if (result != null && result == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        MESSAGE = "Updated successfully",
                        TYPE = "success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        MESSAGE = "Failed to insert form record.",
                        TYPE = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }



        #endregion End For Charge Setup Tab.



        [HttpGet]
        public HttpResponseMessage GetLoadQualityCheckTabData(string formCode)
        {
            try
            {
                var result = _formSetup.GetQualityCheckTabData(formCode);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }


        [HttpPost]
        public HttpResponseMessage UpdateQualityCheckData([FromBody] QualityCheckGetDataModel model)
        {
            try
            {
                bool? result = null;

                result = _formSetup.UpdateChargeSetupData(model);

                if (result != null && result == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        MESSAGE = "Updated successfully",
                        TYPE = "success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        MESSAGE = "Failed to insert form record.",
                        TYPE = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }

        public HttpResponseMessage GetMiscellaneousTabData(string formCode)
        {
            try
            {
                var result = _formSetup.GetMiscellaneousTabData(formCode);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }


        public HttpResponseMessage GetDocumentReportList(string formCode, string searchText)
        {
            try
            {
                var result = _formSetup.GetDocumentReportSetupData(formCode, searchText);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message
                });
            }
        }


        public HttpResponseMessage UpdateMiscellaneousTabData([FromBody] MiscellaneousTabDataModel model)
        {
            try
            {
                bool? result = null;

                result = _formSetup.UpdateMiscellaneousData(model);

                if (result != null && result == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        MESSAGE = "Updated successfully",
                        TYPE = "success"
                    });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new
                    {
                        MESSAGE = "Failed to insert form record.",
                        TYPE = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    MESSAGE = "Error occurred while processing the request - " + ex.Message,
                    TYPE = "error"
                });
            }
        }






    }
}
