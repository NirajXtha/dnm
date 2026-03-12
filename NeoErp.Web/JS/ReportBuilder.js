var ReportBuilder = function () {
    var _reportSaveUrl = "",
     _reportPreviewUrl = Metronic.getGlobalUrl() + "reportbuilder/ReportPreview",
      _reportSaveUrl = Metronic.getGlobalUrl() + "api/reportbuilder/SaveReport",
     _previewButton = $(".report-preview"),
         _saveButton = $(".report-save"),
    _reportPortlet = $("#reportPreview"),
    reportParameters = function() {
        return {
            ReportName: $("#reportName").val(),
            Query: $("#query").code().toString().replace(/<\/p>/gi, "\n")
                .replace(/<br\/?>/gi, "\n")
                .replace(/<\/?[^>]+(>|$)/g, ""),
            //Query: $("#query").val(),
            //Query:$("<div/>").text($("#query").val()).html(),
            Settings: "",
        }
        
    },
    loadPreview = function () {
        _previewButton.button("loading");
        Metronic.blockUI({
            target: _reportPortlet,
            animate: true,
            overlayColor: 'none'
        });
        $.ajax({
            type: "POST",
            url: _reportPreviewUrl,
            data: reportParameters(),
            dataType: 'html',
            //contentType:'application/html',
            success: function (res) {
                Metronic.unblockUI(_reportPortlet);
                res = Metronic.replaceEncoding(res);
                _reportPortlet.html(res);
                _previewButton.button("reset");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                Metronic.unblockUI(_reportPortlet);
                _previewButton.button("reset");
                var msg = 'Error on reloading the content. Please check your connection and try again.';
                if (toastr) {
                    toastr.error(msg);
                } else if ($.notific8) {
                    $.notific8('zindex', 11500);
                    $.notific8(msg, {
                        theme: 'ruby',
                        life: 3000
                    });
                } else {
                    alert(msg);
                }
            }
        });
    };
    saveReport = function () {
        _saveButton.button("loading");
        Metronic.blockUI({
            target: _reportPortlet,
            animate: true,
            overlayColor: 'none'
        });
        $.ajax({
            type: "POST",
            url: _reportSaveUrl,
            data: reportParameters(),
            dataType: 'html',
            //contentType:'application/html',
            success: function (res) {
                Metronic.unblockUI(_reportPortlet);
                res = Metronic.replaceEncoding(res);
            
                _saveButton.button("reset");
            },
            error: function (xhr, ajaxOptions, thrownError) {
                Metronic.unblockUI(_reportPortlet);
                _previewButton.button("reset");
                var msg = 'Error on reloading the content. Please check your connection and try again.';
                if (toastr) {
                    toastr.error(msg);
                } else if ($.notific8) {
                    $.notific8('zindex', 11500);
                    $.notific8(msg, {
                        theme: 'ruby',
                        life: 3000
                    });
                } else {
                    alert(msg);
                }
            }
        });
    };

    return {
        init: function () {
            $("#query").summernote({
                height: 300,
                toolbar: []
            });
            _previewButton.on("click", function () {
                loadPreview();
            });
            _saveButton.on("click", function () {
                saveReport();
            })
        }
        
    };
}();

var ReportBuilderPreview = function () {
    var gridReadUrl = Metronic.getGlobalUrl() + "api/reportbuilder/GetData";
    var readParameters = {
        reportName: "",
        query: "",
        settings:"",
    };
    var dataSource = [];
    var grid = null;
    var loadGrid = function () {
       grid = $("#previewGrid").kendoGrid({
            dataSource: dataSource,
            autobind: true,
            height: 600,
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
            pageable: true,
            scrollable: {
                virtual: true
            },

        });

    }
    var loadDataSource = function () {
        dataSource = new kendo.data.DataSource({
            type: "json",
            transport: {
                read: {
                    url: gridReadUrl, // <-- Get data from here.
                    dataType: "json", // <-- The default was "jsonp".
                    type: "POST",
                    contentType: "application/json; charset=utf-8"
                },
                parameterMap: function (options, type) {
                    console.log(options);
                    var paramMap = JSON.stringify($.extend(options, readParameters));
                    delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                    delete paramMap.$format; // <-- remove format parameter.
                    return paramMap;
                }
            },
            change: function (data) {
                
            },
            schema: {
                data: "Data", // records are returned in the "data" field of the response,
                total:"Total"
                
            },
            serverFiltering: true,
            serverPaging: true,
            serverSorting: true,
            pageSize: 100,
        });
        //dataSource.read();
    }
    return {
        init: function (params) {
            
          
            if(params.query)
            {
                params.query = $("<div/>").text(params.query).html();
            }
            readParameters = $.extend(true, readParameters, params);
            loadDataSource();
            loadGrid();
        }
    }



}();