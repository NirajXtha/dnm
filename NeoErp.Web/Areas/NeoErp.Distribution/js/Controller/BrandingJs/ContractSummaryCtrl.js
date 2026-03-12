distributionModule.filter('camelCase', function () {
    return function (input) {
        input = input || '';
        return input.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
    };
});

distributionModule.controller("ContractSummary", function ($scope, BrandingService, $routeParams) {
    $scope.contractSummary = [];

    $scope.$on('$routeChangeSuccess', function () {
        BrandingService.getContractSummary().then(function (response) {
            $scope.contractSummary = response.data;
            for (var i = 0; i < $scope.contractSummary.length; i++) {
                if (!$scope.contractSummary[i].Questions)
                    continue;
                //if question is present create multiselect accordingly
                for (var j = 0; j < $scope.contractSummary[i].Questions.length; j++) {
                    $scope.contractSummary[i].Questions[j].ANSWER = ''; //answer will be binded to this property

                    var type = $scope.contractSummary[i].Questions[j].QA_TYPE;
                    if (type == 'MCR' || type == 'MCC') {
                        $scope.contractSummary[i].Questions[j].DDLOptions = {
                            placeholder: "Select answer...",
                            animation: {
                                open: {
                                    effects: "zoom:in",
                                    duration: 300
                                }
                            },
                            autoClose: false,
                            valuePrimitive: true,
                            autoBind: false,
                            dataSource: {
                                data: $scope.contractSummary[i].Questions[j].ANSWERS,
                            },
                        }
                    }
                    if (type == "MCR")
                        $scope.contractSummary[i].Questions[j].DDLOptions.maxSelectedItems = 1;
                }
            }
        });
    });

    $scope.SaveQuestion = function (index) {
        var Contract = $scope.contractSummary[index];
        var answers = GetSavingObj(Contract.Questions);
        var SaveObj = {
            CONTRACT_CODE: Contract.CONTRACT_CODE,
            ANSWERS: answers
        };
        BrandingService.saveContractAns(SaveObj).then(function (response) {
            displayPopupNotification(response.data.MESSAGE, response.data.TYPE);
            $scope.CancelItem(index);
        },
        function (ex) {
            displayPopupNotification("Error processing request","error");
        });
    }

    $scope.CancelItem = function (index) {
        var Questions = $scope.contractSummary[index].Questions;
        for (let i = 0; i < Questions.length; i++) {
            $scope.contractSummary[index].Questions[i].ANSWER = '';
        }
    }

    var GetSavingObj = function (Ques) {
        var Result = [];
        Ques.forEach(function (value, key) {
            Result.push({
                QA_CODE: value.QA_CODE,
                ANSWER: (typeof (value.ANSWER) == "object") ? value.ANSWER.join() : value.ANSWER
            });
        });
        return Result;
    }
});

distributionModule.controller("ContractAnsReportCtrl", function ($scope) {

    $scope.mainGridOptions = {
        dataSource: {
            type: 'JSON',
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Branding/GetContractQuesReport",
            },
            pageSize: 100,
            group: {
                field: "CONTRACT_EDESC",
            },
        },

        toolbar: kendo.template($("#toolbar-template").html()),
        height: window.innerHeight - 100,
        sortable: true,
        reorderable: true,
        groupable: true,
        resizable: true,
        pageable: {
            refresh: true,
            pageSizes: [100, 200, 300, 500],
            buttonCount: 2
        },
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
        //scrollable: {
        //    virtual: true
        //},
        dataBound: function (o) {
            $('div').removeClass('.k-header k-grid-toolbar');
        },
        columns: [{
            field: "CONTRACT_EDESC",
            title: "Contract Name",
            width: "10%",
        }, {
            field: "QUESTION",
            title: "Question",
            width: "35%"
        }, {
            field: "ANSWER",
            title: "Answer",
            width: "25%",
        }, {
            field: "ANSWERED_BY",
            title: "Answered_by",
            width: "20%",
        }, {
            field: "SOURCE",
            title:"Source",
            width: "10%",
        }]
    };
});