DTModule.controller('accChildCtrl', function ($scope, $rootScope, $http, $routeParams, $window, $filter) {
    $scope.childRowIndex;
    //show modal popup

    var getChildAccountCodeByUrl = window.location.protocol + "//" + window.location.host + "/api/TemplateApi/getAccountCode";
    $scope.childAccounttreeData = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: getChildAccountCodeByUrl,
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: function (data, evt) {
                }
            },

        },
        schema: {
            parse: function (data) {
                return data;
            },
            model: {
                id: "MASTER_ACC_CODE",
                parentId: "PRE_ACC_CODE",
                children: "Items",
                fields: {
                    ACC_CODE: { field: "ACC_CODE", type: "string" },
                    ACC_EDESC: { field: "ACC_EDESC", type: "string" },
                    parentId: { field: "PRE_ACC_CODE", type: "string", defaultValue: "00" },
                }
            }
        }
    });

    ////treeview expand on startup
    //$scope.onChildAccountDataBound = function () {
    //    if ($scope.childRowIndex == undefined)
    //        $('#fromlocationtree').data("kendoTreeView").expand('.k-item');
    //}

    //treeview on select
    $scope.subaccountoptions = {
        loadOnDemand: false,
        select: function (e) {
            $('.topsearch').show();
            var currentItem = e.sender.dataItem(e.node);
            $('#accountGrid_' + $scope.childRowIndex).removeClass("show-displaygrid");
            $("#accountGrid_" + $scope.childRowIndex).html("");
            BindaccountchildGrid(currentItem.AccountId, currentItem.masterAccountCode, "");
            $scope.$apply();
        },
    };

    //search whole data on search button click
    $scope.BindSearchGrid = function () {
        $scope.searchText = $scope.SubLedgertxtSearchString;
        BindaccountchildGrid("", "", $scope.searchText);
    }

    //Grid Binding main Part
    function BindaccountchildGrid(accId, accMasterCode, searchText) {
        $scope.accountchildGridOptions = {
            dataSource: {
                /*type: "json",*/
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                transport: {
                    read: "/api/TemplateApi/GetAccountListByAccountCode?accId=" + accId + '&accMastercode=' + accMasterCode + '&searchText=' + searchText,
                },
                schema: {
                    /*type: "json",*/
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    model: {
                        fields: {
                            ACC_CODE: { type: "string" },
                            ACC_EDESC: { type: "string" }
                        }
                    }
                },
                pageSize: 30,

            },
            scrollable: true,
            sortable: true,
            resizable: true,
            pageable: true,
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
                        startswith: "Starts with",
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
            columnMenuInit: function (e) {
                wordwrapmenu(e);
            },
            dataBound: function (e) {
                debugger;
                $("#accountGrid_" + $scope.childRowIndex + " tbody tr").css("cursor", "pointer");
                $("#accountGrid_" + $scope.childRowIndex + " tbody tr").on('dblclick', function () {
                    debugger;
                    var subaccountcode = $(this).find('td span').html();
                    //var subaccName = $(this).find('td span[ng-bind="dataItem.ACC_EDESC"]').html();
                    var subaccName = $(this).find('td span[ng-bind="dataItem.ACC_EDESC"]')[0].innerText;
                    //var accName = $(this).find('td span[ng-bind="dataItem.ACC_EDESC"]')[0].innerText;
                    $("#idaccount_" + $scope.childRowIndex).data('kendoComboBox').dataSource.data([{ ACC_CODE: subaccountcode, ACC_EDESC: subaccName, Type: "code" }]);
                    $scope.childModels[$scope.childRowIndex]["ACC_CODE"] = subaccountcode;
                    if ($($(".caccount_" + $scope.childRowIndex)[0]).closest('div').hasClass('borderRed')) {
                        $($(".caccount_" + $scope.childRowIndex)[0]).closest('div').removeClass('borderRed')
                    }
                    $('#AccountModal_' + $scope.childRowIndex).modal('toggle');

                    var index = $scope.childRowIndex;
                    $rootScope.childRowIndexacc = $scope.childRowIndex;
                    var accCode = subaccountcode;
                    window.accCode = accCode;
                    window.globalIndex = index;
                    //$scope.validaccsum($scope.childRowIndex);
                    //---------------------------------------------------------------------------------------------------------------------
                    if ($scope.childModels[index].AMOUNT == $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT) {
                        $scope.dynamicSubLedgerModalData[index].REMAINING_AMOUNT = 0;
                        $scope.dynamicSubLedgerModalData[index].SUBLEDGER_AMOUNT = $scope.childModels[index].AMOUNT;
                        $('.remainingamt').removeClass("borderred");
                    }
                    if (accCode === $scope.dynamicSubLedgerModalData[index].ACC_CODE) {
                        $scope.dynamicSubLedgerModalData[index] = $scope.dynamicSubLedgerModalData[index];
                        $scope.dynamicModalData[index] = $scope.dynamicModalData[index];
                    } else {

                        $scope.dynamicSubLedgerModalData[index].SUBLEDGER = [{
                            SERIAL_NO: index + 1,
                            SUB_CODE: "",
                            SUB_EDESC: "",
                            AMOUNT: "",
                            PARTICULARS: "",
                            REFRENCE: ""
                        }];
                        $scope.dynamicModalData[index].BUDGET = [{
                            SERIAL_NO: index + 1,
                            BUDGET_CODE: "",
                            AMOUNT: "",
                            NARRATION: ""
                        }];
                    }

                    $scope.childModels[index]["TRANSACTION_TYPE"] = $scope.childModels[index].TRANSACTION_TYPE;
                    $scope.transactiontype = $scope.childModels[index].TRANSACTION_TYPE.toString();
                    $scope.dynamicSubLedgerModalData[index].ACC_CODE = accCode;

                    var response = $http.get('/api/TemplateApi/getSubledgerCodeByAccCode?accCode=' + accCode);
                    response.then(function (res) {
                        if (res.data != "0") {
                            $(".dynamicSubLedgerModal_" + index).modal('toggle');
                            $(".subledger_transaction_type").val($scope.transactiontype);
                        }
                    });
                    var response = $http.get('/api/TemplateApi/IfIsTdsByAccCode?accCode=' + accCode);
                    response.then(function (res) {

                        if (res.data != "0") {
                            $(".dynamictdsModal_" + index).modal('toggle');
                            //$scope.dynamicTDSModalData[index].ACC_CODE = accCode;
                            //popupAccessTds = true;
                            //$scope.popUpTds(index);

                        }
                    });
                    var response = $http.get('/api/TemplateApi/IfIsVATByAccCode?accCode=' + accCode);

                    response.then(function (res) {
                        if (res.data != "0") {
                            $(".dynamicVATModal_" + index).modal('toggle');
                            //$scope.dynamicVATModalData[index].ACC_CODE = accCode;
                            //popupAccessVAT = true;
                            //$scope.popUpVAT(index);

                        }
                    });
                    var response = $http.get('/api/TemplateApi/getBudgetCodeCountCodeByAccCode?accCode=' + accCode);
                    response.then(function (res) {
                        if (res.data != "0") {
                            $(".subledgerModal_" + index).modal('toggle');
                            //$scope.dynamicModalData[index].ACC_CODE = accCode;
                            //popupAccessBudget = true;
                            //$scope.popUpBudget(index);

                        }
                    });

                    //---------------------------------------------------------------------------------------------------------------------------
                    $scope.$apply();

                    //var len = (parseInt(index) * 2) + 1;
                    //$($(".subledgerfirst:input")[len]).data('kendoComboBox').dataSource.read();
                    //$($(".subledgersecond:input")[len]).data('kendoComboBox').dataSource.read();

                    var first = $(".subledgerfirst:input");
                    $.each(first, function (i, obj) {
                        obj = $(obj);
                        if (!_.isEmpty(obj.data('kendoComboBox'))) {
                            obj.data('kendoComboBox').dataSource.read();
                        }
                    });
                    var second = $(".subledgersecond:input");
                    $.each(second, function (i, obj) {
                        obj = $(obj);
                        if (!_.isEmpty(obj.data('kendoComboBox'))) {
                            obj.data('kendoComboBox').dataSource.read();
                        }
                    });
                    var a = $(this).find('td span[ng-bind="dataItem.ACC_NATURE"]').html();
                    var response = $http.get('/api/ContraVoucherApi/GetPurchaseExpensesFlag?formCode=' + $scope.formcode);
                    response.then(function (res) {
                        debugger;
                        if (res.data[0] == 'Y') {
                            if (a == 'SB') {
                                $("#PurchaseExpSheet").modal('toggle');
                                $rootScope.ACC_CODE = accCode;
                            }
                        }
                    });
                })
            },

            columns: [{
                field: "ACC_CODE",
                //hidden: true,
                title: "Code",

            }, {
                field: "ACC_EDESC",
                title: "Account Name",

            },
            {
                field: "TRANSACTION_TYPE",
                title: "Transaction Type",

            },
            {
                field: "ACC_NATURE",
                title: "Account Nature",

            },
            {
                field: "CREATED_BY",
                title: "Created By",
                width: "120px"
            }, {
                field: "CREATED_DATE",
                title: "Created Date",
                template: "#= kendo.toString(CREATED_DATE,'dd MMM yyyy') #",
                //template: "#= kendo.toString(kendo.parseDate(CREATED_DATE, 'yyyy-MM-dd'), 'dd MMM yyyy') #",

            },


            ]
        };
    }


    //show modal popup
    $scope.BrowseTreeListForChildAccountCode = function (index) {
        if ($scope.havRefrence == 'Y' && $scope.freeze_master_ref_flag == "Y") {
            var referencenumber = $('#refrencetype').val();
            if ($scope.ModuleCode != '01' && referencenumber !== "") {
                return;
            }
        }
        if ($scope.freeze_master_ref_flag == "N") {
            $scope.childRowIndex = index;
            document.popupindex = index;
            $('#AccountModal_' + index).modal('show');
            if ($('#accounttree_' + $scope.childRowIndex).data("kendoTreeView") != undefined)
                $('#accounttree_' + $scope.childRowIndex).data("kendoTreeView").expand('.k-item');
        }

    }

    $scope.acccode_Cancel = function (e) {
        $('#AccountModal_' + e).modal('hide');
    }
    $scope.show = function (e) {
        debugger;
        $rootScope.ACC_CODE = e.dataItem.ACC_CODE;
        $rootScope.childRowIndexacc = this.$index;
        var response = $http.get('/api/ContraVoucherApi/GetPurchaseExpensesFlag?formCode=' + $scope.formcode);
        //old code
        //response.then(function (res1) {

        //    var response = null;
        //    if (res1.data[0] == 'Y') {
        //        response = $http.get('/api/TemplateApi/GetAccountListByAccountCode?accId=null&accMastercode=niraj&searchText=' + e.dataItem.ACC_EDESC);                    
        //    }
        //    else {
        //        response = $http.get('/api/TemplateApi/GetAccountListByAccountCode?accId=null&accMastercode=&searchText=' + e.dataItem.ACC_EDESC);
        //    }
        //    response.then(function (res2) {
        //        if (res2.data.length > 0) {
        //            if (res2.data[0].ACC_NATURE == 'SB') {
        //                $("#PurchaseExpSheet").modal('toggle');
        //            }
        //        }
        //    });
        //});
        //subin change
        response.then(function (res1) {

            var response = null;
            if (res1.data[0] == 'Y') {
                response = $http.get('/api/TemplateApi/GetAccountListByAccountCode?accId=null&accMastercode=niraj&searchText=' + e.dataItem.ACC_EDESC);
            }
            else {
                response = $http.get('/api/TemplateApi/GetAccountListByAccountCode?accId=null&accMastercode=&searchText=' + e.dataItem.ACC_EDESC);
            }
            response.then(function (res2) {
                if (res2.data.length > 0) {
                    if (res2.data[0].ACC_NATURE == 'SB') {
                        $("#PurchaseExpSheet").modal('toggle');
                    }
                }
            });
        });
    }
});

// ===== Account Info Tooltip - Pure jQuery (outside Angular controller) =====
(function () {
    var cache = {};
    var $tip = null;
    var hideTimer = null;
    // Tracks hovered account data per child row index: { index: { accCode, ACC_EDESC, GENERIC_BL_AMT, TRANSACTION_TYPE } }
    var trackedAccounts = {};

    function ensureTooltip() {
        if (!$tip) {
            $tip = $('<div></div>').css({
                position: 'fixed',
                zIndex: 99999,
                display: 'none',
                backgroundColor: '#fff',
                border: '2px solid #333',
                borderRadius: '4px',
                boxShadow: '0 4px 12px rgba(0,0,0,0.3)',
                minWidth: '350px',
                maxWidth: '420px',
                padding: '0',
                overflow: 'auto',
                maxHeight: '400px'
            }).addClass('account-info-tooltip').appendTo('body');
            $tip.on('mouseenter', function () {
                if (hideTimer) { clearTimeout(hideTimer); hideTimer = null; }
            });
            $tip.on('mouseleave', function () {
                hideTimer = setTimeout(function () { $tip.hide(); }, 300);
            });
        }
        return $tip;
    }

    function getAccTypeLabel(flag) {
        if (!flag) return 'N/A';
        var map = { 'B': 'Balance Sheet Account', 'T': 'Trading Account', 'P': 'Profit & Loss Account' };
        return map[flag] || flag;
    }

    function formatAmount(val) {
        var num = parseFloat(val || 0);
        return num.toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function buildHtml(data, currentIndex) {
        var tdL = 'style="padding:4px 10px;border:1px solid #bbb;font-weight:600;color:#333;background:#eee;width:45%;"';
        var tdR = 'style="padding:4px 10px;border:1px solid #bbb;color:#333;background:#fff;"';
        var tbl = 'style="width:100%;border-collapse:collapse;font-size:12px;"';

        // Header: Current Account Info
        var h = '<div style="padding:6px 10px;font-weight:bold;font-size:13px;text-align:center;color:#333;">Current Account Info...</div>';

        // Dynamic accumulated account bars — show all tracked accounts
        var sortedKeys = Object.keys(trackedAccounts).sort(function (a, b) { return parseInt(a) - parseInt(b); });
        for (var k = 0; k < sortedKeys.length; k++) {
            var idx = sortedKeys[k];
            var entry = trackedAccounts[idx];
            var isCurrent = (String(idx) === String(currentIndex));
            var bgColor = isCurrent ? '#00BCD4' : '#78909C';
            var borderStyle = isCurrent ? 'border-left:4px solid #FFD600;' : 'border-left:4px solid transparent;';

            h += '<div style="background:' + bgColor + ';padding:5px 10px;font-weight:bold;font-size:12px;color:#fff;' + borderStyle + '">' +
                '<table style="width:100%;border-collapse:collapse;">' +
                '<tr>' +
                '<td style="text-align:left;color:#fff;font-weight:bold;width:15px;opacity:0.7;font-size:10px;">R' + (parseInt(idx) + 1) + '</td>' +
                '<td style="text-align:left;color:#fff;font-weight:bold;">' + (entry.ACC_EDESC || 'N/A') + '</td>' +
                '<td style="text-align:right;color:#fff;font-weight:bold;">' + formatAmount(entry.GENERIC_BL_AMT) + ' ' + (entry.TRANSACTION_TYPE || '') + '</td>' +
                '</tr>' +
                '</table></div>';
        }

        // Account details table — show ONLY the currently hovered account's full details
        var txnType = data.TRANSACTION_TYPE || '';
        var accCodeId = (data.ACC_CODE || 'N/A') + ' / [ ' + (data.ACC_ID || 'N/A') + ' ]';
        h += '<table ' + tbl + '>' +
            '<tr><td ' + tdL + '>Acc Code / ID</td><td ' + tdR + '>' + accCodeId + '</td></tr>' +
            '<tr><td ' + tdL + '>Belongs to</td><td ' + tdR + '>' + getAccTypeLabel(data.ACC_TYPE_FLAG) + '</td></tr>' +
            '<tr><td ' + tdL + '>Created By</td><td ' + tdR + '>' + (data.CREATED_BY || 'N/A') + '</td></tr>' +
            '<tr><td ' + tdL + '>Created Date</td><td ' + tdR + '>' + (data.CREATED_DATE || 'N/A') + '</td></tr>' +
            '<tr><td ' + tdL + '>Modify By</td><td ' + tdR + '>' + (data.MODIFY_BY || 'N/A') + '</td></tr>' +
            '<tr><td ' + tdL + '>Modify Date</td><td ' + tdR + '>' + (data.MODIFY_DATE || 'N/A') + '</td></tr>' +
            '</table>';

        // Accounts Historical Data
        h += '<div style="padding:5px 10px;font-weight:bold;font-style:italic;font-size:13px;color:#1a8a4a;">Accounts Historical Data</div>';
        h += '<table ' + tbl + '>' +
            '<tr><td ' + tdL + '>Generic B/L Amount</td><td ' + tdR + '>' + formatAmount(data.GENERIC_BL_AMT) + ' ' + txnType + '</td></tr>' +
            '<tr><td ' + tdL + '>Posted B/L Amount</td><td ' + tdR + '>' + formatAmount(data.POSTED_BL_AMT) + ' ' + txnType + '</td></tr>' +
            '</table>';

        return h;
    }

    function positionTooltip(tip, el) {
        var rect = el.getBoundingClientRect();
        var tipW = tip.outerWidth() || 380;
        var tipH = tip.outerHeight() || 300;
        var winW = window.innerWidth;
        var winH = window.innerHeight;
        var top, left;

        var spaceBelow = winH - rect.bottom;
        var spaceAbove = rect.top;

        if (spaceBelow >= tipH + 5) {
            top = rect.bottom + 5;
        } else if (spaceAbove >= tipH + 5) {
            top = rect.top - tipH - 5;
        } else {
            top = Math.max(5, Math.min(rect.top, winH - tipH - 5));
            left = rect.right + 10;
            if (left + tipW > winW) left = rect.left - tipW - 10;
            if (left < 5) left = 5;
            tip.css({ top: top + 'px', left: left + 'px' });
            return;
        }

        left = rect.left;
        if (left + tipW > winW) left = winW - tipW - 10;
        if (left < 5) left = 5;
        tip.css({ top: top + 'px', left: left + 'px' });
    }

    // Expose cleanup function for child row deletion (called from FormTemplateCtrl.js)
    window.removeTrackedAccount = function (deletedIndex) {
        deletedIndex = parseInt(deletedIndex);
        // Remove the deleted entry
        delete trackedAccounts[deletedIndex];

        // Re-index: shift all entries with index > deletedIndex down by 1
        var newTracked = {};
        var keys = Object.keys(trackedAccounts);
        for (var i = 0; i < keys.length; i++) {
            var oldIdx = parseInt(keys[i]);
            if (oldIdx > deletedIndex) {
                newTracked[oldIdx - 1] = trackedAccounts[oldIdx];
            } else {
                newTracked[oldIdx] = trackedAccounts[oldIdx];
            }
        }
        trackedAccounts = newTracked;
    };

    $(document).on('mouseenter', '.account-info-hover', function () {
        if (hideTimer) { clearTimeout(hideTimer); hideTimer = null; }

        var el = this;
        var accCode = $(el).attr('data-acc-code');
        var accIndex = $(el).attr('data-acc-index');
        if (!accCode) return;

        var tip = ensureTooltip();

        if (cache[accCode]) {
            var d = cache[accCode];
            // Track this row's data
            trackedAccounts[accIndex] = {
                accCode: d.ACC_CODE,
                ACC_EDESC: d.ACC_EDESC,
                GENERIC_BL_AMT: d.GENERIC_BL_AMT,
                TRANSACTION_TYPE: d.TRANSACTION_TYPE
            };
            tip.html(buildHtml(d, accIndex)).show();
            positionTooltip(tip, el);
            return;
        }

        tip.html('<div style="padding:20px;text-align:center;"><i class="fa fa-spinner fa-spin" style="font-size:24px;color:#3498db;"></i><p>Loading...</p></div>').show();
        positionTooltip(tip, el);

        $.get('/api/TemplateApi/GetAccountInfoData?accCode=' + encodeURIComponent(accCode), function (data) {
            cache[accCode] = data;
            // Track this row's data
            trackedAccounts[accIndex] = {
                accCode: data.ACC_CODE,
                ACC_EDESC: data.ACC_EDESC,
                GENERIC_BL_AMT: data.GENERIC_BL_AMT,
                TRANSACTION_TYPE: data.TRANSACTION_TYPE
            };
            tip.html(buildHtml(data, accIndex));
            positionTooltip(tip, el);
        }).fail(function () {
            tip.html('<div style="padding:20px;text-align:center;color:#e74c3c;"><i class="fa fa-exclamation-triangle"></i><p>Error loading info</p></div>');
            positionTooltip(tip, el);
        });
    });

    $(document).on('mouseleave', '.account-info-hover', function () {
        hideTimer = setTimeout(function () {
            ensureTooltip().hide();
        }, 300);
    });
})();