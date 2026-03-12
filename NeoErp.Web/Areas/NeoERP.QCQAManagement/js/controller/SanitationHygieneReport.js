QCQAModule.controller('SanitationHygieneReportList', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    $scope.SanitationHygieneData = [];
    $scope.childModels = [];
    $scope.DaysInMonth = 0;

    if ($routeParams.voucherno != undefined) {
        $scope.voucherno = $routeParams.voucherno;//.split(new RegExp('_', 'i')).join('/');     
    }

    //showNdpCalendarBox('fromInputDateVoucher')
    var date = new Date();
    var d = date.getDate();
    var m = date.getMonth();
    var y = date.getFullYear();
    $scope.QCNO = '@ViewBag.QCNO';
    $("#nepaliDate5").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));
    $('#ddlDateFilterVoucher').val($('option:selected', '#ddlDateFilter').val());
    //$("#ddlDateFilterVoucher").val('This Month');
    //$("#ddlDateFilterVoucher").trigger('change');
    //$('#fromInputDateVoucher').on('click', function () {
    //    showNdpCalendarBox('nepaliDate5');
    //    //showNdpCalendarBox($(this.id));
    //});
    $('#fromInputDateVoucher').nepaliDatePicker({
        ndpEnglishInput: 'FromDateVoucher',
        npdMonth: true,
        npdYear: true,
        npdYearCount: 20,
        onChange: function () {
            $('#ddlDateFilterVoucher').val("Custom");
            $("#FromDateVoucher").val(moment(BS2AD($("#fromInputDateVoucher").val())).format("YYYY-MMM-DD"))
        },
    });
    $('#toInputDateVoucher').nepaliDatePicker({
        ndpEnglishInput: 'ToDateVoucher',
        npdMonth: true,
        npdYear: true,
        npdYearCount: 20,
        onChange: function () {
            $('#ddlDateFilterVoucher').val("Custom");
            $("#ToDateVoucher").val(moment(BS2AD($("#toInputDateVoucher").val())).format("YYYY-MMM-DD"))
        }
    });
    $("#fromInputDateVoucher").addClass('ndp-nepali-calendar');
    $('#toInputDateVoucher').attr('onfocus', "showNdpCalendarBox('toInputDateVoucher')");
    $("#toInputDateVoucher").addClass('ndp-nepali-calendar');
    $("#fromInputDateVoucher").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));
    $("#toInputDateVoucher").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));
    $("#nepaliDate6").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));
    if ($("#ndp-nepali-box")[0].style.display == 'block' || $("#ndp-nepali-box")[0].style.display == "")
        showNdpCalendarBox('nepaliDate5');//$("#nepaliDate5").trigger('onfocus')
    $('#FromDateVoucher').val(moment(BS2AD($("#fromInputDateVoucher").val())).format("YYYY-MMM-DD"));
    $('#ToDateVoucher').val(moment(BS2AD($("#toInputDateVoucher").val())).format("YYYY-MMM-DD"));
    $('#FromDateVoucher, #ToDateVoucher').datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true
    });
    //$('#fromInputDateVoucher').on('change', function () {
    //    console.lo(56);
    //    //$(this).val(moment($(this).val()).format('YYYY-MMM-DD'));
    //    //$('#FromDateVoucher').val(moment(BS2AD($(this).val())).format("YYYY-MMM-DD"));
    //});
    $('#FromDateVoucher').on('change', function () {
        $(this).val(moment($(this).val()).format('YYYY-MMM-DD'));
    });
    $('#ToDateVoucher').on('change', function () {
        $(this).val(moment($(this).val()).format('YYYY-MMM-DD'));
    });
    //$('#FromDateVoucher, #ToDateVoucher').on('change', function () {
    //    $(this).val(moment($(this).val()).format('YYYY-MMM-DD'));
    //});
    var fromDate = $('#FromDateVoucher').val(); // "2025-Oct-13"
    var toDate = $('#ToDateVoucher').val();     // "2025-Oct-20"
    var sandetails = "/api/SanitationHygieneApi/GetSanitationHygieneDetailsReport?frmDate=" + encodeURIComponent(fromDate) + "&toDate=" + encodeURIComponent(toDate);
    $http.get(sandetails).then(function (response) {
        var qcqa = response.data;
        $scope.childModels = [];
        $scope.SanitationHygieneData = [];
        console.log("response.data", response.data);
        if (qcqa && qcqa.length > 0) {
            qcqa.forEach(function (dept) {
                var cm = {
                    DEPARTMENT_EDESC: dept.DEPARTMENT_EDESC,
                    Days: {}
                };
                for (var day = 1; day <= 32; day++) {
                    cm.Days[day] = {
                        STANDARD: dept.Days && dept.Days[day] ? dept.Days[day].STANDARD || null : null,
                        ACTUAL: dept.Days && dept.Days[day] ? dept.Days[day].ACTUAL || null : null,
                        GAP: dept.Days && dept.Days[day] ? dept.Days[day].GAP || null : null
                    };
                }

                $scope.childModels.push(cm);
                $scope.SanitationHygieneData.push(cm);
            });
        }
        })
        .catch(function (error) {
            displayPopupNotification(error, "error");
        })
    // remove child row.
    $scope.remove_child_element = function (index) {
        if ($scope.SanitationHygieneData.length > 1) {
            $scope.SanitationHygieneData.splice(index, 1);
            $scope.childModels.splice(index, 1);
        }
    }
    $scope.updateGAP = function (key) {
        let standard = parseFloat($scope.childModels[key]['STANDARD']) || 0;
        let actual = parseFloat($scope.childModels[key]['ACTUAL']) || 0;
        $scope.childModels[key]['GAP'] = standard - actual;
    };


    $scope.DaysInMonth = Array.from({ length: 1 }, (_, i) => i + 1);
    $scope.TotalDays = 2;
    var width = ($scope.TotalDays * 55) + 'px';
    var styleId = 'day-width-style';
    var styleEl = document.getElementById(styleId);
    if (!styleEl) {
        styleEl = document.createElement('style');
        styleEl.id = styleId;
        document.head.appendChild(styleEl);
    }
    styleEl.innerHTML = `.day { width: ${width} !important; }`;


    function parseDateSafe(dateStr) {
        // Handles: YYYY-MMM-DD, YYYY-MM-DD
        if (dateStr.includes('-') && isNaN(dateStr.split('-')[1])) {
            // YYYY-MMM-DD
            const parts = dateStr.split('-');
            const year = +parts[0];
            const day = +parts[2];
            const monthMap = {
                jan: 0, feb: 1, mar: 2, apr: 3, may: 4, jun: 5,
                jul: 6, aug: 7, sep: 8, oct: 9, nov: 10, dec: 11
            };
            const month = monthMap[parts[1].toLowerCase()];
            return new Date(year, month, day);
        }
        // YYYY-MM-DD (safe ISO)
        return new Date(dateStr + 'T00:00:00');
    }

    $(".applydp").on("click", function (evt) {
        evt.preventDefault();
        var diffDays = 0;
        var fromDate = $('#FromDateVoucher').val();
        var toDate = $('#ToDateVoucher').val();
        if (fromDate && toDate) {
            var start = parseDateSafe(fromDate);
            var end = parseDateSafe(toDate);
            start.setHours(0, 0, 0, 0);
            end.setHours(0, 0, 0, 0);
            var diffTime = Math.abs(end - start);
            diffDays = Math.floor(diffTime / 86400000) + 1;
        }
        $scope.DaysInMonth = Array.from({ length: diffDays }, (_, i) => i + 1);
        $scope.TotalDays = 2;
        var width = ($scope.TotalDays * 55) + 'px';
        var styleId = 'day-width-style';
        var styleEl = document.getElementById(styleId);
        if (!styleEl) {
            styleEl = document.createElement('style');
            styleEl.id = styleId;
            document.head.appendChild(styleEl);
        }
        styleEl.innerHTML = `.day { width: ${width} !important; }`;

        /*var sandetails = "/api/SanitationHygieneApi/GetSanitationHygieneDetailsReport?frmDate=" + $('#FromDateVoucher').val() + "&toDate=" + $('#ToDateVoucher').val();*/
        var sandetails = "/api/SanitationHygieneApi/GetSanitationHygieneDetailsReport?frmDate=" + $('#FromDateVoucher').val() + "&toDate=" + $('#ToDateVoucher').val();
        $http.get(sandetails).then(function (response) {
            var qcqa = response.data;
            $scope.childModels = [];
            $scope.SanitationHygieneData = [];
            if (qcqa && qcqa.length > 0) {
                qcqa.forEach(function (dept) {
                    var cm = {
                        DEPARTMENT_EDESC: dept.DEPARTMENT_EDESC,
                        Days: {}
                    };
                    for (var day = 1; day <= 32; day++) {
                        cm.Days[day] = {
                            STANDARD: dept.Days && dept.Days[day] ? dept.Days[day].STANDARD || null : null,
                            ACTUAL: dept.Days && dept.Days[day] ? dept.Days[day].ACTUAL || null : null,
                            GAP: dept.Days && dept.Days[day] ? dept.Days[day].GAP || null : null
                        };
                    }
                    $scope.childModels.push(cm);
                    $scope.SanitationHygieneData.push(cm);
                });
            }
        })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
        $('[data-dismiss="modal"]').trigger('click');
    })
});