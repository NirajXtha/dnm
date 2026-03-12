DTModule.controller('columnSettingsCtrl', function ($scope, columnSettingsService) {

    $scope.FormName = "Custom Column Define";

    $scope.columnSettingsObj = {
        "FORM_NAME": "",
        "TABLE_NAME": ""
    };

    $scope.tempColumnList = [];

    $scope.getAllFormDetail = function () {
        var formDetail = columnSettingsService.getAllFormDetail();
        formDetail.then(function (fdRes) {
            $scope.AllFormDetail = fdRes.data;
        });
    };

    $scope.getAllFormDetail();

    $scope.getAllTableDetail = function () {
        var tableDetail = columnSettingsService.getAllTableDetail();
        tableDetail.then(function (tdRes) {
            $scope.AllTableDetail = tdRes.data;
        });
    };

    $scope.getAllTableDetail();

    var columnDataSource = new kendo.data.DataSource({
        //batch:true,
        transport: {
            read: {
                url: "/api/CustomFormApi/GetAllFormDetailToEdit",
                dataType: "json"
            },
            //batch: {
            //    type: "POST",
            //    url: "/api/CustomFormApi/BulkInsertSettings/$batch",
            //    dataType: "json",
            //    contentType : "application/json"
            //},
            create: {
                type: "POST",
                url: "/api/CustomFormApi/CreateAllFormDetail",
                dataType: "json",
                contentType: "application/json"
            },
            update:
            {
                type: "POST",
                url: "/api/CustomFormApi/EditAllFormDetail",
                dataType: "json",
                contentType: "application/json"
            },
            destroy: {
                type: "POST",
                url: "/api/CustomFormApi/DeleteAllFormDetail",
                dataType: "json",
                contentType: "application/json"
            },
            parameterMap: function (options, operation) {
                console.log("options========JSON==================>>>" + JSON.stringify(options));
                console.log("options====MODELS====JSON==================>>>" + JSON.stringify(options.models));
                console.log("operations=======JSON===================>>>" + JSON.stringify(operation));
                if (operation === "update" || operation === "create" || operation === "destroy") {
                    options.FORM_CODE = $scope.columnSettingsObj.FORM_NAME.FORM_CODE;
                    $scope.tempColumnList.push(options);
                    return JSON.stringify($scope.tempColumnList);
                    //return { models: JSON.stringify(options) };
                    //return { models: kendo.stringify(options.models) };
                }
                return options;
            }
        },
        pageSize: 30,
        schema: {
            model: {
                id: "ROW_ID",
                fields: {
                    ROW_ID: { editable: false, nullable: true },
                    SERIAL_NO: { type: "number", validation: { required: true, min: 1 } },
                    TABLE_NAME: { defaultValue: { TABLE_ID: 1, TABLE_NAME:"IP_GOODS_REQUISITION"}},
                    COLUMN_NAME: { defaultValue: { COLUMN_ID: 1, COLUMN_NAME: "RequisitionNo" } },
                    COLUMN_HEADER: { validation: { required: true } },
                    DISPLAY_FLAG: { type: "boolean" },
                    IS_DESC_FLAG: { type: "boolean" }
                }
            }
        }
    });
    //columnDataSource.fetch(function () {
    //    columnDataSource.sync();
    //});


    $("#columnSettingsGrid").kendoGrid({
        dataSource: columnDataSource,
        navigatable: true,
        pageable: true,
       // toolbar: ["save", "create"],
        groupable: {
            messages: {
                empty: "Drag a row and drop it any where  to short that row"
            }
        },
        columns: [
            {field:"ROW_ID" ,title:"Row Id" , hidden:true},
            { field: "SERIAL_NO", title: "Serial No" , width:110},
            { field: "TABLE_NAME" , title:"Table Name" , editor:columnDropDownEditor,template:"#=TABLE_NAME.TABLE_NAME#"},
            { field: "COLUMN_NAME", title: "Column Name",  editor: columnDropDownEditor, template: "#=COLUMN_NAME.COLUMN_NAME#" },
            "COLUMN_HEADER",
            { field: "DISPLAY_FLAG",  editor: customBoolEditor,template:"#=DISPLAY_FLAG#" },
            { field: "IS_DESC_FLAG",  editor: customBoolEditor,template:"#=IS_DESC_FLAG#" },
            { command: "destroy", title: "Action"}],
        dataBound: function (e) {
            var grid = e.sender;
            if (grid.dataSource.total() === 0) {
                var colCount = grid.columns.length + 1;
                $(e.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No Data Found For Given Filter. </td></tr>');
            }
        },
        editable: true
    });

    $("#columnSettingsGrid").data("kendoGrid").table.kendoSortable({
        filter: ">tbody >tr",
        hint: function (element) { // Customize the hint.
            var table = $('<table style="width: 600px;" class="k-grid k-widget"></table>'),
                hint;

            table.append(element.clone()); // Append the dragged element.
            table.css("opacity", 0.7);

            return table; // Return the hint element.
        },
        cursor: "move",
        placeholder: function (element) {
            return $('<tr colspan="4" class="placeholder"></tr>');
        },
        change: function (e) {
           // var skip = grid.dataSource.skip(),
            var skip = $("#columnSettingsGrid").data("kendoGrid").dataSource.skip(),
                oldIndex = e.oldIndex + skip,
                newIndex = e.newIndex + skip,
                // data = grid.dataSource.data(),
                data = $("#columnSettingsGrid").data("kendoGrid").dataSource.data(),
                //  dataItem = grid.dataSource.getByUid(e.item.data("uid"));
                dataItem = $("#columnSettingsGrid").data("kendoGrid").dataSource.getByUid(e.item.data("uid"));
              // dataItem.SERIAL_NO = newIndex;  settings serial number as row change 

           // grid.dataSource.remove(dataItem);
           // grid.dataSource.insert(newIndex, dataItem);
            $("#columnSettingsGrid").data("kendoGrid").dataSource.remove(dataItem);
            $("#columnSettingsGrid").data("kendoGrid").dataSource.insert(newIndex, dataItem);
        }
    });

    function columnDropDownEditor(container, options) {
        if (options.field === "TABLE_NAME") {

            $('<input required name="' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    autoBind: false,
                    dataTextField: "TABLE_NAME",
                    dataValueField: "TABLE_ID",
                    dataSource: new kendo.data.DataSource({
                        transport: {
                            read: {
                                //url: "/api/CustomFormApi/GetColumnNameForDDL",
                                url: window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/GetTableNameForDDL",
                                dataType: "json"
                            }
                        }
                    })
                });

        } else {

            $('<input required name="' + options.field + '"/>')
                .appendTo(container)
                .kendoDropDownList({
                    autoBind: false,
                    dataTextField: "COLUMN_NAME",
                    dataValueField: "COLUMN_ID",
                    dataSource: new kendo.data.DataSource({
                        transport: {
                            read: {
                                //url: "/api/CustomFormApi/GetColumnNameForDDL",
                                url: window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/GetColumnNameForDDL",
                                dataType: "json"
                            }
                        }
                    })
                });
        }
        
            //}).data("kendoDropDownList");
    }

    function customBoolEditor(container, options) {
        var guid = kendo.guid();
        if (options.field === "IS_DESC_FLAG") {
            $('<input class="k-checkbox" id="' + guid + '" type="checkbox" name="IS_DESC_FLAG" data-type="boolean" data-bind="checked:IS_DESC_FLAG">').appendTo(container);
            $('<label class="k-checkbox-label" for="' + guid + '">&#8203;</label>').appendTo(container);
        } else {
            $('<input class="k-checkbox" id="' + guid + '" type="checkbox" name="DISPLAY_FLAG" data-type="boolean" data-bind="checked:DISPLAY_FLAG">').appendTo(container);
            $('<label class="k-checkbox-label" for="' + guid + '">&#8203;</label>').appendTo(container);
        }
        
    }

    $scope.searchDataToEdit = function () {
        if ($scope.columnSettingsObj.FORM_NAME && $scope.columnSettingsObj.TABLE_NAME) {
            var detailToEdit = $("#columnSettingsGrid").data("kendoGrid");
            detailToEdit.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/CustomFormApi/GetAllFormDetailToEdit?formCode=" + $scope.columnSettingsObj.FORM_NAME.FORM_CODE + "&tableName=" + $scope.columnSettingsObj.TABLE_NAME.TABLE_NAME;
            detailToEdit.dataSource.read();
            var count = detailToEdit.dataSource.total();
            console.log("Count : " + count);
            if (count <= 0) {
                detailToEdit.setOptions({
                    toolbar: ["save", "create"]
                });
            } else {
                detailToEdit.setOptions({
                    toolbar: ["save"]
                });
            }
        } else {
            displayPopupNotification("Please select FORM and Table beforing searching", "warning");
            return;
        }
        

    };

    //$scope.formDDLChange = function () {
    //    console.log("selectedFOrm: " + JSON.stringify($scope.columnSettingsObj.FORM_NAME));
    //    if ($scope.columnSettingsObj.FORM_NAME.FORM_CODE) {
    //        $("#columnSettingsGrid").data("kendoGrid").setOptions({
    //            toolbar: ["save","create"]
    //        });
    //    }

    //};

});

DTModule.service('columnSettingsService', function ($http) {

    this.getAllFormDetail = function () {
        var formDetail = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllFormDetail",
            dataType: "JSON"
        });

        return formDetail;
    };

    this.getAllTableDetail = function () {
        var tableDetail = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllTableDetail",
            dataType: "JSON"
        });
        return tableDetail;
    };

    this.getAllDetailByCodeAndName = function (formCode,tableName) {
        var detailByCodeAndName = $http({
            method: "GET",
            url: "/api/CustomFormApi/GetAllFormDetailToEdit?formCode="+formCode+"&tableName="+tableName,
            //data: {formCode:JSON.stringify(formCode),tableName:JSON.stringify(tableName)},
            dataType: "JSON"
        });
        return detailByCodeAndName;
    };

});