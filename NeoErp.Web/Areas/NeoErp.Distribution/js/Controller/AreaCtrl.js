
distributionModule.controller('AreaCtrl', function ($scope, ResellerService, $routeParams) {
    debugger;
    $scope.param = $routeParams.param;
    $scope.pageName = "Add Reseller";
    $scope.saveAction = "Save";

    // $('.createPanel').hide();
    $scope.createPanel = false;
    $scope.tempVcdCode = "";

    $scope.distAreaMultiSelect = {
        dataTextField: "DISTRICT_NAME",
        dataValueField: "DISTRICT_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>...District...</strong></div>',
        placeholder: "Select District...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetDistrictList",
                    dataType: "json"
                }
            }
        },
       
        change: distCustomerChange
    };
    $scope.GroupMultiSelect = {
        dataTextField: "GROUP_EDESC",
        dataValueField: "GROUPID",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Group...</strong></div>',
        placeholder: "Select Group...",
        autoClose: true,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllResellerGroups",
                    dataType: "json"
                }
            }
        }
    };
    function distCustomerChange(evt) {
        
        var DISTRICT_CODE = evt.sender.value()[0];
        



        if (DISTRICT_CODE != undefined) {
           
            var vdc = $("#vdcAreaMultiSelect").data("kendoMultiSelect");
            var zone = $("#zoneAreaMultiSelect").data("kendoMultiSelect");
            var region = $("#regionAreaMultiSelect").data("kendoMultiSelect");
            if (vdc != undefined) {
                vdc.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/Setup/GetvdcList?DISTRICT_CODE=" + evt.sender.value()[0];
                vdc.dataSource.read();
            }
            if (zone != undefined) {
                zone.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/Setup/GetZoneList?DISTRICT_CODE=" + evt.sender.value()[0];
                zone.dataSource.read();
            }
            if (region != undefined) {
                region.dataSource.transport.options.read.url = window.location.protocol + "//" + window.location.host + "/api/Setup/GetRegionList?DISTRICT_CODE=" + evt.sender.value()[0];
                region.dataSource.read();
            }

            $scope.areaSelectOptions = {
                dataTextField: "VDC_NAME",
                dataValueField: "VDC_CODE",
                height: 600,
                valuePrimitive: true,
                maxSelectedItems: 1,
                headerTemplate: '<div class="col-md-offset-3"><strong>VDC...</strong></div>',
                placeholder: "Select VDC...",
                autoBind: true,
                enable: true,
                dataBound: function (e) {
                    
                    //$("#vdcAreaMultiSelect").data("kendoMultiSelect").value([e.sender.dataSource.data()[0].VDC_CODE]);
                    //var current = this.value();
                    //this._savedOld = current.slice(0);
                    if ($scope.tempVcdCode != undefined)
                    {
                        $scope.selectedAreaVDC = $scope.tempVcdCode;
                        $("#vdcAreaMultiSelect").data("kendoMultiSelect").value($scope.tempVcdCode);
                        $scope.tempVcdCode = undefined;
                    }
                    $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
                },
                dataSource: {
                    transport: {
                        read: {
                            url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetvdcList?DISTRICT_CODE=" + evt.sender.value()[0],
                            dataType: "json"
                        }
                    }
                },

            };
            $scope.ZoneSelectOptions = {

                dataTextField: "ZONE_NAME",
                dataValueField: "ZONE_CODE",
                height: 600,
                valuePrimitive: true,
                maxSelectedItems: 1,
                headerTemplate: '<div class="col-md-offset-3"><strong>Zone...</strong></div>',
                placeholder: "Select VDC...",
                autoClose: false,
                enable: false,
                dataBound: function (e) {
                    $("#zoneAreaMultiSelect").data("kendoMultiSelect").value([e.sender.dataSource._data[1].ZONE_CODE]);
                    var current = this.value();
                    this._savedOld = current.slice(0);
                },
                dataSource: {
                    transport: {
                        read: {
                            url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetZoneList?DISTRICT_CODE=" + DISTRICT_CODE + "",
                            dataType: "json"
                        }
                    }
                }
            };
            $scope.RegionSelectOptions = {
                dataTextField: "REG_NAME",
                dataValueField: "REG_CODE",
                height: 600,
                enable: false,
                valuePrimitive: true,
                maxSelectedItems: 1,
                headerTemplate: '<div class="col-md-offset-3"><strong>Region...</strong></div>',
                placeholder: "Select VDC...",
                autoClose: false,
                dataBound: function (e) {
                    
                    $("#regionAreaMultiSelect").data("kendoMultiSelect").value([e.sender.dataSource.data()[0].REG_CODE]);
                    var current = this.value();
                    this._savedOld = current.slice(0);
                },
                dataSource: {
                    transport: {
                        read: {
                            url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetRegionList?DISTRICT_CODE=" + DISTRICT_CODE + "",
                            dataType: "json"
                        }
                    }
                }
            };
        }
        else {
            $("#zoneAreaMultiSelect").data("kendoMultiSelect") != undefined ? $("#zoneAreaMultiSelect").data("kendoMultiSelect").value([]) : "";
            $("#regionAreaMultiSelect").data("kendoMultiSelect") != undefined ? $("#regionAreaMultiSelect").data("kendoMultiSelect").value([]) : "";
        }


    }

   
    //grid
    var reportConfig = GetReportSetting("AreaSetup");
    $scope.grid = {
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetAllAreaList",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8"
                },
                parameterMap: function (options, type) {
                    var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                    delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                    delete paramMap.$format; // <-- remove format parameter.
                    return paramMap;
                }
            },

            error: function (e) {
                displayPopupNotification("Sorry error occured while processing data", "error");
            },
            //serverFiltering: false,
            //serverAggregates: true,
            model: {
                fields: {
                    // ASSIGN_DATE: { type: "date" },
                }
            },
            //serverPaging: false,
            //serverSorting: false,
            pageSize: reportConfig.defaultPageSize,
        },
        toolbar: kendo.template($("#toolbar-template").html()),
        excel: {
            fileName: "Area Setup",
            allPages: true,
        },
        pdf: {
            fileName: "Received Schedule",
            allPages: true,
            avoidLinks: true,
            pageSize: "auto",
            margin: {
                top: "2m",
                right: "1m",
                left: "1m",
                buttom: "1m",
            },
            landscape: true,
            repeatHeaders: true,
            scale: 0.8,
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
        columnMenuInit: function (e) {
            wordwrapmenu(e);
            checkboxItem = $(e.container).find('input[type="checkbox"]');
        },
        columnShow: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('AreaSetup', 'grid');
        },
        columnHide: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('AreaSetup', 'grid');
        },
        pageable: {
            refresh: true,
            pageSizes: reportConfig.itemPerPage,
            buttonCount: 5
        },
        pageable:true,
        scrollable: {
            virtual: true
        },
        dataBound: function (o) {
            GetSetupSetting("AreaSetup");
            var grid = o.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
            else {
                var g = $("#grid").data("kendoGrid");
                for (var i = 0; i < g.columns.length; i++) {
                    g.showColumn(i);
                }
                $("div.k-group-indicator").each(function (i, v) {
                    g.hideColumn($(v).data("field"));
                });
            }

            UpdateReportUsingSetting("AreaSetup", "grid");
            $('div').removeClass('.k-header k-grid-toolbar');
        },
        columns: [
            {
                field: "AREA_CODE",
                title: "Area Code",
            },
             {
                 field: "AREA_NAME",
                 title: "Area Name",
             },
              {
                  field: "VDC_NAME",
                  title: "VDC/ Municipality",
              },
             {
                 field: "DISTRICT_NAME",
                 title: "District Name",
             },
             {
                 field: "ZONE_NAME",
                 title: "Zone",
             },
             {
                 field: "GROUP_EDESC",
                 title: "Group",
             },
               {
                   title: "Action",
                   template: " <a class='fa fa-edit editAction' ng-click='UpdateArea($event)' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='deleteArea($event)' title='Delete'></a>  ",
                  
               }



        ]

    };

    $scope.AddButtonClickEvent = function () {
        $scope.clear();       
        $scope.createPanel = true;
    }

    $scope.AreaCreate = function (isValid) {
        
        if (!isValid) {
            displayPopupNotification("Invalid Field", "warning");
            return;
        }
        var AREA_NAME = $scope.AreaName;
        var AREA_CODE = $scope.AreaHidden;
        var DISTRICT_CODE = $scope.selectedAreaDistrict;
       // var VDC_CODE = $scope.selectedAreaVDC;
        if ($scope.selectedAreaVDC == "" || $scope.selectedAreaVDC==null) {
           var VDC_CODE = null
           //var VDC_CODE= VDC_CODE[0];
        }
        else {
            var VDC_CODE = $scope.selectedAreaVDC[0];
        }
        var ZONE_CODE = $("#zoneAreaMultiSelect").val();
        var REG_CODE = $("#regionAreaMultiSelect").val();
        var GROUP_CODE = $("#GroupMultiSelect").val();

        var GCode = '';
        if (_.isNull(GROUP_CODE) == false) {
            GCode = GROUP_CODE[0];
        }
        
        
        if (AREA_CODE == undefined) {
            var data = {
                AREA_NAME: AREA_NAME,
                DISTRICT_CODE: DISTRICT_CODE[0],
                VDC_CODE: VDC_CODE,
                ZONE_CODE: ZONE_CODE[0],
                REG_CODE: REG_CODE[0],
                GROUPID: GCode
            }
        }
        else {
            var data = {
                AREA_NAME: AREA_NAME,
                AREA_CODE: AREA_CODE[0],
                DISTRICT_CODE: DISTRICT_CODE[0],
                VDC_CODE: VDC_CODE,
                ZONE_CODE: ZONE_CODE[0],
                REG_CODE: REG_CODE[0],
                GROUPID: GCode

            }
        }
       
        if ($scope.saveAction == "Update") {
            
            var DISTRICT_CODE = $scope.selectedAreaDistrict;
            var AREA_CODE = $scope.AreaHidden;
            var getData = ResellerService.updateArea(data);
            getData.then(function (result) {

                displayPopupNotification(result.data.MESSAGE, "success");
                $("#grid").data("kendoGrid").dataSource.read();
                $scope.createPanel = false;
            }, function () {

                alert('Error in updating Area');
            });
        }
        else {
            var getData = ResellerService.AddArea(data);
            getData.then(function (result) {
                if (result.data.STATUS_CODE === 200) {
                    displayPopupNotification(result.data.MESSAGE, "success");
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.createPanel = false;
                }
            }, function () {
                displayPopupNotification("Error", "error");
            });
        }
        $scope.Cancel();
    }

    $scope.UpdateArea = function (evt) {
        var grid = $("#grid").data("kendoGrid");
        var row = evt.target.closest("tr");
        var item = grid.dataItem(row);

        var multiselect = $("#distCustomerMultiSelect").data("kendoMultiSelect");
        multiselect.value([item.DISTRICT_CODE]);
       
        $scope.AreaName = item.AREA_NAME;
        $scope.selectedAreaDistrict = [item.DISTRICT_CODE];
        $scope.selectedGroupMultiSelect = [item.GROUPID];
        $("#GroupMultiSelect").data("kendoMultiSelect").value([item.GROUPID]);
        $scope.AreaHidden = [item.AREA_CODE];
        $scope.tempVcdCode = [item.VDC_CODE];
        multiselect.trigger("change");
        $scope.pageName = "Update Area";
        $scope.saveAction = "Update";
        $scope.createPanel = true;
    }

    $scope.deleteArea = function (evt) {
        
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
                    var grid = $("#grid").data("kendoGrid");
                    var row = evt.target.closest("tr");
                    var item = grid.dataItem(row);
                    var AREA_CODE = item.AREA_CODE;

                    var data = {
                        AREA_CODE: AREA_CODE,
                    }

                    var deleteArea = ResellerService.deleteArea(data);
                    deleteArea.then(function (response) {
                        
                        if (response.data.STATUS_CODE == 400) {
                            displayPopupNotification(response.data.MESSAGE, "Success")
                            $scope.clear();
                            $("#grid").data("kendoGrid").dataSource.read();
                        }
                        else {
                            displayPopupNotification(response.data.message, "error");
                        }
                    });
                }
            }

        });
    }

    $scope.Cancel = function () {
        $("#vdcAreaMultiSelect").data("kendoMultiSelect") != undefined ? $("#vdcAreaMultiSelect").data("kendoMultiSelect").value([]) : "";
        $("#zoneAreaMultiSelect").data("kendoMultiSelect") != undefined ? $("#zoneAreaMultiSelect").data("kendoMultiSelect").value([]) : "";
        $("#regionAreaMultiSelect").data("kendoMultiSelect") != undefined ? $("#regionAreaMultiSelect").data("kendoMultiSelect").value([]) : "";
        $("#distCustomerMultiSelect").data("kendoMultiSelect") != undefined ? $("#distCustomerMultiSelect").data("kendoMultiSelect").value([]) : ""; 
        $("#GroupMultiSelect").data("kendoMultiSelect") != undefined ? $("#GroupMultiSelect").data("kendoMultiSelect").value([]) : ""; 
        $scope.AreaCreateForm.distCustomerMultiSelect.$error.required = true;
        $scope.AreaCreateForm.distCustomerMultiSelect.$invalid = true;
        $scope.AreaCreateForm.vdcAreaMultiSelect.$error.required = true;
        $scope.AreaCreateForm.vdcAreaMultiSelect.$invalid = true;
        $scope.createPanel = false;
    }


    $scope.clear = function () {        
        $scope.pageName = "Add Area";
        $scope.AreaName = "";
        $scope.saveAction = "Save";
    }

   
});
