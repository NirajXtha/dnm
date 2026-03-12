
//DTModule.controller('processSetupTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
//    $scope.saveupdatebtn = "Save";
//    $scope.processsArr;
//    $scope.savegroup = false;
//    $scope.editFlag = "N";
//    $scope.newrootinsertFlag = "Y";
//    $scope.treenodeselected = "N";
//    $scope.categoryChecker = false;
//    $scope.processChecker = false;
//    $scope.reguralChecker = false;
//    var processdataFillDefered = $.Deferred();



//    $scope.processsetup =
//        {
//            PRE_PROCESS_CODE: "",
//            INDEX_ITEM_CODE: "",
//            INDEX_CAPACITY: "",
//            INDEX_MU_CODE: "",
//            REMARKS: "",
//            LOCATION_CODE: "",
//            COMPANY_CODE: "",
//            CREATED_BY: "",
//            CREATED_DATE: "",
//            DELETED_FLAG: "",
//            INDEX_PERIOD_CODE: "",
//            INDEX_TIME_REQUIRED: "",
//            INDEX_TIME_MU_CODE: "",
//            INDEX_TIME_PERIOD_CODE: "",
//            INPUT_INDEX_ITEM_CODE: "",
//            PRIORITY_ORDER_NO: "",
//            SYN_ROWID: "",
//            MODIFY_DATE: "",
//            MODIFY_BY: "",
//            PROCESS_CODE: "",
//            PROCESS_EDESC: "",
//            PROCESS_NDESC: "",
//            PROCESS_TYPE_CODE: "",
//            PROCESS_FLAG: "",
//            MASTER_PROCESS_CODE: "",
//            NEW_PROCESS_CODE: "",
//            PARENT_PROCESS_CODE: "",
//            PROCESS_EDESC: "",
//            PROCESS_NDESC: ""
//        }

//    //var natureCustomArr = natureArr;
//    $scope.treeSelectedProcessCode = "";
//    $scope.bankdetailArr = [];
//    $scope.bankdetailArr.push($scope.bankprocessdetail);
//    $scope.processsArr = $scope.processsetup;
//    $scope.processGroupDataSource = [
//        { text: "<PRIMARY>", value: "" }
//    ];



//    $scope.clearFields = function () {

//        $scope.processsArr.PROCESS_EDESC = "";
//        $scope.processsArr.PROCESS_NDESC = "";
//        $scope.processsArr.PROCESS_FLAG = "";
//        $scope.processsArr.PRIORITY_ORDER_NO = "";
//        $scope.processsArr.REMARKS = " ";

//        $('#processType').val(' ');
//        $('#masterlocationcode').val(' ');
//    }

//    var processCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getProcessCodeWithChild";

//    $scope.processGroupDataSource = new kendo.data.DataSource({
//        transport: {
//            read: {
//                url: processCodeUrl,
//            }
//        }
//    });
//    $scope.MACDS = [];

//    $scope.processGroupOptions = {
//        dataSource: $scope.processGroupDataSource,
//        optionLabel: "<PRIMARY>",
//        dataTextField: "PROCESS_EDESC",
//        dataValueField: "PROCESS_CODE",
//        change: function (e) {

//            var currentItem = e.sender.dataItem(e.node);
//            //natureCustomArr = [];
//            if (currentItem.PROCESS_CODE != "") {
//                $scope.getDetailByAccCode(currentItem.PROCESS_CODE);
//            } else {
//                //natureCustomArr = natureArr;
//            }
//            var natureTree = $("#natureprocess").data("kendoDropDownList");
//            //natureTree.setDataSource(natureCustomArr);
//        },
//        dataBound: function () {

//            //var tree = $("#masterprocesscode").data("kendoComboBox");
//            //tree.dataSource.add({ PROCESS_CODE: "", PROCESS_EDESC: "<PRIMARY>" });
//            $scope.processGroupDataSource;
//        }
//    }



//    var getProcessCodeByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getProcessCode";
//    $scope.processtreeData = new kendo.data.HierarchicalDataSource({

//        transport: {
//            read: {
//                url: getProcessCodeByUrl,
//                type: 'GET',
//                data: function (data, evt) {
//                }
//            },

//        },
//        schema: {
//            parse: function (data) {
//                return data;
//            },
//            model: {
//                id: "ProcessId",
//                parentId: "ProcessName",
//                children: "Items",
//                fields: {
//                    ProcessId: { field: "ProcessId", type: "string" },
//                    ProcessName: { field: "ProcessName", type: "string" },
//                    parentId: { field: "preProcessCode", type: "string", defaultValue: "00" },
//                }
//            }
//        }
//    });

//    //Grid Binding main Part

//    $scope.acconDataBound = function () {
//        $('#processtree').data("kendoTreeView").expand('.k-item');
//    }
//    $scope.getDetailByAccCode = function (accId) {

//        var getAccountdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetProcessDataByProcessCode?processcode=" + accId;
//        $http({
//            method: 'GET',
//            url: getAccountdetaisByUrl,

//        }).then(function successCallback(response) {
//            var accNature = response.data.DATA.PROCESS_NATURE;
//            $scope.bindNatureByAccNature(accNature);

//        }, function errorCallback(response) {

//        });
//    }

//    $scope.processOptions = {

//        loadOnDemand: false,
//        select: function (e) {
//            var currentItem = e.sender.dataItem(e.node);
//            $('#processGrid').removeClass("show-displaygrid");
//            $("#processGrid").html("");
//            $($(this._current).parents('ul')[$(this._current).parents('ul').length - 1]).find('span').removeClass('hasTreeCustomMenu');
//            $(this._current.context).find('span').addClass('hasTreeCustomMenu');
//            $scope.processsetup.PROCESS_CODE = currentItem.ProcessId;
//            $scope.processsArr.PROCESS_CODE = currentItem.ProcessId;
//            $scope.processsetup.PROCESS_EDESC = currentItem.ProcessName;
//            $scope.processsetup.PROCESS_FLAG = currentItem.groupSkuFlag;

//            $scope.treeSelectedProcessCode = currentItem.ProcessId;
//            $scope.treenodeselected = "Y";
//            $scope.newrootinsertFlag = "N";

//            var tree = $("#masterprocesscode").data("kendoDropDownList");
//            tree.value($scope.processsArr.PROCESS_CODE);
//            //$scope.movescrollbar();
//        },

//    };

//    $scope.movescrollbar = function () {
//        var element = $(".k-in");
//        for (var i = 0; i < element.length; i++) {
//            var selectnode = $(element[i]).hasClass("k-state-focused");
//            if (selectnode) {
//                $("#processtree").animate({
//                    scrollTop: (parseInt(i * 12))
//                });
//                break;
//            }
//        }
//    }
//    //Grid Binding main Part

//    $scope.processOnDataBound = function () {
//        $('#processtree').data("kendoTreeView").expand('.k-item');
//    }
//    $scope.fillprocessSetupForms = function (processId) {

//        var getprocessdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getProcessDetailsByProcessCode?processCode=" + processId;
//        $http({
//            method: 'GET',
//            url: getprocessdetaisByUrl,

//        }).then(function successCallback(response) {
//            $scope.processsetup = response.data.DATA;
//            $scope.processsArr = $scope.processsetup;
//            //$scope.accountsArr.ACC_TYPE_FLAG = "N";
//            $scope.processsArr.PROCESS_CODE = $scope.processsetup.PROCESS_CODE;
//            if ($scope.editFlag == "Y") {
//                $scope.processsArr.MASTER_PROCESS_CODE = $scope.processsetup.PRE_PROCESS_CODE
//            }
//            //else {
//            //$scope.processsArr.MASTER_PROCESS_CODE = $scope.processsetup.PROCESS_CODE;
//            //}

//            processdataFillDefered.resolve();
//            // this callback will be called asynchronously
//            // when the response is available
//        }, function errorCallback(response) {

//            // called asynchronously if an error occurs
//            // or server returns response with an error status.
//        });
//    }
//    //treeview on select

//    $scope.onContextSelect = function (event) {
//        if ($scope.processsetup.PROCESS_CODE == "")
//            return displayPopupNotification("Select process.", "error");;
//        $scope.saveupdatebtn = "Save";
//        if (event.item.innerText.trim() == "Delete") {

//            bootbox.confirm({
//                title: "Delete",
//                message: "Are you sure?",
//                buttons: {
//                    confirm: {
//                        label: 'Yes',
//                        className: 'btn-success',
//                        label: '<i class="fa fa-check"></i> Yes',
//                    },
//                    cancel: {
//                        label: 'No',
//                        className: 'btn-danger',
//                        label: '<i class="fa fa-times"></i> No',
//                    }
//                },
//                callback: function (result) {
//                    if (result == true) {
//                        debugger;
//                        //if (processFlag === "R")
//                        //    $scope.processsetup.PROCESS_FLAG = "R";
//                        //else
//                        //    $scope.processsetup.PROCESS_FLAG = "XXX";
//                        var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteProcessSetupByProcessCode?processCode=" + $scope.processsetup.PROCESS_CODE;
//                        $http({
//                            method: 'POST',
//                            url: delUrl
//                        }).then(function successCallback(response) {

//                            if (response.data.MESSAGE == "DELETED") {
//                                debugger;
//                                //if ($scope.processsetup.PROCESS_FLAG !== "R") {
//                                var tree = $("#processtree").data("kendoTreeView");
//                                tree.dataSource.read();
//                                //}
//                                var grid = $('#kGrid').data("kendoGrid");
//                                grid.dataSource.read();
//                                displayPopupNotification("Data succesfully deleted ", "success");
//                            }
//                            if (response.data.MESSAGE == "HAS_CHILD") {


//                                displayPopupNotification("Please delete the respective child first", "warning");
//                            }
//                            // this callback will be called asynchronously
//                            // when the response is available
//                        }, function errorCallback(response) {
//                            displayPopupNotification(response.data.STATUS_CODE, "error");
//                            // called asynchronously if an error occurs
//                            // or server returns response with an error status.
//                        });

//                    }

//                }
//            });
//        }

//        else if (event.item.innerText.trim() == "Update") {

//            locationdataFillDefered = $.Deferred();
//            $scope.saveupdatebtn = "Update";

//            $scope.fillprocessSetupForms($scope.processsetup.PROCESS_CODE);

//            $.when(processdataFillDefered).then(function () {

//                //var tree = $("#masterprocesscode").data("kendoDropDownList");
//                //tree.value($scope.processsArr.PROCESS_CODE);
//                if ($scope.processsetup.PARENT_PROCESS_CODE == null || $scope.processsetup.PARENT_PROCESS_CODE == undefined) {
//                    var popUpDropdown = $("#masterprocesscode").data("kendoDropDownList");
//                    popUpDropdown.value('');
//                }
//                else {
//                    var popUpDropdown = $("#masterprocesscode").data("kendoDropDownList");
//                    popUpDropdown.value($scope.processsetup.PARENT_PROCESS_CODE);
//                }

//                $("#processModal").modal();
//            });

//        }
//        else if (event.item.innerText.trim() == "Add") {

//            processdataFillDefered = $.Deferred();
//            $scope.savegroup = true;
//            $("#category").attr('disabled', 'disabled');
//            $("#process").attr('disabled', 'disabled');
//            $("#Routine").attr('disabled', 'disabled');
//            $scope.fillprocessSetupForms($scope.processsetup.PROCESS_CODE);

//            $.when(processdataFillDefered).then(function () {

//                $scope.clearFields();
//                var tree = $("#masterprocesscode").data("kendoDropDownList");
//                tree.value($scope.processsArr.PROCESS_CODE);
//                $scope.processsetup.PROCESS_FLAG = "P";
//                $scope.processsetup.PROCESS_FLAG = "P";
//                if ($scope.processsetup.PROCESS_FLAG == "P") {
//                    $scope.result = true;
//                }
//                else {
//                    $scope.result = false;
//                }
//                //$scope.clearFields();
//                $("#processModal").modal();
//            });

//        }
//    }
//    $scope.saveNewProcess = function (isValid) {
//        debugger;
//        if (!isValid) {
//            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
//            return;
//        }
//        if ($scope.processsArr.PROCESS_FLAG == undefined || $scope.processsArr.PROCESS_FLAG == "" || $scope.processsArr.PROCESS_FLAG == null) {
//            displayPopupNotification("Process Flag is required.", "error");
//            return;
//        }
//        if ($scope.processsArr.PROCESS_FLAG == 'C' || $scope.processsArr.PROCESS_FLAG == 'P')
//        {
//            if ($scope.saveupdatebtn == "Save") {

//                var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewProcessHead";
//                var model = {
//                    //model: $scope.processsetup
//                    PROCESS_CODE: $scope.processsArr.PROCESS_CODE,
//                    PROCESS_EDESC: $scope.processsArr.PROCESS_EDESC,
//                    PROCESS_NDESC: $scope.processsArr.PROCESS_NDESC,
//                    PROCESS_TYPE_CODE: $scope.processsArr.PROCESS_TYPE_CODE,
//                    PROCESS_FLAG: $scope.processsArr.PROCESS_FLAG,
//                    LOCATION_CODE: $scope.processsArr.LOCATION_CODE,
//                    PRIORITY_ORDER_NO: $scope.processsArr.PRIORITY_ORDER_NO,
//                    REMARKS: $scope.processsArr.REMARKS
//                }
               
//                $http({
//                    method: 'POST',
//                    url: createUrl,
//                    data: model
//                }).then(function successCallback(response) {

//                    if (response.data.MESSAGE == "INSERTED") {

//                        $scope.processsArr = [];
//                        if ($scope.processsetup.PROCESS_FLAG !== "R") {
//                            var tree = $("#processtree").data("kendoTreeView");
//                            tree.dataSource.read();
//                        }

//                        var grid = $("#kGrid").data("kendoGrid");
//                        if (grid != undefined) {
//                            grid.dataSource.read();
//                        }
//                        var ddl = $("#masterprocesscode").data("kendoDropDownList");
//                        if (ddl != undefined)
//                            ddl.dataSource.read();
//                        //grid.dataSource.read();
//                        if ($scope.savegroup == false) { $("#processModal").modal("toggle"); }
//                        else { $("#processModal").modal("toggle"); }

//                        displayPopupNotification("Data succesfully saved ", "success");
//                    }
//                    else {
//                        displayPopupNotification("Something went wrong.Please try again later.", "error");
//                    }
//                    // this callback will be called asynchronously
//                    // when the response is available
//                }, function errorCallback(response) {
//                    displayPopupNotification("Something went wrong.Please try again later.", "error");
//                    // called asynchronously if an error occurs
//                    // or server returns response with an error status.
//                });
//            }
//            else {
//                var updateUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateProcessByProcessCode";
//                var model = {
//                    PROCESS_CODE: $scope.processsArr.PROCESS_CODE,
//                    PROCESS_EDESC: $scope.processsArr.PROCESS_EDESC,
//                    PROCESS_NDESC: $scope.processsArr.PROCESS_NDESC,
//                    PROCESS_TYPE_CODE: $scope.processsArr.PROCESS_TYPE_CODE,
//                    PROCESS_FLAG: $scope.processsArr.PROCESS_FLAG,
//                    LOCATION_CODE: $scope.processsArr.LOCATION_CODE,
//                    PRIORITY_ORDER_NO: $scope.processsArr.PRIORITY_ORDER_NO,
//                    REMARKS: $scope.processsArr.REMARKS,

//                }
//                $scope.saveupdatebtn = "Update";
//                $http({
//                    method: 'POST',
//                    url: updateUrl,
//                    data: model
//                }).then(function successCallback(response) {

//                    if (response.data.MESSAGE == "UPDATED") {
//                        $scope.processsArr = [];
//                        if ($scope.processsetup.PROCESS_FLAG !== "R") {
//                            var tree = $("#processtree").data("kendoTreeView");
//                            tree.dataSource.read();
//                        }

//                        var ddl = $("#masterprocesscode").data("kendoDropDownList");
//                        if (ddl != undefined)
//                            ddl.dataSource.read();

//                        var grid = $("#kGrid").data("kendo-grid");
//                        grid.dataSource.read();
//                        $("#processModal").modal("toggle");
//                        displayPopupNotification("Data succesfully updated ", "success");
//                    }
//                    if (response.data.MESSAGE == "ERROR") {
//                        displayPopupNotification("Something went wrong.Please try again later.", "error");
//                    }
//                    // this callback will be called asynchronously
//                    // when the response is available
//                }, function errorCallback(response) {

//                    displayPopupNotification("Something went wrong.Please try again later.", "error");
//                    // called asynchronously if an error occurs
//                    // or server returns response with an error status.
//                });
//            }
//        }
        
//                //$scope.showModalForNext = function (event) {
//                //    //$("#CompanyModal").modal("toggle");
                  
//                //}
//                $('#CompanyModal').modal('show')
//                $("#processModal").modal('hide');
         
        

       
//    }


//    $scope.MACDSOptions = {
//        dataSource: $scope.MACDS,
//        dataTextField: "text",
//        dataValueField: "value",

//    };

//    $scope.masterAccCodeOptions = {
//        dataSource: $scope.masterAccCodeDataSource,
//        dataTextField: "text",
//        dataValueField: "value",
//    };


//    $scope.BindGrid = function (groupId) {
//        $(".topsearch").show();
//        var url = null;
//        if (groupId == "All") {
//            if ($('#pcrtxtSearchString').val() == null || $('#pcrtxtSearchString').val() == '' || $('#pcrtxtSearchString').val() == undefined || $('#pcrtxtSearchString').val() == 'undefined') {
//                alert('Input is empty or undefined.');
//                return;
//            }
//            url = "/api/SetupApi/GetAllProcessList?searchText=" + $('#pcrtxtSearchString').val();
//        }
//        else {
//            $("#pcrtxtSearchString").val('');
//            url = "/api/SetupApi/GetChildOfProcessByGroup?groupId=" + groupId;
//        }
//        $scope.processChildGridOptions = {

//            dataSource: {
//                type: "json",
//                transport: {
//                    read: url,
//                },
//                pageSize: 50,
//                //serverPaging: true,
//                serverSorting: true
//            },
//            scrollable: true,
//            height: 450,
//            sortable: true,
//            pageable: true,
//            dataBound: function (e) {
//                $("#kGrid tbody tr").css("cursor", "pointer");
//                DisplayNoResultsFound($('#kGrid'));
//                $("#kGrid tbody tr").on('dblclick', function () {
//                    var processCode = $(this).find('td span').html()
//                    $scope.edit(processCode);
//                    //var tree = $("#processtree").data("kendoTreeView");
//                    //tree.dataSource.read();
//                })
//            },
//            columns: [

//                {
//                    field: "PROCESS_EDESC",
//                    title: "Name",
//                    width: "120px"
//                },

//                {
//                    field: "LOCATION_EDESC",
//                    title: "Location",
//                    width: "120px"
//                },
//                {
//                    field: "CREATED_BY",
//                    title: "Created By",
//                    width: "120px"
//                }, {
//                    field: "CREATED_DATE",
//                    title: "created Date",
//                    template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",
//                    width: "120px"
//                },

//                {
//                    title: "Action ",
//                    template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(dataItem.PROCESS_CODE)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="Delete" ng-click="delete(dataItem.PROCESS_CODE,dataItem.PROCESS_FLAG)"><span class="sr-only"></span> </a>',
//                    width: "40px"
//                }
//            ],


//        };

//        $scope.onsiteSearch = function ($this) {

//            var q = $("#txtSearchString").val();
//            var grid = $("#kGrid").data("kendo-grid");
//            grid.dataSource.query({
//                page: 1,
//                pageSize: 50,
//                filter: {
//                    logic: "or",
//                    filters: [
//                        { field: "ORDER_NO", operator: "contains", value: q },
//                        { field: "ORDER_DATE", operator: "contains", value: q },
//                        { field: "CREATED_BY", operator: "contains", value: q }
//                    ]
//                }
//            });
//        }
//    }

//    $scope.showModalForNew = function (event) {

//        processdataFillDefered = $.Deferred();
//        //if ($scope.processsetup.PROCESS_CODE != null && $scope.processsetup.PROCESS_CODE != undefined && $scope.processsetup.PROCESS_CODE != "" && $scope.processsetup.PROCESS_FLAG != "R") {
//        $scope.editFlag = "N";
//        $("#category").attr('disabled', 'disabled');
//        $("#process").attr('disabled', 'disabled');
//        $("#Routine").attr('disabled', 'disabled');
       
      
//        //$("#Routine").removeAttr('disabled');
      
//        $scope.reguralChecker = true;
//        //$scope.clearFields();
//        $scope.fillprocessSetupForms($scope.treeSelectedProcessCode);
//        $.when(processdataFillDefered).then(function () {
//            $scope.clearFields();
//            $scope.saveupdatebtn = "Save"
//            $scope.processsetup.PROCESS_FLAG = "R";
//            $scope.processsArr.PROCESS_FLAG = "R";
//            if ($scope.processsetup.PROCESS_FLAG == "R") {
//                $scope.result = true;
//            }
//            else {
//                            $scope.result = false;
//                        }

//            var tree = $("#masterprocesscode").data("kendoDropDownList");

//            tree.value($scope.treeSelectedProcessCode);
//            $("#processModal").modal("toggle");
//            $scope.processsArr.PROCESS_CODE = $scope.processsetup.PROCESS_CODE;

//        }, function errorCallback(response) {
//            displayPopupNotification(response.data.STATUS_CODE, "error");

//        });


//    }
//    $scope.edit = function (processCode) {


//        processdataFillDefered = $.Deferred();
//        $scope.editFlag = "Y";
//        $scope.fillprocessSetupForms(processCode);
//        $scope.groupAccTypeFlag = "N";
//        $scope.saveupdatebtn = "Update"
//        $scope.processChecker = false;
//        $scope.categoryChecker = false;
//        $scope.reguralChecker = false;

//        $.when(processdataFillDefered).then(function () {
//            var tree = $("#masterprocesscode").data("kendoDropDownList");
//            tree.value($scope.processsArr.MASTER_PROCESS_CODE);
//            $("#processModal").modal();
//        });

//    }
//    $scope.delete = function (code, processFlag) {
//        debugger;
//        bootbox.confirm({
//            title: "Delete",
//            message: "Are you sure?",
//            buttons: {
//                confirm: {
//                    label: 'Yes',
//                    className: 'btn-success',
//                    label: '<i class="fa fa-check"></i> Yes',
//                },
//                cancel: {
//                    label: 'No',
//                    className: 'btn-danger',
//                    label: '<i class="fa fa-times"></i> No',
//                }
//            },
//            callback: function (result) {
//                if (result == true) {
//                    debugger;

//                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteProcessSetupByProcessCode?processCode=" + code;
//                    $http({
//                        method: 'POST',
//                        url: delUrl
//                    }).then(function successCallback(response) {

//                        if (response.data.MESSAGE == "DELETED") {

//                            var grid = $('#kGrid').data("kendoGrid");
//                            grid.dataSource.read();
//                            displayPopupNotification("Data succesfully deleted ", "success");
//                        }
//                        if (response.data.MESSAGE == "HAS_CHILD") {


//                            displayPopupNotification("Please delete the respective child first", "warning");
//                        }
//                        // this callback will be called asynchronously
//                        // when the response is available
//                    }, function errorCallback(response) {
//                        displayPopupNotification(response.data.STATUS_CODE, "error");
//                        // called asynchronously if an error occurs
//                        // or server returns response with an error status.
//                    });

//                }

//            }
//        });
//    }
//    //Add in the root
//    $scope.addNewProcess = function () {
      
//        $scope.editFlag = "N";
//        $scope.saveupdatebtn = "Save"
//        $scope.processsArr = [];
//        $("#category").removeAttr('disabled');
//        $("#process").removeAttr('disabled');
//        $("#Routine").attr('disabled', 'disabled');
//        $scope.processChecker = false;
//        $scope.categoryChecker = true;
//        $scope.reguralChecker = false;
//        var tree = $("#masterprocesscode").data("kendoDropDownList");
//        tree.value("");
//        var tree = $("#processtree").data("kendoTreeView");
//        tree.dataSource.read();
//        var grid = $("#kGrid").data("kendo-grid");
//        //grid.dataSource.data([]);
//        $scope.processsArr.PROCESS_FLAG = "C";

//        //if ($scope.processsetup.PROCESS_FLAG == "P") {
//        //    $scope.result = true;
//        //}
//        //else {
//        //    $scope.result = false;
//        //}

//        $(function () {
//            debugger;
//            $('input:radio[name="processflag"]').change(function () {
//                if ($(this).val() == 'P') {
//                    $scope.result = true;
//                } else {
//                    $scope.result = false;
//                }
//            });
//        });
//        $("#processModal").modal("toggle");
//    }
//    var locationCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllLocation";

//    $scope.locationGroupDataSource = new kendo.data.DataSource({
//        transport: {
//            read: {
//                url: locationCodeUrl,
//            }
//        }
//    });
//    $scope.locationGroupOptions = {
//        dataSource: $scope.locationGroupDataSource,

//        dataTextField: "LocationName",
//        dataValueField: "LocationCode",
//        change: function (e) {

//            var currentItem = e.sender.dataItem(e.node);

//        },
//        dataBound: function () {

//            //var tree = $("#masterlocationcode").data("kendoComboBox");
//            //tree.dataSource.add({ LOCATION_CODE: "", LOCATION_EDESC: "<PRIMARY>" });
//            //$scope.locationGroupDataSource;
//        }
//    }

//    $scope.limitPriorityDigit = function () {
//        debugger;
//        var priorityLength = $scope.processsArr.PRIORITY_ORDER_NO;
//        var str = priorityLength.toString();
//        var res = str.substring(0, 5);
//        var prioritylen = parseInt(res);
//        if (str.length > 5) {
//            $scope.processsArr.PRIORITY_ORDER_NO = prioritylen;
//            displayPopupNotification("Priority can not greater then 5 digit.", "warning");
//            return;
//        }
//    };
//    function DisplayNoResultsFound(grid) {

//        // Get the number of Columns in the grid
//        //var grid = $("#kGrid").data("kendo-grid");
//        var dataSource = grid.data("kendoGrid").dataSource;
//        var colCount = grid.find('.k-grid-header colgroup > col').length;

//        // If there are no results place an indicator row
//        if (dataSource._view.length == 0) {
//            grid.find('.k-grid-content tbody')
//                .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" style="text-align:center"><b>No Results Found!</b></td></tr>');
//        }

//        // Get visible row count
//        var rowCount = grid.find('.k-grid-content tbody tr').length;

//        // If the row count is less that the page size add in the number of missing rows
//        if (rowCount < dataSource._take) {
//            var addRows = dataSource._take - rowCount;
//            for (var i = 0; i < addRows; i++) {
//                //grid.find('.k-grid-content tbody').append('<tr class="kendo-data-row"><td>&nbsp;</td></tr>');
//            }
//        }
//    }

//    $scope.SubLedger_Cancel = function () {

//        debugger;
//        $scope.refresh();
//    }

//    $scope.refresh = function () {

//        var tree = $("#processtree").data("kendoTreeView");
//        tree.dataSource.read();
       
//    }

   
//});

