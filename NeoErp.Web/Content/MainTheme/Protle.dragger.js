var PortletDraggable = function () {
    return {
        init: function (type) {
            jQuery().sortable && $("#sortable_portlets").sortable({

                //connectWith: ".portlet",
                connectWith: ".sortable",
                items: ".portlet",
                opacity: .8,
                handle: ".portlet-title",
                coneHelperSize: !0,
                placeholder: "portlet-sortable-placeholder",
                forcePlaceholderSize: !0,
                tolerance: "pointer",
                helper: "clone",
                //tolerance: "pointer",
                //forcePlaceholderSize: !0,
                //helper: "clone",
                cancel: ".portlet-sortable-empty, .portlet-fullscreen",
                revert: 250,
                update: function (e, t) {
                    t.item.prev().hasClass("portlet-sortable-empty") && t.item.prev().before(t.item);
                    var orderArray = $("#sortable_portlets").sortable("toArray");
                    var order = orderArray.filter(function (value) {
                        return value !== "" && value !== null;
                    });
                    var temp = order[1];
                    order[1] = order[2];
                    order[2] = temp;
                    saveToDB(order.join(), type);
                    // $.cookie("sortableOrder", order);
                }
            });
            // loadFromDB();
        }
    }
}();



//jQuery(document).ready(function () { PortletDraggable.init() });

function saveToDB(order, type) { 
    var urltest = Arg.url(window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/SaveDashboardWidgets", { 'order': order, 'type': type });
    $.ajax({
        url: urltest,
        type: "GET",
        dataType: "html",
        success: function (result) {
        },
        error: function (result) {
        }
    });
}



function loadFromDB() {
    var returnValue;
    var foo = $("#sortable_portlets");
    var urltest = Arg.url(window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/GetDashboardWidgets", { 'userId': 'Ashok' });
    $.ajax({
        url: urltest,
        type: "GET",
        dataType: "html",
        success: function (result) {
            if (result) {
                $.each(result.split(','), function (number, id) {
                    var id = id.replace(/\"/g, "");
                    $("#" + id).closest('.sortable').appendTo($("#sortable_portlets"));
                });
            }
        },
        error: function (result) {
        }
    });
}

$("#reset_Sales_widgets").on('click', function () {
    
    var param = window.location.href.split('/').pop() + "_sorted";
    var resetUrl = Arg.url(window.location.protocol + "//" + window.location.host + "/api/SalesDashboard/ResetDashboardWidgets", { 'type': "'" + param + "','Dashboard_Widgets_SpedoMeter'" });
    $.ajax({
        url: resetUrl,
        type: "GET",
        dataType: "html",
        success: function (result) {
            location.reload();
        },
        error: function (result) {
        }
    });
});









