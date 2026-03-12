PMModule.controller('issueTypeCtrl', function ($scope, $http, $routeParams, $window, $filter) {
    $scope.issueTypeDataSource = {
        type: "json",
        serverFiltering: true,
        suggest: true,
        highlightFirst: true,
        transport: {
            read: {
                url: "/api/TemplateApi/GetAllIssueTypeListByFilter",
            },
            parameterMap: function (data, action) {
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

    $scope.IssueTypeOption = {
        dataSource: $scope.issuetypeDataSource,
        template: '<span>{{dataItem.ISSUE_TYPE_CODE}}</span>  ' +
        '<span>{{dataItem.Type}}</span>',
        dataBound: function (e) {
           
            if (this.element[0].attributes['issuetype-index'] == undefined) {
                var issue = $("#issuetype").data("kendoComboBox");
            }
            else {
                var index = this.element[0].attributes['issuetype-index'].value;
                var issueLength = ((parseInt(index) + 1) * 3) - 1;
                var issue = $($(".issue")[issueLength]).data("kendoComboBox");

            }
            //if (issue != undefined) {
            //    issue.setOptions({
            //        template: $.proxy(kendo.template("#= formatValue(ISSUE_TYPE_EDESC, Type, this.text()) #"), issue)
            //    });
            //}
        }
    }

    $scope.issueTypeCodeOnChange = function (kendoEvent) {
        if (kendoEvent.sender.dataItem() == undefined) {
            $scope.issuetypeerror = "Please Enter Valid Code."
            $('#issuetype').data("kendoComboBox").value([]);
            $(kendoEvent.sender.element[0]).addClass('borderRed');
        }
        else {
            $scope.issuetypeerror = "";
            $(kendoEvent.sender.element[0]).removeClass('borderRed');
        }
    }
});