
distributionModule.controller('OtherEntitySetupCtrl', function ($scope, $http, ResellerService, $routeParams) {
    $scope.isViewLoading = false;
    $scope.Contacts = [];
    $scope.Preferences = {};
    $scope.$on('$routeChangeStart', function () {
        $scope.isViewLoading = true;
    });
    $scope.$on('$routeChangeSuccess', function () {
        $scope.isViewLoading = false;
        $scope.fetchSuboutlets();
        ResellerService.GetPreference().then(function (res) {
            $scope.Preferences = res.data;
        }, function (ex) {
            $scope.Preferences.SET_RES_MAP_WHOLESALER = "N";
        });
        //$scope.AddContact();
    });
    $scope.$on('$routeChangeError', function () {
        $scope.isViewLoading = false;
    });


    $scope.param = $routeParams.param;
    $scope.pageName = "Add Other Entity";
    $scope.saveAction = "Save";
    $scope.createPanel = false;

    $scope.areaSelectOptions = {
        dataTextField: "AREA_NAME",
        dataValueField: "AREA_CODE",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Area...</strong></div>',
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
            GetIndividualReport(e);
            //var multiSelect = $("#distCustomerMultiSelect").data("kendoMultiSelect");
            //var ResellermultiSelect = $("#distWholesellerMultiSelect").data("kendoMultiSelect");
            //if (e.sender.value().length > 0) {
            //    //dist select
            //    var selectedDistributor = _.pluck(_.filter(multiSelect.dataSource.data(), function (x) {
            //        return x.AREA_CODE == e.sender.value()[0];
            //    }), "DISTRIBUTOR_CODE");
            //    multiSelect.value(selectedDistributor);

            //    //reseller select
            //    var selectedReseller = _.pluck(_.filter(ResellermultiSelect.dataSource.data(), function (x) {
            //        return x.AREA_CODE == e.sender.value()[0];
            //    }), "CUSTOMER_CODE");
            //    ResellermultiSelect.value(selectedReseller);
            //} else {
            //    multiSelect.value([]);
            //    ResellermultiSelect.value([]);
            //}
        }
    };
    function GetIndividualReport(evt) {
        $http({
            method: 'GET',
            url: window.location.protocol + "//" + window.location.host + "/api/Distribution/GetIndividualGroup?SingleAreaCode=" + evt.sender.value()[0],
        }).then(function successCallback(response) {
            response.data[0] ? $("#distGroupSelect").data("kendoMultiSelect").value(response.data[0].GROUPID) : $("#distGroupSelect").data("kendoMultiSelect").value("");
        }, function errorCallback(response) {
        });
    }
   

    $scope.fetchSuboutlets = function (typeId) {
        var url = '';
        if (typeof (typeId) == 'undefined')
            url = window.location.protocol + "//" + window.location.host + "/api/Setup/getAllSubOutletList";
        else
            url = window.location.protocol + "//" + window.location.host + "/api/Setup/getAllSubOutletList?TYPE_ID=" + typeId
        var dataSource = $("#distSubOutletSelect").data("kendoMultiSelect");
        if (typeof (dataSource) != 'undefined' && dataSource != null) {
            $("#distSubOutletSelect").data("kendoMultiSelect").dataSource.options.transport.read.url = url;
            $("#distSubOutletSelect").data("kendoMultiSelect").dataSource.read();
            return;
        }
        $scope.distSubOutletSelectOptions = {
            dataTextField: "SUBTYPE_EDESC",
            dataValueField: "SUBTYPE_ID",
            height: 600,
            valuePrimitive: true,
            maxSelectedItems: 1,
            headerTemplate: '<div class="col-md-offset-3"><strong>Outlet Category...</strong></div>',
            placeholder: "Select Outlet Category...",
            autoClose: false,
            dataBound: function (e) {
                var current = this.value();
                this._savedOld = current.slice(0);
                $("#" + e.sender.element[0].id + "_listbox").slimScroll({ 'height': '179px', 'scroll': 'scroll' });
            },
            dataSource: {
                transport: {
                    read: {
                        url: url,
                        dataType: "json"
                    }
                }
            }
        };
        //$("#distSubOutletSelect").data("kendoMultiSelect").dataSource.refresh();
    }
 
    $scope.distGroupSelectOptions = {
        
        close: function () {
            var selected = $("#distGroupSelect").data("kendoMultiSelect").dataItem();
            $scope.selectedGroup = typeof (selected) == 'undefined' ? [] : [String(selected.GROUPID)];
            //$scope.$apply();
        },
        dataTextField: "GROUP_EDESC",
        dataValueField: "GROUPID",
        height: 600,
        valuePrimitive: true,
        maxSelectedItems: 1,
        headerTemplate: '<div class="col-md-offset-3"><strong>Group...</strong></div>',
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
        }
    };

    var datas = [
        { text: "Hodding Board", value: "HDB" },
        { text: "Highway Board", value: "HWB" },
        { text: "Futsal", value: "FTS" }]

    $scope.distTypeSelectOptions = {
        dataSource: datas,
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
   

  

    //grid
    var reportConfig = GetReportSetting("OtherEntity");
    $scope.grid = {
        dataSource: {
            type: "json",
            transport: {
                read: {
                    url: window.location.protocol + "//" + window.location.host + "/api/Setup/getAllEntityList",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8"
                },
                parameterMap: function (options, type) {
                    var paramMap = JSON.stringify($.extend(options, ReportFilter.filterAdditionalData()));
                    delete paramMap.$inlinecount; // <-- remove inlinecount parameter.
                    delete paramMap.$format; // <-- remove format parameter.
                    return paramMap;
                }
            },
            schema: {
                parse: function (data) {
                    for (let i = 0; i < data.length; i++) {
                        data[i].Location = (data[i].LATITUDE && data[i].LONGITUDE) ? (data[i].LATITUDE + "," + data[i].LONGITUDE) : "Missing"
                    }
                    return data;
                },
                model: {
                    fields: {
                        // ASSIGN_DATE: { type: "date" },
                    }
                },
            },
            error: function (e) {
                displayPopupNotification("Sorry error occured while processing data", "error");
            },
            pageSize: reportConfig.defaultPageSize,
        },
        toolbar: kendo.template($("#toolbar-template").html()),
        excel: {
            fileName: "Reseller Setup",
            allPages: true,
        },
        //excelExport: function (e) {
        //    ExportToExcel(e);
        //    e.preventDefault();
        //},
        pdf: {
            fileName: "Received Schedule",
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
        height: window.innerHeight - 50,
        sortable: true,
        reorderable: true,
        groupable: true,
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
        columnMenu: true,
        columnMenuInit: function (e) {
            wordwrapmenu(e);
            checkboxItem = $(e.container).find('input[type="checkbox"]');
        },
        columnShow: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('OtherEntity', 'grid');
        },
        columnHide: function (e) {
            if ($(".k-widget.k-reset.k-header.k-menu.k-menu-vertical").is(":visible") && checkboxItem != "")
                SaveReportSetting('OtherEntity', 'grid');
        },
        pageable: {
            refresh: true,
            pageSizes: reportConfig.itemPerPage,
            buttonCount: 5
        },
        //scrollable: {
        //    virtual: true
        //},
        pageable: true,
        dataBound: function (o) {
            GetSetupSetting("OtherEntity");
            var grid = o.sender;
            if (grid.dataSource.total() == 0) {
                var colCount = grid.columns.length;
                $(o.sender.wrapper)
                    .find('tbody')
                    .append('<tr class="kendo-data-row" style="font-size:12px;"><td colspan="' + colCount + '" class="alert alert-danger">Sorry, no data :(</td></tr>');
                displayPopupNotification("No Data Found.", "info");
            }
            else {
                var g = $("#grid").data("kendoGrid");
                for (var i = 0; i < g.columns.length; i++) {
                    g.showColumn(i);
                }
                $("div.k-group-indicator").each(function (i, v) {
                    g.hideColumn($(v).data("field"));
                });
            }

            UpdateReportUsingSetting("OtherEntity", "grid");
            $('div').removeClass('.k-header k-grid-toolbar');
        },
        columns: [
            {
                field: "CONTACT_PERSON",
                title: "Name",
            },
            {
                field: "AREA_NAME",
                title: "Area",
            },

            {
                field: "CONTACT_NO",
                title: "Contact",
            },
            {
                field: "Location",
                title: "Location",
                template: "#= (LATITUDE && LONGITUDE) ? Location : '<span style=\"color:red\">'+Location+'</span>' # ",
                width: "22%",
            },
            {
                title: "Action",
                template: " <a class='fa fa-edit editAction' ng-click='UpdateReseller($event)' title='Edit'></a>&nbsp &nbsp<a class='fa fa-trash-o deleteAction' ng-click='DeleteOtherEntity($event)' title='delete'></a> ",
                width: "8%"
            }
        ]
    };

    ////create empty temp array
    var markersArray = [];
    var map;
    var marker;
    $scope.initialize = function () {
        var myLatlng = new google.maps.LatLng(27.700769, 85.300140);

        var mapOptions = {
            zoom: 10,
            center: myLatlng,
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };

        map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

        marker = new google.maps.Marker({
            map: map,
            position: myLatlng,
            title: 'Center',
            draggable: true
        });

        //uncomment above code for map

        /***************** search box function *****************/
        // Create the search box and link it to the UI element.


        if (document.getElementById('mapSearchBox') == null)
            $("#Reseller-map-panel").prepend('<div id="Reseller-map-fullscreen"></div><input id="mapSearchBox" class="controls" type="text" placeholder="Search Box">');
        var input = document.getElementById('mapSearchBox');
        var searchBox = new google.maps.places.SearchBox(input);
        map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);
        // Bias the SearchBox results towards current map's viewport.
        map.addListener('bounds_changed', function () {
            searchBox.setBounds(map.getBounds());
        });

        // Listen for the event fired when the user selects a prediction and retrieve
        // more details for that place.
        searchBox.addListener('places_changed', function () {
            var places = searchBox.getPlaces();
            // console.log(places);

            if (places.length == 0) {
                return;
            }

            if (!places[0].geometry) {
                console.log("Returned place contains no geometry");
                return;
            }

            var latlng = new google.maps.LatLng(places[0].geometry.location.lat(), places[0].geometry.location.lng());
            marker.setPosition(latlng);

            var bounds = new google.maps.LatLngBounds();

            if (places[0].geometry.viewport) {
                bounds.union(places[0].geometry.viewport);
            } else {
                bounds.extend(places[0].geometry.location);
            }
            map.fitBounds(bounds);
            $('#maplat').html(places[0].geometry.location.lat());
            $('#maplng').html(places[0].geometry.location.lng());
        });


        /***************** search box function *****************/


        google.maps.event.addListener(map, 'click', function (e) {
            marker.setPosition(e.latLng);
            writeLabels(e.latLng);
            $('#maplat').html(e.latLng.lat());
            $('#maplng').html(e.latLng.lng());
        });

        google.maps.event.addListener(marker, 'drag', function (e) {
            $('#maplat').html(e.latLng.lat());
            $('#maplng').html(e.latLng.lng());
        });


        /***************** map full screen button function *****************/
        map.controls[google.maps.ControlPosition.TOP_LEFT].push(document.getElementById('Reseller-map-fullscreen'));
        document.getElementById('Reseller-map-fullscreen').style.display = 'block';

        $('#Reseller-map-fullscreen').on('click', function () {
            var mapCenter = map.getCenter();
            if (!$(this).is('.map-fullscreen')) {
                $('body').css({
                    overflow: 'hidden'
                });

                $('#Reseller-map-panel').css({
                    position: "fixed",

                    top: 0,
                    left: 0,
                    height: $(window).height(),
                    width: "100%",
                    zIndex: 9999,
                    margin: "0 auto"
                });
                $(this).addClass('map-fullscreen');
            }
            else {

                $('#Reseller-map-panel').css({
                    position: "relative",
                    top: 'auto',
                    left: 'auto',
                    height: "425px",
                    width: "100%",
                    zIndex: 1,
                    margin: "0 auto"
                });
                $('body').css({
                    overflow: 'auto'
                });
                $(this).removeClass('map-fullscreen');
            }
            google.maps.event.trigger(map, "resize");
            var latlng = new google.maps.LatLng(document.getElementById('maplat').value, document.getElementById('maplng').value);
            map.setCenter(latlng);
        });
        /***************** map full screen button function *****************/
    }

    $scope.initialize();

    $scope.mapReset = function () {
        var latlong = new google.maps.LatLng(27.700769, 85.300140);
        map.setCenter(latlong);
        map.setZoom(10);
        marker.setPosition(latlong);
        $('#maplat').html(27.700769);
        $('#maplng').html(85.300140);
        google.maps.event.trigger(document.getElementById('map-canvas'), 'resize');

    }
     //uncomment above code for map
    $scope.EntityCreate = function (isValid) {
        if (isValid) {
            for (let i = 0; i < $scope.Contacts.length; i++) {
                if ($scope.Contacts[i].Name == "" && $scope.Contacts[i].Number == "" && $scope.Contacts[i].Designation == "")
                    $scope.RemoveContact(i);
                else if ($scope.Contacts[i].Name == "") {
                    isValid = false;
                    break;
                }
            }
        }
        if (!isValid) {
            displayPopupNotification("Invalid Field", "warning");
            if ($scope.OtherEntityForm.contactName.$invalid == true) {
                $('#contactName').focus();
            }
            else if ($scope.OtherEntityForm.areaMultiSelect.$invalid == true) {
                $('#areaMultiSelect').focus();
            }

            else if ($scope.OtherEntityForm.ContactName.$invalid == true) {
                $('#ContactName').focus();
            }

            else if ($scope.OtherEntityForm.ContactNo.$invalid == true) {
                $('#ContactNo').focus();
            }
            return;

        }
        var selectedWholeseller = [];
        var selectedWholesellerArray = [];
        if ($scope.Preferences.SET_RES_MAP_WHOLESALER == "Y")
        for (let i = 0; i < selectedWholeseller.length; i++) {
            selectedWholesellerArray.push({ Code: selectedWholeseller[i] });
        }
        var isArray = $scope.selectedArea;
        if ($.isArray(isArray) == true) {

            var selectedArea = $scope.selectedArea[0];
        }
        else {
            selectedArea = $scope.selectedArea;
        }
       
        var lat = $('#maplat').html();
        var long = $('#maplng').html();
        var groupId = $("#distGroupSelect").data("kendoMultiSelect").dataItem();
        if (groupId == undefined) {
            GROUP_ID = null;
        }
        else {
            GROUP_ID = $("#distGroupSelect").data("kendoMultiSelect").dataItem().GROUPID;
        }
        var selectedType = $scope.selectedType;
        if (selectedType instanceof Array)
            selectedType = $scope.selectedType[0];

        var data = {
            CONTACT_PERSON: $scope.contactName,
            AREA_CODE: selectedArea,
            ADDRESS: $scope.Address,
            CONTACT_NO: $scope.ContactNo,
            LATITUDE: $('#maplat').html(),
            LONGITUDE: $('#maplng').html(),
            GROUP_ID: GROUP_ID,
            CODE: $scope.Code,
            TYPE: selectedType
        };
        if ($scope.saveAction == "Update") //update mode
        {
            ResellerService.UpdateEntity(data).then(function (result) {
                if (result.data === "success") {
                    displayPopupNotification("Update Successfully", "success");
                    $("#grid").data("kendoGrid").dataSource.read();
                    // $('.createPanel').hide();
                    $scope.createPanel = false;
                }
                else {
                    displayPopupNotification("Something Went Wrong", "warning");
                }
            }, function (error) {
                displayPopupNotification("Something Went Wrong", "error");
            });
        }
        else { //add mode
            ResellerService.AddOtherEntity(data).then(function (result) {
                if (result.data.STATUS_CODE === 200) {
                    displayPopupNotification(result.data.MESSAGE, "success");
                    $("#grid").data("kendoGrid").dataSource.read();
                    //$('.createPanel').hide();
                    $scope.createPanel = false;
                }
                else {
                    displayPopupNotification(result.data.MESSAGE, "warning");
                }
            }, function (error) {
                displayPopupNotification("Error", "error");
            });
        }

    }

    $scope.AddButtonClickEvent = function () {
        $scope.clear();
        $scope.createPanel = true;
        //update map   
        var latlong = new google.maps.LatLng(27.700769, 85.300140);
        map.setCenter(latlong);
        marker.setPosition(latlong);
        $('#maplat').html(27.700769);
        $('#maplng').html(85.300140);

        setTimeout(function () {
            google.maps.event.trigger(document.getElementById('map-canvas'), 'resize');
            map.setCenter(latlong);
        });


    }

    $scope.UpdateReseller = function (evt) {
        var grid = $("#grid").data("kendoGrid");
        var row = evt.target.closest("tr");
        var item = grid.dataItem(row);


        //ResellerService.GetIndividualEntity(item.CODE).then(function (result) {
        //    $scope.pageName = "Update Other Entity";
        //    $scope.saveAction = "Update";
        //    $scope.Contacts = [];
            
        //    item = result.data[0];
        //    if (item == undefined)
        //        return;
        //    $scope.selectedArea = item.AREA_CODE;
        //    $scope.contactName = item.CONTACT_PERSON;
        //    $scope.ContactNo = parseInt(item.CONTACT_NO);
        //    $scope.selectedGroup = [item.GROUP_ID];
        //    $("#distGroupSelect").data("kendoMultiSelect").value([item.GROUP_ID]);
        //    $scope.Contacts = item.Contacts;
        //    //update map   
        //    var latlong = new google.maps.LatLng(item.LATITUDE, item.LONGITUDE);

        //    map.setCenter(latlong);
        //    marker.setPosition(latlong);
        //    $('#maplat').html(item.LATITUDE);
        //    $('#maplng').html(item.LONGITUDE);
        //    //$('.createPanel').show();
        //    $scope.createPanel = true;

        //}, function (error) {
        //    displayPopupNotification("Cannot retrieve contacts", "error");
        //    });

        $scope.selectedArea = item.AREA_CODE;
        $scope.Code = item.CODE;
        $scope.selectedType = item.TYPE;
        $("#distGroupSelect").data("kendoMultiSelect").value([item.GROUP_ID]);
        $scope.pageName = "Update Other Entity";
        $scope.contactName = item.CONTACT_PERSON;
        $scope.ContactNo = parseInt(item.CONTACT_NO)
        $scope.saveAction = "Update";
        //update map   
        var latlong = new google.maps.LatLng(item.LATITUDE, item.LONGITUDE);
        map.setCenter(latlong);
        marker.setPosition(latlong);
        $('#maplat').html(item.LATITUDE);
        $('#maplng').html(item.LONGITUDE);

        $scope.createPanel = true;

    }

    $scope.DeleteOtherEntity = function (evt) {
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
                    var grid = $("#grid").data("kendoGrid");
                    var row = evt.target.closest("tr");
                    var item = grid.dataItem(row);

                    var CODE = item.CODE;
                    var CONTACT_PERSON = item.CONTACT_PERSON;

                    var data = {
                        CODE: CODE,
                        CONTACT_PERSON: CONTACT_PERSON,
                    }
                    var DeleteOtherEntity = ResellerService.DeleteOtherEntity(data);
                    DeleteOtherEntity.then(function (response) {
                        if (response.data == "success") {
                            displayPopupNotification("Deleted successfully", "Success");
                            $("#grid").data("kendoGrid").dataSource.read();
                            $scope.clear();
                        }
                        else {
                            displayPopupNotification("Something Went Wrong", "error");
                        }

                    });
                }

            }
        });

    };

    $scope.Cancel = function () {
        $scope.createPanel = false;
    }

    $scope.clear = function () {
        $scope.pageName = "Add Other Entity";
        $('#contactName').css("border-color", "#e5e5e5")
        $scope.contactName = "";
        $scope.Address = "";
        $scope.ContactNo = "";
        $scope.selectedGroup = '';
        $scope.selectedType = '';
        $("#distGroupSelect").data("kendoMultiSelect").value([]);
        $scope.WholeSeller = true;
        $("#areaMultiSelect").data("kendoMultiSelect").value([]);
        $scope.selectedArea = null;
        $scope.saveAction = "Save";
        $scope.Contacts = [];
    }

    //Contacts operations
 

    $scope.AddContact = function (item) {
        var i = $scope.Contacts.length;
        var available = $scope.Contacts;
        $scope.Contacts = [];
        if (typeof (item) == "undefined" || item == null)
            $scope.Contacts.push({
                Sn: i + 1,
                ContactSuffix: "Mr.",
                Name: '',
                Number: '',
                Designation: '',
                Primary: 'N',
            });
        else
            $scope.Contacts.push({
                Sn: i + 1,
                ContactSuffix: item.ContactSuffix,
                Name: item.Name,
                Number: item.Number,
                Designation: item.Designation,
                Primary: 'N',
            });
        for (var i = 0; i < available.length; i++) {
            var item = available[i];
            $scope.Contacts.push(item);
        }
    }

    $scope.RemoveContact = function (index) {
        $scope.Contacts.splice(index, 1);
        for (var i = 0; i < $scope.Contacts.length; i++) {
            $scope.Contacts[i].Sn = $scope.Contacts.length - i;
        }
    };

    function ExportToExcel(e) {

        //Report header details
        var companyName = ('@workingContent.CurrentUserinformation.company_name');
        var fromADdate = $("#FromDateVoucher").val()
        var toADdate = $("#ToDateVoucher").val();
        var fromBSdate = $("#fromInputDateVoucher").val();
        var toBSdate = $("#toInputDateVoucher").val();
        var branches = ReportFilter.getBranchs();

        //create a footer row to display totals
        //var footer = [];
        //footer.push({ value: "Total", colSpan: 4, textAlign: "center", bold: true, })
        //var footerTemp = e.workbook.sheets[0].rows[e.workbook.sheets[0].rows.length - 1].cells;
        var SheetRow = [];//array to store the rows
        //push the header to row array
        SheetRow.push({
            cells: [
                { value: "Name", background: "#A9A7A6", },
                { value: "Area", background: "#A9A7A6", },
                { value: "Wholeseller", background: "#A9A7A6", },
                { value: "Contact", background: "#A9A7A6", },
                { value: "Status", background: "#A9A7A6", },
                { value: "Address", background: "#A9A7A6", },
                { value: "Location", background: "#A9A7A6", },
                { value: "Group", background: "#A9A7A6", },
                { value: "Outlet Type", background: "#A9A7A6", },
                { value: "Outlet Category", background: "#A9A7A6", },
                { value: "Distributers", background: "#A9A7A6", },
                { value: "S. Contacts", background: "#A9A7A6", },
            ]
        });

        WriteData(e.data);

        //recursive function to write the grouped/ungrouped data
        function WriteData(array) {
            array.forEach(function (row, index) {
                if (typeof (row.items) != "undefined") { //if array contains nested items, write a row with group field and enter recursion
                    SheetRow.push({
                        cells: [{
                            value: row.field + " : " + row.value,
                            background: "#E1E1E1",
                            colSpan: SheetRow[0].length,//to span the total number of columns
                            fontSize: 12,
                        }]
                    });
                    WriteData(row.items);
                }
                else { //if array contains no nested items write the row to excelsheet
                    SheetRow.push({
                        cells: [ {
                            value: row.Reseller_NAME,
                        }, {
                            value: row.AREA_Name,
                        }, {
                            value: row.WholeSeller,
                        }, {
                            value: row.Primary_Contact,
                        },{
                            value: row.Status,
                        }, {
                            value: row.ADDRESS,
                        }, {
                            value: row.Location,
                        }, {
                            value: row.GROUP_NAME,
                        },  {
                            value: row.DISTRIBUTER_DETAILS,
                        }, {
                            value: row.CONTACT_DETAILS,
                        }]
                    });
                }
            });
        }

        //push the footer variable containing total values
        //SheetRow.push({
        //    cells: footer,
        //});

        //pushing Report header details
        SheetRow.unshift({
            cells: [{ value: "Outlet Lists".split('').join(' '), colSpan: SheetRow[0].length }]
        });
        //initializing a workbook with above prepared rows
        var Workbook = new kendo.ooxml.Workbook({
            sheets: [{
                columns: [
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { autoWidth: true },
                    { width: 100 },
                    { width: 100 },
                    { width: 100 },
                ],
                rows: SheetRow,
            }]
        });
        //finally saving the excel sheet
        kendo.saveAs({
            dataURI: Workbook.toDataURL(),
            fileName: "Other Entity Setup.xlsx"
        });
    }

});
