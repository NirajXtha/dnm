// Code goes here
var app = angular.module('myApp', ['kendo.directives', 'ngMessages','datatables']);

app.controller('LCController', function ($scope, $http, $filter, $q, LOCService) {

    //$scope.GetBanks();// = [];
    init();

    $scope.LcBankNames = null;

    //Start Add LC Setups //
    $scope.addlcbank = function (isValid) {
       
        if (isValid) {
            var loadlcbanks = window.location.protocol + "//" + window.location.host + "/api/LCSetup/CreateLCBank";

            var lcbank = {
                BankName: $scope.bankname,
                Address: $scope.address,
                BankCode: $scope.lcbankcode
            };
            var response = $http({
                method: "post",
                url: loadlcbanks,
                data: JSON.stringify(lcbank),
                dataType: "json"
            });

            return response.then(function (data) {
                if (data.data.MESSAGE == "Success") {
                    $scope.bankname = "";
                    $scope.address = "";
                    $scope.lcbankcode = 0;
                    displayPopupNotification("Succesfully Saved LC Bank.", "success");
                    init().LOCService.getallLCBanks();
                }
                else {
                    displayPopupNotification("Error Occured.", "error");

                }

            });

        }
        else {
            toastr.error("Fields cannot be empty.");
        }

    };
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
                    init().LOCService.getallTerms();
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
                    init().LOCService.getallPTerms();
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
                    init().LOCService.getallStatus();
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
    //End Add LC Setups //

    function init() {
        LOCService.getallLCBanks().then(function (d) {
            $scope.LcBankNames = d.data;

        }, function () { alert("Error at: Get All LC Banks") });
        function isEmpty(obj) {
            return Object.keys(obj).length === 0;
        }

        LOCService.getallTerms().then(function (d) {
            $scope.LcTermNames = d.data;

        }, function () { alert("Error at: Get All LC Terms") });
        function isEmpty(obj) {
            return Object.keys(obj).length === 0;
        }

        LOCService.getallPTerms().then(function (d) {
            $scope.LcPTermNames = d.data;

        }, function () { alert("Error at: Get All LC Terms") });
        function isEmpty(obj) {
            return Object.keys(obj).length === 0;
        }

        LOCService.getallStatus().then(function (d) {
            $scope.LcStatusNames = d.data;

        }, function () { alert("Error at: Get All LC Status") });
        function isEmpty(obj) {
            return Object.keys(obj).length === 0;
        }
    }

    //Start Get LC Setups by Setups ID //
    $scope.getBanksByBankCode = function (bankdetail) {
        $scope.bankname = bankdetail.BankName;
        $scope.address = bankdetail.Address;
        $scope.lcbankcode = bankdetail.BankCode;
    }
    $scope.getTermsByTermCode = function (termdetail) {
        $scope.termname = termdetail.TermName;
        $scope.lctermcode = termdetail.TermCode;
    }
    $scope.getPTermsByPTermCode = function (ptermdetail) {
        $scope.ptermname = ptermdetail.PTermName;
        $scope.lcptermcode = ptermdetail.PTermCode;
    }
    $scope.getStatusByStatusCode = function (statusdetail) {
        $scope.lcstatusname = statusdetail.StatusName;
        $scope.lcstatuscode = statusdetail.StatusCode;
    }
    //End Get LC Setups by Setups ID //

    //Start Delete LC Setups //
    $scope.deleteByBankCode = function (BankCode) {
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result) {
                LOCService.deleteByBankCode(BankCode).then(function (data) {
                    displayPopupNotification("Succesfully Deleted LC Status.", "success");
                    init().LOCService.getallLCBanks();

                }, function (e) {

                });
            }
        });

    }
    $scope.deleteByTermCode = function (TermCode) {
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result) {
                LOCService.deleteByTermCode(TermCode).then(function (data) {
                    toastr.success("Succesfully Deleted LC Terms");
                    init().LOCService.getallTerms();

                }, function (e) {

                });
            }
        });

    }
    $scope.deleteByPTermCode = function (PTermCode) {
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result) {
                LOCService.deleteByPTermCode(PTermCode).then(function (data) {
                    toastr.success("Succesfully Deleted LC PAYMENT TERMS");
                    init().LOCService.getallPTerms();

                }, function (e) {

                });
            }
        });

    }
    $scope.deleteByStatusCode = function (StatusCode) {
        bootbox.confirm("Are you sure you want to delete?", function (result) {
            if (result) {
                LOCService.deleteByStatusCode(StatusCode).then(function (data) {
                    toastr.success("Succesfully Deleted Status.");
                    init().LOCService.getallStatus();

                }, function (e) {

                });
            }
        });

    }
    //End Delete LC Setups //

    //Start Cancel LC Setups //
    $scope.cancelbank = function () {
        $scope.bankname = "";
        $scope.address = "";
        $scope.lcbankcode = 0;
    }

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
    //End Cancel LC Setups //
});

app.service('LOCService', function ($http) {
    var fac = {};
    fac.getallLCBanks = function () {
        return $http.get('/api/LCSetup/GetAllBanks');
    }
    //fac.getBanksByBankCode = function (BankCode) {
    //    return $http({
    //        url: '/api/LCSetup/GetBankByBankCode',
    //        method: "GET",
    //        params: { BankCode: BankCode }
    //    });
    //}
    fac.deleteByBankCode = function (BankCode) {
        return $http({
            url: '/api/LCSetup/DeleteLCBanks',
            method: "GET",
            params: { BankCode: BankCode }
        });
    }
    fac.getallTerms = function () {
        return $http.get('/api/LCSetup/GetAllTerms');
    }
    fac.getallPTerms = function () {
        return $http.get('/api/LCSetup/GetAllPTerms');
    }
    fac.getallStatus = function () {
        return $http.get('/api/LCSetup/GetAllStatus');
    }
    //fac.getTermsByTermCode = function (TermCode) {
    //    return $http({
    //        url: '/api/LCSetup/GetTermByTermCode',
    //        method: "GET",
    //        params: { TermCode: TermCode }
    //    });
    //}

    //fac.getPTermsByPTermCode = function (PTermCode) {
    //    return $http({
    //        url: '/api/LCSetup/GetPTermByPTermCode',
    //        method: "GET",
    //        params: { PTermCode: PTermCode }
    //    });
    //}
    //fac.getStatusByStatusCode = function (StatusCode) {
    //    return $http({
    //        url: '/api/LCSetup/GetStatusByStatusCode',
    //        method: "GET",
    //        params: { StatusCode: StatusCode }
    //    });
    //}
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
    return fac;

});

//LOC Performa Invoice Entry 
app.controller('PerformaInvoiceCtrl', function ($scope, $http, PerformaInvoiceService) {
    $scope.performaInvoiceList = [];
    $scope.PerformaInvoice = [
    {
        SUPPLIER: '',
        BRAND: '',
        CURRENCY: '',
        PFI_NUMBER: '',
        PFI_DATE: '',
        TOTAL_QTY: '',
        AMOUNT: '',
        ATTACHEMENT:''
    }];
    $scope.Action = 'Add New';
    $scope.saveAction = 'Save';
    $scope.showFormPanel = false;
    $scope.selectedCurrency = [];
    $scope.selectedBrand = [];
    $scope.selectedSupplier = [];
    $scope.SUPPLIER = [
    {
        SUPPLIER_CODE: '',
        SUPPLIER_EDESC: ''
    }];
    $scope.BRAND = [
        {
            ITEM_CODE: '',
            ITEM_EDESC: ''
        }
    ];
    $scope.CURRENCY = [
    {
        
    }];
    getAllPerformaInvoice();
    loadCurrency();
    loadSupplier();
    loadBrand();
    function getAllPerformaInvoice() {
        var getAllList = PerformaInvoiceService.GetAllList();
        getAllList.then(function (data) {
            $scope.performaInvoiceList = data.data;
        }, function () {

        });
    }
    
    function loadCurrency() {
        var getAllList = PerformaInvoiceService.loadCurrency();
        getAllList.then(function (data) {
            $scope.CURRENCY = data.data;
        }, function () {

        });
    }

    function loadSupplier() {
        var getAllList = PerformaInvoiceService.loadSupplier();
        getAllList.then(function (data) {
            $scope.SUPPLIER = data.data;
        }, function () {

        });
    }

    function loadBrand() {
        var getAllList = PerformaInvoiceService.loadBrand();
        getAllList.then(function (data) {
            $scope.BRAND = data.data;
        }, function () {

        });
    }

    $scope.addNew= function(action) {
        if (action === 'Add New') {
            $scope.PerformaInvoice = '';
            $scope.showFormPanel = true;
            $scope.Action = "Cancel";
            $scope.saveAction = 'Save';
        } else {
             $scope.showFormPanel = false;
            $scope.Action = "Add New";
        }

    }
    $scope.AddUpdatePerformaInvoice = function (isValid) {
        if (isValid) {
            var getAction = $scope.saveAction;
            if (getAction === "Update") {
                updatePerformaInvoice($scope.PerformaInvoice);
            } else if (getAction === "Save") {
                savePerformaInvoice($scope.PerformaInvoice);
                getAllPerformaInvoice();
            } else {
                savePerformaInvoice();
                $scope.showFormPanel = true;
            }
            getAllPerformaInvoice();
        }
        else {
            toastr.error("There is empty field.");
        }
    }

    function updatePerformaInvoice(item) {
        var result = PerformaInvoiceService.updatePerformaInvoice(item);
        result.then(function (data) {
            $scope.saveAction = 'Save';
        }, function () {

        });
    }

    function savePerformaInvoice(item) {
        var performaInvoice = {
            PFI_DATE: item.PFI_DATE,
            CURRENCY_CODE: item.selectedCurrency,
            AMOUNT: item.AMOUNT,
            TOTAL_QTY: item.TOTAL_QTY,
            ITEM_CODE: item.selectedBrand,
            SUPPLIER_CODE: item.selectedSupplier,
            ATTACHEMENT: item.ATTACHEMENT

        };
        var savePerformaInvoice = PerformaInvoiceService.savePerformaInvoice(performaInvoice);
        savePerformaInvoice.then(function (data) {

        },
            function () {

            });

    }

    $scope.addPerformaInvoice = function (item) {
        var performaInvoice = {
            PFI_DATE: item.PFI_DATE,
            CURRENCY_CODE: item.selectedCurrency,
            AMOUNT: item.AMOUNT,
            TOTAL_QTY: item.TOTAL_QTY,
            BRAND_CODE: item.selectedBrand,
            SUPPLIER_CODE: item.selectedSupplier,
            ATTACHEMENT: item.ATTACHEMENT

        };
        var savePerformaInvoice = PerformaInvoiceService.savePerformaInvoice(performaInvoice);
        savePerformaInvoice.then(function (data) {
            $scope.PerformaInvoice = '';
            $scope.showFormPanel = false;

        },
            function () {

            });
    }
    $scope.SaveNContinueePerformaInvoice = function() {
        var performaInvoice = {
            PFI_DATE: item.PFI_DATE,
            CURRENCY_CODE: item.selectedCurrency,
            AMOUNT: item.AMOUNT,
            TOTAL_QTY: item.TOTAL_QTY,
            BRAND_CODE: item.selectedBrand,
            SUPPLIER_CODE: item.selectedSupplier,
            ATTACHEMENT: item.ATTACHEMENT

        };
        var savePerformaInvoice = PerformaInvoiceService.savePerformaInvoice(performaInvoice);
        savePerformaInvoice.then(function (data) {

        },
            function () {

            });
    }
    $scope.CancelPerformaInvoiceForm = function () {
        $scope.PerformaInvoice = '';
        $scope.showFormPanel = false;
    }
    $scope.ResetPerformaInvoiceForm = function () {
         $scope.PerformaInvoice = '';
    }
    $scope.EditPerformaInvoice = function (PerformaInvoice) {
        var getByID = PerformaInvoiceService.GetPerformaInvoiceByID(PerformaInvoice.PFI_CODE);
        getByID.then(function (data) {
            $scope.PerformaInvoice = data.data;
            $scope.PerformaInvoice.PFI_DATE = PerformaInvoice.PFI_DATE;
            $scope.PerformaInvoice.PFI_NUMBER = PerformaInvoice.PFI_NUMBER;
            $scope.PerformaInvoice.selectedCurrency = PerformaInvoice.CURRENCY_CODE;
            $scope.PerformaInvoice.AMOUNT = PerformaInvoice.AMOUNT;
            $scope.PerformaInvoice.TOTAL_QTY = PerformaInvoice.TOTAL_QTY;
            $scope.PerformaInvoice.selectedBrand = PerformaInvoice.ITEM_CODE;
            $scope.PerformaInvoice.selectedSupplier = PerformaInvoice.SUPPLIER_CODE ;
            $scope.PerformaInvoice.ATTACHEMENT = PerformaInvoice.ATTACHEMENT;
            $scope.saveAction = 'Update';
            $scope.showFormPanel = true;
            $scope.Action = "Cancel";
        }, function () {

        });
    }
    $scope.deletePerformaInvoice = function (pfinum) {
         var result = PerformaInvoiceService.deletePerformaInvoice(pfinum);
        result.then(function (data) {

        },
            function () {

            });
    }
});
app.service('PerformaInvoiceService', function ($http, $q, $timeout) {
    this.GetAllList = function () {
        return $http.get("/api/LocPerformaInvoice/getAllPerformaInvoice");
    }
    this.GetPerformaInvoiceByID = function (id) {
        var response = $http({
            method: "post",
            url: "/api/LocPerformaInvoice/getPerformaInvoiceByID",
            params: {
                id: JSON.stringify(id)
            },
            dataType: "json"
        });
        return response;
    }
    this.loadCurrency = function () {
        return $http.get("/api/LocPerformaInvoice/getCurrencyType");
    }
    this.loadSupplier = function () {
        return $http.get("/api/LocPerformaInvoice/getSuplierType");
    }
    this.loadBrand = function () {
        return $http.get("/api/LocPerformaInvoice/getBrandType");
    }
    this.savePerformaInvoice = function (invoice) {
        var response = $http({
            method: "post",
            url: "/Api/LocPerformaInvoice/SavePerformaInvoice",
            data: JSON.stringify(invoice),
            dataType: "json"
        });
        return response;
    }
    this.updatePerformaInvoice = function (invoice) {
        var response = $http({
            method: "post",
            url: "/Api/LocPerformaInvoice/UpdatePerformaInvoice",
            data: JSON.stringify(invoice),
            dataType: "json"
        });
        return response;
    }
    this.deletePerformaInvoice = function (pfinum) {
        var response = $http({
            method: "post",
            url: "/Api/LocPerformaInvoice/DeletePerformaInvoice",
            data: JSON.stringify(pfinum),
            dataType: "json"
        });
        return response;
    }

});


var app1 = angular.module('my-app', ['kendo.directives', 'ngMessages','angularFileUpload']);
app1.controller('AppController', ['$scope', 'FileUploader', function ($scope, FileUploader) {
    
        var uploader = $scope.uploader = new FileUploader({
            url: 'upload/upload'
        });

        // FILTERS

        uploader.filters.push({
            name: 'customFilter',
            fn: function(item /*{File|FileLikeObject}*/, options) {
                return this.queue.length < 10;
            }
        });

        // CALLBACKS

        uploader.onWhenAddingFileFailed = function(item /*{File|FileLikeObject}*/, filter, options) {
            console.info('onWhenAddingFileFailed', item, filter, options);
        };
        uploader.onAfterAddingFile = function(fileItem) {
            console.info('onAfterAddingFile', fileItem);
        };
        uploader.onAfterAddingAll = function(addedFileItems) {
            console.info('onAfterAddingAll', addedFileItems);
        };
        uploader.onBeforeUploadItem = function(item) {
            console.info('onBeforeUploadItem', item);
        };
        uploader.onProgressItem = function(fileItem, progress) {
            console.info('onProgressItem', fileItem, progress);
        };
        uploader.onProgressAll = function(progress) {
            console.info('onProgressAll', progress);
        };
        uploader.onSuccessItem = function(fileItem, response, status, headers) {
            console.info('onSuccessItem', fileItem, response, status, headers);
        };
        uploader.onErrorItem = function(fileItem, response, status, headers) {
            console.info('onErrorItem', fileItem, response, status, headers);
        };
        uploader.onCancelItem = function(fileItem, response, status, headers) {
            console.info('onCancelItem', fileItem, response, status, headers);
        };
        uploader.onCompleteItem = function(fileItem, response, status, headers) {
            console.info('onCompleteItem', fileItem, response, status, headers);
        };
        uploader.onCompleteAll = function() {
            console.info('onCompleteAll');
        };

        console.info('uploader', uploader);
    }]);



