
transactionModule.controller('BankSetupCtrl', function ($scope, $window, BankSetupservice, $routeParams, $rootScope) {
    $scope.BankSetup = {
        BankCode: '',
        BankName: '',
        AccountNumber: '',
        SwiftCode: '',
        Address: '',
        Branch: '',
        PhoneNumber: '',
        Contacts: [],
    };
    $scope.addContact = function () {
        debugger;
        var i = $scope.BankSetup.Contacts.length;
        var available = $scope.BankSetup.Contacts;
        $scope.BankSetup.Contacts = [];
        $scope.BankSetup.Contacts.push({
            Sn: i + 1,
            ContactPerson: '',
            MobileNumber: '',
            TelephoneNumber: '',
            Designation: '',
            Remarks: '',
        });
        for (var i = 0; i < available.length; i++) {
            var item = available[i];
            $scope.BankSetup.Contacts.push(item);
        }
    };
    $scope.removeContact = function (index) {
        $scope.BankSetup.Contacts.splice(index, 1);
        for (var i = 0; i < $scope.BankSetup.Contacts.length; i++) {
            $scope.BankSetup.Contacts[i].Sn = $scope.BankSetup.Contacts.length - i;
        }
    };
    $scope.$on('$routeChangeSuccess', function () {
        $scope.GetAllBanks();
        $scope.addContact();
    });
    $scope.ResetBankSetup = function (form) {
        debugger;
        form.$setPristine();
        form.$setUntouched();
        $scope.BankSetup = {
            BankCode: '',
            BankName: '',
            AccountNumber: '',
            SwiftCode: '',
            Address: '',
            Branch: '',
            PhoneNumber: '',
            Remarks: '',
            Contacts: [],
        };
        $scope.addContact();
    };
    $scope.SaveBank = function (form, isValid) {
        if (validateForm(isValid)) {
            BankSetupservice.SaveBankSetup($scope.BankSetup).then(function (status) {
                displayPopupNotification("Bank saved.", "success");
                $scope.GetAllBanks();
                $scope.ResetBankSetup(form);
            }, function (error) {
                displayPopupNotification("Error on saving.Please fix the error and Retry. \n Message:" + error, "error");
            });
        }
        else {

        }
    }
    $scope.DeleteBank = function (bankCode, form) {
        var DlgOptions = {
            width: 400,
            pinned: true,
            resizable: false,
            position: {
                left: "35%",
                top: "10%",
            },
            //height: 80,
            visible: false,
            title: "Confirm",
            draggable: false,
            actions: ["Close"]
        };
        $scope.dlgWindow.setOptions(DlgOptions);
        $scope.dlgWindow.open();
        $("#btn-okay").on("click", function () {
            debugger;
            BankSetupservice.DeleteBank(bankCode).then(function (status) {
                displayPopupNotification("Bank deleted.", "success");
                $scope.GetAllBanks();
                debugger;
                $scope.ResetBankSetup(form);
            }, function (error) {
                displayPopupNotification("Error on Deleting.Please fix the error and Retry. \n Message:" + error, "error");
            });
            $scope.dlgWindow.close();
        });
        $("#btn-cancel").on("click", function () {
            $scope.dlgWindow.close();
        });
    }
    $scope.GetAllBanks = function () {
        debugger;
        var response = BankSetupservice.GetBankList().then(function (result) {
            $scope.AllBanks = result.data;
        }, function (ex) { });
    }
    $scope.GetBank = function (bankCode) {
        var response = BankSetupservice.GetBank(bankCode).then(function (result) {
            $scope.BankSetup = result.data;
        }, function (ex) { });
    }
    validateForm = function (IsValid) {
        if (!IsValid) {
            displayPopupNotification("Input fileds are not valid, Please review form and Retry.", "warning");
            return false;
        }
        if ($scope.BankSetup.Contacts.length < 1) {
            displayPopupNotification("At least one contact should be entered, Please review contact list and Retry.", "warning");
            return false;
        }
        for (var i = 0; i < $scope.BankSetup.Contacts.length; i++) {
            if ($scope.BankSetup.Contacts[i].ContactPerson == '' || $scope.BankSetup.Contacts[i].MobileNumber == '' || $scope.BankSetup.Contacts[i].Designation == '') {
                displayPopupNotification("Input fileds are not valid, Please review form and Retry.", "warning");
                return false;
            }
        }
        return true;
    }
});