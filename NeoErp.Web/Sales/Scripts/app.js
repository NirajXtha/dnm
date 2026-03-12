//Controls Declaration
var fFlexGrid = null,
    IncludeHeaderExport = null,
    IncludeHeaderImport = null;

//Controls Initialization
function InitialControls() {
    fFlexGrid = wijmo.Control.getControl("#fFlexGrid");
    IncludeHeaderImport = document.getElementById('IncludeHeaderImport');
    IncludeHeaderExport = document.getElementById('IncludeHeaderExport');
}

// export 
function exportExcel() {
    fFlexGrid = wijmo.Control.getControl("#fFlexGrid");
    IncludeHeaderImport = document.getElementById('IncludeHeaderImport');
    IncludeHeaderExport = document.getElementById('IncludeHeaderExport');
    var result = wijmo.grid.ExcelConverter.export(fFlexGrid, { includeColumnHeader: IncludeHeaderExport.checked});

    if (navigator.msSaveBlob) {
        // Saving the xlsx file using Blob and msSaveBlob in IE.
        var blob = new Blob([result.base64Array]);

        navigator.msSaveBlob(blob, $('#export').attr("download"));
    } else {
        $('#export')[0].href = result.href();
    }
};

//Group By Modules
function gMenu_SelectedIndexChanged(sender) {
    var grid = wijmo.Control.getControl("#fFlexGrid");
    if (sender.selectedValue && grid) {
        var name = sender.selectedValue;
        var groupDescriptions = grid.collectionView.groupDescriptions;
        grid.beginUpdate();
        groupDescriptions.clear();

        if (name.indexOf("Item_Code") > -1) {
            groupDescriptions.push(new wijmo.collections.PropertyGroupDescription("Item_Code"));
        }

        if (name.indexOf("Mu_Code") > -1) {
            groupDescriptions.push(new wijmo.collections.PropertyGroupDescription("Mu_Code"));
        }

        if (name.indexOf("Quantity") > -1) {
            groupDescriptions.push(new wijmo.collections.PropertyGroupDescription("Quantity"));
        }
        if (name.indexOf("Unit_Price") > -1) {
            groupDescriptions.push(new wijmo.collections.PropertyGroupDescription("Unit_Price"));
        }
        if (name.indexOf("Total_Price") > -1) {
            groupDescriptions.push(new wijmo.collections.PropertyGroupDescription("Total_Price"));
        }
        if (name.indexOf("Customer_Code") > -1) {
            groupDescriptions.push(new wijmo.collections.PropertyGroupDescription("Customer_Code"));
        }
        if (name.indexOf("Branch_Code") > -1) {
            groupDescriptions.push(new wijmo.collections.PropertyGroupDescription("Branch_Code"));
        }
    
        grid.endUpdate();
    }
}
