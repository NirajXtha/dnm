/**
 * Universal Grid Print Utility
 * This utility provides a reusable print function for Kendo Grid reports
 * Usage: Call UniversalGridPrint.init() after your grid is initialized
 */

var UniversalGridPrint = (function () {

    // Configuration object that can be customized per report
    var config = {
        gridSelector: '#grid',
        companyNameSelector: null,
        branchNameSelector: null,
        reportName: '',
        fromDateSelector: '#FromDateVoucher',
        toDateSelector: '#ToDateVoucher',
        fromBSDateSelector: '#fromInputDateVoucher',
        toBSDateSelector: '#toInputDateVoucher',
        includeExportColumns: true,
        customColumns: null
    };

    function init(options) {
        if (options) {
            $.extend(config, options);
        }

        $(document).off("click", ".k-grid-print").on("click", ".k-grid-print", function (e) {
            e.preventDefault();
            printReport();
        });
    }

    /**
     * Main print function - Updated to use hidden iframe
     */
    function printReport() {
        var grid = $(config.gridSelector).data("kendoGrid");
        var treeList = $(config.gridSelector).data("kendoTreeList");
        var widget = grid || treeList;

        if (!widget) {
            console.error("UniversalGridPrint: No Grid or TreeList widget found!");
            return;
        }

        try {
            var reportName = typeof config.reportName === 'function' ? config.reportName() : (config.reportName || getReportName());
            var companyName = typeof config.companyName === 'function' ? config.companyName() : (config.companyNameSelector ? $(config.companyNameSelector).text() : (config.companyName || getCompanyName()));
            var branchName = typeof config.branchName === 'function' ? config.branchName() : (config.branchNameSelector ? $(config.branchNameSelector).text() : (config.branchName || getBranchName()));

            var fromADdate = $(config.fromDateSelector).val() || '';
            var toADdate = $(config.toDateSelector).val() || '';
            var fromBSdate = $(config.fromBSDateSelector).val() || '';
            var toBSdate = $(config.toBSDateSelector).val() || '';

            var dataSource = widget.dataSource;
            var allData = dataSource.data();
            var aggregates = dataSource.aggregates();

            var printContent = buildPrintContent(
                reportName, companyName, branchName,
                fromADdate, toADdate, fromBSdate, toBSdate,
                allData, aggregates, widget
            );

            // --- THE FIX: HIDDEN IFRAME INSTEAD OF WINDOW.OPEN ---

            // 1. Clean up any existing print frames
            var frameId = "universal-grid-print-frame";
            $('#' + frameId).remove();

            // 2. Create the hidden iframe
            var $iframe = $('<iframe id="' + frameId + '" name="' + frameId + '" style="position:absolute;width:0px;height:0px;top:-1000px;left:-1000px;"></iframe>');
            $('body').append($iframe);

            var frameDoc = $iframe[0].contentWindow || $iframe[0].contentDocument;
            if (frameDoc.document) frameDoc = frameDoc.document;

            // 3. Write content to iframe
            frameDoc.write(printContent);
            frameDoc.close();

            // 4. Wait for styles to load, then print and remove
            setTimeout(function () {
                window.frames[frameId].focus();
                window.frames[frameId].print();

                // Cleanup: remove the iframe after the print dialog is handled
                setTimeout(function () { $iframe.remove(); }, 1000);
            }, 500);

        } catch (error) {
            console.error("UniversalGridPrint: Error during print:", error);
        }
    }

    function buildPrintContent(reportName, companyName, branchName, fromADdate, toADdate, fromBSdate, toBSdate, data, aggregates, grid) {
        var html = '<!DOCTYPE html><html><head><meta charset="utf-8"><title>Print Report</title>';
        html += '<style>';

        /* 1. Force Landscape and Tight Margins */
        html += '@page { size: landscape; margin: 0.5cm; }';

        /* 2. Base Body Styles */
        html += 'body { font-family: "Arial Narrow", Arial, sans-serif; margin: 0; padding: 0; width: 100%; }';

        /* 3. Container for Scaling */
        html += '.print-wrapper { width: 100%; }';

        /* 4. Header Styling - Keeping it readable even when scaled */
        html += '.report-header { text-align: left; margin-bottom: 20px; border-bottom: 1px solid #000; padding-bottom: 10px; }';
        html += '.report-header p { margin: 3px 0; font-size: 14px; }'; /* Larger font for header to compensate for scaling */

        /* 5. Table Compression */
        html += 'table { width: 100%; border-collapse: collapse; font-size: 9px; table-layout: auto; }';
        html += 'table, th, td { border: 0.5pt solid #000; }';
        html += 'th { background-color: #f0f0f0; padding: 4px 2px; text-align: center; font-weight: bold; }';
        html += 'td { padding: 3px 2px; word-wrap: break-word; }';

        html += '.text-right { text-align: right; }';
        html += '.text-center { text-align: center; }';
        html += '.footer-row { font-weight: bold; background-color: #f9f9f9; }';

        /* 6. FORCE SCALE ON PRINT - Adjust 'scale' value if 35 columns still feel tight */
        html += '@media print { ';
        html += '  .print-wrapper { ';
        html += '    transform: scale(0.70); ';
        html += '    transform-origin: top left; ';
        html += '    width: 142%; '; /* width should be (1 / scale * 100) */
        html += '  }';
        html += '  thead { display: table-header-group; }';
        html += '  tr { page-break-inside: avoid; }';
        html += '}';

        html += '</style></head><body>';

        // Open Scaling Wrapper
        html += '<div class="print-wrapper">';

        // Your Requested Header Format
        html += '<div class="report-header">';
        html += '<p><strong>Company Name :-</strong> ' + companyName + '</p>';
        html += '<p><strong>Branch Name :-</strong> ' + branchName + '</p>';
        html += '<p><strong>Report Name :-</strong> ' + reportName + '</p>';

        if (fromADdate && toADdate) {
            html += '<p><strong>From :-</strong> ' + fromADdate;
            if (fromBSdate) html += ' (' + fromBSdate + ')';
            html += ' <strong>To :-</strong> ' + toADdate;
            if (toBSdate) html += ' (' + toBSdate + ')';
            html += '</p>';
        }
        html += '</div>';

        // Table
        html += '<table>';
        html += buildTableHeaders(grid);
        html += '<tbody>';
        html += buildTableRows(data, grid);
        html += buildGrandTotalRow(aggregates, grid);
        html += '</tbody></table>';

        html += '</div>'; // Close print-wrapper
        html += '</body></html>';

        return html;
    }
    //function buildPrintContent(reportName, companyName, branchName, fromADdate, toADdate, fromBSdate, toBSdate, data, aggregates, grid) {
    //    var html = '<!DOCTYPE html><html><head><meta charset="utf-8"><title></title>';
    //    html += '<style>';
    //    html += 'body { font-family: Arial, sans-serif; margin: 20px; }';
    //    html += '.report-header { text-align: left; margin-bottom: 20px; }';
    //    html += '.report-header p { margin: 3px 0; font-size: 12px; font-weight: normal; }';
    //    html += 'table { width: 100%; border-collapse: collapse; font-size: 10px; margin-top: 10px; }';
    //    html += 'table, th, td { border: 1px solid #000; }';
    //    html += 'th { background-color: #f0f0f0; padding: 8px; text-align: center; font-weight: bold; }';
    //    html += 'td { padding: 6px; }';
    //    html += '.text-right { text-align: right; }';
    //    html += '.text-center { text-align: center; }';
    //    html += '.footer-row { font-weight: bold; background-color: #f9f9f9; }';
    //    html += '@page { margin: 0; }';
    //    html += '@media print {';
    //    html += '  body { margin: 10mm; }';
    //    html += '  table { page-break-inside: auto; }';
    //    html += '  tr { page-break-inside: avoid; page-break-after: auto; }';
    //    html += '  thead { display: table-header-group; }';
    //    html += '}';
    //    html += '</style></head><body>';

    //    // Header
    //    html += '<div class="report-header">';
    //    html += '<p><strong>Company Name :-</strong> ' + companyName + '</p>';
    //    html += '<p><strong>Branch Name :-</strong> ' + branchName + '</p>';
    //    html += '<p><strong>Report Name :-</strong> ' + reportName + '</p>';

    //    if (fromADdate && toADdate) {
    //        html += '<p><strong>From :-</strong> ' + fromADdate;
    //        if (fromBSdate) html += ' (' + fromBSdate + ')';
    //        html += ' <strong>To :-</strong> ' + toADdate;
    //        if (toBSdate) html += ' (' + toBSdate + ')';
    //        html += '</p>';
    //    }
    //    html += '</div>';

    //    // Table
    //    html += '<table>';
    //    html += buildTableHeaders(grid);
    //    html += '<tbody>';
    //    html += buildTableRows(data, grid);
    //    html += buildGrandTotalRow(aggregates, grid);
    //    html += '</tbody>';
    //    html += '</table>';
    //    html += '</body></html>';

    //    return html;
    //}
   

    function buildTableHeaders(grid) {
        var html = '<thead>';
        var columns = grid.columns;
        var hasMultiRowHeader = columns.some(function (c) { return c.columns && c.columns.length > 0; });

        if (hasMultiRowHeader) {
            html += '<tr>';
            for (var i = 0; i < columns.length; i++) {
                var col = columns[i];
                if (col.columns && col.columns.length > 0) {
                    html += '<th colspan="' + col.columns.length + '">' + (col.title || '') + '</th>';
                } else {
                    html += '<th rowspan="2">' + (col.title || '') + '</th>';
                }
            }
            html += '</tr><tr>';
            for (var i = 0; i < columns.length; i++) {
                var col = columns[i];
                if (col.columns && col.columns.length > 0) {
                    for (var j = 0; j < col.columns.length; j++) {
                        html += '<th>' + (col.columns[j].title || '') + '</th>';
                    }
                }
            }
            html += '</tr>';
        } else {
            html += '<tr>';
            for (var i = 0; i < columns.length; i++) {
                html += '<th>' + (columns[i].title || '') + '</th>';
            }
            html += '</tr>';
        }
        return html + '</thead>';
    }

    function buildTableRows(data, grid) {
        var html = '';
        var columns = getAllLeafColumns(grid.columns);
        for (var i = 0; i < data.length; i++) {
            var item = data[i];
            html += '<tr>';
            for (var j = 0; j < columns.length; j++) {
                var col = columns[j];
                var value = item[col.field] || '';
                var cssClass = '';
                if (col.attributes && col.attributes.style) {
                    var styleStr = typeof col.attributes.style === 'string' ? col.attributes.style : JSON.stringify(col.attributes.style).toLowerCase();
                    if (styleStr.indexOf('right') >= 0) cssClass = 'text-right';
                    else if (styleStr.indexOf('center') >= 0) cssClass = 'text-center';
                }
                var formattedValue = formatCellValue(value, col, item);
                var indentation = (j === 0 && typeof item.level === 'function') ? '&nbsp;'.repeat(item.level() * 4) : '';
                html += '<td class="' + cssClass + '">' + indentation + formattedValue + '</td>';
            }
            html += '</tr>';
        }
        return html;
    }

    function buildGrandTotalRow(aggregates, grid) {
        if (!aggregates || Object.keys(aggregates).length === 0) return '';
        var html = '<tr class="footer-row">';
        var columns = getAllLeafColumns(grid.columns);
        var grandTotalAdded = false;
        for (var i = 0; i < columns.length; i++) {
            var col = columns[i];
            if (!grandTotalAdded && col.footerTemplate) {
                html += '<td class="text-right"><strong>Grand Total</strong></td>';
                grandTotalAdded = true;
            } else if (aggregates[col.field] && aggregates[col.field].sum !== undefined) {
                html += '<td class="text-right"><strong>' + formatNumber(Math.abs(aggregates[col.field].sum)) + '</strong></td>';
            } else {
                html += '<td></td>';
            }
        }
        return html + '</tr>';
    }

    function getAllLeafColumns(columns) {
        var leafColumns = [];
        for (var i = 0; i < columns.length; i++) {
            if (columns[i].columns && columns[i].columns.length > 0) leafColumns = leafColumns.concat(getAllLeafColumns(columns[i].columns));
            else leafColumns.push(columns[i]);
        }
        return leafColumns;
    }

    function formatCellValue(value, col, item) {
        if (value === null || value === undefined || value === '') return '';
        if (col.format && col.format.indexOf('n') >= 0) return formatNumber(value);
        return value.toString();
    }

    function formatNumber(value) {
        var num = parseFloat(value);
        return isNaN(num) ? value : num.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function getReportName() { return $('#submenu').text() || document.title || 'Report'; }
    function getCompanyName() { return window.companyName || ''; }
    function getBranchName() { return window.branchName || ''; }

    return { init: init, printReport: printReport };

})();