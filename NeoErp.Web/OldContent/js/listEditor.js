
function AddAutoComplete(id, object)
{
    var url = '/WebServices/ItemAutoComplete';
    //alert(url);
    $('#searchItem_'+id).autocomplete({
        source: function (request, response) {
            $.ajax({
                url: url,
                dataType: 'json',
                type: 'POST',
                data: { term: request.term },
                success: function (data) {
                    response($.map(data,
                    function (item) {
                        return {
                            label: item.ItemName,
                            value: item.ItemName,
                            selectedValue: item.ItemID,
                            ItemRate: item.PurchaseRate,
                            ItemUnit: item.Unit
                        }
                    }));
                }
            })
        },
        select:
            function (event, ui) {
                $('#searchItem_'+id).val(ui.item.label);
                $('#'+object+'_'+id+'__'+'ItemID').val(ui.item.selectedValue);
                $('#'+object+'_'+id+'__'+'ItemRate').val(ui.item.ItemRate);
                $('#'+object+'_'+id+'__'+'ItemUnit').val(ui.item.ItemUnit);
                $('#'+object+'_'+id+'__'+'Unit').val(ui.item.ItemUnit);
                return false;
            },
        minLength: 1
    });
}

function BindCalcFields(id,object)
{
    // ItemRate * Qty for Inventory   
    $('#'+object+'_'+id+'__'+'ItemRate'+','+'#'+object+'_'+id+'__'+'Qty').on("change", function () {
        var Total = $('#'+object+'_'+id+'__'+'ItemRate').val() * $('#'+object+'_'+id+'__'+'Qty').val();
        $('#' + object + '_' + id + '__' + 'TotalAmt').val(Total);
           // alert("Welocme to the dialog box");
        });    
}

var addRow = function () {
    addTableRow($("#item-table"));
    return false;
};

var deleteRow = function (event) {    
    $(event.target).closest("tr").remove();
    return false;
};


$(function () {
    $(".new-row").click(addRow);
    $(".remove-row").click(deleteRow);
})

function addTableRow(table) {
    /* Sources:
    http://www.simonbingham.me.uk/index.cfm/main/post/uuid/adding-a-row-to-a-table-containing-form-fields-using-jquery-18
    http://stackoverflow.com/questions/5104288/adding-validation-with-mvc-3-jquery-validator-in-execution-time
    */

    var $ttc = $(table).find("tbody tr:last");
    var $tr = $ttc.clone();

    $tr.find("input,select").attr("name", function () { // find name in the cloned row
        var parts = this.id.match(/(\D+)_(\d+)__(\D+)$/); // extract parts from id, including index
        return parts[1] + "[" + ++parts[2] + "]." + parts[3]; // build new name
    }).attr("id", function () { // change id also
        var parts = this.id.match(/(\D+)_(\d+)__(\D+)$/); // extract parts
        return parts[1] + "_" + ++parts[2] + "__" + parts[3]; // build new id
    });
    $tr.find("span[data-valmsg-for]").attr("data-valmsg-for", function () { // find validation message
        var parts = $(this).attr("data-valmsg-for").match(/(\D+)\[(\d+)]\.(\D+)$/); // extract parts from the referring attribute
        return parts[1] + "[" + ++parts[2] + "]." + parts[3]; // build new value
    })

    $ttc.find(".new-row").attr("class", "remove-row").attr("title", "Delete row").unbind("click").click(deleteRow); // change buttin function
    $tr.find(".new-row").click(addRow); // add function to the cloned button

    // reset fields in the new row
    $tr.find("select").val("");
    $tr.find("input[type=text]").val("");

    // add cloned row as last row
    $(table).find("tbody tr:last").after($tr);

    // Find the affected form
    var $form = $tr.closest("FORM");

    // Unbind existing validation
    $form.unbind();
    $form.data("validator", null);

    // Check document for changes
    $.validator.unobtrusive.parse(document);

    // We could re-validate with changes
     $form.validate($form.data("unobtrusiveValidation").options);
};


//$("#addItem").click(function() {
//    $.ajax({
//        url: this.href,
//        cache: false,
//        success: function (html) {
//            var table = $("#item-table");
//            // add returned row as last row
//            $(table).find("tbody tr:last").after(html);
//            //$("#editorRows").append(html);
//        }
//    });
//    return false;
//});

//$(document).on('click', '.deleteRow', function () {    
//    $(this).closest("tr").remove();
//    return false;
//});


//AddAutoComplete("@ID", "IssueItems");
//BindCalcFields("@ID", "IssueItems");

