
DTModule.controller('areaSetupCtrl', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.areaSetupDataSource = {
       
        serverFiltering: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllAreaSetupByFilter",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                type: "GET"
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
    };
    $scope.AreaSetupOption = {
        dataSource: $scope.areaSetupDataSource,
        template: '<span>{{dataItem.AREA_EDESC}}</span>  ' +
        '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            var issue;
            if (this.element[0].attributes['areasetup-index'] == undefined) {
                issue = $("#areasetup").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['areasetup-index'].value;
                var issueLength = ((parseInt(index) + 1) * 3) - 1;
                issue = $($(".areasetup")[issueLength]).data("kendoComboBox");

            }
            if (issue != undefined) {
                issue.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(AREA_EDESC, '', this.text()) #"), issue)
                });
            }
        }
    }

    $scope.areaSetupOnChange = function (kendoEvent) {

        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.issuetypeerror = "Please Enter Valid Code."
            $('#areasetup').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.areasetuperror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }
});