var Report = (function (report, $, ko) {
    "use strict"
    report.summary = function () {
        var config = {

        };
        var viewModel = {
            

        };
        return {
            config: config,
            viewModel: viewModel,
            init: function () {
                $("#currentdate").datepicker();
                ko.applyBindings(viewModel);
            },
            render: function () {
                
            },

        }

    };
    return report;
}(Report || {}, jQuery, ko));