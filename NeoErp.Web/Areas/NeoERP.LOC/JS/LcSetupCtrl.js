app.controller('LcSetupCtrl', function ($scope, $http, $filter, $q, LOCService) {
    $scope.phoneNumber = /^[0-9-+]+$/;
    $scope.showBeneficiary = false;
    $scope.GetPOfromERP = false;
    $scope.showsupplier = false;
    $scope.lcterms = function () {
        LOCService.getallTerms().then(function (d) {
            $scope.LcTermNames = d.data;

        }, function () { alert("Error at: Get All LC Terms") });
        function isEmpty(obj) {
            return Object.keys(obj).length === 0;
        }

    }
    $scope.AccountNumberExists = false;
    $scope.ApplicantIsEdit = false;
     //Lc Clearing Agent SETUP START
    $scope.LcClearingAgent = {
        CAGENT_CODE: "",
        CAGENT_EDESC: "",
        CAGENT_NDESC: "",
        CAGENT_ADDRESS: "",
        PHONE_NUMBER: ""
    };
    $scope.GetLcClearingAgentList = function () {
     
        LOCService.GetLcClearingAgentList().then(function (response) {
         
            $scope.LcClearingAgentData = response.data;
        });
    };  
    $scope.cancelLcClearingAgent = function () {
        $scope.LcClearingAgent = {
            CAGENT_CODE: "",
            CAGENT_EDESC: "",
            CAGENT_NDESC: "",
            CAGENT_ADDRESS: "",
            PHONE_NUMBER: ""
        };
    };

    $scope.addLcClearingAgent = function (isValid) {
     
        if (isValid) {
            var url = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateLcClearingAgent";
            var LC_ClearingAgentModel = {
                CAGENT_CODE: $scope.LcClearingAgent.CAGENT_CODE,
                CAGENT_EDESC: $scope.LcClearingAgent.CAGENT_EDESC,
                CAGENT_NDESC: $scope.LcClearingAgent.CAGENT_NDESC,
                CAGENT_ADDRESS: $scope.LcClearingAgent.CAGENT_ADDRESS,
                PHONE_NUMBER: $scope.LcClearingAgent.PHONE_NUMBER

                //INSERT FIELD VALUE
            }
            var response = $http({
                method: "post",
                url: url,
                data: JSON.stringify(LC_ClearingAgentModel),
                dataType: "json",
                async: false
            });
            return response.then(function (result) {
                if (result.status == "200") {
                    $scope.cancelLcClearingAgent();
                    $scope.GetLcClearingAgentList();
                    displayPopupNotification("Succesfully Saved Lc Clearing Agent.", "success");
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }

            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");
        }
    };

    $scope.getAllLcClearingAgent = function (items) {
     
        $scope.LcClearingAgent = {
            CAGENT_CODE: items.CAGENT_CODE,
            CAGENT_EDESC: items.CAGENT_EDESC,
            CAGENT_NDESC: items.CAGENT_NDESC,
            CAGENT_ADDRESS: items.CAGENT_ADDRESS,
            PHONE_NUMBER: items.PHONE_NUMBER
        };
    };
    $scope.deleteLcClearingAgent = function (CAGENT_CODE) {
     
        bootbox.confirm("Are you sure want to delete?", function (result) {
         
            if (result) {
                LOCService.deleteLcClearingAgent(CAGENT_CODE).then(function (data) {
                    displayPopupNotification("Succesfully Deleted.", "success");
                    $scope.GetLcClearingAgentList();
                }, function (e) {
                });
            }
        });
    };
    //end 

    //LC_PREFERENCE SETUP START
    $scope.GetPreferenceSetup = function () {
        var url = window.location.protocol + "//" + window.location.host + "/api/LCSetup/GetPreferenceSetup";

        var response = $http({
            method: "get",
            url: url,
        });
        return response.then(function (result) {
            debugger;
            if (result!=null) {
            $scope.showBeneficiary = result.data.Beneficiary === "true" ? true : false;
            $scope.GetPOfromERP = result.data.PO === "true" ? true : false;
            if ($scope.showBeneficiary) {
                $scope.showsupplier = true;
            } else
            {
                $scope.showsupplier = false;
            }
            }
         
        });

    };
    $scope.GetPreferenceSetup();

    $scope.SavePreference = function () {
        debugger;
        var Beneficiary= $scope.showBeneficiary;
        var GetPOfromERP = $scope.GetPOfromERP;
        var url = window.location.protocol + "//" + window.location.host + "/api/LCSetup/SavePreferenceSetup?Beneficiary=" + Beneficiary +"&GetPOfromERP="+GetPOfromERP;
      
        var response = $http({
            method: "post",
            url: url,
           });
        return response.then(function (result) {
            debugger;
            $scope.showBeneficiary = false;
            $scope.GetPOfromERP = false;
            $scope.showBeneficiary = result.data.Beneficiary === "true" ? true : false;
            $scope.GetPOfromERP = result.data.PO === "true" ? true : false;
            if ($scope.showBeneficiary) {
                $scope.showsupplier = true;
            } else {
                $scope.showsupplier = false;
            }
            displayPopupNotification("Succesfully Saved Lc Preference.", "success");
        });
    }
    //LC_PREFERENCE END

    //LC_END

    //LC_CONTRACTOR SETUP START
    $scope.LC_Contractor = {
        CONTRACTOR_CODE: "",
        CONTRACTOR_EDESC: "",
        CONTRACTOR_NDESC: "",
        CONTRACTOR_ADDRESS: "",
        PHONE_NUMBER: ""
    };
    $scope.GetLC_ContractorList = function () {
     
        LOCService.Getall_Lc_ContractorList().then(function (response) {
         
            $scope.LC_ContractorData = response.data;
        });

    };  
    $scope.cancelLcContractor = function () {
        $scope.LC_Contractor = {
            CONTRACTOR_CODE: "",
            CONTRACTOR_EDESC: "",
            CONTRACTOR_NDESC: "",
            CONTRACTOR_ADDRESS: "",
            PHONE_NUMBER: ""     
        };
    };
    $scope.addLcContractor = function (isValid) {
     
        if (isValid) {
            var url = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateLc_Contractor";
            var LC_ContractorModel = {
                CONTRACTOR_CODE: $scope.LC_Contractor.CONTRACTOR_CODE,
                CONTRACTOR_EDESC: $scope.LC_Contractor.CONTRACTOR_EDESC,
                CONTRACTOR_NDESC: $scope.LC_Contractor.CONTRACTOR_NDESC,
                CONTRACTOR_ADDRESS: $scope.LC_Contractor.CONTRACTOR_ADDRESS,
                PHONE_NUMBER: $scope.LC_Contractor.PHONE_NUMBER
              
                //INSERT FIELD VALUE
            }
            var response = $http({
                method: "post",
                url: url,
                data: JSON.stringify(LC_ContractorModel),
                dataType: "json",
                async: false
            });
            return response.then(function (result) {
                if (result.status == "200") {
                    $scope.cancelLcContractor();
                    $scope.GetLC_ContractorList();               
                    displayPopupNotification("Succesfully Saved Lc Contractor.", "success");
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }

            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");
        }
    };
    $scope.getLcContractors = function (items) {
     
        $scope.LC_Contractor = {
            CONTRACTOR_CODE: items.CONTRACTOR_CODE,
            CONTRACTOR_EDESC: items.CONTRACTOR_EDESC,
            CONTRACTOR_NDESC: items.CONTRACTOR_NDESC,
            CONTRACTOR_ADDRESS: items.CONTRACTOR_ADDRESS,
            PHONE_NUMBER: items.PHONE_NUMBER
        };
    };
    $scope.deleteLcContractors = function (CONTRACTOR_CODE) {
     
        bootbox.confirm("Are you sure want to delete?", function (result) {
         
            if (result) {
                LOCService.DeleteLC_Contractor(CONTRACTOR_CODE).then(function (data) {
                    displayPopupNotification("Succesfully Deleted.", "success");
                    $scope.GetLC_ContractorList();
                }, function (e) {
                });
            }
        });
    };

    //end

    //applicant setup start
    $scope.ApplicantBank = {
        BANK_CODE: "",
        BANK_NAME: "",
        BANK_ACC_NO: "",
        SWIFT_CODE: "",
        LINK_ACC_CODE: "",
        ADDRESS: "",
        BRANCH: "",
        PHONE_NO: "",
        REMARKS: ""
    };
    //drop down data fetching
    $scope.ApplicantBankDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/LcSetup/getApplicantBank",
            },

        },
    };
    //applicant drop downlist
    $scope.ApplicantBankList = {
        optionLabel: "--Select Bank--",
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        dataSource: $scope.ApplicantBankDatasource,

        select: function (e) {
         
            var dataItem = this.dataItem(e.item);
            $scope.ApplicantBank.BANK_NAME = dataItem.ACC_EDESC;
            $scope.ApplicantBank.LINK_ACC_CODE = dataItem.ACC_CODE;
            $scope.BankAccountExists();
        },
        dataBound: function () {

        }
    };
    $scope.GetApplicantBankList = function () {
     
        LOCService.GetallApplicantBankList().then(function (response) {
         
            $scope.ApplicantBankData = response.data;
        });

    }
    $scope.cancelapplicantbank = function () {
        $scope.ApplicantBank = {
            BANK_CODE: "",
            BANK_NAME: "",
            BANK_ACC_NO: "",
            SWIFT_CODE: "",
            LINK_ACC_CODE: "",
            ADDRESS: "",
            BRANCH: "",
            PHONE_NO: "",
            REMARKS: ""
        };
        $scope.AccountNumberExists = false;
        $scope.ApplicantIsEdit = false;
    };
    $scope.addapplicantbank = function (isValid) {
     
        if (isValid) {
            var url = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateLCApplicantBank";
            var ApplicantBankModel = {
                BANK_CODE: $scope.ApplicantBank.BANK_CODE,
                BANK_NAME: $scope.ApplicantBank.BANK_NAME,
                BANK_ACC_NO: $scope.ApplicantBank.BANK_ACC_NO,
                SWIFT_CODE: $scope.ApplicantBank.SWIFT_CODE,
                LINK_ACC_CODE: $scope.ApplicantBank.LINK_ACC_CODE,
                ADDRESS: $scope.ApplicantBank.ADDRESS,
                BRANCH: $scope.ApplicantBank.BRANCH,
                PHONE_NO: $scope.ApplicantBank.PHONE_NO,
                REMARKS: $scope.ApplicantBank.REMARKS
                //INSERT FIELD VALUE
            }
            var response = $http({
                method: "post",
                url: url,
                data: JSON.stringify(ApplicantBankModel),
                dataType: "json",
                async: false
            });
            return response.then(function (result) {
                if (result.status == "200") {
                    $scope.cancelapplicantbank();
                    $scope.GetApplicantBankList();
                    $scope.AccountNumberExists = false;
                    $scope.ApplicantIsEdit = false;
                    displayPopupNotification("Succesfully Saved Applicant Bank.", "success");
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }

            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");
        }
    };
    $scope.getApplicantBanks = function (items) {
     
        $scope.ApplicantBank = {
            BANK_CODE: items.BANK_CODE,
            BANK_NAME: items.BANK_NAME,
            BANK_ACC_NO: items.BANK_ACC_NO,
            SWIFT_CODE: items.SWIFT_CODE,
            LINK_ACC_CODE: items.LINK_ACC_CODE,
            ADDRESS: items.ADDRESS,
            BRANCH: items.BRANCH,
            PHONE_NO: items.PHONE_NO,
            REMARKS: items.REMARKS
        };
        $scope.AccountNumberExists = false;
        $scope.ApplicantIsEdit = true;
    }
    $scope.deleteApplicantBanks = function (BANK_CODE) {
     
        bootbox.confirm("Are you sure want to delete?", function (result) {
         
            if (result) {
                LOCService.deleteApplicantBank(BANK_CODE).then(function (data) {
                    displayPopupNotification("Succesfully Deleted.", "success");
                    $scope.GetApplicantBankList();
                }, function (e) {
                });
            }
        });
    }
    $scope.BankAccountExists = function () {
     
        if ($scope.ApplicantIsEdit === false) {
            if ($scope.ApplicantBank.BANK_NAME != "" || $scope.ApplicantBank.BANK_NAME != undefined && $scope.ApplicantBank.BANK_ACC_NO != "") {
                var BANK_NAME = $scope.ApplicantBank.BANK_NAME;
                var BANK_ACC_NO = $scope.ApplicantBank.BANK_ACC_NO;
                LOCService.BankAccountNumberExists(BANK_NAME, BANK_ACC_NO).then(function (response) {
                 
                    if (response.data.MESSAGE === true) {
                        $scope.AccountNumberExists = true;
                    }
                    else {
                        $scope.AccountNumberExists = false;
                    }

                })
            }
        }

    }
    //end
    $scope.clearlocations = function () {
        $scope.locationCode = 0;
        $scope.locationId = "";
        $scope.locationName = "";
        $scope.maxStoringDays = "";
        $scope.perDayCharge = "";
        $scope.currencyCode = "";
    }
    $scope.cleardocumentinfo = function () {
        $scope.documentCode = 0;
        $scope.documentName = "";
        $scope.remarks = "";
    }
    $scope.lcpterms = function () {
        LOCService.getallPTerms().then(function (d) {

            $scope.LcPTermNames = d.data;

        }, function () { alert("Error at: Get All LC Terms") });
        function isEmpty(obj) {
            return Object.keys(obj).length === 0;
        }
    }
    $scope.lcstatus = function () {
        LOCService.getallStatus().then(function (d) {
            $scope.LcStatusNames = d.data;

        }, function () { alert("Error at: Get All LC Status") });
        function isEmpty(obj) {
            return Object.keys(obj).length === 0;
        }
    }
    $scope.lcbeneficiary = function () {

        LOCService.getallBeneficiary().then(function (d) {

            $scope.LcBeneficiaryNames = d.data;
        }, function () {
            alert("Error at: Get All LC Beneficiary")
        });
        function isEmpty(obj) {
            return Object.keys(obj).length == 0;
        }
    };
    $scope.currencyDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/PurchaseOrder/getCurrency",
            },

        },
    };
    $scope.currencylist = {
        optionLabel: "--Select Currency--",
        dataTextField: "CURRENCY_CODE",
        dataValueField: "CURRENCY_CODE",
        dataSource: $scope.currencyDatasource,

    };
    $scope.lchs = function () {

        LOCService.getallhs().then(function (d) {

            $scope.LcHsNames = d.data;
        }, function () {
            alert("Error at: Get All LC Beneficiary")
        });
        function isEmpty(obj) {
            return Object.keys(obj).length == 0;
        }
    };
    $scope.lcSupplierBank = function () {

        LOCService.getallSupplierBanks().then(function (d) {

            $scope.lcSupplierBanks = d.data;
        }, function () {
            alert("Error at: Get All LC Supplier Banks")
        });
        function isEmpty(obj) {
            return Object.keys(obj).length == 0;
        }
    };
    $scope.lcLocationList = function () {

        LOCService.getallLocationList().then(function (d) {

            $scope.lcLocationLists = d.data;
        }, function () {
            alert("Error at: Get All LC Supplier Banks")
        });
        function isEmpty(obj) {
            return Object.keys(obj).length == 0;
        }
    };
    $scope.lcDocumentLists = function () {

        LOCService.getallDocumentList().then(function (d) {

            $scope.lcDocumentList = d.data;
        }, function () {
            alert("Error at: Get All LC Supplier Banks")
        });
        function isEmpty(obj) {
            return Object.keys(obj).length == 0;
        }
    };
    $scope.lcic = function () {
     
        LOCService.getallic().then(function (d) {

            $scope.LcicNames = d.data;
        }, function () {
            alert("Error at: Get All Ic Beneficiary")
        });
        function isEmpty(obj) {
            return Object.keys(obj).length == 0;
        }
    };
    $scope.lccontainer = function () {
     
        LOCService.getallcontainer().then(function (d) {

            $scope.LccNames = d.data;
        }, function () {
            alert("Error at: Get All Ic Beneficiary")
        });
        function isEmpty(obj) {
            return Object.keys(obj).length == 0;
        }
    };

    $scope.lcterms();
    $scope.lcpterms();
    $scope.lcstatus();
    $scope.lcbeneficiary();
    $scope.lchs();
    $scope.lcic();
    $scope.lccontainer();
    $scope.lcSupplierBank();
    $scope.lcLocationList();
    $scope.lcDocumentLists();
    $scope.GetApplicantBankList();
    $scope.GetLC_ContractorList();
    $scope.GetLcClearingAgentList();
    //Start Add LC Setups //

    $scope.addlcterms = function (isValid) {

        if (isValid) {
            var loadlcterm = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateLCTerm";

            var lcterm = {
                TermName: $scope.termname,
                TermCode: $scope.lctermcode
            };
            var response = $http({
                method: "post",
                url: loadlcterm,
                data: JSON.stringify(lcterm),
                dataType: "json"
            });

            return response.then(function (data) {

                if (data.data.MESSAGE == "Success") {
                    $scope.termname = "";
                    $scope.lctermcode = 0;
                    displayPopupNotification("Succesfully Saved LC Terms.", "success");
                    $scope.lcterms();
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }
            });

        }
        else {
            displayPopupNotification("Fields Cannot be Empty.", "error");
        }

    };
    $scope.addlcpterms = function (isValid) {

        if (isValid) {
            var loadlcpterm = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateLCPTerm";

            var lcPterm = {
                PTermName: $scope.ptermname,
                PTermCode: $scope.lcptermcode
            };
            var response = $http({
                method: "post",
                url: loadlcpterm,
                data: JSON.stringify(lcPterm),
                dataType: "json"
            });

            return response.then(function (data) {

                if (data.data.MESSAGE == "Success") {
                    $scope.ptermname = "";
                    $scope.lcptermcode = 0;
                    displayPopupNotification("Succesfully Saved LC Payment Terms.", "success");
                    $scope.lcpterms();
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }
            });

        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");
        }

    };
    $scope.addlcstatus = function (isValid) {

        if (isValid) {
            var loadlcstatus = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateLCStatus";

            var lcstatus = {
                StatusName: $scope.lcstatusname,
                StatusCode: $scope.lcstatuscode
            };
            var response = $http({
                method: "post",
                url: loadlcstatus,
                data: JSON.stringify(lcstatus),
                dataType: "json",
                async: false
            });

            return response.then(function (data) {
                if (data.data.MESSAGE == "Success") {
                    $scope.lcstatusname = "";
                    $scope.lcstatuscode = 0;
                    displayPopupNotification("Succesfully Saved LC Status.", "success");
                    $scope.lcstatus();
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }
            });

        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");
        }

    };
    $scope.addlcbeneficiary = function (isValid) {
        debugger;
        if (isValid) {

            var loadlcbeneficiary = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateLCBeneficiary";

            var lcbeneficiary = {
                BNF_CODE: $scope.BnfCode,
                BNF_EDESC: $scope.lcbeneficiaryname,
                ADDRESS: $scope.lcbeneficiaryaddress,
                COUNTRY_CODE: $scope.lcCountryCode,
                REMARKS: $scope.lcbeneficiaryremarks,
                SUPPLIER_CODE:$scope.suppliercode
                //INSERT FIELD VALUE
            }
            var response = $http({
                method: "post",
                url: loadlcbeneficiary,
                data: JSON.stringify(lcbeneficiary),
                dataType: "json",
                async: false
            });
            return response.then(function (data) {
                debugger;
                if (data.data.MESSAGE == "Success") {
                    $scope.BnfCode = 0,
                        $scope.lcbeneficiaryname = "";
                    $scope.lcbeneficiaryaddress = "";
                    $scope.lcCountryCode = "";
                    $scope.lcbeneficiaryremarks = "";
                    $scope.suppliercode = "";
                    displayPopupNotification("Succesfully Saved beneficiary Status.", "success");
                    $("#lcCountryCode").data('kendoDropDownList').value($scope.lcCountryCode = "");
                    $("#lcCountryCode").val();
                    $("#suppliercode").data('kendoDropDownList').value($scope.suppliercode = "");
                    $("#suppliercode").val();
                    $scope.lcbeneficiary();
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }
            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");
        }
    };
    $scope.addhs = function (isValid) {

     
        if (isValid) {

            var loadhs = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateHs";

            var lchs = {
                HS_NO: $scope.hsno,
                HS_CODE: $scope.hscode,
                HS_NDESC: $scope.hsname,
                DUTY_RATE: $scope.dutyRate,
                REMARKS: $scope.hsremarks,
                //INSERT FIELD VALUE
            }
            var response = $http({
                method: "post",
                url: loadhs,
                data: JSON.stringify(lchs),
                dataType: "json",
                async: false
            });
            return response.then(function (result) {
             
                if (result.data.MESSAGE === "Success") {
                    displayPopupNotification("Succesfully Saved HS Status.", "success");
                    $scope.lchs();
                    $scope.cancelHs();
                }
                if (result.data.MESSAGE === "Exist") {
                    displayPopupNotification("HS Code Already Exist.", "warning");
                }
                if (result.data.MESSAGE === "Error") {
                    displayPopupNotification("Fields cannot be empty", "error");
                }

            },
                function errorCallback(response) {
                    displayPopupNotification(response.data.MESSAGE, "error");
                });
        }
       
    };
    $scope.addsupplierbank = function (isValid) {
        if (isValid) {
            var loadhs = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateLCBank";
            var bankdetails = {
                BANK_CODE: $scope.suppliersbankcode,
                BANK_NAME: $scope.bankname,
                ADDRESS: $scope.bankaddress,
                REMARKS: $scope.bankremarks,
                BANK_ACC_NO: $scope.bankaccno,
                SWIFT_CODE: $scope.swiftcode,
                BRANCH: $scope.bankbranch,
                PHONE_NO: $scope.phoneno,
                //INSERT FIELD VALUE
            }
            var response = $http({
                method: "post",
                url: loadhs,
                data: JSON.stringify(bankdetails),
                dataType: "json",
                async: false
            });
            return response.then(function (result) {

                if (result.status == "200") {
                    $scope.bankname = "";
                    $scope.bankaddress = "";
                    $scope.bankremarks = "";
                    $scope.swiftcode = "";
                    $scope.bankbranch = "";
                    $scope.phoneno = "";
                    $scope.bankaccno = "";
                    $scope.suppliersbankcode = 0;
                    displayPopupNotification("Succesfully Saved Suppliers Bank.", "success");
                    $scope.lcSupplierBank();
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }

            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");
        }
    };
    $scope.addLocation = function (isValid) {
     
        if (isValid) {
            var loadlocationurl = window.location.protocol + "//" + window.location.host + "/api/LCSetup/AddLocation";
            var locationdetails = {
                LOCATION_ID: $scope.locationId,
                LOCATION_CODE: $scope.locationCode,
                LOCATION_EDESC: $scope.locationName,
                MAX_STORING_DAYS: $scope.maxStoringDays,
                PER_DAY_CHARGE: $scope.perDayCharge,
                CURRENCY_CODE: $scope.currencyCode
                //INSERT FIELD VALUE
            }
            var response = $http({
                method: "post",
                url: loadlocationurl,
                data: JSON.stringify(locationdetails),
                dataType: "json",
                async: false
            });
            return response.then(function (result) {
             
                if (result.data.MESSAGE === "Success") {
                    $scope.lcLocationList();
                    $scope.clearlocations();
                    displayPopupNotification("Succesfully Saved Location.", "success");
                }
                else if (result.data.MESSAGE === "Exist") {
                    displayPopupNotification("Location Code Already Exits.", "warning");
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }

            });
        }
        else {
            displayPopupNotification("Error Occured.", "error");
        }
    };
    $scope.addDocumentinfo = function (isValid) {
        if (isValid) {
            var loadDocumenturl = window.location.protocol + "//" + window.location.host + "/api/LCSetup/AddDocumentInfo";
            var documentdetails = {
                DOCUMENT_CODE: $scope.documentCode,
                DOCUMENT_EDESC: $scope.documentName,
                REMARKS: $scope.remarks,
            }
            var response = $http({
                method: "post",
                url: loadDocumenturl,
                data: JSON.stringify(documentdetails),
                dataType: "json",
                async: false
            });
            return response.then(function (result) {

                if (result.status == "200") {
                    $scope.cleardocumentinfo();
                    displayPopupNotification("Succesfully Saved Document Info.", "success");
                    $scope.lcDocumentLists();
                }
                else {
                    displayPopupNotification("Error Occured.", "error");
                }

            });
        }
        else {
            displayPopupNotification("Error Occured.", "error");
        }
    };
    $scope.addic = function (isValid) {

     
        if (isValid) {
         
            var loadhs = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateIc";

            var lcic = {

                CARRIER_CODE: $scope.iccode,
                CARRIER_EDESC: $scope.icname,
                CARRIER_NDESC: $scope.icname,
                CARRIER_ADDRESS1: $scope.icaddress1,
                CARRIER_ADDRESS2: $scope.icaddress2,
                CARRIER_PHONE_NO: $scope.icphoneno,
                REMARKS: $scope.icremarks,
                //INSERT FIELD VALUE
            }
            var response = $http({
                method: "post",
                url: loadhs,
                data: JSON.stringify(lcic),
                dataType: "json",
                async: false
            });
            return response.then(function (result) {

                if (result.data.MESSAGE == "success") {
                    $scope.cancelIc();
                    displayPopupNotification("Succesfully Saved.", "success");
                    $scope.lcic();
                }
                else {

                    displayPopupNotification(result.data.MESSAGE, "error");
                }
            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");
        }
    };
    $scope.addcontainer = function (isValid) {

     
        if (isValid) {
         
            var loadcontainer = window.location.protocol + "//" + window.location.host + "/api/LCSetup/Createcontainer";

            var lcc = {

                CONTAINER_CODE: $scope.containercode,
                CONTAINER_EDESC: $scope.containername,
                CONTAINER_NDESC: $scope.containername,
                CONTAINER_NO: $scope.containerno,
                CONTAINER_SIZE: $scope.containersize,
                REMARKS: $scope.containerremarks,
                //INSERT FIELD VALUE
            }
            var response = $http({
                method: "post",
                url: loadcontainer,
                data: JSON.stringify(lcc),
                dataType: "json",
                async: false
            });
            return response.then(function (result) {
             
                if (result.data.MESSAGE == "success") {
                    $scope.cancelcontainer();
                    displayPopupNotification("Succesfully Saved.", "success");
                    $scope.lccontainer();
                }
                else {

                    displayPopupNotification(result.data.MESSAGE, "error");
                }
            });
        }
        else {
            displayPopupNotification("Fields cannot be empty.", "error");
        }
    };
    //End Add LC Setups //

    //Start Get LC Setups by Setups ID //

    $scope.getTermsByTermCode = function (termdetail) {
        $scope.termname = termdetail.TermName;
        $scope.lctermcode = termdetail.TermCode;
    }
    $scope.getSupplierBanks = function (bankdetail) {

        $scope.suppliersbankcode = bankdetail.BANK_CODE;
        $scope.bankname = bankdetail.BANK_NAME;
        $scope.bankaddress = bankdetail.ADDRESS;
        $scope.phoneno = bankdetail.PHONE_NO;
        $scope.bankbranch = bankdetail.BRANCH;
        $scope.swiftcode = bankdetail.SWIFT_CODE;
        $scope.bankaccno = bankdetail.BANK_ACC_NO;
    }
    $scope.getPTermsByPTermCode = function (ptermdetail) {
        $scope.ptermname = ptermdetail.PTermName;
        $scope.lcptermcode = ptermdetail.PTermCode;
    }
    $scope.getStatusByStatusCode = function (statusdetail) {
        $scope.lcstatusname = statusdetail.StatusName;
        $scope.lcstatuscode = statusdetail.StatusCode;
    }
    $scope.getBeneficiaryByBNFCode = function (bnfdetails) {
        $scope.BnfCode = bnfdetails.BNF_CODE;
        $scope.lcbeneficiaryname = bnfdetails.BNF_EDESC;
        $scope.lcbeneficiaryaddress = bnfdetails.ADDRESS;
        $("#lcCountryCode").data('kendoDropDownList').value(bnfdetails.COUNTRY_CODE);
        $("#suppliercode").data('kendoDropDownList').value(bnfdetails.SUPPLIER_CODE);
        $scope.suppliercode=bnfdetails.SUPPLIER_CODE;
        $scope.lcCountryCode = bnfdetails.COUNTRY_CODE;
        $scope.lcbeneficiaryremarks = bnfdetails.REMARKS;
        $scope.lcbeneficiaryform.lcCountryCode.$invalid = false;
    }
    $scope.getHsCodeByCode = function (hsdetails) {
        $scope.hscode = hsdetails.HS_CODE;
        $scope.hsname = hsdetails.HS_EDESC;
        $scope.dutyRate = hsdetails.DUTY_RATE;
        $scope.hsremarks = hsdetails.REMARKS;
        $scope.hsno = hsdetails.HS_NO;
        //$scope.hscompanycode = hsdetails.COMPANY_CODE;

    }
    $scope.getLocationByCode = function (locdetail) {
     
        $scope.locationCode = locdetail.LOCATION_CODE;
        $scope.locationId = locdetail.LOCATION_ID;
        $scope.locationName = locdetail.LOCATION_EDESC;
        $scope.maxStoringDays = locdetail.MAX_STORING_DAYS;
        $scope.perDayCharge = locdetail.PER_DAY_CHARGE;
        $scope.currencyCode = locdetail.CURRENCY_CODE;

    }
    $scope.getDocumentByCode = function (docdetails) {
        $scope.documentName = docdetails.DOCUMENT_EDESC;
        $scope.documentCode = docdetails.DOCUMENT_CODE;
        $scope.remarks = docdetails.REMARKS;
    }
    $scope.getIcCodeByCode = function (icdetails) {
        $scope.iccode = icdetails.CARRIER_CODE;
        $scope.icname = icdetails.CARRIER_EDESC;
        $scope.icaddress1 = icdetails.CARRIER_ADDRESS1;
        $scope.icaddress2 = icdetails.CARRIER_ADDRESS2;
        $scope.icphoneno = icdetails.CARRIER_PHONE_NO;
        $scope.icremarks = icdetails.REMARKS
    }
    $scope.getContainerCodeByCode = function (cdetails) {
        $scope.containercode = cdetails.CONTAINER_CODE;
        $scope.containername = cdetails.CONTAINER_EDESC;
        $scope.containerno = cdetails.CONTAINER_NO;
        $scope.containersize = cdetails.CONTAINER_SIZE;
        $scope.containerremarks = cdetails.REMARKS;

    }

    //End Get LC Setups by Setups ID //

    //Start Delete LC Setups //

    $scope.deleteByTermCode = function (TermCode) {
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result) {
                LOCService.deleteByTermCode(TermCode).then(function (data) {
                    displayPopupNotification("Succesfully Deleted.", "success");
                    $scope.lcterms();

                }, function (e) {

                });
            }
        });
    }
    $scope.deleteByPTermCode = function (PTermCode) {
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result) {
                LOCService.deleteByPTermCode(PTermCode).then(function (data) {
                    displayPopupNotification("Succesfully Deleted.", "success");
                    $scope.lcpterms();

                }, function (e) {

                });
            }
        });

    }
    $scope.deleteByStatusCode = function (StatusCode) {
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result) {
                LOCService.deleteByStatusCode(StatusCode).then(function (data) {
                    displayPopupNotification("Succesfully Deleted.", "success");
                    $scope.lcstatus();

                }, function (e) {

                });
            }
        });

    }
    $scope.deleteByBeneficiaryCode = function (BNF_CODE) {
        bootbox.confirm("Are you sure want to deleted ?", function (result) {
            if (result) {
                LOCService.deleteByBeneficiaryCode(BNF_CODE).then(function (data) {
                    displayPopupNotification("Succesfully Deleted.", "success");
                    $scope.lcbeneficiary();
                }, function (e) {
                });
            }
        });
    }
    $scope.deleteByHSCode = function (HS_CODE) {

        bootbox.confirm("Are you sure want to delete?", function (result) {
            if (result) {
                LOCService.deleteByHSCode(HS_CODE).then(function (data) {
                    displayPopupNotification("Successfully Deleted HS Codes.", "success");
                    $scope.lchs();
                }, function (e) { });
            }
        });
    }
    $scope.deleteSupplierBanks = function (BANK_CODE) {
        bootbox.confirm("Are you sure want to delete?", function (result) {
            if (result) {
                LOCService.deleteByBankCode(BANK_CODE).then(function (data) {
                    displayPopupNotification("Succesfully Deleted.", "success");
                    $scope.lcSupplierBank();
                }, function (e) {
                });
            }
        });
    }
    $scope.deleteLocationByCode = function (LOCATION_CODE) {
        bootbox.confirm("Are you sure want to delete?", function (result) {
            if (result) {
                LOCService.deleteByLocationCode(LOCATION_CODE).then(function (data) {
                    displayPopupNotification("Succesfully Deleted.", "success");
                    $scope.lcLocationList();
                }, function (e) {
                });
            }
        });
    }
    $scope.deleteDocumentByCode = function (DOCUMENT_CODE) {
        bootbox.confirm("Are you sure want to delete?", function (result) {
            if (result) {
                LOCService.deleteByDocumentCode(DOCUMENT_CODE).then(function (data) {
                    displayPopupNotification("Succesfully Deleted.", "success");
                    $scope.lcDocumentLists();
                }, function (e) {
                });
            }
        });
    }
    $scope.deleteByIcCode = function (CARRIER_CODE) {

        bootbox.confirm("Are you sure want to delete?", function (result) {
            if (result) {
                LOCService.deleteByIcCode(CARRIER_CODE).then(function (data) {
                    displayPopupNotification("Successfully Deleted.", "success");
                    $scope.lcic();
                }, function (e) { });
            }
        });
    }
    $scope.deleteByContainerCode = function (CONTAINER_CODE) {

        bootbox.confirm("Are you sure want to delete?", function (result) {
            if (result) {
                LOCService.deleteBycCode(CONTAINER_CODE).then(function (data) {
                    displayPopupNotification("Successfully Deleted.", "success");
                    $scope.lccontainer();
                }, function (e) { });
            }
        });
    }

    //End Delete LC Setups //

    //Start Cancel LC Setups //
    $scope.cancelterm = function () {
        $scope.termname = "";
        $scope.lctermcode = 0;
    }
    $scope.cancelpterm = function () {
        $scope.ptermname = "";
        $scope.lcptermcode = 0;
    }
    $scope.cancelstatus = function () {
        $scope.lcstatusname = "";
        $scope.lcstatuscode = 0;
    }
    $scope.cancelbeneficiary = function () {

        $scope.BnfCode = 0;
        $scope.lcbeneficiaryname = "";
        $scope.lcbeneficiaryremarks = "";
        $scope.lcbeneficiaryaddress = "";
        $("#lcCountryCode").data('kendoDropDownList').value($scope.lcCountryCode = "");
        $("#lcCountryCode").val();


    }
    $scope.cancelHs = function () {
        $scope.hscode = "";
        $scope.hsname = "";
        $scope.hsremarks = "";
        $scope.hsno = 0;
        $scope.dutyRate = "";
    }
    $scope.cancelsupplierbank = function () {
        $scope.bankname = "";
        $scope.bankaddress = "";
        $scope.bankremarks = "";
        $scope.swiftcode = "";
        $scope.bankbranch = "";
        $scope.phoneno = "";
        $scope.bankaccno = "";
        $scope.suppliersbankcode = 0
    }
    $scope.cancelIc = function () {
        $scope.iccode = 0;
        $scope.icname = "";
        $scope.icaddress1 = "";
        $scope.icaddress2 = "";
        $scope.icphoneno = "";
        $scope.icremarks = "";

    }
    $scope.cancelcontainer = function () {
        $scope.containercode = 0;
        $scope.containername = "";
        $scope.containerno = "";
        $scope.containersize = "";
        $scope.containerremarks = "";

    }
    //End Cancel LC Setups //
});

app.service('LOCService', function ($http) {
    var fac = {};
    fac.getallTerms = function () {
        return $http.get('/api/LCSetup/GetAllTerms');
    }
    fac.getallPTerms = function () {
        return $http.get('/api/LCSetup/GetAllPTerms');
    }
    fac.getallStatus = function () {

        return $http.get('/api/LCSetup/GetAllStatus');
    }
    fac.getallBeneficiary = function () {

        return $http.get('/api/LCSetup/GetAllBeneficiary');
    }
    fac.getallhs = function () {

        return $http.get('/api/LCSetup/Getallhs');
    }
    fac.getallSupplierBanks = function () {
        return $http.get('/api/LCSetup/GetAllBanks');
    }
    fac.getallLocationList = function () {
        return $http.get('/api/LCSetup/GetAllLocations');
    }
    fac.getallDocumentList = function () {
        return $http.get('/api/LCSetup/GetAllDocuments');
    }
    fac.getallic = function () {

        return $http.get('/api/LCSetup/Getallic');
    }
    fac.getallcontainer = function () {

        return $http.get('/api/LCSetup/Getallcontainer');
    }
    fac.deleteByBankCode = function (BankCode) {

        return $http({
            url: '/api/LCSetup/DeleteLCBanks',
            method: "GET",
            params: { BankCode: BankCode }
        });
    }
    fac.deleteByTermCode = function (TermCode) {

        return $http({
            url: '/api/LCSetup/DeleteLCTerms',
            method: "GET",
            params: { TermCode: TermCode }
        });
    }
    fac.deleteByPTermCode = function (PTermCode) {

        return $http({
            url: '/api/LCSetup/DeleteLCPTerm',
            method: "GET",
            params: { PTermCode: PTermCode }
        });
    }
    fac.deleteByStatusCode = function (StatusCode) {

        return $http({
            url: '/api/LCSetup/DeleteLCStatus',
            method: "GET",
            params: { StatusCode: StatusCode }
        });
    }
    fac.deleteByBeneficiaryCode = function (BNF_CODE) {

        return $http({
            url: '/api/LCSetup/DeleteLCBeneficiary',
            method: "GET",
            params: { BNF_CODE: BNF_CODE }
        });

    }
    fac.deleteByHSCode = function (HS_CODE) {

        return $http({
            url: '/api/LCSetup/DeleteHs',
            method: "GET",
            params: { HS_CODE: HS_CODE }
        });
    }
    fac.deleteByLocationCode = function (LOCATION_CODE) {

        return $http({
            url: '/api/LCSetup/DeleteLocation',
            method: "GET",
            params: { locationCode: LOCATION_CODE }
        });
    }
    fac.deleteByDocumentCode = function (DOCUMENT_CODE) {
        return $http({
            url: '/api/LCSetup/DeleteDocumentInfo',
            method: "GET",
            params: { documentCode: DOCUMENT_CODE }
        });
    }
    fac.deleteByIcCode = function (CARRIER_CODE) {

        return $http({
            url: '/api/LCSetup/DeleteIc',
            method: "GET",
            params: { CARRIER_CODE: CARRIER_CODE }
        });
    }
    fac.deleteBycCode = function (CONTAINER_CODE) {

        return $http({
            url: '/api/LCSetup/Deletecontainer',
            method: "GET",
            params: { CONTAINER_CODE: CONTAINER_CODE }
        });
    }

    fac.GetallApplicantBankList = function () {
        return $http({
            url: '/api/LCSetup/GetallApplicantBankList',
            method: "GET",

        })
    }
    fac.Getall_Lc_ContractorList = function () {
        return $http({
            url: '/api/LCSetup/getAll_LC_ContractorList',
            method: "GET",

        })
    }
    fac.DeleteLC_Contractor = function (CONTRACTOR_CODE) {
        return $http({
            url: '/api/LCSetup/DeleteLC_Contractor',
            method: "GET",
            params: { CONTRACTOR_CODE: CONTRACTOR_CODE }
        })
    }

    fac.GetLcClearingAgentList = function () {
        return $http({
            url: '/api/LCSetup/GetAllLcClearingAgentList',
            method: "GET",

        })
    }
    fac.deleteLcClearingAgent = function (CAGENT_CODE) {
        return $http({
            url: '/api/LCSetup/DeleteLcClearingAgent',
            method: "GET",
            params: { CAGENT_CODE: CAGENT_CODE }
        })
    }
    fac.deleteApplicantBank = function (BANK_CODE) {
        return $http({
            url: '/api/LCSetup/DeleteApplicantBank',
            method: "GET",
            params: { BANK_CODE: BANK_CODE }
        })
    }
    fac.BankAccountNumberExists = function (BANK_NAME, BANK_ACC_NO) {
        return $http({
            url: '/api/LCSetup/BankAccountExists',
            method: "GET",
            params: { BANK_NAME: BANK_NAME, BANK_ACC_NO: BANK_ACC_NO }
        })


    }

    return fac;

});
