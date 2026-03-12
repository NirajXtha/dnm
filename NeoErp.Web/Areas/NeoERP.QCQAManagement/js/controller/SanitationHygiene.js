QCQAModule.controller('SanitationHygieneList', function ($scope, $rootScope, $http, $filter, $timeout, $routeParams) {
    $scope.voucherno = "";
    $scope.SanitationHygieneData = [];
    $scope.childModels = [];
    $scope.SanitationHygieneChildData = [];

    if ($routeParams.voucherno != undefined) {
        $scope.voucherno = $routeParams.voucherno;//.split(new RegExp('_', 'i')).join('/');     
    }
    $("#englishdatedocument").kendoDatePicker({
        value: new Date(),
        format: "dd-MMM-yyyy"
    });
    $scope.QCNO = '@ViewBag.QCNO';
    $("#nepaliDate5").val(AD2BS(kendo.toString(new Date, 'yyyy-MM-dd')));

    $http.get('/api/SanitationHygieneAPI/GetMasterSanitationHygiene')
        .then(function (response) {
            var qcqa = response.data || [];
            $scope.SanitationHygieneData = qcqa;
            $scope.childModels = qcqa;

            // Initialize each parent row
            angular.forEach($scope.SanitationHygieneData, function (row) {
                row.isExpanded = false;
                row.children = []; // prepare for child data
            });

            // Return parent data so we can use in the next call
            return $scope.SanitationHygieneData;
        })
        .then(function (parentData) {
            // Load all children
            return $http.get('/api/SanitationHygieneAPI/GetAllSanitationHygieneDetails')
                .then(function (response) {
                    var allChildren = response.data || [];

                    // Map children to their respective parent
                    angular.forEach(parentData, function (row) {
                        row.children = allChildren.filter(function (child) {
                            return child.PARENT_ID === row.ID; // ⚡ map children correctly
                        });
                        angular.forEach(row.children, function (child) {
                            child.isExpanded = false;
                            child.children = [];
                        });
                    });
                });
        })
        .catch(function (error) {
            displayPopupNotification(error, "error");
        });

    $scope.toggleRow = function (row) {
        row.isExpanded = !row.isExpanded;

        if (row.isExpanded && !row.childrenLoaded) {
            if ($scope.voucherno == "") {
                $http.get('/api/SanitationHygieneAPI/GetSanitationHygieneDetails?LocationCode=' + row.DEPARTMENT_CODE)
                    .then(function (res) {
                        var parentCode = String(row.DEPARTMENT_CODE).trim();
                        console.log("i m row", row.children);
                        // Store children inside this row
                        row.children = (res.data || []).filter(function (item) {
                            var code = String(item.DEPARTMENT_CODE).trim();
                            return code.indexOf(parentCode + '.') === 0; // only children of this parent
                        });
                       
                        // Initialize children for further nesting
                        row.children.forEach(function (child) {
                            //child.isExpanded = false;
                            child.children = []; // prepare for next level
                        });
                        row.childrenLoaded = true;
                    }).catch(function (err) {
                        console.error("Error loading children:", err);
                    });
            }
        }
    };
  
    $scope.remove_child_element = function (index, parentRow, childObject, parentChildrenArray, data) {
        if (data === "AllSanitationHygieneChildData") {
            $scope.SanitationHygieneData.forEach(function (row) {
                if (row.children && row.children.length) {
                    row.children = row.children.filter(function (c) {
                        return c.DEPARTMENT_CODE !== childObject.DEPARTMENT_CODE;
                    });
                }
            });
            if (!$scope.$$phase) $scope.$apply();
            return;
        }

        // DELETE MASTER + ALL PRECEDING DATA
        if ($scope.SanitationHygieneData.length > 1) {
            var deletedRow = $scope.SanitationHygieneData[index];
            var parentCode = deletedRow.DEPARTMENT_CODE;

            // Recursive function to remove children from a given row
            function removeChildren(rows, parentCode) {
                return rows
                    .filter(row => row.DEPARTMENT_CODE !== parentCode &&
                        !row.DEPARTMENT_CODE.startsWith(parentCode + '.'))
                    .map(row => {
                        if (row.children && row.children.length) {
                            row.children = removeChildren(row.children, parentCode);
                        }
                        return row;
                    });
            }

            // 1️⃣ Remove master + all descendants from parent array
            $scope.SanitationHygieneData = removeChildren($scope.SanitationHygieneData, parentCode);

            // 2️⃣ Remove from childModels if used
            if ($scope.childModels && $scope.childModels.length) {
                $scope.childModels = $scope.childModels.filter(item =>
                    item.DEPARTMENT_CODE !== parentCode &&
                    !item.DEPARTMENT_CODE.startsWith(parentCode + '.')
                );
            }

            // 3️⃣ Trigger digest if needed
            if (!$scope.$$phase) $scope.$apply();
        }
    };

    function cleanCode(code) {
        return (code || "").toString().trim().replace(/\u00A0/g, "").toUpperCase();
    }
    
    $scope.blockIfExceeds = function (e, key, maxValue) {
        const charCode = e.which || e.keyCode;

        // allow backspace, arrows, delete
        if ([8, 9, 13, 37, 38, 39, 40, 46].includes(charCode)) return;

        let standard_val = $scope.childModels[key]['STANDARD'] || '';
        let actual_val = $scope.childModels[key]['ACTUAL'] || '';
        let newChar = String.fromCharCode(charCode);

        // allow only digits
        if (!/^\d$/.test(newChar)) {
            e.preventDefault();
            return;
        }

        // simulate new value
        let futureVal = Number(standard_val + newChar);
        if (futureVal > maxValue) {
            e.preventDefault();
        }

        let futureVal_actual = Number(actual_val + newChar);
        if (futureVal_actual > maxValue) {
            e.preventDefault();
        }
    };

    $scope.blockIfExceeds_Actual = function (e, key, maxValue) {
        const charCode = e.which || e.keyCode;
        // allow backspace, arrows, delete
        if ([8, 9, 13, 37, 38, 39, 40, 46].includes(charCode)) return;
        let newChar = String.fromCharCode(charCode);

        // allow only digits
        if (!/^\d$/.test(newChar)) {
            e.preventDefault();
            return;
        }

        // simulate new value
        let futureVal_actual = Number(actual_val + newChar);
        if (futureVal_actual > maxValue) {
            e.preventDefault();
        }
    };

    $scope.blockIfExceedschild = function (e, key, maxValue, parentChildrenArray) {
        const charCode = e.which || e.keyCode;

        // allow backspace, arrows, delete
        if ([8, 9, 13, 37, 38, 39, 40, 46].includes(charCode)) return;

        let standard_val = parentChildrenArray[key]['STANDARD'] || '';
        let actual_val = parentChildrenArray[key]['ACTUAL'] || '';
        let newChar = String.fromCharCode(charCode);

        // allow only digits
        if (!/^\d$/.test(newChar)) {
            e.preventDefault();
            return;
        }

        // simulate new value
        let futureVal = Number(standard_val + newChar);
        if (futureVal > maxValue) {
            e.preventDefault();
        }

        let futureVal_actual = Number(actual_val + newChar);
        if (futureVal_actual > maxValue) {
            e.preventDefault();
        }
    };

    $scope.blockIfExceedschild_Actual = function (e, key, maxValue, parentChildrenArray) {
        const charCode = e.which || e.keyCode;
        // allow backspace, arrows, delete
        if ([8, 9, 13, 37, 38, 39, 40, 46].includes(charCode)) return;
        /*let actual_val = $scope.childModels[key]['ACTUAL'] || '';*/
        let actual_val = parentChildrenArray[key]['ACTUAL'] || '';
        let newChar = String.fromCharCode(charCode);

        // allow only digits
        if (!/^\d$/.test(newChar)) {
            e.preventDefault();
            return;
        }

        // simulate new value
        let futureVal_actual = Number(actual_val + newChar);
        if (futureVal_actual > maxValue) {
            e.preventDefault();
        }
    };

    $scope.updateGAP = function (key) {
        let standard = parseFloat($scope.childModels[key]['STANDARD']) || 0;
        let actual = parseFloat($scope.childModels[key]['ACTUAL']) || 0;

        if (standard > 100) {
            $scope.childModels[key]['STANDARD'] = parseFloat($scope.childModels[key]['STANDARD']) || 0;
            standard = standard.substring(0, 100);
        }
        $scope.childModels[key]['GAP'] = standard - actual;
    };
    $scope.updateChildGAP = function (key, parentChildrenArray) {
        let standard = parseFloat(parentChildrenArray[key]['STANDARD']) || 0;
        let actual = parseFloat(parentChildrenArray[key]['ACTUAL']) || 0;

        if (standard > 100) {
            parentChildrenArray[key]['STANDARD'] = parseFloat(parentChildrenArray[key]['STANDARD']) || 0;
            standard = standard.substring(0, 100);
        }
        parentChildrenArray[key]['GAP'] = standard - actual;
    };
    /*var rawMateriallList = [];*/
    $scope.saveSanitationHygiene = function () {
        //var rawMateriallList = [];
        //var rows = document.querySelectorAll("#idSanitationHygiene");
       
        var payload = [];
        var seen = new Set(); // track unique DEPARTMENT_CODE

        $scope.SanitationHygieneData.forEach(function (row) {
            // Save master
            if (!seen.has(row.DEPARTMENT_CODE)) {
                payload.push({
                    DEPARTMENT_CODE: row.DEPARTMENT_CODE,
                    STANDARD: Number(row.STANDARD) || 0,
                    ACTUAL: Number(row.ACTUAL) || 0,
                    GAP: Number(row.GAP) || 0
                });
                seen.add(row.DEPARTMENT_CODE);
            }

            // Save only children of this master
            if (row.children && row.children.length) {
                row.children.forEach(function (child) {
                    if (!seen.has(child.DEPARTMENT_CODE)) {
                        payload.push({
                            DEPARTMENT_CODE: child.DEPARTMENT_CODE,
                            STANDARD: Number(child.STANDARD) || 0,
                            ACTUAL: Number(child.ACTUAL) || 0,
                            GAP: Number(child.GAP) || 0
                        });
                        seen.add(child.DEPARTMENT_CODE);
                    }
                });
            }
        });

        var wrapper = {
            SANITATION_NO: $scope.SANITATION_NO,
            CREATED_DATE: $('#englishdatedocument').val(),
            SanitationHygieneList: payload  // lowercase 'list'
        };
        $http.post('/api/SanitationHygieneAPI/saveSanitationHygiene', wrapper)
            .then(function (response) {
                var message = response.data.message; // Extract message from response
                displayPopupNotification(message, "success");
                setTimeout(function () {
                    window.location.reload();
                }, 5000)
            })
            .catch(function (error) {
                displayPopupNotification(error, "error");
            })
    }

    if ($scope.voucherno != "") {
        $http.get('/api/SanitationHygieneAPI/GetEditSanitationHygiene?transactionno=' + $scope.voucherno)
            .then(function (response) {
                $scope.SANITATION_NO = $scope.voucherno;//response.data.SANITATION_NO;
                $scope.SanitationHygieneList = response.data.SanitationHygieneList;
                $scope.SanitationHygieneData = response.data.SanitationHygieneList;
                $scope.AllSanitationHygieneChildData = response.data.SanitationHygieneChildList;
                angular.forEach($scope.SanitationHygieneData, function (row) {
                    row.isExpanded = true; // expand row to show children during edit

                    // Assign only children for this parent
                    row.children = (response.data.SanitationHygieneChildList || []).filter(function (child) {
                        return child.DEPARTMENT_CODE.startsWith(row.DEPARTMENT_CODE + '.');
                    });

                    // Ensure numeric values and GAP
                    row.children.forEach(function (c) {
                        c.isExpanded = true;
                        c.STANDARD = Number(c.STANDARD) || 0;
                        c.ACTUAL = Number(c.ACTUAL) || 0;
                        c.GAP = c.STANDARD - c.ACTUAL;
                    });
                });
            });
    }
    function hasValue(x) {
        return x !== undefined && x !== null && x !== "";
    }
});