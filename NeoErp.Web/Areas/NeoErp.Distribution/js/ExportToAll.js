function exportToAll(e)
{
    switch (e.ExportClass)
    {
        case "export-pdf": var a = $("#" + e.PageId).getKendoChart(); a.exportPDF({ paperSize: "auto", margin: { left: "1cm", top: "1cm", right: "1cm", bottom: "1cm" } }).done(function (a) { kendo.saveAs({ dataURI: a, fileName: e.headerName + ".pdf", proxyURL: e.urlInfo }) }); break;
        case "export-img": var a = $("#" + e.PageId).getKendoChart(); a.exportImage().done(function (a) { kendo.saveAs({ dataURI: a, fileName: e.headerName + ".png", proxyURL: e.urlInfo }) }); break;
        case "export-svg": var a = $("#" + e.PageId).getKendoChart(); a.exportSVG().done(function (a) { kendo.saveAs({ dataURI: a, fileName: e.headerName + ".svg", proxyURL: e.urlInfo }) }); break; default: alert("Setup Pending")
    }
}