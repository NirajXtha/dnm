
DTModule.controller('locationSetupTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, $timeout) {
    $scope.saveupdatebtn = "Save";
    $scope.phoneNumbr = /^\+?(\d+(\.(\d+)?)?|\.\d+)$/;
    $scope.masterlocationdisabled = false;
    $scope.groupchilddisabled = true;
    $scope.locationsArr;
    $scope.savegroup = false;
    $scope.editFlag = "N";
    $scope.newrootinsertFlag = "Y";
    $scope.treenodeselected = "N";
    $scope.treeselectedlocationCode = "";
    var locationdataFillDefered = $.Deferred();
    //$scope.groupLocationTypeFlag = "Y";
    $scope.locationsetup =
        {
            LOCATION_CODE: "",
            LOCATION_EDESC: "",
            LOCATION_NDESC: "",
            PRE_LOCATION_CODE: "",
            ADDRESS: "",
            AUTH_CONTACT_PERSON: "",
            TELEPHONE_MOBILE_NO: "",
            EMAIL: "",
            FAX: "",
            LOCATION_TYPE_CODE: "",
            GROUP_SKU_FLAG: "G",
            REMARKS: "",
            COMPANY_CODE: "",
            CREATED_BY: "",
            CREATED_DATE: "",
            DELETED_FLAG: "",
            SYN_ROWID: "",
            MODIFY_DATE: "",
            MODIFY_BY: "",
            BRANCH_CODE: "",
            STORAGE_CAPACITY: "",
            MU_CODE: "",
            LOCATION_ID: "",
            PARENT_LOCATION_CODE: "",
        }
    //ng-disabled="masterlocationdisabled" is ommited from view
    $scope.locationsArr = [];
    $scope.locationsArr = $scope.locationsetup;
    $scope.masterLocationCodeDataSource = [
        { text: "<PRIMARY>", value: "" }
    ];


    var locationCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetLocationByGroup";

    $scope.locationGroupDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: locationCodeUrl,
            }
        }
    });

    var muCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetMuCode"

    $scope.muCodeDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: muCodeUrl,
            }
        }
    });

    //var locationTypeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getLocationType";
    //$scope.locationTypeDataSource = new kendo.data.DataSource({
    //    transport: {
    //        read: {
    //            url: locationTypeUrl,
    //        }
    //    }
    //});


    $http.get("/api/TemplateApi/getLocationType")
        .then(function (response) {
            // Initialize the Kendo dropdown with API data
            $scope.locationTypeOptions = {
                dataSource: new kendo.data.DataSource({
                    data: response.data // set API data here
                }),
                popup: {
                    appendTo: $("#locationModal") // Forces the list to stay inside the modal context
                },
                filter: "contains",
                optionLabel: "-select location type-",
                dataTextField: "LOCATION_TYPE_EDESC",
                dataValueField: "LOCATION_TYPE_CODE",
            };
        })
        .catch(function (error) {
            console.error("API error:", error);
        });


    $scope.MACDS = [];

    //$scope.locationTypeOptions = {
    //    minLength: 0,
    //    dataSource: $scope.locationTypeDataSource,
    //    dataTextField: "LOCATION_TYPE_EDESC",
    //    dataValueField: "LOCATION_TYPE_CODE",

    //    placeholder: "Select Location Type...",
    //    filter: "contains",
    //    change: function (e) {
    //        var currentItem = e.sender.dataItem(e.node);
    //    },
    //    dataBound: function () {
    //    }
    //}
    $scope.muCodeOptions = {
        dataSource: $scope.muCodeDataSource,
        dataTextField: "MU_EDESC",
        dataValueField: "MU_CODE",

        placeholder: "Select MU Code...",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    }
    var branchUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetBranch"

    $scope.locationBranchDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: branchUrl,
            }
        }
    });
    $scope.locationBranchOptions = {
        dataSource: $scope.locationBranchDataSource,
        dataTextField: "BranchName",
        dataValueField: "BranchId",
        placeholder: "Select Branch...",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    }

    $scope.locationGroupOptions = {
        dataSource: $scope.locationGroupDataSource,
        optionLabel: "<PRIMARY>",
        dataTextField: "LOCATION_EDESC",
        dataValueField: "LOCATION_CODE",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);

        },
        dataBound: function () {
        }
    }


    var gettolocationsByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/Getlocation";
    $scope.locationtreeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: gettolocationsByUrl,
                type: 'GET',
                data: function (data, evt) {
                }
            },

        },
        schema: {
            parse: function (data) {
                return data;
            },
            model: {
                //id: "LOCATION_CODE",
                //parentId: "PRE_LOCATION_CODE",
                children: "Items",
                fields: {
                    LOCATION_CODE: { field: "LOCATION_CODE", type: "string" },
                    LOCATION_EDESC: { field: "LOCATION_EDESC", type: "string" },
                    parentId: { field: "PRE_LOCATION_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    function deepClone(obj) {
        return JSON.parse(JSON.stringify(obj));
    }

    function filterNodes(nodes, searchText) {
        const result = [];

        angular.forEach(nodes, function (node) {
            debugger
            const children = node.Items ? (node.Items.data ? node.Items.data() : node.Items) : [];
            const filteredChildren = filterNodes(children, searchText);

            let isMatch = false;
            let nodeText = "";
            let newNode = {};

            newNode = deepClone(node);

            if (newNode.LOCATION_CODE && newNode.LOCATION_EDESC) {
                nodeText = (newNode.LOCATION_EDESC || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            } else {
                newNode.LOCATION_CODE = node.LocationId;
                newNode.LOCATION_EDESC = node.LocationName;
                nodeText = (newNode.LocationName || "").toLowerCase();
                isMatch = nodeText.includes(searchText);
            }

            if (isMatch || filteredChildren.length > 0) {
                newNode.Items = filteredChildren;
                result.push(newNode);
            }
        });

        return result;
    }

    $scope.filterTreeView = function () {
        const searchText = ($scope.txtMainGroupSearchString || '').toLowerCase();

        if (!searchText) {
            $scope.tree.setDataSource($scope.locationtreeData);
            return;
        }

        const pristine = $scope.locationtreeData._pristineData || $scope.locationtreeData.data();
        const filteredData = filterNodes(pristine, searchText);

        const filteredDataSource = new kendo.data.HierarchicalDataSource({
            data: filteredData,
            schema: {
                model: {
                    id: "itemCode",
                    children: "Items"
                }
            }
        });

        $scope.tree.setDataSource(filteredDataSource);

        setTimeout(() => {
            $scope.tree.expand(".k-item");
        }, 100);
    };

    //Grid Binding main Part

    $scope.locationOnDataBound = function () {
        $('#locationtree').data("kendoTreeView").expand('.k-item');
    }
    $scope.getDetailByLocationCode = function (locationId) {
        var getLocationdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getLocationDetailsByLocationCode?locationCode=" + locationId;
        $http({
            method: 'GET',
            url: getLocationdetaisByUrl,

        }).then(function successCallback(response) {

            var locationsArr = response.data.DATA.LOCATION_NATURE;

        }, function errorCallback(response) {

        });
    }

    $scope.fillLocationSetupForms = function (locationId) {
        var getLocationdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getLocationDetailsByLocationCode?locationCode=" + locationId;
        $http({
            method: 'GET',
            url: getLocationdetaisByUrl,

        }).then(function successCallback(response) {

            $scope.locationsetup = response.data.DATA;
            $scope.locationsArr = response.data.DATA;
            if ($scope.locationsetup.MU_CODE != null && $scope.locationsetup.MU_CODE != "") {
                var tree = $("#locationMUCode").data("kendoComboBox");
                tree.value($scope.locationsetup.MU_CODE);
            }
            //if ($scope.locationsetup.LOCATION_TYPE_CODE != null && $scope.locationsetup.LOCATION_TYPE_CODE != "") {
            //    var locationType = $("#locationTypeCode").data("kendoComboBox");
            //    locationType.value($scope.locationsetup.LOCATION_TYPE_CODE);
            //}
            locationdataFillDefered.resolve();
            // this callback will be called asynchronously
            // when the response is available
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
    }
    //treeview on select
    $scope.locationoptions = {
        loadOnDemand: false,
        select: function (e) {

            var currentItem = e.sender.dataItem(e.node);
            $('#locationGrid').removeClass("show-displaygrid");
            $("#locationGrid").html("");
            $($(this._current).parents('ul')[$(this._current).parents('ul').length - 1]).find('span').removeClass('hasTreeCustomMenu');
            $(this._current.context).find('span').addClass('hasTreeCustomMenu');
            $scope.locationsetup.LOCATION_CODE = currentItem.LocationId;
            $scope.locationsArr.LOCATION_CODE = currentItem.LocationId;
            $scope.treeselectedlocationCode = currentItem.LocationId;
            $scope.locationsetup.LOCATION_EDESC = currentItem.LocationName;
            $scope.locationsetup.GROUP_SKU_FLAG = "G";
            $scope.treenodeselected = "Y";
            $scope.newrootinsertFlag = "N";

            $scope.$apply();
            $scope.movescrollbar();
        },
    };




    $scope.movescrollbar = function () {
        var element = $(".k-in");
        for (var i = 0; i < element.length; i++) {
            var selectnode = $(element[i]).hasClass("k-state-focused");
            if (selectnode) {
                $("#locationtree").animate({
                    scrollTop: (parseInt(i * 12))
                });
                break;
            }
        }
    }

    $scope.onContextSelect = function (event) {

        $scope.masterlocationdisabled = false;
        if ($scope.locationsetup.LOCATION_CODE == "")
            return displayPopupNotification("Select location.", "error");;
        $scope.saveupdatebtn = "Save";
        if (event.item.innerText.trim() == "Delete") {

            $scope.delete($scope.locationsArr.LOCATION_CODE,"");
        }
        else if (event.item.innerText.trim() == "Update") {

            locationdataFillDefered = $.Deferred();
            $scope.saveupdatebtn = "Update";
            $scope.masterlocationdisabled = true;
            $scope.fillLocationSetupForms($scope.locationsetup.LOCATION_CODE);
            $.when(locationdataFillDefered).then(function () {

                //var tree = $("#masterlocationcode").data("kendoDropDownList");
                //tree.value($scope.treeselectedlocationCode);
                if ($scope.locationsetup.PARENT_LOCATION_CODE == null || $scope.locationsetup.PARENT_LOCATION_CODE == undefined) {
                    var popUpDropdown = $("#masterlocationcode").data("kendoDropDownList");
                    popUpDropdown.value('');
                }
                else {
                    var popUpDropdown = $("#masterlocationcode").data("kendoDropDownList");
                    popUpDropdown.value($scope.locationsetup.PARENT_LOCATION_CODE);
                }
                $("#locationModal").modal();
            })


        }
        else if (event.item.innerText.trim() == "Add") {

            locationdataFillDefered = $.Deferred();
            $scope.savegroup = true;
            $scope.locationsArr = [];

            if ($scope.locationsetup.GROUP_SKU_FLAG == "G") {

                $scope.fillLocationSetupForms($scope.locationsetup.LOCATION_CODE);

                $.when(locationdataFillDefered).then(function () {

                    var tree = $("#masterlocationcode").data("kendoDropDownList");
                    tree.value($scope.locationsetup.LOCATION_CODE);
                    $scope.locationsArr.LOCATION_CODE = $scope.locationsetup.LOCATION_CODE;
                    $scope.locationsArr.GROUP_SKU_FLAG = "G";
                    $scope.clearFields();
                    $("#locationModal").modal();

                })
            }
            else {
                var tree = $("#locationtree").data("kendoTreeView");
                tree.dataSource.read();
                displayPopupNotification("Please Select Location Head First.", "warning");
            }


        }
    }
    $scope.saveNewLocation = function (isValid) {
        debugger
        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }
       /* debugger;*/
        var prelocationcode = $("#masterlocationcode").data("kendoDropDownList").value();
        var model = {
            LOCATION_CODE: $scope.locationsArr.LOCATION_CODE,
            LOCATION_EDESC: $scope.locationsArr.LOCATION_EDESC,
            LOCATION_NDESC: $scope.locationsArr.LOCATION_NDESC,
            PRE_LOCATION_CODE: prelocationcode,
            ADDRESS: $scope.locationsArr.ADDRESS,
            AUTH_CONTACT_PERSON: $scope.locationsArr.AUTH_CONTACT_PERSON,
            TELEPHONE_MOBILE_NO: $scope.locationsArr.TELEPHONE_MOBILE_NO,
            EMAIL: $scope.locationsArr.EMAIL,
            FAX: $scope.locationsArr.FAX,
            LOCATION_TYPE_CODE: $scope.locationsArr.LOCATION_TYPE_CODE,
            GROUP_SKU_FLAG: $scope.locationsArr.GROUP_SKU_FLAG,
            REMARKS: $scope.locationsArr.REMARKS,
            BRANCH_CODE: $scope.locationsArr.BRANCH_CODE,
            STORAGE_CAPACITY: $scope.locationsArr.STORAGE_CAPACITY,
            MU_CODE: $scope.locationsArr.MU_CODE,
            LOCATION_ID: $scope.locationsArr.LOCATION_ID,
        }
       

        if (model.GROUP_SKU_FLAG == "I" && prelocationcode == "") {
            return displayPopupNotification("You can not insert child as group.", "error");
        }
        if ($scope.saveupdatebtn == "Save") {
            debugger
            var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewLocationHead";
            $http({
                method: 'POST',
                url: createUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {
                    $scope.locationsArr = [];
                    if ($scope.locationsetup.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#locationtree").data("kendoTreeView");
                        tree.dataSource.read();
                    }
                    var ddl = $("#masterlocationcode").data("kendoDropDownList");
                    if (ddl != undefined)
                        ddl.dataSource.read();
                    if ($scope.savegroup == false) { $("#locationModal").modal("toggle"); }
                    else { $("#locationModal").modal("toggle"); }
                    var grid = $("#kGrid").data("kendo-grid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }
                    displayPopupNotification("Data succesfully saved ", "success");
                }
               else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Location name already exist please try another location name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Location name already exist please try another location name.", "error");
                }
                else {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }
        else {
            var updateUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateLocationByLocationCode";

            $scope.saveupdatebtn = "Update";
            $http({
                method: 'POST',
                url: updateUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "UPDATED") {
                    $scope.locationsArr = [];
                    if ($scope.locationsetup.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#locationtree").data("kendoTreeView");
                        tree.dataSource.read();
                    }
                    var ddl = $("#masterlocationcode").data("kendoDropDownList");
                    if (ddl != undefined)
                        ddl.dataSource.read();

                    $("#kGrid").data("kendoGrid").dataSource.read();
                    $("#locationModal").modal("toggle");
                   
                    displayPopupNotification("Data succesfully updated ", "success");
                }
                if (response.data.MESSAGE == "ERROR") {
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
                // this callback will be called asynchronously
                // when the response is available
            }, function errorCallback(response) {

                displayPopupNotification("Something went wrong.Please try again later.", "error");
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
        }
    }

    $scope.BindGrid = function (groupId) {
        $(".topsearch").show();
        var url = null;
        if (groupId == "All") {
            if ($('#loctxtSearchString').val() == null || $('#loctxtSearchString').val() == '' || $('#loctxtSearchString').val() == undefined || $('#loctxtSearchString').val() == 'undefined') {
                alert('Input is empty or undefined.');
                return;
            }
            url = "/api/SetupApi/GetAllLocationList?searchtext=" + $('#loctxtSearchString').val();
        }
        else {
            $("#loctxtSearchString").val('');
            url = "/api/SetupApi/GetChildOfLocationByGroup?groupId=" + groupId;
        }

        $timeout(function () {
            var $input = $("#loctxtSearchString");
            if ($input.data("bindGlobalSearch")) return;
            $input.data("bindGlobalSearch", true);

            function debounce(fn, delay) {
                var t;
                return function () {
                    var ctx = this, args = arguments;
                    clearTimeout(t);
                    t = setTimeout(function () { fn.apply(ctx, args); }, delay);
                };
            }

            var triggerGlobal = function () {
                var val = $input.val();
                if (val && val.toString().trim().length > 0) {
                    $scope.$evalAsync(function () { $scope.BindGrid('All'); });
                }
            };

            $input.on('keydown.customerGlobal', function (e) {
                if (e.key === 'Enter' || e.keyCode === 13) {
                    e.preventDefault();
                    triggerGlobal();
                }
            });

            $input.on('input.customerGlobal', debounce(triggerGlobal, 400));
        }, 0);

        $scope.locationChildGridOptions = {

            dataSource: {
                type: "json",
                transport: {
                    read: url,
                },
                pageSize: 50,
                //serverPaging: true,
                serverSorting: true
            },
            scrollable: true,
            height: 450,
            sortable: true,
            resizable: true,
            pageable: true,
            dataBound: function (e) {
                $("#kGrid tbody tr").css("cursor", "pointer");
                DisplayNoResultsFound($('#kGrid'));
                $("#kGrid tbody tr").on('dblclick', function () {
                    var locationCode = $(this).find('td span').html()
                    $scope.edit(locationCode);
                    //var tree = $("#locationtree").data("kendoTreeView");
                    //tree.dataSource.read();
                })
            },
            columns: [

                {
                    field: "LOCATION_CODE",
                    title: "Shortcut",
                    width: "100px"
                },
                {
                    field: "LOCATION_EDESC",
                    title: "Location Name",
                    width: "120px"
                },
              
                {
                    field: "AUTH_CONTACT_PERSON",
                    title: "Contact Person",
                    width: "120px"
                },
                {
                    field: "TELEPHONE_MOBILE_NO",
                    title: "Mobile No.",
                    width: "120px"
                },
                {
                    field: "CREATED_BY",
                    title: "CREATED BY",
                    width: "120px"
                }, {
                    field: "CREATED_DATE",
                    title: "CREATED DATE",
                    template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",
                    width: "120px"
                },
               
                {
                    title: "Action ",
                    template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="edit(dataItem.LOCATION_CODE)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="Delete" ng-click="delete(dataItem.LOCATION_CODE,dataItem.GROUP_SKU_FLAG)"><span class="sr-only"></span> </a>',
                    width: "55px"
                }
            ],


        };

        $scope.onsiteSearch = function ($this) {

            var q = $("#txtSearchString").val();
            var grid = $("#kGrid").data("kendo-grid");
            grid.dataSource.query({
                page: 1,
                pageSize: 50,
                filter: {
                    logic: "or",
                    filters: [
                        { field: "ORDER_NO", operator: "contains", value: q },
                        { field: "ORDER_DATE", operator: "contains", value: q },
                        { field: "CREATED_BY", operator: "contains", value: q }
                    ]
                }
            });
        }
    }

    $scope.showModalForNew = function (event) {

        $scope.editFlag = "N";
        $scope.saveupdatebtn = "Save"


        //var getLocationdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getLocationDetailsByLocationCode?locationCode=" + $scope.locationsetup.LOCATION_CODE;
        var getLocationdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getLocationDetailsByLocationCode?locationCode=" + $scope.treeselectedlocationCode;

        $http({
            method: 'GET',
            url: getLocationdetaisByUrl,

        }).then(function successCallback(response) {

            $scope.locationsetup = response.data.DATA;
            $scope.locationsArr = response.data.DATA;
            if ($scope.locationsetup.GROUP_SKU_FLAG == "G") {
                $scope.locationsetup.GROUP_SKU_FLAG = "I";
                //$scope.saveupdatebtn = "Save"
                $scope.locationsArr.GROUP_SKU_FLAG = "I";
                $scope.masterlocationdisabled = false;
                var tree = $("#masterlocationcode").data("kendoDropDownList");
                tree.value($scope.locationsetup.LOCATION_CODE);
                if ($scope.locationsetup.MU_CODE != null && $scope.locationsetup.MU_CODE != "") {
                    var tree = $("#locationMUCode").data("kendoComboBox");
                    tree.value($scope.locationsetup.MU_CODE);
                }
                if ($scope.locationsetup.LOCATION_TYPE_CODE != null && $scope.locationsetup.LOCATION_TYPE_CODE != "") {
                    var locationType = $("#locationTypeCode").data("kendoComboBox");
                    locationType.value($scope.locationsetup.LOCATION_TYPE_CODE);
                }
                $scope.clearFields();
                $("#locationModal").modal("toggle");
            }
            else {
                var tree = $("#locationtree").data("kendoTreeView");
                tree.dataSource.read();
                displayPopupNotification("Please Select Location Head First.", "error");

            }

            // this callback will be called asynchronously
            // when the response is available
        }, function errorCallback(response) {

            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });

        //}
        //else
        //{
        //    var tree = $("#locationtree").data("kendoTreeView");
        //    tree.dataSource.read();
        //}


    }
    $scope.edit = function (locationCode) {

        $scope.editFlag = "Y";
        $scope.saveupdatebtn = "Update"
        $scope.masterlocationdisabled = true;
        $scope.fillLocationSetupForms(locationCode);
        $scope.groupLocationTypeFlag = "N";
        $scope.locationsetup.GROUP_SKU_FLAG = "I";
        $scope.locationsArr.GROUP_SKU_FLAG = "I";
        var tree = $("#masterlocationcode").data("kendoDropDownList");
        tree.value($scope.treeselectedlocationCode);
        $("#locationModal").modal();
    }
    $scope.delete = function (code, groupSkuFlag) {
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
              
                    if (groupSkuFlag === "I")
                        $scope.locationsetup.GROUP_SKU_FLAG = "I";
                    else
                        $scope.locationsetup.GROUP_SKU_FLAG = "G";
                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteLocationSetupByLocationCode?locationCode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {

                        if (response.data.MESSAGE == "DELETED") {
                         
                            if ($scope.locationsetup.GROUP_SKU_FLAG !== "I") {
                                var tree = $("#locationtree").data("kendoTreeView");
                                tree.dataSource.read();
                            }
                            $("#kGrid").data("kendoGrid").dataSource.read();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        else if (response.data.MESSAGE == "HAS_CHILD") {
                            displayPopupNotification("You can not delete. It has child.", "warning");
                        }
                        $scope.treenodeselected = "N";
                        // this callback will be called asynchronously
                        // when the response is available
                    }, function errorCallback(response) {
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                        // called asynchronously if an error occurs
                        // or server returns response with an error status.
                    });

                }

            }
        });
    }
    $scope.addNewLocation = function () {


        $scope.editFlag = "N";
        $scope.saveupdatebtn = "Save"
        $scope.locationsArr = [];
        $scope.masterlocationdisabled = false;
        $scope.groupLocationTypeFlag = "Y";
        $scope.locationsetup.GROUP_SKU_FLAG = "N";
        $scope.locationsArr.GROUP_SKU_FLAG = "G";
        var tree = $("#masterlocationcode").data("kendoDropDownList");
        tree.value("");
        //var tree = $("#locationtree").data("kendoTreeView");
        //tree.dataSource.read();
        $("#locationModal").modal("toggle");
    }
    $scope.Cancel = function () {
        $scope.saveupdatebtn = "Save";
        $scope.locationsArr = [];
        $("#kGrid").data("kendoGrid").clearSelection();
    }
    function DisplayNoResultsFound(grid) {

        // Get the number of Columns in the grid
        //var grid = $("#kGrid").data("kendo-grid");
        var dataSource = grid.data("kendoGrid").dataSource;
        var colCount = grid.find('.k-grid-header colgroup > col').length;

        // If there are no results place an indicator row
        if (dataSource._view.length == 0) {
            grid.find('.k-grid-content tbody')
                .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" style="text-align:center"><b>No Results Found!</b></td></tr>');
        }

        // Get visible row count
        var rowCount = grid.find('.k-grid-content tbody tr').length;

        // If the row count is less that the page size add in the number of missing rows
        if (rowCount < dataSource._take) {
            var addRows = dataSource._take - rowCount;
            for (var i = 0; i < addRows; i++) {
                //grid.find('.k-grid-content tbody').append('<tr class="kendo-data-row"><td>&nbsp;</td></tr>');
            }
        }
    }

    $scope.Location_Cancel = function () {

        $scope.refresh();
    }

    $scope.refresh = function () {

        var tree = $("#locationtree").data("kendoTreeView");
        tree.dataSource.read();
       
    }


    $scope.clearFields = function () {

        $scope.locationsArr.LOCATION_EDESC = "";
        $scope.locationsArr.LOCATION_NDESC = "";
        $scope.locationsArr.STORAGE_CAPACITY = "";
        $scope.locationsArr.ADDRESS = "";
        $scope.locationsArr.AUTH_CONTACT_PERSON = "";
        $scope.locationsArr.TELEPHONE_MOBILE_NO = "";
        $scope.locationsArr.EMAIL = "";
        $scope.locationsArr.FAX = "";
        $scope.locationsArr.REMARKS = "";
    }
});

