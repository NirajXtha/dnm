$(document).ready(function () {

    
    var data = document.ParentData;

    
    var favourite = document.globalFavourite;
    $(".page-breadcrumb").prepend(" <li style='display:inline-block;list-style:none;'><a href='#' id='submenu' style='font-size:14px'></a></li> ");
    $(".page-breadcrumb").prepend("<li style='display:inline-block;list-style:none;'><a href='#' id='group' style='font-style:normal !important;font-size:14px;'></a>   <i class='fa fa-angle-right'></i></li>");
    $(".page-breadcrumb").prepend("<li style='display:inline-block;list-style:none;'><a href='sales/saleshome/dashboard' id='dfds' style='font-style:normal !important;font-size:14px;'>Organiser</a>   <i class='fa fa-angle-right'></i></li>");
    $(".page-breadcrumb").prepend("<li style='display:inline-block;list-style:none;'> <i class='fa fa-home'></i><a href='/main/DashBoard' id='sdfsd' style='font-style:normal !important;font-size:14px;'>Home</a>   <i class='fa fa-angle-right'></i></li>");

    //Glob css for all page 
    $("#group").css({ "border-bottom": "none" });
    $("#submenu").css({ "border-bottom": "none" });

    var newvalue = "";
    var selectedData = "";
    var childArr = "";
    //$("#group").text(newvalue);

    //  $.ajax({
    //    type: 'GET',
    //    dataType: "json",
    //    url: "/api/SalesHome/GetDynamicMenu?ModuleCode=" + moduleCode,
    //    success: function (data) {
    //        debugger;
    //        selectedData = data;
    //        var mydata = data.length;
    //        var myArr = [];
    //        myArr.push({ value: "Favourite", text: "Favourite" });
    //        for (i = 0; i < mydata; i++) {
    //            myArr.push({ value: data[i].MENU_EDESC, text: data[i].MENU_EDESC });
    //        }

    //        var mydata = data.length;
    //        var myArrr = [];
    //        myArrr.push({ value: "hehe", text: "huhu" });
    //        for (i = 0; i < mydata; i++) {
    //            myArrr.push({ value: data[i].MENU_EDESC, text: data[i].MENU_EDESC });
    //        }



    //        //Getting value from child and set into the field

    //        var selectedURL = $($("a[href='/" + location.hash + "']").parents()[2]).find("a > span").html()
    //        var parentLength = data.length;
    //        var childArr = [];
    //        for (k = 0; k < parentLength; k++) {
    //            if (data[k].MENU_EDESC == selectedURL)
    //            {
    //                var newLength = data[k].Items.length;
    //                var childLocation = location.hash;
    //                debugger;
    //                for(l=0;l<newLength;l++)
    //                {
    //                    if (data[k].Items[l].VIRTUAL_PATH == childLocation)
    //                    {
    //                        $("#submenu").text(data[k].Items[l].MENU_EDESC)
    //                        // $('#submenu').data('disabled', true);
    //                        var childData = data[k].Items
    //                        var childLength = data[k].Items.length;
    //                        for (x = 0; x < childLength; x++) {
    //                            childArr.push({ value: '/' + childData[x].VIRTUAL_PATH, text: childData[x].MENU_EDESC });
    //                        }

    //                    }
    //                }


    //            }
    //        }


    //        // displayPopupNotification(data, "success");
    //        $.fn.editable.defaults.mode = 'inline';
    //        $('#group').editable({
    //            type: 'select',
    //            placement: 'top',
    //            showbuttons: false,
    //            //value: "dfdfd",
    //            source: myArr,
    //            success: function (response, successValue) {
    //                debugger;
    //                //$("#Response").load("../ajax/get/{{issue_obj.id}}/response/");
    //            },
    //            // { value: 1, text: data[0].MENU_EDESC },
    //            //uncomment these lines to send data on server
    //            // ,pk: 1
    //            // ,url: '/post'
    //        });

    //        $('#submenu').editable({
    //            type: 'select',
    //            placement: 'top',
    //            showbuttons: false,
    //            source: childArr,
    //            // { value: 1, text: data[0].MENU_EDESC },
    //            //uncomment these lines to send data on server
    //            // ,pk: 1
    //            // ,url: '/post'
    //        });

    //    }
    //});
    selectedData = data;
    var mydata = data.length;
    var myArr = [];
    myArr.push({ value: "Favourite", text: "Favourite" });
    for (i = 0; i < mydata; i++) {
        myArr.push({ value: data[i].MENU_EDESC, text: data[i].MENU_EDESC });
    }

    var mydata = data.length;
    var myArrr = [];
    myArrr.push({ value: "hehe", text: "huhu" });
    for (i = 0; i < mydata; i++) {
        myArrr.push({ value: data[i].MENU_EDESC, text: data[i].MENU_EDESC });
    }



    //Getting value from child and set into the field

    var selectedURL = $($("a[href='/" + location.hash + "']").parents()[2]).find("a > span").html()
    var parentLength = data.length;
    var childArr = [];
    if (selectedURL == "Favourite") {
        var childArr = [];
        var childLocation = location.hash.substring(0, location.hash.indexOf('?'));
        var fevouriteLength = favourite.length;
        for (z = 0; z < fevouriteLength; z++) {
            if (favourite[z].Report.virtualPath == childLocation) {
                //  $("#submenu").text(favourite[z].Report.reportName);
                $("#submenu").text(favourite[z].Report.reportName);
            }
            childArr.push({ value: '/' + favourite[z].Report.virtualPath + "?fav=" + favourite[z].Report.reportName, text: favourite[z].Report.reportName });
        }

    }
    else {
        for (k = 0; k < parentLength; k++) {
            if (data[k].MENU_EDESC == selectedURL) {
                var newLength = data[k].Items.length;
                var childLocation = location.hash;
                for (l = 0; l < newLength; l++) {
                    if (data[k].Items[l].VIRTUAL_PATH == childLocation) {
                        $("#submenu").text(data[k].Items[l].MENU_EDESC);
                        // $('#submenu').data('disabled', true);
                        var childData = data[k].Items;
                        var childLength = data[k].Items.length;
                        for (x = 0; x < childLength; x++) {
                            childArr.push({ value: '/' + childData[x].VIRTUAL_PATH, text: childData[x].MENU_EDESC });
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
                     childArr = [];
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
                var childValList = evt.currentTarget.value
                window.location.href = childValList
            });
        }, 1);
    });
    var els = document.getElementsByTagName("a");
    var dd = window.location.href.substring(window.location.href);
    for (var i = 0, l = els.length; i < l; i++) {
        var el = els[i];
        if (el.href === dd) {
            var name = $($("a[href='/" + location.hash + "']").parents()[2]).find("a > span").html()
            newvalue = name;
            $("#group").text(newvalue);
        }
    }
});
var UrlName = window.location.href.substring(window.location.href.lastIndexOf('/') + 1);
$("#hehe").html(UrlName);
