
distributionModule.controller('EmployeeSetupCtrl', function ($scope, DistSetupService, $http, $routeParams) {
    //$scope.GridSearch = GridSearch;

    $scope.saveAction = "Save";
    $scope.$on('$routeChangeSuccess', function () {
        $scope.fetchEmployee();
    });
    reportConfig = GetReportSetting("EmployeeSetup");

    $scope.AddButtonClickEvent = function () {
        $scope.Cancel();
        $scope.createPanel = true;
    }
    //Create Employee
    $scope.EmployeeSetup = {
        EMPLOYEE_CODE: null,
        EMPLOYEE_EDESC: null,
        EPERMANENT_ADDRESS1: null,
        COMPANY_CODE: null,
        BRANCH_CODE: null
    };
    
    $scope.Cancel = function () {
        $scope.createPanel = false;
        $scope.pageName = "Add Employee";
        $scope.saveAction = "Save";

        $scope.EmployeeSetup = {
            EMPLOYEE_CODE: null,
            EMPLOYEE_EDESC: null,
            EPERMANENT_ADDRESS1: null,
            COMPANY_CODE: null,
            BRANCH_CODE: null

        };
    }
    //Save and update Employee
    $scope.SaveEmployee = function (isValid) {
        //if (isValid == false) {
        //    displayPopupNotification("Invalid Input fields", "warning");
        //    return;
        //}
        if (!isValid)
        {
            var validation = [
                { EmployeeName: $scope.EployeeForm.EmployeeName.$invalid },
                { Address: $scope.EployeeForm.Address.$invalid },
                { Company: $scope.EployeeForm.distCompanySelect.$invalid },
                { Branch: $scope.EployeeForm.distBranchSelect.$invalid }


            ]
            if (validation[0].EmployeeName == true) {

                displayPopupNotification("Please Select Employee Name", "warning");
                $('#EmployeeName').focus();
                return
            }
            if (validation[1].Address == true) {

                displayPopupNotification("Please Select Address", "warning");
                $('#Address').focus();
                return
            }

            if (validation[2].Company == true) {

                displayPopupNotification("Plese Seleact Company", "warning");
                $("#distCompanySelect").data("kendoMultiSelect").open();
                return
            }
            if (validation[3].Branch== true) {

                displayPopupNotification("Please Select Branch", "warning");
                $("#distBranchSelect").data("kendoMultiSelect").open();
                return
            }

        }


        DistSetupService.CreateEmployee($scope.EmployeeSetup).then(function (response) {
            displayPopupNotification(response.data.MESSAGE, response.data.TYPE);
            if (response.data.TYPE == "success") {
                $("#grid").data("kendoGrid").dataSource.read();
                $scope.Cancel();
            }
        }, function (response) {
            displayPopupNotification("Something went wrong", "error");
        });
    }

    $scope.grid = {
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetEmployee",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8"
                },
                schema: {
                    model: {
                        fields: {
                            EMPLOYEE_CODE: { type: "string" },
                            EMPLOYEE_EDESC: { type: "string" },
                            EPERMANENT_ADDRESS1: { type: "string" },
                            COMPANY_EDESC: { type: "string" },
                            BRANCH_EDESC: { type: "string" },
                            // Add any other fields your dataset includes here
                        }
                    }
                },
            },
            error: function (e) {
                displayPopupNotification("Sorry error occured while processing data", "error");
            },
            pageSize: 500,
        },
        toolbar: kendo.template($("#toolbar-template").html()),
        excel: {
            fileName: "Employees",
            allPages: true,
        },
        height: window.innerHeight - 50,
        sortable: true,
        reorderable: true,
        groupable: true,
        resizable: true,
        filterable: {
            extra: false,
            operators: {
                number: {
                    eq: "Is equal to",
                    neq: "Is not equal to",
                    gte: "is greater than or equal to	",
                    gt: "is greater than",
                    lte: "is less than or equal",
                    lt: "is less than",
                },
                string: {

                    eq: "Is equal to",
                    neq: "Is not equal to",
                    startswith: "Starts with	",
                    contains: "Contains",
                    doesnotcontain: "Does not contain",
                    endswith: "Ends with",
                },
                date: {
                    eq: "Is equal to",
                    neq: "Is not equal to",
                    gte: "Is after or equal to",
                    gt: "Is after",
                    lte: "Is before or equal to",
                    lt: "Is before",
                }
            }
        },
        columnMenu: true,
        //columnMenuInit: function (e) {
        //    wordwrapmenu(e);
        //    checkboxItem = $(e.container).find('input[type="checkbox"]');
        //},
        //columnShow: function (e) {
        //    if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
        //        SaveReportSetting('CompItemSetup', 'grid');
        //},
        //columnHide: function (e) {
        //    if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
        //        SaveReportSetting('CompItemSetup', 'grid');
        //},
        pageable: {
            refresh: true,
            pageSizes: reportConfig.itemPerPage,
            buttonCount: 5
        },
        model: {
            fields: {
            }
        },
        columnMenuInit: function (e) {
            wordwrapmenu(e);
            checkboxItem = $(e.container).find('input[type="checkbox"]');
        },
        dataBound: function (o) {
            GetSetupSetting("EmployeeSetup");
            var grid = o.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
            $('.k-grid td').css("white-space", "normal"); //wrap text
            $('div').removeClass('.k-header k-grid-toolbar');
        },
        columns: [
            {
                field: "EMPLOYEE_CODE",
                title: "Employee Code",
                width: "10%",
            },
            {
                field: "EMPLOYEE_EDESC",
                title: "Employee Name",
                width: "20%",

            },
            {
                field: "EPERMANENT_ADDRESS1",
                title: "Employee Address",
                width: "20%",

            },

            {
                field: "COMPANY_EDESC",
                title: "Company Name",
                width: "20%",
                headerAttributes: {
                    style: "white-space: normal"
                }
            },
            {
                field: "BRANCH_EDESC",
                title: "Branch Name",
                width: "20%",
                headerAttributes: {
                    style: "white-space: normal"
                }
            },
            {
                title: "Actions",
                width: "10%",
                template: " <a class='fa fa-edit editAction' ng-click='UpdateEmployee(#:EMPLOYEE_CODE#)' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='DeleteEmployee(#:EMPLOYEE_CODE#)' title='Delete'></a>  ",

            }
        ]
    };

    function GridSearch() {
        var grid = $("#grid").data("kendoGrid");
        var filter = [];
        var fields = grid.dataSource.options.fields;

        if (fields === undefined) {
            var columns = [];
            $.each(grid.columns, function (idx, item) {
                if (item.columns) {
                    columns = columns.concat(item.columns);
                } else {
                    columns.push(item);
                }
            });
            fields = columns;
        }

        var v = fields[1];
        if (v.field !== undefined) {
            var searchText = $.trim($('.grid-search').val());
            var model = grid.dataSource.transport.options.schema.model;

            if (model === undefined) {
                model = grid.dataSource.options.model;
            }
            // Check if model and model.fields are defined
            if (model && model.fields && model.fields[v.field]) {
                var fieldModel = model.fields[v.field];
                var type = fieldModel.type || "string"; // Default to string if type is not defined

                if (type === "string") {
                    filter.push({ field: v.field, operator: "contains", value: searchText });
                } else if (type === "number") {
                    filter.push({ field: v.field, operator: "eq", value: searchText });
                } else if (type === "date") {
                    var dateValue = new Date(searchText);
                    searchText = isNaN(dateValue) ? searchText : dateValue;
                    filter.push({ field: v.field, operator: "eq", value: searchText });
                }
            } else {
                console.log("Model or field not defined for:", v.field);
            }
        } else {
            console.log("Column does not have a field property:", v);
        }

        var filterObject = {
            logic: "or",
            filters: filter,
        };

        grid.dataSource.filter(filterObject);

        // Clear filters if search input is empty
        if ($(".grid-search").val().length === 0) {
            grid.dataSource.filter([]);
        }
    }

    //Binding while editing
    $scope.UpdateEmployee = function (id) {
        var gridDs = $("#grid").data("kendoGrid").dataSource.data();
        var items = _.filter(gridDs, function (x) { return x.EMPLOYEE_CODE == id });

        var emp = angular.copy(items[0]);
        emp.COMPANY_CODE = emp.COMPANY_CODES.split(",");
        emp.BRANCH_CODE = emp.BRANCH_CODES.split(",");
        $scope.EmployeeSetup = angular.copy(emp);
        $scope.pageName = "Update Employee";
        $scope.saveAction = "Update";
        $scope.createPanel = true;
    }
    
    $scope.DeleteEmployee = function (id) {
        if (id) {
            bootbox.confirm("<strong>This Employee will be deleted</strong><br /> Are you sure want to delete?", function (result) {
                if (result) {
                    DistSetupService.DeleteEmployee(id).then(function (res) {
                        displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
                        if (res.data.TYPE == "success") {
                            $scope.Cancel();
                            $("#grid").data("kendoGrid").dataSource.read();
                        }
                    }, function (ex) {
                        displayPopupNotification("Error processing request", "error");
                    });
                }
            });
        }
    }

    //Company multiselect
    $scope.distCompanySelectOptions = {
        close: function (e) {
            $scope.EmployeeSetup.BRANCH_CODE = null;
            var selected = $scope.EmployeeSetup.COMPANY_CODE;
            if (typeof (selected) != 'undefined')
                $scope.fetchEmployee(selected.join("','"));
        },
        dataTextField: "COMPANY_EDESC",
        dataValueField: "COMPANY_CODE",
        //height: 600,
        valuePrimitive: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Select Company...</strong></div>',
        placeholder: "Select Company...",
        autoClose: false,
        dataBound: function (e) {
            $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetCompany",
                    dataType: "json"
                }
            }
        }
    };

    //Branch multiselect
    $scope.fetchEmployee = function (typeId) {
        var url = '';
        if (typeof (typeId) == 'undefined')
            url = window.location.protocol + "//" + window.location.host + "/api/Setup/GetBranch";
        else
            url = window.location.protocol + "//" + window.location.host + "/api/Setup/GetBranch?COMPANY_CODE=" + typeId
        var dataSource = $("#distBranchSelect").data("kendoMultiSelect");
        if (typeof (dataSource) != 'undefined' && dataSource != null) {
            $("#distBranchSelect").data("kendoMultiSelect").dataSource.options.transport.read.url = url;
            $("#distBranchSelect").data("kendoMultiSelect").dataSource.read();
            return;
        }

        $scope.distBranchSelectOptions = {
            dataTextField: "BranchName",
            dataValueField: "BranchCode",
            //height: 600,
            valuePrimitive: true,
            headerTemplate: '<div class="col-md-offset-3"><strong>Select Branch Name...</strong></div>',
            placeholder: "Select Branch Name...",
            autoClose: false,
            dataBound: function (e) {
                $(".k-list.k-reset").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
            },
            dataSource: {
                transport: {
                    read: {
                        url: url,
                        dataType: "json"
                    }
                }
            }
        };
    }

});