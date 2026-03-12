
distributionModule.controller('ImageCategoryCtrl', function ($scope, DistSetupService) {
    $scope.saveAction = "Save";
    $scope.ImageCategory = {
        CATEGORY_CODE: '',
        CATEGORY_EDESC: '',
        CATEGORYID: '',
        Max_Items: '',
        selectedType:''
    };
    $scope.ImageCategoryHeader = "Add Category";
    $scope.grid = {
        change: ImageCategoryChange,
        dataSource: {
            transport: {
                read: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllCategoryImage",
            },
            pageSize: 20
        },
        dataBound: function (e) {
            GetSetupSetting("ImageCategorySetup");
        },
        height: window.innerHeight - 50,
        ImageCategoryable: false,
        sortable: false,
        height: 400,
        selectable: true,
        columns: [
            {
                field: "CATEGORY_EDESC",
                title: "Image Category Name",
                width: 150
            }]

    }
    function ImageCategoryChange(evt) {
        selectedRow = this.select();
        var item = this.dataItem(selectedRow);

        if (item) {
            $scope.saveAction = "Update";
            $scope.ImageCategoryHeader = "Update Category";

            $scope.ImageCategory.CATEGORY_EDESC = item.CATEGORY_EDESC;
            $scope.ImageCategory.CATEGORY_CODE = item.CATEGORY_CODE;
            $scope.ImageCategory.CATEGORYID = item.CATEGORYID;
            $scope.ImageCategory.Max_Items = item.Max_Items;
            $scope.ImageCategory.selectedType = item.selectedType;
            $scope.$apply();
        }
    }

    $scope.SaveImageCategory = function (isValid) {
        if (!isValid) {
            displayPopupNotification("Input fields are not valid. Please review and try again", "warning");
            return;
        }
        DistSetupService.AddImageCategory($scope.ImageCategory).then(function (result) {
            displayPopupNotification(result.data.MESSAGE, result.data.TYPE);
            $("#grid").data("kendoGrid").dataSource.read();
            $scope.Cancel();
        }, function (result) {
            displayPopupNotification(result.data.MESSAGE, "error");
        });
    }

    var typeDatas = [
        { text: "Distribution", value: "D" },
        { text: "Branding", value: "B" },
        { text: "Audit", value: "A" }]

    $scope.typesOptions = {
        dataSource: typeDatas,
        close: function () {
            var selected = $("#distTypeSelect").data("kendoDropDownList").dataItem();
            $scope.selectedType = typeof (selected) == 'undefined' ? [] : [String(selected.value)];
        },
        dataTextField: "text",
        dataValueField: "value",
        optionLabel: "Please Select...",
        height: 400,
        placeholder: "Select Type...",
        autoClose: false,
        dataBound: function (e) {
            var current = this.value();
            this._savedOld = current.slice(0);
            $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
        },
    };

    $scope.deleteImage = function () {
        bootbox.confirm({
            title: "Delete",
            message: "Are you sure?",
            buttons: {
                confirm: {
                    label: 'Yes',
                    className: 'btn-success',
                    label: '<i class="fa fa-check"></i> Yes',
                },
                cancel: {
                    label: 'No',
                    className: 'btn-danger',
                    label: '<i class="fa fa-times"></i> No',
                }
            },
            callback: function (result) {

                if (result == true) {
                    var CATEGORY_CODE = $scope.ImageCategory.CATEGORY_CODE;
                    var CATEGORYID = $scope.ImageCategory.CATEGORYID;
                    var CATEGORY_EDESC = $scope.ImageCategory.CATEGORY_EDESC;

                    var data = {
                        CATEGORY_CODE: CATEGORY_CODE,
                        CATEGORYID: CATEGORYID,
                        CATEGORY_EDESC: CATEGORY_EDESC,
                    }

                    var deleteImage = DistSetupService.deleteImage(data);
                    deleteImage.then(function (response) {
                        if (response.data.STATUS_CODE == 200) {
                            displayPopupNotification(response.data.MESSAGE, "Success");
                            $("#grid").data("kendoGrid").dataSource.read();
                            $scope.Cancel();
                        }
                        else {
                            displayPopupNotification(response.data.MESSAGE, "error")
                        }

                    });
                }

            
            }
        });
    }

    $scope.Cancel = function () {
        $scope.saveAction = "Save";
        $scope.ImageCategoryHeader = "Add Image Category";
        $scope.ImageCategory = {
            CATEGORY_CODE: '',
            CATEGORY_EDESC: '',
            CATEGORYID: '',
            Max_Items: ''
        };
        $("#grid").data("kendoGrid").clearSelection();
    }

});

