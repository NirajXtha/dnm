
//LOC Settlement Invoice Entry 
app.controller('SettlementInvoiceCtrl', function ($scope, $http, SettlementInvoiceService) {
    $scope.SettlementInvoiceList = [];
    $scope.shipmentType = [];
    $scope.shipmentLoad = [];
    $scope.itemlist = [];
    $scope.saveAction = 'Save';

    $scope.selectedInvoice = [];

    $scope.ItemsOnChange = function () {
        var OrderCode = $scope.pi.purchaseOrder;
        var response = $http({
            method: "get",
            url: "/api/ProformaInvoice/getItemDetailsByOrderCode?OrderCode=" + OrderCode,
            dataType: "json"
        });
        return response.then(function (data) {
            if (data.data.length == 0) {
                $scope.itemlist = [];
            }
            else {
                $scope.itemlist = data.data;
            }
        }, function errorCallback(response) {
            displayPopupNotification("Fields should not be empty.", "error");
        });
    }

    $scope.shipmentTypeDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/ProformaInvoice/GetShipmentType",
            }
        },
    };

    $scope.shipmentType = {
        optionLabel: "--Select Shipment Type--",
        dataSource: $scope.shipmentTypeDatasource,
    };

    $scope.shipmentLoadTypeDatasource = {
        type: "json",
        transport: {
            read: {
                url: "/api/ProformaInvoice/GetLoadShipmentType",
            }
        },
    };

    $scope.shipmentLoad = {
        optionLabel: "--Select Load Type--",
        dataSource: $scope.shipmentLoadTypeDatasource,
    };
});
app.service('SettlementInvoiceService', function ($http, $q, $timeout) {
    this.GetAllList = function () {
        return $http.get("/api/SettlementInvoice/getAllSettlementInvoice");
    }
    this.GetSettlementInvoiceByID = function (id) {
        var response = $http({
            method: "post",
            url: "/api/SettlementInvoice/getSettlementInvoiceByID",
            params: {
                id: JSON.stringify(id)
            },
            dataType: "json"
        });
        return response;
    }
    this.loadInvoice = function () {
        return $http.get("/api/SettlementInvoice/getInvoiceType");
    }
    this.saveSettlementInvoice = function (invoice) {
        var response = $http({
            method: "post",
            url: "/Api/SettlementInvoice/saveSettlementInvoice",
            data: JSON.stringify(invoice),
            dataType: "json"
        });
        return response;
    }
    this.updateSettlementInvoice = function (invoice) {
        var response = $http({
            method: "post",
            url: "/Api/SettlementInvoice/UpdateSettlementInvoice",
            data: JSON.stringify(invoice),
            dataType: "json"
        });
        return response;
    }
    this.deleteSettlementInvoice = function (pfinum) {
        var response = $http({
            method: "post",
            url: "/Api/SettlementInvoice/DeleteSettlementInvoice",
            params: {
                pfiCode: JSON.stringify(pfinum)
            },
            dataType: "json"
        });
        return response;
    }

});




