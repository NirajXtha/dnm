distributionModule.controller('SurveySetupCtrl', function ($scope, DistSetupService) {
    $scope.showForm = false;
    reportConfig = GetReportSetting("SurveySetup");

    $scope.Survey = {
        SURVEY_CODE: null,
        GROUP_CODES: null,
        AREA_CODES: null,
        SP_CODES: null,
        SURVEY_EDESC: '',
        QUESTIONS: null,
        EXPIRY_DATE: ''
    };

    $scope.GroupOptions = {
        dataTextField: "GROUP_EDESC",
        dataValueField: "GROUPID",
        height: 600,
        valuePrimitive: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Groups...</strong></div>',
        placeholder: "Select Group...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllResellerGroups",
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            debugger;
            let areaCodes = [], spCodes = [];
            if ($scope.Survey.GROUP_CODES) {
                //select areas
                areas = $("#AreaCode").data('kendoMultiSelect').dataSource.data();
                areaIngroup = _.filter(areas, function (item) { return $scope.Survey.GROUP_CODES.includes(String(item.GROUPID)); });
                areaCodes = _.pluck(areaIngroup, 'AREA_CODE');

                //select sales persons
                spData = $("#spCode").data('kendoMultiSelect').dataSource.data();
                spInArea = _.filter(spData, function (item) { return _.intersection(areaCodes, item.AREA_CODE.split(",")).length > 0; });
                spCodes = _.pluck(spInArea, 'SP_CODE');
            }
            $scope.Survey.AREA_CODES = areaCodes.length > 0 ? areaCodes : null;
            $scope.Survey.SP_CODES = spCodes.length > 0 ? spCodes : null;
        }
    }

    $scope.AreaOptions = {
        dataTextField: "AREA_NAME",
        dataValueField: "AREA_CODE",
        height: 600,
        valuePrimitive: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Areas...</strong></div>',
        placeholder: "Select Area...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetDistArea",
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            let groupCodes = [], spCodes = [];
            if ($scope.Survey.AREA_CODES) {

                //select groups
                areas = $("#AreaCode").data('kendoMultiSelect').dataSource.data();
                areaIngroup = _.filter(areas, function (item) { return $scope.Survey.AREA_CODES.includes(item.AREA_CODE); });
                groupCodes = _.pluck(areaIngroup, 'GROUPID');

                //select sales persons
                spData = $("#spCode").data('kendoMultiSelect').dataSource.data();
                spInArea = _.filter(spData, function (item) { return _.intersection($scope.Survey.AREA_CODES, item.AREA_CODE.split(",")).length > 0; });
                spCodes = _.pluck(spInArea, 'SP_CODE');
            }
            $scope.Survey.GROUP_CODES = groupCodes.length > 0 ? groupCodes : null;
            $scope.Survey.SP_CODES = spCodes.length > 0 ? spCodes : null;
        }
    }

    $scope.SPOptions = {
        dataTextField: "EMPLOYEE_EDESC",
        dataValueField: "SP_CODE",
        height: 600,
        valuePrimitive: true,
        headerTemplate: '<div class="col-md-offset-3"><strong>Salespersons...</strong></div>',
        placeholder: "Select Salesperson...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/UserSetup/GetSalePerson",
                    dataType: "json"
                }
            }
        },
        change: function (e) {
            let groupIds = [], areas = [];
            if ($scope.Survey.SP_CODES) {
                spData = $("#spCode").data('kendoMultiSelect').dataSource.data();
                selectedSP = _.filter(spData, function (item) { return $scope.Survey.SP_CODES.includes(item.SP_CODE); })
                groupIds = _.unique(_.pluck(selectedSP, "GROUPID"));
                areas = _.unique(_.pluck(selectedSP, 'AREA_CODE').join(',').split(','));
            }
            $scope.Survey.GROUP_CODES = groupIds.length > 0 ? groupIds : null;
            $scope.Survey.AREA_CODES = areas.length > 0 ? areas : null;
        }
    }

    $scope.QuestionOptions = {
        dataTextField: "TITLE",
        dataValueField: "SET_CODE",
        height: 600,
        valuePrimitive: false,
        headerTemplate: '<div class="col-md-offset-3"><strong>Questions...</strong></div>',
        placeholder: "Select Questions...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetAllquestions",
                    dataType: "json"
                }
            }
        },
    }

    $scope.CreateSurvey = function () {
        if (!$scope.Survey.QUESTIONS || $scope.Survey.QUESTIONS.length < 1)
            $scope.SurveyForm.$valid = false;
        if (!$scope.SurveyForm.$valid) {
            displayPopupNotification("Invalid input fileds", "warning");
            return;
        }
        $scope.Survey.GROUP_CODES = $scope.Survey.GROUP_CODES.join(',');
        $scope.Survey.AREA_CODES = $scope.Survey.AREA_CODES.join(',');

        DistSetupService.SaveSurvey($scope.Survey).then(
            function (res) {
                if (res.data.TYPE == 'success') {
                    $("#grid").data("kendoGrid").dataSource.read();
                    $scope.Cancel();
                }
                else {
                    $scope.Survey.GROUP_CODES = $scope.Survey.GROUP_CODES.split(',');
                    $scope.Survey.AREA_CODES = $scope.Survey.AREA_CODES.split(',');
                }
                displayPopupNotification(res.data.MESSAGE, res.data.TYPE);
            },
            function (ex) {
                $scope.Survey.GROUP_CODES = $scope.Survey.GROUP_CODES.split(',');
                $scope.Survey.AREA_CODES = $scope.Survey.AREA_CODES.split(',');
                displayPopupNotification("Error", "error");
            });
    }

    $scope.AddClickEvent = function () {
        $scope.showForm = true;
    }

    $scope.Cancel = function () {
        $scope.Survey = {
            SURVEY_CODE: null,
            GROUP_CODES: null,
            AREA_CODES: null,
            SP_CODES: null,
            SURVEY_EDESC: '',
            QUESTIONS: null,
            EXPIRY_DATE: ''
        };
        $scope.showForm = false;
    }

    $scope.grid = {
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Setup/GetSurveyList",
            },
            schema: {
                model: {
                    fields: {
                        EXPIRY_DATE: { type: "date" },
                    }
                },
            },
            pageSize: 1000,
        },
        height: window.innerHeight - 50,
        groupable: true,
        sortable: true,
        pageable: {
            refresh: true,
            pageSizes: reportConfig.itemPerPage,
            buttonCount: 5
        },
        scrollable: {
            virtual: true
        },
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
        toolbar: kendo.template($("#toolbar-template").html()),
        dataBound: function (o) {
            var grid = o.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
            UpdateReportUsingSetting("SurveySetup", "grid");
            $('.k-grid td').css("white-space", "normal"); //wrap text
            $('div').removeClass('.k-header k-grid-toolbar');
        },
        columnMenu: true,
        excel: {
            fileName: "Surveys",
            allPages: true,
        },
        pdf: {
            fileName: "Survey",
            allPages: true,
            avoidLinks: true,
            pageSize: "auto",
            margin: {
                top: "2m",
                right: "1m",
                left: "1m",
                buttom: "1m",
            },
            landscape: true,
            repeatHeaders: true,
            scale: 0.8,
        },
        columns: [{
            field: "SURVEY_EDESC",
            title: "Survey Name",
        }, {
            field: "TITLE",
            title: "Question Sets",
            encoded: false
        }, {
            field: "EXPIRY_DATE",
            title: "Expiry Date",
            format: "{0:dd-MMM-yyyy}",
        }, {
            title: "Actions",
            width: "10%",
            attributes: {
                style: "text-align: center;"
            },
            template: " <a class='fa fa-edit editAction' ng-click='UpdateItem(#:SURVEY_CODE#)' title='Edit'>", //</a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='deleteItem(#:ITEM_CODE#)' title='Delete'></a>  

        }]
    }

    $scope.UpdateItem = function (surveyCode) {
        var gridDs = $("#grid").data("kendoGrid").dataSource.data();
        var survey = _.filter(gridDs, function (x) { return x.SURVEY_CODE == surveyCode })[0];
        sp_codes = survey.SP_CODE_STR.split(',');
        questions = survey.SET_INFO.split(',').map(function (e) { arr = e.split('-'); return { SET_CODE: arr[0], TYPE: arr[1] }; });
        $scope.Survey = {
            SURVEY_CODE: surveyCode,
            GROUP_CODES: survey.GROUP_CODES.split(','),
            AREA_CODES: survey.AREA_CODES.split(','),
            SP_CODES: sp_codes,
            SURVEY_EDESC: survey.SURVEY_EDESC,
            QUESTIONS: questions,
            EXPIRY_DATE: moment(survey.EXPIRY_DATE).format('MM/DD/YYYY')
        };
        $scope.showForm = true;
    }

});