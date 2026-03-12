var calendar = function () {
    return {

        url: "",
        filters: function () {
            return {
                CalenderTypeValue: $('input[name="radioCalendarType"]:checked').val(),
                ShowGroup: $('input[name="radioReportType"]:checked').val(),
                AsOnDate: $("#datePickerAd").val(),
                formDate: $("#FromDateVoucher").val(),
                toDate: $("#ToDateVoucher").val(),
                FirstHorizontalPeriod: $("#ddlFirstHP").val().toString().length != 0 ? $("#ddlFirstHP").val() : "Y",
                SecondHorizontalPeriod: $("#ddlSecondHP").val().toString().length != 0 ? $("#ddlSecondHP").val() : "W",
                ReportId: $("#ddlReportList").data("kendoDropDownList").value()
            }
        },
        getReportListFilter: function()
        {
            
            var reportId = "";
            var reportListDropDown = $("#ddlReportList").data("kendoDropDownList");
            reportId = reportListDropDown.value();
            return reportListDropDown;
        },
        init: function (serviceUrl) {
            
            calendar.url = serviceUrl;

            $("#RunQuery").on("click", function () {
                calendar.loadCalendarReport();
            });
            calendar.loadCalendarReport();

            $("#applyFilters").on("click", function () {
                //filters();
                $("#RunQuery").trigger("click");
            });

            $("#reportListFilterButton").click(function () {
                $("#reportListTreeview").data("kendoTreeView").dataSource.bind("change",
                    $("#reportListTreeview").data("kendoTreeView").dataSource.read()
                );
            });
        },
        loadCalendarReport: function () {
            var pageContentBody = $('#calendar-content');
            $.ajax({
                type: "GET",
                cache: false,
                url: calendar.url,
                data: calendar.filters(),
                dataType: "html",
                success: function (res) {
                    pageContentBody.html(res);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    pageContentBody.html('<h4>Could not load the requested content.</h4>');
                },
                async: false
            });

        },
    };
}();