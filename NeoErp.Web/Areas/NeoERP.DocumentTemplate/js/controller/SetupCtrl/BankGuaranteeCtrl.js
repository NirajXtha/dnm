DTModule.controller('BankGuaranteeCtrl', function ($scope, $filter, $window, $timeout, $http, $q) {
    $scope.FormName = "Bank Guarantee";
    $scope.FROM_DATE = "";
    $scope.TO_DATE = "";
    $scope.EXPIRY_FILTER = "Active Only";
    $scope.RECORD_FILTER = "All Records";
    $scope.HIDE_COMPANY = false;
    $scope.companyInfo = null;
    $scope.reportDateRange = "";
    $scope.grandTotal = {
        BG_AMOUNT: 0,
        DEPOSIT: 0,
        TOTAL_SECURITY: 0
    };
    $scope.branchList = [];
    $scope.selectedBranches = [];
    $scope.selectAllBranches = false;

    $scope.ConvertEngToNep = function () {
        var engdate = $("#englishDate5").val();
        var nepalidate = ConvertEngDateToNep(engdate);
        $("#nepaliDate5").val(nepalidate);
        $("#nepaliDate51").val(nepalidate);
    };

    $scope.ConvertNepToEng = function ($event) {
        var date = BS2AD($("#nepaliDate5").val());
        var date1 = BS2AD($("#nepaliDate51").val());
        $("#englishdatedocument").val($filter('date')(date, "dd-MMM-yyyy"));
        $("#englishdatedocument1").val($filter('date')(date1, "dd-MMM-yyyy"));
        $('#nepaliDate5').trigger('change')
        $('#nepaliDate51').trigger('change')
    };

    $scope.ConvertEngToNepang = function (data) {
        $("#nepaliDate5").val(AD2BS(data));
    };
    $scope.ConvertEngToNepang1 = function (data) {
        $("#nepaliDate51").val(AD2BS(data));
    }

    $scope.someDateFn = function () {
        var engdate = $filter('date')(new Date(new Date().setDate(new Date().getDate() - 1)), 'dd-MMM-yyyy');
        var a = ConvertEngDateToNep(engdate);
        $scope.Dispatch_From = engdate;
        $scope.NepaliDate = a;
        $scope.Dispatch_To = a;
        $scope.PlanningTo = ConvertEngDateToNep($filter('date')(new Date(new Date().setDate(new Date().getDate())), 'dd-MMM-yyyy'));
    };

    $scope.someDateFn();

    $scope.monthSelectorOptionsSingle = {
        value: new Date(),
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepang(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.monthSelectorOptionsSingle1 = {
        open: function () {
            var calendar = this.dateView.calendar;
            calendar.wrapper.width(this.wrapper.width() - 6);
        },
        change: function () {
            $scope.ConvertEngToNepang1(kendo.toString(this.value(), 'yyyy-MM-dd'))
        },
        format: "dd-MMM-yyyy",
        dateInput: true
    };

    $scope.BindBankGuaranteeGrid = function () {
        var grid = $("#bankGuaranteeGrid").data("kendoGrid");

        if (grid) {
            grid.dataSource.read();
        }
    };

    $scope.toggleCompanyHeader = function () {
        // This will automatically show/hide the company header based on HIDE_COMPANY value
    };

    // Load company information
    $scope.loadCompanyInfo = function () {
        $http.get('/api/SetupApi/GetCompanyInfo').then(function (res) {
            if (res.data && res.data.DATA) {
                $scope.companyInfo = res.data.DATA;
            }
        });
    };

    $scope.calculateGrandTotal = function (data) {
        var total = {
            BG_AMOUNT: 0,
            DEPOSIT: 0,
            TOTAL_SECURITY: 0
        };

        if (data && data.length > 0) {
            data.forEach(function (item) {
                total.BG_AMOUNT += parseFloat(item.BG_AMOUNT || 0);
                total.DEPOSIT += parseFloat(item.DEPOSIT || 0);
                total.TOTAL_SECURITY += parseFloat(item.TOTAL_SECURITY || 0);
            });
        }

        $scope.grandTotal = total;
    };

    // Branch Selection Functions
    $scope.loadBranches = function () {
        return $http.get('/api/SetupApi/GetCompanyBranches').then(function (res) {
            if (res.data && res.data.DATA) {
                var branches = res.data.DATA;
                var companyMap = {};
                
                // Group branches by company
                branches.forEach(function (branch) {
                    if (!companyMap[branch.COMPANY_CODE]) {
                        companyMap[branch.COMPANY_CODE] = {
                            COMPANY_CODE: branch.COMPANY_CODE,
                            COMPANY_NAME: branch.COMPANY_NAME,
                            selected: false,
                            expanded: true,
                            branches: []
                        };
                    }
                    companyMap[branch.COMPANY_CODE].branches.push({
                        BRANCH_CODE: branch.BRANCH_CODE,
                        BRANCH_NAME: branch.BRANCH_NAME,
                        selected: false
                    });
                });
                
                $scope.branchList = Object.values(companyMap);
                return $scope.branchList;
            } else {
                $scope.branchList = [];
                return [];
            }
        }, function(error) {
            $scope.branchList = [];
            return [];
        });
    };

    $scope.openBranchModal = function () {
        $scope.branchList = [];
        $scope.loadBranches().then(function() {
            $('#branchSelectionModal').modal('show');
        });
    };

    $scope.toggleSelectAll = function () {
        $scope.branchList.forEach(function (company) {
            company.selected = $scope.selectAllBranches;
            company.branches.forEach(function (branch) {
                branch.selected = $scope.selectAllBranches;
            });
        });
    };

    $scope.toggleCompanyBranches = function (company) {
        company.branches.forEach(function (branch) {
            branch.selected = company.selected;
        });
        $scope.updateBranchSelection();
    };

    $scope.updateBranchSelection = function () {
        // Check if all branches are selected
        var allSelected = true;
        $scope.branchList.forEach(function (company) {
            company.branches.forEach(function (branch) {
                if (!branch.selected) {
                    allSelected = false;
                }
            });
            // Update company checkbox based on its branches
            var allBranchesSelected = company.branches.every(function (b) { return b.selected; });
            company.selected = allBranchesSelected;
        });
        $scope.selectAllBranches = allSelected;
    };

    $scope.applyBranchSelection = function () {
        $scope.selectedBranches = [];
        $scope.branchList.forEach(function (company) {
            company.branches.forEach(function (branch) {
                if (branch.selected) {
                    $scope.selectedBranches.push(branch.BRANCH_CODE);
                }
            });
        });
        $('#branchSelectionModal').modal('hide');
        if (typeof displayPopupNotification === 'function') {
            displayPopupNotification($scope.selectedBranches.length + ' branch(es) selected', 'success');
        }
    };

    $timeout(function () {
        let startDate, endDate;
        let today = moment();
        endDate = today.toDate();
        
        // Set to start of year for Bank Guarantee
        let todayNepaliDateForYear = ConvertEngDateToNep(moment().format('DD-MMM-YYYY'));
        let firstDayOfYearNepali = todayNepaliDateForYear.substring(0, 4) + '-01-01';
        startDate = BS2AD(firstDayOfYearNepali);

        $("#englishdatedocument").data("kendoDatePicker").value(startDate);
        $("#englishdatedocument1").data("kendoDatePicker").value(endDate);

        let nepaliFromDate = ConvertEngDateToNep(moment(startDate).format('DD-MMM-YYYY'));
        let nepaliToDate = ConvertEngDateToNep(moment(endDate).format('DD-MMM-YYYY'));

        $("#nepaliDate5").val(nepaliFromDate);
        $("#nepaliDate51").val(nepaliToDate);

        $scope.loadCompanyInfo();
    }, 200)

    var record = 0;

    $scope.BankGuaranteeGridOptions = {
        dataSource: new kendo.data.DataSource({
            transport: {
                read: {
                    url: "/api/SetupApi/GetBankGuaranteeList",
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json"
                },
                parameterMap: function (options, type) {
                    if (type === "read") {
                        let fromDate = $(".fromDate input").val();
                        let toDate = $(".toDate input").val();
                        let expiryFilter = $("#expiryFilter").val();
                        let recordFilter = $("#recordFilter").val();
                        
                        // Format branch codes for SQL IN clause
                        var branchCodes = '';
                        if ($scope.selectedBranches && $scope.selectedBranches.length > 0) {
                            branchCodes = $scope.selectedBranches.map(function(code) {
                                return "'" + code + "'";
                            }).join(',');
                        }

                        var combinedOptions = angular.extend({}, options, {
                            FROM_DATE: $scope.toDate(fromDate),
                            TO_DATE: $scope.toDate(toDate),
                            EXPIRY_FILTER: expiryFilter,
                            RECORD_FILTER: recordFilter,
                            BRANCH_CODES: branchCodes
                        });

                        return JSON.stringify(combinedOptions);
                    }
                    return options;
                }
            },
            autoBind: false,
            schema: {
                data: function(response) {
                    var data = response.DATA || [];
                    
                    // Calculate grand total
                    var grandTotal = {
                        BG_AMOUNT: 0,
                        DEPOSIT: 0,
                        TOTAL_SECURITY: 0
                    };
                    
                    data.forEach(function (item) {
                        grandTotal.BG_AMOUNT += parseFloat(item.BG_AMOUNT || 0);
                        grandTotal.DEPOSIT += parseFloat(item.DEPOSIT || 0);
                        grandTotal.TOTAL_SECURITY += parseFloat(item.TOTAL_SECURITY || 0);
                    });
                    
                    // Always add grand total row
                    var grandTotalRow = {
                        SR_NO: null,
                        BG_NO: '<strong>Grand Total:</strong>',
                        PARTY_NAME: '',
                        ADDRESS: '',
                        PARTY_TYPE_EDESC: '',
                        AREA_EDESC: '',
                        BG_AMOUNT: grandTotal.BG_AMOUNT,
                        ISSUING_BANK: '',
                        BG_DATE: null,
                        BG_MITI: '',
                        VALIDITY_DATE: null,
                        VALIDITY_MITI: '',
                        EXPAIRY_DUE_DAYS: null,
                        DEPOSIT: grandTotal.DEPOSIT,
                        TOTAL_SECURITY: grandTotal.TOTAL_SECURITY,
                        REMARKS: '',
                        isGrandTotal: true
                    };
                    
                    data.push(grandTotalRow);
                    return data;
                },
                total: "TOTAL"
            },
            serverPaging: false,
            serverSorting: false,
            serverFiltering: false,
            pageSize: 50,
            sort: {
                field: "END_DATE",
                dir: "asc"
            }
        }),
        autoBind: false,
        selectable: "single",
        scrollable: true,
        height: 450,
        sortable: true,
        pageable: {
            refresh: true,
            pageSizes: [10, 20, 50, 100, "all"],
            buttonCount: 5
        },
        resizable: true,
        dataBound: function (e) {
            record = 0;
            $("#bankGuaranteeGrid tbody tr").css("cursor", "pointer");
            
            var grid = this;
            var data = grid.dataSource.data();
            
            $scope.$apply(function () {
                // Calculate grand total excluding the grand total row
                var actualData = data.filter(function(item) { return !item.isGrandTotal; });
                $scope.calculateGrandTotal(actualData);
                
                // Update report date range
                let fromDate = $(".fromDate input").val();
                let toDate = $(".toDate input").val();
                let fromMiti = $("#nepaliDate5").val();
                let toMiti = $("#nepaliDate51").val();
                $scope.reportDateRange = fromDate + " [" + fromMiti + "] to " + toDate + " [" + toMiti + "]";
            });

            // Apply row styling
            grid.tbody.find('tr').each(function () {
                var dataItem = grid.dataItem(this);
                if (dataItem) {
                    if (dataItem.isGrandTotal) {
                        $(this).addClass('grand-total-row');
                        $(this).find('td').css('font-weight', 'bold');
                    } else if (dataItem.EXPAIRY_DUE_DAYS < 0) {
                        $(this).addClass('expired-row');
                    }
                }
            });
        },
        columns: [
            {
                field: "SR_NO",
                title: "Sr No.",
                width: "60px",
                template: function(dataItem) {
                    if (dataItem.isGrandTotal) {
                        return '';
                    }
                    return ++record;
                }
            },
            {
                field: "BG_NO",
                title: "BG No",
                width: "100px",
                encoded: false
            },
            {
                field: "PARTY_NAME",
                title: "Party Name",
                width: "200px"
            },
            {
                field: "ADDRESS",
                title: "Address",
                width: "200px"
            },
            {
                field: "PARTY_TYPE_EDESC",
                title: "Dealer Name",
                width: "150px"
            },
            {
                field: "AREA_EDESC",
                title: "Area",
                width: "120px"
            },
            {
                field: "BG_AMOUNT",
                title: "BG Amount",
                width: "120px",
                format: "{0:n2}",
                attributes: {
                    style: "text-align: right;"
                }
            },
            {
                field: "ISSUING_BANK",
                title: "Issuing Bank",
                width: "150px"
            },
            {
                field: "BG_MITI",
                title: "Issue Date",
                width: "120px"
            },
            {
                field: "VALIDITY_MITI",
                title: "Validity Date",
                width: "120px"
            },
            {
                field: "EXPAIRY_DUE_DAYS",
                title: "Expairy Due Days",
                width: "120px",
                attributes: {
                    style: "text-align: right;"
                },
                template: function (dataItem) {
                    var days = dataItem.EXPAIRY_DUE_DAYS || 0;
                    var color = days < 0 ? 'red' : (days < 30 ? 'orange' : 'green');
                    return '<span style="color: ' + color + '; font-weight: bold;">' + days + '</span>';
                }
            },
            {
                field: "DEPOSIT",
                title: "Deposit",
                width: "120px",
                format: "{0:n2}",
                attributes: {
                    style: "text-align: right;"
                }
            },
            {
                field: "TOTAL_SECURITY",
                title: "Total Security",
                width: "120px",
                format: "{0:n2}",
                attributes: {
                    style: "text-align: right;"
                }
            },
            {
                field: "REMARKS",
                title: "Remarks",
                width: "200px"
            }
        ]
    };

    $scope.toDate = function (val) {
        if (!val) return null;
        var m = moment(val);
        return m.isValid() ? m.format("YYYY-MM-DD") : null;
    }

    $scope.printBankGuarantee = function () {
        var grid = $("#bankGuaranteeGrid").data("kendoGrid");
        if (!grid) {
            console.error("Kendo Grid instance not found.");
            return;
        }

        var data = grid.dataSource.data();

        if (data.length === 0) {
            displayPopupNotification("No data available to print.", "info");
            return;
        }

        var companyHeader = '';
        if (!$scope.HIDE_COMPANY && $scope.companyInfo) {
            companyHeader = `
                <div style="background-color: #f5f5f5; padding: 10px; border: 1px solid #ddd; margin-bottom: 15px;">
                    <h3 style="margin: 0 0 5px 0;">${kendo.htmlEncode($scope.companyInfo.COMPANY_NAME || '')}</h3>
                    <p style="margin: 0;">${kendo.htmlEncode($scope.companyInfo.BRANCH_NAME || '')}</p>
                    <p style="margin: 0;">${kendo.htmlEncode($scope.companyInfo.ADDRESS || '')}</p>
                </div>
            `;
        }

        var printContent = `
            <!DOCTYPE html>
            <html>
            <head>
                <title>Bank Guarantee Report</title>
                <style>
                    body { font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; font-size: 10px; }
                    table { width: 100%; border-collapse: collapse; margin-top: 10px; }
                    th, td { border: 1px solid #ccc; padding: 4px; text-align: left; font-size: 9px; }
                    th { background-color: #4CAF50; color: white; font-weight: bold; }
                    .report-header { text-align: center; margin-bottom: 10px; }
                    .report-header h2 { color: #2980b9; margin: 5px 0; }
                    .report-header p { color: #7f8c8d; font-size: 11px; margin: 0; }
                    .expired-row { background-color: #ffcccc; }
                    .grand-total-row { background-color: #e8f5e9; font-weight: bold; }
                    .text-right { text-align: right; }
                    @media print {
                        @page { margin: 0.5cm; }
                    }
                </style>
            </head>
            <body>
                ${companyHeader}
                <div class='report-header'>
                    <h2>Bank Guarantee</h2>
                    <p>${$scope.reportDateRange}</p>
                </div>
                <table>
                    <thead>
                        <tr>
                            <th>Sr No.</th>
                            <th>BG No</th>
                            <th>Party Name</th>
                            <th>Address</th>
                            <th>Dealer Name</th>
                            <th>Area</th>
                            <th>BG Amount</th>
                            <th>Issuing Bank</th>
                            <th>Issue Date</th>
                            <th>Validity Date</th>
                            <th>Expairy Due Days</th>
                            <th>Deposit</th>
                            <th>Total Security</th>
                            <th>Remarks</th>
                        </tr>
                    </thead>
                    <tbody>
        `;

        var record = 0;
        data.forEach(function (item) {
            record++;
            var rowClass = (item.EXPAIRY_DUE_DAYS < 0) ? 'expired-row' : '';
            var daysColor = item.EXPAIRY_DUE_DAYS < 0 ? 'red' : (item.EXPAIRY_DUE_DAYS < 30 ? 'orange' : 'green');
            
            printContent += `
                <tr class="${rowClass}">
                    <td>${record}</td>
                    <td>${kendo.htmlEncode(item.BG_NO || '')}</td>
                    <td>${kendo.htmlEncode(item.PARTY_NAME || '')}</td>
                    <td>${kendo.htmlEncode(item.ADDRESS || '')}</td>
                    <td>${kendo.htmlEncode(item.PARTY_TYPE_EDESC || '')}</td>
                    <td>${kendo.htmlEncode(item.AREA_EDESC || '')}</td>
                    <td class="text-right">${kendo.toString(item.BG_AMOUNT || 0, 'n2')}</td>
                    <td>${kendo.htmlEncode(item.ISSUING_BANK || '')}</td>
                    <td>${kendo.htmlEncode(item.BG_MITI || '')}</td>
                    <td>${kendo.htmlEncode(item.VALIDITY_MITI || '')}</td>
                    <td class="text-right" style="color: ${daysColor}; font-weight: bold;">${item.EXPAIRY_DUE_DAYS || 0}</td>
                    <td class="text-right">${kendo.toString(item.DEPOSIT || 0, 'n2')}</td>
                    <td class="text-right">${kendo.toString(item.TOTAL_SECURITY || 0, 'n2')}</td>
                    <td>${kendo.htmlEncode(item.REMARKS || '')}</td>
                </tr>
            `;
        });

        printContent += `
                        <tr class="grand-total-row">
                            <td colspan="6"><strong>Grand Total:</strong></td>
                            <td class="text-right"><strong>${kendo.toString($scope.grandTotal.BG_AMOUNT, 'n2')}</strong></td>
                            <td colspan="4"></td>
                            <td class="text-right"><strong>${kendo.toString($scope.grandTotal.DEPOSIT, 'n2')}</strong></td>
                            <td class="text-right"><strong>${kendo.toString($scope.grandTotal.TOTAL_SECURITY, 'n2')}</strong></td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
                <div style="margin-top: 20px; font-size: 9px;">
                    <p>Printed on ${new Date().toLocaleDateString()} at ${new Date().toLocaleTimeString()}</p>
                    <p><strong>Notes:</strong> Red Color BG has expired.</p>
                </div>
            </body>
            </html>
        `;

        var printWindow = window.open("", "_blank");

        if (!printWindow) {
            alert("Pop-up window blocked. Please enable pop-ups for this site.");
            return;
        }

        printWindow.document.open();
        printWindow.document.write(printContent);
        printWindow.document.close();

        printWindow.onafterprint = function () {
            printWindow.close();
        };

        printWindow.focus();
        printWindow.print();
    };

    $scope.exportToExcel = function () {
        var grid = $("#bankGuaranteeGrid").data("kendoGrid");
        if (!grid) {
            console.error("Kendo Grid instance not found.");
            return;
        }

        var data = grid.dataSource.data();

        if (data.length === 0) {
            displayPopupNotification("No data available to export.", "info");
            return;
        }

        // Use Kendo's built-in Excel export
        grid.saveAsExcel();
    };

    $scope.showReportOnStart = function () {
        $("#showreportbtn").click(function () {
            $("#showreportbtn").attr("id", "");
            let text = $("#showreportbtn").text();
            let value = $("#showreportbtn").val();
            console.log(`The value for the text ${text} is ${value}`);
        })
    }

});

