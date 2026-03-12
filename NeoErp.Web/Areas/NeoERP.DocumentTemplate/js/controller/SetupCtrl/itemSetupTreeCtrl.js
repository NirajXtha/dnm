
DTModule.controller('itemSetupTreeCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter, $timeout) {
    $scope.saveupdatebtn = "Save";
    $scope.itemArr;
    $scope.savegroup = false;
    $scope.editFlag = "N";
    $scope.treenodeselected = true;
    $scope.newrootinsertFlag = "Y";
    $scope.treeselecteditemcode = "";
    $scope.treeselectedmastercode = "";
    $scope.editcode = "";
    $scope.edesc = "";
    $scope.AE = false;
    $scope.primitive_value = true;
    $scope.updateparentcode = "";
    var d1 = "";
    var d2 = "";
    $scope.parentchangenew = true;
    $scope.chargehideshow = true;
    $scope.parentaccountcodechange = true;
    $scope.chargecurrentitemindex = "";
    $scope.type = "";
    $scope.CHARGE_INDEX_MU_CODE = "";
    $scope.IsChargeValidate = true;
    $scope.ChargeShowRequired = false;
    $scope.imageurledit = "";
    $scope.itemsetup =
    {
        AVG_RATE: "",
        BATCH_FLAG: "N",
        BATCH_SERIAL_FLAG: "N",
        NON_VAT_FLAG: "N",
        FREEZE_FLAG: "N",
        BRANCH_CODE: "",
        CATEGORY_CODE: "",
        COMPANY_CODE: "",
        COSTING_METHOD_FLAG: "",
        CREATED_BY: "",
        CREATED_DATE: "",
        CURRENT_STOCK: "",
        DANGER_LEVEL: "",
        DEFAULT_WIP_STOCK: "",
        DELETED_FLAG: "",
        DELTA_FLAG: "",
        DIMENSION: "",
        ECO_ORDER_QUANTITY: "",
        FRACTION_VALUE: "",
        GROUP_SKU_FLAG: "",
        HS_CODE: "",
        RACK_LOCATION: "",
        IMAGE_FILE_NAME: "",
        INDEX_MU_CODE: "",
        ITEM_CODE: "",
        ITEM_EDESC: "",
        ITEM_NDESC: "",
        LEAD_TIME: "",
        LINK_SUB_CODE: "",
        MASTER_ITEM_CODE: "",
        MAX_LEVEL: "",
        MAX_USAGE: "",
        MAX_VALUE: "",
        MIN_LEVEL: "",
        MIN_USAGE: "",
        MIN_VALUE: "",
        MODIFY_BY: "",
        MODIFY_DATE: "",
        MULTI_MU_CODE: "",
        NORMAL_USAGE: "",
        PREFERRED_LEVEL: "",
        PREFERRED_SUPPLIER_CODE: "",
        PRE_ITEM_CODE: "",
        PRODUCT_CODE: "",
        PURCHASE_PRICE: "",
        REEM_WEIGHT_KG: "",
        REMARKS: "",
        REMARKS2ND: "",
        REORDER_LEVEL: "",
        SALES_PRICE: "",
        SERIAL_FLAG: "N",
        SERIAL_PREFIX_LENGTH: "",
        SERIAL_PREFIX_TEXT: "",
        SERVICE_ITEM_FLAG: "N",
        SHELF_LIFE_DAYS: "",
        SYN_ROWID: "",
        VALUATION_FLAG: "",
        REMARKS: "",
        REMARKS2ND: "",
    }

    $scope.PARENT_ITEM_CODE = "";
    $scope.multimuConversion = {
        MU_CODE: "",
        CONVERSION_FACTOR: "",
        FRACTION: "",
        REMARKS: ""
    }
    $scope.integration = {
        ACC_CODE: "",
        FORM_CODE: ""
    }
    $scope.integrationArr = [{
        ACC_CODE: "",
        FORM_CODE: ""
    }];
    $scope.inteArr = [{
        ACC_CODE: "",
        FORM_CODE: ""
    }];
    $scope.multiMuArr = [{
        MU_CODE: "",
        CONVERSION_FACTOR: "",
        FRACTION: "",
        REMARKS: ""
    }];
    $scope.parameterSpecification = [{
        PARAMETER_ENAME: "",
        PARAMETER_NNAME: "",
        IDEAL_VALUE: "",
        MIN_VALUE: "",
        MAX_VALUE: "",
        REMARKS: ""
    }];

    $scope.itemSpec = {
        ITEM_CODE: "",
        PART_NUMBER: "",
        BRAND_NAME: "",
        ITEM_SPECIFICATION: "",
        ITEM_APPLY_ON: "",
        INTERFACE: "",
        TYPE: "",
        LAMINATION: "",
        ITEM_SIZE: "",
        THICKNESS: "",
        COLOR: "",
        GRADE: "",
        REMARKS: "",
        SYN_ROWID: "",
        GSM: "",
        SIZE_LENGHT: "",
        SIZE_WIDTH: "",
        SPEC_COMPULSORY_FLAG: "",
        ITEM_ATTRIBUTE_CODES: [],
    };

    $scope.itemSpecification = $scope.itemSpec;

    $scope.qcParameter = [{
        PARAMETER_NAME: "",
        PERCENTAGE: "",
        REMARKS: ""
    }]

    $scope.charges = [{
        FORM_CODE: "",
        CHARGE_CODE: "",
        CHARGE_TYPE: "",
        VALUE_QUANTITY_BASED: "",
        VALUE_PERCENT_FLAG: "",
        VALUE_PERCENT_AMOUNT: "",
        CHARGE_INDEX_UNIT: "",
        ACC_CODE: "",
        SUB_CODE: "",
        IMPACT_ON: "",
        APPLY_QUANTITY: "",

        //CHARGE_DOCUMENT: "",
        //CHARGE: "",
        //VALUE_PERCENT: "",
        //VALUE_RATE: "",
        //TAKE_QUANTITY: "",
        //IMPACT_ON: "",
        //BASED_ON: "",
        //ACC_CODE: "",
        //SUB_CODE: ""
    }]

    debugger;
    $scope.GroupMU = { MU_CODE: "", MU_DESC: "" };
    $scope.add_parameter_spec = function (parameterSpecDetail) {

        $scope.parameterSpecification.push({
            'PARAMETER_ENAME': parameterSpecDetail.NAME,
            'PARAMETER_NNAME': parameterSpecDetail.DESIGNATION,
            'IDEAL_VALUE': parameterSpecDetail.CONTACT_PERSON,
            'MAX_VALUE': parameterSpecDetail.TELEPHONE_NO,
            'MIN_VALUE': parameterSpecDetail.TELEPHONE_NO,
            'REMARKS': parameterSpecDetail.REMARKS
        });
    };
    $scope.remove_parameter_spec = function (index) {
        if ($scope.parameterSpecification.length > 1) {
            $scope.parameterSpecification.splice(index, 1);
        }
    }

    $scope.itemArr = $scope.itemsetup;
    $scope.initchargeradio = function () {
        $scope.itemArr.CALCULATION_BASIS = "A";
        $scope.itemArr.VALUE_PERCENT = "V";
        $scope.itemArr.Impact = "R";
    };

    $scope.MACDS = [];
    //$scope.chargedocumentcoun"";
    //#region "drop down or combo box region start"

    var itemCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getProductCodeWithChild";

    $scope.itemGroupDataSource = {
        transport: {
            read: {
                url: itemCodeUrl,
            }
        }
    };

    $scope.itemGroupOptions = {
        dataSource: $scope.itemGroupDataSource,
        optionLabel: "<Primary>",
        dataTextField: "ITEM_EDESC",
        dataValueField: "ITEM_CODE",
        filter: "contains",
        select: function (e) {

            $rootScope.quickmasteritemcode = e.dataItem.MASTER_ITEM_CODE;

        },
    }

    //var attributeCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAttributeFromPreference";

    //$scope.attributeDataSource = {
    //    transport: {
    //        read: {
    //            url: attributeCodeUrl,
    //        }
    //    }
    //};
    //debugger;
    //$scope.attributeOptions = {
    //    dataSource: $scope.attributeDataSource,
    //    select: function (e) {
    //        debugger;
    //        $rootScope.quickmasteritemcode = e.dataItem.MASTER_ITEM_CODE;

    //    },
    //}

    $scope.loadAttributeData = function () {
        var attributeCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAttributeFromPreference";

        $http.get(attributeCodeUrl)
            .then(function (response) {
                debugger;
                // ✅ API returned successfully
                $scope.attributeData = response.data;

                // Example: set something globally
                if ($scope.attributeData.length > 0) {
                    $rootScope.attributeFromPreferenceSetup = $scope.attributeData;
                }

                $rootScope.showAttributeTab = ($rootScope.attributeFromPreferenceSetup === "Y");

            })
            .catch(function (error) {
                console.error("Error loading attributes:", error);
            });
    };

    // 🔥 Automatically call when controller loads
    $scope.loadAttributeData();



    var muCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetMuCode";
    $scope.muCodeDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: muCodeUrl,
            }
        }
    });

    $scope.Bind = function () {

        $('.k-list').slimScroll({
            height: '250px'
        });
    };

    //PARENT MU CODE
    $scope.muCodeOptions = {
        dataSource: new kendo.data.DataSource({
            transport: {
                read: {
                    url: muCodeUrl,
                }
            }
        }),
        dataTextField: "MU_EDESC",
        dataValueField: "MU_CODE",
        filter: "contains",
        change: function (e) {
        },
        dataBound: function () {
        },
    };

    // To prevent Index Units and Tran Unit being cleared when incorrect value not present in the dropdown is typed in UOM Name
    $timeout(function () {
        var allComboBoxInputs = $('.MU_CODE-input[data-role="combobox"]');

        angular.forEach(allComboBoxInputs, function (inputElement) {
            var comboBox = $(inputElement).data("kendoComboBox");
            comboBox.input.on("input", function () {
                $scope.$watch('itemArr.INDEX_MU_CODE', function (newVal, oldVal) {
                    if (!newVal) {
                        console.warn("Blocked clearing INDEX_MU_CODE");
                        $scope.itemArr.INDEX_MU_CODE = oldVal;
                    }
                });

                $scope.$watch('itemArr.MULTI_MU_CODE', function (newVal, oldVal) {
                    if (!newVal) {
                        console.warn("Blocked clearing MULTI_MU_CODE");
                        $scope.itemArr.MULTI_MU_CODE = oldVal;
                    }
                });
            });
        })
    });

    //charge index unit
    $scope.chargechargeindexunitDataSource = [{ MU_CODE: "", MU_EDESC: "" }];
    $scope.chargeindexunitOptions = {
        dataSource: $scope.chargechargeindexunitDataSource,
        dataTextField: "MU_EDESC",
        dataValueField: "MU_CODE",
        filter: "contains",
        dataBound: function () {
        }
    };

    //ASSOC CODE
    $scope.UCmuCodeOptions = {
        dataSource: $scope.muCodeDataSource,
        dataTextField: "MU_CODE",
        dataValueField: "MU_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
            $scope.GroupMU.MU_CODE = currentItem.MU_CODE;
        },
        dataBound: function () {
        }
    };
    //ASSCO DESC CODE
    $scope.UCmuDescOptions = {
        dataSource: new kendo.data.DataSource({
            transport: {
                read: {
                    url: muCodeUrl,
                }
            }
        }),
        dataTextField: "MU_EDESC",
        dataValueField: "MU_CODE",
        filter: "contains",
        dataBound: function () {
        }
    }

    $scope.onChangeIndexUnitType = function (e) {
        var currentItem = e.sender.dataItem(e.node);
        $scope.GroupMU.MU_CODE = currentItem.MU_CODE;
        $scope.charges;
        var len = $scope.charges.length;
        for (var i = 0; i < len; i++) {
            var chargeindexunitComboBox = $("#CHARGE_INDEX_UNIT_" + i).data("kendoComboBox");
            $scope.CHARGE_INDEX_MU_CODE = [{ MU_CODE: currentItem.MU_CODE, MU_EDESC: currentItem.MU_EDESC }]
            chargeindexunitComboBox.setDataSource($scope.CHARGE_INDEX_MU_CODE);
            chargeindexunitComboBox.value(currentItem.MU_CODE);
        }
        $scope.itemArr.MULTI_MU_CODE = e.sender.dataItem().MU_CODE;
    };

    $scope.onChangeInitialIndexUnitType = function (e) {
        $scope.GroupMU.MU_CODE = $scope.itemArr.INDEX_MU_CODE;
        $scope.charges;
        var len = $scope.charges.length;
        for (var i = 0; i < len; i++) {
            var chargeindexunitComboBox = $("#CHARGE_INDEX_UNIT_" + i).data("kendoComboBox");
            //$scope.CHARGE_INDEX_MU_CODE = [{ MU_CODE: currentItem.MU_CODE, MU_EDESC: currentItem.MU_EDESC }]
            chargeindexunitComboBox.setDataSource($scope.CHARGE_INDEX_MU_CODE);
            chargeindexunitComboBox.value($scope.itemArr.INDEX_MU_CODE);
        }
        $scope.itemArr.MULTI_MU_CODE = $scope.itemArr.INDEX_MU_CODE;
    };

    var accountUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllAccountCode";
    $scope.accountsDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: accountUrl,
            }
        }
    });

    $scope.accountsOptions = {
        dataSource: $scope.accountsDataSource,
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        filter: "contains",
        change: function (e) {
        },
        dataBound: function () {
            $scope.Bind();
        }
    };

    //var chargeaccountUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllAccountCode";
    var chargeaccountUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetAllChargeAccountSetupByFilter";

    $scope.chargeaccountsDataSource = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: chargeaccountUrl,
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
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
    });

    //$scope.chargeaccountsDataSource = new kendo.data.DataSource({
    //    transport: {
    //        read: {
    //            url: accountUrl,
    //        }
    //    }
    //});


    $scope.chargeaccountsOptions = {
        dataSource: $scope.chargeaccountsDataSource,
        dataTextField: "ACC_EDESC",
        dataValueField: "ACC_CODE",
        filter: "contains",
        change: function (e) {

            //var currentindex = this.element[0].attributes['account-index'].value;
            //var type = this.element[0].attributes['type'].value;
            //$scope.type = type;
            //$scope.chargecurrentitemindex = currentindex;
            //var currentItem = e.sender.dataItem(e.node);
            //if (currentItem!==undefined) {
            //    var accountcode = currentItem.ACC_CODE;
            //}
            //$scope.GetSubCodeByAccountCode(accountcode);
        },
        dataBound: function () {

            $scope.Bind();
        }
    };



    //$scope.GetSubCodeByAccountCode();
    $scope.subLedgerDataSource = [{ SUB_EDESC: "", SUB_CODE: "" }];
    $scope.subLedgerOptions = {
        dataSource: $scope.subLedgerDataSource,
        dataTextField: "SUB_EDESC",
        dataValueField: "SUB_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
            // $scope.bind();
        }
    }
    $scope.GetSubCodeByAccountCode = function (accountcode, i) {
        var subLedgerByAccUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetSubLedgerbyAccountCode?accountcode=" + accountcode;
        $http({
            method: 'GET',
            url: subLedgerByAccUrl,

        }).then(function successCallback(response) {

            var currentindex = $scope.chargecurrentitemindex;
            //var currenttype = $scope.type;

            $scope.itemArr;
            var type = $scope.itemArr.GROUP_SKU_FLAG;
            if (type == "I") {
                if (i === "") {
                    var subCodeComboBox = $("#CHILD_CHARGE_SUB_CODE_" + currentindex).data("kendoComboBox");
                } else {
                    var subCodeComboBox = $("#CHILD_CHARGE_SUB_CODE_" + i).data("kendoComboBox");
                }
            }
            if (type == "G") {
                if (i === "") {
                    var subCodeComboBox = $("#CHARGE_SUB_CODE_" + currentindex).data("kendoComboBox");
                } else {
                    var subCodeComboBox = $("#CHARGE_SUB_CODE_" + i).data("kendoComboBox");
                }
            }
            subCodeComboBox.setDataSource(response.data.MESSAGE);
        });
    };



    //CHARGE SELECT OPTION KENDO DROP DOWNLIST START
    //$scope.CHARGE_TYPE_DATASCOURCE = [{ text: "Addition", value: "A" }, { text: "Deduction", value: "D" }];
    //$scope.chargetypeOptions = {
    //    dataSource: $scope.CHARGE_TYPE_DATASCOURCE,
    //    dataTextField: "text",
    //    dataValueField: "value",
    //    filter: "contains",
    //    change: function (e) {
    //        var currentItem = e.sender.dataItem(e.node);
    //    },
    //    dataBound: function () {
    //        // $scope.bind();
    //    }
    //}
    //$scope.VALUE_QUANTITY_BASED_DATASCOURCE = [{ text: "Amount", value: "V" }, { text: "Quantity", value: "P" }];
    //$scope.valuequantityOptions = {
    //    dataSource: $scope.VALUE_QUANTITY_BASED_DATASCOURCE,
    //    dataTextField: "text",
    //    dataValueField: "value",
    //    filter: "contains",
    //    change: function (e) {
    //        var currentItem = e.sender.dataItem(e.node);
    //    },
    //    dataBound: function () {
    //        // $scope.bind();
    //    }
    //}
    //$scope.VALUE_PERCENT_FLAG_DATASCOURCE = [{ text: "Value", value: "V" }, { text: "Percentage", value: "P" }];
    //$scope.valuepercentageOptions = {
    //    dataSource: $scope.VALUE_PERCENT_FLAG_DATASCOURCE,
    //    dataTextField: "text",
    //    dataValueField: "value",
    //    filter: "contains",
    //    change: function (e) {
    //        var currentItem = e.sender.dataItem(e.node);
    //    },
    //    dataBound: function () {
    //        // $scope.bind();
    //    }
    //}
    //$scope.APPLY_QUANTITY_DATASCOURCE = [{ text: "Document", value: "D" }, { text: "Cumulative", value: "C" }];
    //$scope.applyquantityOptions = {
    //    dataSource: $scope.APPLY_QUANTITY_DATASCOURCE,
    //    dataTextField: "text",
    //    dataValueField: "value",
    //    filter: "contains",
    //    change: function (e) {
    //        var currentItem = e.sender.dataItem(e.node);
    //    },
    //    dataBound: function () {
    //        // $scope.bind();
    //    }
    //}
    //$scope.IMPACT_ON_DATASCOURCE = [{ text: "Rate", value: "R" }, { text: "Quantity", value: "Q" }];
    //$scope.impactonOptions = {
    //    dataSource: $scope.IMPACT_ON_DATASCOURCE,
    //    dataTextField: "text",
    //    dataValueField: "value",
    //    filter: "contains",
    //    change: function (e) {
    //        var currentItem = e.sender.dataItem(e.node);
    //    },
    //    dataBound: function () {
    //        // $scope.bind();
    //    }
    //}
    //END





    //var subLedgerUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetSubLedger";
    //$scope.subLedgerDataSource = new kendo.data.DataSource({
    //    transport: {
    //        read: {
    //            url: subLedgerUrl,
    //        }
    //    }
    //});
    //$scope.subLedgerOptions = {
    //    dataSource: $scope.subLedgerDataSource,
    //    dataTextField: "SUB_EDESC",
    //    dataValueField: "SUB_CODE",
    //    filter: "contains",
    //    change: function (e) {
    //        var currentItem = e.sender.dataItem(e.node);
    //    },
    //    dataBound: function () {
    //    }
    //}


    var integrationformSetupUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetFormSetup";
    $scope.formSetupDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: integrationformSetupUrl,
            }
        }
    });




    $scope.formSetupOptions = {
        dataSource: $scope.formSetupDataSource,
        dataTextField: "FORM_EDESC",
        dataValueField: "FORM_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
            $scope.Bind();
        }

    };


    var formSetupUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetFormSetup";
    $scope.chargeformSetupDataSource = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: formSetupUrl,
            },
            parameterMap: function (data, action) {

                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
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
    });





    $scope.changechargecodebyformcode = function (e) {


        var currentindex = e.sender.element[0].attributes['document-index'].value;
        //var type = e.sender.element[0].attributes['k-type'].value;
        //$scope.type = type;
        $scope.chargecurrentitemindex = currentindex;
        //var currentItem = e.sender.dataItem(e.node);
        //if (currentItem !== undefined) {

        $scope.ChargeShowRequired = true;
        var formcode = $(e.sender.element[0]).val();
        var i = "";
        $scope.GetChargeByFormCode(formcode, i);

        //  }

        var sublen = $scope.charges.length;
        for (var j = 0; j < sublen; j++) {

            var subcode = $scope.charges[j].FORM_CODE;
            if (currentindex != j) {

                if (subcode === formcode) {

                    $($(".chargeDocument_" + currentindex)[0]).addClass("borderRed");
                    $("#savedocumentformdata").prop("disabled", true);
                    $scope.chargedocumentcount = true;
                    return;

                }
                else {

                    $($(".chargeDocument_" + currentindex)[0]).removeClass("borderRed");
                    $scope.chargedocumentcount = false;
                    $("#savedocumentformdata").prop("disabled", false);
                };

            }


        }
    }

    $scope.ChangeSubCodeByAccountCode = function (e) {

        var currentindex = e.sender.element[0].attributes['account-index'].value;
        //var type = e.sender.element[0].attributes['k-type'].value;
        //        $scope.type = type;
        $scope.chargecurrentitemindex = currentindex;
        //var currentItem = e.sender.dataItem(e.node);
        //   if (currentItem!==undefined) {
        var accountcode = $(e.sender.element[0]).val();
        //     }
        var i = "";
        $scope.GetSubCodeByAccountCode(accountcode, i);
    }

    //charge charge


    $scope.chargeSetupDataSource = [{ CHARGE_CODE: "", CHARGE_EDESC: "" }];

    $scope.ChargeSetupOptions = {
        dataSource: $scope.chargeSetupDataSource,
        dataTextField: "CHARGE_EDESC",
        dataValueField: "CHARGE_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        //dataBound: function (e) {

        // }
    };


    //document option for charge
    //$scope.chargeformSetupOptions = {
    //    dataSource: $scope.chargeformSetupDataSource,
    //    dataTextField: "FORM_EDESC",
    //    dataValueField: "FORM_CODE",
    //    filter: "contains",
    //    change: function (e) {
    //        var currentindex = this.element[0].attributes['document-index'].value;
    //        var type = this.element[0].attributes['type'].value;
    //        $scope.type = type;
    //        $scope.chargecurrentitemindex = currentindex;
    //        var currentItem = e.sender.dataItem(e.node);
    //        if (currentItem !== undefined) {
    //            var formcode = currentItem.FORM_CODE;
    //        }
    //        $scope.GetChargeByFormCode(formcode);
    //    },
    //    dataBound: function () {
    //    }
    //};


    $scope.GetChargeByFormCode = function (formcode, i) {
        var ChargebyFormCodeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetChargeCodebyFormCode?formcode=" + formcode;
        $http({
            method: 'GET',
            url: ChargebyFormCodeUrl,

        }).then(function successCallback(response) {

            var currentindex = $scope.chargecurrentitemindex;
            var currenttype = $scope.type;
            $scope.itemArr;
            var type = $scope.itemArr.GROUP_SKU_FLAG;
            if (type == "I") {
                if (i === "") {
                    var ChargeComboBox = $("#CHILD_CHARGE_CHARGE_" + currentindex).data("kendoComboBox");
                } else {
                    var ChargeComboBox = $("#CHILD_CHARGE_CHARGE_" + i).data("kendoComboBox");
                }


            }
            if (type == "G") {
                if (i === "") {
                    var ChargeComboBox = $("#CHARGE_CHARGE_" + currentindex).data("kendoComboBox");
                } else {
                    var ChargeComboBox = $("#CHARGE_CHARGE_" + i).data("kendoComboBox");
                }



            }
            ChargeComboBox.setDataSource(response.data.MESSAGE);
            if (i !== "") {
                ChargeComboBox.value($scope.charges[i].CHARGE_CODE);
            }
        });

    };

    var categoryUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetCategoryCode";

    $scope.categoryDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: categoryUrl,
            }
        }
    });

    $scope.categoryOptions = {
        serverFiltering: false,
        dataSource: $scope.categoryDataSource,
        dataTextField: "CATEGORY_EDESC",
        dataValueField: "CATEGORY_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    }

    $scope.onCategoryCodeChanged = function (e) {
        const selected = e.sender.dataItem(e.sender.select());
        if (selected) {
            $scope.selectedCategoryCode = selected.CATEGORY_CODE;
            $timeout(function () {
                $scope.itemArr.CATEGORY_CODE = $scope.selectedCategoryCode;
            }, 0)
        }
    };

    var chargeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetChargeCode";
    $scope.chargeDataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: chargeUrl,
            }
        }
    });
    $scope.chargeOptions = {
        dataSource: $scope.chargeDataSource,
        dataTextField: "CHARGE_EDESC",
        dataValueField: "CHARGE_CODE",
        filter: "contains",
        change: function (e) {
            var currentItem = e.sender.dataItem(e.node);
        },
        dataBound: function () {
        }
    }

    //#endregion "drop down or combo box region end"


    $scope.resetintegration = function () {
        var groupdocument = $("#int_document").data("kendoComboBox");
        if (groupdocument !== null || groupdocument !== undefined || groupdocument !== "") {
            groupdocument.value("");
        }
        var groupaccount = $("#int_accountcode").data("kendoComboBox");
        if (groupaccount !== null || groupaccount !== undefined || groupaccount !== "") {
            groupaccount.value("");
        }
    }
    $scope.refresh = function () {
        $scope.itemsetup.MULTI_MU_CODE = "";
        $scope.itemsetup.INDEX_MU_CODE = "";
        $scope.itemsetup.COSTING_METHOD_FLAG = "";
        $scope.updateparentcode = "";
        $scope.PARENT_ITEM_CODE = "";
        var groupItem = $("#itemGroupCode").data("kendoDropDownList");
        if (groupItem !== null || groupItem !== "" || groupItem !== undefined) {
            groupItem.value("");
        }

        //$scope.groupItem();
        $scope.reset();
        //var tree = $("#producttree").data("kendoTreeView");
        //tree.dataSource.read();
        $($("#myTab1").find("li a")[0]).trigger("click");
        $($("#myTab2").find("li a")[0]).trigger("click");

    };


    $scope.ItemCancel = function () {
        $scope.multiMuArr = [];
        $scope.itemArr.INDEX_MU_CODE = "";
        $scope.itemsetup.MULTI_MU_CODE = "";
        $scope.itemsetup.INDEX_MU_CODE = "";
        $scope.itemsetup.COSTING_METHOD_FLAG = "";
        $scope.updateparentcode = "";
        $scope.PARENT_ITEM_CODE = "";
        var groupItem = $("#itemGroupCode").data("kendoDropDownList");
        if (groupItem !== null || groupItem !== "" || groupItem !== undefined) {
            groupItem.value("");
        }

        //$scope.groupItem();
        $scope.reset();
        var tree = $("#producttree").data("kendoTreeView");
        tree.dataSource.read();
        $($("#myTab1").find("li a")[0]).trigger("click");
        $($("#myTab2").find("li a")[0]).trigger("click");

    };

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
            model: {
                id: "itemCode",
                parentId: "preItemCode",
                children: "Items",
                fields: {
                    ITEM_CODE: { field: "itemCode", type: "string" },
                    ITEM_EDESC: { field: "itemName", type: "string" },
                    parentId: { field: "preItemCode", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    function deepClone(obj) {
        return JSON.parse(JSON.stringify(obj));
    }

    function filterNodes(nodes, searchText, nodeProperties) {
        const result = [];

        angular.forEach(nodes, function (node) {
            const children = node.Items ? (node.Items.data ? node.Items.data() : node.Items) : [];
            const filteredChildren = filterNodes(children, searchText);

            let isMatch = false;
            let nodeText = "";
            let newNode = {};

            newNode = deepClone(node);


            if (newNode.ITEM_CODE && newNode.ITEM_EDESC) {
                var nameText = (newNode.ITEM_EDESC || "").toLowerCase();
                var codeText = (newNode.ITEM_CODE || "").toLowerCase();
                isMatch = nameText.includes(searchText) || codeText.includes(searchText);
            } else {
                newNode.ITEM_CODE = node.itemCode;
                newNode.ITEM_EDESC = node.itemName;
                var nameText2 = (newNode.itemName || "").toLowerCase();
                var codeText2 = (newNode.itemCode || "").toLowerCase();
                isMatch = nameText2.includes(searchText) || codeText2.includes(searchText);
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
            $scope.tree.setDataSource($scope.treeData);
            return;
        }

        const pristine = $scope.treeData._pristineData || $scope.treeData.data();
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

    // Trigger search on change (no search button), with debounce
    var searchDebouncePromise = null;
    $scope.$watch('txtMainGroupSearchString', function (newVal, oldVal) {
        if (newVal === oldVal) return;
        if (searchDebouncePromise) {
            $timeout.cancel(searchDebouncePromise);
        }
        searchDebouncePromise = $timeout(function () {
            $scope.filterTreeView();
        }, 250);
    });

    //treeview expand on startup
    $scope.itemonDataBound = function () {
        //$('#producttree').data("kendoTreeView").expand('.k-item');
    }

    $scope.getFirstMaxItemCode = function (gFlag) {
        var getItemCodeByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getMaxItemCode?gFlag=" + gFlag;
        $http({
            method: 'GET',
            url: getItemCodeByUrl,

        }).then(function successCallback(response) {

            //$scope.itemsetup.ITEM_CODE = response.data.DATA;
            $scope.itemArr.ITEM_CODE = response.data.DATA;
            //$scope.itemArr.GROUP_CODE = response.data.DATA
        }, function errorCallback(response) {
        });
    }


    $scope.fillAccSetupForms = function (accId) {
        var getitemdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getitemDetailsByItemCode?accCode=" + accId;
        $http({
            method: 'GET',
            url: getitemdetaisByUrl,

        }).then(function successCallback(response) {
            $scope.itemsetup = response.data.DATA;
            $scope.itemArr = $scope.itemsetup;
            $scope.PARENT_ITEM_CODE = $scope.itemsetup.PARENT_ITEM_CODE;
            var datas = response.data.DATA;

            if (datas.GROUP_SKU_FLAG == "G") {
                $scope.integrationArr = datas.assoModel.length <= 0 ? $scope.integrationArr : datas.assoModel;
                $scope.GroupMU.MU_CODE = datas.multiMu.length <= 0 ? $scope.GroupMU.MU_CODE : $scope.itemArr.multiMu[0].MU_CODE;
                $scope.charges = datas.charges.length <= 0 ? $scope.charges : datas.charges;


            }
            else if (datas.GROUP_SKU_FLAG == "I") {
                //$scope.inteArr = datas.assoModel.length <= 0 ? $scope.inteArr : datas.assoModel;
                //$scope.multiMuArr = datas.multiMu.length <= 0 ? $scope.multiMuArr : datas.multiMu;
                //$scope.itemArr.INDEX_MU = $scope.itemsetup.INDEX_MU_CODE;
            };
            $scope.itemSpecification = datas.specModel == null ? $scope.itemSpecification : datas.specModel;

            $scope.imageurledit = window.location.protocol + "//" + window.location.host + "/Areas/NeoERP.DocumentTemplate/images/item/" + $scope.itemArr.ITEM_CODE + "/" + response.data.DATA.IMAGE_FILE_NAME;
            if ($scope.editFlag === "Y") {
                $scope.AE = false;
                var childItemParent = $("#childmasteritemcode").data("kendoDropDownList");
                childItemParent.value($scope.treeselecteditemcode);
                var childIndexUnit = $("#childitemmucode").data("kendoDropDownList");
                childIndexUnit.value($scope.itemsetup.INDEX_MU_CODE);
                //$scope.Bind();
            };
            d1.resolve(response);


        }, function errorCallback(response) {
        });
    }

    $scope.setSelectedAttributes = function () {
        angular.forEach($scope.attributes, function (attribute) {
            let selectElementJS = document.getElementById(attribute.AttributeId + "MultiSelect");
            let selectElement = $(selectElementJS);
            var multiSelect = selectElement.data("kendoMultiSelect");

            if (multiSelect) {
                var allAvailableValues = [];
                var dataSource = multiSelect.dataSource;

                dataSource.data().forEach(function (item) {
                    allAvailableValues.push(item.ATTRIBUTE_CODE);
                });

                let valuesToSetAsSelected = [];
                if ($scope.itemArr && $scope.itemArr.specModel.ITEM_ATTRIBUTE_CODES) {
                    $scope.itemArr.specModel.ITEM_ATTRIBUTE_CODES.forEach(function (selectedCodes) {
                        if (allAvailableValues.includes(selectedCodes)) {
                            valuesToSetAsSelected.push(selectedCodes);
                        }
                    });
                }

                if (valuesToSetAsSelected.length > 0) {
                    multiSelect.value(valuesToSetAsSelected);
                } else {
                    multiSelect.value([]);
                }

                $scope.selectedAttributes[attribute.AttributeId] = multiSelect.value();
            }
        });

        if (!$scope.$$phase) {
            $scope.$apply();
        }
    }
    $scope.fillChildItemSetupForms = function (accId) {
        var getitemdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getitemDetailsByItemCode?accCode=" + accId;
        $http({
            method: 'GET',
            url: getitemdetaisByUrl,
        }).then(function successCallback(response) {
            $scope.itemsetup = response.data.DATA;
            $scope.itemArr = $scope.itemsetup;
            $scope.PARENT_ITEM_CODE = $scope.itemsetup.PARENT_ITEM_CODE;
            var datas = response.data.DATA;
            $scope.inteArr = datas.assoModel.length <= 0 ? $scope.inteArr : datas.assoModel;
            $scope.multiMuArr = datas.multiMu.length <= 0 ? $scope.multiMuArr : datas.multiMu;
            $scope.itemArr.MULTI_MU_CODE = $scope.itemsetup.MULTI_MU_CODE;
            $scope.itemArr.INDEX_MU_CODE = $scope.itemsetup.INDEX_MU_CODE;
            $scope.itemSpecification = datas.specModel == null ? $scope.itemSpecification : datas.specModel;
            $scope.parameterSpecification = $scope.itemsetup.parameterSpecificationModel;
            $scope.storageSetupModel = $scope.itemsetup.storageSetupModel;
            $scope.otherInfoModel = $scope.itemsetup.otherInfoModel;
            $scope.itemInfo = $scope.itemsetup.itemInfo;
            $scope.itemArr.specModel.SPECIFICATION_FLAG = "Y";

            $scope.setSelectedAttributes();
            $scope.charges = datas.charges.length <= 0 ? $scope.charges : datas.charges;
            if ($scope.editFlag === "Y") {
                $scope.AE = false;
                var ddl1 = $("#childmasteritemcode").data("kendoDropDownList");
                if (ddl1 != undefined)
                    ddl1.dataSource.read();
                var childItemParent = $("#childmasteritemcode").data("kendoDropDownList");
                childItemParent.value($scope.itemsetup.PARENT_ITEM_CODE);
                var childIndexUnit = $("#childitemmucode").data("kendoDropDownList");
                childIndexUnit.value($scope.itemsetup.INDEX_MU_CODE);
                d2.resolve(response);
            };

        }
            , function errorCallback(response) {
            }
        );
    }

    $scope.itemoptions = {
        loadOnDemand: false,
        select: function (e) {
            var currentItem = e.sender.dataItem(e.node);
            $('#itemGrid').removeClass("show-displaygrid");
            $("#itemGrid").html("");
            $($(this._current).parents('ul')[$(this._current).parents('ul').length - 1]).find('span').removeClass('hasTreeCustomMenu');
            $(this._current.context).find('span').addClass('hasTreeCustomMenu');
            $scope.itemArr = $scope.itemsetup;
            $scope.itemsetup.ITEM_CODE = currentItem.ITEM_CODE;
            //$scope.itemsetup.ITEM_EDESC = currentItem.ITEM_EDESC;
            $scope.itemsetup.MASTER_ITEM_CODE = currentItem.masterItemCode;
            $scope.treeselecteditemcode = currentItem.ITEM_CODE;
            $scope.treeselectedmastercode = currentItem.masterItemCode;
            $scope.editcode = $scope.itemsetup.MASTER_ITEM_CODE;
            $scope.edesc = $scope.itemsetup.ITEM_EDESC;
            $scope.treenodeselected = false;
            $scope.newrootinsertFlag = "N";

            //$scope.$apply();
            //$scope.movescrollbar();
        },

    };

    //$scope.movescrollbar = function () {
    //    var element = $(".k-in");
    //    for (var i = 0; i < element.length; i++) {
    //        var selectnode = $(element[i]).hasClass("k-state-focused");
    //        if (selectnode) {
    //            $("#producttree").animate({
    //                scrollTop: (parseInt(i))
    //            });
    //            break;
    //        }
    //    }
    //}

    function charge_charge_validation() {

        if ($scope.itemArr.GROUP_SKU_FLAG == "G") {
            // var tab_id = 1;
            //var tabname = "#myTab1";
            var tabname = '#myTab1 a[href="#tab_2"]';
        } else {
            //   var tab_id = 5
            //   var tabname = "#myTab2";
            var tabname = '#myTab2 a[href="#child_tab_6"]';
        }
        for (var i = 0; i < $scope.charges.length; i++) {


            if ($scope.charges[i].FORM_CODE !== "") {
                if ($scope.charges[i].CHARGE_CODE === null) {
                    //   $($(tabname).find("li a")[tab_id]).trigger("click");
                    displayPopupNotification("CHARGE CODE cannot be empty.", "warning");
                    $(tabname).tab('show');
                    $scope.IsChargeValidate = false;
                    return false;


                };

                if ($scope.charges[i].VALUE_QUANTITY_BASED === "") {

                    $(tabname).tab('show');
                    //$($(tabname).find("li a")[tab_id]).trigger("click");
                    displayPopupNotification("BASED ON  cannot be empty.", "warning");
                    $scope.IsChargeValidate = false;
                    return false;

                };
                if ($scope.charges[i].VALUE_PERCENT_FLAG === "") {
                    //$($(tabname).find("li a")[tab_id]).trigger("click");
                    $(tabname).tab('show');
                    displayPopupNotification("VALUE/PERCENTAGE  cannot be empty.", "warning");
                    $scope.IsChargeValidate = false;
                    return false;

                };
            }
            if ($scope.charges[i].FORM_CODE === "") {
                $scope.IsChargeValidate = true;
            }

        }
    }

    $scope.onContextSelect = function (event) {
        if ($scope.itemsetup.ITEM_CODE == "")
            //return displayPopupNotification("Select item.", "error");
            //$scope.itemsetup.ITEM_CODE = $scope.treeselecteditemcode;
            $scope.saveupdatebtn = "Save";
        if (event.item.innerText.trim() == "Delete") {
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

                        var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteitemsetupByItemcode?itemcode=" + $scope.itemsetup.ITEM_CODE;
                        $http({
                            method: 'POST',
                            url: delUrl
                        }).then(function successCallback(response) {

                            if (response.data.MESSAGE == "DELETED") {
                                $scope.itemArr = {};
                                $scope.itemsetup.MASTER_ITEM_CODE = "";
                                $scope.editcode = "";
                                $scope.edesc = "";

                                var tree = $("#producttree").data("kendoTreeView");
                                tree.dataSource.read();
                                $("#itemGroupModal").modal("hide");
                                $scope.refresh();

                                displayPopupNotification("Data succesfully deleted ", "success");
                            }
                            if (response.data.MESSAGE == "HAS_CHILD") {
                                $scope.itemArr = {};
                                $scope.itemsetup.MASTER_ITEM_CODE = "";
                                $scope.editcode = "";
                                $scope.edesc = "";

                                var tree = $("#producttree").data("kendoTreeView");
                                tree.dataSource.read();
                                $("#itemGroupModal").modal("hide");
                                $scope.refresh();
                                displayPopupNotification("Cannot Delete, Its has Child Item", "warning");
                            }

                        }, function errorCallback(response) {

                            var tree = $("#producttree").data("kendoTreeView");
                            tree.dataSource.read();
                            $scope.refresh();
                            displayPopupNotification(response.data.STATUS_CODE, "error");

                        });

                    }
                    else if (result == false) {

                        var tree = $("#producttree").data("kendoTreeView");
                        tree.dataSource.read();
                        $scope.refresh();
                        $("#itemGroupModal").modal("hide");
                    }

                }
            });
        }
        else if (event.item.innerText.trim() == "Update") {
            //$scope.resetintegration();
            //showloader();
            $($("#myTab1").find("li a")[0]).trigger("click");

            //$scope.inteArr = [];
            $scope.saveupdatebtn = "Update";
            $scope.editFlag = "Y";
            $scope.AE = false;
            d1 = $.Deferred();
            //$scope.itemArr.ITEM_CODE = $scope.itemsetup.GROUP_CODE;

            var multicode = $("#itemmucode").data("kendoComboBox");
            if (multicode != null || multicode != undefined || multicode !== "")
                multicode.value("");
            var multidesc = $("#itemmudescription").data("kendoComboBox");
            if (multidesc != null || multidesc != undefined || multidesc !== "")
                multidesc.value("");

            $scope.fillAccSetupForms($scope.treeselecteditemcode);
            $.when(d1).done(function () {
                $("#itemGroupModal").modal();
                var parentitemcode = $scope.PARENT_ITEM_CODE;
                var groupItem = $("#itemGroupCode").data("kendoDropDownList");
                if (groupItem != undefined && parentitemcode != undefined || parentitemcode !== "")
                    groupItem.value(parentitemcode);
                if ($scope.multiMuArr.length > 0) {
                    $("#multiMuArr").prop("checked", true);
                }
                hideloader();

            });
        }
        else if (event.item.innerText.trim() == "Add") {
            $($("#myTab1").find("li a")[0]).trigger("click");
            //$scope.refresh();
            $scope.savegroup = true;
            $scope.primitive_value = false;
            $scope.editFlag = "N";
            $scope.AE = true;
            //mk
            $scope.getDetailByItemCode($scope.itemsetup.ITEM_CODE);
            $scope.itemsetup.MASTER_ITEM_CODE = $scope.treeselectedmastercode;
            $scope.itemArr = {};
            $scope.itemArr.MASTER_ITEM_CODE = $scope.treeselectedmastercode;
            $scope.getFirstMaxItemCode("I");
            //$scope.reset();
            var groupItem = $("#itemGroupCode").data("kendoDropDownList");
            if (groupItem != undefined)
                groupItem.value($scope.treeselecteditemcode);
            $scope.itemsetup.GROUP_SKU_FLAG = "G";
            $scope.itemArr.GROUP_SKU_FLAG = "G";
            $("#itemGroupModal").modal();
        }


    }
    $scope.saveNewitem = function () {
        debugger
        var masteritemvalue = $scope.treeselectedmastercode;
        if ($scope.saveupdatebtn == "Save") {

            var createUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/createNewitem";
            $scope.charges;
            $scope.itemArr.specModel = $scope.itemSpecification;
            $scope.getSelectedAttributes()
            //if ($scope.itemArr.GROUP_SKU_FLAG == "G") {
            //    var tab_id = 1;
            //    var tabname="#myTab1";
            //}else
            //{
            //    var tab_id = 5
            //    var tabname = "#myTab2";
            //}
            //for (var i = 0; i < $scope.charges.length; i++) {
            //   

            //   if ($scope.charges[i].FORM_CODE !== null || $scope.charges[i].FORM_CODE !== "" || $scope.charges[i].FORM_CODE !== undefined) {
            //       if ($scope.charges[i].CHARGE_CODE === null || $scope.charges[i].CHARGE_CODE === "" || $scope.charges[i].CHARGE_CODE === undefined) {
            //           $($(tabname).find("li a")[tab_id]).trigger("click");
            //                return displayPopupNotification("CHARGE CODE cannot be empty.", "warning");
            //            };

            //            if ($scope.charges[i].VALUE_QUANTITY_BASED === null || $scope.charges[i].VALUE_QUANTITY_BASED === "" || $scope.charges[i].VALUE_QUANTITY_BASED === undefined) {
            //                $($(tabname).find("li a")[tab_id]).trigger("click");
            //                return displayPopupNotification("BASED ON  cannot be empty.", "warning");
            //            };
            //            if ($scope.charges[i].VALUE_PERCENT_FLAG === null || $scope.charges[i].VALUE_PERCENT_FLAG === "" || $scope.charges[i].VALUE_PERCENT_FLAG === undefined) {
            //                $($(tabname).find("li a")[tab_id]).trigger("click");
            //                return displayPopupNotification("VALUE/PERCENTAGE  cannot be empty.", "warning");
            //            };
            //        }
            //    }
            charge_charge_validation();
            if ($scope.IsChargeValidate === false) {
                $scope.IsChargeValidate === true;
                return;
            }
            if ($scope.itemArr.GROUP_SKU_FLAG == "G") {
                $scope.itemArr.assoModel = $scope.integrationArr;
                $scope.multiMuArray = [];
                if ($scope.GroupMU.MU_CODE !== "") {
                    $scope.multiMuArray.push({
                        MU_CODE: $scope.GroupMU.MU_CODE,
                        CONVERSION_FACTOR: "1",
                        FRACTION: "1",
                        REMARKS: ""
                    });
                }
                $scope.itemArr.multiMu = $scope.multiMuArray;
            }
            else {
                $scope.itemArr.assoModel = $scope.inteArr;
                $scope.itemArr.multiMu = $scope.multiMuArr;
            }
            $scope.itemArr.charges = $scope.charges;
            $scope.itemArr.MASTER_ITEM_CODE = masteritemvalue;
            $scope.itemArr.parameterSpecificationModel = $scope.parameterSpecification;
            $scope.itemArr.storageSetupModel = $scope.storageSetupModel;
            $scope.itemArr.otherInfoModel = $scope.otherInfoModel;
            $scope.itemArr.MIN_VALUE = parseFloat($scope.itemArr.MIN_VALUE);
            $scope.itemArr.MAX_VALUE = parseFloat($scope.itemArr.MAX_VALUE);


            console.log($scope.itemArr);

            document.uploadFile();
            if ($('#txtFile')[0].files[0] !== undefined) { $scope.itemArr.IMAGE_FILE_NAME = $('#txtFile')[0].files[0].name; }
            var model = { model: $scope.itemArr };

            $http({
                method: 'POST',
                url: createUrl,
                data: model
            }).then(function successCallback(response) {
                if (response.data.MESSAGE == "INSERTED") {

                    $scope.editcode = "";
                    $scope.edesc = "";
                    $scope.itemsetup.MASTER_ITEM_CODE = "";
                    $scope.multiMuArray = [];
                    var preserveditemcode = $scope.itemsetup.ITEM_CODE;
                    //$("#kGrid").data("kendoGrid").dataSource.read();
                    if ($("#categorycode").data("kendoDropDownList") != undefined) {
                        $("#categorycode").data("kendoDropDownList").dataSource.read();
                    }
                    if ($scope.itemsetup.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#producttree").data("kendoTreeView");
                        tree.dataSource.read();
                        $scope.treeselecteditemcode = "";
                        $scope.treeselectedmastercode = "";
                    }
                    $scope.refresh();
                    $scope.clearData();
                    $scope.itemArr.IMAGE_FILE_NAME = "";
                    var grid = $("#kGrid").data("kendo-grid");
                    if (grid != undefined) {
                        grid.dataSource.read();
                    }

                    var ddl = $("#itemGroupCode").data("kendoDropDownList");
                    if (ddl != undefined)
                        ddl.dataSource.read();

                    var ddl1 = $("#childmasteritemcode").data("kendoDropDownList");
                    if (ddl1 != undefined)
                        ddl1.dataSource.read();
                    $scope.itemsetup.INDEX_MU_CODE = "";
                    $scope.itemsetup.MULTI_MU_CODE = "";
                    $scope.itemsetup.ITEM_CODE = preserveditemcode;
                    if ($scope.savegroup == true) { $("#itemGroupModal").modal("toggle"); }
                    else { $("#itemModal").modal("toggle"); }

                    displayPopupNotification("Data succesfully saved ", "success");
                }
                else if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Item name already exist please try another item name.", "error");
                }
                else {
                    $scope.refresh();
                    $scope.multiMuArray = [];
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            }, function errorCallback(response) {
                if (response.data.MESSAGE == "INVALIDINSERTED") {
                    displayPopupNotification("Item name already exist please try another Item name.", "error");
                }
                else {
                    $scope.refresh();
                    $scope.multiMuArray = [];
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            });
        }
        else {
            document.uploadFile();
            if ($('#txtFile')[0].files[0] !== undefined) { $scope.itemArr.IMAGE_FILE_NAME = $('#txtFile')[0].files[0].name; }
            var updateUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/updateitemByItemCode";
            $scope.charges;

            $scope.itemArr.specModel = $scope.itemSpecification;
            $scope.getSelectedAttributes()
            //if ($scope.itemArr.GROUP_SKU_FLAG == "G") {
            //    var tab_id = 1;
            //    var tabname = "#myTab1";
            //} else {
            //    var tab_id = 5
            //    var tabname = "#myTab2";
            //}
            //for (var i = 0; i < $scope.charges.length; i++) {
            //   

            //    if ($scope.charges[i].FORM_CODE !== "") {
            //        if ($scope.charges[i].CHARGE_CODE === "") {
            //            $($(tabname).find("li a")[tab_id]).trigger("click");
            //            return displayPopupNotification("CHARGE CODE cannot be empty.", "warning");
            //        };

            //        if ($scope.charges[i].VALUE_QUANTITY_BASED === "") {
            //            $($(tabname).find("li a")[tab_id]).trigger("click");
            //            return displayPopupNotification("BASED ON  cannot be empty.", "warning");
            //        };
            //        if ($scope.charges[i].VALUE_PERCENT_FLAG === "") {
            //            $($(tabname).find("li a")[tab_id]).trigger("click");
            //            return displayPopupNotification("VALUE/PERCENTAGE  cannot be empty.", "warning");
            //        };
            //    }
            //}
            charge_charge_validation();
            if ($scope.IsChargeValidate === false) {
                $scope.IsChargeValidate === true;
                return;
            }

            if ($scope.itemArr.GROUP_SKU_FLAG == "G") {
                $scope.itemArr.assoModel = $scope.integrationArr;
                $scope.multiMuArray = [];

                $scope.multiMuArray.push({
                    MU_CODE: $scope.GroupMU.MU_CODE,
                    CONVERSION_FACTOR: "1",
                    FRACTION: "1",
                    REMARKS: ""
                });
                $scope.itemArr.multiMu = $scope.multiMuArray;
            }

            else {
                $scope.itemArr.assoModel = $scope.inteArr;
                $scope.itemArr.multiMu = $scope.multiMuArr;
            }
            $scope.itemArr.charges = $scope.charges;
            $scope.itemArr.parameterSpecificationModel = $scope.parameterSpecification;
            $scope.itemArr.storageSetupModel = $scope.storageSetupModel;
            $scope.itemArr.otherInfoModel = $scope.otherInfoModel;
            $scope.itemArr.MIN_VALUE = parseFloat($scope.itemArr.MIN_VALUE);
            $scope.itemArr.MAX_VALUE = parseFloat($scope.itemArr.MAX_VALUE);

            var model = { model: $scope.itemArr };

            $scope.saveupdatebtn = "Update";
            $http({
                method: 'POST',
                url: updateUrl,
                data: model
            }).then(function successCallback(response) {


                if (response.data.MESSAGE == "UPDATED") {

                    $scope.itemArr = {};
                    $scope.itemSpecification = [];
                    $scope.editcode = "";
                    $scope.edesc = "";
                    $scope.itemsetup.MASTER_ITEM_CODE = "";

                    $scope.itemsetup.INDEX_MU_CODE = "";
                    $scope.itemsetup.MULTI_MU_CODE = "";
                    $scope.itemsetup.COSTING_METHOD_FLAG = "";


                    if ($scope.itemsetup.GROUP_SKU_FLAG !== "I") {
                        var tree = $("#producttree").data("kendoTreeView");
                        tree.dataSource.read();
                        $scope.treeselecteditemcode = "";
                        $scope.treeselectedmastercode = "";
                    }



                    $scope.refresh();
                    $scope.clearData();
                    $scope.itemArr.IMAGE_FILE_NAME = "";
                    $("#kGrid").data("kendo-grid").dataSource.read();
                    var ddl = $("#itemGroupCode").data("kendoDropDownList");
                    if (ddl != undefined)
                        ddl.dataSource.read();

                    var ddl1 = $("#childmasteritemcode").data("kendoDropDownList");
                    if (ddl1 != undefined)
                        ddl1.dataSource.read();
                    $("#itemGroupModal").modal("hide");
                    $("#itemModal").modal("hide");
                    displayPopupNotification("Data succesfully updated ", "success");
                }
                if (response.data.MESSAGE == "ERROR") {
                    $scope.refresh();
                    displayPopupNotification("Something went wrong.Please try again later.", "error");
                }
            }, function errorCallback(response) {

                $scope.refresh();
                displayPopupNotification("Something went wrong.Please try again later.", "error");
            });
        }
    }


    $scope.MACDSOptions = {
        dataSource: $scope.MACDS,
        dataTextField: "text",
        dataValueField: "value",
    };

    $scope.masterAccCodeOptions = {
        dataSource: $scope.masterItemCodeDataSource,
        dataTextField: "text",
        dataValueField: "value",
    };
    $scope.clearData = function () {

        $scope.integrationArr = [{
            ACC_CODE: "",
            FORM_CODE: ""
        }];
        $scope.inteArr = [{
            ACC_CODE: "",
            FORM_CODE: ""
        }];
        $scope.multiMuArr = [{
            MU_CODE: "",
            CONVERSION_FACTOR: "",
            FRACTION: "",
            REMARKS: ""
        }];
        $scope.itemArr = $scope.itemsetup;
        $scope.itemSpecification = $scope.itemSpec;
        // Reset image preview and file input so previous image does not persist for new creations
        $scope.imageurledit = "";
        $timeout(function () {
            try {
                var $fileInput = $("#txtFile");
                if ($fileInput && $fileInput.length) { $fileInput.val(""); }
                var $img = $("#blah");
                if ($img && $img.length) { $img.attr('src', ''); }
                var $fi = $fileInput.closest('.fileinput');
                if ($fi && $fi.length) { $fi.removeClass('fileinput-exists').addClass('fileinput-new'); }
            } catch (e) { /* no-op */ }
        }, 0);
    }

    $scope.BindGrid = function (groupId) {
        $(".topsearch").show();
        var url = null;
        if (groupId == "All") {
            if ($('#itemtxtSearchString').val() == null || $('#itemtxtSearchString').val() == '' || $('#itemtxtSearchString').val() == undefined || $('#itemtxtSearchString').val() == 'undefined') {
                alert('Input is empty or undefined.');
                return;
            }
            debugger;
            var searchVal = $('#itemtxtSearchString').val()
            url = "/api/SetupApi/GetAllItemList?searchtext=" + encodeURIComponent(searchVal);
        }
        else {
            $("#itemtxtSearchString").val('');
            url = "/api/SetupApi/GetChildOfItemByGroup?groupId=" + groupId;
        }

        //grid Bind and sorting and paging Prem Prakash Dhakal
        var reportConfig = GetReportSetting("_itemSetupPartial");
        $scope.ItemChildGridOptions = {
            dataSource: {
                transport: {
                    read: {
                        url: url,
                        dataType: "json",
                        contentType: "application/json; charset=utf-8"
                    },
                    parameterMap: function (options, type) {
                        var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                        delete paramMap.$inlinecount;
                        delete paramMap.$format;
                        return paramMap;
                    }
                },
                error: function (e) {
                    displayPopupNotification("Sorry error occured while processing data", "error");
                },
                model: {
                    fields: {
                        ITEM_CODE: { type: "string" },
                        ITEM_EDESC: { type: "string" },
                        CATEGORY_EDESC: { type: "string" },
                        CREATED_BY: { type: "string" },
                        CREATED_DATE: { type: "string" },
                    }
                },
                /*pageSize: reportConfig.defaultPageSize,*/
                pageSize: 50,
            },
            height: 500,
            sortable: true,
            reorderable: true,
            scrollable: true,
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
                    SaveReportSetting('_itemSetupPartial', 'kGrid');
            },
            columnHide: function (e) {
                if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                    SaveReportSetting('_itemSetupPartial', 'kGrid');
            },
            pageable: {
                refresh: true,
                //pageSizes: reportConfig.itemPerPage,
                pageSize: 50,
                buttonCount: 5
            },
            pageable: true,
            dataBound: function (o) {
                var grid = o.sender;
                if (grid.dataSource.total() == 0) {
                    var colCount = grid.columns.length;
                    $(o.sender.wrapper)
                        .find('tbody')
                        .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, No data found</td></tr>');
                    /*displayPopupNotification("No Data Found.", "info");*/
                }
                else {
                    var g = $("#kGrid").data("kendoGrid");
                    for (var i = 0; i < g.columns.length; i++) {
                        g.showColumn(i);
                    }
                    $("div.k-group-indicator").each(function (i, v) {
                        g.hideColumn($(v).data("field"));
                    });
                }
                UpdateReportUsingSetting("_itemSetupPartial", "kGrid");
                $('div').removeClass('.k-header k-grid-toolbar');
            },
            columns: [
                {
                    //hidden: false,
                    field: "ITEM_CODE",
                    title: "ITEM CODE",
                    width: "120px"
                },
                {
                    field: "ITEM_EDESC",
                    title: "NAME",
                    width: "120px"
                },
                //{
                //    field: "MU_EDESC",
                //    title: "UNIT",
                //    width: "120px"
                //},
                {
                    field: "CATEGORY_EDESC",
                    title: "CATEGORY",
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
                    template: `
                                <div style="display: flex; gap: 10px; justify-content: center; align-items: center;">
                                  <a class="fa fa-pencil-square-o editAction"
                                     title="Edit"
                                     ng-click="editchilditem(dataItem.ITEM_CODE)"
                                     style="cursor: pointer;"
                                     aria-label="Edit Child Item">
                                  </a>

                                  <a class="fa fa-trash deleteAction"
                                     title="Delete"
                                     ng-click="delete(dataItem.ITEM_CODE)"
                                     style="cursor: pointer;"
                                     aria-label="Delete Child Item">
                                  </a>
                                </div>
                              `,
                    title: "Actions",
                    width: "50px"
                }

            ]
        };

        //$scope.ItemChildGridOptions = {

        //    //dataSource: {
        //    //    type: "json",
        //    //    transport: {
        //    //        read: url,
        //    //    },
        //    //    pageSize: 50,
        //    //    //serverPaging: true,
        //    //    serverSorting: true
        //    //},

        //    dataSource: {
        //        transport: {
        //            read: {
        //                url: url,
        //                dataType: "json",

        //            },
        //        },
        //        pageSize: 50,
        //        serverPaging: true,
        //        serverSorting: true,
        //    },

        //    //scrollable: true,
        //    //resizable: true,
        //    //height: 450,
        //    //sortable: true,
        //    //pageable: true,
        //    scrollable: true,
        //    resizable: true,
        //    height: 450,
        //    sortable: {
        //        mode: "single",
        //        allowUnsort: true
        //    },
        //    pageable: {
        //        pageSizes: [10, 25, 50, 100],
        //        buttonCount: 5,
        //        refresh: true
        //    },
        //    dataBound: function (e) {

        //        $("#kGrid tbody tr").css("cursor", "pointer");
        //        DisplayNoResultsFound($('#kGrid'));
        //        $("#kGrid tbody tr").on('dblclick', function () {
        //            var accCode = $(this).find('td span').html()
        //            $scope.editchilditem(accCode);
        //            var tree = $("#producttree").data("kendoTreeView");
        //            tree.dataSource.read();
        //        })
        //    },
        //    columns: [
        //        //{
        //        //    hidden: false,
        //        //    field: "ITEM_CODE",
        //        //    title: "ID",
        //        //    width: "120px"

        //        //},
        //        {
        //            field: "ITEM_EDESC",
        //            title: "NAME",
        //            width: "120px"
        //        },
        //        //{
        //        //    field: "MU_EDESC",
        //        //    title: "UNIT",
        //        //    width: "120px"
        //        //},
        //           {
        //               field: "CATEGORY_EDESC",
        //               title: "CATEGORY",
        //               width: "120px"
        //           },


        //        {
        //            field: "CREATED_BY",
        //            title: "CREATED BY",
        //            width: "120px"
        //        }, {
        //            field: "CREATED_DATE",
        //            title: "CREATED DATE",
        //            template: "#= kendo.toString(kendo.parseDate(CREATED_DATE),'dd MMM yyyy') #",
        //            width: "120px"
        //        },
        //        {
        //            template: '<a class="fa fa-pencil-square-o editAction" title="Edit" ng-click="editchilditem(dataItem.ITEM_CODE)"><span class="sr-only"></span> </a><a class="fa fa-trash deleteAction" title="delete" ng-click="delete(dataItem.ITEM_CODE)"><span class="sr-only"></span> </a>',
        //            title: " ",
        //            width: "45px"
        //        }
        //    ],
        //};

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

        $timeout(function () {
            var $input = $("#itemtxtSearchString");
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
    }

    $scope.reset = function () {

        $scope.updateparentcode = "";
        $scope.integrationArr = [{ ACC_CODE: "", FORM_CODE: "" }];
        $scope.inteArr = [{ ACC_CODE: "", FORM_CODE: "" }];
        $scope.multiMuArr = [{ MU_CODE: "", CONVERSION_FACTOR: "", FRACTION: "", REMARKS: "" }];
        $scope.itemSpecification = {
            ITEM_CODE: "",
            PART_NUMBER: "",
            BRAND_NAME: "",
            ITEM_SPECIFICATION: "",
            ITEM_APPLY_ON: "",
            INTERFACE: "",
            TYPE: "",
            LAMINATION: "",
            ITEM_SIZE: "",
            THICKNESS: "",
            COLOR: "",
            GRADE: "",
            REMARKS: "",
            SYN_ROWID: "",
            GSM: "",
            SIZE_LENGHT: "",
            SIZE_WIDTH: "",
            SPEC_COMPULSORY_FLAG: "",
            ITEM_ATTRIBUTE_CODES: []
        };
        //$scope.multiMuArray = [];
        //$scope.treeselecteditemcode = "";
        //$scope.treeselectedmastercode = "";
        $scope.GroupMU = { MU_CODE: "", MU_DESC: "" };
        $($("#myTab1").find("li a")[0]).trigger("click");
        $($("#myTab2").find("li a")[0]).trigger("click");
        // Also clear image preview and file input on full reset
        $scope.imageurledit = "";
        $timeout(function () {
            try {
                var $fileInput = $("#txtFile");
                if ($fileInput && $fileInput.length) { $fileInput.val(""); }
                var $img = $("#blah");
                if ($img && $img.length) { $img.attr('src', ''); }
                var $fi = $fileInput.closest('.fileinput');
                if ($fi && $fi.length) { $fi.removeClass('fileinput-exists').addClass('fileinput-new'); }
            } catch (e) { /* no-op */ }
        }, 0);
    }

    $scope.clearNewFields = function () {

        $scope.itemArr.AVG_RATE = "";
        $scope.itemArr.BATCH_FLAG = "N";
        $scope.itemArr.BATCH_SERIAL_FLAG = "N";
        $scope.itemArr.NON_VAT_FLAG = "N";
        $scope.itemArr.FREEZE_FLAG = "N";
        $scope.itemArr.BRANCH_CODE = "";
        $scope.itemArr.ITEM_CODE = "";
        $scope.itemArr.ITEM_EDESC = "";
        $scope.itemArr.ITEM_NDESC = "";
        $scope.itemArr.LEAD_TIME = "";
        $scope.itemArr.LINK_SUB_CODE = "";
        $scope.itemArr.MASTER_ITEM_CODE = "";
        $scope.itemArr.PRE_ITEM_CODE = "";

        $scope.itemArr.MAX_USAGE = "";
        $scope.itemArr.MAX_VALUE = "";
        $scope.itemArr.MIN_LEVEL = "";
        $scope.itemArr.MIN_USAGE = "";
        $scope.itemArr.MIN_VALUE = "";
        $scope.itemArr.SERVICE_ITEM_FLAG = "N";
        $scope.itemArr.SHELF_LIFE_DAYS = "";
        $scope.itemArr.SYN_ROWID = "";
        $scope.itemArr.VALUATION_FLAG = ""
        $scope.itemArr.REMARKS = ""
        $scope.itemArr.REMARKS2ND = "";
        $scope.itemArr.HS_CODE = "";
        $scope.itemArr.RACK_LOCATION = "";
        // Clear image related fields for new item creation to avoid showing last image
        $scope.imageurledit = "";
        $timeout(function () {
            try {
                var $fileInput = $("#txtFile");
                if ($fileInput && $fileInput.length) { $fileInput.val(""); }
                var $img = $("#blah");
                if ($img && $img.length) { $img.attr('src', ''); }
                var $fi = $fileInput.closest('.fileinput');
                if ($fi && $fi.length) { $fi.removeClass('fileinput-exists').addClass('fileinput-new'); }
            } catch (e) { /* no-op */ }
        }, 0);
    }

    $scope.showModalForNew = function (event) {
        $($("#myTab2").find("li a")[0]).trigger("click");
        $scope.editFlag = "N";
        $scope.savegroup = false;
        $scope.saveupdatebtn = "Save"
        $scope.groupItemTypeFlag = "N";
        $scope.getFirstMaxItemCode("I");
        //mk
        $scope.getDetailByItemCode($scope.treeselecteditemcode);
        $scope.getDetailIntigration($scope.treeselecteditemcode);

        $scope.itemArr = $scope.itemsetup;
        var ddl1 = $("#childmasteritemcode").data("kendoDropDownList");
        if (ddl1 != undefined)
            ddl1.dataSource.read();
        var childItemParent = $("#childmasteritemcode").data("kendoDropDownList");
        childItemParent.value($scope.treeselecteditemcode);

        $scope.itemArr = {};
        $scope.itemsetup.GROUP_SKU_FLAG = "I";
        $scope.itemArr.GROUP_SKU_FLAG = "I";
        $('#masterlocationcode').val(' ');

        //$scope.itemArr.MASTER_ITEM_CODE = $scope.itemSetup.MASTER_ITEM_CODE;
        //$scope.itemArr.PRE_ITEM_CODE = $scope.itemSetup.PRE_ITEM_CODE;

        $("#itemModal").modal("toggle");
        //$('.col-sm-6 col-md-3 col-lg-3').find('fileinput fileinput-exists').removeClass('fileinput fileinput-exists').addClass('fileinput fileinput-new');

    }

    //mk 
    $scope.getDetailByItemCode = function (accId) {

        var getitemdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getitemDetailsByItemCode?accCode=" + accId;
        $http({
            method: 'GET',
            url: getitemdetaisByUrl,

        }).then(function successCallback(response) {
            var itemData = response.data.DATA;
            $scope.itemArr.CATEGORY_CODE = itemData.CATEGORY_CODE;
            $scope.itemArr.MASTER_ITEM_CODE = itemData.MASTER_ITEM_CODE;
            $scope.itemArr.PRE_ITEM_CODE = itemData.PRE_ITEM_CODE;
            $scope.itemArr.INDEX_MU_CODE = itemData.INDEX_MU_CODE;
            $scope.onChangeInitialIndexUnitType();
            $scope.groupItemTypeFlag = "N";
            //$scope.itemsetup.GROUP_SKU_FLAG = "I";
            //$scope.itemArr.GROUP_SKU_FLAG = "I";
            //$scope.itemArr.CATEGORY_CODE = $scope.itemsetup.CATEGORY_CODE;
            //$scope.itemArr.MASTER_ITEM_CODE = $scope.itemsetup.MASTER_ITEM_CODE;
            //$scope.itemArr.PRE_ITEM_CODE = $scope.itemsetup.PRE_ITEM_CODE;

        }, function errorCallback(response) {

        });
    }
    //mk
    $scope.getDetailIntigration = function (accId) {
        var getIntegrationdetaisByUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getitemDetailsByItemCode?accCode=" + accId;
        $http({
            method: 'GET',
            url: getIntegrationdetaisByUrl,

        }).then(function successCallback(response) {
            var datas = response.data.DATA;
            //subin edit
            //$scope.itemArr.INDEX_MU_CODE = datas.INDEX_MU_CODE;
            /*$scope.multiMuArr = datas.multiMu;*/
            $scope.inteArr = datas.assoModel.length <= 0 ? $scope.inteArr : datas.assoModel;
            $scope.attributes.forEach(function (attribute) {
                let selectElementJS = document.getElementById(attribute.AttributeId + "MultiSelect");
                $(selectElementJS).data("kendoMultiSelect").value([])
            })
            $scope.parameterSpecification = [];
            $scope.storageSetupModel = [];
            $scope.otherInfoModel = [];
            $scope.itemArr.specModel = {};
            $scope.itemArr.specModel.SPECIFICATION_FLAG = "Y";
            $scope.itemInfo = []

        }, function errorCallback(response) {

        });
    }

    $scope.editchilditem = function (accCode) {
        //showloader();
        $($("#myTab2").find("li a")[0]).trigger("click");
        $scope.editFlag = "Y";
        $scope.savegroup = false;
        $scope.saveupdatebtn = "Update";
        d2 = $.Deferred();
        $scope.fillChildItemSetupForms(accCode);
        $.when(d2).done(function () {

            //var cddl = $("#categorycode").data("kendoDropDownList");
            //cddl.value($scope.itemsetup.CATEGORY_CODE);
            var cddl = $("#categorycode").data("kendoDropDownList");
            if (cddl != undefined) {
                cddl.value($scope.itemsetup.CATEGORY_CODE);
            }

            //$scope.itemArr.GROUP_CODE = $scope.itemArr.ITEM_CODE;
            $scope.groupItemTypeFlag = "N";
            $scope.itemsetup.GROUP_SKU_FLAG = "I";
            $scope.itemArr.GROUP_SKU_FLAG = "I";
            //$("#categorycode").data("kendoDropDownList").dataSource.read();
            $("#itemModal").modal("toggle");
            //hideloader();
        });


    };

    $scope.delete = function (code) {
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

                    var delUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/DeleteitemsetupByItemcode?itemcode=" + code;
                    $http({
                        method: 'POST',
                        url: delUrl
                    }).then(function successCallback(response) {

                        if (response.data.MESSAGE == "DELETED") {
                            $scope.itemArr = {};
                            $scope.itemsetup.MASTER_ITEM_CODE = "";
                            $scope.editcode = "";
                            $scope.edesc = "";
                            $("#itemGroupModal").modal("hide");
                            $scope.refresh();
                            $("#kGrid").data("kendoGrid").dataSource.read();
                            displayPopupNotification("Data succesfully deleted ", "success");
                        }
                        if (response.data.MESSAGE == "HAS_CHILD") {
                            $scope.itemArr = {};
                            $scope.itemsetup.MASTER_ITEM_CODE = "";
                            $scope.editcode = "";
                            $scope.edesc = "";
                            $("#itemGroupModal").modal("hide");
                            $scope.refresh();
                            displayPopupNotification("Cannot Delete, Its has Child Item", "warning");
                        }

                    }, function errorCallback(response) {
                        $scope.refresh();
                        displayPopupNotification(response.data.STATUS_CODE, "error");
                    });

                }
                else if (result == false) {
                    $scope.refresh();
                    $("#itemGroupModal").modal("hide");
                }

            }
        });
    }

    $scope.addNewItem = function () {
        $scope.editFlag = "N";
        $scope.reset();
        $scope.itemsetup.MASTER_ITEM_CODE = "";
        $scope.itemArr = {};
        $scope.savegroup = true;
        $scope.saveupdatebtn = "Save";
        $scope.groupItemTypeFlag = "Y";
        $scope.itemsetup.GROUP_SKU_FLAG = "G";
        $scope.itemArr.GROUP_SKU_FLAG = "G";
        $scope.getFirstMaxItemCode("G");
        $scope.treeselectedmastercode = "";
        $scope.primitive_value = false;
        $($("#myTab1").find("li a")[0]).trigger("click");
        $scope.initchargeradio();
        $("#itemGroupModal").modal("toggle");

    }

    //charge
    $scope.add_charges = function (chargeItems) {

        if ($scope.chargedocumentcount === true) {
            displayPopupNotification("Same code cannot be selected", "warning");
            e.preventDefault();
            e.stopPropagation();
            return false;
        }
        var arr = $scope.CHARGE_INDEX_MU_CODE;

        if (arr === "") {


            $scope.charges.push({
                FORM_CODE: "",
                CHARGE_CODE: "",
                CHARGE_TYPE: "",
                VALUE_QUANTITY_BASED: "",
                VALUE_PERCENT_FLAG: "",
                VALUE_PERCENT_AMOUNT: "",
                CHARGE_INDEX_UNIT: "",
                ACC_CODE: "",
                SUB_CODE: "",
                IMPACT_ON: "",
                APPLY_QUANTITY: "",
                CHARGE_INDEX_UNIT: ""



                //CHARGE_DOCUMENT: "",
                //CHARGE: "",
                //VALUE_PERCENT: "",
                //VALUE_RATE: "",
                //TAKE_QUANTITY: "",
                //IMPACT_ON: "",
                //BASED_ON: "",
                //ACC_CODE: "",
                //SUB_CODE: ""
            });
        }
        if (arr !== "") {


            $scope.charges.push({
                FORM_CODE: "",
                CHARGE_CODE: "",
                CHARGE_TYPE: "",
                VALUE_QUANTITY_BASED: "",
                VALUE_PERCENT_FLAG: "",
                VALUE_PERCENT_AMOUNT: "",
                CHARGE_INDEX_UNIT: "",
                ACC_CODE: "",
                SUB_CODE: "",
                IMPACT_ON: "",
                APPLY_QUANTITY: "",
                CHARGE_INDEX_UNIT: arr[0].MU_EDESC
            });

        }








    }

    $scope.remove_charges = function ($index) {
        if ($scope.charges.length > 1) {
            $scope.charges.splice($index, 1);
            $scope.chargedocumentcount = false;

        }

        var count = 0;
        for (var i = 0; i < $scope.charges.length; i++) {
            if ($scope.charges[i].FORM_CODE !== "") {
                count = 1;
                break;
            }
            if (count == 0) {
                $scope.IsChargeValidate === true;
            }
        };
    }

    $scope.newParentCharge = function () {

        $scope.parentchangenew = false;
        $scope.chargehideshow = false;
    }

    $scope.removeParentCharge = function () {



    };

    $scope.muCodeOnChange = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.mucodeerror = "Please Enter Valid Code."
            $('#itemMuCode').data("kendoComboBox").value([]);
            $scope.itemArr.INDEX_MU_CODE = "";
            //$(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {

            $scope.mucodeerror = "";
            //$(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }




    $scope.ApplyParentCharge = function () {

        $scope.parentchangenew = true;
        $scope.chargehideshow = true;
        var groupchargedocument = $("#ChargeDocument").data("kendoComboBox");
        groupchargedocument.value("");
        var groupchargecharge = $("#ChargeCharge").data("kendoComboBox");
        groupchargecharge.value("");
        var groupchargeaccount = $("#accounts").data("kendoComboBox");
        groupchargeaccount.value("");
        var groupchargesubledger = $("#subledger").data("kendoComboBox");
        groupchargesubledger.value("");


    }


    $scope.CancelParentCharge = function () {

        $scope.parentchangenew = true;
        $scope.chargehideshow = true;
        var groupchargedocument = $("#ChargeDocument").data("kendoComboBox");
        groupchargedocument.value("");
        var groupchargecharge = $("#ChargeCharge").data("kendoComboBox");
        groupchargecharge.value("");
        var groupchargeaccount = $("#accounts").data("kendoComboBox");
        groupchargeaccount.value("");
        var groupchargesubledger = $("#subledger").data("kendoComboBox");
        groupchargesubledger.value("");

    }

    //charge 
    $scope.add_multi_mu_conversion = function (index, muItem) {

        $scope.multiMuArr.push({
            MU_CODE: "",
            CONVERSION_FACTOR: "",
            FRACTION: "",
            REMARKS: ""
        })
    }

    $scope.remove_mu_conversion = function ($index) {
        if ($scope.multiMuArr.length > 1) {
            $scope.multiMuArr.splice($index, 1);
        }
    }
    $scope.add_child_integration = function (index, inteItem) {

        $scope.inteArr.push({
            ACC_CODE: "",
            FORM_CODE: ""
        });
    }
    $scope.remove_child_integration = function ($index) {
        if ($scope.inteArr.length > 1) {
            $scope.inteArr.splice($index, 1);
        }
    }
    $scope.add_group_integration = function (index, inteItem) {

        $scope.integrationArr.push({
            ACC_CODE: "",
            FORM_CODE: ""
        });
        $scope.primitive_value = true;
    };

    $scope.remove_group_integration = function ($index) {
        if ($scope.integrationArr.length > 1) {
            $scope.integrationArr.splice($index, 1);
        }
    }

    $scope.tabcharge = function () {

        var formcode = "";
        var accountcode = "";
        var len = $scope.charges.length;
        for (var i = 0; i < len; i++) {
            formcode = $scope.charges[i].FORM_CODE;
            accountcode = $scope.charges[i].ACC_CODE;
            if (formcode !== "") {
                $scope.GetChargeByFormCode(formcode, i);
            }
            if (accountcode !== "") {
                $scope.GetSubCodeByAccountCode(accountcode, i);
            }


        }
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
            //Prem Prakash Dhakal
            //for (var i = 0; i < addRows; i++) {
            //    grid.find('.k-grid-content tbody').append('<tr class="kendo-data-row"><td>&nbsp;</td></tr>');
            //}
        }
    };

    $scope.changeOnCalculation = function (kendoEvent) {


        if ($scope.charges.push.VALUE_QUANTITY_BASED == "Q") {
            $scope.charges.push.VALUE_PERCENT_FLAG = "P";
        }
    }



    //Filter data in the Item    
    //$scope.filterTreeView = function () {
    //    const searchText = ($scope.txtMainGroupSearchString || '').toLowerCase();
    //    if (!searchText) {
    //        $scope.tree.setDataSource($scope.treeData);
    //        return;
    //    }
    //    const pristine = $scope.treeData._pristineData || $scope.treeData.data();
    //    const filteredData = filterNodes(pristine, searchText);
    //    const filteredDataSource = new kendo.data.HierarchicalDataSource({
    //        data: filteredData,
    //        schema: {
    //            model: {
    //                id: "itemCode",
    //                children: "Items"
    //            }
    //        }
    //    });
    //    $scope.tree.setDataSource(filteredDataSource);
    //    setTimeout(() => {
    //        $scope.tree.expand(".k-item");
    //    }, 100);
    //};
    //function filterNodes(nodes, searchText, nodeProperties) {
    //    const result = [];
    //    angular.forEach(nodes, function (node) {
    //        const children = node.Items ? (node.Items.data ? node.Items.data() : node.Items) : [];
    //        const filteredChildren = filterNodes(children, searchText);
    //        let isMatch = false;
    //        let nodeText = "";
    //        let newNode = {};
    //        newNode = deepClone(node);
    //        if (newNode.ITEM_CODE && newNode.ITEM_EDESC) {
    //            nodeText = (newNode.ITEM_EDESC || "").toLowerCase();
    //            isMatch = nodeText.includes(searchText);
    //        } else {url
    //            newNode.ITEM_CODE = node.itemCode;
    //            newNode.ITEM_EDESC = node.itemName;
    //            nodeText = (newNode.itemName || "").toLowerCase();
    //            isMatch = nodeText.includes(searchText);
    //        }
    //        if (isMatch || filteredChildren.length > 0) {
    //            newNode.Items = filteredChildren;
    //            result.push(newNode);
    //        }
    //    });
    //    return result;
    //}

    $scope.add_child_element = function (index) {
        $scope.charges.splice(index + 1, 0, {
            //CHARGE_DOCUMENT: "",
            //CHARGE: "",
            //VALUE_PERCENT: "",
            //VALUE_RATE: "",
            //TAKE_QUANTITY: "",
            //IMPACT_ON: "",
            //BASED_ON: "",
            //ACC_CODE: "",
            //SUB_CODE: ""
            FORM_CODE: "",
            CHARGE_CODE: "",
            CHARGE_TYPE: "",
            VALUE_QUANTITY_BASED: "",
            VALUE_PERCENT_FLAG: "",
            VALUE_PERCENT_AMOUNT: "",
            CHARGE_INDEX_UNIT: "",
            ACC_CODE: "",
            SUB_CODE: "",
            IMPACT_ON: "",
            APPLY_QUANTITY: "",
            CHARGE_INDEX_UNIT: ""

        })
    }

    $scope.remove_child_element = function (index) {
        $scope.charges.splice(index, 1);
    }

    $scope.add_parameter_spec_row = function (index) {
        $scope.parameterSpecification.splice(index + 1, 0, {
            PARAMETER_ENAME: "",
            PARAMETER_NNAME: "",
            IDEAL_VALUE: "",
            MIN_VALUE: "",
            MAX_VALUE: "",
            REMARKS: ""
        });
    }

    $scope.remove_parameter_spec_row = function (index) {
        $scope.parameterSpecification.splice(index, 1);
    }

    $scope.add_qc_parameter_row = function (index) {
        $scope.qcParameter.splice(index + 1, 0, {
            PARAMETER_NAME: "",
            PERCENTAGE: "",
            REMARKS: ""
        });
    }

    $scope.remove_qc_parameter_row = function (index) {
        $scope.qcParameter.splice(index, 1);
    }

    $scope.add_storage_setup = function (index) {
        $scope.storageSetupModel.splice(index + 1, 0, {
            LOCATION_CODE: "",
            STORAGE_CAPACITY: "",
            REMARKS: "",
        });
    }

    $scope.remove_storage_setup = function (index) {
        $scope.storageSetupModel.splice(index, 1);
    }

    $scope.add_other_info = function (index) {
        $scope.otherInfoModel.splice(index + 1, 0, {
            FIELD_NAME: "",
            FIELD_VALUE: "",
            REMARKS: "",
        });
    }

    $scope.remove_other_info = function (index) {
        $scope.otherInfoModel.splice(index, 1);
    }

    $scope.attributes = [];
    $scope.selectedAttributes = {};

    $scope.getAttributesList = function () {
        var getAttributeUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/GetTreeAttribute";
        $http.get(getAttributeUrl)
            .then(function (response) {
                $scope.attributes = response.data;
                $timeout(function () {
                    $scope.initializeKendoMultiSelectsManual();
                });
            })
            .catch(function (error) {
                console.error("Error fetching attributes list:", error);
            });
    };

    $scope.initializeKendoMultiSelectsManual = function () {
        var getAttributeChildUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/GetChildOfAttributeByGroup";

        angular.forEach($scope.attributes, function (attribute) {
            var selectElementJS = document.getElementById(attribute.AttributeId + "MultiSelect");
            var selectElement = $(selectElementJS)

            if (selectElement.data("kendoMultiSelect")) {
                selectElement.data("kendoMultiSelect").destroy();
            }

            selectElement.empty();

            $http.get(getAttributeChildUrl + "?groupId=" + attribute.AttributeId)
                .then(function (childResponse) {
                    var childOptions = childResponse.data;

                    angular.forEach(childOptions, function (optionData) {

                        var optionValue = optionData.ATTRIBUTE_CODE;
                        var optionText = optionData.ATTRIBUTE_EDESC;

                        selectElement.append($("<option>", {
                            value: optionValue,
                            text: optionText
                        }));
                    });

                    var kendoOptions = {
                        dataTextField: "ATTRIBUTE_EDESC",
                        dataValueField: "ATTRIBUTE_CODE",
                        autoClose: false,
                        change: function () {
                            $scope.selectedAttributes[attribute.AttributeId] = this.value();
                            if (!$scope.$$phase) {
                                $scope.$apply();
                            }
                        }
                    };
                    selectElement.kendoMultiSelect(kendoOptions);

                })
                .catch(function (error) {
                    console.error("Error fetching child attributes for groupId " + attribute.preAttributeCode + ":", error);
                });
        });
    };

    $scope.getAttributesList();

    $scope.getSelectedAttributes = function () {
        $scope.itemArr.specModel.ITEM_ATTRIBUTE_CODES = [];

        angular.forEach($scope.attributes, function (attribute) {
            let selectElementJS = document.getElementById(attribute.AttributeId + "MultiSelect");
            let selectElement = $(selectElementJS);
            var multiSelect = selectElement.data("kendoMultiSelect");

            if (multiSelect) {
                var currentlySelectedValues = multiSelect.value();

                if (currentlySelectedValues && currentlySelectedValues.length > 0) {
                    $scope.itemArr.specModel.ITEM_ATTRIBUTE_CODES = $scope.itemArr.specModel.ITEM_ATTRIBUTE_CODES.concat(currentlySelectedValues);
                }
            }
        });

        if (!$scope.$$phase) {
            $scope.$apply();
        }
    };

    var locationUrl = window.location.protocol + "//" + window.location.host + "/api/SetupApi/getAllLocation";
    $scope.locationComboDataSource = new kendo.data.DataSource({
        serverFiltering: true,
        transport: {
            read: {
                url: locationUrl,
            },
            parameterMap: function (data, action) {
                var newParams;
                if (data.filter != undefined) {
                    if (data.filter.filters[0] != undefined) {
                        newParams = {
                            filter: data.filter.filters[0].value
                        };
                        return newParams;
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
    });

    $scope.locationCodeOnChange = function (kendoEvent) {
        var Code = kendoEvent.sender.dataItem().LOCATION_CODE;
        var index = this.$index;
        var sublen = $scope.alternativeLocationInfoList.length;

        for (var j = 0; j < sublen; j++) {
            var subcode = $scope.alternativeLocationInfoList[j].LOCATION_CODE;
            if (index != j) {
                if (subcode === Code) {

                    $($(".locationcode_" + index)[0]).addClass("borderRed");
                    $("#savedocumentformdatachild").prop("disabled", true);
                    $scope.locationcount = true;
                    return;
                }
                else {

                    $($(".locationcode_" + index)[0]).removeClass("borderRed");
                    $scope.locationcount = false;
                    $("#savedocumentformdatachild").prop("disabled", false);
                };

            }
        }
    }
});

