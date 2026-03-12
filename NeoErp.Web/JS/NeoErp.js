$("#header_inbox_bar").on("click", function () {
    //$.root_.on("click", , function(b) {
    //    a.launchFullscreen(document.documentElement), b.preventDefault()
    var a = document.documentElement;
    $.root_ = $(this);
    $.root_.hasClass("full-screen") ? ($.root_.removeClass("full-screen"), document.exitFullscreen ? document.exitFullscreen() : document.mozCancelFullScreen ? document.mozCancelFullScreen() : document.webkitExitFullscreen && document.webkitExitFullscreen()) : ($.root_.addClass("full-screen"), a.requestFullscreen ? a.requestFullscreen() : a.mozRequestFullScreen ? a.mozRequestFullScreen() : a.webkitRequestFullscreen ? a.webkitRequestFullscreen() : a.msRequestFullscreen && a.msRequestFullscreen())
});
jQuery(document).ready(function () {
    var a = document.documentElement;
    $.root_ = $("#header_inbox_bar");
    if ($.root_.hasClass("full-screen"))
    {
        document.onwebkitfullscreenchange = fullscreenChanged;
      goFullscreen();
    }
});


$(".non-negative-non-decimal").keypress(function (evt) {

    var keycode = evt.charCode || evt.keyCode;
    if (keycode == 45 || keycode == 43 || keycode == 46) { //Enter key's keycode
        return false;
    }
});

function goFullscreen() {
    // Must be called as a result of user interaction to work
    mf = document.getElementById("main_frame");
    mf.webkitRequestFullscreen();
    mf.style.display = "";
}

function fullscreenChanged() {
    if (document.webkitFullscreenElement == null) {
        mf = document.getElementById("main_frame");
        mf.style.display = "none";
    }
}

//document.onwebkitfullscreenchange = fullscreenChanged;
//document.documentElement.onclick = goFullscreen;
//document.onkeydown = goFullscreen;
function OpenWindow(query, w, h, scroll) {
    var l = (screen.width - w) / 2;
    var t = (screen.height - h) / 2;

    winprops = 'resizable=0, height=' + h + ',width=' + w + ',top=' + t + ',left=' + l + 'w';
    if (scroll) winprops += ',scrollbars=1';
    var f = window.open(query, "_blank", winprops);
}

function setLocation(url) {
    window.location.href = url;
}

function displayAjaxLoading(display) {
    if (display) {
        $('.ajax-loading-block-window').show();
    }
    else {
        $('.ajax-loading-block-window').hide('slow');
    }
}

function displayPopupNotification(message, messagetype) {

    toastr.options = {
        closeButton: true,
        progressBar: true,
        positionClass: 'toast-top-left',
        preventDuplicates: true,
        showDuration: 600,
        hideDuration: 1000,
        timeOut: 5000,
        showEasing: 'swing',
        hideEasing: 'linear',
        showMethod: 'fadeIn',
        hideMethod: 'fadeOut',
    };

    // "success", "info", "warning" or "error"

    var cssclass = 'success';
    if (messagetype == 'success') {
        cssclass = 'success';
    }
    else if (messagetype == 'error') {
        cssclass = 'error';
    }
    else if (messagetype == 'warning') {
        cssclass = 'warning';
    }
    else if (messagetype == 'info') {
        cssclass = 'info';
    }

    toastr[cssclass](message, cssclass);
}


var barNotificationTimeout;
function displayBarNotification(message, messagetype,showDuration,hideDuration, timeout) {
    toastr.options = {
        closeButton: true,
        progressBar: false,
        positionClass: 'toast-top-right',
        showDuration: 0 || showDuration,
        hideDuration: 0 || hideDuration,
        timeOut: 0 || timeout,
        showEasing: 'swing',
        hideEasing: 'linear',
        showMethod: 'fadeIn',
        hideMethod: 'fadeOut',
        extendedTimeOut: 0 || timeout
    };
    // "success", "notice", "warning" or "error"

    var cssclass = 'success';
    if (messagetype == 'success') {
        cssclass = 'success';
    }
    else if (messagetype == 'error') {
        cssclass = 'error';
    }
    else if (messagetype == 'warning') {
        cssclass = 'warning';
    }
    else if (messagetype == 'info') {
        cssclass = 'info';
    }

    toastr[cssclass](message, cssclass);

}
//subin changes
function DisplayBarNotificationMessage(message) {
    //$.notifyBar({ html: message, close: false, waitingForClose: false, closeOnClick: false, cssClass: "success", display:2000 });
    $.notifyBar({ html: message, close: true, waitingForClose: true, closeOnClick: true, cssClass: "success" });
}
function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}


// CSRF (XSRF) security
function addAntiForgeryToken(data) {
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};

function ajaxGridMVcSorting(selector) {
    var url = $("#baseUrl").val() + $(selector).attr("href");
    $.get(url, function (data) {
        $("#operationmanager").html(data);
    });
}
function PostFormDataWithAjax(form, contailer) {
    var postUrl = $(form).attr('action');
    var formdata = $(form).serialize()
    $.ajax({
        cache: false,
        url: postUrl,
        type: 'Post',
        data: formdata,
        success: function (response) {
            $(contailer).html(response);
            displayPopupNotification("Update Sucessfully");
            return false;
        },
        complete: this.resetLoadWaiting,
        error: function (response) {

            displayPopupNotification(response, 'error');
            return false;
        },
    });
};
var AjaxLoad = {
    usepopupnotifications: true,
    usebarnotifications: false,
    partialviewcontainer: "#operationmanager",
    init: function (mainContatin) {
        this.partialviewcontainer = mainContatin;

    },
    setLoadWaiting: function (display) {
        displayAjaxLoading(display);
    },
    // sumit form with ajax
    PostFormDataWithAjax: function (urladd, container, formdata) {
        this.setLoadWaiting(true);
        this.partialviewcontainer = container
        $.ajax({
            cache: false,
            url: urladd,
            type: 'Post',
            data: formdata,
            success: function (response) {
                $(container).html(response);
                displayBarNotification("Update Sucessfully");
            },
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },

    loadPartialViewWithAjax: function (urladd, container) {
        this.setLoadWaiting(true);
        //this.partialviewcontainer=container
        $.ajax({
            cache: false,
            url: urladd,
            type: 'get',
            success: this.success_loadpartialView,
            complete: this.resetLoadWaiting,
            error: this.ajaxFailure
        });
    },
    success_loadpartialView: function (response) {
        if (response.updatetopcartsectionhtml) {
            $(AjaxCart.topcartselector).html(response.updatetopcartsectionhtml);
        }
        if (response.updatetopwishlistsectionhtml) {
            $(AjaxCart.topwishlistselector).html(response.updatetopwishlistsectionhtml);
        }
        if (response.updateflyoutcartsectionhtml) {
            $(AjaxCart.flyoutcartselector).replaceWith(response.updateflyoutcartsectionhtml);
        }
        if (response.message) {
            //display notification
            if (response.success == true) {
                //success
                if (AjaxCart.usepopupnotifications == true) {
                    displayPopupNotification(response.message, 'success', true);
                }
                else {
                    //specify timeout for success messages
                    displayBarNotification(response.message, 'success', 3500);
                }
            }
            else {
                //error
                if (AjaxCart.usepopupnotifications == true) {
                    displayPopupNotification(response.message, 'error', true);
                }
                else {
                    //no timeout for errors
                    displayBarNotification(response.message, 'error', 0);
                }
            }
            return false;
        }
        if (response.redirect) {
            location.href = response.redirect;
            return true;
        }
        return false;
    },
    success_process: function (response) {

        if (response.message) {
            //display notification
            if (response.success == true) {
                //success
                if (AjaxCart.usepopupnotifications == true) {
                    displayPopupNotification(response.message, 'success', true);
                }
                else {
                    //specify timeout for success messages
                    displayBarNotification(response.message, 'success', 3500);
                }
            }
            else {
                //error
                if (AjaxCart.usepopupnotifications == true) {
                    displayPopupNotification(response.message, 'error', true);
                }
                else {
                    //no timeout for errors
                    displayBarNotification(response.message, 'error', 0);
                }
            }
            return false;
        }
        //if (response.redirect) {
        //    location.href = response.redirect;
        //    return true;
        //}
        return false;
    },

    resetLoadWaiting: function () {
        AjaxCart.setLoadWaiting(false);
    },

    ajaxFailure: function (response) {
        if (AjaxCart.usepopupnotifications == true) {
            displayPopupNotification(response.message, 'error', true);
        }
        else {
            //no timeout for errors
            displayBarNotification(response.message, 'error', 0);
        }

    }
};