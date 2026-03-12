

$(document).ready(function () {

    //$('#filter-categories').hideseek({
    //    nodata: 'No Match found'
    //});

    // Code is replace in anjular js
    //$(document).on('click', '#update-note', function () {
    //    var n = $('#snote').val();
    
    //    $('#note').val(n);
    //    $('#noteModal').modal('hide');
    //});
    // Code is end replace in anjular js
    /* =============================
    Edit Item Modal
    ============================= */
    // Code is replace in anjular js
   // $("#posTable").on("click", '.edit', function () {
        //var row = $(this).closest('tr');
        //var id = row.attr('id');
        //var item_id = row.attr('data-item-id');
        //var item = spositems[item_id];
        //var unit_price = formatDecimal(row.find('.realuprice').val());
        //var net_price = unit_price;
        //var ds = item.row.discount ? item.row.discount : '0';
        //item_discount = formatDecimal(parseFloat(ds));
        //if (ds.indexOf("%") !== -1) {
        //    var pds = ds.split("%");
        //    if (!isNaN(pds[0])) {
        //        item_discount = formatDecimal((net_price * parseFloat(pds[0])) / 100);
        //    }
        //}
        //net_price -= item_discount;
        //var pr_tax = parseFloat(item.row.tax), pr_tax_val = 0, tax = '';
        //if (pr_tax !== null && pr_tax != 0) {
        //    if (parseFloat(item.row.tax_method) == 0) {
        //        pr_tax_val = formatDecimal((net_price * parseFloat(pr_tax)) / (100 + parseFloat(pr_tax)));
        //        net_price -= pr_tax_val;
        //        tax = lang.inclusive;
        //    } else {
        //        pr_tax_val = formatDecimal((net_price * parseFloat(pr_tax)) / 100);
        //        tax = lang.exclusive;
        //    }
        //}
        //$('#proModalLabel').html('<a href="' + base_url + '/products/view/' + item_id + '" data-toggle="ajax">' + item.label + '</a>');
        //$('#net_price').text(formatMoney(net_price));
        //$('#pro_tax').text(formatMoney(pr_tax_val));
        //$('#pro_tax_method').text('(' + tax + ')');
        //$('#row_id').val(row_id);
        //$('#item_id').val(item_id);
        //$('#nPrice').val(unit_price);
        //$('#nQuantity').val(item.row.qty);
        //$('#nDiscount').val(ds);
     //   $('#proModal').modal({ backdrop: 'static' });
 //   });
    // Code is end replace in anjular js
    $(document).on('change', '#nPrice, #nDiscount', function () {
        //var item_id = $('#item_id').val();
        //var unit_price = parseFloat($('#nPrice').val());
        //var net_price = unit_price;
        //var item = spositems[item_id];
        //var ds = $('#nDiscount').val() ? $('#nDiscount').val() : '0';
        //item_discount = formatDecimal(parseFloat(ds));
        //if (ds.indexOf("%") !== -1) {
        //    var pds = ds.split("%");
        //    if (!isNaN(pds[0])) {
        //        item_discount = formatDecimal((unit_price * parseFloat(pds[0])) / 100);
        //    }
        //}
        //net_price -= item_discount;
        //var pr_tax = parseFloat(item.row.tax), pr_tax_val = 0;
        //if (pr_tax !== null && pr_tax != 0) {
        //    if (parseFloat(item.row.tax_method) == 0) {
        //        pr_tax_val = formatDecimal((net_price * parseFloat(pr_tax)) / (100 + parseFloat(pr_tax)));
        //        net_price -= pr_tax_val;
        //        tax = lang.inclusive;
        //    } else {
        //        pr_tax_val = formatDecimal((net_price * parseFloat(pr_tax)) / 100);
        //        tax = lang.exclusive;
        //    }
        //}

        //$('#net_price').text(formatMoney(net_price));
        //$('#pro_tax').text(formatMoney(pr_tax_val));
    });

    /* =============================
    Edit Item Method
    ============================= */

    // Code is replace in anjular js
  //  $(document).on('click', '#editItem', function () {
        //var item_id = $('#item_id').val();
        //var price = parseFloat($('#nPrice').val());
        //if (!is_valid_discount($('#nDiscount').val())) {
        //    bootbox.alert(lang.unexpected_value);
        //    return false;
        //}
        //spositems[item_id].row.qty = parseFloat($('#nQuantity').val()),
        //spositems[item_id].row.real_unit_price = price,
        //spositems[item_id].row.discount = $('#nDiscount').val() ? $('#nDiscount').val() : '0',
        //localStorage.setItem('spositems', JSON.stringify(spositems));
      //  $('#proModal').modal('hide');

        //loadItems();
      //  return;
  //  });
    // Code is end replace in anjular js
    /* =============================
    Row quantity change
    ============================= */
 

    //$('#reset').click(function (e) {
    //    //if (count <= 1) {
    //    //    return false;
    //    //}
    //    bootbox.confirm(lang.r_u_sure, function (result) {
    //        //if (result) {
    //        //    if (get('spositems')) {
    //        //        remove('spositems');
    //        //    }
    //        //    if (get('spos_tax')) {
    //        //        remove('spos_tax');
    //        //    }
    //        //    if (get('spos_discount')) {
    //        //        remove('spos_discount');
    //        //    }
    //        //    if (get('spos_customer')) {
    //        //        remove('spos_customer');
    //        //    }

    //        //    window.location.href = base_url + "pos";
    //       //}
    //    });
    //});

    if (Settings.java_applet == 1) {
        //$(document).ready(function () {
        //    $('#print_order').click(function () {
        //        if (count <= 1) {
        //            bootbox.alert(lang.please_add_product);
        //            return false;
        //        } else {
        //            if (spos_note = get('spos_note')) {
        //                order_data += "\r\n" + spos_note;
        //            }
        //            printOrder(order_data);
        //        }
        //    });
        //    $('#print_bill').click(function () {
        //        if (count <= 1) {
        //            bootbox.alert(lang.please_add_product);
        //            return false;
        //        } else {
        //            if (spos_note = get('spos_note')) {
        //                bill_data += "\r\n" + spos_note;
        //            }
        //            printBill(bill_data);
        //        }
        //    });
        //});
    } else {
        $(document).ready(function () {
            //$('#print_order').click(function () {
            //    if (count <= 1) {
            //        bootbox.alert(lang.please_add_product);
            //        return false;
            //    } else {
            //        var odata = $('#order_tbl').html();
            //        if (spos_note = get('spos_note')) {
            //            odata += '<div style="text-align:center;margin-top:10px;">' + spos_note + '</div>';
            //        }
            //        Popup(odata);
            //    }
            //});
            //$('#print_bill').click(function () {
            //    if (count <= 1) {
            //        bootbox.alert(lang.please_add_product);
            //        return false;
            //    } else {
            //        var bdata = $('#bill_tbl').html();
            //        if (spos_note = get('spos_note')) {
            //            bdata += '<div style="text-align:center;margin-top:10px;">' + spos_note + '</div>';
            //        }
            //        Popup(bdata);
            //    }
            //});
        });
    }
    // Code is  replace in anjular js
    $("#updateDiscount").click(function () {
        //var ds = $('#get_ds').val() ? $('#get_ds').val() : '0';
        //var apply_to = $('input[name=apply_to]:checked').val();
        //if (ds.length != 0) {
        //    if (apply_to == 'order') {
        //        $('#discount_val').val(ds);
        //        store('spos_discount', ds);
        //        if (ds.indexOf("%") !== -1) {
        //            var pds = ds.split("%");

        //            order_discount = (total * parseFloat(pds[0])) / 100;
        //            order_tax = calTax();
        //            var g_total = total + order_tax - order_discount;
        //            grand_total = parseFloat(g_total);
        //            $("#ds_con").text('(' + formatMoney(product_discount) + ') ' + formatMoney(order_discount));
        //            $("#total-payable").text(formatMoney(grand_total));

        //        } else {

        //            order_discount = ds;
        //            order_tax = calTax();
        //            var g_total = (total + order_tax) - parseFloat(order_discount);
        //            grand_total = parseFloat(g_total);
        //            $("#ds_con").text('(' + formatMoney(product_discount) + ') ' + formatMoney(order_discount));
        //            $("#total-payable").text(formatMoney(grand_total));
        //        }
        //    } else if (apply_to == 'products') {
        //        var items = {};
        //        spositems = JSON.parse(get('spositems'));
        //        $.each(spositems, function () {
        //            this.row.discount = ds;
        //        });
        //        store('spositems', JSON.stringify(spositems));
        //        loadItems();
        //    }
         // $('#dsModal').modal('hide');
        //}
         
    });
    // Code is end replace in anjular js
    // Code is  replace in anjular js
    //$("#add_discount").click(function () {
     
    //    $('#dsModal').modal({ backdrop: 'static' });
    //    return false;
    //});
    // Code is end replace in anjular js

    // Code is  replace in anjular js
  //  $("#updateTax").click(function () {
        //var ts = $('#get_ts').val();
        //if (ts.length != 0) {
        //    $('#tax_val').val(ts);
        //    store('spos_tax', ts);
        //    if (ts.indexOf("%") !== -1) {
        //        var pts = ts.split("%");
        //        if (!isNaN(pts[0])) {
        //            order_tax = ((total - order_discount) * parseFloat(pts[0])) / 100;
        //            var g_total = (total + order_tax) - order_discount;
        //            grand_total = parseFloat(g_total);
        //            $("#ts_con").text(formatMoney(order_tax));
        //            $("#total-payable").text(formatMoney(grand_total));
        //        } else {
        //            $('#get_ts').val('0'); $('#tax_val').val('0');
        //            var g_total = total - order_discount;
        //            grand_total = parseFloat(g_total);
        //            $("#ts_con").text('0');
        //            $("#total-payable").text(formatMoney(grand_total));
        //        }
        //    } else {
        //        if (!isNaN(ts) && ts != 0) {
        //            order_tax = ts;
        //            var g_total = (total + parseFloat(ts)) - order_discount;
        //            grand_total = parseFloat(g_total);
        //            $("#ts_con").text(formatMoney(order_tax));
        //            $("#total-payable").text(formatMoney(grand_total));
        //        } else {
        //            $('#get_ts').val('0'); $('#tax_val').val('0');
        //            var g_total = total - order_discount;
        //            grand_total = parseFloat(g_total);
        //            $("#ts_con").text('0');
        //            $("#total-payable").text(formatMoney(grand_total));
        //        }
        //    }
        //    $('#tsModal').modal('hide');
        //}
       
   // });
    // Code is end replace in anjular js

    // Code is  replace in anjular js
    //$("#add_tax").click(function () {
    //    var tval = $('#tax_val').val();
    //    $('#get_ts').val(tval);
    //    $('#tsModal').modal({ backdrop: 'static' });
    //    return false;
    //});

    // Code is end replace in anjular js
 

  //  $(document).on('click', '.category', function () {
        //var cid = $(this).attr('id');
        //if (cat_id != cid) {
        //    cat_id = cid;
        //    $.ajax({
        //        type: "get",
        //        url: base_url + 'pos/ajaxproducts',
        //        data: { category_id: cat_id, tcp: 1 },
        //        dataType: "json",
        //        success: function (data) {
        //            p_page = 'n';
        //            $('[data-toggle="control-sidebar"]').click();
        //            //ocat_id = cat_id;
        //            tcp = data.tcp;
        //            $('.items').html(data.products);
        //            $('.category').removeClass('active');
        //            $('#' + cat_id).addClass('active');
        //            nav_pointer();
        //        }
        //    });
        //}
        return false;
  //  });
  

   // $('#next').click(function () {
        //if (p_page == 'n') { p_page = 0; }
        //p_page += pro_limit;
        //if (tcp >= pro_limit && p_page < tcp) {
        //    $.ajax({
        //        type: "get",
        //        url: base_url + 'pos/ajaxproducts',
        //        data: { category_id: cat_id, per_page: p_page },
        //        dataType: "html",
        //        success: function (data) {
        //            $('.items').html(data);
        //            nav_pointer();
        //        }
        //    });
        //} else {
        //    p_page -= pro_limit;
        //}
  //  });

    //$('#previous').click(function () {
        //if (p_page == 'n') { p_page = 0; }
        //if (p_page != 0) {
        //    p_page -= pro_limit;
        //    if (p_page == 0) { p_page = 'n'; }
        //    $.ajax({
        //        type: "get",
        //        url: base_url + 'pos/ajaxproducts',
        //        data: { category_id: cat_id, per_page: p_page },
        //        dataType: "html",
        //        success: function (data) {
        //            $('.items').html(data);
        //            nav_pointer();
        //        }
        //    });
        //}
   // });

   // $("#add_item").autocomplete({
        //source: base_url + 'pos/suggestions',
        //minLength: 1,
        //autoFocus: false,
        //delay: 200,
        //response: function (event, ui) {
        //    if ($(this).val().length >= 16 && ui.content[0].id == 0) {
        //        bootbox.alert(lang.no_match_found, function () {
        //            $('#add_item').focus();
        //        });
        //        $(this).val('');
        //    }
        //    else if (ui.content.length == 1 && ui.content[0].id != 0) {
        //        ui.item = ui.content[0];
        //        $(this).data('ui-autocomplete')._trigger('select', 'autocompleteselect', ui);
        //        $(this).autocomplete('close');
        //    }
        //    else if (ui.content.length == 1 && ui.content[0].id == 0) {
        //        bootbox.alert(lang.no_match_found, function () {
        //            $('#add_item').focus();
        //        });
        //        $(this).val('');
        //    }
        //},
        //select: function (event, ui) {
        //    event.preventDefault();
        //    if (ui.item.id !== 0) {
        //        var row = add_invoice_item(ui.item);
        //        if (row)
        //            $(this).val('');
        //    } else {
        //        bootbox.alert(lang.no_match_found);
        //    }
        //}
  //  });

    //$('#add_item').bind('keypress', function (e) {
    //    if (e.keyCode == 13) {
    //        e.preventDefault();
    //      //  $(this).autocomplete("search");
    //    }
    //});

    $('#add_item').focus();
    $('#gccard_no').inputmask("9999 9999 9999 9999");
    $('#gift_card_no').inputmask("9999 9999 9999 9999");
    $('#genNo').click(function () {
        var no = generateCardNo();
        $(this).parent().parent('.input-group').children('input').val(no);
        return false;
    });

    //$(document).on('click', '#sellGiftCard', function (e) {
    //    if (count == 1) {
    //        spositems = {};
    //    }
    //    $('#gcModal').modal({ backdrop: 'static' });
    //});

   // $(document).on('click', '#addGiftCard', function (e) {
        //var mid = (new Date).getTime(),
        //gccode = $('#gccard_no').val(),
        //gcname = $('#gcname').val(),
        //gcvalue = $('#gcvalue').val(),
        //gcprice = parseFloat($('#gcprice').val());
        //if (gccode == '' || gcvalue == '' || gcprice == '' || gcvalue == 0 || gcprice == 0) {
        //    $('#gcerror').text(lang.file_required_fields);
        //    $('.gcerror-con').show();
        //    return false;
        //}
        //var gc_data = new Array();
        //gc_data[0] = gccode;
        //gc_data[1] = gcvalue;

        //$.ajax({
        //    type: 'get',
        //    url: base_url + 'gift_cards/sell_gift_card',
        //    dataType: "json",
        //    data: { gcdata: gc_data },
        //    success: function (data) {
        //        if (data.result === 'success') {
        //            spositems[mid] = { "id": mid, "item_id": mid, "label": gcname + ' (' + gccode + ')', "row": { "id": mid, "code": gccode, "name": gcname, "quantity": 1, "price": gcprice, "real_unit_price": gcprice, "tax": 0, "qty": 1, "type": "manual", "discount": "0" } };
        //            store('spositems', JSON.stringify(spositems));
        //            loadItems();
        //            $('#gcModal').modal('hide');
        //            $('#gccard_no').val('');
        //            $('#gcvalue').val('');
        //            $('#gcprice').val('');
        //        } else {
        //            $('#gcerror').text(data.message);
        //            $('.gcerror-con').show();
        //        }
        //    }
        //});
  //  });



    var pwacc = false;
   

   // $('#suspend').click(function () {
     //   $('#susModal').modal({ backdrop: 'static' });
        //if (count <= 1) {
        //    bootbox.alert(lang.please_add_product);
        //    return false;
        //} else {
        //    $('#susModal').modal({ backdrop: 'static' });
        //}
    //});

   // $('#suspend_sale').click(function () {
        //ref = $('#reference_note').val();
        //if (!ref || ref == '') {
        //    bootbox.alert(lang.type_reference_note);
        //    return false;
        //} else {
        //    suspend = $('<span></span>');
        //    if (sid !== 0) {
        //        suspend.html('<input type="hidden" name="delete_id" value="' + sid + '" /><input type="hidden" name="suspend" value="yes" /><input type="hidden" name="suspend_note" value="' + ref + '" />');
        //    } else {
        //        suspend.html('<input type="hidden" name="suspend" value="yes" /><input type="hidden" name="suspend_note" value="' + ref + '" />');
        //    }
        //    suspend.appendTo("#hidesuspend");
        //    $('#pos-sale-form').submit();
        //}
    //});

  //  $('#payment').click(function () {
        //if (count <= 1) {
        //    bootbox.alert(lang.please_add_product);
        //    return false;
        //} else {

        //    if (sid) {
        //        suspend = $('<span></span>');
        //        suspend.html('<input type="hidden" name="delete_id" value="' + sid + '" />');
        //        suspend.appendTo("#hidesuspend");
        //    }

        //    gtotal = formatDecimal(total - order_discount + order_tax);
        //    if (Settings.rounding != 0) {
        //        round_total = roundNumber(gtotal, parseInt(Settings.rounding));
        //        var rounding = formatDecimal(round_total - gtotal);
        //        $('#twt').text(formatMoney(round_total) + ' (' + formatMoney(rounding) + ')');
        //        $('#quick-payable').text(round_total);
        //    } else {
        //        $('#twt').text(formatMoney(gtotal));
        //        $('#quick-payable').text(gtotal);
        //    }
        //    $('#item_count').text((an - 1) + ' (' + (count - 1) + ')');
        //    $("#order_quantity").val(count - 1);
        //    $("#order_items").val(an - 1);
        //    $('#balance').text('0.00');
        //    $('#payModal').modal({ backdrop: 'static' });

        //}
        //$('#payModal').modal({ backdrop: 'static' });
   // });
    $('#payModal').on('shown.bs.modal', function (e) {
        $('#amount').focus();
    });
    $('#payModal').on('hidden.bs.modal', function (e) {
        $('#amount').val('').change();
    });

   // $('#amount').change(function (e) {
        //var total_paying = $(".amount").val();
        //$('#total_paying').text(formatMoney(total_paying));
        //if (Settings.rounding != 0) {
        //    $('#balance').text(formatMoney(total_paying - round_total));
        //    $('#balance_val').val(formatDecimal(total_paying - round_total));
        //    total_paid = total_paying;
        //    grand_total = round_total;
        //} else {
        //    $('#balance').text(formatMoney(total_paying - gtotal));
        //    $('#balance_val').val(formatDecimal(total_paying - gtotal));
        //    total_paid = total_paying;
        //    grand_total = gtotal;
        //}
   // });

    //$('#add-customer').click(function () {
    //    $('#customerModal').modal({ backdrop: 'static' });
    //});

    $('#payModal').on('change', '#paid_by', function () {
        var p_val = $(this).val();
        $('#paid_by_val').val(p_val);
        var gtotal = formatDecimal(total - order_discount + order_tax);
        if (Settings.rounding != 0) {
            var rounded_total = formatDecimal(roundNumber(gtotal, parseInt(Settings.rounding)));
        } else {
            var rounded_total = formatDecimal(gtotal);
        }
        $('#rpaidby').val(p_val);
        if (p_val == 'gift_card') {
            $('.gc').slideDown();
            $('.ngc').slideUp('fast');
            setTimeout(function () { $('#gift_card_no').focus(); }, 10);
            $('#amount').attr('readonly', true);
        } else {
            $('.ngc').slideDown();
            $('.gc').slideUp('fast');
            $('#gc_details').html('');
            $('#amount').attr('readonly', false);
        }
        if (p_val == 'cash' || p_val == 'other') {
            $('.pcash').slideDown();
            $('.pcheque').slideUp('fast');
            $('.pcc').slideUp('fast');
            setTimeout(function () { $('#amount').focus(); }, 10);
        } else if (p_val == 'CC' || p_val == 'stripe') {
            $('.pcc').slideDown();
            $('.pcheque').slideUp('fast');
            $('.pcash').slideUp('fast');
            $('#amount').val(rounded_total);
            setTimeout(function () { $('#swipe').val('').focus(); }, 10);
        } else if (p_val == 'Cheque') {
            $('.pcheque').slideDown();
            $('.pcc').slideUp('fast');
            $('.pcash').slideUp('fast');
            $('#amount').val(rounded_total);
            setTimeout(function () { $('#cheque_no').focus(); }, 10);
        } else {
            $('.pcheque').hide();
            $('.pcc').hide();
            $('.pcash').hide();
        }
    });

    $(document).on('change', '.gift_card_no', function () {
        //var cn = $(this).val() ? $(this).val() : '';
        //if (cn != '') {
        //    $.ajax({
        //        type: "get", async: false,
        //        url: base_url + "pos/validate_gift_card/" + cn,
        //        dataType: "json",
        //        success: function (data) {
        //            if (data === false || data.balance < 0) {
        //                $('#gift_card_no').parent('.form-group').addClass('has-error');
        //                bootbox.alert(lang.incorrect_gift_card);
        //            } else {
        //                $('#gc_details').html(lang.card_no + ': ' + data.card_no + '<br>' + lang.value + ': ' + data.value + ' - ' + lang.balance + ': ' + data.balance);
        //                $('#gift_card_no').parent('.form-group').removeClass('has-error');
        //                var paying = (gtotal > data.balance) ? data.balance : gtotal;
        //                $('#amount_val').val(paying);
        //                $('#amount').val(paying);
        //            }
        //        }
        //    });
        //}
        return false;
    });

    $(document).on('click', '.quick-cash', function () {
        var $quick_cash = $(this);
        var amt = $quick_cash.contents().filter(function () {
            return this.nodeType == 3;
        }).text();
        var th = Settings.thousands_sep == 0 ? '' : Settings.thousands_sep;
        var $pi = $('#amount');
        amt = formatDecimal(amt.split(th).join("")) * 1 + $pi.val() * 1;
        $pi.val(formatDecimal(amt)).change().focus();
        var note_count = $quick_cash.find('span');
        if (note_count.length == 0) {
            $quick_cash.append('<span class="badge">1</span>');
        } else {
            note_count.text(parseInt(note_count.text()) + 1);
        }
    });

    $(document).on('click', '#clear-cash-notes', function () {
        $('.quick-cash').find('.badge').remove();
        $('#amount').val('').change().focus();
    });

    $('#payModal').on('change', '#amount', function (e) {
        $('#amount_val').val($(this).val());
    });
    $('#payModal').on('blur', '#amount', function (e) {
        $('#amount_val').val($(this).val());
    });
    $('#payModal').on('select2-close', '#paid_by', function (e) {
        $('#paid_by_val').val($(this).val());
    });
    $('#payModal').on('change', '#pcc_no', function (e) {
        $('#cc_no_val').val($(this).val());
    });
    $('#payModal').on('change', '#pcc_holder', function (e) {
        $('#cc_holder_val').val($(this).val());
    });
    $('#payModal').on('change', '#gift_card_no', function (e) {
        $('#paying_gift_card_no_val').val($(this).val());
    });
    $('#payModal').on('change', '#pcc_month', function (e) {
        $('#cc_month_val').val($(this).val());
    });
    $('#payModal').on('change', '#pcc_year', function (e) {
        $('#cc_year_val').val($(this).val());
    });
    $('#payModal').on('change', '#pcc_type', function (e) {
        $('#cc_type_val').val($(this).val());
    });
    $('#payModal').on('change', '#pcc_cvv2', function (e) {
        $('#cc_cvv2_val').val($(this).val());
    });
    $('#payModal').on('change', '#cheque_no', function (e) {
        $('#cheque_no_val').val($(this).val());
    });
    $('#payModal').on('change', '#payment_note', function (e) {
        $('#payment_note_val').val($(this).val());
    });
    $('#payModal').on('change', '#note', function (e) {
        var n = $(this).val();
        $('#spos_note').val(n);
    });
  
    $('#spos_customer').change(function (e) {
        //store('spos_customer', $(this).val());
    });
  

    $(".modal").each(function (index) {
        $(this).on('show.bs.modal', function (e) {
            var open = $(this).attr('data-easein');
            $('.modal-dialog').velocity('transition.' + open);
        });
    });

    $('[data-toggle="ajax"]').click(function (event) {
        //event.preventDefault();
        //var href = $(this).attr('href');
        //$.get(href, function (data) {
        //    $("#posModal").html(data);
        //    $("#posModal").modal({ backdrop: 'static' });
        //    return false;
        //});
    });

    $('#submit-sale').click(function () {
        //$('#total_items').val(an - 1);
        //$('#total_quantity').val(count - 1);
        //console.log($('#amount_val').val());
        //$('#submit').click();
    });

    $('#suspend_sale').click(function () {
        //if ($('#reference_note').val()) {
        //    $('#hold_ref').val($('#reference_note').val());
        //    $('#total_items').val(an - 1);
        //    $('#total_quantity').val(count - 1);
        //    $('#submit').click();
        //}
        return false;
    });

    $("#customer-form").on("submit", function (e) {
        //e.preventDefault();
        //$.ajax({
        //    type: "post",
        //    url: base_url + 'customers/add',
        //    data: $(this).serialize(),
        //    dataType: "json",
        //    success: function (res) {
        //        if (res.status == 'success') {
        //            $('#spos_customer').append($("<option></option>").attr("value", res.id).text(res.val));
        //            $('#spos_customer').select2('val', res.id);
        //            $('#customerModal').modal('hide');
        //        } else {
        //            $('#c-alert').html(res.msg);
        //            $('#c-alert').show();
        //        }
        //    },
        //    error: function () {
        //        bootbox.alert(lang.customer_request_failed);
        //        return false;
        //    }
        //});
        return false;
    });

    $('#customerModal').on('hidden.bs.modal', function (e) {
        $('#c-alert').hide();
        $('#cname').val('');
        $('#cemail').val('');
        $('#cphone').val('');
        $('#cf1').val('');
        $('#cf2').val('');
    });

});


function display_keyboards() {

    $('.kb-text').keyboard({
        autoAccept: true,
        alwaysOpen: false,
        openOn: 'focus',
        usePreview: false,
        // layout: 'qwerty',
        layout: 'custom',
        display: {
            'bksp': "\u2190",
            'accept': 'return',
            'default': 'ABC',
            'meta1': '123',
            'meta2': '#+='
        },
        customLayout: {
            'default': [
                'q w e r t y u i o p {bksp}',
                'a s d f g h j k l {enter}',
                '{s} z x c v b n m , . {s}',
                '{meta1} {space} {cancel} {accept}'
            ],
            'shift': [
                'Q W E R T Y U I O P {bksp}',
                'A S D F G H J K L {enter}',
                '{s} Z X C V B N M / ? {s}',
                '{meta1} {space} {meta1} {accept}'
            ],
            'meta1': [
                '1 2 3 4 5 6 7 8 9 0 {bksp}',
                '- / : ; ( ) \u20ac & @ {enter}',
                '{meta2} . , ? ! \' " {meta2}',
                '{default} {space} {default} {accept}'
            ],
            'meta2': [
                '[ ] { } # % ^ * + = {bksp}',
                '_ \\ | &lt; &gt; $ \u00a3 \u00a5 {enter}',
                '{meta1} ~ . , ? ! \' " {meta1}',
                '{default} {space} {default} {accept}'
            ]
        }
    });

    $('.kb-pad').keyboard({
        restrictInput: true,
        preventPaste: true,
        autoAccept: true,
        alwaysOpen: false,
        openOn: 'click',
        usePreview: false,
        layout: 'costom',
        display: {
            'b': '\u2190:Backspace'
        },
        customLayout: {
            'default': [
                '1 2 3 {b}',
                '4 5 6 . {clear}',
                '7 8 9 0 %',
                '{accept} {cancel}'
            ]
        }
    });

}

function calTax() {
    //var ts = get('spos_tax') ? get('spos_tax') : $('#tax_val').val();
    //if (ts.indexOf("%") !== -1) {
    //    var pts = ts.split("%");
    //    order_tax = ((total - order_discount) * parseFloat(pts[0])) / 100;
    //    $("#ts_con").text(formatMoney(order_tax));
    //} else {
    //    order_tax = parseFloat(ts);
    //    $("#ts_con").text(formatMoney(order_tax));
    //}
    return order_tax;
}

function nav_pointer() {
    //var pp = p_page == 'n' ? 0 : p_page;
    //(pp == 0) ? $('#previous').attr('disabled', true) : $('#previous').attr('disabled', false);
    //((pp + pro_limit) > tcp) ? $('#next').attr('disabled', true) : $('#next').attr('disabled', false);
}

function Popup(data) {
    var mywindow = window.open('', 'spos_print', 'height=500,width=300');
    mywindow.document.write('<!DOCTYPE html><html><head><title>Print</title>');
    mywindow.document.write('<link rel="stylesheet" href="' + assets + 'bootstrap/css/bootstrap.min.css" type="text/css" />');
    mywindow.document.write('<style>a {color: #333;} #totaltbl td, #totaltbl th { vertical-align: middle; }</style>');
    mywindow.document.write('</head><body >');
    mywindow.document.write(data);
    mywindow.document.write('</body></html>');
    mywindow.print();
    mywindow.close();
    return true;
}

$(document).ready(function ($) {
    window.setTimeout(function () {
        $('.alerts').slideUp();
    }, 15000);
    $('.alerts').on('click', function (e) {
        $(this).slideUp();
    });
    // $('html').perfectScrollbar();
    $('.navbar .menu').perfectScrollbar({ suppressScrollX: true });
    $('.control-sidebar').perfectScrollbar({ suppressScrollX: true });
    $('#list-table-div').perfectScrollbar({ suppressScrollX: true });
    $('.items').perfectScrollbar({ suppressScrollX: true });
});

function posScreen() {
    var wh = $(window).height(),
    total_dh = $('#totaldiv').height(),
    buttons_dh = $('.botbuttons').height();
    var items_dh = wh - 120, list_table_dh = wh - 258 - total_dh - buttons_dh;
    $('#right-col').height(wh - 100);
    $('.items').height((items_dh > 400 ? items_dh : 400));
    $('#list-table-div').height(list_table_dh);
    // $('html').perfectScrollbar('update');
    $('.items').perfectScrollbar('update');
    $('#list-table-div').perfectScrollbar('update');
}

function printLine(str) {
    var size = Settings.char_per_line;
    var len = str.length;
    var res = str.split(":");
    var newd = res[0];
    for (i = 1; i < (size - len) ; i++) {
        newd += " ";
    }
    newd += res[1];
    return newd;
}

$(window).bind("resize", posScreen);

function read_card() {

}

$.extend($.keyboard.keyaction, {
    enter: function (base) {
        base.accept();
    }
});

$(document).ready(function () {

    $('.swipe').keypress(function (e) {
        var TrackData = $(this).val() ? $(this).val() : '';
        if (TrackData != '') {
            if (e.keyCode == 13) {
                e.preventDefault();
                var p = new SwipeParserObj(TrackData);

                if (p.hasTrack1) {

                    var CardType = null;
                    var ccn1 = p.account.charAt(0);
                    if (ccn1 == 4)
                        CardType = 'Visa';
                    else if (ccn1 == 5)
                        CardType = 'MasterCard';
                    else if (ccn1 == 3)
                        CardType = 'Amex';
                    else if (ccn1 == 6)
                        CardType = 'Discover';
                    else
                        CardType = 'Visa';

                    $('#pcc_no').val(p.account).change();
                    $('#pcc_holder').val(p.account_name).change();
                    $('#pcc_month').val(p.exp_month).change();
                    $('#pcc_year').val(p.exp_year).change();
                    $('#pcc_cvv2').val('');
                    $('#pcc_type').select2('val', CardType);

                } else {
                    $('#pcc_no').val('').change();
                    $('#pcc_holder').val('').change();
                    $('#pcc_month').val('').change();
                    $('#pcc_year').val('').change();
                    $('#pcc_cvv2').val('').change();
                    $('#pcc_type').val('').change();
                }

                $('#pcc_cvv2').focus();
            }
        }

    }).blur(function (e) {
        $(this).val('');
    }).focus(function (e) {
        $(this).val('');
    });

    $(document).on('blur', '#pcc_no', function () {
        var cn = $(this).val();
        var ccn1 = cn.charAt(0);
        if (ccn1 == 4)
            CardType = 'Visa';
        else if (ccn1 == 5)
            CardType = 'MasterCard';
        else if (ccn1 == 3)
            CardType = 'Amex';
        else if (ccn1 == 6)
            CardType = 'Discover';
        else
            CardType = 'Visa';

        $('#pcc_type').select2('val', CardType);
    });

    $('.modal').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
    });
    $('#clearLS').click(function (event) {
        bootbox.confirm(lang.r_u_sure, function (result) {
            if (result == true) {
                localStorage.clear();
                location.reload();
            }
        });
        return false;
    });

    if (Settings.focus_add_item != '') {
        shortcut.add(Settings.focus_add_item, function () { $("#add_item").focus(); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }
    if (Settings.add_customer != '') {
        shortcut.add(Settings.add_customer, function () { $("#add-customer").trigger('click'); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }
    if (Settings.toggle_category_slider != '') {
        shortcut.add(Settings.toggle_category_slider, function () { $('[data-toggle="control-sidebar"]').trigger('click'); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }
    if (Settings.cancel_sale != '') {
        shortcut.add(Settings.cancel_sale, function () { $("#reset").click(); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }
    if (Settings.suspend_sale != '') {
        shortcut.add(Settings.suspend_sale, function () { $("#suspend").trigger('click'); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }
    if (Settings.print_order != '') {
        shortcut.add(Settings.print_order, function () { $("#print_order").click(); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }
    if (Settings.print_bill != '') {
        shortcut.add(Settings.print_bill, function () { $("#print_bill").click(); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }
    if (Settings.finalize_sale != '') {
        shortcut.add(Settings.finalize_sale, function () { $("#payment").trigger('click'); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }
    if (Settings.today_sale != '') {
        shortcut.add(Settings.today_sale, function () { $("#today_sale").click(); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }
    if (Settings.open_hold_bills != '') {
        shortcut.add(Settings.open_hold_bills, function () { $("#opened_bills").trigger('click'); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }
    if (Settings.close_register != '') {
        shortcut.add(Settings.close_register, function () { $("#close_register").click(); }, { 'type': 'keydown', 'propagate': false, 'target': document });
    }

});

$.ajaxSetup({ cache: false, headers: { "cache-control": "no-cache" } });