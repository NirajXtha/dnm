function ManualEmailSender(config) {
    var mythis;
    var _gridName = "grid";
    var _gridRefreshTriggerName = "RunQuery";
    var _reportName = "file";
    var _submitButtonName = "sendMail";
    var _submitFormName = "sendMailForm";
    var _fileUploaded = false;
    var _isSending = false; // Guard: prevents multiple concurrent send operations
    var _exportFunction = null; // optional custom workbook builder function
    var _responseMessage = "responseMessage";
    var _emailFormField = {
        emailAddress: "Email",
        subject: "Subject",
        fileType: "FileType",
        message: "Message",
        fileName: "FileName"
    }

    var _saveUrl = Metronic.getGlobalUrl() + "api/Common/Save";
    var _messageSaveUrl = Metronic.getGlobalUrl() + "api/Common/SaveMessage";
    if (config != undefined && config != null) {
        // Accept both PascalCase (GridName) and camelCase (gridName) for all config keys
        _gridName = config.GridName != undefined ? config.GridName
            : config.gridName != undefined ? config.gridName : _gridName;
        _gridRefreshTriggerName = config.GridRefreshTriggerName != undefined ? config.GridRefreshTriggerName
            : config.gridRefreshTriggerName != undefined ? config.gridRefreshTriggerName : _gridRefreshTriggerName;
        _reportName = config.reportName != undefined ? config.reportName : _reportName;
        _submitButtonName = config.SubmitButtonName != undefined ? config.SubmitButtonName
            : config.submitButtonName != undefined ? config.submitButtonName : _submitButtonName;
        _submitFormName = config.SubmitFormName != undefined ? config.SubmitFormName
            : config.submitFormName != undefined ? config.submitFormName : _submitFormName;
        _responseMessage = config.ResponseMessage != undefined ? config.ResponseMessage
            : config.responseMessage != undefined ? config.responseMessage : _responseMessage;
        _exportFunction = config.exportFunction != undefined ? config.exportFunction
            : config.ExportFunction != undefined ? config.ExportFunction : _exportFunction;
        if (config.emailFormField != undefined) {
            var emailFields = config.emailFormField;
            _emailFormField.emailAddress = emailFields.emailAddress != undefined ? emailFields.emailAddress : _emailFormField.emailAddress;
            _emailFormField.subject = emailFields.subject != undefined ? emailFields.subject : _emailFormField.subject;
            _emailFormField.fileType = emailFields.fileType != undefined ? emailFields.fileType : _emailFormField.emailAddress;
            _emailFormField.message = emailFields.message != undefined ? emailFields.message : _emailFormField.message;
            _emailFormField.fileName = emailFields.fileName != undefined ? emailFields.fileName : _emailFormField.fileName;
        }
    }

    var generateFileName = function (fileExtension) {
        var date = new Date();
        return _reportName + "_" + date.getTime().toString() + "." + fileExtension;
    };

    var postfile = function () {
        // Prevent multiple simultaneous send operations (fixes file-lock and duplicate email bugs)
        if (_isSending) return;

        if (_fileUploaded) {
            $("#" + _submitButtonName).button("loading");
            $("#" + _submitFormName).submit();
            return;
        }
        var fileExtention = $("#" + _emailFormField.fileType + ":checked") != undefined ? $("#" + _emailFormField.fileType + ":checked").val() : "xlsx";

        // Check if it's a Grid or TreeList
        var resolvedGridName = typeof _gridName === 'function' ? _gridName() : _gridName;
        var grid = $("#" + resolvedGridName).data("kendoGrid");
        var treeList = $("#" + resolvedGridName).data("kendoTreeList");
        var widget = grid || treeList;

        if (!widget) {
            var elementExists = $("#" + resolvedGridName).length > 0;
            var message = elementExists
                ? "Please run the report first before sending mail. Click the Run (&#9654;) button to load the report data."
                : "Report grid not found. Please run the report first before sending mail.";
            Metronic.alert({
                container: "#" + _responseMessage,
                place: "prepend",
                type: "warning",
                message: message,
                close: true,
                reset: true,
                focus: true,
                closeInSeconds: 8,
                icon: "warning"
            });
            return;
        }

        var file = generateFileName(fileExtention);

        // ============================================================
        // SHARED HELPERS — used by both the _exportFunction path and
        // the default Grid/TreeList path below.
        // ============================================================

        // Upload a base64 string to the server, then submit the email form.
        var uploadBase64 = function (base64, onComplete) {
            if (!base64) {
                if (onComplete) onComplete();
                $("#" + _submitButtonName).button("reset");
                return;
            }
            _isSending = true;
            $("#" + _submitButtonName).button("loading");
            $.post(_saveUrl, { base64: base64, fileName: file })
                .done(function (response) {
                    _isSending = false;
                    if (onComplete) onComplete();
                    if (response.Success) {
                        _fileUploaded = true;
                        $("#" + _emailFormField.fileName).val(file);
                        $("#" + _gridRefreshTriggerName).trigger("click");
                        $("#" + _submitFormName).submit();
                    } else {
                        $("#" + _submitButtonName).button("reset");
                        Metronic.alert({
                            container: "#" + _responseMessage,
                            place: "prepend",
                            type: "danger",
                            message: response.Message,
                            close: true,
                            reset: true,
                            focus: true,
                            closeInSeconds: 0,
                            icon: "warning"
                        });
                    }
                })
                .fail(function () {
                    _isSending = false;
                    if (onComplete) onComplete();
                    $("#" + _submitButtonName).button("reset");
                });
        };

        // Resolve a dataURL into base64 (handles both data: URIs and blob: URLs)
        // and upload it.  onComplete is called after upload succeeds or fails.
        var processDataURL = function (dataURL, onComplete) {
            if (!dataURL) { if (onComplete) onComplete(); return; }
            if (typeof dataURL === "string" && dataURL.indexOf(";base64,") !== -1) {
                uploadBase64(dataURL.split(";base64,")[1], onComplete);
            } else if (typeof dataURL === "string" && dataURL.indexOf("blob:") === 0) {
                var xhr = new XMLHttpRequest();
                xhr.open("GET", dataURL, true);
                xhr.responseType = "arraybuffer";
                xhr.onload = function () {
                    var bytes = new Uint8Array(xhr.response);
                    var binary = "";
                    for (var b = 0; b < bytes.byteLength; b++) binary += String.fromCharCode(bytes[b]);
                    uploadBase64(window.btoa(binary), onComplete);
                };
                xhr.send();
            } else {
                if (onComplete) onComplete();
            }
        };

        // Dispatch toDataURL result — handles all three Kendo variants:
        //   Sync   : toDataURL() returns a string immediately
        //   Promise: toDataURL() returns a Promise (modern Kendo)
        //   Callback: toDataURL(fn) calls fn with the result (legacy Kendo)
        var dispatchToDataURL = function (workbookObj, onDataURL) {
            var result = workbookObj.toDataURL();
            if (result && typeof result.then === "function") {
                result.then(function (dataURL) { onDataURL(dataURL); });
            } else if (result && typeof result === "string") {
                onDataURL(result);
            } else {
                workbookObj.toDataURL(onDataURL);
            }
        };

        // ============================================================
        // PATH 1 — Custom exportFunction (lazy-load TreeLists)
        // The caller supplies exportFunction() which returns a kendo.ooxml.Workbook
        // already containing all custom headers (Company, Branch, Date Range, etc.).
        // We just need to call toDataURL() correctly (async-safe) and upload it.
        // ============================================================
        if (_exportFunction && typeof _exportFunction === 'function') {
            var workbook = _exportFunction();
            if (!workbook) return;
            $("#" + _submitButtonName).button("loading");
            dispatchToDataURL(workbook, function (dataURL) {
                processDataURL(dataURL, null);
            });
            return;
        }

        // ============================================================
        // PATH 2 — Default Grid / TreeList path
        //          (saveAsExcel() triggers the excelExport event)
        //
        // KEY INSIGHT: The manual Excel export ALREADY produces the correct file
        // (single set of custom headers, correct data) via the report's own
        // excelExport handler. We should NOT re-implement any workbook logic.
        //
        // APPROACH — simply redirect kendo.saveAs to upload instead of download:
        //   1. Stub kendo.saveAs globally BEFORE saveAsExcel() is called
        //   2. The report's excelExport handler runs UNCHANGED:
        //        - e.preventDefault() suppresses the browser Save-As dialog
        //        - custom header rows are added (company, branch, date, etc.)
        //        - kendo.saveAs({ dataURI: workbook.toDataURL() }) is called
        //   3. Our stub captures the dataURI (may be a Promise in modern Kendo)
        //   4. We .then() the Promise — stub stays active → no ghost download
        //   5. Upload the resolved base64, then restore kendo.saveAs
        // ============================================================

        var originalKendoSaveAs = kendo.saveAs;
        var originalExcelExport = widget.options.excelExport;
        var _saveAsCaptured = false;

        // Restore kendo.saveAs (called after upload completes or on error)
        var restoreKendoSaveAs = function () {
            kendo.saveAs = originalKendoSaveAs;
        };

        // Stub kendo.saveAs BEFORE saveAsExcel() so it is already in place when
        // the report's excelExport handler calls it after building the workbook.
        // We keep it stubbed (blocking all calls) for the full async lifetime so
        // Kendo's internal Promise resolution never triggers a real browser download.
        kendo.saveAs = function (options) {
            if (_saveAsCaptured) {
                // Already handled the first call — silently block any subsequent
                // calls (e.g. from allPages:true double-fire) to prevent ghost downloads.
                return;
            }
            _saveAsCaptured = true;

            var dataURIOrPromise = (options && options.dataURI !== undefined) ? options.dataURI : null;

            if (!dataURIOrPromise) {
                restoreKendoSaveAs();
                $("#" + _submitButtonName).button("reset");
                return;
            }

            if (typeof dataURIOrPromise.then === "function") {
                // Modern Kendo: toDataURL() returned a Promise.
                // Keep stub ACTIVE until the Promise resolves — this prevents Kendo's
                // internal .then() callback from reaching the real kendo.saveAs.
                dataURIOrPromise.then(function (dataURL) {
                    restoreKendoSaveAs(); // safe to restore now — Promise is fully resolved
                    processDataURL(dataURL, null);
                });
            } else if (typeof dataURIOrPromise === "string") {
                // Older Kendo: toDataURL() returned a data URI string synchronously.
                restoreKendoSaveAs();
                processDataURL(dataURIOrPromise, null);
            } else {
                restoreKendoSaveAs();
            }
        };

        if (originalExcelExport) {
            // The report has its own excelExport handler — it already calls e.preventDefault()
            // and kendo.saveAs({ dataURI: ... }) with the correctly-built workbook.
            // Our global kendo.saveAs stub above will capture the call automatically.
            // We do NOT need to replace the handler — just run saveAsExcel() as-is.
        } else {
            // No custom excelExport handler — use widget.one() instead of setOptions()
            // because Kendo TreeList in some production versions does NOT expose setOptions(),
            // which is what causes the "Cannot read properties of undefined (reading 'setOptions')" error.
            var _noHandlerExportDone = false;

            // widget.one() binds a one-time event handler using the Kendo event system,
            // which works on both kendoGrid and kendoTreeList across all versions.
            widget.one("excelExport", function (e) {
                if (_noHandlerExportDone) { e.preventDefault(); return; }
                _noHandlerExportDone = true;
                e.preventDefault();
                _saveAsCaptured = true; // prevent the stub from firing
                restoreKendoSaveAs();   // restore immediately (stub not needed)
                var wb = new kendo.ooxml.Workbook({ sheets: e.workbook.sheets });
                dispatchToDataURL(wb, function (dataURL) { processDataURL(dataURL, null); });
            });
        }
        widget.saveAsExcel();


    }

    this.getFormData = function () {
        return {
            Email: $("#" + _emailFormField.emailAddress).val(),
            FileName: $("#" + _emailFormField.fileName).val(),
            Subject: $("#" + _emailFormField.subject).val(),
            Message: $("#" + _emailFormField.message).val()
        }
    }
    this.submitForm = function (form) {
        $.ajax({
            type: form.method,
            url: form.action,
            data: myThis.getFormData(),
            dataType: 'json'
        }).done(function (response) {
            if (response.Success) {
                Metronic.alert({
                    container: "#" + _responseMessage, // alerts parent container(by default placed after the page breadcrumbs)
                    place: "prepend", // append or prepent in container 
                    type: "success",  // alert's type
                    message: response.Message,  // alert's message
                    close: true, // make alert closable
                    reset: true, // close all previouse alerts first
                    focus: true, // auto scroll to the alert after shown
                    closeInSeconds: 10, // auto close after defined seconds
                    icon: "check" // put icon before the message
                });
                $("#" + _submitButtonName).button("reset");
                myThis.resetForm(form);
            }
            else {
                Metronic.alert({
                    container: "#" + _responseMessage, // alerts parent container(by default placed after the page breadcrumbs)
                    place: "prepend", // append or prepent in container 
                    type: "danger",  // alert's type
                    message: response.Message,  // alert's message
                    close: true, // make alert closable
                    reset: true, // close all previouse alerts first
                    focus: true, // auto scroll to the alert after shown
                    closeInSeconds: 0, // auto close after defined seconds
                    icon: "warning" // put icon before the message
                });
                $("#" + _submitButtonName).button("reset");
            }
        });
    }

    ManualEmailSender.prototype.registerInstance = function () {
        myThis = this;
    }
    var validator = null;

    var formReset = function (form) {
        $(form).find("input[type=text], textarea, input[type=hidden]").each(function (index, element) {
            $(element).val("");
        });
        $('#' + _emailFormField.message).code('');
        _fileUploaded = false;
        _isSending = false; // Always reset send guard when form is reset/closed
        validator.resetForm();
    }

    this.resetForm = function (form) {
        formReset(form);
    }
    this.init = function () {
        this.registerInstance();
        $("#" + _emailFormField.message).summernote({
            height: 200,
            toolbar: [
                ['style', ['style']],
                ['font', ['bold', 'italic', 'underline', 'clear']],
                ['fontname', ['fontname']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['table', ['table']],
                ['insert', ['hr']],
                ['view', ['fullscreen', 'codeview']]]

        });

        $("input[name='" + _emailFormField.fileType + "']").iCheck({
            radioClass: "iradio_square-grey"
        });

        $("#" + _submitButtonName).off("click.mailSender").on("click.mailSender", function (e) {
            postfile();
        });

        $("#" + _submitFormName).attr("action", _messageSaveUrl);
        var $modal = $("#" + _submitFormName).closest(".modal");

        $($modal).on('hidden.bs.modal', function () {
            formReset($("#" + _submitFormName));
        });

        validator = $("#" + _submitFormName).validate({
            // ajax submit
            errorElement: 'span', //default input error message container
            errorClass: 'help-block help-block-error', // default input error message class
            focusInvalid: false, // do not focus the last invalid input
            ignore: "input[class|='note'], textarea[class|='note'] ", // validate all fields including form hidden input
            submitHandler: function (form) {
                myThis.submitForm(form);
                return false;
            },
            invalidHandler: function (event, validator) { //display error alert on form submit              
                $("#" + _submitButtonName).button("reset");
            },

            errorPlacement: function (error, element) { // render error placement for each input type
                if (element.parent(".input-group").size() > 0) {
                    error.insertAfter(element.parent(".input-group"));
                } else if (element.attr("data-error-container")) {
                    error.appendTo(element.attr("data-error-container"));
                } else if (element.parents('.radio-list').size() > 0) {
                    error.appendTo(element.parents('.radio-list').attr("data-error-container"));
                } else if (element.parents('.radio-inline').size() > 0) {
                    error.appendTo(element.parents('.radio-inline').attr("data-error-container"));
                } else if (element.parents('.checkbox-list').size() > 0) {
                    error.appendTo(element.parents('.checkbox-list').attr("data-error-container"));
                } else if (element.parents('.checkbox-inline').size() > 0) {
                    error.appendTo(element.parents('.checkbox-inline').attr("data-error-container"));
                } else {
                    error.insertAfter(element); // for other inputs, just perform default behavior
                }
            },

            highlight: function (element) { // hightlight error inputs
                $(element).closest('.form-group').addClass('has-error'); // set error class to the control group  
            },

            unhighlight: function (element) { // revert the change done by hightlight
                $(element).closest('.form-group').removeClass('has-error'); // set error class to the control group
            },

            success: function (label, element) {
                label.closest('.form-group').removeClass('has-error'); // set success class to the control group
            },
        });

        $("#" + _emailFormField.emailAddress).rules("add", {
            required: true,
        });
        $("#" + _emailFormField.emailAddress).rules("add", {
            multipleEmail: true,
        });

        $("#" + _emailFormField.fileName).rules("add", {
            required: true,
        });

        $("#" + _emailFormField.subject).rules("add", {
            required: true
        });
        $("#" + _emailFormField.message).rules("add", {
            required: true
        });
    }
}