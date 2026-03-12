QCQAModule.controller('IncomingMaterialList', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    if ($routeParams.voucherno != undefined) {
        $scope.voucherno = $routeParams.voucherno;//.split(new RegExp('_', 'i')).join('/');

    }
    else { $scope.voucherno = "undefined"; }

    $("#ddlProductType").kendoDropDownList({
        optionLabel: "Select Product Type...",
        dataTextField: "PRODUCT_TYPE",   // <- replace with actual property
        dataValueField: "PRODUCT_TYPE",    // <- replace with actual property
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/ProductSetupAPI/ProductTypeList",
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            var selectedVal = this.value();//$(this).val(); //$(this).val();//this.value();
            SelectProductByProductType(selectedVal);
        },
        dataBound: function (e) {
        }
    }).data("kendoDropDownList");

    function SelectProductByProductType(productType, callback) {
        $http.get("/api/ProductSetupAPI/ProductList?ProductType=" + productType + "&Category_Code=" + "RM")
            .then(function (response) {
                var items = response.data;
                // Destroy existing KendoDropDownList if already created
                var ddl = $("#ddlMaterials").data("kendoDropDownList");
                if (ddl) {
                    ddl.destroy();
                    $("#ddlMaterials").empty();
                }
                // Create new Kendo DropDownList
                $("#ddlMaterials").kendoDropDownList({
                    dataSource: items,
                    dataTextField: "ITEM_EDESC",
                    dataValueField: "ITEM_CODE",
                    optionLabel: "-- Select Material --",
                    autoClose: true
                });

                // Callback to send items back
                if (callback) callback(items);
            });
    }

    function MultiSelectProduct(productType, selectedItemCodes = []) {
        var selectElement = $("#productList");

        if (selectElement.data("kendoMultiSelect")) {
            selectElement.data("kendoMultiSelect").destroy();
            selectElement.closest(".k-widget").find('.k-multiselect-wrap').remove();
        }
        selectElement.empty();

        $http.get("/api/ProductSetupAPI/ProductList?ProductType=" + productType)
            .then(function (response) {
                var items = response.data;

                // Destroy existing multiselect if already created
                var selectElement = $("#productList");
                if (selectElement.data("kendoMultiSelect")) {
                    selectElement.data("kendoMultiSelect").destroy();
                    selectElement.empty();
                }

                // Initialize MultiSelect with proper datasource
                var multi = selectElement.kendoMultiSelect({
                    dataSource: items,
                    dataTextField: "ITEM_EDESC",
                    dataValueField: "ITEM_CODE",
                    autoClose: false
                }).data("kendoMultiSelect");
                if (selectedItemCodes) {
                    if (typeof selectedItemCodes === "string") {
                        selectedItemCodes = selectedItemCodes.split(",");
                    }
                    selectedItemCodes = selectedItemCodes.map(function (val) {
                        return val.trim();
                    });
                    multi.value(selectedItemCodes);
                }
            });
    }

    $scope.saveQCParameter = function () {
        if ($("#ddlProductType").val() == null || $("#ddlProductType").val() == "" || $("#ddlProductType").val() == undefined) {
            displayPopupNotification("Product Type is required", "warning");
            return false;
        }
        if ($("#ddlMaterials").val() == null || $("#ddlMaterials").val() == "" || $("#ddlMaterials").val() == undefined) {
            displayPopupNotification("Material is required", "warning");
            return false;
        }
        if ($('#txtQCNo').val() == null || $('#txtQCNo').val() == "" || $('#txtQCNo').val() == 'undefined') {
            displayPopupNotification("Voucher No is required", "warning");
            return false;
        }
       
        var qcDetailList = {

            ITEM_CODE: $('#ddlProductType').val(),
            MATERIAL_CODE: $("#ddlMaterials").val(),
            VOUCHER_NO: $("#idGRNNo").val(),
            TRANSACTION_NO: $scope.voucherno,
            MANUAL_NO: $("#txtManualNo").val(),
            GRN_DATE: $scope.GRN_DATE,
            Remarks: $("#txtRemarks").val(),
            QC_CODE: $('#txtQCNo').val(),
            QC_DATE: $('#iQC_DATE').val(),
            /* VENDOR_NAME: $('#txtVendorName').val(),*/
            VENDOR_NAME: $('#hidtxtVendorCode').val(),
            LC_NO: $('#hidtxtVendorCode').val(),
            /*LC_NO: $('#txtLCNo').val(),*/
            INVOICE_NO: $('#txtInvoiceNo').val(),
            NAME: $('#txtMaterialName').val(),
            GRN_DATE: $('#idReceiptDate').val(),
            QUANTITY: $('#txtReceivedQty').val(),
            Thickness0: $('.thick1').text() ? $('.thick1').text() : undefined,
            Thickness1: $scope.roll11 ? $scope.roll11 : undefined,
            Thickness2: $scope.roll21 ? $scope.roll21 : undefined,
            Thickness3: $scope.roll31 ? $scope.roll31 : undefined,
            RollDiameter0: $scope.thick2 ? $scope.thick2 : undefined,
            RollDiameter1: $scope.roll12 ? $scope.roll12 : undefined,
            RollDiameter2: $scope.roll22 ? $scope.roll22 : undefined,
            RollDiameter3: $scope.roll32 ? $scope.roll32 : undefined,
            PH0: $('.thick3').text() ? $('.thick3').text() : undefined,
            PH1: $scope.roll13 ? $scope.roll13 : undefined,
            PH2: $scope.roll23 ? $scope.roll23 : undefined,
            PH3: $scope.roll33 ? $scope.roll33 : undefined,
            UnpleasantSmell0: $('.thick4').text() ? $('.thick4').text() : undefined,
            UnpleasantSmell1: $(".UnpleasantSmell1").val(),
            UnpleasantSmell2: $(".UnpleasantSmell2").val(),
            UnpleasantSmell3: $(".UnpleasantSmell3").val(),
            DustDirt0: $('.thick5').text() ? $('.thick5').text() : undefined,
            DustDirt1: $(".DustDirt1").val(),
            DustDirt2: $(".DustDirt2").val(),
            DustDirt3: $(".DustDirt3").val(),
            DamagingMaterial0: $('.thick6').text() ? $('.thick6').text() : undefined,
            DamagingMaterial1: $(".DamagingMaterial1").val(),
            DamagingMaterial2: $(".DamagingMaterial2").val(),
            DamagingMaterial3: $(".DamagingMaterial3").val(),
            CoreDamaging0: $('.thick7').text() ? $('.thick7').text() : undefined,
            CoreDamaging1: $(".CoreDamaging1").val(),
            CoreDamaging2: $(".CoreDamaging2").val(),
            CoreDamaging3: $(".CoreDamaging3").val(),
            Width0: $('.thick8').text() ? $('.thick8').text() : undefined,
            Width1: $scope.roll18 ? $scope.roll18 : undefined,
            Width2: $scope.roll28 ? $scope.roll28 : undefined,
            Width3: $scope.roll38 ? $scope.roll38 : undefined,
            GSM0: $('.thick9').text() ? $('.thick9').text() : undefined,
            GSM1: $scope.roll19 ? $scope.roll19 : undefined,
            GSM2: $scope.roll29 ? $scope.roll29 : undefined,
            GSM3: $scope.roll39 ? $scope.roll39 : undefined,
            TensileCD0: $('.thick10').text() ? $('.thick10').text() : undefined,
            TensileCD1: $scope.roll110 ? $scope.roll110 : undefined,
            TensileCD2: $scope.roll210 ? $scope.roll210 : undefined,
            TensileCD3: $scope.roll310 ? $scope.roll310 : undefined,
            TensileMD0: $('.thick11').text() ? $('.thick11').text() : undefined,
            TensileMD1: $scope.roll111 ? $scope.roll111 : undefined,
            TensileMD2: $scope.roll211 ? $scope.roll211 : undefined,
            TensileMD3: $scope.roll311 ? $scope.roll311 : undefined,
            VisualInspection0: $('.thick12').text() ? $('.thick12').text() : undefined,
            VisualInspection1: $(".VisualInspection1").val(),
            VisualInspection2: $(".VisualInspection2").val(),
            VisualInspection3: $(".VisualInspection3").val(),
            Remarks1: $scope.remarks1 ? $scope.remarks1 : undefined,
            Remarks2: $scope.remarks2 ? $scope.remarks2 : undefined,
            Remarks3: $scope.remarks3 ? $scope.remarks3 : undefined,
            Remarks4: $scope.remarks4 ? $scope.remarks4 : undefined,
            Remarks5: $scope.remarks5 ? $scope.remarks5 : undefined,
            Remarks6: $scope.remarks6 ? $scope.remarks6 : undefined,
            Remarks7: $scope.remarks7 ? $scope.remarks7 : undefined,
            Remarks8: $scope.remarks8 ? $scope.remarks8 : undefined,
            Remarks9: $scope.remarks9 ? $scope.remarks9 : undefined,
            Remarks10: $scope.remarks10 ? $scope.remarks10 : undefined,
            Remarks11: $scope.remarks11 ? $scope.remarks11 : undefined,
            Remarks12: $scope.remarks12 ? $scope.remarks12 : undefined
        }
        $http.post('/api/IncomingMaterialAPI/saveQCParameter', qcDetailList)
            .then(function (response) {
                var message = response.data.message; // Extract message from response
                displayPopupNotification(message, "success");
                setTimeout(function () {
                    window.location.reload();
                }, 5000)
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }
    if ($scope.voucherno != "undefined") {
        $http.get('/api/IncomingMaterialAPI/GetEditMaterialDetails?transactionno=' + $scope.voucherno)
            .then(function (response) {
                console.log("response", response);
                $scope.ITEM_CODE = response.data.item_code;
                var dropdown = $("#ddlProductType").data("kendoDropDownList");
                dropdown.value(response.data.item_code);
                SelectProductByProductType(response.data.item_code, function (items) {
                    $scope.MATERIAL_CODE = response.data.MATERIAL_CODE || "";
                    var ddl = $("#ddlMaterials").data("kendoDropDownList");
                    ddl.value($scope.MATERIAL_CODE);   // <-- now value exists
                    ddl.trigger("change");
                });
                $scope.QC_NO = response.data.QC_NO;
                $scope.QC_DATE = kendo.toString(kendo.parseDate(response.data.QC_DATE), 'M/d/yyyy');
                $scope.supplier_edesc = response.data.supplier_edesc;
                $scope.to_location_code = response.data.supplier_edesc;
                $scope.invoice_no = response.data.invoice_no;
                /*$scope.VENDOR_NAME = response.data.supplier_edesc;*/
                $scope.VENDOR_NAME = response.data.VENDOR_NAME;
                $('#txtVendorName').text(response.data.VENDOR_NAME);
                $('#txtManualNo').val(response.data.MANUAL_NO);
                $('#txtRemarks').val(response.data.REMARKS);
                $scope.GRN_NO = response.data.GRN_NO;
                $scope.GRN_DATE = kendo.toString(kendo.parseDate(response.data.GRN_DATE), 'M/d/yyyy')
                $scope.quantity = response.data.quantity;
                $scope.item_edesc = response.data.item_edesc;
                $scope.Remarks = response.data.Remarks;
                $('.thick1').text(response.data.thickness);
                $scope.thick2 = response.data.RollDiameter0;
                $('.thick3').text(response.data.PH);
                $('.thick8').text(response.data.SIZE_WIDTH);
                $('.thick9').text(response.data.GSM);
                $('.thick10').text(response.data.Tensile_CD);
                $('.thick11').text(response.data.Tensile_MD);
                $('.thick12').text(response.data.Visual_Inspection);
                $('td.roll11 input.roll11').val(response.data.Thickness1);
                $('td.roll21 input.roll21').val(response.data.Thickness2);
                $('td.roll31 input.roll31').val(response.data.Thickness3);
                $scope.roll11 = response.data.Thickness1;
                $scope.roll21 = response.data.Thickness2;
                $scope.roll31 = response.data.Thickness3;
                $('td.roll12 input.roll12').val(response.data.RollDiameter1);
                $('td.roll22 input.roll22').val(response.data.RollDiameter2);
                $('td.roll32 input.roll32').val(response.data.RollDiameter3);
                $scope.roll12 = response.data.RollDiameter1;
                $scope.roll22 = response.data.RollDiameter2;
                $scope.roll32 = response.data.RollDiameter3;
                $('td.roll13 input.roll13').val(response.data.PH1);
                $('td.roll23 input.roll23').val(response.data.PH2);
                $('td.roll33 input.roll33').val(response.data.PH3);
                $scope.rol113 = response.data.PH1;
                $scope.rol123 = response.data.PH2;
                $scope.rol133 = response.data.PH3;
                $scope.UnpleasantSmell1 = response.data.UnpleasantSmell1;
                $scope.UnpleasantSmell2 = response.data.UnpleasantSmell2;
                $scope.UnpleasantSmell3 = response.data.UnpleasantSmell3;
                $scope.DustDirt1 = response.data.DustDirt1;
                $scope.DustDirt2 = response.data.DustDirt2;
                $scope.DustDirt3 = response.data.DustDirt3;
                $scope.DamagingMaterial1 = response.data.DamagingMaterial1;
                $scope.DamagingMaterial2 = response.data.DamagingMaterial2;
                $scope.DamagingMaterial3 = response.data.DamagingMaterial3;
                $scope.CoreDamaging1 = response.data.CoreDamaging1;
                $scope.CoreDamaging2 = response.data.CoreDamaging2;
                $scope.CoreDamaging3 = response.data.CoreDamaging3;
                $('td.roll18 input.roll18').val(response.data.Width1);
                $('td.roll28 input.roll28').val(response.data.Width2);
                $('td.roll38 input.roll38').val(response.data.Width3);
                $scope.roll18 = response.data.Width1;
                $scope.roll28 = response.data.Width2;
                $scope.roll38 = response.data.Width3;
                $('td.roll19 input.roll19').val(response.data.GSM1);
                $('td.roll29 input.roll29').val(response.data.GSM2);
                $('td.roll39 input.roll39').val(response.data.GSM3);
                $scope.roll19 = response.data.GSM1;
                $scope.roll29 = response.data.GSM2;
                $scope.roll39 = response.data.GSM3;
                $('td.roll110 input.roll110').val(response.data.TensileCD1);
                $('td.roll210 input.roll210').val(response.data.TensileCD2);
                $('td.roll310 input.roll310').val(response.data.TensileCD3);
                $scope.roll110 = response.data.TensileCD1;
                $scope.roll210 = response.data.TensileCD2;
                $scope.roll310 = response.data.TensileCD3;
                $('td.roll111 input.roll111').val(response.data.TensileMD1);
                $('td.roll211 input.roll211').val(response.data.TensileMD2);
                $('td.roll311 input.roll311').val(response.data.TensileMD3);
                $scope.roll111 = response.data.TensileMD1;
                $scope.roll211 = response.data.TensileMD2;
                $scope.roll311 = response.data.TensileMD3;
                $scope.VisualInspection1 = response.data.VisualInspection1;
                $scope.VisualInspection2 = response.data.VisualInspection2;
                $scope.VisualInspection3 = response.data.VisualInspection3;
                $('td.remarks1 input.remarks1').val(response.data.Remarks1);
                $('td.remarks2 input.remarks2').val(response.data.Remarks2);
                $('td.remarks3 input.remarks3').val(response.data.Remarks3);
                $('td.remarks4 input.remarks4').val(response.data.Remarks4);
                $('td.remarks5 input.remarks5').val(response.data.Remarks5);
                $('td.remarks6 input.remarks6').val(response.data.Remarks6);
                $('td.remarks7 input.remarks7').val(response.data.Remarks7);
                $('td.remarks8 input.remarks8').val(response.data.Remarks8);
                $('td.remarks9 input.remarks9').val(response.data.Remarks9);
                $('td.remarks10 input.remarks10').val(response.data.Remarks10);
                $('td.remarks11 input.remarks11').val(response.data.Remarks11);
                $('td.remarks12 input.remarks12').val(response.data.Remarks12);
                $scope.remarks1 = response.data.Remarks1;
                $scope.remarks2 = response.data.Remarks2;
                $scope.remarks3 = response.data.Remarks3;
                $scope.remarks4 = response.data.Remarks4;
                $scope.remarks5 = response.data.Remarks5;
                $scope.remarks6 = response.data.Remarks6;
                $scope.remarks7 = response.data.Remarks7;
                $scope.remarks8 = response.data.Remarks8;
                $scope.remarks9 = response.data.Remarks9;
                $scope.remarks10 = response.data.Remarks10;
                $scope.remarks11 = response.data.Remarks11;
                $scope.remarks12 = response.data.Remarks12;


                function getItemsDropdown(modelName, selectedValue) {
                    return `
									<select class="${modelName}" ng-model="${modelName}">
										<option value="" ${selectedValue === "" ? "selected" : ""}></option>
										<option value="OK" ${selectedValue === "OK" ? "selected" : ""}>OK</option>
										<option value="Not OK" ${selectedValue === "Not OK" ? "selected" : ""}>Not OK</option>
									</select>
								`;
                }

                $('.thick4').html("OK / Not OK");
                $('.roll14').html(getItemsDropdown('UnpleasantSmell1', $scope.UnpleasantSmell1));
                $('.roll24').html(getItemsDropdown('UnpleasantSmell2', $scope.UnpleasantSmell2));
                $('.roll34').html(getItemsDropdown('UnpleasantSmell3', $scope.UnpleasantSmell3));
                $('.thick5').html("OK / Not OK");
                $('.roll15').html(getItemsDropdown('DustDirt1', response.data.DustDirt1));
                $('.roll25').html(getItemsDropdown('DustDirt2', response.data.DustDirt2));
                $('.roll35').html(getItemsDropdown('DustDirt3', response.data.DustDirt3));
                $('.thick6').html("OK / Not OK");
                $('.roll16').html(getItemsDropdown('DamagingMaterial1', response.data.DamagingMaterial1));
                $('.roll26').html(getItemsDropdown('DamagingMaterial2', response.data.DamagingMaterial2));
                $('.roll36').html(getItemsDropdown('DamagingMaterial3', response.data.DamagingMaterial3));
                $('.thick7').html("OK / Not OK");
                $('.roll17').html(getItemsDropdown('CoreDamaging1', response.data.CoreDamaging1));
                $('.roll27').html(getItemsDropdown('CoreDamaging2', response.data.CoreDamaging2));
                $('.roll37').html(getItemsDropdown('CoreDamaging3', response.data.CoreDamaging3));
                $('.thick12').html("OK / Not OK");
                $('.roll112').html(getItemsDropdown('VisualInspection1', response.data.VisualInspection1));
                $('.roll212').html(getItemsDropdown('VisualInspection2', response.data.VisualInspection2));
                $('.roll312').html(getItemsDropdown('VisualInspection3', response.data.VisualInspection3));
            })
            .catch(function (error) {
                displayPopupNotification("Error fetching ID", "error");
            });
    }
});
