var appsummay = $.sammy("#operationContent", function () {

   
    var $this = this;

    var loader = "<div class=''><img src='/images/ring.gif' style='margin-left: 41%; margin-top: 35%; width: 50px;'  /></div>";
    var opContainer = $("#operationContent");
    var dynamicUrl = window.location.protocol + "//" + window.location.host + "/api/MenuSettings/GetMenuRouting?moduleCode=01";
    $.ajax({
        url: dynamicUrl,
        error: function () {

        },
        dataType: 'json',
        success: function (data) {
            console.log("foreach", opContainer);
            $.each(data, function (i, e) {
                //if (e.VIRTUAL_PATH.indexOf('#') != -1) {
                    $this.get(e.VIRTUAL_PATH, function (context) {
                        opContainer.html(loader).load(e.FULL_PATH, mobileAutoClick());
                        return false;
                    });
                //}                
            })
           // appRunSammy();
        },
        type: 'GET'
    });
    //$.get("#", function (data, status) {
    //    debugger;
    //    return true;
    //    //var url = getUrlFromHash(context.path);
    //  //  context.load(url).swap();

    //  //  app.setLocation('#/Calendar/CalendarReport');
    //   // redirect('#/other/route');
    //  // redirect('#/Calendar/CalendarReport');
    //    //session check through javascript
    //    //true
    //    //if true rediect #dashboard
    //    //if false login

    //});
    this.get("#/Calendar/CalendarReport", function (context) {
        $("#operationContent").html(loader).load('sales/CalendarReport/index', mobileAutoClick());
        return false;
    });
    //this.get("/sales/saleshome/dashboard/#/Sales/FinalSalesReport", function (context) {
    //    $("#operationContent").html(loader).load('/Sales/SalesHome/FinalSalesReport', mobileAutoClick());
    //    return false;
    //});
    this.get("#/Calendar/weekCalendarReport", function (context) {
        $("#operationContent").html(loader).load('sales/CalendarReport/weekIndex', mobileAutoClick());
        return false;
    });
    this.get("#/Calendar/newweekCalendarReport", function (context) {
        $("#operationContent").html(loader).load('sales/CalendarReport/FinalweekIndex', mobileAutoClick());
        return false;
    });
    this.get("#/Calendar/CalendarReportParam/:id", function (context) {
        var id = this.params['id'];
        $("#operationContent").html(loader).load('sales/CalendarReport/CalendarReportIndex?id=' + id, mobileAutoClick());
        return false;
    });

});

$(document).ready(function () {
    console.log("ready",$("#operationContent").length);
    appsummay.run();
    console.log(appsummay);
});
function appRunSammy() {
}
function mobileAutoClick() {
    //if ($('#sidebar').parent().hasClass('slimScrollDiv')) {
    //    $("#sidebar-collapse i").trigger('click');
    //}
}
function getUrlFromHash(hash) {

    var url = hash.replace('#/', '');
    if (url === appRoot)
        url = defaultRoute;

    return url;
}

