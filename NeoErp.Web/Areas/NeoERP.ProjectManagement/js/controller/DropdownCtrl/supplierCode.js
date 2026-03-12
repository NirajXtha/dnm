PMModule.controller('supplierCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {

    $scope.supplierDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllSupplierListByFilter",

            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        //newParams = {
                        //    filter: data.filter.filters[0].value
                        //};
                        //return newParams;
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

    //$scope.supplierCodeOption = {
    //    dataSource: $scope.supplierDataSource,
    //    template: '<span>{{dataItem.SUPPLIER_EDESC}}</span>  --- ' +
    //    '<span>{{dataItem.Type}}</span>',
    //    dataBound: function (e) {
    //        var supplier = $("#supplier").data("kendoComboBox");
    //        if (supplier != undefined) {
    //            supplier.setOptions({
    //                template: $.proxy(kendo.template("#= formatValue(SUPPLIER_EDESC, this.text()) #"), supplier)
    //            });
    //        }
    //    }
    // }

    $scope.supplierCodeOption = {
        dataSource: $scope.supplierDataSource,
        template: '<span>{{dataItem.SUPPLIER_EDESC}}</span>  --- ' +
              '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            
            if (this.element[0].attributes['supplier-index'] == undefined) {
                var supplier = $("#supplier").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['supplier-index'].value;
                var supplierlength = ((parseint(index) + 1) * 3) - 1;
                var supplier = $($(".supplier")[supplierlength]).data("kendoComboBox");
            }
        //    if (supplier != undefined) {
        //        supplier.setOptions({
        //            template: $.proxy(kendo.template("#= formatValue(SUPPLIER_EDESC,Type, this.text()) #"), supplier)
        //        });
        //    }
        }
   
    }




    $scope.supplierCodeOnChange = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.suppliercodeerror = "Please Enter Valid Code."
            $rootScope.suppliervalidation = $scope.suppliercodeerror;
            $('#supplier').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.suppliercodeerror = "";
            $rootScope.suppliervalidation = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }

    $scope.orientation = "horizontal";

    //customer popup advanced search// --start

    var getSuppliersByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetSuppliers";
    $scope.treeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getSuppliersByUrl,
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
                id: "MASTER_SUPPLIER_CODE",
                parentId: "PRE_SUPPLIER_CODE",
                children: "Items",
                fields: {
                    SUPPLIER_CODE: { field: "SUPPLIER_CODE", type: "string" },
                    SUPPLIER_EDESC: { field: "SUPPLIER_EDESC", type: "string" },
                    parentId: { field: "PRE_SUPPLIER_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    //treeview expand on startup
    $scope.onDataBound = function () {
        $('#suppliertree').data("kendoTreeView").expand('.k-item');
    }

    //treeview on select
    $scope.options = {
        loadOnDemand: false,
        select: function (e) {
            
            var currentItem = e.sender.dataItem(e.node);
            if ($scope.childRowIndex == undefined) {
                $('#supplierGrid').removeClass("show-displaygrid");
                $("#supplierGrid").html("");
            }
            else {
                $('#supplierGrid_' + $scope.childRowIndex).removeClass("show-displaygrid");
                $("#supplierGrid_" + $scope.childRowIndex).html("");
            }
            BindSupplierGrid(currentItem.supplierId, currentItem.masterSupplierCode, "");
            $scope.$apply();
        },
       
    };

    //search whole data on search button click
    $scope.BindSearchGrid = function () {
        
        $scope.searchText = $scope.txtSearchString;
        BindSupplierGrid("", "", $scope.searchText);
    }


    //Grid Binding main Part
    function BindSupplierGrid(supplierId, supplierMasterCode, searchText) {
        $scope.supplierGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetSupplierListBySupplierCode?supplierId=" + supplierId + '&supplierMasterCode=' + supplierMasterCode + '&searchText=' + searchText,
                },

                schema: {
                    type: "json",
                    model: {
                        fields: {
                            SUPPLIER_CODE: { type: "string" },
                            SUPPLIER_EDESC: { type: "string" }
                        }
                    }
                },
                pageSize: 30,

            },
            scrollable: true,
            sortable: true,
            resizable: true,
            pageable: true,
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
                        startswith: "Starts with",
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
            },
           dataBound: function (e) {
        
        if ($scope.childRowIndex == undefined) {
            $("#supplierGrid tbody tr").css("cursor", "pointer");
            $("#supplierGrid tbody tr").on('dblclick', function () {
              
                var suppliercode = $(this).find('td span').html();
                $scope.masterModels["SUPPLIER_CODE"] = suppliercode;
                if ($("#supplier").hasClass('borderRed')) {
                    $scope.suppliercodeerror = "";
                    $("#supplier").removeClass('borderRed');
                }
                $('#supplierModal').modal('toggle');
                $scope.$apply();
            })
        }
        else {
            $("#supplierGrid_" + $scope.childRowIndex + " tbody tr").css("cursor", "pointer");
            $("#supplierGrid_" + $scope.childRowIndex + " tbody tr").on('dblclick', function () {
                
                var suppliercode = $(this).find('td span').html();
                $scope.childModels[$scope.childRowIndex]["SUPPLIER_CODE"] = suppliercode;
                $('#supplierModal_' + $scope.childRowIndex).modal('toggle');
                $scope.$apply();
            })
        }
    },
   
            columns: [{
                field: "SUPPLIER_CODE",
                hidden: true,
                title: "Supplier Code",

            }, {
                field: "SUPPLIER_EDESC",
                title: "Supplier Name",

            }, {
                field: "REGD_OFFICE_EADDRESS",
                title: "Address",

            }, {
                field: "TEL_MOBILE_NO1",
                title: "Phone",

            }]
        };
    }
   

    //show modal popup
    $scope.BrowseTreeListForSupplier = function (index) {
        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            if ($scope.ModuleCode != '01' && referencenumber !== "") {
                return;
            }
        }
       // $('#supplierModal').modal('show');
        if ($scope.freeze_master_ref_flag == "N") {
            $scope.childRowIndex = index;
            document.popupindex = index;
            if (index == undefined) {
                $('#supplierModal').modal('show');
            }
            else {
                $('#supplierModal_' + index).modal('show');
                if ($('#suppliertree_' + $scope.childRowIndex).data("kendoTreeView") != undefined)
                    $('#suppliertree_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
            }
        }
    }
  
});