
DTModule.controller('lineItemChargeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {

    $scope.childRowIndex;
    $scope.productDataSource = {
        type: "json",
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllProductsListByFilter",

            },
            parameterMap: function (data, action) {
                var ItemCodeValue = $rootScope.ITEM_CODE_DEFAULTVAL;
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
                        //newParams = {
                        //    filter: ""
                        //};
                        //return newParams;

                        if (ItemCodeValue != "" && ItemCodeValue != undefined) {
                            newParams = {
                                filter: ItemCodeValue
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

                }
                else {
                    //newParams = {
                    //    filter: ""
                    //};
                    //return newParams;
                    if (ItemCodeValue != "" && ItemCodeValue != undefined) {
                        newParams = {
                            filter: ItemCodeValue
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
            }
        },

    };

    $scope.ItemCodeOption = {
        dataSource: $scope.productDataSource,
        template: '<span>{{dataItem.ItemDescription}}</span>  --- ' +
            '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            var index = this.element[0].attributes['product-index'].value;
            var productLength = ((parseInt(index) + 1) * 3) - 1;
            var product = $($(".cproducts")[productLength]).data("kendoComboBox");
            if (product != undefined) {
                product.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(ItemDescription,Type,this.text()) #"), product)
                });
                $scope.getmucode(index, $rootScope.ITEM_CODE_DEFAULTVAL);

            }
            setTimeout(function () {
                if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
                    $(".cproducts").prop('readonly', true);
                }
            }, 50);
        }
    }

    //customer popup advanced search// --start

    var getProductsByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetProducts";
    $scope.treeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getProductsByUrl,
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
                id: "ITEM_CODE",
                parentId: "PRE_ITEM_CODE",
                children: "Items",
                fields: {
                    ITEM_CODE: { field: "ITME_CODE", type: "string" },
                    ITEM_EDESC: { field: "ITEM_EDESC", type: "string" },
                    parentId: { field: "PRE_ITEM_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    //treeview expand on startup
    $scope.onDataBound = function () {
        //$('#producttree_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
    }

    //treeview on select
    $scope.options = {
        loadOnDemand: false,
        select: function (e) {
            var currentItem = e.sender.dataItem(e.node);
            $('#productGrid_' + $scope.childRowIndex).removeClass("show-displaygrid");
            $("#productGrid_" + $scope.childRowIndex).html("");
            //BindProductGrid(currentItem.itemCode, currentItem.masterItemCode, "");
            $scope.$apply();
        },
    };

    //search whole data on search button click
    $scope.BindSearchGrid = function () {
        $scope.searchText = $scope.txtSearchString;
        //BindProductGrid("", "", $scope.searchText);
    }

    //Grid Binding main Part
    function BindProductGrid(itemCode, itemMasterCode, searchText) {
        $scope.productGridOptions = {
            dataSource: {
                type: "json",
                transport: {
                    read: "/api/TemplateApi/GetProductListByItemCode?itemCode=" + itemCode + '&itemMastercode=' + itemMasterCode + '&searchText=' + searchText,
                },
                schema: {
                    type: "json",
                    model: {
                        fields: {
                            ITEM_CODE: { type: "string" },
                            ITEM_EDESC: { type: "string" }
                        }
                    }
                },
                pageSize: 30,
            },
            scrollable: true,
            sortable: true,
            resizable: false,
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
                $("#productGrid_" + $scope.childRowIndex + " tbody tr").css("cursor", "pointer");
                $("#productGrid_" + $scope.childRowIndex + " tbody tr").on('dblclick', function () {

                    var productcode = $(this).find('td span').html();
                    /*var productName = $(this).find('td span[ng-bind="dataItem.ITEM_EDESC"]').html();*/
                    var productName = 30;
                    console.log("ProductName==================>>>" + productName);
                    $("#products_" + $scope.childRowIndex).data('kendoComboBox').dataSource.data([{ ItemCode: productcode, ItemDescription: productName, Type: "code" }]);
                    $scope.childModels[$scope.childRowIndex]["ITEM_CODE"] = productcode;
                    if ($($(".cproduct_" + $scope.childRowIndex)[0]).closest('div').parent().hasClass('borderRed')) {
                        $($(".cproduct_" + $scope.childRowIndex)[0]).closest('div').parent().removeClass('borderRed')
                    }
                    $('#lineItemChargeModal_' + $scope.childRowIndex).modal('toggle');
                    $scope.$apply();
                })
            },
            columns: [{
                hidden: true,
                field: "ITEM_CODE",

            },
            {
                field: "ITEM_EDESC",
                title: "item Name",
                width: "120px"
            },
            {
                field: "MU_EDESC",
                title: "INDEX UNIT",
                width: "120px"
            },
            {
                field: "CATEGORY_EDESC",
                title: "CATEGORY",
                width: "120px"
            },
            {
                field: "PURCHASE_PRICE",
                title: "Purchase Price",
                width: "120px",
                template: "#= (PURCHASE_PRICE == null) ? '0 ' : PURCHASE_PRICE #"
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
            }


            ]
        };
    }



    //show modal popup
    $scope.BrowseLineItemCharge = function (index, columnName, customer) {

       
       
        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            var custref = $(".referenceCustomer").val();
            var btnref = $("#refrenceTypeMultiSelect").data('kendoDropDownList').dataItem().REF_CODE;
            if ($scope.ModuleCode != '01' && (referencenumber !== "" || custref !== "" || btnref !== "")) {
                return;
            }
        }
       
        if ($scope.freeze_master_ref_flag == "N") {

            var manualcalcChargeValue = $scope.lineItemChargeDetails.filter(t => t.CHARGE_CODE == columnName)[0].MANUAL_CALC_CHARGE;
            if (manualcalcChargeValue == 'Y') {
                $('.clsManualCalCharge_N').hide(); $('.clsManualCalCharge_Y').show();
            }
            else {  $('.clsManualCalCharge_N').show(); $('.clsManualCalCharge_Y').hide(); }
            $scope.ChargeCode = columnName;
            $('#lineItemChargeModal_' + index).modal('show');
            if ($scope.percentFlag === undefined) {
                var req = "/api/TemplateApi/GetLineItemChargeParticularInfo?companycode=" + $scope.companycode + "&FormCode=" + $scope.FormCode + "&ChargeCode=" + columnName + "&CustomerCode=" + $scope.masterModels.CUSTOMER_CODE + "&ItemCode=" + $scope.childModels[index]["ITEM_CODE"];
                $http.get(req).then(function (results) {
                   
                    $scope.percentFlag = results.data[0].VALUE_PERCENT_FLAG;
                    $scope.percentAmount = results.data[0].VALUE_PERCENT_AMOUNT;
                    $scope.ChargeCode = columnName;
                    $scope.ManualCalCharge = results.data[0].MANUAL_CALC_CHARGE;
                    $scope.lineItemParticularChargeDetails = results.data;
                   
                    $('#lineItemChargeModal_' + index).modal('show');
                   
                    if ($scope.percentFlag == 'P') {
                        $('#P_Id').val(results.data[0].VALUE_PERCENT_AMOUNT);
                        $('.' + columnName).val(results.data[0].VALUE_PERCENT_AMOUNT);
                        $('#P').val($scope.percentFlag);
                        $('#P').prop('checked', true);
                        $scope.percentFlag = 'P';
                        $('#Q_Id').val(0);
                        $('#V_Id').val(0);
                    }
                    if ($scope.percentFlag == 'V') {
                        $('#V_Id').val(results.data[0].VALUE_PERCENT_AMOUNT);
                        $('.' + columnName).val(results.data[0].VALUE_PERCENT_AMOUNT);
                        $('#V').val($scope.percentFlag);
                        $scope.percentFlag = 'V';
                        $('#V').prop('checked', true);
                        $('#P_Id').val(0);
                        $('#Q_Id').val(0);
                    }
                    if ($scope.percentFlag == 'Q') {
                        $('#Q_Id').val(results.data[0].VALUE_PERCENT_AMOUNT);
                        $('.' + columnName).val(results.data[0].VALUE_PERCENT_AMOUNT);
                        $('#Q').val($scope.percentFlag);
                        $scope.percentFlag = 'Q';
                        $('#Q').prop('checked', true);
                        $('#P_Id').val(0);
                        $('#V_Id').val(0);
                    }
                });
            }
            else {
                $('#lineItemChargeModal_' + index).modal('show');
                $scope.percentFlag = $scope.lineItemChargeDetails.VALUE_PERCENT_FLAG;
                $scope.percentAmount = $scope.childModels[index].columnName;
                $scope.ChargeCode = columnName;

                if ($scope.lineItemChargeDetails.CHARGE_CODE == columnName)
                {
                    if ($scope.lineItemChargeDetails.VALUE_PERCENT_FLAG == 'P')
                    {
                        $('#p_id').val($scope.childModels[index].columnName);
                        $('#p').val('P');
                        $scope.percentFlag = 'P';
                        $('#q_id').val(0);
                        $('#v_id').val(0);
                    }
                    if ($scope.lineItemChargeDetails.VALUE_PERCENT_FLAG == 'V') {
                        $('#v_id').val($scope.childModels[index].columnName);
                        $('#v').val('V');
                        $scope.percentFlag = 'V';
                        $('#q_id').val(0);
                        $('#p_id').val(0);
                    }
                    if ($scope.lineItemChargeDetails.VALUE_PERCENT_FLAG == 'Q') {
                        $('#q_id').val($scope.childModels[index].columnName);
                        $('#q').val('Q');
                        $scope.percentFlag = 'Q';
                        $('#p_id').val(0);
                        $('#v_id').val(0);
                    }
				}
            }
            $('input#P.P_checkRadio').change(function (e) {
                e.preventDefault;
                $scope.percentFlag = 'P';
                $('#p_id').val($scope.childModels[index].columnName);
                $scope.lineItemChargeDetails.VALUE_PERCENT_FLAG = 'P';
                $('#P').val('P');
                $(this).prop('checked', true);
                $('#V_Id').val(0);
                $('#Q_Id').val(0);
            });
            $('input#V.V_checkRadio').change(function (e) {
                e.preventDefault;
                $scope.percentFlag = 'V';
                $(this).prop('checked', true);
                $('#V_Id').val($scope.childModels[index].columnName);
                $scope.lineItemChargeDetails.VALUE_PERCENT_FLAG = 'V';
                $('#V').val('V');
                $('#P_Id').val(0);
                $('#Q_Id').val(0);
            });
            $('input#Q.Q_checkRadio').change(function (e) {
                e.preventDefault;
                $scope.percentFlag = 'Q';
                $(this).prop('checked', true);
                $('#Q_Id').val($scope.childModels[index].columnName);
                $scope.lineItemChargeDetails.VALUE_PERCENT_FLAG = 'Q';
                $('#Q').val('Q');
                $('#P_Id').val(0);
                $('#V_Id').val(0);

            });
        }
    }
});

