distributionModule.directive('ngPlaceholder', function($document) {
    return {
        restrict: 'A',
        scope: {
            placeholder: '=ngPlaceholder'
        },
        link: function(scope, elem, attr) {
            scope.$watch('placeholder',function() {
                elem[0].placeholder = scope.placeholder;
            });
        }
    }
});

distributionModule.controller('QuestionCtrl', function ($scope, QuestionService, $routeParams, $http) {
    debugger;
    $scope.GeneralSet = {
        SetId: '',
        SetTitle: '',
        SetType: '',
        IsActive: 'Y',
        Questions: []
    };
    $scope.GeneralTypeDDL = [];
    $scope.TabularSet = [];
    $scope.loadingBtn = false;
    $scope.TypeDDL = {
        optionLabel: "--Select--",
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: [
            { Text: "General", Value: "G" },
            { Text: "Branding", Value: "B" },
            { Text: "Scheme", Value: "S" },
            { Text: "Web", Value: "w" },
            { Text: "Other", Value: "O" },
        ]
    };
    $scope.StatusDDl = {
        optionLabel: "--Select--",
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: [
            { Text: "Active", Value: "Y" },
            { Text: "Inactive", Value: "N" },
        ]
    };

    var cellDetails = function (size) {
        this.array = [];
        for (let i = 0; i < size; i++) {
            this.array.push({
                Type: '',
                Label: '',
                Desc: ''
            });
        }
        return this.array;
    }
    $scope.AddGeneralQuestion = function () {
        var len = $scope.GeneralSet.Questions.length;
        var available = $scope.GeneralSet.Questions;
        $scope.GeneralSet.Questions = [];
        $scope.GeneralSet.Questions.push({
            Type: '',
            Question: '',
            Answer: '',
            Placeholder: '',
            Disabled: true
        });
        for (let i = 0; i < len; i++)
            $scope.GeneralSet.Questions.push(available[i]);
    }
    $scope.$on('$routeChangeSuccess', function () {
        $scope.AddGeneralQuestion();
        $scope.AddTabularQuestion();
        if (typeof ($routeParams.setId) != "undefined" && typeof ($routeParams.type) != "undefined") {
            if ($routeParams.type == 'General') {
                QuestionService.GetGeneral($routeParams.setId).then(function (result) {
                    for (let i = 0; i < result.data.Questions.length; i++) {
                        if (result.data.Questions[i].Type == "TXT") {
                            result.data.Questions[i].Disabled = true;
                            result.data.Questions[i].Placeholder = "No answer required for Text";
                        }
                        if (result.data.Questions[i].Type == "BOL") {
                            result.data.Questions[i].Disabled = true;
                            result.data.Questions[i].Placeholder = "Only Yes/No";
                        }
                        if (result.data.Questions[i].Type == "MCR" || result.data.Questions[i].Type == "MCC") {
                            result.data.Questions[i].Disabled = false;
                            result.data.Questions[i].Placeholder = "Seperate multiple answers by comma. eg:-Red,Green";
                        }
                    }
                    $scope.GeneralSet = result.data;
                }, function (ex) {
                    displayPopupNotification("Error retrieving details.Please fix the error and Retry.", "error");
                });
            }
            else if ($routeParams.type == 'Tabular') {
                $('a[href="#tabular-tab"]').click();
                QuestionService.GetTabular($routeParams.setId).then(function (result) {
                    for (let i = 0; i < result.data.Cells.length; i++) {
                        for (let j = 0; j < result.data.Cells[i].length; j++) {
                            if (result.data.Cells[i][j].Type == 'TXT')
                                result.data.Cells[i][j].Desc = "Text";
                            if (result.data.Cells[i][j].Type == 'NUM')
                                result.data.Cells[i][j].Desc = "Number";
                            if (result.data.Cells[i][j].Type == 'LBL')
                                result.data.Cells[i][j].Desc = "Enter label text";
                            if (result.data.Cells[i][j].Type == 'DDL')
                                result.data.Cells[i][j].Desc = "Dropdown list (eg: A,B,C)";
                            if (result.data.Cells[i][j].Type == 'RDB')
                                result.data.Cells[i][j].Desc = "Radio Option (eg: Yes,No)";
                        }
                    }
                    $scope.TabularSet = [];
                    $scope.TabularSet.push(result.data);
                    $scope.TabularSet[0].Rows = result.data.Cells.length;
                    $scope.TabularSet[0].Cols = result.data.Cells[0].length;
                }, function (ex) {
                    displayPopupNotification("Error retrieving details.Please fix the error and Retry.", "error");
                });
            }
            else { }
        }
        var DlgOptions = {
            width: 500,
            //pinned: true,
            resizable: false,
            visible: false,
            title: "Tabular cell type",
            draggable: true,
            actions: ["Close"]
        };
        $scope.dlgTabularWindow.setOptions(DlgOptions);
    });
    $scope.RemoveGeneralQuestion = function (idx) {
        $scope.GeneralSet.Questions.splice(idx, 1);
    }
    $scope.SaveGeneral = function () {
        if (validateGeneral()) {
            $scope.loadingBtn = true;
            QuestionService.AddGeneral($scope.GeneralSet).then(function (result) {
                displayPopupNotification(result.data.MESSAGE, result.data.TYPE);
                if (result.data.TYPE == "success")
                    $scope.ResetGeneral();
                $scope.loadingBtn = false;
            }, function (ex) {
                displayPopupNotification("Error on saving.Please fix the error and Retry.", "error");
                $scope.loadingBtn = false;
            });
        }
        else {
            displayPopupNotification("Input fileds are not valid, Please review form and retry.", "warning");
        }
    }
    var validateGeneral = function () {
        if ($scope.GeneralSet.SetTitle == '' || $scope.GeneralSet.SetType == '')
            return false;
        for (let i = 0; i < $scope.GeneralSet.Questions.length; i++) {
            if ($scope.GeneralSet.Questions[i].Type == '' || $scope.GeneralSet.Questions[i].Question == '')
                return false;
            if ($scope.GeneralSet.Questions[i].Type == 'MCR' || $scope.GeneralSet.Questions[i].Type == 'MCC') {
                if ($scope.GeneralSet.Questions[i].Answer == '')
                    return false;
            }
        }
        return true;
    }
    $scope.GeneralTypeDDL = {
        optionLabel: "--Select--",
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: [
            { Text: "Text", Value: "TXT" },
            { Text: "Yes/No", Value: "BOL" },
            { Text: "Multi-Option One-Answer", Value: "MCR" },
            { Text: "Multi-Option Multi-Answers", Value: "MCC" }
        ]
    };


    $scope.GeneralTypeChange = function (type, i) {
        debugger;
        if (type == 'TXT') {
            $scope.GeneralSet.Questions[i].Placeholder = "No answer required for Text";
            $scope.GeneralSet.Questions[i].Disabled = true;
            $scope.GeneralSet.Questions[i].Answer = '';
        }
        else if (type == 'BOL') {
            $scope.GeneralSet.Questions[i].Placeholder = "Only Yes/No";
            $scope.GeneralSet.Questions[i].Disabled = true;
            $scope.GeneralSet.Questions[i].Answer = '';
        }
        else if (type == 'MCR' || type == 'MCC') {
            $scope.GeneralSet.Questions[i].Placeholder = "Seperate multiple answers by comma. eg:-Red,Green";
            $scope.GeneralSet.Questions[i].Disabled = false;
        }
        else {
            $scope.GeneralSet.Questions[i].Placeholder = "";
            $scope.GeneralSet.Questions[i].Disabled = true;
            $scope.GeneralSet.Questions[i].Answer = '';
        }
    }
    $scope.AddTabularQuestion = function () {
        var len = $scope.TabularSet.length;
        var available = $scope.TabularSet;
        $scope.TabularSet = [];
        $scope.TabularSet.push({
            SetTitle: "",
            SetId: '',
            SetType: '',
            IsActive: 'Y',
            Cols: 2,
            Rows: 2,
            Cells: new Array(new cellDetails(2), new cellDetails(2)),
            WidthPer: parseInt(100 / 2)
        });
        for (let i = 0; i < len; i++)
            $scope.TabularSet.push(available[i]);
    }
    $scope.UpdateWidth = function (item) {
        item.Rows = typeof (item.Rows) == 'undefined' ? item.Cells.length : item.Rows == '' ? 0 : parseInt(item.Rows);
        item.Cols = typeof (item.Cols) == 'undefined' ? item.Cells[0].length : item.Cols == '' ? 0 : parseInt(item.Cols);
        var rowDiff = item.Rows - item.Cells.length;
        var colDiff = item.Cols - item.Cells[0].length;
        if (rowDiff < 0)
            item.Cells.splice(item.Cells.length + rowDiff, -(rowDiff));

        else if (colDiff < 0) {
            for (let i = 0; i < item.Rows; i++) {
                item.Cells[i].splice(item.Cells[i].length + colDiff, -(colDiff));
            }
        }
        else if (rowDiff > 0) {
            for (let i = 0; i < rowDiff; i++) {
                item.Cells.push(new cellDetails(item.Cols));
            }
        }
        else if (colDiff > 0) {
            for (let i = 0; i < item.Rows; i++) {
                var tmpCols = new cellDetails(colDiff)
                item.Cells[i].push.apply(item.Cells[i], new cellDetails(colDiff));
            }
        }
        //item.Cells = [];
        //for (let i = 0; i < item.Rows; i++) {
        //    item.Cells.push(new cellDetails(item.Cols));
        //}
        item.WidthPer = parseInt(100 / item.Cols);
    }
    $scope.SelectTabularType = function (Cell) {
        $scope.TempCell = new Object();
        $scope.TempCell = Cell;
        $scope.tabType = Cell.Type;
        $scope.dlgTabularWindow.center();
        $scope.dlgTabularWindow.open();
    }
    $scope.RemoveTable = function (index) {
        $scope.TabularSet.splice(index, 1);
    }
    $scope.ResetGeneral = function (form) {
        form = typeof (form) == 'undefined' ? $scope.generalForm : form;
        form.$setPristine();
        form.$setUntouched();
        $scope.GeneralSet.Questions = [];
        $scope.GeneralSet.SetTitle = '';
        $scope.GeneralSet.SetId = '';
        $scope.GeneralSet.SetType = '';
        $scope.GeneralSet.Questions.push({
            Type: '',
            Question: '',
            Answer: '',
            Placeholder: '',
            Disabled: true
        });
    }
    $scope.ResetTabular = function (form) {
        form = typeof (form) == 'undefined' ? $scope.TabularForm : form;
        form.$setPristine();
        form.$setUntouched();
        $scope.TabularSet = [];
        $scope.AddTabularQuestion();
    }
    $scope.SetTabularType = function (type) {
        debugger;
        if (type == '') {
            displayPopupNotification("Select a input type.", "warning");
        }
        else {
            $scope.TempCell.Type = type;
            if (type == 'LBL')
                $scope.TempCell.Desc = 'Enter Label text';
            if (type == 'TXT')
                $scope.TempCell.Desc = 'Text';
            if (type == 'NUM')
                $scope.TempCell.Desc = 'Number';
            if (type == 'DDL')
                $scope.TempCell.Desc = 'Dropdown list (eg: A,B,C)';
            if (type == 'RDB')
                $scope.TempCell.Desc = 'Radio Button (eg: Yes,No)';
            $scope.dlgTabularWindow.close();
            $scope.TempCell = new Object();
            $scope.tabType = '';
        }
    }
    $scope.SaveTabular = function () {
        if (validateTabuler()) {
            $scope.loadingBtn = true;
            QuestionService.AddTabular($scope.TabularSet).then(function (result) {
                displayPopupNotification(result.data.MESSAGE, result.data.TYPE);
                if (result.data.TYPE == "success")
                    $scope.ResetTabular();
                $scope.loadingBtn = false;
            }, function (ex) {
                displayPopupNotification("Error on saving.Please fix the error and Retry.", "error");
                $scope.loadingBtn = false;
            });
        }
        else {
            displayPopupNotification("Input fileds are not valid, Please review form and retry.", "warning");
        }
    }
    var validateTabuler = function () {
        for (let i = 0; i < $scope.TabularSet.length; i++) {
            if ($scope.TabularSet[i].SetTitle == '' || $scope.TabularSet[i].SetType == '' || $scope.TabularSet[i].Cells.length == 0 || $scope.TabularSet[i].Cells[0].length == 0)
                return false;
            for (let j = 0; j < $scope.TabularSet[i].Cells.length; j++) {
                for (let k = 0; k < $scope.TabularSet[i].Cells[j].length; k++) {
                    if ($scope.TabularSet[i].Cells[j][k].Type == '')
                        return false;
                    else if ($scope.TabularSet[i].Cells[j][k].Type == 'LBL' && $scope.TabularSet[i].Cells[j][k].Label=='')
                        return false;
                }
            }
        }
        return true;
    }
   
});

distributionModule.controller('QuestionListCtrl', function ($scope, QuestionListService, $routeParams, $http) {

    $scope.distCustomerOptions = {
        dataTextField: "CUSTOMER_EDESC",
        dataValueField: "CUSTOMER_CODE",
        height: 600,
        valuePrimitive: true,
        filter: "contains",
        optionLabel: "Select Customer...",
        //placeholder: "Select Customer...",
        //autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetDistributorResellerList",
                    dataType: "json",
                    type: "POST",
                },
            }
        },
        close: function (e) {
            var selectedName = $(e.sender.input.select()).val();
            $("#customername").val(selectedName);
        },
        change: function (e) {
            var selectedName = $(e.sender.input.select()).val();
            $("#customername").val(selectedName);
        }
    }   

    $scope.mainGridOptions = {
        dataSource: {
            type: "json",
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Setup/GetQuestions"
            },
            pageSize: 100,
            schema: {
                parse: function (items) {
                    var NewDataSource = [];
                    for (var i = 0; i < items.General.length; i++) {
                        var newItem = {
                            Type: 'General',
                            Data: items.General[i].Questions,
                            SetId: items.General[i].SetId,
                            SetTitle: items.General[i].SetTitle,
                            SetType: items.General[i].SetType,
                        }
                        NewDataSource.push(newItem);
                    }
                    for (var i = 0; i < items.Tabular.length; i++) {
                        var celldata = [];
                        var colsLen = 0;
                        for (let j = 0; j < items.Tabular[i].Cells.length; j++) {
                            var tempcells = {};
                            for (let k = 0; k < items.Tabular[i].Cells[j].length; k++) {
                                tempcells["Column" + (k + 1)] = $scope.detailGridHelper(items.Tabular[i].Cells[j][k]);
                                colsLen = k;
                            }
                            celldata.push(tempcells);
                        }
                        var newItem = {
                            Type: 'Tabular',
                            Data: celldata,
                            colsLength:colsLen,
                            SetId: items.Tabular[i].SetId,
                            SetTitle: items.Tabular[i].SetTitle,
                            SetType: items.Tabular[i].SetType,
                        }
                        NewDataSource.push(newItem);
                    }
                    return NewDataSource;
                }
            }
        },
        dataBound: function (e) {
            GetSetupSetting("QuestionList");
        },
        height: window.innerHeight - 150,
        sortable: true,
        pageable: true,
        columns: [{
            field: "SetTitle",
            title: "Set Title",
            width: "50%"
        }, {
            field: "Type",
            title: "Questionnaire Type",
            width: "20%"
        }, {
            field: "SetType",
            title: "Set Type",
            width: "10%"
        }, {
            title: "Actions",
            attributes: {
                style: "text-align: center;"
            },
            template: function (dataItem) {
                return kendo.template($("#action-template").html())(dataItem);
            },
        }]
    };

    $scope.detailGridOptions = function (dataItem) {
        var cols = [];
        if (dataItem.Type == 'General') {
            cols = [{ field: "Type", title: "Type" },
                    { field: "Question", title: "Question" },
                    { field: "Answer", title: "Answers" }]
        }
        else {
            var wid = parseInt(100 / dataItem.colsLength) + "%";
            for (let i = 0; i <= dataItem.colsLength; i++) {
                cols.push({
                    field: "Column" + (+i + 1),
                    title: "Column" + (+i + 1),
                    encoded:false,
                    width: wid
                });
            }
        }
        return {
            dataSource: { data: dataItem.Data, pageSize: 100 },
            scrollable: false,
            sortable: true,
            pageable: true,
            columns: cols,
        }
    };

    $scope.detailGridHelper = function (Item) {
        var template = "";
        if (Item.Type == "LBL") {
            template = '<b>' + Item.Label + '</b>';
        }
        else if (Item.Type == "DDL") {
            if (Item.Label == null)
                return template = "";
            var items = Item.Label.split(",");
            var _select = $('<select>');
            for (var i = 0; i < items.length; i++) {
                _select.append(
                    $('<option></option>').val(i+1).html(items[i])
                );
            }
            template = '<select>'+_select.html()+'</select>';
        }
        else if (Item.Type == "RDB") {
            if (Item.Label == null)
                return template = "";
            var items = Item.Label.split(",");
            var _html = "";
            for (var i = 0; i < items.length; i++) {
                _html.append(' <input type="radio" name="RDB" value="male" checked> ' + items[i] + '<br>');
                }
            template = _html.html();
        }
        else if (Item.Type == "TXT") {
            template = '<input type="text" placeholder="Text field" style="width: 99%;" disabled>';
        }
        else if (Item.Type == "NUM") {
            template = '<input type="text" placeholder="Number field" style="width: 99%;" disabled>';
        }
        else if (Item.Type == "CHK") {
            template = '<input type="checkbox" style="width: 99%;" disabled>';
        }
        else if (Item.Type == "FILE") {
            template = '<input type="file" disabled>';
        }
        return template;
    }


    $http.get('/api/Setup/GetWebQAList')
        .then(function (response) {
            $scope.webQA = response.data;
            angular.forEach(response.data, function (v) {
                if (v.QA_TYPE == "MCR")
                {
                    if (v.ANSWERS !== null || v.ANSWERS !== undefined) {
                        var answerArr = v.ANSWERS.split(",");
                        v["MCR_ARR"] = answerArr;
                    }
                }
                if (v.QA_TYPE == "MCC") {
                    if (v.ANSWERS !== null || v.ANSWERS !== undefined) {
                        var answerArr = v.ANSWERS.split(",");
                        v["MCC_ARR"] = answerArr;
                    }

                }
            });

        })
        .catch(function (response) {
            console.error('error', response.status, response.data);
        })
        .finally(function () {
            console.log("finally finished");
        });

    $scope.messagetemplateOptions = {
        dataTextField: "NAME",
        dataValueField: "ID",
        height: 600,
        valuePrimitive: true,
        filter: "contains",
        optionLabel: "Select Template...",
        dataBound: function (e) {
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
        dataSource: {
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/GetMessageTemplateList",
                    dataType: "json",
                    type: "POST",
                },
            }
        },
        close: function (e) {
            var name = e.sender.dataItem().NAME;
            if (!name) 
                return displayPopupNotification("Please choose the Template", "error");
            
            var getUrl = window.location.protocol + "//" + window.location.host + "/api/Setup/GetTemplates?name="+name+".html";
            $.post(getUrl)
                .done((data) => {
                    $("#result").html("");
                    $("#result").html(data);
                });
        },
        change: function (e) {

        }
    }   

});