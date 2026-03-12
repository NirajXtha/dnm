DTModule.controller('PostDateChequeCtrl', function ($scope, $filter, $window, $timeout, $http, $q) {
    $scope.FormName = "Post Date Cheque";
    $scope.FROM_DATE = "";
    $scope.TO_DATE = "";
    $scope.PDC_TYPE_FILTER = "";
    $scope.CHEQUE_TYPE_FILTER = "";
    $scope.activeTab = "receipt";
    $scope.PARTY_TYPE_EDESC = "";
    $scope.PARTY_TYPE_EDESC_PAYMENT = "";
    $scope.disableEncash = true;
    $scope.disableBounce = true;
    $scope.disableIntransit = true;
    $scope.disableOnEdit = false;
    $scope.transitMode = true;
    $scope.encashMode = false;
    $scope.INTRANSIT = false;
    $scope.BOUNCE = false;
    $scope.RETURN = false;
    $scope.ISINTRANSIT = true;
    $scope.ISBOUNCE = true;
    $scope.ISRETURN = true;
    $scope.editMode = false;
    $scope.PAYMENTEDITMODE = false;
    $scope.ISENCASH = false;
    $scope.ISACCON = false;
    $scope.ACCOUNTDISABLE = false;
    $scope.DISABLEVOUCHER = true;
    $scope._newReceiptNo = "";
    $scope._newPaymentNo = "";
    $scope.isChequeDuplicate = false;
    $scope.showPaymentAddButton = false;
    $scope.showReceiptAddButton = true;
    $scope.showPaymentVoucherButton = false;
    $scope.IsPaymentEncashed = true;
    $scope.IsPaymentReturned = true;
    $scope.RETURNPAYMENT = false;
    $scope.ConfirmACC = false;


    $scope.preferencesModel = {
        RECEIPT_ACC_CODE: "",
        PAYMENT_ACC_CODE: "",
        RECEIPT_FORM_CODE: "",
        PAYMENT_FORM_CODE: ""
    };

   

    $scope.pdcModel = {
        RECEIPT_NO: "",
        RECEIPT_DATE: "",
        CHEQUE_TYPE: "PDC",
        CHEQUE_DATE: "",
        CUSTOMER_CODE: "",
        PARTY_TYPE_CODE: "",
        PDC_AMOUNT: "",
        PDC_DETAILS: "",
        BANK_NAME: "",
        CHEQUE_NO: "",
        REMARKS: "",
        MR_ISSUED_BY: "",
        MR_NO:"",
        PRIOR_DAYS: "",
        ENCASH_DATE: "",
        DAY: "",
        ENCASH_REMARKS: "",
        CREATED_BY: "",
        CREATED_DATE: "",
        BOUNCE_BY: "",
        BOUNCE_DATE: "",
        INTRANSIT_BY: "",
        IN_TRANSIT_DATE: "",
        ACC_CODE: "",
        ENCASH_REMARKS: "",
        EMPLOYEE_CODE: "",
        INTRANSIT_FLAG: "",
        RETURN_DATE: "",
        RETURN_FLAG: "",
        CHEQUE_PATH: '',
        CHEQUE_PHOTO: ''    
    };





    $scope.pdcPaymentModel = {
        PAYMENT_NO: '',
        PAYMENT_DATE: null,
        CHEQUE_DATE: null,
        ENCASH_DATE: null,
        SUPPLIER_CODE: '',
        PDC_AMOUNT: '',
        PDC_DETAILS: '',
        BANK_NAME: '',
        REMARKS: '',
        BRANCH_CODE: '',
        COMPANY_CODE: '',
        CREATED_BY: '',
        CREATED_DATE: '',
        VG_DATE: null,
        MODIFY_DATE: null,
        SYN_ROWID: '',
        MANUAL_NO: '',
        MODIFY_BY: '',
        BOUNCE_FLAG: 'N',
        BOUNCE_DATE: null,
        BOUNCE_VC_NO: '',
        PRIOR_DAYS: 0,
        CHEQUE_NO: '',
        CHEQUE_PHOTO: '',
        CHEQUE_PATH :'' ,
        CHEQUE_TYPE:'PDC',  
        DELETED_FLAG :'',
        ACC_CODE:'',   
        BANK_ACC_CODE:'',
        RETURN_FLAG:'',  
        RETURN_DATE:'',  
        ENCASH_FLAG:'',  
        RETURN_REASON:''
    };




    $scope.inTransitDataModel = {
        INTRANSIT_FLAG: "",
        RECEIPT_NO: "",
        ACC_CODE:"",
    }

    $scope.enCashDataModel = {
        RECEIPT_NO: "",
        REMARKS: "",
        DAYS: "",
        ENCASH_DATE:""
    }


     $scope.bounceModel = {
        RECEIPT_NO: "",
        BOUNCE_REASON: "",
        BOUNCE_FLAG: ""
    }

    $scope.returnModel = {
        RECEIPT_NO: "",
        RETURN_FLAG: "",
        RETURN_DATE: ''
    }

    $scope.resetPdcModel = function () {
        $scope.resetFormValidation();

        $scope.pdcModel = {
            RECEIPT_NO: "",
            RECEIPT_DATE: "",
            CHEQUE_TYPE: "PDC",
            CHEQUE_DATE: "",
            CUSTOMER_CODE: "",
            PARTY_TYPE_CODE: "",
            PDC_AMOUNT: "",
            PDC_DETAILS: "",
            BANK_NAME: "",
            CHEQUE_NO: "",
            REMARKS: "",
            MR_ISSUED_BY: "",
            MR_NO: "",
            PRIOR_DAYS: "",
            ENCASH_DATE: "",
            DAY: "",
            ENCASH_REMARKS: "",
            CREATED_BY: "",
            CREATED_DATE: "",
            BOUNCE_BY: "",
            BOUNCE_DATE: "",
            INTRANSIT_BY: "",
            IN_TRANSIT_DATE: "",
            ACC_CODE: "",
            EMPLOYEE_CODE: "",
            INTRANSIT_FLAG: "",
            RETURN_DATE: "",
            RETURN_FLAG: "",
            CHEQUE_PHOTO: ""
        };

        $scope.inTransitDataModel = {
            INTRANSIT_FLAG: "",
            RECEIPT_NO: "",
            ACC_CODE: ""
        };

        $scope.enCashDataModel = {
            RECEIPT_NO: "",
            REMARKS: "",
            DAYS: "",
            ENCASH_DATE: ""
        };
        $scope.enCashPaymentDataModel = {
            PAYMENT_NO: "",
            ENCASH_DATE: ""
        };

        $scope.bounceModel = {
            RECEIPT_NO: "",
            BOUNCE_REASON: "",
            BOUNCE_FLAG: ""
        };

        $scope.returnModel = {
            RECEIPT_NO: "",
            RETURN_FLAG: "",
            RETURN_DATE: ""
        };
        $scope.returnPaymentModel = {
            RECEIPT_NO: "",
            RETURN_FLAG: "",
            RETURN_DATE: ""
        };

        $scope.PARTY_TYPE_EDESC = "";
        $scope.transitMode = false;
        $scope.ISINTRANSIT = false;
        $scope.editMode = false;
        $scope.disableOnEdit = false;
        $scope.INTRANSIT = false;
        $scope.ISINTRANSIT = false;
        $scope.BOUNCE = false;
        $scope.ISBOUNCE = false;
        $scope.ISACCON = false;
    };

    $scope.resetPreferenceForm = function () {
        $scope.preferencesModel = {
            RECEIPT_ACC_CODE: "",
            PAYMENT_ACC_CODE: "",
            RECEIPT_FORM_CODE: "",
            PAYMENT_FORM_CODE: ""
        };
    }

    $scope.resetPdcPaymentModel = function () {
        $scope.resetPaymentFormValidation();
        $scope.pdcPaymentModel = {
            PAYMENT_NO: '',
            PAYMENT_DATE: null,
            CHEQUE_DATE: null,
            ENCASH_DATE: null,
            SUPPLIER_CODE: '',
            PDC_AMOUNT: '',
            PDC_DETAILS: '',
            BANK_NAME: '',
            REMARKS: '',
            BRANCH_CODE: '',
            COMPANY_CODE: '',
            CREATED_BY: '',
            CREATED_DATE: "",
            VG_DATE: null,
            MODIFY_DATE: null,
            SYN_ROWID: '',
            MANUAL_NO: '',
            MODIFY_BY: '',
            BOUNCE_FLAG: 'N',
            BOUNCE_DATE: null,
            BOUNCE_VC_NO: '',
            PRIOR_DAYS: 0,
            CHEQUE_NO: '',
            CHEQUE_PHOTO: '',
            CHEQUE_PATH: '',
            CHEQUE_TYPE: 'PDC',
            DELETED_FLAG: '',
            ACC_CODE: '',
            BANK_ACC_CODE: '',
            RETURN_FLAG: '',
            RETURN_DATE: '',
            ENCASH_FLAG: '',
            RETURN_REASON: ''
        };
        $scope.PARTY_TYPE_EDESC_PAYMENT = "";
        $scope.enCashPaymentDataModel = {
            PAYMENT_NO: "",
            ENCASH_DATE: ""
        };
    }


    $scope.ReturnPaymentPDC = function (paymentNo) {
        $scope.returnPaymentModel = {};
        if (!paymentNo) return;
        if ($scope.RETURNPAYMENT) {
            var returnFlag = "Y";
        }
        var returnDate = $("#paymentChqueReturnEnglishDate").val();
        if (returnDate === "") {
            displayPopupNotification("Please select Return Date!", "warning");
            return;
        }
        $scope.returnPaymentModel.RETURN_DATE = returnDate;
        $scope.returnPaymentModel.PAYMENT_NO = paymentNo;
        $scope.returnPaymentModel.RETURN_FLAG = returnFlag;

        $http({
            method: 'POST',
            url: '/api/SetupApi/UpdatePaymentReturnData',
            data: $scope.returnPaymentModel

        }).then(function (response) {

            if (response.data.STATUS_CODE = 200) {
                $scope.resetPdcPaymentModel();
                $scope.BindPDCPaymentGrid();
                $('#pdcPaymentEntryModal').modal('hide');
                displayPopupNotification("Return Status Updated Successfully!", "success");
            }
        }, function (error) {
            displayPopupNotification("Something Went Wrong!", "error");
        });
    }


    $scope.EncashPaymentPDC = function (paymentNo) {
        $scope.enCashPaymentDataModel = {};
        if (!paymentNo) return;
        var encashDate = $("#paymentEncashChequeEnglishDate").val();
        if (encashDate === "") {
            displayPopupNotification("Please select Encash Date!", "warning");
            return;
        }
        $scope.enCashPaymentDataModel.ENCASH_DATE = encashDate;
        $scope.enCashPaymentDataModel.PAYMENT_NO = paymentNo

        $http({
            method: 'POST',
            url: '/api/SetupApi/UpdatePaymentPDCEncashData',
            data: $scope.enCashPaymentDataModel

        }).then(function (response) {

            if (response.data.STATUS_CODE = 200) {
                $scope.resetPdcPaymentModel();
                $scope.BindPDCPaymentGrid();
                $('#pdcPaymentEntryModal').modal('hide');
                displayPopupNotification("Encash Status Updated Successfully!", "success");
            }
        }, function (error) {
            displayPopupNotification("Something went wrong!", "error");
        });
    }



    $scope.DeletePaymentPDC = function (paymentNo) {
        if (!paymentNo) return;
        showUniversalPopup({
            title: "Delete PDC Payment",
            message: `Are you sure you want to delete this PDC Payment? (<strong>${paymentNo}</strong>)`,
            confirmText: "Delete",
            confirmColor: "#dc3545",
            onConfirm: function () {
                $http.delete('/api/SetupApi/DeletePaymentPDC', { params: { paymentNo: paymentNo } }).then(function (res) {
                    if (typeof displayPopupNotification === 'function') displayPopupNotification('Deleted', 'success');
                    $scope.BindPDCPaymentGrid();
                }, function () {
                    if (typeof displayPopupNotification === 'function') displayPopupNotification('Delete failed', 'error');
                });
            }
        });
    };





    $scope.SetSupplierCode = function (suppplierCode) {
        try {
            

            // RESET PARTY TYPE BEFORE LOADING NEW DATA
            $scope.pdcPaymentModel.PARTY_TYPE_CODE = null;
            $scope.partyTypesForPDCPayment = [];

            if (suppplierCode) {

                $http.get("/api/SetupApi/GetPartyTypeCodeForSupplier?supplierCode=" +
                    encodeURIComponent(suppplierCode))  
                    .then(function (response) {

                        $scope.partyTypesForPDCPayment = response.data;

                        if ($scope.partyTypesForPDCPayment && $scope.partyTypesForPDCPayment.length > 0) {

                            let firstItem = $scope.partyTypesForPDCPayment[0];

                            // Assign first item's EDESC
                            $scope.PARTY_TYPE_EDESC_PAYMENT = firstItem.PARTY_TYPE_EDESC;

                            // Assign first item's CODE into model
                            $scope.pdcPaymentModel.PARTY_TYPE_CODE = firstItem.PARTY_TYPE_CODE;
                        }

                    }, function (error) {
                        console.error("API error:", error);
                    });


            }

        } catch (ex) { }
    };

    $scope.editPaymentPDC = function (paymentNo) {
        if (!paymentNo) return;
        $scope.PAYMENTEDITMODE = true;
        $http.get("/api/SetupApi/GetPaymentPDCByIdForEdit", {
            params: { paymentNo: paymentNo }
        })
            .then(function (response) {
                if (response.data.STATUS_CODE = 200) {
                    var d = response.data.DATA;


                    $scope.pdcPaymentModel = {
                        PAYMENT_NO: d.PAYMENT_NO,
                        PAYMENT_DATE: d.PAYMENT_DATE,
                        CHEQUE_DATE: d.CHEQUE_DATE,
                        ENCASH_DATE: d.ENCASH_DATE,
                        SUPPLIER_CODE: d.SUPPLIER_CODE,
                        PDC_AMOUNT: d.PDC_AMOUNT,
                        PDC_DETAILS: d.PDC_DETAILS,
                        BANK_NAME: d.BANK_NAME,
                        REMARKS: d.REMARKS,
                        CHEQUE_NO: d.CHEQUE_NO,
                        CHEQUE_TYPE: d.CHEQUE_TYPE,
                        CHEQUE_NO: d.CHEQUE_NO,
                        PRIOR_DAYS: d.PRIOR_DAYS,
                        CREATED_BY: d.CREATED_BY,
                        CREATED_DATE: d.CREATED_DATE,
                        BANK_ACC_CODE: d.BANK_ACC_CODE,
                        ACC_CODE: d.ACC_CODE,
                        BANK_ACC_CODE: d.BANK_ACC_CODE,
                        RETURN_DATE: d.RETURN_DATE,
                        RETURN_FLAG: d.RETURN_FLAG,
                        CHEQUE_PATH: d.CHEQUE_PATH,
                    };

                    $scope.SetSupplierCode(d.SUPPLIER_CODE);
                    function toDateOnly(dateStr) {
                        return dateStr ? dateStr.split("T")[0] : null;
                    }

                    var paymentDate = toDateOnly(d.PAYMENT_DATE);
                    var chequeDate = toDateOnly(d.CHEQUE_DATE);
                    var returnDate = toDateOnly(d.RETURN_DATE);
                    var createdDate = toDateOnly(d.CREATED_DATE);
                    var encashDate = toDateOnly(d.ENCASH_DATE);

                    // English datepickers
                    $("#paymentEnglishDate").data("kendoDatePicker").value(paymentDate);
                    $("#paymentChequeEnglishDate").data("kendoDatePicker").value(chequeDate);
                    $("#paymentEncashChequeEnglishDate").data("kendoDatePicker").value(encashDate);
                    $("#paymentCreatedChequeEnglishDate").data("kendoDatePicker").value(createdDate);
                    $("#paymentChqueReturnEnglishDate").data("kendoDatePicker").value(returnDate);

                    // Nepali date inputs
                    $("#paymentNepaliDate").val(paymentDate ? AD2BS(paymentDate) : "");
                    $("#paymentChequeNepaliDate").val(chequeDate ? AD2BS(chequeDate) : "");
                    $("#paymentChequeEncashNepaliDate").val(encashDate ? AD2BS(encashDate) : "");
                    $("#paymentChequeCreatedNepaliDate").val(createdDate ? AD2BS(createdDate) : "");
                    $("#paymentChequeReturnNepaliDate").val(returnDate ? AD2BS(returnDate) : "");
                    if (!d.ACC_CODE) {
                        $scope.pdcPaymentModel.ACC_CODE = $scope.preferencesModel.PAYMENT_ACC_CODE;
                        $scope.ConfirmACC = true;
                    }
                    if (d.ACC_CODE) {
                        $scope.ConfirmACC = true;
                    }
                    if (d.RETURN_FLAG === "Y") {
                        $scope.RETURNPAYMENT = true;
                    }
                    
                    $("#pdcPaymentEntryModal").modal({
                        backdrop: 'static',
                        keyboard: false
                    });

                }

            }, function (error) {
                displayPopupNotification("Failed to get data!", "error");
            });
    };



    $scope.savePaymentPDC = function () {
        if (!$scope.pdcPaymentForm) {
            displayPopupNotification("Form not initialized yet!", "warning");
            return;
        }

        if ($scope.pdcPaymentForm.$invalid) {
            angular.forEach($scope.pdcPaymentForm.$error.required, function (field) {
                field.$setTouched();
            });
            return;
        }


        var paymentDate = $("#paymentEnglishDate").val();
        var chequeDate = $("#paymentChequeEnglishDate").val();

        var jsPaymentDate = new Date(paymentDate);
        var jsChequeDate = new Date(chequeDate);

        $scope.pdcPaymentModel.PAYMENT_DATE = $filter('date')(jsPaymentDate, 'MM/dd/yyyy');
        $scope.pdcPaymentModel.CHEQUE_DATE = $filter('date')(jsChequeDate, 'MM/dd/yyyy');

        var formData = new FormData();

        //  Append all model fields
        angular.forEach($scope.pdcPaymentModel, function (value, key) {
            if (value != null && key !== "CHEQUE_PHOTO") {
                formData.append(key, value);
            }
        });

        //  Append file
        if ($scope.pdcPaymentModel.CHEQUE_PHOTO) {
            formData.append("CHEQUE_PHOTO", $scope.pdcPaymentModel.CHEQUE_PHOTO[0]);
        }

        $http({
            method: 'POST',
            url: '/api/SetupApi/SavePaymentPDC',
            data: formData,
            headers: { 'Content-Type': undefined },
            transformRequest: angular.identity
        }).then(function (response) {
            console.log(response)
            debugger;
            if (response.data.STATUS_CODE = 200) {
                $scope.resetFormValidation();
                $scope.resetPdcPaymentModel();
                var grid = $("#paymentsGrid").data("kendoGrid");
                if (grid != undefined) {
                    grid.dataSource.read();
                }
                $('#pdcPaymentEntryModal').modal('hide');
                displayPopupNotification("PDC saved successfully!", "success");
            }
        }, function (error) {
            if (error.data && error.data.STATUS_CODE === 409) {
                displayPopupNotification("PDC Already Exists!", "warning");
                return;
            }
            displayPopupNotification("Something went wrong!", "error");
        });
    };



    $scope.onChangeBankName = function (e) {
        try {
            var item = e.sender.dataItem();
            if (!item) return;
            $scope.pdcPaymentModel.BANK_ACC_CODE = item.ACC_CODE;
            $scope.pdcPaymentModel.BANK_NAME = item.ACC_EDESC;

        } catch (ex) { }
    };



    $scope.partyTypesForPDCPayment = [];

    $scope.onChangeSupplierCode = function (e) {
        try {
            var item = e.sender.dataItem();
            if (!item) return;

            // RESET PARTY TYPE BEFORE LOADING NEW DATA
            $scope.pdcPaymentModel.PARTY_TYPE_CODE = null;
            $scope.partyTypesForPDCPayment = [];
            $scope.SUPPLIER_CODE = item.SUPPLIER_CODE;

            if (item.SUPPLIER_CODE) {

                $http.get("/api/SetupApi/GetPartyTypeCodeForSupplier?supplierCode=" +
                    encodeURIComponent(item.SUPPLIER_CODE))
                    .then(function (response) {

                        $scope.partyTypesForPDCPayment = response.data;

                        if ($scope.partyTypesForPDCPayment && $scope.partyTypesForPDCPayment.length > 0) {

                            let firstItem = $scope.partyTypesForPDCPayment[0];

                            // Assign first item's EDESC
                            $scope.PARTY_TYPE_EDESC_PAYMENT = firstItem.PARTY_TYPE_EDESC;

                            // Assign first item's CODE into model
                            $scope.pdcPaymentModel.PARTY_TYPE_CODE = firstItem.PARTY_TYPE_CODE;
                        }

                    }, function (error) {
                        console.error("API error:", error);
                    });


            }

        } catch (ex) { }
    };

    
    $scope.GeneratePDCReceiptVoucher = function () {
        var receiptNo = $scope.pdcModel.RECEIPT_NO;
        $http.post("/api/SetupApi/GenerateVoucher?receiptNo=" + receiptNo)
            .then(function (response) {
                if (response.data.STATUS_CODE === 200) {
                    var grid = $("#pdcGrid").data("kendoGrid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    $('#createPDCModal').modal('hide');
                    displayPopupNotification("Voucher has been generated successfully!", "success");
                } else {
                    displayPopupNotification(response.data.MESSAGE || "Failed to generate voucher!", "error");
                }
            }, function (error) {
                displayPopupNotification("Something went wrong!", "error");
            });
    };

    $scope.GeneratePDCPaymentVoucher = function () {
        var paymentNo = $scope.pdcPaymentModel.PAYMENT_NO;
        $http.post("/api/SetupApi/GeneratePaymentVoucher?paymentNo=" + paymentNo)
            .then(function (response) {
                if (response.data.STATUS_CODE === 200) {
                    $scope.BindPDCPaymentGrid();
                    $('#pdcPaymentEntryModal').modal('hide');
                    displayPopupNotification("Voucher has been generated successfully!", "success");
                } else {
                    displayPopupNotification(response.data.MESSAGE || "Failed to generate voucher!", "error");
                }
            }, function (error) {
                displayPopupNotification("Something went wrong!", "error");
            });
    };


    $scope.savePreference = function () {
        if (!$scope.preferencesModel.RECEIPT_FORM_CODE ) {
            displayPopupNotification(
                "Please select the Receipt Document!",
                "warning"
            );
            return;
        }
        if (!$scope.preferencesModel.RECEIPT_ACC_CODE) {
            displayPopupNotification(
                "Please select the Receipt Account!",
                "warning"
            );
            return;
        }
        if (!$scope.preferencesModel.PAYMENT_FORM_CODE) {
            displayPopupNotification(
                "Please select the Payment Document!",
                "warning"
            );
            return;
        }
        if (!$scope.preferencesModel.PAYMENT_FORM_CODE) {
            displayPopupNotification(
                "Please select the Payment Account!",
                "warning"
            );
            return;
        }

        $http({
            method: 'POST',
            url: '/api/SetupApi/SavePreference',
            data: $scope.preferencesModel

        }).then(function (response) {
            console.log(response)
            debugger;
            if (response.data.STATUS_CODE = 200) {
                if (response.data.DATA = "INSERTED") {
                    $scope.resetPreferenceForm();
                    $scope.getPreference();
                    displayPopupNotification("Record has been saved successfully!", "success");
                }
                else {
                    $scope.getPreference();
                    displayPopupNotification("Record has been updated successfully!", "success");
                }
                
            }
        }, function (error) {
            displayPopupNotification("Something went wrong!", "error");
        });
    }


    $scope.Return = function () {
        if ($scope.RETURN) {
            $scope.returnModel.RETURN_FLAG = "Y";
        }
        var returnDate = $("#returnEnglishDate").val();

        if (!returnDate) {
            displayPopupNotification(
                "Please provide Return date to Proceed the Cheque in Return Process!",
                "warning"
            );
            return;
        }
        $scope.returnModel.RETURN_DATE = returnDate;
        
        if (!$scope.returnModel.RETURN_FLAG) {
            displayPopupNotification(
                "Please provide Return falg to Proceed the Cheque in Return Process!",
                "warning"
            );
            return;
        }

        $http({
            method: 'POST',
            url: '/api/SetupApi/UpdateReturnData',
            data: $scope.returnModel

        }).then(function (response) {

            if (response.data.STATUS_CODE = 200) {
                $scope.resetFormValidation();
                $scope.resetPdcModel();
                var grid = $("#pdcGrid").data("kendoGrid");
                if (grid != undefined) {
                    grid.dataSource.read();
                }
                $('#createPDCModal').modal('hide');
                displayPopupNotification("Return status updated successfully!", "success");
            }
        }, function (error) {
            displayPopupNotification("Something went wrong!", "error");
        });
    }


    $scope.Bounce = function () {
        if ($scope.BOUNCE) {
            $scope.bounceModel.BOUNCE_FLAG = "Y";
        }
        if (!$scope.bounceModel.BOUNCE_REASON) {
            displayPopupNotification(
                "Please provide Bounce reason to Proceed the Cheque in Bounce Process!",
                "warning"
            );
            return;
        }


        if (!$scope.bounceModel.BOUNCE_FLAG) {
            displayPopupNotification(
                "Please select Bounce flag to Proceed the Cheque in Bounce Process!",
                "warning"
            );
            return;
        }

        $http({
            method: 'POST',
            url: '/api/SetupApi/UpdateBounceData',
            data: $scope.bounceModel

        }).then(function (response) {

            if (response.data.STATUS_CODE = 200) {
                $scope.resetFormValidation();
                $scope.resetPdcModel();
                var grid = $("#pdcGrid").data("kendoGrid");
                if (grid != undefined) {
                    grid.dataSource.read();
                }
                $('#createPDCModal').modal('hide');
                displayPopupNotification("Bounce status updated successfully!", "success");
            }
        }, function (error) {
            displayPopupNotification("Something went wrong!", "error");
        });
    }




    $scope.UpdateEncash = function () {
        var encashDate = $("#encashEnglishDate").val();
        $scope.enCashDataModel.ENCASH_DATE = encashDate;

        if ($scope.pdcModel.CHEQUE_DATE) {
            // Convert cheque date to Date object
            var chequeDate = new Date($scope.pdcModel.CHEQUE_DATE);

            // Get today's date (without time)
            var today = new Date();
            today.setHours(0, 0, 0, 0);

            // Remove time from cheque date as well
            chequeDate.setHours(0, 0, 0, 0);

            // Check if cheque date is in the future
            if (chequeDate > today) {
                displayPopupNotification(
                    "Future cheque dates cannot be encashed!",
                    "warning"
                );
                return;
            }
        }

        if (!$scope.enCashDataModel.REMARKS) {
            displayPopupNotification(
                "Please provide remarks to Proceed the Cheque in encash Process!",
                "warning"
            );
            return;
        }

        $http({
            method: 'POST',
            url: '/api/SetupApi/UpdateEncashData',
            data: $scope.enCashDataModel

        }).then(function (response) {

            if (response.data.STATUS_CODE = 200) {
                $scope.resetFormValidation();
                $scope.resetPdcModel();
                var grid = $("#pdcGrid").data("kendoGrid");
                if (grid != undefined) {
                    grid.dataSource.read();
                }
                $('#createPDCModal').modal('hide');
                displayPopupNotification("Encash status updated successfully!", "success");
            }
        }, function (error) {
            displayPopupNotification("Something went wrong!", "error");
        });
    }



    $scope.UpdatePdcToIntransit = function () {
        if ($scope.INTRANSIT) {
            $scope.inTransitDataModel.INTRANSIT_FLAG = "Y";
        }

        if (!$scope.pdcModel.ACC_CODE ) {
            displayPopupNotification(
                "Please select the Account Name to Proceed the Cheque in Transit Process!",
                "warning"
            );
            return;
        }
        $scope.inTransitDataModel.ACC_CODE = $scope.pdcModel.ACC_CODE;
        if (!$scope.inTransitDataModel.INTRANSIT_FLAG) {
            displayPopupNotification(
                "Please select the intransit Flag for the Intransit!",
                "warning"
            );
            return;
        }
        if ($scope.pdcModel.CHEQUE_DATE) {
            var chequeDate = new Date($scope.pdcModel.CHEQUE_DATE);

            var today = new Date();
            today.setHours(0, 0, 0, 0);

            chequeDate.setHours(0, 0, 0, 0);

            if (chequeDate > today) {
                displayPopupNotification(
                    "Future cheque dates cannot be process for the Intransit!",
                    "warning"
                );
                return;
            }
        }

        $http({
            method: 'POST',
            url: '/api/SetupApi/UpdatePdcToIntransit',
            data: $scope.inTransitDataModel

        }).then(function (response) {
            console.log(response)
            debugger;
            if (response.data.STATUS_CODE = 200) {
                $scope.resetFormValidation();
                $scope.resetPdcModel();
                var grid = $("#pdcGrid").data("kendoGrid");
                if (grid != undefined) {
                    grid.dataSource.read();
                }
                $('#createPDCModal').modal('hide');
                displayPopupNotification("PDC updated successfully!", "success");
            }
        }, function (error) {
            displayPopupNotification("Something went wrong!", "error");
        });
    }

    $scope.onTransitChange = function () {

        if ($scope.INTRANSIT) {
            $scope.inTransitDataModel.INTRANSIT_FLAG = "Y";
        } else {
            $scope.inTransitDataModel.INTRANSIT_FLAG = "";
        }
    };

    
    
    $scope.onBounceChange = function () {

        if ($scope.BOUNCE) {
            $scope.ISRETURN = true;
            $scope.ISENCASH = true;
            $scope.bounceModel.BOUNCE_FLAG = "Y";
        } else {
            $scope.ISRETURN = false;
            $scope.ISENCASH = false;
            $scope.bounceModel.BOUNCE_FLAG = "";
        }
    };



    $scope.onReturnChange = function () {

        if ($scope.RETURN) {
            $scope.ISBOUNCE = true;
            $scope.ISENCASH = true;
            $scope.returnModel.RETURN_FLAG = "Y";
        } else {
            $scope.ISBOUNCE = false;
            $scope.ISENCASH = false;
            $scope.returnModel.RETURN_FLAG = "";
        }
    };




    $scope.editPDC = function (receiptNo) {
        if (!receiptNo) return;
       // $scope.ISBOUNCE = true;
       // $scope.ISRETURN = true;
        //$scope.ISENCASH = true;
        $http.get("/api/SetupApi/GetPDCByIdForEdit", {
            params: { receiptNo: receiptNo }
        })
            .then(function (response) {
                if (response.data.STATUS_CODE = 200) {
                    var d = response.data.DATA;


                    $scope.pdcModel = {
                        RECEIPT_NO: d.RECEIPT_NO,
                        RECEIPT_DATE: d.RECEIPT_DATE,
                        CHEQUE_DATE: d.CHEQUE_DATE,
                        ENCASH_DATE: d.CHEQUE_DATE,
                        CUSTOMER_CODE: d.CUSTOMER_CODE,
                        PDC_AMOUNT: d.PDC_AMOUNT,
                        PDC_DETAILS: d.PDC_DETAILS,
                        BANK_NAME: d.BANK_NAME,
                        REMARKS: d.REMARKS,
                        CHEQUE_NO: d.CHEQUE_NO,
                        CHEQUE_TYPE: d.CHEQUE_TYPE,
                        EMPLOYEE_CODE: d.MR_ISSUED_BY,
                        CHEQUE_NO: d.CHEQUE_NO,
                        MR_NO: d.MR_NO,
                        PRIOR_DAYS: d.PRIOR_DAYS,
                        INTRANSIT_DATE: d.INTRANSIT_DATE,
                        INTRANSIT_BY: d.INTRANSIT_BY,
                        CREATED_BY: d.CREATED_BY,
                        CREATED_DATE: d.CREATED_DATE,
                        ACC_CODE: d.ACC_CODE,
                        INTRANSIT_FLAG: d.INTRANSIT_FLAG,
                        RETURN_DATE: d.RETURN_DATE,
                        RETURN_FLAG: d.RETURN_FLAG,
                        BOUNCE_DATE: d.BOUNCE_DATE,
                        BOUNCE_REASON: d.BOUNCE_REASON,
                        BOUNCE_FLAG: d.BOUNCE_FLAG,
                        BOUNCE_BY: d.BOUNCE_BY,
                        CHEQUE_PATH: d.CHEQUE_PATH,
                    };                               
                      
                    $scope.setPartyType(d.CUSTOMER_CODE);
                    function toDateOnly(dateStr) {
                        return dateStr ? dateStr.split("T")[0] : null;
                    }

                    var receiptDate = toDateOnly(d.RECEIPT_DATE);
                    var chequeDate = toDateOnly(d.CHEQUE_DATE);
                    var returnDate = toDateOnly(d.RETURN_DATE);
                    var createdDate = toDateOnly(d.CREATED_DATE);
                    var intransitDate = toDateOnly(d.INTRANSIT_DATE);
                    var bounceDate = toDateOnly(d.BOUNCE_DATE);

                    // English datepickers
                    $("#receiptEnglishDate").data("kendoDatePicker").value(receiptDate);
                    $("#chequeEnglishDate").data("kendoDatePicker").value(chequeDate);
                    $("#encashEnglishDate").data("kendoDatePicker").value(chequeDate);
                    $("#createdEnglishDate").data("kendoDatePicker").value(createdDate);
                    $("#inTransitEnglishDate").data("kendoDatePicker").value(intransitDate);
                    $("#bounceEnglishDate").data("kendoDatePicker").value(bounceDate);
                    $("#returnEnglishDate").data("kendoDatePicker").value(returnDate);

                    // Nepali date inputs
                    $("#nepaliDateReceipt").val(receiptDate ? AD2BS(receiptDate) : "");
                    $("#nepaliDateForCheque").val(chequeDate ? AD2BS(chequeDate) : "");
                    $("#encashNepaliDate").val(chequeDate ? AD2BS(chequeDate) : "");
                    $("#createdNepaliDate").val(createdDate ? AD2BS(createdDate) : "");
                    $("#inTransitNepaliDate").val(intransitDate ? AD2BS(intransitDate) : "");
                    $("#bounceNepaliDate").val(bounceDate ? AD2BS(bounceDate) : "");
                    $("#returnNepaliDate").val(returnDate ? AD2BS(returnDate) : "");


                    if (d.INTRANSIT_FLAG === "Y") {
                        //$scope.disableEncash = false;
                        $scope.ISINTRANSIT = true;
                        //$scope.ISRETURN = false;
                       // $scope.ISBOUNCE = false;
                        //$scope.ISENCASH = false;
                        $scope.editMode = false;
                        $scope.ACCOUNTDISABLE = true;
                        $scope.ISACCON = true;
                        $scope.DISABLEVOUCHER = true;
                        $scope.INTRANSIT =  true;


                        //let nepaliToday = ConvertEngDateToNep(moment().format('DD-MMM-YYYY'));
                        //$("#encashNepaliDate").val(nepaliToday);

                        //var date = BS2AD(nepaliToday);
                        //$scope.pdcModel.ENCASH_DATE = date;
                        //$("#encashEnglishDate").data("kendoDatePicker").value(moment(date).format("DD-MMM-YYYY"));
                    }
                    else {
                        $scope.ISINTRANSIT = false;
                    }
                    //else {
                    //    var encashDate = toDateOnly(d.ENCASH_DATE);
                    //    $("#encashEnglishDate").data("kendoDatePicker").value(encashDate);
                    //    $("#encashNepaliDate").val(encashDate ? AD2BS(encashDate) : "");

                    //}
                    $scope.editMode = true;
                    $scope.disableOnEdit = true;
                    //$scope.INTRANSIT = d.INTRANSIT_FLAG === "Y" ? true : false;
                    //$scope.RETURN = d.RETURN_FLAG === "Y" ? true : false;
                    if (d.RETURN_FLAG === "Y") {
                        $scope.ISRETURN = true;
                        $scope.ISBOUNCE = true;
                        $scope.disableEncash = true;
                        $scope.DISABLEVOUCHER = true;
                        $scope.RETURN =  true;
                    }
                   // $scope.BOUNCE = d.BOUNCE_FLAG === "Y" ? true : false;
                    if (d.BOUNCE_FLAG === "Y") {
                        $scope.ISBOUNCE = true;
                        $scope.ISRETURN = true;
                        $scope.disableEncash = true;
                        $scope.DISABLEVOUCHER = true;
                        $scope.BOUNCE =  "Y" ;
                    }
                    if (d.ENCASH_DATE) {
                        $scope.DISABLEVOUCHER = false;
                    }
                    if (d.VG_DATE && d.VOUCHER_NO) {
                        $scope.DISABLEVOUCHER = true;
                        $scope.disableEncash = true;
                    }
                    if (!d.ACC_CODE) {
                        $scope.pdcModel.ACC_CODE = $scope.preferencesModel.RECEIPT_ACC_CODE;
                    }

                    $scope.inTransitDataModel.RECEIPT_NO = d.RECEIPT_NO;
                    $scope.enCashDataModel.RECEIPT_NO = d.RECEIPT_NO;
                    $scope.returnModel.RECEIPT_NO = d.RECEIPT_NO;
                    $scope.bounceModel.RECEIPT_NO = d.RECEIPT_NO;
                    $scope.enCashDataModel.ENCASH_DATE = d.CHEQUE_DATE;
                    $scope.bounceModel.BOUNCE_REASON = d.BOUNCE_REASON;
                    if (d.ENCASH_DATE) {
                        $scope.ISRETURN = true;
                        $scope.ISBOUNCE = true;
                    }
                    $("#createPDCModal").modal({
                        backdrop: 'static',
                        keyboard: false
                    });
                }
                
            }, function (error) {
                displayPopupNotification("Failed to get data!", "error");
            });
    };

    $scope.setPartyType = function (customerCode) {
        if (customerCode) {

            $http.get("/api/SetupApi/GetPartyTypeCodeForCustomer?customerCode=" +
                encodeURIComponent(customerCode))
                .then(function (response) {

                    $scope.partyTypesForPDC = response.data;

                    if ($scope.partyTypesForPDC && $scope.partyTypesForPDC.length > 0) {

                        let firstItem = $scope.partyTypesForPDC[0];

                        // Assign first item's EDESC
                        $scope.PARTY_TYPE_EDESC = firstItem.PARTY_TYPE_EDESC;

                        // Assign first item's CODE into model
                        $scope.pdcModel.PARTY_TYPE_CODE = firstItem.PARTY_TYPE_CODE;
                    }

                }, function (error) {
                    console.error("API error:", error);
                });


        }
    }


    $scope.saveData = function () {
        if (!$scope.pdcEntryForm) {
            displayPopupNotification("Form not initialized yet!", "warning");
            return;
        }

        if ($scope.pdcEntryForm.$invalid) {
            angular.forEach($scope.pdcEntryForm.$error.required, function (field) {
                field.$setTouched();
            });
            return;
        }
        var receiptDate = $("#receiptEnglishDate").val();
        var chequeDate = $("#chequeEnglishDate").val();

        var jsReceiptDate = new Date(receiptDate);
        var jsChequeDate = new Date(chequeDate);

        $scope.pdcModel.RECEIPT_DATE = $filter('date')(jsReceiptDate, 'MM/dd/yyyy');
        $scope.pdcModel.CHEQUE_DATE = $filter('date')(jsChequeDate, 'MM/dd/yyyy');

        var formData = new FormData();

        //  Append all model fields
        angular.forEach($scope.pdcModel, function (value, key) {
            if (value != null && key !== "CHEQUE_PHOTO") {
                formData.append(key, value);
            }
        });

        //  Append file
        if ($scope.pdcModel.CHEQUE_PHOTO) {
            formData.append("CHEQUE_PHOTO", $scope.pdcModel.CHEQUE_PHOTO[0]);
        }

        $http({
            method: 'POST',
            url: '/api/SetupApi/SaveNewPDC',
            data: formData,
            headers: { 'Content-Type': undefined },   
            transformRequest: angular.identity        
        }).then(function (response) {
            console.log(response)
            debugger;
            if (response.data.STATUS_CODE = 200) {
                $scope.resetFormValidation();
                $scope.resetPdcModel();
                var grid = $("#pdcGrid").data("kendoGrid");
                if (grid != undefined) {
                    grid.dataSource.read();
                }
                $('#createPDCModal').modal('hide');
                displayPopupNotification("PDC saved successfully!", "success");
            }           
        }, function (error) {
            if (error.data && error.data.STATUS_CODE === 409) {
                displayPopupNotification("PDC Already Exists!", "warning");
                return;
            }
            displayPopupNotification("Something went wrong!", "error");
        });
    };


    // Reset duplicate validation whenever model changes
    $scope.$watch('pdcModel.CHEQUE_NO', function (newVal, oldVal) {
        if (newVal !== oldVal) {
            $scope.pdcEntryForm.chequeNo.$setValidity("duplicate", true);
        }
    });

    // Updated check function - only validate non-empty values
    $scope.checkDuplicateChequeNo = function () {
        var field = $scope.pdcEntryForm.chequeNo;

        // If empty or whitespace only, stay valid
        if (!$scope.pdcModel.CHEQUE_NO || $scope.pdcModel.CHEQUE_NO.trim() === '') {
            field.$setValidity("duplicate", true);
            return;
        }

        // API call for non-empty values
        $http.post('/api/SetupApi/CheckDuplicateChequeNo',
            JSON.stringify($scope.pdcModel.CHEQUE_NO),
            { headers: { 'Content-Type': 'application/json' } }
        ).then(function (response) {
            field.$setValidity("duplicate", !response.data.IS_DUPLICATE);
        }).catch(function (error) {
            displayPopupNotification("Error checking duplicate cheque number!", "error");
            field.$setValidity("duplicate", true);
        });
    };






    $scope.resetFormValidation = function () {
        if ($scope.pdcEntryForm) {
            // Reset form to pristine state
            $scope.pdcEntryForm.$setPristine();
            $scope.pdcEntryForm.$setUntouched();
        }
    };



    $scope.resetPaymentFormValidation = function () {
        if ($scope.pdcPaymentForm) {
            $scope.pdcPaymentForm.$setPristine();
            $scope.pdcPaymentForm.$setUntouched();
        }
    };



    $scope.setChequePhoto = function (files) {
        $scope.pdcModel.CHEQUE_PHOTO = files;
    };


    $scope.setPaymentChequePhoto = function (files) {
        $scope.pdcPaymentModel.CHEQUE_PHOTO = files;
    };


    $scope.customersForPDC = [];

    $http.get("/api/SetupApi/GetCustomerForPDC")
        .then(function (response) {
            $scope.customersForPDC = response.data;
            $scope.customerForPDCOptions = {
                dataSource: new kendo.data.DataSource({
                    data: $scope.customersForPDC
                }),
                filter: "contains",
                optionLabel: "-Select Customer-",
                dataTextField: "CUSTOMER_EDESC",
                dataValueField: "CUSTOMER_CODE",
                popup: {
                    appendTo: "#createPDCModal"   // modal ID
                }
            };

        }, function (error) {
            console.error("API error:", error);
        });


    $scope.employeesForPDC = [];

    $http.get("/api/SetupApi/GetEmployeeForPDC")
        .then(function (response) {
            $scope.employeesForPDC = response.data;
            $scope.employeesForPDCOptions = {
                dataSource: new kendo.data.DataSource({
                    data: $scope.employeesForPDC
                }),
                filter: "contains",
                optionLabel: "-Select Employee-",
                dataTextField: "EMPLOYEE_EDESC",
                dataValueField: "EMPLOYEE_CODE",
                popup: {
                    appendTo: "#createPDCModal"   // modal ID
                }
            };

        }, function (error) {
            console.error("API error:", error);
        });


    $scope.suppliersForPDC = [];

    $http.get("/api/SetupApi/GetSupplierForPDC")
        .then(function (response) {
            $scope.suppliersForPDC = response.data;
            $scope.supplierForPDCOptions = {
                dataSource: new kendo.data.DataSource({
                    data: $scope.suppliersForPDC
                }),
                filter: "contains",
                optionLabel: "-Select supplier-",
                dataTextField: "SUPPLIER_EDESC",
                dataValueField: "SUPPLIER_CODE",
                popup: {
                    appendTo: "#pdcPaymentEntryModal"   // modal ID
                }
            };

        }, function (error) {
            console.error("API error:", error);
        });


    $scope.chartOfAccounts = [];

    $http.get("/api/SetupApi/GetChartOfAccounts")
        .then(function (response) {
            $scope.chartOfAccounts = response.data;
            $scope.chartOfAccountForPDCOptions = {
                dataSource: new kendo.data.DataSource({
                    data: $scope.chartOfAccounts
                }),
                filter: "contains",
                optionLabel: "-Select chart of acc-",
                dataTextField: "ACC_EDESC",
                dataValueField: "ACC_CODE",
                popup: {
                    appendTo: "#createPDCModal"   // modal ID
                }
            };

        }, function (error) {
            console.error("API error:", error);
        });



    $http.get("/api/SetupApi/GetChartOfAccounts")
        .then(function (response) {
            $scope.chartOfAccounts = response.data;
            $scope.chartOfAccountForPDCPaymentOptions = {
                dataSource: new kendo.data.DataSource({
                    data: $scope.chartOfAccounts
                }),
                filter: "contains",
                optionLabel: "-Select bank name-",
                dataTextField: "ACC_EDESC",
                dataValueField: "ACC_CODE",
                popup: {
                    appendTo: "#pdcPaymentEntryModal"   // modal ID
                }
            };

        }, function (error) {
            console.error("API error:", error);
        });


    $http.get("/api/SetupApi/GetChartOfAccounts")
        .then(function (response) {
            $scope.chartOfAccounts = response.data;
            $scope.coaForPDCPaymentOptions = {
                dataSource: new kendo.data.DataSource({
                    data: $scope.chartOfAccounts
                }),
                filter: "contains",
                optionLabel: "-Select acc name-",
                dataTextField: "ACC_EDESC",
                dataValueField: "ACC_CODE",
                popup: {
                    appendTo: "#pdcPaymentEntryModal"   // modal ID
                }
            };

        }, function (error) {
            console.error("API error:", error);
        });



    $http.get("/api/SetupApi/GetChartOfAccountsForPreferences")
        .then(function (response) {

            // Set the full Kendo configuration just once
            $scope.accDataForReceipt = {
                dataSource: new kendo.data.DataSource({
                    data: response.data   // <-- set data here
                }),
                filter: "contains",
                optionLabel: "-select receipt acc-",
                dataTextField: "ACC_EDESC",
                dataValueField: "ACC_CODE"
            };

        })
        .catch(function (error) {
            console.error("API error:", error);
        });

  

    $http.get("/api/SetupApi/GetChartOfAccountsForPreferences")
        .then(function (response) {

            // Initialize the Kendo dropdown with the data from API
            $scope.accDataForPayment = {
                dataSource: new kendo.data.DataSource({
                    data: response.data  // set API data here
                }),
                filter: "contains",
                optionLabel: "-select payment acc-",
                dataTextField: "ACC_EDESC",
                dataValueField: "ACC_CODE"
            };

        })
        .catch(function (error) {
            console.error("API error:", error);
        });



   

    $http.get("/api/SetupApi/GetFormSetupForPreferences")
        .then(function (response) {
            // Initialize the Kendo dropdown with API data
            $scope.formDataForReceipt = {
                dataSource: new kendo.data.DataSource({
                    data: response.data // set API data here
                }),
                filter: "contains",
                optionLabel: "-select receipt form-",
                dataTextField: "FORM_EDESC",
                dataValueField: "FORM_CODE"
            };
        })
        .catch(function (error) {
            console.error("API error:", error);
        });




  
    $http.get("/api/SetupApi/GetFormSetupForPreferences")
        .then(function (response) {
            // Initialize the Kendo dropdown with API data
            $scope.formDataForPayment = {
                dataSource: new kendo.data.DataSource({
                    data: response.data // set API data here
                }),
                filter: "contains",
                optionLabel: "-select payment form-",
                dataTextField: "FORM_EDESC",
                dataValueField: "FORM_CODE"
            };
        })
        .catch(function (error) {
            console.error("API error:", error);
        });




    $http.get("/api/SetupApi/GetPreference")
        .then(function (response) {
            $scope.preferencesModel = response.data.DATA;
        }, function (error) {
            console.error("API error:", error);
        });

    $scope.getPreference = function () {
        $http.get("/api/SetupApi/GetPreference")
            .then(function (response) {
                $scope.preferencesModel = response.data.DATA;
            }, function (error) {
                console.error("API error:", error);
            });
    }

    


    $scope.partyTypesForPDC = [];

    $scope.onChangeCustomerCode = function (e) {
        try {
            var item = e.sender.dataItem();
            if (!item) return;

            // RESET PARTY TYPE BEFORE LOADING NEW DATA
            $scope.pdcModel.PARTY_TYPE_CODE = null;       
            $scope.partyTypesForPDC = [];        
            $scope.CUSTOMER_CODE = item.CUSTOMER_CODE;

            if (item.CUSTOMER_CODE) {

                $http.get("/api/SetupApi/GetPartyTypeCodeForCustomer?customerCode=" +
                    encodeURIComponent(item.CUSTOMER_CODE))
                    .then(function (response) {

                        $scope.partyTypesForPDC = response.data;

                        if ($scope.partyTypesForPDC && $scope.partyTypesForPDC.length > 0) {

                            let firstItem = $scope.partyTypesForPDC[0];

                            // Assign first item's EDESC
                            $scope.PARTY_TYPE_EDESC = firstItem.PARTY_TYPE_EDESC;

                            // Assign first item's CODE into model
                            $scope.pdcModel.PARTY_TYPE_CODE = firstItem.PARTY_TYPE_CODE;
                        }

                    }, function (error) {
                        console.error("API error:", error);
                    });


            }

        } catch (ex) { }
    };


    //$scope.onChangeEmployeeCode = function (e) {

    //    try {
    //        var item = e.sender.dataItem();
    //        if (!item) return;
    //        scope.EMPLOYEE_CODE = item.EMPLOYEE_CODE;
    //    }
    //    catch (ex) { }
    //}

    $scope.onChangePartyType = function (e) {
        try {
            var item = e.sender.dataItem();
            if (!item) return;            
            $scope.PARTY_TYPE_CODE = item.PARTY_TYPE_CODE;
        } catch (ex) { }
    };

    $scope.onChangeEmployeeCode = function (e) {
        debugger;
        try {
            var item = e.sender.dataItem();
            if (!item) return;            
            $scope.pdcModel.EMPLOYEE_CODE = item.EMPLOYEE_CODE;

        } catch (ex) { }
    };

    $scope.onChangeAccCode = function (e) {
        try {
            var item = e.sender.dataItem();
            if (!item) return;
            $scope.pdcModel.ACC_CODE = item.ACC_CODE;
            $scope.inTransitDataModel.ACC_CODE = item.ACC_CODE;
            

        } catch (ex) { }
    }

  

    $scope.onChangeReceiptAccCode = function (e) {
        try {
            var item = e.sender.dataItem();
            if (!item) return;
            $scope.preferencesModel.RECEIPT_ACC_CODE = item.ACC_CODE;           
        } catch (ex) { }
    }

    $scope.onChangeReceiptDocumentCode = function (e) {
        try {
            var item = e.sender.dataItem();
            if (!item) return;
            $scope.preferencesModel.RECEIPT_FORM_CODE = item.FORM_CODE;          
        } catch (ex) { }
    }

    $scope.onChangePaymentAccCode = function (e) {
        try {
            var item = e.sender.dataItem();
            if (!item) return;
            $scope.preferencesModel.PAYMENT_ACC_CODE = item.ACC_CODE;
        } catch (ex) { }
    }

    $scope.onChangePaymentDocumentCode = function (e) {
        try {
            var item = e.sender.dataItem();
            if (!item) return;
            $scope.preferencesModel.PAYMENT_FORM_CODE = item.FORM_CODE;
        } catch (ex) { }
    }


    $scope.generateNewReceiptNo = function () {
        $http({
            method: "GET",
            url: "/api/CustomFormApi/GenerateNewReceipt",
            dataType: "JSON"
        }).then(function (response) {

            let data = response.data;

            $scope._newReceiptNo = data[0].toString().padStart(5, "0");

            $scope.pdcModel.RECEIPT_NO = $scope._newReceiptNo;
            //$scope.pdcModel.MONEY_RECEIPT_NO = Number(data[0]);

            $scope.pdcModel.CREATED_BY = data[1];
            //$scope.pdcModel.INTRANSIT_BY = data[1];
            //$scope.pdcModel.BOUNCE_BY = data[1];

        }, function (error) {
            console.error("Error fetching new receipt no:", error);
        });
    };



    $scope.generateNewPaymentNo = function () {
        $http({
            method: "GET",
            url: "/api/CustomFormApi/GenerateNewPaymentNo",
            dataType: "JSON"
        }).then(function (response) {

            let data = response.data;

            $scope._newPaymentNo = data[0].toString().padStart(5, "0");

            $scope.pdcPaymentModel.PAYMENT_NO = $scope._newPaymentNo;
            //$scope.pdcModel.MONEY_RECEIPT_NO = Number(data[0]);

            $scope.pdcPaymentModel.CREATED_BY = data[1];
            //$scope.pdcModel.INTRANSIT_BY = data[1];
            //$scope.pdcModel.BOUNCE_BY = data[1];

        }, function (error) {
            console.error("Error fetching new receipt no:", error);
        });
    };


    $scope.setActiveTab = function (tabName) {
        if (tabName === 'payments') {
            $scope.showPaymentAddButton = true;
            $scope.showReceiptAddButton = false;
        }
        if (tabName === 'receipt') {
            $scope.showPaymentAddButton = false;
            $scope.showReceiptAddButton = true;
        }
        $scope.activeTab = tabName;
    };

    $scope.ConvertEngToNep = function () {
        var engdate = $("#englishDate5").val();
        var nepalidate = ConvertEngDateToNep(engdate);
        $("#nepaliDate5").val(nepalidate);
        $("#nepaliDate51").val(nepalidate);
    };

    $scope.ConvertNepToEng = function ($event) {
        var date = BS2AD($("#nepaliDate5").val());
        var date1 = BS2AD($("#nepaliDate51").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        $("#englishdatedocument1").val($filter('date')(date1, "dd-MMM-yyyy"));
        $('#nepaliDate5').trigger('change')
        $('#nepaliDate51').trigger('change')
    };

    $scope.ConvertNepToEnglishCheque = function ($event) {
        var date = BS2AD($("#nepaliDateForCheque").val());
        $("#chequeEnglishDate").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#nepaliDateForCheque').trigger('change')
    };
    
    $scope.ConvertNepToEnglishReceipt = function ($event) {
        var date = BS2AD($("#nepaliDateReceipt").val());     
        $("#receiptEnglishDate").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#nepaliDateReceipt').trigger('change')
    };

    $scope.ConvertNepToEnglishEncash = function ($event) {
        var date = BS2AD($("#encashNepaliDate").val());     
        $("#encashEnglishDate").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#encashNepaliDate').trigger('change')
    };

    $scope.ConvertNepToEnglishCreated = function ($event) {
        var date = BS2AD($("#createdNepaliDate").val());
        $("#createdEnglishDate").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#createdNepaliDate').trigger('change')
    };

    $scope.ConvertNepToEnglishBounce = function ($event) {
        var date = BS2AD($("#bounceNepaliDate").val());
        $("#bounceEnglishDate").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#bounceNepaliDate').trigger('change')
    };

    $scope.ConvertNepToEnglishInTransit = function ($event) {
        var date = BS2AD($("#inTransitNepaliDate").val());
        $("#inTransitEnglishDate").val($filter('date')(date, "dd-MMM-yyyy"));
        $('#inTransitNepaliDate').trigger('change')
    };

    $scope.ConvertEngToNepang = function (data) {
        $("#nepaliDate5").val(AD2BS(data));
    };

    $scope.ConvertEngToNepang1 = function (data) {
        $("#nepaliDate51").val(AD2BS(data));
    }

    $scope.ConvertEngToNepaliCheque = function (data) {
        $("#nepaliDateForCheque").val(AD2BS(data));
    }

    $scope.ConvertEngToNepaliReceipt = function (data) {
        $("#nepaliDateReceipt").val(AD2BS(data));
    }
    
    $scope.ConvertEngToNepaliEncash = function (data) {
        $("#encashNepaliDate").val(AD2BS(data));
    }

    $scope.ConvertEngToNepaliCreated = function (data) {
        $("#createdNepaliDate").val(AD2BS(data));
    }

    $scope.ConvertEngToNepaliBounce = function (data) {
        $("#bounceNepaliDate").val(AD2BS(data));
    }

    $scope.ConvertEngToNepaliPaymentPDC = function (data) {
        $("#paymentNepaliDate").val(AD2BS(data));
    }
    $scope.ConvertEngToNepaliPaymentChequePDC = function (data) {
        $("#paymentChequeNepaliDate").val(AD2BS(data));
    }
    $scope.ConvertEngToNepaliPaymentReturnChequePDC = function (data) {
        $("#paymentChequeReturnNepaliDate").val(AD2BS(data));
    }
    $scope.ConvertEngToNepaliPaymentEncashChequePDC = function (data) {
        $("#paymentChequeEncashNepaliDate").val(AD2BS(data));
    }

    $scope.ConvertEngToNepaliInTransit = function (data) {
        $("#inTransitNepaliDate").val(AD2BS(data));
    }

    $scope.monthSelectorOptionsSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsReceiptSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepaliReceipt(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsChequeSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepaliCheque(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsEncashSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepaliEncash(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsCreatedSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepaliCreated(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsBounceSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepaliBounce(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsPaymentPDCSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepaliPaymentPDC(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsPaymentPDC = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepaliPaymentChequePDC(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };
    $scope.monthSelectorOptionsPaymentEncashPDC = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepaliPaymentEncashChequePDC(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };
    $scope.monthSelectorOptionsPaymentReturnPDC = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepaliPaymentReturnChequePDC(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsInTransitSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepaliInTransit(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsSingle1 = {
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepang1(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };


    $scope.LoadData = function () {
        if ($scope.showReceiptAddButton) {
            $scope.BindPDCGrid();
        }
        if ($scope.showPaymentAddButton) {
            $scope.BindPDCPaymentGrid();
        }
    }


    $scope.BindPDCGrid = function () {
        var grid = $("#pdcGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
        }
    };

    $scope.BindPDCPaymentGrid = function () {
        var grid = $("#paymentsGrid").data("kendoGrid");
        if (grid) {
            grid.dataSource.read();
        }
    };

    $scope.toDate = function (val) {
        if (!val) return null;
        var m = moment(val);
        return m.isValid() ? m.format("YYYY-MM-DD") : null;
    }

    $scope.selectedPDC = null;

    $scope.PDCGridOptions = {
        dataSource: new kendo.data.DataSource({
            transport: {
                read: {
                    url: "/api/SetupApi/GetPDCList",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json"
                },
                parameterMap: function (options, type) {
                    if (type === "read") {

                        let fromDate = $(".fromDate input").val();
                        let toDate = $(".toDate input").val();

                        var combinedOptions = angular.extend({}, options, {
                            FROM_DATE: $scope.toDate(fromDate),
                            TO_DATE: $scope.toDate(toDate),
                            PDC_TYPE: $scope.PDC_TYPE_FILTER,
                            CHEQUE_TYPE: $scope.CHEQUE_TYPE_FILTER
                        });

                        return JSON.stringify(combinedOptions);
                    }
                }
            },
            autoBind: false,
            schema: {
                data: "DATA"
            },
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            pageSize: 10,
            sort: { field: "RECEIPT_NO", dir: "desc" }
        }),

        autoBind: false,
        selectable: "single",
        scrollable: true,
        height: 350,
        sortable: true,
        pageable: true,
        resizable: true,

        dataBound: function () {
            var grid = this;

            $("#pdcGrid tbody tr").css("cursor", "pointer");

            // DOUBLE CLICK  EDIT
            //grid.tbody.find("tr").off("dblclick").on("dblclick", function () {
            //    var dataItem = grid.dataItem(this);
            //    if (dataItem) {
            //        $scope.$apply(function () {
            //            $scope.editPDC(dataItem.RECEIPT_NO);
            //        });
            //    }
            //});

            // CAPTURE RIGHT CLICK ROW
            grid.tbody.find("tr").off("contextmenu").on("contextmenu", function () {
                $scope.selectedPDC = grid.dataItem(this);
            });

            // CONTEXT MENU
            if (!$("#pdcContextMenu").data("kendoContextMenu")) {
                $("#pdcContextMenu").kendoContextMenu({
                    target: "#pdcGrid",
                    filter: "tbody tr",
                    showOn: "contextmenu",
                    select: function (e) {
                        var action = $(e.item).data("action");
                        var row = $scope.selectedPDC;

                        if (!row) return;

                        $scope.$apply(function () {
                            switch (action) {

                                case "transit":
                                    if (row.INTRANSIT_FLAG === "Y") {
                                        displayPopupNotification("PDC is already InTransited!", "warning");
                                        return;
                                    }
                                    $scope.INTRANSIT = true;
                                    $scope.ISBOUNCE = true;
                                    $scope.editPDC(row.RECEIPT_NO);
                                    break;

                                case "encash":
                                    if (row.ENCASH_DATE) {
                                        displayPopupNotification("PDC is already EnCashed!", "warning");
                                        return;
                                    }
                                    if (row.INTRANSIT_FLAG !== "Y") {   
                                        displayPopupNotification("Please process for Intransit before EnCash!", "warning");
                                        return;
                                    }

                                    if (row.BOUNCE_FLAG === "Y") {
                                        displayPopupNotification("PDC is already Bounced, you cannot EnCash!", "warning");
                                        return;
                                    }
                                    if (row.RETURN_FLAG === "Y") {
                                        displayPopupNotification("PDC is already Returned, you cannot EnCash Returned PDC!", "warning");
                                        return;
                                    }
                                    $scope.ISRETURN = true;
                                    $scope.ISENCASH = false;
                                    $scope.ISBOUNCE = true;
                                    $scope.disableEncash = false;
                                    $scope.editPDC(row.RECEIPT_NO);
                                    break;

                                case "voucher":
                                    if (!row.ENCASH_DATE) {
                                        displayPopupNotification("You cannot Generate Voucher, proceed for EnCash!", "warning");
                                        return;
                                    }
                                    if (row.INTRANSIT_FLAG !== "Y") {
                                        displayPopupNotification("You cannot Generate Voucher, proceed for InTransit!", "warning");
                                        return;
                                    }
                                    if (!row.INTRANSIT_FLAG === "Y") {
                                        displayPopupNotification("Please process for Intransit before EnCash!", "warning");
                                        return;
                                    }
                                    if (row.BOUNCE_FLAG === "Y") {
                                        displayPopupNotification("Voucher can't Generated for Bounced Cheque!", "warning");
                                        return;
                                    }
                                    if (row.RETURN_FLAG === "Y") {
                                        displayPopupNotification("PDC is already Returned, you cannot Generate Voucher !", "warning");
                                        return;
                                    }
                                    if (row.VOUCHER_NO) {
                                        displayPopupNotification("Voucher is already Generated!", "warning");
                                        return;
                                    }
                                    $scope.DISABLEVOUCHER = false;
                                    $scope.disableEncash = true;
                                    $scope.editPDC(row.RECEIPT_NO);
                                    break;

                                case "direct-bounce":
                                    if (row.INTRANSIT_FLAG !== "Y") {
                                        displayPopupNotification("You cannot bounce, proceed for InTransit!", "warning");
                                        return;
                                    }
                                    if (!row.INTRANSIT_FLAG === "Y") {
                                        displayPopupNotification("Please process for InTransit before EnCash!", "warning");
                                        return;
                                    }
                                    if (row.RETURN_FLAG === "Y") {
                                        displayPopupNotification("You cannot Bounce the Returned Cheque!", "warning");
                                        return;
                                    }
                                     if (row.BOUNCE_FLAG === "Y") {
                                         displayPopupNotification("PDC is already Bounced!", "warning");
                                         return;
                                    }
                                    if (row.ENCASH_DATE) {
                                        displayPopupNotification("Encashed PDC cannot be Bounce!", "warning");
                                        return;
                                    }

                                    $scope.ISBOUNCE = false;
                                    $scope.BOUNCE = true;
                                    $scope.RETURN = false;
                                    $scope.disableEncash = true;
                                    $scope.ISRETURN = true;
                                    $scope.editPDC(row.RECEIPT_NO);
                                    break;

                                case "return":
                                    if (row.INTRANSIT_FLAG !== "Y") {
                                        displayPopupNotification("You cannot Return, proceed for InTransit!", "warning");
                                        return;
                                    }
                                    if (row.BOUNCE_FLAG === "Y") {
                                        displayPopupNotification("PDC is already Bounced!", "warning");
                                        return;
                                    }
                                    if (row.RETURN_FLAG === "Y") {
                                        displayPopupNotification("PDC is already Returned!", "warning");
                                        return;
                                    }
                                    if (row.ENCASH_DATE) {
                                        displayPopupNotification("Encashed PDC cannot be Return!", "warning");
                                        return;
                                    }
                                    $scope.ISRETURN = false;
                                    $scope.RETURN = true;
                                    $scope.disableEncash = true;
                                    $scope.ISBOUNCE = true;
                                    $scope.BOUNCE = false;
                                    $scope.editPDC(row.RECEIPT_NO);
                                    break;

                                case "delete":
                                    $scope.deletePDC(row.RECEIPT_NO);
                                    break;
                            }
                        });
                    }
                });
            }
        },

        columns: [
            {
                field: "RECEIPT_NO",
                title: "Receipt No",
                width: "100px"
            },
            {
                field: "RECEIPT_DATE",
                title: "Receipt Date",
                width: "120px",
                template: function (dataItem) {
                    return dataItem.RECEIPT_DATE ? kendo.toString(kendo.parseDate(dataItem.RECEIPT_DATE), 'dd-MMM-yyyy') : '';
                }
            },
            {
                field: "CHEQUE_NO",
                title: "Cheque No",
                width: "120px"
            },
            {
                field: "CHEQUE_DATE",
                title: "Cheque Date",
                width: "120px",
                template: function (dataItem) {
                    return dataItem.CHEQUE_DATE ? kendo.toString(kendo.parseDate(dataItem.CHEQUE_DATE), 'dd-MMM-yyyy') : '';
                }
            },
            {
                field: "ENCASH_DATE",
                title: "Encash Date",
                width: "120px",
                template: function (dataItem) {
                    return dataItem.ENCASH_DATE ? kendo.toString(kendo.parseDate(dataItem.ENCASH_DATE), 'dd-MMM-yyyy') : '';
                }
            },
            {
                field: "CUSTOMER_EDESC",
                title: "Customer",
                width: "200px"
            },
            {
                field: "PDC_AMOUNT",
                title: "PDC Amount",
                width: "120px",
                format: "{0:n2}"
            },
            {
                field: "BANK_NAME",
                title: "Bank Name",
                width: "150px"
            },
            {
                field: "PDC_DETAILS",
                title: "PDC Details",
                width: "150px"
            },
            {
                field: "REMARKS",
                title: "Remarks",
                width: "150px"
            },
            {
                field: "CHEQUE_TYPE",
                title: "Cheque Type",
                width: "100px"
            },
            {
                field: "MANUAL_NO",
                title: "Manual No",
                width: "100px"
            },
            {
                field: "VOUCHER_NO",
                title: "Voucher No",
                width: "100px"
            },
            {
                field: "BOUNCE_DATE",
                title: "Bounce Date",
                width: "120px",
                template: function (dataItem) {
                    return dataItem.BOUNCE_DATE ? kendo.toString(kendo.parseDate(dataItem.BOUNCE_DATE), 'dd-MMM-yyyy') : '';
                }
            },
            {
                field: "BOUNCE_REASON",
                title: "Bounce Reason",
                width: "150px"
            },
            {
                field: "VERIFIED_BY",
                title: "Verified By",
                width: "120px"
            },
            {
                field: "AUTHORIZED_BY",
                title: "Authorized By",
                width: "120px"
            },
        ]
    };

    // Payments Grid Options
    $scope.selectedPaymentPDC = null;
    $scope.PaymentsGridOptions = {
        dataSource: new kendo.data.DataSource({
            transport: {
                read: {
                    url: "/api/SetupApi/GetPaymentPDCList",
                    type: "POST",
                    dataType: "json",   
                    contentType: "application/json"
                },
                parameterMap: function (options, type) {
                    if (type === "read") {
                        let fromDate = $(".fromDate input").val();
                        let toDate = $(".toDate input").val();
                        let pdcType = $scope.PDC_TYPE_FILTER;
                        let chequeType = $scope.CHEQUE_TYPE_FILTER;

                        var combinedOptions = angular.extend({}, options, {
                            FROM_DATE: $scope.toDate(fromDate),
                            TO_DATE: $scope.toDate(toDate),
                            PDC_TYPE: pdcType,
                            CHEQUE_TYPE: chequeType
                        });

                        return JSON.stringify(combinedOptions);
                    }
                    return options;
                }
            },
            autoBind: false,
            schema: {
                data: "DATA",
            },
            serverPaging: true,
            serverSorting: true,
            serverFiltering: true,
            serverGrouping: false,
            pageSize: 10,
            sort: {
                field: "PAYMENT_NO",
                dir: "desc"
            }
        }),
        autoBind: false,
        selectable: "single",
        scrollable: true,
        height: 350,
        sortable: true,
        pageable: true,
        groupable: true,
        resizable: true,
        dataBound: function (e) {

            var grid = e.sender;

            // Change cursor
            grid.tbody.find("tr").css("cursor", "pointer");

            // Capture right-click row
            grid.tbody.find("tr").off("contextmenu").on("contextmenu", function () {
                $scope.selectedPaymentPDC = grid.dataItem(this);
            });

            // Initialize context menu once
            if (!$("#pdcpaymentContextMenu").data("kendoContextMenu")) {
                $("#pdcpaymentContextMenu").kendoContextMenu({
                    target: "#paymentsGrid",   //  MUST match grid ID
                    filter: "tbody tr",
                    showOn: "contextmenu",
                    select: function (e) {

                        var action = $(e.item).data("action");
                        var row = $scope.selectedPaymentPDC;

                        if (!row) return;

                        $scope.$apply(function () {
                            switch (action) {

                                case "encash":
                                    if (row.ENCASH_DATE) {
                                        displayPopupNotification("The PDC is already OutCashed by the Supplier!", "warning");
                                        return;
                                    }
                                    if (row.RETURN_FLAG === "Y") {
                                        displayPopupNotification("The Cheque is already Returned by Party!", "warning");
                                        return;
                                    }
                                    
                                    $scope.RETURNPAYMENT = false;
                                    $scope.showPaymentVoucherButton = true;
                                    $scope.IsPaymentEncashed = false;
                                    $scope.IsPaymentReturned = true;

                                    $scope.editPaymentPDC(row.PAYMENT_NO);
                                    break;
                                case "voucher":
                                    if (!row.ENCASH_DATE) {
                                        displayPopupNotification("PDC needs to be OutCashed to Generate Voucher!", "warning");
                                        return;
                                    }
                                    if (row.VOUCHER_NO) {
                                        displayPopupNotification("The Voucher is already Genrated!", "warning");
                                        return;
                                    }
                                    if (row.RETURN_FLAG === 'Y') {
                                        displayPopupNotification("You cannot Create Voucher of Returned Cheque!", "warning");
                                        return;
                                    }
                                    $scope.IsPaymentEncashed = true;
                                    $scope.IsPaymentReturned = true;
                                    $scope.showPaymentVoucherButton = false;
                                    $scope.editPaymentPDC(row.PAYMENT_NO);
                                    break;
                                case "return":
                                    if (row.ENCASH_DATE) {
                                        displayPopupNotification("The PDC is already OutCashed so you cannot Return!", "warning");
                                        return;
                                    }
                                    if (row.RETURN_FLAG === 'Y') {
                                        displayPopupNotification("The Cheque is already Returned by Party!", "warning");
                                        return;
                                    }
                                    
                                    $scope.RETURNPAYMENT = true;
                                    $scope.IsPaymentEncashed = true;
                                    $scope.IsPaymentReturned = false;
                                    $scope.showPaymentVoucherButton = true;
                                    $scope.editPaymentPDC(row.PAYMENT_NO);
                                    break;

                                case "delete":
                                    $scope.DeletePaymentPDC(row.PAYMENT_NO);
                                    break;
                            }
                        });
                    }
                });
            }
        },

        columns: [
            {
                field: "PAYMENT_NO",
                title: "Payment No",
                width: "100px"
            },
            {
                field: "PAYMENT_DATE",
                title: "Payment Date",
                width: "120px",
                template: function (dataItem) {
                    return dataItem.PAYMENT_DATE ? kendo.toString(kendo.parseDate(dataItem.PAYMENT_DATE), 'dd-MMM-yyyy') : '';
                }
            },
            {
                field: "CHEQUE_DATE",
                title: "Cheque Date",
                width: "120px",
                template: function (dataItem) {
                    return dataItem.CHEQUE_DATE ? kendo.toString(kendo.parseDate(dataItem.CHEQUE_DATE), 'dd-MMM-yyyy') : '';
                }
            },
            {
                field: "ENCASH_DATE",
                title: "OutCash Date",
                width: "120px",
                template: function (dataItem) {
                    return dataItem.ENCASH_DATE ? kendo.toString(kendo.parseDate(dataItem.ENCASH_DATE), 'dd-MMM-yyyy') : '';
                }
            },
            {
                field: "SUPPLIER_EDESC",
                title: "Supplier",
                width: "200px"
            },
            {
                field: "PDC_AMOUNT",
                title: "PDC Amount",
                width: "120px",
                format: "{0:n2}"
            },
            {
                field: "VOUCHER_NO",
                title: "Voucher No.",
                width: "80px"
            },
            {
                field: "PDC_DETAILS",
                title: "PDC Details",
                width: "150px"
            },
            {
                field: "BANK_NAME",
                title: "Bank Name",
                width: "150px"
            },
            {
                field: "REMARKS",
                title: "Remarks",
                width: "150px"
            }   
           
        ]
    };

    $timeout(function () {
        let startDate, endDate;
        let today = moment();
        endDate = today.toDate();
        
        // Default to this month
        let todayNepaliDateForMonth = ConvertEngDateToNep(moment().format('DD-MMM-YYYY'));
        let firstDayOfMonthNepali = todayNepaliDateForMonth.substring(0, 7) + '-01';
        startDate = BS2AD(firstDayOfMonthNepali);

        $("#englishdatedocument").data("kendoDatePicker").value(startDate);
        $("#englishdatedocument1").data("kendoDatePicker").value(endDate);
        $("#receiptEnglishDate").data("kendoDatePicker").value(endDate);
        $("#chequeEnglishDate").data("kendoDatePicker").value(endDate);
        $("#encashEnglishDate").data("kendoDatePicker").value(endDate);
        $("#bounceEnglishDate").data("kendoDatePicker").value(endDate);
        $("#createdEnglishDate").data("kendoDatePicker").value(endDate);
        $("#inTransitEnglishDate").data("kendoDatePicker").value(endDate);
        $("#returnEnglishDate").data("kendoDatePicker").value(endDate);
        $("#paymentEnglishDate").data("kendoDatePicker").value(endDate);
        $("#paymentChqueReturnEnglishDate").data("kendoDatePicker").value(endDate);
        $("#paymentCreatedChequeEnglishDate").data("kendoDatePicker").value(endDate);
        $("#paymentEncashChequeEnglishDate").data("kendoDatePicker").value(endDate);
        $("#paymentChequeEnglishDate").data("kendoDatePicker").value(endDate);

        let nepaliFromDate = ConvertEngDateToNep(moment(startDate).format('DD-MMM-YYYY'));
        let nepaliToDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliReceiptDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliChequeDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliEncashDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliBounceDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliCreatedDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliInTransitDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliReturnDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliChequeForPaymentDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliChequeReturnDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliChequeCreatedDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliChequeEncashDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));
        let nepaliChequePaymentDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));

        $("#nepaliDate5").val(nepaliFromDate);
        $("#nepaliDate51").val(nepaliToDate);
        $("#nepaliDateReceipt").val(nepaliReceiptDate);
        $("#nepaliDateForCheque").val(nepaliChequeDate);
        $("#encashNepaliDate").val(nepaliEncashDate);
        $("#bounceNepaliDate").val(nepaliBounceDate);
        $("#createdNepaliDate").val(nepaliCreatedDate);
        $("#inTransitNepaliDate").val(nepaliInTransitDate);
        $("#returnNepaliDate").val(nepaliReturnDate);
        $("#paymentNepaliDate").val(nepaliChequeForPaymentDate);
        $("#paymentChequeReturnNepaliDate").val(nepaliChequeReturnDate); 
        $("#paymentChequeCreatedNepaliDate").val(nepaliChequeCreatedDate);
        $("#paymentChequeEncashNepaliDate").val(nepaliChequeEncashDate);
        $("#paymentChequeNepaliDate").val(nepaliChequePaymentDate);
        
        $scope.BindPDCGrid();
        $scope.BindPDCPaymentGrid();
    }, 200)


 
    $scope.showAddPaymentModal = function (event) {
        $scope.showPaymentVoucherButton = true;
        $scope.PAYMENTEDITMODE = false;
        $scope.IsPaymentEncashed = true;

        let nepaliToday = ConvertEngDateToNep(moment().format('DD-MMM-YYYY'));
        $("#paymentNepaliDate").val(nepaliToday);
        $("#paymentChequeNepaliDate").val(nepaliToday);

        var date = BS2AD(nepaliToday);
        $scope.pdcModel.ENCASH_DATE = date;
        $("#paymentEnglishDate").data("kendoDatePicker").value(moment(date).format("DD-MMM-YYYY"));
        $("#paymentChequeEnglishDate").data("kendoDatePicker").value(moment(date).format("DD-MMM-YYYY"));

        $scope.generateNewPaymentNo();
        $("#pdcPaymentEntryModal").modal({
            backdrop: 'static',
            keyboard: false
        });
    }


    $scope.OpenAddPDCModal = function(){
        if ($scope.showReceiptAddButton) {
            $scope.showAddReceiptModal();
        }
        if ($scope.showPaymentAddButton) {
            $scope.showAddPaymentModal();
        }
    }

    $scope.PrintPDC = function(){
        if ($scope.showReceiptAddButton) {
            $scope.PrintReceiptPDC();
        }
        if ($scope.showPaymentAddButton) {
            $scope.PrintPaymentPDC();
        }
    }


    $scope.showAddReceiptModal = function () {
        $scope.resetPdcModel();
        let nepaliToday = ConvertEngDateToNep(moment().format('DD-MMM-YYYY'));
        $("#nepaliDateReceipt").val(nepaliToday);
        $("#nepaliDateForCheque").val(nepaliToday);

        var date = BS2AD(nepaliToday);
        $scope.pdcModel.ENCASH_DATE = date;
        $("#receiptEnglishDate").data("kendoDatePicker").value(moment(date).format("DD-MMM-YYYY"));
        $("#chequeEnglishDate").data("kendoDatePicker").value(moment(date).format("DD-MMM-YYYY"));
        $scope.generateNewReceiptNo();
        //$("#createPDCModal").modal('show'); // Open modal
        $("#createPDCModal").modal({
            backdrop: 'static',
            keyboard: false
        });
    };

    $scope.hideModal = function () {
        $("#createPDCModal").modal('hide'); // Close modal
    };

    $scope.hideModal = function () {
        $("#pdcPaymentEntryModal").modal('hide'); // Close modal
    };

    $scope.deletePDC = function (receiptNo) {
        if (!receiptNo) return;

        showUniversalPopup({
            title: "Delete PDC Receipt",
            message: `Are you sure you want to delete this PDC Receipt? (<strong>${receiptNo}</strong>)`,
            confirmText: "Delete",
            confirmColor: "#dc3545",
            onConfirm: function () {
                $http.delete('/api/SetupApi/DeletePDC', { params: { receiptNo: receiptNo } }).then(function (res) {
                    if (typeof displayPopupNotification === 'function') displayPopupNotification('Deleted', 'success');
                    $scope.BindPDCGrid();
                    var grid = $("#pdcGrid").data("kendoGrid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                }, function () {
                    if (typeof displayPopupNotification === 'function') displayPopupNotification('Delete failed', 'error');
                });
            }
            });
    };


    

    $scope.PrintReceiptPDC = function () {
        var grid = $("#pdcGrid").data("kendoGrid");
        if (!grid) {
            console.error("Kendo Grid instance not found.");
            return;
        }

        var data = grid.dataSource.data();

        if (data.length === 0) {
            displayPopupNotification("No data available to print.", "info");
            return;
        }

        var allColumns = [
            { field: "RECEIPT_NO", title: "Receipt No" },
            { field: "RECEIPT_DATE", title: "Receipt Date" },
            { field: "CHEQUE_NO", title: "Cheque No" },
            { field: "CHEQUE_DATE", title: "Cheque Date" },
            { field: "CUSTOMER_EDESC", title: "Customer" },
            { field: "PDC_AMOUNT", title: "Amount" },
            { field: "BANK_NAME", title: "Bank" },
            { field: "PDC_DETAILS", title: "Details" },
            { field: "REMARKS", title: "Remarks" }
        ];

        var printContent = `
            <!DOCTYPE html>
            <html>
            <head>
                <title>Post Date Cheque Receipt Report</title>
                <style>
                    body { font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; font-size: 10px; }
                    table { width: 100%; border-collapse: collapse; }
                    th, td { border: 1px solid #ccc; padding: 6px; text-align: left; }
                    th { background-color: #f2f2f2; }
                    .report-header { text-align: center; margin-bottom: 20px; }
                    .footer { display: flex; justify-content: space-between; margin-top: 20px; font-size: 8px; }
                </style>
            </head>
            <body>
                <h2 class='report-header'>Post Date Cheque Receipt Report</h2>
                <table>
                    <thead>
                        <tr>
                            ${allColumns.map(col => `<th>${kendo.htmlEncode(col.title)}</th>`).join('')}
                        </tr>
                    </thead>
                    <tbody>
                        ${data.map(item => `
                            <tr>
                                ${allColumns.map(col => {
                                    let value = item[col.field] || '';
                                    if (col.field.includes('DATE') && value) {
                                        value = kendo.toString(kendo.parseDate(value), 'dd-MMM-yyyy');
                                    }
                                    return `<td>${kendo.htmlEncode(value)}</td>`;
                                }).join('')}
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
                <div class='footer'>
                    <span>Printed on ${new Date().toLocaleDateString()} at ${new Date().toLocaleTimeString()}</span>
                    <span>Total Records: ${data.length}</span>
                </div>
            </body>
            </html>
        `;

        var printWindow = window.open("", "_blank");

        if (!printWindow) {
            alert("Pop-up window blocked. Please enable pop-ups for this site.");
            return;
        }

        printWindow.document.open();
        printWindow.document.write(printContent);
        printWindow.document.close();

        printWindow.onafterprint = function () {
            printWindow.close();
        };

        printWindow.focus();
        printWindow.print();
    };


    $scope.PrintPaymentPDC = function () {
        var grid = $("#paymentsGrid").data("kendoGrid");
        if (!grid) {
            console.error("Kendo Grid instance not found.");
            return;
        }

        var data = grid.dataSource.data();

        if (data.length === 0) {
            displayPopupNotification("No data available to print.", "info");
            return;
        }

        var allColumns = [
            { field: "PAYMENT_NO", title: "Payment No" },
            { field: "PAYMENT_DATE", title: "Payment Date" },
            { field: "CHEQUE_NO", title: "Cheque No" },
            { field: "CHEQUE_DATE", title: "Cheque Date" },
            { field: "SUPPLIER_EDESC", title: "Supplier" },
            { field: "PDC_AMOUNT", title: "Amount" },
            { field: "BANK_NAME", title: "Bank" },
            { field: "PDC_DETAILS", title: "Details" },
            { field: "REMARKS", title: "Remarks" }
        ];

        var printContent = `
            <!DOCTYPE html>
            <html>
            <head>
                <title>Post Date Cheque Payment Report</title>
                <style>
                    body { font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; font-size: 10px; }
                    table { width: 100%; border-collapse: collapse; }
                    th, td { border: 1px solid #ccc; padding: 6px; text-align: left; }
                    th { background-color: #f2f2f2; }
                    .report-header { text-align: center; margin-bottom: 20px; }
                    .footer { display: flex; justify-content: space-between; margin-top: 20px; font-size: 8px; }
                </style>
            </head>
            <body>
                <h2 class='report-header'>Post Date Cheque Payment Report</h2>
                <table>
                    <thead>
                        <tr>
                            ${allColumns.map(col => `<th>${kendo.htmlEncode(col.title)}</th>`).join('')}
                        </tr>
                    </thead>
                    <tbody>
                        ${data.map(item => `
                            <tr>
                                ${allColumns.map(col => {
            let value = item[col.field] || '';
            if (col.field.includes('DATE') && value) {
                value = kendo.toString(kendo.parseDate(value), 'dd-MMM-yyyy');
            }
            return `<td>${kendo.htmlEncode(value)}</td>`;
        }).join('')}
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
                <div class='footer'>
                    <span>Printed on ${new Date().toLocaleDateString()} at ${new Date().toLocaleTimeString()}</span>
                    <span>Total Records: ${data.length}</span>
                </div>
            </body>
            </html>
        `;

        var printWindow = window.open("", "_blank");

        if (!printWindow) {
            alert("Pop-up window blocked. Please enable pop-ups for this site.");
            return;
        }

        printWindow.document.open();
        printWindow.document.write(printContent);
        printWindow.document.close();

        printWindow.onafterprint = function () {
            printWindow.close();
        };

        printWindow.focus();
        printWindow.print();
    };

});
