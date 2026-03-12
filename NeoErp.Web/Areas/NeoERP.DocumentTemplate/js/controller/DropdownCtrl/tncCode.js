
DTModule.controller('tncCtrl', function ($scope, $http, $routeParams, $window, $filter, $timeout, $rootScope) {
    $scope.saveupdatebtn = "Save";
    $rootScope.masterValue = "";
    $scope.tncsetupInOrder =
    {
        COMPANY_CODE: "",
        CREATED_BY: "",
        CREATED_DATE: "",
        DELETED_FLAG: "",
        GROUP_SKU_FLAG: "",
        TNC_CODE: "",
        TNC_EDESC: "",
        TNC_NDESC: "",
        MASTER_TNC_CODE: "",
        MODIFY_BY: "",
        MODIFY_DATE: "",
        PRE_TNC_CODE: "",
        PARENT_TNC_CODE: ""
    }

    $scope.tncArrInOrder = $scope.tncsetupInOrder;

    $scope.tncDataSource = {
        serverFiltering: true,
        transport: {
            read: {
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET",
                url: "/api/TemplateApi/GetTNCListByFilter",

            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        if (data.filter.filters[0].value != "") {
                            newParams = {
                                filter: data.filter.filters[0].value
                            };
                            return newParams;
                        }
                        else {
                            newParams = {
                                filter: "!@$"
                            };
                            return newParams;
                        }
                    }
                    else {
                        newParams = {
                            filter: ""
                        };
                        return newParams;
                    }
                }
                else {
                    newParams = {
                        filter: ""
                    };
                    return newParams;
                }
            }
        }
    };

    $scope.TNCCodeOption = {
        dataSource: $scope.tncDataSource,
        dataBound: function (e) {
            debugger;
            var tnc = $("#tnc").data("kendoComboBox");
            if (tnc != undefined) {
                tnc.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(TNC_EDESC,Type,  this.text()) #"), tnc)
                });
            }
        }
    }

    $scope.tncCodeOnChange = function (kendoEvent) {
        debugger;
        var tncCode = $('#tnc').val();
        $rootScope.masterValue = tncCode;
        var tncURL = "/api/TemplateApi/GetTNCDataInOrder?tncCode=" + tncCode;
        $rootScope.tncData = [];
        $http.get(tncURL).then(function (res) {
            // Kendo event is outside Angular, wrap in $apply
            debugger;
            $timeout(function () {
                $rootScope.tncData = res.data || [];
            });
        }, function () {
            $timeout(function () {
                $rootScope.tncData = [];
            });
        });
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.tncerror = "Please Enter Valid Code."
            $('#tnc').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.tncerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }
    $scope.reset = function () {
        $scope.tncsetupInOrder =
        {
            COMPANY_CODE: "",
            CREATED_BY: "",
            CREATED_DATE: "",
            DELETED_FLAG: "",
            GROUP_SKU_FLAG: "",
            TNC_CODE: "",
            TNC_EDESC: "",
            TNC_NDESC: "",
            MASTER_TNC_CODE: "",
            MODIFY_BY: "",
            MODIFY_DATE: "",
            PRE_TNC_CODE: "",
            PARENT_TNC_CODE: ""
        }

        $scope.tncArrInOrder = $scope.tncsetupInOrder;
    }

    $scope.saveNewTNC = function () {
        debugger;
        var mastertncvalue = $("#childmastertnccodeInOrder").val();
        if ($scope.saveupdatebtn == "Save") {

            var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewTNC";

            $scope.tncArrInOrder.MASTER_TNC_CODE = $scope.masterValue;
            $scope.tncArrInOrder.GROUP_SKU_FLAG = 'I';

            var model = { model: $scope.tncArrInOrder };

            $http({
                method: 'POST',
                url: createUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    debugger;
                    $scope.editcode = "";
                    $scope.edesc = "";
                    $scope.reset();
                    //var grid = $("#kGrid").data("kendo-grid");
                    //if (grid != undefined) {
                    //    grid.dataSource.read();
                    //}
                    updateTNCData(mastertncvalue, 'Saved');

                    //$("#tncChildModal").modal("hide");

                    //displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("TNC name already exist please try another TNC name.", "error");
                }
                else {
                    $scope.reset();
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("TNC name already exist please try another TNC name.", "error");
                }
                else {
                    $scope.reset();
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            });
        }
        else {
            //document.uploadFile();
            //if ($('#txtFile')[0].files[0] !== undefined) { $scope.itemArr.IMAGE_FILE_NAME = $('#txtFile')[0].files[0].name; }
            var updateUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateTNCByTNCCode";

            var model = { model: $scope.tncArrInOrder };

            $scope.saveupdatebtn = "Update";
            $http({
                method: 'POST',
                url: updateUrl,
                data: model
            }).then(function successCallback(response) {


                if (response.data.MESSAGE == "UPDATED") {

                    $scope.tncArrInOrder = [];
                    $scope.editcode = "";
                    $scope.edesc = "";

                    $scope.reset();
                    updateTNCData(mastertncvalue, 'Updated');
                }
                if (response.data.MESSAGE == "ERROR") {
                    $scope.reset();
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            }, function errorCallback(response) {

                $scope.refresh();
                reset("Something went wrong.Please try again later.", "error");
            });
        }
    }

    function updateTNCData(mastertncvalue, crud) {
        debugger;
        var tncURL = "/api/TemplateApi/GetTNCDataInOrder?tncCode=" + mastertncvalue;

        $http.get(tncURL).then(function (res) {
            $rootScope.tncData = res.data; // update ng-repeat

            // Use $timeout to ensure this runs after the digest cycle
            $timeout(function () {
                $("#tncChildModal").modal("hide");               // hide modal
                displayPopupNotification("Data successfully" + " " + crud, "success"); // show notification
            }, 0);

        }, function (err) {
            console.error("Failed to fetch updated TNC data", err);
        });

    }
    $scope.fillChildTNCSetupForms = function (tncCode) {
        var gettncdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getTNCDetailsByTNCCode?accCode=" + tncCode;
        $http({
            method: 'GET',
            url: gettncdetaisByUrl,
        }).then(function successCallback(response) {
            debugger;
            $scope.tncsetupInOrder = response.data.DATA;
            $scope.tncArrInOrder = $scope.tncsetupInOrder;

            var datas = response.data.DATA;

        }
            , function errorCallback(response) {
            }
        );
    }
    $scope.addchildtncInOrder = function (tncCode) {
        debugger;

        $scope.saveupdatebtn = "Save"
        setParentTNCCodeInOrder();
        $("#tncChildModal").modal("toggle");
    }

    function setParentTNCCodeInOrder() {
        var parentItem = $rootScope.tncData.find(function (item) {
            return item.GROUP_SKU_FLAG === 'G';
        });

        if (parentItem) {
            // Get the Kendo DropDownList
            var childItemParent = $("#childmastertnccodeInOrder").data("kendoDropDownList");

            // Set its value to the TNC_CODE of the found item
            childItemParent.value(parentItem.TNC_CODE);

            $scope.masterValue = parentItem.MASTER_TNC_CODE;

            // If needed, trigger the change event
            childItemParent.trigger("change");
        }

    }
    $scope.editchildtncInOrder = function (tncCode) {
        debugger;

        $scope.saveupdatebtn = "Update";
        setParentTNCCodeInOrder();
        $scope.fillChildTNCSetupForms(tncCode);
        $("#tncChildModal").modal("toggle");
    }

    $scope.deletechildtncInOrder = function (code) {
        var mastertncvalue = $("#childmastertnccodeInOrder").val();
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

                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeletetncsetupByTNCcode?tnccode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {

                        if (response.data.MESSAGE == "DELETED") {
                            debugger;
                            $scope.tncArrInOrder = [];
                            $scope.editcode = "";
                            $scope.edesc = "";
                            $("#tncChildModal").modal("hide");
                            $scope.reset();
                            updateTNCData($rootScope.masterValue, 'Deleted');
                        }

                    }, function errorCallback(response) {
                        $scope.reset();
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                    });

                }
                else if (result == false) {
                    $scope.reset();
                    $("#tncChildModal").modal("hide");
                }

            }
        });
    }


    var tncCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getTNCCodeWithChild";

    $scope.tncGroupDataSource = {
        transport: {
            read: {
                url: tncCodeUrl,
            }
        }
    };

    $scope.tncGroupOptionsInOrder = {
        dataSource: $scope.tncGroupDataSource,
        optionLabel: "<Primary>",
        dataTextField: "TNC_EDESC",
        dataValueField: "TNC_CODE",
        filter: "contains",
        select: function (e) {
            debugger;
            $rootScope.quickmasteritemcode = e.dataItem.MASTER_ITEM_CODE;

        },
    }



});