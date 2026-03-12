DTModule.controller('bankGuranteeCtrl', function ($scope, $filter,$http,bankGuranteeService) {
    console.log("bankGuranteeCtrl controller : ");

    $scope.FormName = "Bank Gurantee";

    $scope.BankGuranteeAction="Save";

    $scope.bankGuranteeObj ={
       VOUCHER_NO:"",
       LOG_DATE_ENGLISH:"",
        LOG_DATE_NEPALI: "",
        CUSTOMER_NAME: "",
        SUPPLIER_NAME:"",
       PARTY_NAME: "",
        CS_CODE: "",
        CS_FLAG: "",
       PARTY_TYPE_CODE:"",
       ISSUING_BANK_ACCOUNT:"",
       ADDRESS:"",
       BANK_GURANTEE_NO:"",
       DEALER: "",
       DEALER_CODE:"",
       BG_AMOUNT:"",
       START_DATE_ENGLISH:"",
       START_DATE_NEPALI:"",
       EXPIRY_DATE_ENGLISH:"",
       EXPIRY_DATE_NEPALI:"",
       REMARKS:"",
       ALERT_PRIOR_DAYS:"",
       EXPIRE: false,
       EXPIRY: "",
       FILTERS:""
   };



$scope.bankGuranteeGridOptions = {
    dataSource: new kendo.data.DataSource({
        transport: {
            read: {
                method: "GET",
                url: "/api/CustomFormApi/GetBankGuranteeList",
                dataType: "json"
            },

        },
        pageSize: 10,

        }),
        scrollable: true,
        pageable: true,
        selectable: true,
        dataBound: function (e) {
            $("#bankGuranteeGrid tbody tr").css("cursor", "pointer");
            var grid = e.sender;
            if (grid.dataSource.total() === 0) {
                var colCount = grid.columns.length + 1;
                $(e.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');
            }

        },
        columns: [
            {
                field: "BG_NO", title: "BG. No.", width: 50
            },
            {
                field: "PARTY_NAME", title: "Party Name", width: 70,
                //template: "#= kendo.toString(kendo.parseDate(RECEIPT_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
            },
            { field: "ADDRESS", title: "Address", width: 60 },
            //{
            //    field: "DEALER_NAME", title: "Dealer Name", width: 60,
            //    //template: "#= kendo.toString(kendo.parseDate(CHEQUE_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
            //},
            //{
            //    field: "AREA", title: "Area", width: 60,
            //    //template: "#= kendo.toString(kendo.parseDate(ENCASH_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
            //},
            { field: "BG_AMOUNT", title: "BG Amount", width: 60 },
            //{ field: "ISSUING_BANK", title: "Issuing Bank", width: 80 },
            { field: "ACC_CODE", title: "Issuing Bank", width: 80 },
            {
                field: "START_DATE", title: "Issue Date", width: 80,
                template: "#= kendo.toString(kendo.parseDate(START_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
            },
            {
                field: "END_DATE", title: "Validity Date", width: 80,
                template: "#= kendo.toString(kendo.parseDate(END_DATE, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
            },
            { field: "EXPAIRY_DUE_DAYS", title: "Expiry Due Days", width: 80 },
            { field: "BG_AMOUNT", title: "Total Security", width: 80 },
            { field: "REMARKS", title: "Remarks", width: 80 }
            ,
            {
                title: "Action ",
                template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="EditBankGuranteeDetail(dataItem.BG_NO)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="Delete" ng-click="DeleteBankGuranteeDetail(dataItem.BG_NO)"><span class="sr-only"></span> </a>',
                width: "60px"
            }

        ],
    };


$scope.DeleteBankGuranteeDetail = function (pdcId) {
        bootbox.confirm({
            title: "Delete",
            message: "Are you sure?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success',
                    label: '<i class="fa fa-check"></i> Yes',
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger',
                    label: '<i class="fa fa-times"></i> No',
                }
            },
            callback: function (result) {

                if (result == true) {

                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/DeletePDCFormDetail?pdcId=" + pdcId;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {

                        if (response.data == "Successful") {
                            var grid = $("#bankGuranteeGrid").data("kendoGrid");
                            grid.dataSource.read();
                            bootbox.hideAll();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                    }, function errorCallback(response) {
                        $scope.refresh();
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                    });

                }
                else if (result == false) {


                    bootbox.hideAll();
                }

            }
        });
    };


    $scope.EditBankGuranteeDetail = function (pdcId) {
        debugger;
        $scope.BankGuranteeAction = "Update";
        //console.log("i am clicked");
        var editPdcdUrl = window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/EditPDCFormDetail?pdcId=" + pdcId + '&pdcStatus=encash';
        $http({
            method: 'GET',
            url: editPdcdUrl,

        }).then(function successCallback(response) {
            debugger;
            console.log("Response To Edit===========>>" + JSON.stringify(response));
            $scope.PDCFormObject = response.data;
          
            $("#bankGuranteeModal").toggle();
          //  $scope.PDCFormObject.IS_UPDATE = true;
            //$scope.PDCFormObject.CUSTOMER_CODE = { "TYPE_CODE": response.data.CUSTOMER_CODE };
           // $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY = {"TYPE_CODE":response.data.MONEY_RECEIPT_ISSUED_BY};
           // $scope.PDCFormObject.PARTY_TYPE = { "PARTY_TYPE_CODE": response.data.PARTY_TYPE };
           // $scope.PDCFormObject.REMINDER_PRIOR_DAYS = { "id": response.data.REMINDER_PRIOR_DAYS };
          

        }, function errorCallback(response) {
            displayPopupNotification(response.data.STATUS_CODE, "error");

        });
    };

 $scope.newBankGurantee = function () {
     $scope.generateNewVoucherNo();
     $scope.bankGuranteeModal = "Save";
     $scope.someDateFn();
     $("#bankGuranteeModal").toggle();
};

    $scope.closeBankGuranteeDialog = function () {
        $("#SupName").prop("disabled", false);
        $("#CustName").prop("disabled", false); 
     $("#bankGuranteeModal").toggle();
     $scope.bankGuranteeObj = {};
   };

  $scope.saveBankGurantee=function(){

     //console.log("bankGuranteeObj===>>>>>",JSON.stringify($scope.bankGuranteeObj));

        //$scope.PDCFormObject.CUSTOMER_CODE = $scope.PDCFormObject.CUSTOMER_CODE.TYPE_CODE;
        //$scope.PDCFormObject.PARTY_TYPE = $scope.PDCFormObject.PARTY_TYPE.PARTY_TYPE_CODE; 
        //$scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY = $scope.PDCFormObject.MONEY_RECEIPT_ISSUED_BY.TYPE_CODE; 
        console.log("PDC FORM OBJECT ON SAVe=====>>>>" + JSON.stringify($scope.bankGuranteeObj));
        debugger;
        $scope.bankGuranteeObj.PARTY_TYPE_CODE = $scope.bankGuranteeObj.PARTY_NAME.PARTY_TYPE_CODE
      $scope.bankGuranteeObj.DEALER_CODE = $scope.bankGuranteeObj.DEALER.PARTY_TYPE_CODE
      var closeflag = "";
      if ($scope.bankGuranteeObj.EXPIRE == true) {
          closeflag = "Y";
      }
      else {
          closeflag = "N";
      }

      var model = {
          BG_NO: $scope.bankGuranteeObj.VOUCHER_NO,
          BG_DATE: $scope.bankGuranteeObj.LOG_DATE_ENGLISH,
          CS_CODE: $scope.bankGuranteeObj.PARTY_NAME.PARTY_TYPE_CODE,
          CUSTOMER_CODE: $scope.bankGuranteeObj.CUSTOMER_NAME,
          SUPPLIER_CODE: $scope.bankGuranteeObj.SUPPLIER_NAME,
          ACC_CODE: $scope.bankGuranteeObj.ISSUING_BANK_ACCOUNT,
          BANK_GNO: $scope.bankGuranteeObj.BANK_GURANTEE_NO,
          START_DATE_ENGLISH: $scope.bankGuranteeObj.START_DATE_ENGLISH,
          EXPIRY_DATE_ENGLISH: $scope.bankGuranteeObj.EXPIRY_DATE_ENGLISH,
          BG_AMOUNT: $scope.bankGuranteeObj.BG_AMOUNT,
          REMARKS: $scope.bankGuranteeObj.REMARKS,
          CLOSE_FLAG: closeflag,
          ALERT_PRIOR_DAYS: $scope.PDCFormObject.REMINDER_PRIOR_DAYS,
          ADDRESS: $scope.bankGuranteeObj.ADDRESS,
          CS_FLAG: $scope.bankGuranteeObj.CS_FLAG,
          PARTY_NAME:$scope.bankGuranteeObj.DEALER.PARTY_TYPE_CODE,

      }
        //var saveResponse = bankGuranteeService.saveNewBankGurantee($scope.bankGuranteeObj);
      var saveResponse = bankGuranteeService.saveNewBankGurantee(model);
       
        saveResponse.then(function (savRes) {
            if (savRes.data === "Successful") {
                $("#SupName").prop("disabled", false);
                $("#CustName").prop("disabled", false);
                if ($scope.BankGuranteeAction == "UPDATE") DisplayBarNotificationMessage("Bank Gurantee updated successfully");
                else DisplayBarNotificationMessage("Bank Gurantee saved successfully");
                setTimeout(function () {
                    location.reload(true);
                },3000);
            } else {
                displayPopupNotification("Error while saving bank gurantee", "error");
            }
        }); 

   };

  $scope.someDateFn = function () {

        var engdate = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 1)), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        $scope.Dispatch_From = engdate;
        $scope.bankGuranteeObj.LOG_DATE_ENGLISH = engdate;
        $scope.bankGuranteeObj.START_DATE_ENGLISH = engdate;
        $scope.bankGuranteeObj.EXPIRY_DATE_ENGLISH = engdate;
        $scope.bankGuranteeObj.LOG_DATE_NEPALI = a;
        $scope.bankGuranteeObj.START_DATE_NEPALI = a;
        $scope.bankGuranteeObj.EXPIRY_DATE_NEPALI = a;
        $scope.Dispatch_To = a;
        $scope.PlanningTo = ConvertEngDateToNep($filter('date')(new Date(new Date().setDate(new Date().getDate())), 'dd-MMM-yyyy'));
      //  $scope.PlanningDate = a;

    };

  $scope.someDateFn();

  $scope.getAllPartyType = function () {
        var allPartyType = bankGuranteeService.getAllPartyType();
        allPartyType.then(function (partyRes) {
            $scope.AllPartyType = partyRes.data;
        });
    };
    $scope.getAllPartyType();

    $scope.getAllCustomer = function () {
        var allCustomer = bankGuranteeService.getAllCustomer();
        allCustomer.then(function (customerRes) {
            $scope.allCustomer = customerRes.data;
        });
    };
    $scope.getAllCustomer();

    $scope.getAllSupplier = function () {
        var allSupplier = bankGuranteeService.getAllSupplier();
        allSupplier.then(function (SupplierRes) {
            $scope.allSupplier = SupplierRes.data;
        });
    };
    $scope.getAllSupplier();

    $scope.getAllDealerType = function () {
        var allDealerType = bankGuranteeService.getAllDealerType();
        allDealerType.then(function (dealerRes) {
            $scope.AllDealerType = dealerRes.data;
        });
    };

    $scope.getAllDealerType();

    $scope.partyChanged = function () {
        $scope.bankGuranteeObj.ADDRESS = $scope.bankGuranteeObj.PARTY_NAME.ADDRESS;
    };
    $scope.customerChanged = function () {
        debugger;
        $("#SupName").prop("disabled", true);
        $scope.bankGuranteeObj.CS_FLAG = "C";
       
    };
    $scope.supplierChanged = function () {
        debugger;
        $("#CustName").prop("disabled", true);
        $scope.bankGuranteeObj.CS_FLAG = "S";
    };


    $scope.generateNewVoucherNo = function () {
        var newVoucherNo = bankGuranteeService.generateNewVoucherNo();
        newVoucherNo.then(function (nrRes) {
            $scope.bankGuranteeObj.VOUCHER_NO = nrRes.data[0];
        });
    };

});

DTModule.service('bankGuranteeService', function ($http) {

 this.getAllPartyType = function () {
        var allPT = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllPartyType",
            dataType: "json"
        });
        return allPT;
    };
    this.getAllCustomer = function () {
        var allC = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllCustomer",
            dataType: "JSON"
        });
        return allC;
    };

    this.getAllSupplier = function () {
        var allS = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllSupplier",
            dataType: "json"
        });
        return allS;
    };

  this.getAllDealerType = function () {
        var allDlr = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllDealerType",
            dataType: "json"
        });
        return allDlr;
    };

  this.saveNewBankGurantee = function (saveParam) {
        var saveRes = $http({
            method: "POST",
            url: "/api/CustomFormApi/SaveNewBankGuaranteeForm",
            data: JSON.stringify(saveParam),
            dataType: "json"
        });
        return saveRes;
  };

  this.generateNewVoucherNo = function () {
      var allCOA = $http({
          method: "GET",
          url: "/api/CustomFormApi/GenerateNewBGVoucher",
          dataType: "json"
      });
      return allCOA;
  };


});