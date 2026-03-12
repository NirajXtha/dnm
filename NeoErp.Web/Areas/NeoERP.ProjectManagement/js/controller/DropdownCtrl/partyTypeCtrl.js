DTModule.controller('partyTypeCtrl', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.partyTypeDataSource = {
        type: "json",
        serverFiltering: true,
        suggest: true,
        highlightFirst: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllPartyTypeByFilter",
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
        },
    };
    $scope.PartyTypeOption = {
        dataSource: $scope.issuetypeDataSource,
        template: '<span>{{dataItem.PARTY_TYPE_EDESC}}</span>  ' +
        '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
            var issue;
            if (this.element[0].attributes['partytype-index'] == undefined) {
                issue = $("#partytype").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['partytype-index'].value;
                var issueLength = ((parseInt(index) + 1) * 3) - 1;
                issue = $($(".partytype")[issueLength]).data("kendoComboBox");

            }
            if (issue != undefined) {
                issue.setOptions({
                    template: $.proxy(kendo.template("#= formatValue(PARTY_TYPE_EDESC, '', this.text()) #"), issue)
                });
            }
        }
    }

    $scope.partyTypeCodeOnChange = function (kendoEvent) {

        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.partytypeerror = "Please Enter Valid Code."
            $('#partytype').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.partytypeerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }
});