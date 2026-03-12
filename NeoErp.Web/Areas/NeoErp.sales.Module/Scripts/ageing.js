var ageing = function () {
    return {

        url: "",
        filters: function(){
            return {
               
                AsOnDate: $("#datePickerAd").val(),
                FrequencyInDay: !($("#range-frequency-days").val() == "") && parseInt($("#range-frequency-days").val()) != 0 ? $("#range-frequency-days").val() : $("#ddl-frequency-days :selected").val(),
                FixedInDay: !($("#range-fixed-days").val() == "") && parseInt($("#range-fixed-days").val()) != 0 ? $("#range-fixed-days").val() : $("#ddl-fixed-days :selected").val(),
                Type: $("input[name='ageing-type-filter']:checked").attr("data-label"),
                Codes: ageing.getCodeFilters(),
                BillWiseOrLedgerWise: $("#BillWiseOrLedgerWise").val(),
                ShowGroupWise: $("input[name='ShowGroupSwitch']").prop('checked')?"True":"false",
            }
        },
        init: function (serviceUrl) {           
            ageing.url = serviceUrl;

            $("#RunQuery").on("click", function () {
                ageing.loadAgeingReport();
            });
           // ageing.loadAgeingReport();

            $("#applyFilters").on("click", function () {
                $("#RunQuery").trigger("click");
            });

            $(".applydp").on("click", function () {
                //$("#RunQuery").trigger("click");

            });
        },
        loadAgeingReport: function () {
            var pageContentBody = $('#ageing-content');
            Pace.track(function () {
                $.ajax({
                    type: "GET",
                    cache: false,
                    url: ageing.url,
                    data: ageing.filters(),
                    dataType: "html",
                    success: function (res) {
                        pageContentBody.html(res);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        pageContentBody.html('<h4>Could not load the requested content.</h4>');
                    },
                    async: false
                });
            });
           
        },
        getCodeFilters: function () {  
            var type = $("input[name='ageing-type-filter']:checked").attr("data-label");
            var codeIds = [];
            if(type == "Customer")
            {
                var treeview = $("#customerTreeView").data("kendoTreeView");
                var items = $("#customerTreeView .k-item input[type=checkbox]:checked").closest(".k-item");
            
                $(items).each(function () {

                    codeIds.push(treeview.dataSource.getByUid($(this).attr("data-uid")).masterCustomerCode);
                });

            }
            else if (type == "Supplier") {
                var treeview = $("#supplierTreeView").data("kendoTreeView");
                var items = $("#supplierTreeView .k-item input[type=checkbox]:checked").closest(".k-item");
                $(items).each(function () {
                    codeIds.push(treeview.dataSource.getByUid($(this).attr("data-uid")).MasterSupplierCode);
                });
            }
            else if (type == "Dealer") {
                var treeview = $("#dealerTreeView").data("kendoTreeView");
                var items = $("#dealerTreeView .k-item input[type=checkbox]:checked").closest(".k-item");
                $(items).each(function () {
                    codeIds.push(treeview.dataSource.getByUid($(this).attr("data-uid")).MasterSupplierCode);
                });
            }

            return codeIds;
        }
    };
}();

var AgeingReportFilter = function () {
    return {
        init: function () {
            $("#applyFilters").on("click", function () {
                $("#ageingReportFilter").modal("toggle");

            });

            $(".non-negative-non-decimal").keypress(function (evt) {
                var keycode = evt.charCode || evt.keyCode;
                if (keycode == 45 || keycode == 43 || keycode == 46) { //Enter key's keycode
                    return false;
                }
            });

            if (!$().iCheck) {
                return;
            }

            $('.icheck.ageing').each(function () {

                var checkboxClass = $(this).attr('data-checkbox') ? $(this).attr('data-checkbox') : 'icheckbox_line-green';
                var radioClass = $(this).attr('data-radio') ? $(this).attr('data-radio') : 'iradio_line-green';

                if (checkboxClass.indexOf('_line') > -1) {
                    $(this).iCheck({
                        checkboxClass: checkboxClass,
                        insert: '<div class="icheck_line-icon"></div>' + $(this).attr("data-label"),
                        radioClass: radioClass
                    });
                } else {
                    $(this).iCheck({
                        checkboxClass: checkboxClass,
                        radioClass: radioClass,
                    });
                }

                //if ($(this).attr("checked") == undefined && $(this).attr("id") == "user-custom-frequency-days") {
                //    AgeingReportFilter.toggleFrequencyFilter(false);
                //}

                //if ($(this).attr("checked") == undefined && $(this).attr("id") == "user-custom-fixed-days") {
                //    AgeingReportFilter.toggleFixedDayFilter(false);
                //}
            });

            $('.icheck.ageing.tab').on('ifChecked', function () {
                AgeingReportFilter.toggleTab(this);
                AgeingReportFilter.activateTab(this);
                AgeingReportFilter.bindTree(this);
            });

            AgeingReportFilter.toggleTab('.icheck.ageing.tab:checked');
            AgeingReportFilter.activateTab('.icheck.ageing.tab:checked');
            AgeingReportFilter.bindTree('.icheck.ageing.tab:checked');

            $('.icheck.ageing.toggle').on("ifToggled", function (event) {

                if ($(this).attr("id") == "user-custom-fixed-days") {
                    if ($(this).attr("checked") == undefined && $(this).attr("id") == "user-custom-fixed-days") {

                        AgeingReportFilter.toggleFixedDayFilter(false);
                    }
                    else {
                        AgeingReportFilter.toggleFixedDayFilter(true);
                    }
                }

                if ($(this).attr("id") == "user-custom-frequency-days") {
                    if ($(this).attr("checked") == undefined && $(this).attr("id") == "user-custom-frequency-days") {

                        AgeingReportFilter.toggleFrequencyFilter(false);
                    }
                    else {
                        AgeingReportFilter.toggleFrequencyFilter(true);
                    }
                }
            });
        },
        toggleFrequencyFilter: function (customChecked) {
            if (customChecked == undefined || !customChecked) {

                $("#range-frequency-days").attr("disabled", "disabled");
                $("#ddl-frequency-days").removeAttr("disabled");
            }
            else {
                $("#range-frequency-days").removeAttr("disabled");
                $("#ddl-frequency-days").attr("disabled", "disabled");
            }
        },
        toggleFixedDayFilter: function (customChecked) {

            if (customChecked == undefined || !customChecked) {

                $("#range-fixed-days").attr("disabled", "disabled");
                $("#ddl-fixed-days").removeAttr("disabled");
            }
            else {
                $("#range-fixed-days").removeAttr("disabled");
                $("#ddl-fixed-days").attr("disabled", "disabled");
            }

        },
        activateTab: function (tab) {

            $('.nav-tabs a[href="#' + $(tab).val() + '"]').tab('show');
        },
        bindTree: function (tab) {
            if ($(tab).val() == "ageingProductTab") {
                $("#productTreeView").data("kendoTreeView").dataSource.bind("change",
                    $("#productTreeView").data("kendoTreeView").dataSource.read());
            }
            else if ($(tab).val() == "ageingCustomerTab") {
                $("#customerTreeView").data("kendoTreeView").dataSource.bind("change",
                    $("#customerTreeView").data("kendoTreeView").dataSource.read());

            }
            else if ($(tab).val() == "ageingSupplierTab") {
                $("#supplierTreeView").data("kendoTreeView").dataSource.bind("change",
                    $("#supplierTreeView").data("kendoTreeView").dataSource.read());
            }
            else if ($(tab).val() == "ageingDealerTab") {
                $("#dealerTreeView").data("kendoTreeView").dataSource.bind("change",
                    $("#dealerTreeView").data("kendoTreeView").dataSource.read());
            }
        },
        toggleTab: function (tab) {
            $("#href-" + $(tab).val()).attr("data-toggle", "tab");
            $("#href-" + $(tab).val()).attr("href", "#" + $(tab).val());
            $("#href-" + $(tab).val()).parent().siblings().each(function (index, element) {
                var href = $(element).find("a");

                $(href).removeAttr("data-toggle");
                $(href).attr("href", "javascript:void();")
            });
        }
    };

}();