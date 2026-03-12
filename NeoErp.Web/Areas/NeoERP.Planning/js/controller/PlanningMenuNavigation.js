$(document).ready(function () {
    //Get The parent and Child data from here ...
    var favourite = document.distGlobalFavourite;
    var moduleCode = '10';
    var data = "";
    $.ajax({
        type: 'GET',
        dataType: "json",
        async: false,
        //  url: "/api/SalesHome/GetDynamicMenu?ModuleCode=" + moduleCode,
        url: window.location.protocol + "//" + window.location.host + "/api/SalesHome/GetDynamicMenu?ModuleCode=30",
        success: function (data) {
            data = data;
            menuBreadCumList(data);
        }
    });
});
function menuBreadCumList(data) {
    
    var favourite = document.distGlobalFavourite;
    $(".page-breadcrumb").prepend(" <li style='display:inline-block;list-style:none;'><a href='#' id='submenu' style='font-size:14px'></a></li> ");
    $(".page-breadcrumb").prepend("<li style='display:inline-block;list-style:none;'><a href='#' id='group' style='font-style:normal !important;font-size:14px;'></a></li><i class='fa fa-angle-right organiser' style='padding-left:7px;padding_right:2px;color:#c3b2aa'></i>");
    $(".page-breadcrumb").prepend("<li style='display:inline-block;list-style:none;'><a href='/Planning/Home/PlanningDashboard' id='dfds' style='font-style:normal !important;font-size:14px;'>Organiser</a>   <i class='fa fa-angle-right'></i></li>");
    $(".page-breadcrumb").prepend("<li style='display:inline-block;list-style:none;'> <i class='fa fa-home'></i><a href='/main/GlobalDashboard' id='sdfsd' style='font-style:normal !important;font-size:14px;'>Home</a>   <i class='fa fa-angle-right'></i></li>");

    //Glob css for all page 
    $("#group").css({ "border-bottom": "none" });
    $("#submenu").css({ "border-bottom": "none" })

    var newvalue = "";
    var selectedData = "";
    var childArr = "";

    selectedData = data;
    var mydata = data.length;
    var myArr = [];
    myArr.push({ value: "Favourite", text: "Favourite" });
    for (i = 0; i < mydata; i++) {
        myArr.push({ value: data[i].MENU_EDESC, text: data[i].MENU_EDESC });
    }

    var mydata = data.length;
    var myArrr = [];
    for (i = 0; i < mydata; i++) {
        myArrr.push({ value: data[i].MENU_EDESC, text: data[i].MENU_EDESC });
    }

    //Getting value from child and set into the field
    setTimeout(function () {

        var absulateUrlText = (location.pathname + location.hash);
        if ($.isNumeric(parseInt(absulateUrlText.split('/')[absulateUrlText.split('/').length - 1]))) {
            absulateUrlText = absulateUrlText.split('/').slice(0, -1).join('/');
        }
        var selectedURL = $($("a[href='" + absulateUrlText + "']").parents()[2]).find("a > span").html()
        if (selectedURL == undefined || location.hash.indexOf("fav") > -1 == true) {
            var haslocation = location.hash;
            var myString = haslocation.substr(haslocation.indexOf("!") + 1)
            //if (myString == "undefined" || myString == "") {
            //    var selectedURL = "Dashboard";
            //}
            //else
            //{

            //var words = myString.split("/");
            //if (words.length < 2) {
            //    var selectedURL = "Favourite";
            //}
            //else {
            //    var selectedURL = "Planning";
            //}
            var words = myString.split("/");
            if (words == "undefined" || words == "") {
                var selectedURL = "";
            }
            else {
                var selectedURL = "Favourite";
            }
            //}

        }
        var parentLength = data.length;
        var childArr = [];
        if (selectedURL == "Favourite") {
            var childArr = [];
            var childLocation = location.hash.substring(0, location.hash.indexOf('?'));
            var parentLocation = location.pathname.substr(1);
            var FLocation = (parentLocation + childLocation)
            var fevouriteLength = favourite == undefined ? 0 : favourite.length;

            for (z = 0; z < fevouriteLength; z++) {
                if (favourite[z].Report.virtualPath == FLocation) {
                    //  $("#submenu").text(favourite[z].Report.reportName);
                    $("#submenu").text(favourite[z].Report.reportName);
                    $("#group").text("Favourite");
                }
                childArr.push({ value: '/' + favourite[z].Report.virtualPath + "?fav=" + favourite[z].Report.reportName, text: favourite[z].Report.reportName });
            }
        }

        else {
            for (k = 0; k < parentLength; k++) {

                var newLength = data[k].Items.length;
                var childLocation = location.hash.substr(10);
                if (childLocation == "" || childLocation == "undefined") {
                    //$("#group").text("DashBoard");
                    var childLocationwords = window.location.pathname.split("/").pop(-1);
                    if (childLocationwords.length > 1) {
                        childLocation = '/' + childLocationwords
                    }
                    for (l = 0; l < newLength; l++) {

                        var pieces = data[k].Items[l].VIRTUAL_PATH.split("=");
                        var piece = '/' + pieces[pieces.length - 1]
                        //if (data[k].Items[l].VIRTUAL_PATH == childLocation) { 
                        if (piece == childLocation) {

                            $("#submenu").text(data[k].Items[l].MENU_EDESC);
                            // $('#submenu').data('disabled', true);
                            var childData = data[k].Items;
                            var childLength = data[k].Items.length;
                            for (x = 0; x < childLength; x++) {

                                if (childData[x].DASHBOARD_FLAG === 'Y') {
                                    childArr.push({ value: '/' + childData[x].VIRTUAL_PATH, text: childData[x].MENU_EDESC });
                                }
                            }
                        }
                    }
                }
                else {
                    var childLocationwords = childLocation.split("/");
                    if (childLocationwords.length > 1) {
                        childLocation = '/' + childLocationwords[1]
                    }
                    for (l = 0; l < newLength; l++) {

                        var pieces = data[k].Items[l].VIRTUAL_PATH.split(/[\s/]+/);
                        var piece = '/' + pieces[pieces.length - 1]
                        //if (data[k].Items[l].VIRTUAL_PATH == childLocation) { 
                        if (piece == childLocation) {

                            $("#submenu").text(data[k].Items[l].MENU_EDESC);
                            // $('#submenu').data('disabled', true);
                            var childData = data[k].Items;
                            var childLength = data[k].Items.length;
                            for (x = 0; x < childLength; x++) {

                                if (childData[x].DASHBOARD_FLAG === 'Y') {
                                    childArr.push({ value: '/' + childData[x].VIRTUAL_PATH, text: childData[x].MENU_EDESC });
                                }
                            }
                        }
                    }
                }



            }
        }
        // displayPopupNotification(data, "success");
        $.fn.editable.defaults.mode = 'inline';
        $('#group').editable({
            type: 'select',
            placement: 'top',
            showbuttons: false,
            source: myArr,
            success: function (response, successValue) {
            },
        });

        $('#submenu').editable({
            type: 'select',
            placement: 'top',
            showbuttons: false,
            source: childArr,
        });
    }, 1000);
    $("#group").click(function () {

        $(".input-medium").css({ "font-size": "14px", "padding": "2px", "padding-bottom": "2px", "height": "26px", "width": "131px" })
        setTimeout(function () {

            var group = $("#group").next().find("select");
            group.change(function (evt) {

                var currentValue = evt.currentTarget.value;
                if (currentValue == "Favourite") {
                    var childArr = [];
                    var fevouriteLength = favourite.length;
                    if (fevouriteLength == 0) {

                        childArr.push({ value: "Empty", text: "Empty" });
                        $('#submenu').editable('option', 'source', childArr);
                    }
                    for (z = 0; z < fevouriteLength; z++) {

                        childArr.push({ value: '/' + favourite[z].Report.virtualPath + "?fav=" + favourite[z].Report.reportName, text: favourite[z].Report.reportName });
                        $('#submenu').editable('option', 'source', childArr);
                    }
                }
                else {

                    selectedData;
                    var selectedLength = selectedData.length;
                    var childArr = [];
                    // childArr.push({ value: "Favourite", text: "Favourite" });
                    for (i = 0; i < selectedLength; i++) {

                        //myArr.push({ value: data[i].MENU_EDESC, text: data[i].MENU_EDESC });
                        if (selectedData[i].MENU_EDESC == currentValue) {
                            var childData = selectedData[i].Items;
                            var childLength = childData.length;
                            for (j = 0; j < childLength; j++) {
                                childArr.push({ value: '/' + childData[j].VIRTUAL_PATH, text: childData[j].MENU_EDESC });
                            }
                            $('#submenu').editable('option', 'source', childArr);
                        }
                    }
                }
            })
        }, 1
        )
    })

    //click function for the child 
    $("#submenu").click(function () {

        $(".input-medium").css({ "font-size": "14px", "padding": "2px", "padding-bottom": "2px", "height": "26px", "width": "131px" })
        setTimeout(function () {

            var subGroup = $("#submenu").next().find("select");
            subGroup.change(function (evt) {

                var firstLocation = location.href.substr(location.href.substr, 70);
                if (evt.currentTarget.value.substr(1).indexOf("fav") > -1 == true) {
                    var secondLocation = evt.currentTarget.value.substr(42);
                }
                else {
                    var secondLocation = evt.currentTarget.value.substr(1);
                }

                //var fullLocation = (firstLocation + secondLocation);
                //var fullLocation = (location.host + secondLocation);
                location.href = secondLocation
            });
        }, 1);
    });

    setTimeout(function () {
        var els = document.getElementsByTagName("a");
        var dd = window.location.href.substring(window.location.href);
        var hashlocation = window.location.hash;
        if (hashlocation == "" || hashlocation == "undefined") {
            var hashselectedURL = window.location.pathname.split("/")[window.location.pathname.split("/").length - 2];
            $("#group").text(hashselectedURL);
        }
        else {
            var lochash = hashlocation.substr(hashlocation.indexOf("!") + 1)
            var hashwords = lochash.split("/");
            if (hashwords.length > 2) {
                var hashselectedURL = "Sales Planning";
                $("#group").text(hashselectedURL);
            }
            else {
                for (var i = 0, l = els.length; i < l; i++) {
                    var el = els[i].href;
                    if (el === dd) {
                        var AbsulateUrl = (location.pathname + location.hash);
                        var name = $($("a[href='" + AbsulateUrl + "']").parents()[2]).find("a > span").html()
                        $("#group").text(name);
                    }
                }
            }

        }


    }, 1000);
}
