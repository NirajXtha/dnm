
app.controller('FormSetupController', function ($scope, $http, $filter, $q, $document, processSetupBomService) {

    $scope.mappings = [
        { columnName: 'CUSTOMER_CODE', masterTable: 'SA_CUSTOMER_SETUP' },
        { columnName: 'PRIORITY_CODE', masterTable: 'IP_PRIORITY_CODE' },
        { columnName: 'SALES_TYPE_CODE', masterTable: 'SA_SALES_TYPE' },
        { columnName: 'EMPLOYEE_CODE', masterTable: 'SA_SALES_TYPE' },
        { columnName: 'ITEM_CODE', masterTable: 'IP_ITEM_MASTER_SETUP' },
        { columnName: 'MU_CODE', masterTable: 'IP_ITEM_MASTER_SETUP' },
        { columnName: 'SECOND_QUANTITY', masterTable: 'IP_ITEM_UNIT_SETUP' },
        { columnName: 'THIRD_QUANTITY', masterTable: 'IP_ITEM_UNIT_SETUP' },
        { columnName: 'AGENT_CODE', masterTable: 'AGENT_SETUP' },
        { columnName: 'AREA_CODE', masterTable: 'AREA_SETUP' },
        { columnName: 'CURRENCY_CODE', masterTable: 'CURRENCY_SETUP' },
        { columnName: 'LC_NO', masterTable: 'LC_SETUP' },
        { columnName: 'PARTY_TYPE_CODE', masterTable: 'IP_PARTY_TYPE_CODE' },
        { columnName: 'PAYMENT_CODE', masterTable: 'FA_PAYMENT_MODE_COD' },
        { columnName: 'SECTOR_CODE', masterTable: 'SECTOR_SETUP' },
        { columnName: 'SECTOR_NAME', masterTable: 'SECTOR_SETUP' },
        { columnName: 'TRANSPORTER_CODE', masterTable: 'TRANSPORTER_SETUP' }
    ];
    console.log("controller hit");

    //// Query will use for API

    $scope.selectedRowItemDataOnGrid = null;


    $scope.selectedFormModule = null;

    $scope.selectedTreeFormItem = null;
    $scope.moduleId = "";
    $scope.document = {};
    $scope.FormItemEntryUpdateModel = null;
    $scope.is_edit = false;


    $scope.InitDocumetModel = function () {
        // Model for form
        $scope.document = {
            FORM_CODE: "",
            PARENT: "",
            ENG_DESC: "",
            BRANCH_DOC: "",
            NEP_DESC: "",
            MODULE_CODE: "",
            REMARKS: ""
        };
    }; $scope.InitDocumetModel();

    $scope.GetALlFormList = function () {

        $http.get("/api/FormSetupApi/GetAllFormList")
            .then(function (response) {
                if (response.data && response.status === 200) {
                    $scope.moduleList = response.data;
                } else {
                    console.error("Unexpected response:", response);
                }
            })
            .catch(function (error) {
                console.error("Error while fetching form list:", error);
            });

    }; $scope.GetALlFormList();

    $scope.RenderFormTree = function () {
        $("#categoryTreeGrid").html("");
        // 04 will be replace with real model; this should me from current module and probabely needed  from URL 
        var getCategoryTree = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetFormTreeStructureList?moduleId=" + $scope.moduleId;
        var categoryTreeDataSource = new kendo.data.HierarchicalDataSource({
            transport: {
                read: {
                    url: getCategoryTree,
                    dataType: "json"
                }
            },
            schema: {
                model: {
                    id: "FORM_CODE",
                    children: "ITEMS",
                    hasChildren: "HAS_BRANCH"
                },
                parse: function (response) {

                    // recursive function to build tree
                    function buildTree(parentCode) {
                        return _.map(_.filter(response, function (item) {
                            return item.PRE_FORM_CODE === parentCode;
                        }), function (item) {
                            var children = buildTree(item.MASTER_FORM_CODE);
                            item.ITEMS = children;
                            item.HAS_BRANCH = children.length > 0;
                            return item;
                        });
                    }

                    // top-level nodes (PRE_FORM_CODE = '00')
                    return buildTree('00');

                    //return _.each(_.filter(response, function (x) {
                    //    return x.PRE_FORM_CODE === '00';
                    //}), function (y) {
                    //    // console.log(y);
                    //    y.ITEMS = _.filter(response, function (z) {
                    //        return z.PRE_FORM_CODE === y.MASTER_FORM_CODE;
                    //    });
                    //    y.HAS_BRANCH = y.ITEMS.length === 0 ? false : true;
                    //});
                }
            }
        });

        // 🔹 Render / refresh Kendo TreeView
        $("#categoryTreeGrid").kendoTreeView({
            loadOnDemand: false,
            // autoScroll: true,
            // autoBind: true,
            dataSource: categoryTreeDataSource,
            dataTextField: "FORM_EDESC",
            // height: 400,
            select: onCategorySelect,
            //scrollable: {
            //    virtual: true
            //},
            dataBound: function (e) {
                // expand all nodes
                this.expand(".k-item");
            }
        });

        $("#dropdowntree").kendoDropDownList({
            dataTextField: "FORM_EDESC",
            dataValueField: "PRE_FORM_CODE",
            dataSource: categoryTreeDataSource,
            //index: 0,
            change: onChange
        });


    }; //$scope.RenderFormTree();

    $scope.FormattingTab_TableList = [];
    $scope.FormattingTab_SubLedgerList = [];
    $scope.FormattingTab_FormDetailSteupList = [];
    $scope.FormattingTab_UnmappedColumnList = [];

    $scope.FormattingTab_NumberList = [
        { key: "123456789.00", name: "123456789.00" },
        { key: "123,456,789.00", name: "123,456,789.00" },
        { key: "12,34,56,789.00", name: "12,34,56,789.00" }
    ];

    $scope.FormattingTab_DateFormatList = [
        { key: "M/d/yyyy", name: "M/d/yyyy" },
        { key: "M/d/yy", name: "M/d/yy" },
        { key: "MM/dd/yy", name: "MM/dd/yy" },
        { key: "yy/MM/dd", name: "yy/MM/dd" },
        { key: "dd-MMM-yyyy", name: "dd-MMM-yyyy" }
    ];

    // Sample data structure based on your image
    $scope.checkingOptions = [
        { value: '000', name: 'NONE' },
        { value: '001', name: 'Posting' },
        { value: '011', name: 'Verifying / Posting' },
        { value: '111', name: 'Checking / Verifying / Posting' },
        { value: '110', name: 'Checking / Posting' },
        { value: '100', name: 'Checking' }
    ];

    // Define your document type options
    $scope.documentTypeOptions = [
        { value: 'JV', name: 'JV' },
        { value: 'CH', name: 'Cash' },
        { value: 'BK', name: 'Bank' },
        { value: 'PV', name: 'Post JV' },
        { value: 'OT', name: 'Others' }
    ];


    $scope.dynamicListObj = [];

    $scope.FormattingTab_SaveToObj = {};
    $scope.subLedgerSearchText = "";

    $scope.customerSearchText = "";
    function onChange(e) {
    }
    function onCategorySelect(e) {
        debugger;
        var treeview = $("#categoryTreeGrid").data("kendoTreeView");
        var item = treeview.dataItem(e.node);
        console.log("selected category : " + JSON.stringify(item));

        if (item) {
            console.log(item);
            $scope.selectedTreeFormItem = item;
            //$scope.BindFormSetupResultGrid();
            $scope.BindFormSetupResultGrid();
            //return;
            //var formSetupResultGrid = $("#formSetupResultGrid").data("kendoGrid");
            //formSetupResultGrid.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/ProcessSetupBomApi/GetRoutineBasedOnProcessCode?processCode=" + selectedItemObj.PROCESS_CODE;
            //formSetupResultGrid.dataSource.read();
        } else {
            selectedItem.pop();
        }
    }
    function filter(dataSource, query) {
        var hasVisibleChildren = false;
        var data = dataSource instanceof kendo.data.HierarchicalDataSource && dataSource.data();

        for (var i = 0; i < data.length; i++) {
            var item = data[i];
            var text = item.PROCESS_EDESC.toLowerCase();
            var itemVisible =
                query === true // parent already matches
                || query === "" // query is empty
                || text.indexOf(query) >= 0; // item text matches query

            var anyVisibleChildren = filter(item.children, itemVisible || query); // pass true if parent matches

            hasVisibleChildren = hasVisibleChildren || anyVisibleChildren || itemVisible;

            item.hidden = !itemVisible && !anyVisibleChildren;
        }

        if (data) {
            // Re-apply the filter on the children.
            dataSource.filter({ field: "hidden", operator: "neq", value: true });
        }

        return hasVisibleChildren;
    }

    var selectedItemList = [];

    $scope.BindFormSetupResultGrid = function () {
        $("#formSetupResultGrid").html("");
        $scope.formSetupResultOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: {
                        url: "/api/FormSetupApi/GetFormListByPreFormCode",
                        dataType: "json",
                        data: function () {
                            return {
                                preFormCode: $scope.selectedTreeFormItem.MASTER_FORM_CODE,
                                moduleId: $scope.moduleId
                            };
                        }
                    }
                },
                pageSize: 20,
                serverSorting: true,
                schema: {
                    data: function (response) {
                        // 🔹 here you can remove unwanted properties
                        return $.map(response, function (item) {
                            return {
                                FORM_CODE: item.FORM_CODE,
                                FORM_EDESC: item.FORM_EDESC,
                                MODULE_CODE: item.MODULE_CODE,
                                NUMBERING_FORMAT: item.NUMBERING_FORMAT,
                                DATE_FORMAT: item.DATE_FORMAT,
                                START_ID_FLAG: item.START_ID_FLAG,
                                ID_GENERATION_FLAG: item.ID_GENERATION_FLAG,
                                CUSTOM_PREFIX_TEXT: item.CUSTOM_PREFIX_TEXT,
                                CUSTOM_SUFFIX_TEXT: item.CUSTOM_SUFFIX_TEXT,
                                PREFIX_LENGTH: item.PREFIX_LENGTH,
                                SUFFIX_LENGTH: item.SUFFIX_LENGTH,
                                BODY_LENGTH: item.BODY_LENGTH,
                                START_NO: item.START_NO,
                                LAST_NO: item.LAST_NO,
                                // 🔹 Convert string → Date
                                START_DATE: kendo.parseDate(item.START_DATE),
                                LAST_DATE: kendo.parseDate(item.LAST_DATE),
                                PRINT_REPORT_FLAG: item.PRINT_REPORT_FLAG,
                                COPY_VALUES_FLAG: item.COPY_VALUES_FLAG,
                                QUALITY_CHECK_FLAG: item.QUALITY_CHECK_FLAG,
                                SERIAL_TRACKING_FLAG: item.SERIAL_TRACKING_FLAG,
                                BATCH_TRACKING_FLAG: item.BATCH_TRACKING_FLAG,
                                REMARKS: item.REMARKS
                            };
                        });
                    }
                }
            },

            selectable: "single",
            resizable: false,
            scrollable: true,
            pageable: true,
            height: 400,
            groupable: false,
            // change: formSetupResultRowSelected,
            dataBound: function (e) {
                var grid = this; // current grid instance

                // make rows clickable
                $("#formSetupResultGrid tbody tr").css("cursor", "pointer");

                // attach click event to rows
                $("#formSetupResultGrid tbody tr").off("click").on("click", function () {
                    var dataItem = grid.dataItem($(this)); // get the clicked row's data

                    $scope.selectedRowItemDataOnGrid = dataItem;


                    $scope.$apply();
                    // highlight selected row (optional)
                    $("#formSetupResultGrid tbody tr").removeClass("k-state-selected");
                    $(this).addClass("k-state-selected");

                    // now you can access values
                    console.log("Selected Item:", dataItem);
                    console.log("FORM_CODE:", dataItem.FORM_CODE);

                    // for example, set value using jQuery
                    $("#txtFormCode").val(dataItem.FORM_CODE);
                });
            },
            columns: [
                { field: "FORM_CODE", title: "Shortcut", width: 100 },
                { field: "FORM_EDESC", title: "Description", width: 200 },
                { field: "MODULE_CODE", title: "Module", width: 100 },
                { field: "NUMBERING_FORMAT", title: "Numbering Format", width: 100 },
                { field: "DATE_FORMAT", title: "Date Format", width: 100 },
                { field: "START_ID_FLAG", title: "Trans. no restart", width: 100 },
                { field: "ID_GENERATION_FLAG", title: "ID Style", width: 100 },
                { field: "CUSTOM_PREFIX_TEXT", title: "Prefix", width: 100 },
                { field: "CUSTOM_SUFFIX_TEXT", title: "Suffix", width: 100 },
                { field: "PREFIX_LENGTH", title: "Prefix Length", width: 100 },
                { field: "SUFFIX_LENGTH", title: "Suffix Length", width: 100 },
                { field: "BODY_LENGTH", title: "Body Length", width: 100 },
                { field: "START_NO", title: "Trans. Start No.", width: 100 },
                { field: "LAST_NO", title: "Trans. End No.", width: 100 },
                { field: "START_DATE", title: "Trans. Start Date", format: "{0:dd/MM/yyyy}", width: 100 },
                { field: "LAST_DATE", title: "Trans. End Date", format: "{0:dd/MM/yyyy}", width: 100 },
                { field: "PRINT_REPORT_FLAG", title: "Print Report", width: 100 },
                { field: "COPY_VALUES_FLAG", title: "Copy Values", width: 100 },
                { field: "QUALITY_CHECK_FLAG", title: "Quality Check", width: 100 },
                { field: "SERIAL_TRACKING_FLAG", title: "Serial Track", width: 100 },
                { field: "BATCH_TRACKING_FLAG", title: "Batch Track", width: 100 },
                { field: "REMARKS", title: "Remarks", width: 100 }
            ]
        };

        // 🔹 now create/refresh the grid
        $("#formSetupResultGrid").kendoGrid($scope.formSetupResultOptions);
    };

    function formSetupResultRowSelected(e) {
        debugger;
        var grid = $("#formSetupResultGrid").data("kendoGrid");
        var selectedItem = grid.dataItem(grid.select());

        if (selectedItem) {
            $scope.selectedRowItemDataOnGrid = selectedItem;

        } else {
            selectedItem.pop();
        }
    }


    $scope.onModuleChange = function (m) {
        $scope.moduleId = $scope.selectedFormModule.MODULE_CODE;
        $scope.RenderFormTree();
    };


    $scope.GetNextFormCode = function () {
        $scope.InitDocumetModel();
        $scope.OpenCreateFormPopup();
        //var getNextFormCodeUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetNextFormCode";

        //$http.get(getNextFormCodeUrl)
        //    .then(function (response) {
        //        if (response && response.data) {
        //            $scope.nextFormCode = response.data;
        //            $scope.document.FORM_CODE = response.data;
        //            $scope.document.PARENT = $scope.selectedTreeFormItem.FORM_EDESC;


        //            // ✅ Open popup after API success
        //            $('#documentGroupModal').modal('show');
        //        }
        //    })
        //    .catch(function (error) {
        //        console.error("Error while getting next form code:", error);
        //    });
    };


    $scope.OpenCreateFormPopup = function () {
        var baseUrl = window.location.protocol + "//" + window.location.host;

        var nextFormCodeUrl = baseUrl + "/api/FormSetupApi/GetNextFormCode";
        var moduleListUrl = baseUrl + "/api/FormSetupApi/GetAllModuleList";

        // Call both API requests together
        $q.all([
            $http.get(nextFormCodeUrl),
            $http.get(moduleListUrl)
        ])
            .then(function (responses) {
                console.log(responses);

                var nextFormCodeResponse = responses[0].data;
                var moduleListResponse = responses[1].data;

                // Assign results to scope
                $scope.nextFormCode = nextFormCodeResponse;
                $scope.modules = moduleListResponse;

                $scope.document.FORM_CODE = nextFormCodeResponse;
                $scope.document.PARENT = $scope.selectedTreeFormItem.FORM_EDESC;


                // ✅ Open popup after both requests complete
                $('#documentGroupModal').modal('show');
            })
            .catch(function (error) {
                console.error("Error while loading data:", error);
                alert("Error while loading form data.");
            });
    };



    $scope.GetBranchList = function () {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetAllBranchList" + "?form_code=" + $scope.document.FORM_CODE;

        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.branches = response.data;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
                alert("Error while loading branch data.");
            });
    }

    // Save button action
    $scope.saveDocument = function (doc) {
        $scope.InsertForm();
    };

    $scope.InsertForm = function () {
        debugger;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/InsertFormSetupGroupedItem";

        var MASTER_ITEM_CODE = $scope.selectedTreeFormItem.MASTER_FORM_CODE;
        var formData = {
            FORM_CODE: $scope.document.FORM_CODE,
            FORM_EDESC: $scope.document.ENG_DESC,
            FORM_NDESC: $scope.document.NEP_DESC,
            //MASTER_FORM_CODE: "",
            PRE_FORM_CODE: MASTER_ITEM_CODE,
            REMARKS: $scope.document.REMARKS,
            MODULE_CODE: $scope.document.MODULE_CODE,
            IS_EDIT: $scope.is_edit
        };

        console.log(formData);

        //return;
        $http.post(apiUrl, formData)
            .then(function (response) {
                $scope.onModuleChange(undefined);
                $('#documentGroupModal').modal('hide');
                displayPopupNotification(response.data.MESSAGE, "success");
            })
            .catch(function (error) {
                console.error("Error inserting form:", error);
                alert("Error while saving form.");
            });
    };

    $scope.EditGroupedTreeItem = function () {
        $scope.is_edit = true;
        console.log($scope.selectedTreeFormItem);

        var baseUrl = window.location.protocol + "//" + window.location.host;
        var moduleListUrl = baseUrl + "/api/FormSetupApi/GetAllModuleList";

        // Call both API requests together
        $q.all([
            $http.get(moduleListUrl)
        ])
            .then(function (responses) {

                var moduleListResponse = responses[0].data;

                $scope.modules = moduleListResponse;
                $scope.document.FORM_CODE = $scope.selectedTreeFormItem.FORM_CODE;
                $scope.document.PARENT = $scope.selectedTreeFormItem.FORM_EDESC;
                $scope.document.MODULE_CODE = $scope.selectedFormModule.MODULE_CODE;

                //$scope.document = {
                //    FORM_CODE: "",
                //    PARENT: "",
                //    ENG_DESC: "",
                //    BRANCH_DOC: "",
                //    NEP_DESC: "",
                //    MODULE_CODE: "",
                //    REMARKS: ""
                //};

                $scope.document.ENG_DESC = $scope.selectedTreeFormItem.FORM_EDESC;
                $scope.document.NEP_DESC = $scope.selectedTreeFormItem.FORM_NDESC;
                $scope.document.PRE_FORM_CODE = $scope.selectedTreeFormItem.PRE_FORM_CODE;

                $scope.document.REMARKS = $scope.selectedTreeFormItem.REMARKS;
                // ✅ Open popup after both requests complete
                $('#documentGroupModal').modal('show');
            })
            .catch(function (error) {
                console.error("Error while loading data:", error);
                alert("Error while loading form data.");
            });

    };



    $scope.tabs = [
        { title: "Information" },
        { title: "Formatting" },
        { title: "Reference" },
        { title: "Numbering" },
        { title: "Charge Setup" },
        { title: "Quality Check" },
        { title: "Custom" },
        { title: "Quick Info/Validation" },
        { title: "Miscellaneous" }
    ];


    $scope.currentTab = "Information";
    $scope.selectedTab = 0;

    $scope.tabOptions = {
        animation: {
            open: { effects: "fadeIn" }
        }
    };

    // Close dropdown when clicking outside
    $document.on('click', function (event) {
        const isDropdownClick = $(event.target).closest('.dropdown-menu').length > 0;
        const isButtonClick = $(event.target).closest('.emoji-btn').length > 0;

        if (!isDropdownClick && !isButtonClick) {
            $scope.$apply(function () {
                //$scope.activeDropdown.show = false;
            });
        }
    });

    $scope.IsDuplicate = false;
    // Open modal and initialize Kendo TabStrip
    $scope.OpenTabPopup = function (isDuplicate) {
        $scope.IsDuplicate = isDuplicate;


        $scope.GetInitialInformationTabInfo();

        $('#tabPopupModal').modal('show');
        $scope.selectedTab = 0;
        setTimeout(function () {
            $scope.tabStrip = $("#tabPopupModal").find("[kendo-tab-strip]").data("kendoTabStrip");
            if ($scope.tabStrip) $scope.tabStrip.select($scope.selectedTab);
        }, 300);
    };

    //$scope.GetInitialInformationTabInfo = function () {

    //    //var is = $scope.IsDuplicate;


    //    var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
    //    var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetInitialInformationTabInfo" + "?form_code=" + form_code;
    //    //$scope.FormItemEntryUpdateModel.FORM_TYPE = "OT";
    //    $http.get(apiUrl)
    //        .then(function (response) {
    //            if (response && response.data) {
    //                $scope.FormItemEntryUpdateModel = response.data;

    //                var apiValue = response.data.FORM_TYPE;
    //                $('input[name="documentType"][value="' + apiValue + '"]')
    //                    .prop('checked', true)
    //                    .closest('div.radio')
    //                    .find('span')
    //                    .addClass('checked');
    //            }
    //        })
    //        .catch(function (error) {
    //            console.error("Error while fetching branch list:", error);
    //            alert("Error while loading branch data.");
    //        });
    //};

    function setDocumentType(formType) {
        if (!formType) return;

        var radio = $('input[name="documentType"][value="' + formType + '"]');
        radio.prop('checked', true);
        radio.closest('div.radio')
            .find('span')
            .addClass('checked');
    }


    $scope.GetInitialInformationTabInfo = function () {

        if (!$scope.selectedRowItemDataOnGrid) {
            console.warn("No row selected");
            return;
        }

        var requestParams = {
            form_code: $scope.selectedRowItemDataOnGrid.FORM_CODE,
            is_duplicate: $scope.IsDuplicate
            // Add more parameters here when needed
            // company_code: $scope.selectedRowItemDataOnGrid.COMPANY_CODE,
            // branch_code: $scope.selectedRowItemDataOnGrid.BRANCH_CODE,
            // isDuplicate: $scope.IsDuplicate
        };

        var apiUrl = window.location.protocol + "//" + window.location.host +
            "/api/FormSetupApi/GetInitialInformationTabInfo";

        $http.get(apiUrl, { params: requestParams })
            .then(function (response) {
                if (!response || !response.data) {
                    return;
                }

                // Bind API response to model
                $scope.FormItemEntryUpdateModel = response.data;

                // Set radio button value
                setDocumentType(response.data.FORM_TYPE);
            })
            .catch(function (error) {
                console.error("Error while fetching initial information:", error);
                alert("Error while loading initial information.");
            });
    };


    $scope.isShowSaveSuccessMsg = true;

    $scope.saveInformationTabData = function () {
        var apiUrlUpdate = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/UpdateFormSetupGridItemInformationTabData";
        //return;
        $http.post(apiUrlUpdate, $scope.FormItemEntryUpdateModel)
            .then(function (response) {
                if ($scope.isShowSaveSuccessMsg) {
                    displayPopupNotification(response.data.MESSAGE, "success");



                    if ($scope.IsDuplicate) {
                        $scope.LoadFormattingData();
                    }


                }
            })
            .catch(function (error) {
                console.error("Error inserting form:", error);
                //alert("Error while saving form.");
            });
    };


    // #region For Formatting Tab

    $scope.GetSubLedgerList = function () {
        var subLedgerSearchText = $scope.subLedgerSearchText;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetSubLedgerList?searchText=" + subLedgerSearchText;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.FormattingTab_SubLedgerList = response.data;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
                alert("Error while loading branch data.");
            });
    };

    $scope.GetTableListData = function () {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetTransactionTableList";
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.FormattingTab_TableList = response.data;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
                alert("Error while loading branch data.");
            });
    };

    $scope.GetFormattingTabEditData = function () {
        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetDataForFormattingTab?form_code=" + form_code;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    console.log(response);
                    $scope.FormattingTab_SaveToObj = response.data;

                    if ($scope.FormattingTab_SaveToObj.COINAGE_SUB_CODE) {
                        var tmpData = $scope.FormattingTab_SubLedgerList.filter(function (item) {
                            return item.SUB_CODE === $scope.FormattingTab_SaveToObj.COINAGE_SUB_CODE;
                        });
                        if (tmpData) {
                            $scope.FormattingTab_SaveToObj.SUB_LEDGER_NAME = tmpData[0].SUB_EDESC;
                        }
                    }
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
                alert("Error while loading branch data.");
            });
    };

    $scope.GetFormDetailSetupList = function () {
        if ($scope.FormattingTab_FormDetailSteupList.length == 0) {
            var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
            var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetFormDetailSetupList?form_code=" + form_code;
            $http.get(apiUrl)
                .then(function (response) {
                    if (response && response.data) {
                        $scope.FormattingTab_FormDetailSteupList = response.data;
                        // $scope.FormattingTab_UnmappedColumnList = response.data;
                    }
                })
                .catch(function (error) {
                    console.error("Error while fetching branch list:", error);
                });
        }
    };

    $scope.addingColname = "";
    $scope.openConfirm = function (colname) {
        $scope.addingColname = colname;
        $('#confirmModal').modal('show');
    };


    $scope.onConfirm = function (choice) {
        $('#confirmModal').modal('hide');
        if (choice === 'Y') {
            console.log("You selected Master.");
            $scope.AddUnMappedColumnFinal($scope.addingColname, "M");
        } else if (choice === 'N') {
            console.log("You selected Child.");
            $scope.AddUnMappedColumnFinal($scope.addingColname, "C");
        }
    };

    $scope.FormattingTab_GetUnmappedColumnList = function () {
        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetUnmappedColumnList?form_code=" + form_code;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.FormattingTab_UnmappedColumnList = response.data;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });
    };


    $scope.AddUnMappedColumnFinal = function (colname, childParentValue) {

        debugger;

        var nextSerial = Math.max(...$scope.FormattingTab_FormDetailSteupList.map(x => x.SERIAL_NO)) + 1;
        var blnk_obj = {
            "SERIAL_NO": nextSerial,
            "TABLE_NAME": "",
            "COLUMN_NAME": colname,
            "COLUMN_HEADER": "",
            "COLUMN_WIDTH": null,
            "TOP_POSITION": null,
            "LEFT_POSITION": null,
            "MASTER_CHILD_FLAG": childParentValue,
            "IS_DESC_FLAG": "N",
            "DEFA_VALUE": null,
            "DISPLAY_FLAG": "",
            "FILTER_VALUE": null
        };

        $scope.FormattingTab_FormDetailSteupList.push(blnk_obj);
    };

    $scope.AddUnMappedColumn = function (colname) {

        if (confirm("Are you sure you want to add this item?")) {
            $scope.openConfirm(colname);
        } else {
            console.log("User cancelled action.");
        }
    };

    $scope.closeSubLedgerModel = function () {
        $('#subLedgerModal').modal('hide');
    };

    $scope.FormattingTab_OnSubLedgerRowDoubleClick = function (item) {
        $scope.FormattingTab_SaveToObj.COINAGE_SUB_CODE = item.COINAGE_SUB_CODE;
        $scope.FormattingTab_SaveToObj.SUB_LEDGER_NAME = item.SUB_EDESC;
        $('#subLedgerModal').modal('hide');
    };

    $scope.typeListMasterChild = [
        { Key: "M", Value: "Master" },
        { Key: "C", Value: "Child" }
    ];

    $scope.typeListYesNo = [
        { Key: "Y", Value: "Yes" },
        { Key: "N", Value: "No" }
    ];

    $scope.currentDropDownIndex = -1;
    $scope.currentColumnName = "";
    $scope.showDropdown = function ($event, item, index) {
        $scope.currentDropDownIndex = index;
        $scope.currentColumnName = item.COLUMN_NAME;

        $scope.FormattingTab_dropDownItems = [];
        console.log(item.COLUMN_NAME);
        // if ($scope.FormattingTab_dropDownItems == null || $scope.FormattingTab_dropDownItems.length == 0) {
        if (item.COLUMN_NAME == 'CUSTOMER_CODE') {
            $scope.FormattingTab_GetCustomerListData();
        }

        if (item.COLUMN_NAME == 'ITEM_CODE') {
            $scope.FormattingTab_GetItemMasterLookupList();
        }

        if (item.COLUMN_NAME == 'EMPLOYEE_CODE') {
            $scope.FormattingTab_GetEmployeeLookupList();
        }

        if (item.COLUMN_NAME == 'PRIORITY_CODE') {
            $scope.FormattingTab_GetPRIORITY_CODELookupList();
        }

        if (item.COLUMN_NAME == 'CURRENCY_CODE') {
            $scope.FormattingTab_GetCURRENCY_CODELookupList();
        }

        if (item.COLUMN_NAME == 'MU_CODE') {
            $scope.FormattingTab_GetMU_CODELookupList();
        }

        if (item.COLUMN_NAME == 'SALES_TYPE_CODE') {
            $scope.FormattingTab_GetSALES_TYPE_CODELookupList();
        }



    };

    $scope.FormattingTab_GetSALES_TYPE_CODELookupList = function () {

        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetSalesTypeList";
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    var listData = response.data;
                    var newList = [];
                    angular.forEach(listData, function (item) {
                        newList.push({
                            VALUE_CODE: item.SALES_TYPE_CODE,
                            VALUE_EDESC: item.SALES_TYPE_EDESC
                        });
                    });

                    $scope.FormattingTab_dropDownItems = newList;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });
    };

    $scope.FormattingTab_GetMU_CODELookupList = function () {

        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetMUList";
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    var listData = response.data;
                    var newList = [];
                    angular.forEach(listData, function (item) {
                        newList.push({
                            VALUE_CODE: item.MU_CODE,
                            VALUE_EDESC: item.MU_EDESC
                        });
                    });

                    $scope.FormattingTab_dropDownItems = newList;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });

    };

    $scope.FormattingTab_GetCURRENCY_CODELookupList = function () {

        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetCurrencyList";
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {

                    //{
                    //    "CURRENCY_CODE": "1524",
                    //    "CURRENCY_EDESC": "Center For Regenarative Medicine Pvt. Ltd. - Tokha, Kathmandu"
                    //},

                    var listData = response.data;
                    var newList = [];
                    angular.forEach(listData, function (item) {
                        newList.push({
                            VALUE_CODE: item.CURRENCY_CODE,
                            VALUE_EDESC: item.CURRENCY_EDESC
                        });
                    });

                    $scope.FormattingTab_dropDownItems = newList;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });

    };

    $scope.FormattingTab_GetPRIORITY_CODELookupList = function () {

        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetPriorityList";
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {

                    //{
                    //    "PRIORITY_CODE": "1524",
                    //    "PRIORITY_EDESC": "Center For Regenarative Medicine Pvt. Ltd. - Tokha, Kathmandu"
                    //},

                    var listData = response.data;
                    var newList = [];
                    angular.forEach(listData, function (item) {
                        newList.push({
                            VALUE_CODE: item.PRIORITY_CODE,
                            VALUE_EDESC: item.PRIORITY_EDESC
                        });
                    });

                    $scope.FormattingTab_dropDownItems = newList;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });

    };

    $scope.FormattingTab_GetCustomerListData = function () {
        // if ($scope.FormattingTab_dropDownItems.length==0) {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetCustomerList";
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {

                    //{
                    //    "CUSTOMER_CODE": "1524",
                    //    "CUSTOMER_EDESC": "Center For Regenarative Medicine Pvt. Ltd. - Tokha, Kathmandu"
                    //},

                    var listData = response.data;
                    var newList = [];
                    angular.forEach(listData, function (item) {
                        newList.push({
                            VALUE_CODE: item.CUSTOMER_CODE,
                            VALUE_EDESC: item.CUSTOMER_EDESC
                        });
                    });

                    $scope.FormattingTab_dropDownItems = newList;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });
        // }
    };

    $scope.FormattingTab_GetEmployeeLookupList = function () {
        // if ($scope.FormattingTab_dropDownItems.length==0) {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetEmployeeList";
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {

                    ////{
                    ////    "EMPLOYEE_CODE": "1524",
                    ////    "EMPLOYEE_EDESC": "Center For Regenarative Medicine Pvt. Ltd. - Tokha, Kathmandu"
                    ////},

                    var listData = response.data;
                    var newList = [];
                    angular.forEach(listData, function (item) {
                        newList.push({
                            VALUE_CODE: item.EMPLOYEE_CODE,
                            VALUE_EDESC: item.EMPLOYEE_EDESC
                        });
                    });

                    $scope.FormattingTab_dropDownItems = newList;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });
        // }
    };

    $scope.prepareValueList = function (items) {

    };

    $scope.FormattingTab_GetItemMasterLookupList = function () {
        // if ($scope.FormattingTab_dropDownItems.length==0) {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetItemMasterLookupList";
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    //{
                    //    "ITEM_CODE": "49",
                    //        "ITEM_EDESC": "BEARING 6314 ZZ"
                    //},
                    var listData = response.data;
                    var newList = [];
                    angular.forEach(listData, function (item) {
                        newList.push({
                            VALUE_CODE: item.ITEM_CODE,
                            VALUE_EDESC: item.ITEM_EDESC
                        });
                    });
                    $scope.FormattingTab_dropDownItems = newList;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });
        // }
    };

    $scope.setDropDownDataInList = function (item) {
        $scope.FormattingTab_FormDetailSteupList[$scope.currentDropDownIndex].DEFA_VALUE_DESC = item.VALUE_EDESC;
        $scope.FormattingTab_FormDetailSteupList[$scope.currentDropDownIndex].DEFA_VALUE = item.VALUE_CODE;
    };

    // 1️⃣ Function to load lookup list only once
    $scope.lookupCache = {}; // cache to avoid reloading same data
    $scope.columnList = []; // cache to avoid reloading same data

    $scope.loadLookupList = function (columnName, code) {
        columnName = columnName.toUpperCase();

        // If already loaded, return cached promise
        if ($scope.lookupCache[code]) {
            return $scope.lookupCache[code];
        }

        let apiUrl = '';
        switch (columnName) {
            case 'CURRENCY_CODE':
                apiUrl = '/api/FormSetupApi/GetCurrencyCodeDESCValue?code=' + code;
                break;
            //case 'MU_CODE':
            //    apiUrl = '/api/FormSetup/GetMUList';
            //    break;
            //case 'SALES_TYPE_CODE':
            //    apiUrl = '/api/FormSetup/GetSalesTypeList';
            //    break;
            //case 'PRIORITY_CODE':
            //    apiUrl = '/api/FormSetup/GetPriorityList';
            //    break;
            default:
                return Promise.resolve([]);
        }

        // Fetch and store promise in cache
        const promise = $http.get(apiUrl).then(function (res) {
            return res.data;
        });
        // $scope.lookupCache[code] = promise;
        return promise;
    };

    // 2️⃣ Function to get EDESC by CODE (async-friendly)
    $scope.getCodeDescValue = function (codeValue, columnName) {

        console.log(columnName);

        if (!codeValue || !columnName) return '';


        if ($scope.columnList.indexOf('columnName') > -1) {
            return "";
        }

        $scope.columnList.push(columnName);

        codeValue = codeValue.toUpperCase();

        // Load lookup list (if not already loaded)
        $scope.loadLookupList(columnName, codeValue).then(function (dataValue) {
            //if (!list || list.length === 0) return;

            let found;
            switch (columnName.toUpperCase()) {
                case 'CURRENCY_CODE':
                    found = dataValue;
                    break;
                //case 'MU_CODE':
                //    found = list.find(x => x.MU_CODE === codeValue);
                //    break;
                //case 'SALES_TYPE_CODE':
                //    found = list.find(x => x.SALES_TYPE_CODE === codeValue);
                //    break;
                //case 'PRIORITY_CODE':
                //    found = list.find(x => x.PRIORITY_CODE === codeValue);
                //    break;
            }

            if (found) {
                // alert(found);
                return found;
            }
        });


    };

    $scope.IsShowDefaultInput = function (columnName) {
        return $scope.mappings.some(function (mapping) {
            if (mapping.columnName === columnName) {
                // $scope.FormattingTab_GetCustomerListData();
                $scope.dynamicListObj.push({ [columnName]: [] });

                return true;
            }
        });
    };

    $scope.LoadFormattingData = function () {
        $scope.currentTab = "Formatting";
        $scope.GetSubLedgerList();
        $scope.GetTableListData();
        $scope.GetFormDetailSetupList();
        $scope.FormattingTab_GetUnmappedColumnList();
        setTimeout(function () {
            $scope.GetFormattingTabEditData();
        }, 500);
    };

    $scope.CheckFormattingTabSaveData = function () {

        var invalidItems = [];

        angular.forEach($scope.FormattingTab_FormDetailSteupList, function (item, index) {
            // Check if any of the target properties are missing or invalid (0 is allowed)
            if (
                item.COLUMN_HEADER === null || item.COLUMN_HEADER === undefined || item.COLUMN_HEADER === '' ||
                (item.COLUMN_WIDTH === null || item.COLUMN_WIDTH === undefined || item.COLUMN_WIDTH === '') ||
                (item.TOP_POSITION === null || item.TOP_POSITION === undefined || item.TOP_POSITION === '') ||
                (item.LEFT_POSITION === null || item.LEFT_POSITION === undefined || item.LEFT_POSITION === '')
            ) {
                invalidItems.push(index + 1); // record invalid row index
            }
        });

        if (invalidItems.length > 0) {
            alert("Please add values in following record(s): " + invalidItems.join(", "));
            return false;
        }
        //alert("Validation successful!");
        return true;
    }

    $scope.saveFormattingTabData = function () {
        var v = $scope.CheckFormattingTabSaveData();
        if (v) {
            var apiUrlUpdate = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/UpdateFormatingTabInfo";
            var saveToObj = {
                FormDataInfoModel: $scope.FormItemEntryUpdateModel,
                FormattingFormDetailSetupList: $scope.FormattingTab_FormDetailSteupList,
                FormFormattingModel: $scope.FormattingTab_SaveToObj
            };
            $http.post(apiUrlUpdate, saveToObj)
                .then(function (response) {
                    if ($scope.isShowSaveSuccessMsg) {
                        displayPopupNotification(response.data.MESSAGE, "success");
                    }
                })
                .catch(function (error) {
                    console.error("Error inserting form:", error);
                    //alert("Error while saving form.");
                });
        }
    };

    $scope.selectedCustomerCategoryData = {};
    $scope.selectedCustomer = {};
    $scope.loadCustomerTree = function () {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetCustomerTreeStructureList";
        $http.get(apiUrl)
            .then(function (response) {
                $("#customerTreeView").kendoTreeView({
                    dataSource: response.data,
                    dataTextField: "text",
                    // Add this for click/selection event
                    select: function (e) {
                        var treeView = $("#customerTreeView").data("kendoTreeView");
                        var selectedNode = treeView.dataItem(e.node);

                        // Example: log the selected item
                        console.log("Selected node:", selectedNode);
                        $scope.selectedCustomerCategoryData = selectedNode;
                        //// Example: if you want to use Angular scope variables
                        //$scope.$applyAsync(function () {
                        //    $scope.selectedCustomer = {
                        //        id: selectedNode.id, // adjust field name if different
                        //        text: selectedNode.text
                        //    };
                        //});

                        $scope.getSelectedCustomerDetailList(selectedNode.id);
                    }
                });
            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };


    $scope.loadItemTree = function () {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetItemGroupList";
        $http.get(apiUrl)
            .then(function (response) {
                $("#customerTreeView").kendoTreeView({
                    dataSource: response.data,
                    dataTextField: "text",
                    // Add this for click/selection event
                    select: function (e) {
                        var treeView = $("#customerTreeView").data("kendoTreeView");
                        var selectedNode = treeView.dataItem(e.node);

                        // Example: log the selected item
                        console.log("Selected node:", selectedNode);
                        $scope.selectedCustomerCategoryData = selectedNode;
                        //// Example: if you want to use Angular scope variables
                        //$scope.$applyAsync(function () {
                        //    $scope.selectedCustomer = {
                        //        id: selectedNode.id, // adjust field name if different
                        //        text: selectedNode.text
                        //    };
                        //});

                        $scope.getSelectedCustomerDetailList(selectedNode.id);
                    }
                });
            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };

    $scope.showFilterTreeModal = function ($event, item, index) {
        alert(item.COLUMN_NAME);
        $scope.currentDropDownIndex = index;
        $scope.currentColumnName = item.COLUMN_NAME;

        if (item.COLUMN_NAME == 'CUSTOMER_CODE') {
            $("#customerTreeView").html("");
            $scope.loadCustomerTree();
            $('#TreeViewFormattingTabPopupModel').modal('show');
        }

        if (item.COLUMN_NAME == 'ITEM_CODE') {
            $("#ItemGroupedTreeView").html("");
            $scope.LoadItemGroupedTree();

        }

        if (item.COLUMN_NAME == 'EMPLOYEE_CODE') {
            $("#EmployeeTreeView").html("");
            $scope.LoadEmployeeGroupedTree();
        }

    };

    $scope.closePopupModel = function (popupId) {
        $('#' + popupId).modal('hide');
    };

    $scope.PreCustomerCodeRelatedDataList = [];
    $scope.getSelectedCustomerDetailList = function (pre_code) {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetCustomerDetailListByPreCustCode?preCustomerCode=" + pre_code;

        $http.get(apiUrl)
            .then(function (response) {

                if (response && response.data)
                    $scope.PreCustomerCodeRelatedDataList = response.data;

            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };

    $scope.ParticularCustomerDetailData = {};
    $scope.getDetailByCustomerCode = function (item) {

        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetCustomerDetailByCustomerCode?customerCode=" + item.CUSTOMER_CODE;

        $http.get(apiUrl)
            .then(function (response) {

                if (response && response.data)
                    $scope.ParticularCustomerDetailData = response.data;

            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };



    $scope.checkEnterCustomerListSearch = function (event) {
        if (event.which === 13) { // 13 is the Enter key
            $scope.GetCustomerListBySearchText(); // or any function you want to call
        }
    };

    $scope.GetCustomerListBySearchText = function () {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetCustomerListBySearch?searchText=" + $scope.customerSearchText;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data)
                    $scope.PreCustomerCodeRelatedDataList = response.data;
            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };


    $scope.ReturnSelectedCode = function () {
        $scope.FormattingTab_FormDetailSteupList[$scope.currentDropDownIndex].FILTER_VALUE_DESC = $scope.selectedCustomerCategoryData.text;
        $scope.FormattingTab_FormDetailSteupList[$scope.currentDropDownIndex].FILTER_VALUE = $scope.selectedCustomerCategoryData.id;
        $('#TreeViewFormattingTabPopupModel').modal('hide');
    };


    $scope.ParticularItemGroupedDetailData = {};
    $scope.getItemDetailByItemCode = function (item) {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetItemDetailByItemCode?itemCode=" + item.ITEM_CODE;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.ParticularItemGroupedDetailData = response.data;
                }
            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };


    $scope.PreItemGroupedRelatedDataList = [];
    $scope.getSelectedItemGroupedDetailList = function (preItemCode) {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetItemDetailListByPreItemCode?preItemCode=" + preItemCode;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.PreItemGroupedRelatedDataList = response.data;
                }
            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };

    $scope.selectedItemCategoryData = {};
    $scope.LoadItemGroupedTree = function () {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetItemGroupList";
        $http.get(apiUrl)
            .then(function (response) {
                $("#ItemGroupedTreeView").kendoTreeView({
                    dataSource: response.data,
                    dataTextField: "text",
                    // Add this for click/selection event
                    select: function (e) {
                        var treeView = $("#ItemGroupedTreeView").data("kendoTreeView");
                        var selectedNode = treeView.dataItem(e.node);
                        console.log("Selected node:", selectedNode);
                        $scope.selectedItemCategoryData = selectedNode;
                        $scope.getSelectedItemGroupedDetailList(selectedNode.id);
                    }
                });
                $('#ItemGroupedTreeViewFormattingTabPopupModel').modal('show');
            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };

    $scope.ReturnSelectedItemCode = function () {
        $scope.FormattingTab_FormDetailSteupList[$scope.currentDropDownIndex].FILTER_VALUE_DESC = $scope.selectedItemCategoryData.text;
        $scope.FormattingTab_FormDetailSteupList[$scope.currentDropDownIndex].FILTER_VALUE = $scope.selectedItemCategoryData.id;
        $('#ItemGroupedTreeViewFormattingTabPopupModel').modal('hide');
    };

    $scope.selectedEmployeeCategoryData = {};
    $scope.LoadEmployeeGroupedTree = function () {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetEmployeeTreeStructureList";
        $http.get(apiUrl)
            .then(function (response) {
                $('#EmployeeTreeViewFormattingTabPopupModel').modal('show');
                $("#EmployeeTreeView").kendoTreeView({
                    dataSource: response.data,
                    dataTextField: "text",
                    // Add this for click/selection event
                    select: function (e) {
                        var treeView = $("#EmployeeTreeView").data("kendoTreeView");
                        var selectedNode = treeView.dataItem(e.node);
                        console.log("Selected node Emp:", selectedNode);
                        $scope.selectedEmployeeCategoryData = selectedNode;
                        $scope.getSelectedEmployeeGroupedDetailList(selectedNode.id);
                    }
                });

            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };


    $scope.PreEmployeeGroupedRelatedDataList = [];
    $scope.getSelectedEmployeeGroupedDetailList = function (preEmpCode) {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetEmployeeListByPreCode?preEmployeeCode=" + preEmpCode;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.PreEmployeeGroupedRelatedDataList = response.data;
                }
            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };

    $scope.ParticularEmployeeDetailData = {};
    $scope.getEmployeeDetailByItemCode = function (item) {

        console.log(item);

        //var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetItemDetailByItemCode?itemCode=" + item.ITEM_CODE;
        //$http.get(apiUrl)
        //    .then(function (response) {
        //        if (response && response.data) {
        //            $scope.ParticularEmployeeDetailData = response.data;
        //        }
        //    }, function (error) {
        //        console.error("Error loading tree:", error);
        //    });
    };

    $scope.ReturnSelectedEmpCode = function () {
        $scope.FormattingTab_FormDetailSteupList[$scope.currentDropDownIndex].FILTER_VALUE_DESC = $scope.selectedEmployeeCategoryData.text;
        $scope.FormattingTab_FormDetailSteupList[$scope.currentDropDownIndex].FILTER_VALUE = $scope.selectedEmployeeCategoryData.id;
        $('#EmployeeTreeViewFormattingTabPopupModel').modal('hide');
    };


    // #endregion for FORMATTING TAB




    // #region For Reference Tab

    $scope.referenceTabInfo = {};
    $scope.isValueTrue = false;

    $scope.TransactionTableListWithoutVoucher = [];
    $scope.GetReferenceTabInfo = function () {
        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetFormSetupReferenceTabInfo" + "?form_code=" + form_code;
        //$scope.FormItemEntryUpdateModel.FORM_TYPE = "OT";
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.isValueTrue = true;
                    var data = response.data;
                    data.REFERENCE_FLAG = (data.REFERENCE_FLAG === 'Y') ? 'Y' : 'N';
                    $scope.referenceTabInfo = angular.copy(data);


                    if ($scope.referenceTabInfo.REF_TABLE_NAME) {
                        $scope.onRefTableChange($scope.referenceTabInfo.REF_TABLE_NAME);
                    }

                    $('.checkedClsClass').find(".checker").attr("class", "");
                    $('#removeClsChecker').find('.checker').removeClass('checker');
                    $('.removeClsCheckerReceiptAgainstSalesInvoice').find(".checker").attr("class", "");

                }
            }).catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });
    };


    $scope.GetTransactionTableListWithoutVoucher = function () {
        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetTransactionTableListWithoutVoucher";
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.TransactionTableListWithoutVoucher = response.data;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });
    };

    $scope.BasedOnTableRelatedList = [];
    $scope.onRefTableChange = function (selectedTableName) {
        if (selectedTableName) {
            var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetListBasedOnTableName?table_name=" + selectedTableName;
            $http.get(apiUrl)
                .then(function (response) {
                    console.log(response);
                    if (response && response.data) {

                        if (response.data.result) {
                            $scope.BasedOnTableRelatedList = response.data.result;
                        }

                        if (response.data.result1) {
                            $scope.referenceTabInfo.REF_COLUMN_NAME = response.data.result1;
                        }
                    }
                })
                .catch(function (error) {
                    console.error("Error while fetching branch list:", error);
                });

        } else {
            console.log("No table selected.");
        }
    };

    $scope.saveReferenceTabData = function () {

        $scope.referenceTabInfo['BRANCH_LIST'] = $scope.branchList;

        console.log($scope.referenceTabInfo);

        // return;

        var apiUrlUpdate = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/UpdateReferenceTabInfo";
        var saveToObj = $scope.referenceTabInfo;

        $http.post(apiUrlUpdate, saveToObj)
            .then(function (response) {
                if ($scope.isShowSaveSuccessMsg) {
                    displayPopupNotification(response.data.MESSAGE, "success");
                }
            })
            .catch(function (error) {
                console.error("Error inserting form:", error);
            });
    };

    $scope.branchList = [];

    $scope.vouchersToBeMapped = [
    ];

    // Handle invoice mapping click event
    $scope.openInvoiceMapping = function (voucherForText) {

        $scope.voucherForText = voucherForText;

        $scope.isLoading = true;
        // Call the API to get invoice mapping data
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetInvoiceToBeMatchedList";
        $http.get(apiUrl).then(function (response) {
            if (response.data && response.data.length > 0) {
                $scope.vouchersToBeMapped = response.data;
                $('#voucherModal').modal('show');
            }
        }).catch(function (error) {
            console.error('Error fetching invoice mapping data:', error);
        }).finally(function () {
            $scope.isLoading = false;
        });
    };

    // select row
    $scope.selectVoucher = function (selectedItem) {
        $scope.selectedVoucher = selectedItem;

        if ($scope.voucherForText == 'Invoice Is To be Mapped') {
            $scope.referenceTabInfo.INVOICE_PJV_FORM_CODE = selectedItem.FORM_CODE;
            $scope.referenceTabInfo.INVOICE_PJV_FORM_EDESC = selectedItem.FORM_EDESC;
        }

        if ($scope.voucherForText == 'Receipt Form') {
            $scope.referenceTabInfo.RECEIPT_FORM_CODE = selectedItem.FORM_CODE;
            $scope.referenceTabInfo.RECEIPT_FORM_CODE_EDESC = selectedItem.FORM_EDESC;
        }

        $scope.closePopupModel('voucherModal');
    };

    $scope.GetBranchListData = function () {
        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetBranchListData?formCode=" + form_code;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.branchList = response.data;
                }
            })
            .catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });
    };

    $scope.accountMappingList = [];
    $scope.accountSearchText = "";
    $scope.openAccountMapping = function (isOpenPopup) {
        $scope.isLoading = true;
        // Call the API to get invoice mapping data
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetAccountList?searchText=" + $scope.accountSearchText;
        $http.get(apiUrl).then(function (response) {
            if (response.data && response.data.length > 0) {
                $scope.accountMappingList = response.data;
                if (isOpenPopup) {
                    $('#accountMappingModal').modal('show');
                }
            }
        }).catch(function (error) {
            console.error('Error fetching invoice mapping data:', error);
        }).finally(function () {
            $scope.isLoading = false;
        });
    };

    $scope.searchAccounts = function () {
        var isOpenPopUp = false;
        $scope.openAccountMapping(isOpenPopUp);
    }

    // select row
    $scope.selectAccount = function (selectedItem) {
        $scope.referenceTabInfo.RECEIPT_CASH_ACC_CODE = selectedItem.ACC_CODE;
        $scope.referenceTabInfo.RECEIPT_CASH_ACC_CODE_EDESC = selectedItem.ACC_EDESC;
        $scope.closePopupModel('accountMappingModal');
    };

    $scope.LoadReferenceTabData = function () {
        $scope.saveRecentTabDataJustPrev($scope.currentTab);

        $scope.currentTab = "Reference";
        $scope.GetReferenceTabInfo();
        $scope.GetBranchListData();
        $scope.GetTransactionTableListWithoutVoucher();
    };
    // #endregion Reference TAB








    //Start Numbering Tab

    // #region For Numbering Tab


    $scope.numberingTabInfo = {};
    $scope.GetNumberingTabInfo = function () {
        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetFormSetupNumberingTabInfo" + "?form_code=" + form_code;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {

                    $scope.numberingTabInfo = angular.copy(response.data);
                    // ALWAYS clean the flag
                    $scope.numberingTabInfo.ID_GENERATION_FLAG =
                        ($scope.numberingTabInfo.ID_GENERATION_FLAG || "")
                            .trim()
                            .toUpperCase();

                    $scope.numberingTabInfo.START_DATE = new Date($scope.numberingTabInfo.START_DATE);
                    $scope.numberingTabInfo.LAST_DATE = new Date($scope.numberingTabInfo.LAST_DATE);

                    var apiValue = $scope.numberingTabInfo.ID_GENERATION_FLAG;
                    $('input[name="idFlag"][value="' + apiValue + '"]')
                        .prop('checked', true)
                        .closest('div.radio')
                        .find('span')
                        .addClass('checked');

                    $('.checkerRemoveCls').find(".checker").attr("class", "");

                }
            }).catch(function (error) {
                console.error("Error while fetching branch list:", error);
            });
    };

    $scope.setCounetC_PREFIX = function () {
        $scope.numberingTabInfo.PREFIX_LENGTH = $scope.numberingTabInfo.CUSTOM_PREFIX_TEXT.length;
    };

    $scope.setCounetC_SUFFIX = function () {
        $scope.numberingTabInfo.SUFFIX_LENGTH = $scope.numberingTabInfo.CUSTOM_SUFFIX_TEXT.length;
    };

    $scope.saveNumberingTabData = function () {
        var apiUrlUpdate = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/UpdateNumberingTabInfo";
        var saveToObj = $scope.numberingTabInfo;
        $http.post(apiUrlUpdate, saveToObj)
            .then(function (response) {
                if ($scope.isShowSaveSuccessMsg) {
                    displayPopupNotification(response.data.MESSAGE, "success");
                }
            })
            .catch(function (error) {
                console.log(JSON.stringify(error));
                if (error.status == '422' && error.data.TYPE == "validation") {
                    displayPopupNotification(error.data.MESSAGE, "error");
                }
                console.error("Error inserting form:", error);
            });
    };




    $scope.LoadNumberingTabData = function () {
        //alert($scope.currentTab);
        $scope.saveRecentTabDataJustPrev($scope.currentTab);

        $scope.currentTab = "Numbering";
        $scope.GetNumberingTabInfo();
    };

    // endregion Numbering Tab





    //Charge Setup

    //$scope.chargeList = [
    //    { name: "Discount" },
    //    { name: "VAT 13%" }
    //];

    $scope.selectCharge = function (item) {
        $scope.selectedCharge = item;
    };

    $scope.model = {
        calcBasis: "Value",
        glImpact: true,
        activeFlag: true
    };

    $scope.priorityIndexList = [];
    for (var i = 1; i <= 99; i++) {
        $scope.priorityIndexList.push(i);
    }

    $scope.createNew = function () { };
    $scope.edit = function () { };
    $scope.delete = function () { };

    $scope.divDisableFlagEdit = false;
    $scope.divDisableFlagList = true;

    $scope.accountList = [];
    $scope.chargeList = [];
    $scope.chargeSetupDataObj = {};

    $scope.accountChargeSetupTabSearchText = "";
    $scope.chargeSetupTabAccountSetupMappingList = [];

    $scope.accountSearchText = "";
    $scope.GetChargeSetupAccountList = function () {
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/ChargeSetupGetAccountList?searchText=" + $scope.accountSearchText;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.chargeSetupTabAccountSetupMappingList = response.data;




                }
            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };

    $scope.selectedChargeCode = "";
    $scope.ChargeSetupChargeList = function () {
        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/ChargeSetupGetChargeList?form_code=" + form_code;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {
                    $scope.chargeList = response.data;

                    $scope.selectedCharge = $scope.chargeList[0];
                    $scope.GetChargeSetupData($scope.chargeList[0].CHARGE_CODE);
                }
            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };

    $scope.selectedCharge = {};
    $scope.selectChargeListItem = function (item) {
        $scope.selectedCharge = item;
        $scope.selectedChargeCode = item.CHARGE_CODE;
        $scope.GetChargeSetupData(item.CHARGE_CODE);
    };



    $scope.GetChargeSetupData = function (chargeCode) {

        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetChargeSetup?formCode=" + form_code + "&chargeCode=" + chargeCode;
        $http.get(apiUrl)
            .then(function (response) {
                if (response && response.data) {

                    $scope.chargeSetupDataObj = angular.copy(response.data);
                    $scope.chargeSetupDataObj.CHARGE_CODE = chargeCode;
                    $scope.chargeSetupDataObj.APPLY_FROM_DATE = new Date($scope.chargeSetupDataObj.APPLY_FROM_DATE);
                    $scope.chargeSetupDataObj.APPLY_TO_DATE = new Date($scope.chargeSetupDataObj.APPLY_TO_DATE);

                    var apiValue = response.data.VALUE_PERCENT_FLAG;

                    $('input[name="VALUE_PERCENT_FLAG"][value="' + 'Q' + '"]')
                        .prop('checked', false)
                        .closest('div.radio')
                        .find('span')
                        .removeClass('checked');

                    $('input[name="VALUE_PERCENT_FLAG"][value="' + 'V' + '"]')
                        .prop('checked', false)
                        .closest('div.radio')
                        .find('span')
                        .removeClass('checked');

                    $('input[name="VALUE_PERCENT_FLAG"][value="' + 'P' + '"]')
                        .prop('checked', false)
                        .closest('div.radio')
                        .find('span')
                        .removeClass('checked');

                    $('.chargeSetupCheckboxCheckerRemoveCls').find(".checker").attr("class", "");

                    $('input[name="VALUE_PERCENT_FLAG"][value="' + apiValue + '"]')
                        .prop('checked', true)
                        .closest('div.radio')
                        .find('span')
                        .addClass('checked');

                }
            }, function (error) {
                console.error("Error loading tree:", error);
            });
    };


    $scope.LoadChargeSetupTabData = function () {
        // $scope.saveRecentTabDataJustPrev($scope.currentTab);

        $scope.currentTab = "Charge Setup";

        $scope.ChargeSetupChargeList();
        $scope.GetChargeSetupAccountList();
        //$scope.GetNumberingTabInfo();
    };

    $scope.OnBasicOrImpactTypeList = [
        { Key: "F", Value: "Flat" },
        { Key: "Q", Value: "Quantity" },
        { Key: "R", Value: "Rate" }
    ];



    $scope.accountMappingListChargeSetup = [];
    $scope.accountSearchTextChargeSetup = "";
    $scope.openAccountMappingChargeSetup = function (isOpenPopup) {
        $scope.isLoading = true;
        // Call the API to get invoice mapping data
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetGetAccountList?searchText=" + $scope.accountSearchTextChargeSetup;
        $http.get(apiUrl).then(function (response) {
            if (response.data && response.data.length > 0) {
                $scope.accountMappingListChargeSetup = response.data;
                if (isOpenPopup) {
                    $('#accountMappingModalChargeSetup').modal('show');
                }
            }
        }).catch(function (error) {
            console.error('Error fetching invoice mapping data:', error);
        }).finally(function () {
            $scope.isLoading = false;
        });
    };

    $scope.searchAccountsChargeSetup = function () {
        var isOpenPopUp = false;
        $scope.openAccountMappingChargeSetup(isOpenPopUp);
    };

    // select row
    $scope.selectAccountChargeSetup = function (selectedItem) {
        $scope.chargeSetupDataObj.ACC_CODE = selectedItem.ACC_CODE;
        $scope.chargeSetupDataObj.ACC_CODE_EDESC = selectedItem.ACC_EDESC;
        $scope.closePopupModel('accountMappingModalChargeSetup');
    };
    $('#applyBtnId').hide();
    $scope.makeEditable = function () {
        $scope.divDisableFlagEdit = true;
        $scope.divDisableFlagList = false;
        $('#editBtnId').hide();
        $('#applyBtnId').show();
    };

    $scope.cancleEditSection = function () {
        $scope.divDisableFlagEdit = false;
        $scope.divDisableFlagList = true;
        $('#editBtnId').show();
        $('#applyBtnId').hide();
    };

    $scope.ApplyToSave = function () {
        $scope.saveChargeSetupTabData();
    };

    $scope.saveChargeSetupTabData = function () {
        var apiUrlUpdate = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/UpdateChargeSetupTabInfo";
        var saveToObj = $scope.chargeSetupDataObj;
        $http.post(apiUrlUpdate, saveToObj)
            .then(function (response) {
                // if ($scope.isShowSaveSuccessMsg) {
                displayPopupNotification(response.data.MESSAGE, "success");
                //  }
            })
            .catch(function (error) {
                console.error("Error inserting form:", error);
            });
    };


    /// Quality Check Tab details
    /// Quality Check Tab
    $scope.qualityCheckDataObj = {};

    $scope.batchSerialFlag = {};


    $scope.LoadQualityCheckTabData = function () {

        $scope.currentTab = "Quality Check";


        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetLoadQualityCheckTabData?formCode=" + form_code;
        $http.get(apiUrl).then(function (response) {
            if (response.data && response.data) {

                $scope.qualityCheckDataObj = angular.copy(response.data);

                if ($scope.qualityCheckDataObj.SERIAL_TRACKING_FLAG == "Y") {
                    $scope.qualityCheckDataObj.batchSerialFlag = "Serial";

                    $('input[name="batchserialflag"][value="' + 'Serial' + '"]')
                        .prop('checked', true)
                        .closest('div.radio')
                        .find('span')
                        .addClass('checked');
                }

                if ($scope.qualityCheckDataObj.BATCH_TRACKING_FLAG == "Y") {
                    $scope.qualityCheckDataObj.batchSerialFlag = "Batch";

                    $('input[name="batchserialflag"][value="' + 'Batch' + '"]')
                        .prop('checked', true)
                        .closest('div.radio')
                        .find('span')
                        .addClass('checked');
                }

                if ($scope.qualityCheckDataObj.SERIAL_TRACKING_FLAG != "Y" && $scope.qualityCheckDataObj.BATCH_TRACKING_FLAG != "Y") {
                    $scope.qualityCheckDataObj.batchSerialFlag = "None";

                    $('input[name="batchserialflag"][value="' + 'None' + '"]')
                        .prop('checked', true)
                        .closest('div.radio')
                        .find('span')
                        .addClass('checked');
                }

            }
        }).catch(function (error) {
            console.error('Error fetching invoice mapping data:', error);
        }).finally(function () {
            $scope.isLoading = false;
        });


    }; //$scope.LoadQualityCheckTabData();


    $scope.saveQualityCheckTabData = function () {
        var apiUrlUpdate = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/UpdateQualityCheckData";
        var saveToObj = $scope.qualityCheckDataObj;
        $http.post(apiUrlUpdate, saveToObj)
            .then(function (response) {
                // if ($scope.isShowSaveSuccessMsg) {
                displayPopupNotification(response.data.MESSAGE, "success");
                //  }
            })
            .catch(function (error) {
                console.error("Error inserting form:", error);
            });
    };



    ///////////

    $scope.freezeBackDays = 60;
    $scope.uptoRounding = 2;
    $scope.uptoDecimal = 2;
    $scope.maxRows = 0;

    $scope.miscellaneousTabDataObj = {};

    $scope.reportSearchText = "";
    $scope.voucherPrintReportList = [];
    $scope.selectedVoucherPrintRPT = {};

    $scope.LoadMiscellaneousTabData = function () {

        $scope.currentTab = "Miscellaneous";

        if ($scope.IsDuplicate) {
            $scope.duplicateBtnText = "Save";
            $scope.selectedTab = 8;
        }

        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetMiscellaneousTabData?formCode=" + form_code;
        $http.get(apiUrl).then(function (response) {
            if (response.data && response.data) {
                $scope.miscellaneousTabDataObj = angular.copy(response.data);
                $('.checkerRemoveFromMiscellaneousTabCls').find(".checker").attr("class", "");
            }
        }).catch(function (error) {
            console.error('Error fetching invoice mapping data:', error);
        }).finally(function () {
            $scope.isLoading = false;
        });

    };


    $scope.GetDocumentReportList = function () {
        var form_code = $scope.selectedRowItemDataOnGrid.FORM_CODE;
        var apiUrl = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/GetDocumentReportList?formCode=" + form_code + "&searchText=" + $scope.reportSearchText;
        $http.get(apiUrl).then(function (response) {
            if (response.data && response.data) {
                $scope.voucherPrintReportList = response.data;
                $('#printSearchReportModal').modal('show');
            }
        }).catch(function (error) {
            console.error('Error fetching invoice mapping data:', error);
        }).finally(function () {
            $scope.isLoading = false;
        });
    };

    $scope.selectVoucherPrintRPT = function (item) {
        $scope.selectedVoucherPrintRPT = item;
    }

    $scope.selectReportPrint = function () {
        $scope.miscellaneousTabDataObj.REPROT_NO = $scope.selectedVoucherPrintRPT.REPORT_NO;
        $scope.miscellaneousTabDataObj.REPORT_EDESC = $scope.selectedVoucherPrintRPT.REPORT_EDESC;
        $scope.closePopupModel('printSearchReportModal');
    }

    $scope.SaveMiscellaneousTabData = function () {

        var apiUrlUpdate = window.location.protocol + "//" + window.location.host + "/api/FormSetupApi/UpdateMiscellaneousTabData";
        var saveToObj = $scope.miscellaneousTabDataObj;
        $http.post(apiUrlUpdate, saveToObj)
            .then(function (response) {
                // if ($scope.isShowSaveSuccessMsg) {
                displayPopupNotification(response.data.MESSAGE, "success");
                //  }
            })
            .catch(function (error) {
                console.error("Error inserting form:", error);
            });
    }












    $scope.saveCurrentTabInfo = function () {
        $scope.isShowSaveSuccessMsg = true;
        debugger;
        if ($scope.currentTab == 'Information') {
            $scope.saveInformationTabData();
        }
        if ($scope.currentTab == 'Formatting') {
            $scope.saveFormattingTabData();
        }
        if ($scope.currentTab == 'Reference') {
            $scope.saveReferenceTabData();
        }
        if ($scope.currentTab == 'Numbering') {
            $scope.saveNumberingTabData();
        }

        if ($scope.currentTab == 'Quality Check') {
            $scope.saveQualityCheckTabData();
        }

        if ($scope.currentTab == 'Miscellaneous') {
            $scope.SaveMiscellaneousTabData();
        }

    };

    $scope.saveRecentTabDataJustPrev = function (justRecentTab) {

        if ($scope.IsDuplicate) return;

        $scope.isShowSaveSuccessMsg = false;
        if (justRecentTab == 'Information') {
            $scope.saveInformationTabData();
        }
        if (justRecentTab == 'Formatting') {
            $scope.saveFormattingTabData();
        }

        if (justRecentTab == 'Reference') {
            $scope.saveReferenceTabData();
        }
        if (justRecentTab == 'Numbering') {
            $scope.saveNumberingTabData();
        }

        if ($scope.currentTab == 'Quality Check') {
            $scope.saveQualityCheckTabData();
        }
    }


    $scope.duplicateBtnText = "Next";

    // Next Button
    $scope.nextTab = function () {
        debugger;
        //$scope.saveInformationTabData();

        var previousTab = $scope.tabs[$scope.selectedTab].title;

        if ($scope.selectedTab < $scope.tabs.length - 1) {
            $scope.selectedTab++;
            if ($scope.tabStrip) $scope.tabStrip.select($scope.selectedTab);
            $scope.currentTab = $scope.tabs[$scope.selectedTab].title;
        }



        if (!$scope.IsDuplicate) {
            if (previousTab == 'Information') {
                $scope.saveInformationTabData();
            }

            if (previousTab == 'Formatting') {
                $scope.saveFormattingTabData();
            }
        }



        // add all other 



        if ($scope.currentTab == 'Information') {

        }

        if ($scope.currentTab == 'Formatting') {
            $scope.LoadFormattingData();
        }

        if ($scope.currentTab == 'Reference') {
            $scope.LoadReferenceTabData();
        }

        if ($scope.currentTab == 'Numbering') {
            $scope.LoadNumberingTabData();
        }

        if ($scope.currentTab == 'Charge Setup') {
            $scope.LoadChargeSetupTabData();
        }

        if ($scope.currentTab == 'Quality Check') {
            $scope.LoadQualityCheckTabData();
        }

        if ($scope.currentTab == 'Miscellaneous') {

            if ($scope.duplicateBtnText == "Save" && $scope.IsDuplicate) {
                $scope.saveInformationTabData();
                return;
            }


            $scope.LoadMiscellaneousTabData();
            $scope.duplicateBtnText = "Save";
        }



    };






    // Previous Button
    $scope.previousTab = function () {
        if ($scope.selectedTab > 0) {
            $scope.selectedTab--;
            if ($scope.tabStrip) $scope.tabStrip.select($scope.selectedTab);
        }
    };

});