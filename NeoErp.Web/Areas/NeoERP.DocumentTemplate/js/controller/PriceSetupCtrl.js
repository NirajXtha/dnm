DTModule.controller('PriceSetupCtrl', function ($scope, PriceSetupService, $window,$filter,$timeout) {

    $scope.ActionButton = "Add Price";

    $scope.FormName = "Price Set Up";

    $scope.MasterField = {};

    $scope.MasterField = {
        PriceListName: "",
        CompanyName: "",
        DateEnglish: $filter('date')(new Date(), 'dd-MMM-yyyy'),
        DateNepali: ""/*$filter('date')(new Date(), 'dd-MMM-yyyy')*/,
        Status: true,
        isUpdated:false
    };

    $scope.selectedCompany;
    $scope.selectedCompany1;
    $scope.selectedPriceName;

    $scope.ChildField = {};

    $scope.ChildField = {
        ITEM_CODE: "",
        ITEM_EDESC: "",
        OLD_PRICE: "",
        NEW_PRICE:""
    };

    $scope.select2Options = {
        allowClear: true
    };

    $scope.AddPrice = function () {
        alert("m clicked");
        $scope.AddNewPrice = true;
    }

    $scope.GetCompanyList = function () {
        var companyList = PriceSetupService.GetCompanyList();
        companyList.then(function (allCompanyList) {
            $scope.CompanyNameList = allCompanyList.data;
            $scope.CompanyListForDialog = allCompanyList.data;
        }, function () { $scope.CompanyMessage="Error while getting company list" });
    };

    $scope.GetCompanyList();

    $scope.db = {
        items: []
    };

    $scope.GetItemName = function () {
        var itenNameList = PriceSetupService.GetAllItemsName();
        itenNameList.then(function (allItemName) {
            $scope.ItemNameList = allItemName.data;
            $scope.ItemForSheet = allItemName.data;
            $scope.db.items = allItemName.data;
        }, function () {$scope.ItemMessage = "Error while getting item list" });
    };


   // $scope.GetItemName();

    $scope.rowHeader = true;
    $scope.colHeader = true;

    $scope.settings = {
        contextMenu: [
            'row_above','row_below','remove_row'
        ]
    };

    $scope.addedPrice = [];


    $scope.SaveUpdatedCellValue = function (addedPrice, MasterField) {
            $scope.dataToSave = {
                ChildField: addedPrice,
                MasterField:MasterField
            };
            $scope.SaveUpdatedCell($scope.dataToSave);
    };


    $scope.afterChange = function (changes,source) {
        if (source != 'loadData') {
            var data = this.getSourceDataAtRow(changes[0][0]);
           // alert(JSON.stringify(data));
            $scope.addedPrice.push(data);
        };
     //   console.log("addedPrice Arrya==============>>" + JSON.stringify($scope.addedPrice));
    };

    $scope.SaveUpdatedCell = function (dataToSave) {

            var response = PriceSetupService.SaveUpdatedCell(dataToSave);
                 response.then(function (message) {
                     toastr.success(message.data);
                     setTimeout(function () {
                         location.reload(true);
                     },2000); 
                 }, function () { toastr.error("Error while creating or updating setup"); location.reload(true);});
       
    };


    $scope.FetchItemBasedOnCompany = function () {
        var itemOfCompany = PriceSetupService.GetItemByCompany($scope.selectedCompany);
        itemOfCompany.then(function (itemByCompany) {
            $scope.db.items = itemByCompany.data;
        });
    };



    $scope.SaveAddedPrice = function () {
        if ($scope.ActionButton == "Update") {

            $scope.MasterField.isUpdated = true;
            $scope.MasterField.CompanyName = $scope.selectedCompany;
            $scope.SaveUpdatedCellValue($scope.addedPrice, $scope.MasterField);
        }
        else {
            $scope.MasterField.isUpdated = false;

            if ($scope.MasterField.PriceListName == "" || $scope.addedPrice.length== 0) {
                toastr.warning("Price List Name or item name can't be empty.");
                var name = $window.document.getElementById('priceNameList');
                name.focus();
            } else {
                $scope.MasterField.CompanyName = $scope.selectedCompany;
                $scope.SaveUpdatedCellValue($scope.addedPrice, $scope.MasterField);
            }
        }
        
    };

    $scope.AddNewPrice = false;
    $scope.EditOldPrice = false;

    $scope.NewPriceSetup = function () {
        //$timeout(function () {
        //    $scope.AddNewPrice = true;
        //}, 10);
        location.reload(true);
        var name = $window.document.getElementById('priceNameList');
        name.focus();
    };


    $scope.EditPriceSetup = function () {
        $scope.EditOldPrice = true;
    }

    $scope.Cancel = function () {
        $scope.AddNewPrice = false;
    };

    $scope.CancelEdit = function () {
        $scope.EditOldPrice = false;
    };

    $scope.ConvertEngToNep = function () {
        console.log(this);

        var engdate = $("#englishDate5").val();
        var nepalidate = ConvertEngDateToNep(engdate);
        $("#nepaliDate5").val(nepalidate);
    };

    $scope.someDateFn = function () {

        var engdate = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 1)), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        $scope.NepaliDate = a;
       // var nepaliDate = $window.document.getElementById('nepaliDate5');
        //$("#nepaliDate5").val(a);
        $scope.MasterField.DateNepali = a;

    };

    $scope.someDateFn();


    $scope.GetSavedItemName = function () {
        var allSavedItem = PriceSetupService.GetAllSavedItemsName();
        allSavedItem.then(function (savedItem) {
            //$("#autoCompleteItem").autocomplete({
            //    source: savedItem.data
            //});
            $scope.SavedItemName = savedItem.data;
        });
    };

    $scope.GetSavedItemName();
    $scope.isEdit=false;

    $scope.SearchItem = function () {
        //var itemToEdit = PriceSetupService.GetItemToEdit($scope.selectedCompany1, $scope.selectedPriceName);
        if ($scope.selectedPriceName == "") {
            toastr.warning("Price name must selectd while editing");
        } else {
            var itemToEdit = PriceSetupService.GetItemToEdit($scope.selectedPriceName);
            itemToEdit.then(function (iie) {
                $scope.db.items = iie.data.m_Item1;
                $scope.MasterField.PriceListName = iie.data.m_Item2.PRICE_LIST_NAME;
                console.log("M_Item2===================>>>" + JSON.stringify(iie.data.m_Item2));
                $scope.selectedCompany = iie.data.m_Item2.COMPANY;
                $scope.ActionButton = "Update";
                $scope.isEdit = true;
            });
            $scope.EditOldPrice = false;
        }

    };
});



DTModule.service('PriceSetupService', function ($http) {

    this.GetItemToEdit = function (selectedPriceName) {
        var itemToEditResponse = $http({
            method: "GET",
            url: "/api/PriceSetupApi/GetItemToEdit",
            dataType: "json",
            params: {
                selectedPriceName: JSON.stringify(selectedPriceName)
            }
        });

        return itemToEditResponse;
    };

    this.GetItemByCompany = function (selectedCompany) {
        var itemByCompanyResponse = $http({
            method: "GET",
            url: "/api/PriceSetupApi/GetItemByCompany?selectedCompany=" + selectedCompany,
            data:JSON.stringify(selectedCompany),
            dataType: "json"
        });

        return itemByCompanyResponse;
    };

    this.GetCompanyList = function () {
        var companyList = $http({
            method: "GET",
            url: window.location.protocol + "//" + window.location.host + "/api/Purchase/GetCompanyList",
            dataType: "json"
        });

        return companyList;
    }

    this.GetAllItemsName = function () {

        var itemResponse = $http({
            method: "GET",
            url: "/api/PriceSetupApi/GetAllItemWithName",
            dataType: "json"
        });
        return itemResponse;
    }

    this.GetAllSavedItemsName = function () {
        var itemResponse = $http({
            method: "GET",
            url: "/api/PriceSetupApi/GetAllSavedItemsName",
            dataType: "json"
        });
        return itemResponse;
    }

    this.SaveUpdatedCell = function (params) {
        console.log("params=====================> " + JSON.stringify(params));
        var saveResponse = $http({
            method: "POST",
            url: "/api/PriceSetupApi/SaveUpdatedCell",
            data: JSON.stringify(params),
            dataType: "json"
        });
        return saveResponse;
    }

});