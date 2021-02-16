
$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');

    var currentDate = new Date();
    currentDate.setDate(currentDate.getDate() - 60);
    $('#txtStartDate').val(ConvertDateToUSFormat(currentDate));
    $('#txtToDate').val(ConvertDateToUSFormat(new Date));
    $('#txtInvoiceDate').val(ConvertDateToUSFormat(new Date));

    //$('#txtChequeDate').val(ConvertDateToUSFormat(new Date));


    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });

});

var wayBillNumberArray = [];
var invoiceNumberArray = [];
var wayBillNumberArrayForInvoicePayment = [];
var employeeNumber;
var orderType = 0; // 1: Delivery, 3: Misc order

$('#pending-list .chkOrderSelected').on('change', function (event) {
    event.preventDefault();

    var wbNumber =
    {
        wbillNumber: $(this).data('waybillnumber')
    };

    var isChecked = $(this).is(':checked');

    var index = wayBillNumberArray.findIndex(c => c.wbillNumber === wbNumber.wbillNumber);
    if (index >= 0) {
        wayBillNumberArray.splice(index, 1);
    }

    if (isChecked) {
        wayBillNumberArray.push(wbNumber);
    }

});
$('#pending-list .chkSelectAllOrders').on('change', function (event) {
    event.preventDefault();
    var isChecked = $(this).is(':checked');
    if (isChecked === true) {
        $('.chkOrderSelected').prop('checked', true);
        var wbArrayString = $('#hfWaybillArray').val();
        wayBillNumberArray = [];
        var wbArray = wbArrayString.split(',');
        $.each(wbArray, function (i, item) {
            if (item !== '') {
                wayBillNumberArray.push({ wbillNumber: parseInt(item) });
            }
        });
    } else {
        $('.chkOrderSelected').prop('checked', false);
        wayBillNumberArray = [];
    }
});

$('#pending-list .btnUndoDelivery').unbind().on('click', function (event) {
    event.preventDefault();

    var orderId = $(this).data('waybillnumber');
    if (orderId !== '') {
        bootbox.confirm("Delivery information related to this order will be deleted. Are you sure to proceed?", function (result) {
            if (result === true) {
                var status = PerformPostActionWithId('Order/RemoveDeliveryStatusByWaybill', orderId);
                if (status.length > 0) {
                    //$('#loadPendingInvoiceDataTable').load('Invoice/PartialPendingInvoiceDataTable');
                    location.reload();
                }
            }
        });
    }
});
$('#pending-list .btnPrintWaybill').unbind().on('click', function (event) {
    event.preventDefault();

    var isMiscellaneous = $('#rdoMiscOrder').is(':checked');
    var viewName = "";
    if (isMiscellaneous === true) {
        viewName = "PrintMiscellaneousWaybill";
    } else {
        viewName = "PrintDeliveryWaybill";
    }

    var orderId = $(this).data('waybillnumber');
    if (orderId !== '') {
        var printUrl = 'Order/PrintWaybillAsPdf';
        var printOption = {
            numberOfcopyOnEachPage: 1,
            numberOfcopyPerItem: 1,
            ignorePrice: 0,
            isMiscellaneous: isMiscellaneous === true ? 1 : 0,
            viewName: viewName,
            printUrl: printUrl
        };

        var dataArray = [[orderId], printOption];

        $.ajax({
            'async': false,
            url: printOption.printUrl,
            type: 'POST',
            data: JSON.stringify(dataArray),
            dataType: 'json',
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result.length > 0) {
                    window.open(result, "_blank");
                }
            },
            error: function (result) {
                bootbox.alert('Printing failed! There are some eror occurred while processing the printing.');
            }
        });
    }
});

$('#invoiced-list .chkInvoiceSelected').on('change', function (event) {
    event.preventDefault();

    var invNumber =
    {
        invoiceNumber: $(this).data('invoiceid')
    };

    var isChecked = $(this).is(':checked');

    var index = invoiceNumberArray.findIndex(c => c.invoiceNumber === invNumber.invoiceNumber);
    if (index >= 0) {
        invoiceNumberArray.splice(index, 1);
    }

    if (isChecked) {
        invoiceNumberArray.push(invNumber);
    }
    console.log(invoiceNumberArray);

});
$('#invoiced-list .chkCheckAllInvoices').on('change', function (event) {
    event.preventDefault();

    var isChecked = $(this).is(':checked');
    if (isChecked === true) {
        $('.chkInvoiceSelected').prop('checked', true);
        var invArrayString = $('#hfInvoiceArray').val();
        invoiceNumberArray = [];
        var invArray = invArrayString.split(',');
        $.each(invArray, function (i, item) {
            if (item !== '') {
                invoiceNumberArray.push({ invoiceNumber: parseInt(item) });
            }
        });
    } else {
        $('.chkInvoiceSelected').prop('checked', false);
        invoiceNumberArray = [];
    }
    console.log(invoiceNumberArray);
});

$('#btnDownloadData').unbind().on('click', function (event) {
    event.preventDefault();
    var isMisc = $('#chkIsMiscellaneous').is(':checked');
    $("#divLoadOrders").css("display", "none");
    $("#divLoadInvoices").css("display", "block");
    $('#loadInvoicedDataTable').load('Invoice/PartialViewDataTable/' + isMisc);

});

$('#invoiced-list .btnEdit').unbind().on('click', function () {
    $('#txtInvoiceNumberToModify').val('');
    $('#txtBillerCustomerName').val('');
    $('#txtWaybillNumbers').val('');
    $('#txtTotalInvoiceAmount').val('');

    var invoiceId = $(this).data('invoiceid');
    var billerName = $(this).data('billername');
    var waybillNumbers = $(this).data('waybills');
    var totalAmount = $(this).data('totalamount');

    $('#txtInvoiceNumberToModify').val(invoiceId);
    $('#txtBillerCustomerName').val(billerName);
    $('#txtWaybillNumbers').val(waybillNumbers);
    $('#txtTotalInvoiceAmount').val(totalAmount);

    if (parseFloat(totalAmount) <= 0) {
        $("#btnUndoInvoice").prop("disabled", true);
        $("#btnRegenerate").prop("disabled", false);
    } else {
        $("#btnUndoInvoice").prop("disabled", false);
        $("#btnRegenerate").prop("disabled", true);
    }

    $('#modifyInvoice').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#modifyInvoice').draggable();
    $('#modifyInvoice').modal('show');


});

$('#btnRegenerate').unbind().on('click', function () {
    var invoiceId = $('#txtInvoiceNumberToModify').val();;
    if (invoiceId !== '') {
        var result = PerformPostActionWithId('Invoice/RegenerateInvoice', invoiceId);
        if (result.length > 0) {
            $('#modifyInvoice').modal('hide');
            bootbox.alert('Invoice successfully regenerated.');
            var isMisc = $('#chkIsMiscellaneous').is(':checked');
            $('#loadInvoicedDataTable').load('Invoice/PartialViewDataTable/' + isMisc);

        } else {
            bootbox.alert('Failed! There was an error occurred during regenerating invoice. Please try again.');
        }
    }
});

$('#btnUndoInvoice').unbind().on('click', function () {
    var invoiceId = $('#txtInvoiceNumberToModify').val();
    if (invoiceId !== '') {
        bootbox.confirm("This will remove all associated transactions from the system to allow you to modify order/s. Are you sure you want to continue?", function (result) {
            if (result === true) {
                var resultObject = PerformPostActionWithId('Invoice/UndoInvoicing', invoiceId);
                if (resultObject.length > 0) {
                    bootbox.alert('All relevant transactions have been removed for this invoice. Please regenerate invoice for associated orders.');
                    var isMisc = $('#chkIsMiscellaneous').is(':checked');
                    $('#loadInvoicedDataTable').load('Invoice/PartialViewDataTable/' + isMisc);
                    $('#modifyInvoice').modal('hide');
                } else {
                    bootbox.alert('Failed! There was an error occurred during this operation. Please check and try again.');
                }
            }
        });
    }
});

$('#btnDeleteInvoice').unbind().on('click', function () {
    var invoiceId = $('#txtInvoiceNumberToModify').val();
    if (invoiceId !== '') {
        bootbox.confirm("This will remove the invoice number along with all associated transactions. This process cannot be undone. Are you sure you want to continue?", function (result) {
            if (result === true) {
                var resultObject = PerformPostActionWithId('Invoice/Remove', invoiceId);
                if (resultObject.length > 0) {
                    bootbox.alert('The invoice number has been removed from the system. Please generate invoice for associated orders.');
                } else {
                    bootbox.alert('Failed! There was an error occurred during this operation. Please check and try again.');
                }
            }
        });
    }
});

$('#frmInvoiceGenerationForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmInvoiceGenerationForm').unbind('submit').submit(function (event) {
    event.preventDefault();
    var dataArray = wayBillNumberArray;
    var invoiceDate = $('#txtInvoiceDate').val();
    if (dataArray.length < 1) {
        bootbox.alert('Please select waybill number/s to generate invoice');
        return;
    }

    bootbox.confirm("This will generate invoices for selected customer/s and cannot be undone. Did you see the trial print and found everything ok? ", function (result) {
        if (result === true) {
            var output = PerformPostActionWithObject('Invoice/Add', [dataArray, invoiceDate]);
            if (output !== '') {
                $('#loadPendingInvoiceDataTable').load('Invoice/PartialPendingInvoiceDataTable');
                $('#loadInvoicedDataTable').load('Invoice/PartialViewDataTable');
                location.reload();
                wayBillNumberArray = [];

            } else {
                bootbox.alert('Failed! There was an unknowned error occured during generating invoice/s. Please try again or contact support.');
                return;
            }
        }
    });
});

$('input[name="orderType"]').on('change', function () {
    $('#pending-list tbody').empty();
    $(".chkSelectAllOrders").prop("checked", false);
    $(".chkOrderSelected").prop("checked", false);

    var checkedValue = $(this).val();

    if (checkedValue == "1") {
        $("#lblOrderName").text("DELIVERY ORDERS");
    } else {
        $("#lblOrderName").text("MISC. ORDERS");
    }

    wayBillNumberArray = [];

    $('#pending-list').DataTable().clear().draw();
});

$('#btnFilter').unbind().on('click', function (event) {
    event.preventDefault();
    $("#divLoadInvoices").css("display","none");
    $("#divLoadOrders").css("display","block");
    var startDate = $('#txtStartDate').val();
    var toDate = $('#txtToDate').val();
    var selectedCustomer = $('#ddlCustomerId').val();
    orderType = parseInt($('input[name="orderType"]:checked').val());
    var filterData = {
        startDate: startDate,
        toDate: toDate,
        selectedCustomer: selectedCustomer,
        orderType: orderType
    };

    $('#loadPendingInvoiceDataTable').load('Invoice/FilterPendingInvoiceDataTable?filterData=' + JSON.stringify(filterData));
    // doesnt work for some reason. check it later. works but the loading gets exponentially slow if loading button is continuously clicked several times

    event.preventDefault();

});

$('#btnTrialPrint').unbind().on('click', function (event) {
    event.preventDefault();
    if (wayBillNumberArray.length < 1) {
        bootbox.alert('Please select waybill for trial print');
        return false;
    }

    orderType = parseInt($('input[name="orderType"]:checked').val());

    var printUrl = "";
    printUrl = 'Invoice/PrintInvoiceAsPdf';


    var printData = { wayBillNumberArray: wayBillNumberArray.sort(), invoiceNumberArray: null };
    var printOption = {
        isMiscellaneous: orderType === 3 ? 1 : 0,
        viewName: orderType === 3 ? 'PrintMiscellaneousInvoice' : 'PrintDeliveryInvoice',
        isFinalPrint: 0,
        invoiceDate: $('#txtInvoiceDate').val(),
        printUrl: printUrl
    };

    PrintAsPdf(printData, printOption);
});

$('#btnInvoiceFinalPrint').unbind().on('click', function (event) {
    event.preventDefault();
    if (invoiceNumberArray.length < 1) {
        bootbox.alert('Please select invoice for printing');
        return false;
    }
    var printUrl = "";
    printUrl = 'Invoice/PrintInvoiceAsPdf';

    var isMisc = $('#chkIsMiscellaneous').is(':checked');

    var printData = { wayBillNumberArray: null, invoiceNumberArray: invoiceNumberArray };
    var printOption = {
        isMiscellaneous: isMisc === true ? 1 : 0,
        viewName: isMisc === true ? 'PrintMiscellaneousInvoice' : 'PrintDeliveryInvoice',
        isFinalPrint: 1,
        invoiceDate: $('#txtInvoiceDate').val(),
        printUrl: printUrl
    };

    PrintAsPdf(printData, printOption);

});

$('#btnWaybillFinalPrint').unbind().on('click', function (event) {
    event.preventDefault();

    if (invoiceNumberArray.length < 1) {
        bootbox.alert('Please select invoice from the list below to print corresponding waybills');
        return false;
    }

    var wbArrayForPrint = [];

    $.each(invoiceNumberArray, function (i, item) {
        var invoiceWiseWaybills = GetListById('Invoice/GetDueWaybillsByInvoiceId', item.invoiceNumber);
        if (invoiceWiseWaybills !== '') {
            var waybills = JSON.parse(invoiceWiseWaybills);
            console.log('waybills' + waybills);
            $.each(waybills, function (i, wb) {
                var index = wbArrayForPrint.indexOf(wb.WaybillNumber);
                if (index >= 0) {
                    wbArrayForPrint.splice(index, 1);
                }
                wbArrayForPrint.push(wb.WaybillNumber);
            });
        }
    });

    var printUrl = "";
    printUrl = 'Order/PrintWaybillAsPdf';

    var isMisc = $('#chkIsMiscellaneous').is(':checked');

    var printOption = {
        numberOfcopyOnEachPage: 1,
        numberOfcopyPerItem: 1,
        ignorePrice: 0,
        isMiscellaneous: isMisc === true ? 1 : 0,
        viewName: isMisc === true ? 'PrintMiscellaneousWaybill' : 'PrintDeliveryWaybill',
        printUrl: printUrl
    };

    PrintAsPdf(wbArrayForPrint, printOption);

});

$('#customer-paid-invoices .btnPrintPaidInvoice').unbind().on('click', function (event) {
    event.preventDefault();

    var invoiceNo =
    {
        invoiceNumber: $(this).data('invoiceid')
    };

    invoiceNumberArray = [];
    invoiceNumberArray.push(invoiceNo);

    var printUrl = "";
    printUrl = 'PrintInvoiceAsPdf';

    var isMisc = $('#chkShowMiscOrderInvoice').is(':checked');

    var printData = { wayBillNumberArray: null, invoiceNumberArray: invoiceNumberArray };
    var printOption = {
        isMiscellaneous: isMisc === true ? 1 : 0,
        viewName: isMisc === true ? 'PrintMiscellaneousInvoice' : 'PrintDeliveryInvoice',
        isFinalPrint: 1,
        printUrl: printUrl
    };

    PrintAsPdf(printData, printOption);

});
function PrintAsPdf(printData, printOption) {

    var printOptions = [printData, printOption];

    $.ajax({
        'async': false,
        url: printOption.printUrl,
        type: 'POST',
        data: JSON.stringify(printOptions),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.length > 0) {
                window.open(result, "_blank");
            }
        },
        error: function (result) {
            bootbox.alert('Error occurred: ' + result);
        }
    });
}


//Payment Colelction
$('#customerdues-list').on('click', '.lnkCollectPayment', function (event) {
    event.preventDefault();

    var customerId = $(this).data('customerid');
    var custName = $(this).data('customername');
    //$('#txtCustomerName').val(custName);
    $('#lblCustomerName').text(custName);
    $('#lblCustomerNo').text(customerId);

    ClearPaymentForm();
    LoadDueInvoicesByCustomer(customerId);

    $('#collectPayment').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#collectPayment').draggable();
    $('#collectPayment').modal('show');
});

$('#ddlPaymentMethodId').on('change', function () {
    var selectedValue = parseInt($('#ddlPaymentMethodId').val());
    if (selectedValue === 3) {
        $('#ddlBankId').prop('disabled', true);
        $('#ddlBankId').val('0');
        $('#txtChequeNo').prop('disabled', true);
        $('#txtChequeNo').val('');
        $('#txtChequeDate').prop('disabled', true);
        $('#txtChequeDate').val('');
        $('#txtChequeAmount').prop('disabled', true);
        $('#txtChequeAmount').val('');
        $('#txtCashAmount').prop('disabled', false);
    } else if (selectedValue === 2) {
        $('#ddlBankId').prop('disabled', false);
        $('#txtChequeNo').prop('disabled', false);
        $('#txtChequeDate').prop('disabled', false);
        $('#txtChequeAmount').prop('disabled', false);
        $('#txtCashAmount').prop('disabled', false);
    } else {
        $('#ddlBankId').prop('disabled', false);
        $('#txtChequeNo').prop('disabled', false);
        $('#txtChequeDate').prop('disabled', false);
        $('#txtChequeAmount').prop('disabled', false);
        $('#txtCashAmount').prop('disabled', true);
        $('#txtCashAmount').val('');
    }

});

$('#chkPayAllWaybill').unbind().on('change', function (event) {
    event.preventDefault();
    var isChecked = $('#chkPayAllWaybill').is(':checked');
    if (isChecked === true) {
        $('.chkAddToPayment').prop('checked', true);
        $('.chkAddToPayment').trigger('change');
    }
    else {
        $('.chkAddToPayment').prop('checked', false);
        $('.chkAddToPayment').trigger('change');
    }
});

function LoadDueInvoicesByCustomer(customerId) {
    var customerWiseDueInvoices = GetListById('GetDueInvoicesByCustomerId', customerId);

    var invoices = JSON.parse(customerWiseDueInvoices);

    $('#customer-wise-due-invoices tbody').empty();
    $('.customer-wise-due-invoices tbody').empty();

    $('#waybill-list-for-invoice-payment tbody').empty();
    $('.waybill-list-for-invoice-payment tbody').empty();

    $.each(invoices, function (i, item) {

        var paidAmnt = item.PaidAmount == null ? 0 : parseFloat(item.PaidAmount);
        var remainingAmount = (item.TotalInvoiceAmount - item.PaidAmount).toFixed(2);
        var appendString = "";
        appendString += "<tr>";
        appendString += "<td style='width: 230px'>";
        appendString += "<a style='color:#1b8eb7;cursor:pointer;font-weight:bolder' role='button' id='lnkDisplayInvoice' class='lnkDisplayInvoice' onclick='FillInvoiceData(this)' data-invoicedate='" + item.CreateDate + "' data-paidamount='" + paidAmnt + "' data-remainingamount='" + remainingAmount + "'  data-invoiceid='" + item.Id + "'>" + item.Id + "</a>";
        appendString += "</td>";
        appendString += "<td style='width: 230px'>";
        appendString += ConvertDateToUSFormat(item.CreateDate);
        appendString += "</td>";
        appendString += "<td style='width: 230px'>";
        appendString += item.TotalInvoiceAmount;
        appendString += "</td>";
        appendString += "<td style='width: 230px'>";
        if (item.PaidAmount == null) {
            appendString += 0;
        } else {
            appendString += item.PaidAmount;
        }
        appendString += "</td>";
        appendString += "<td style='width: auto'>";
        appendString += remainingAmount;
        appendString += "</td>";
        appendString += "</tr>";
        $('.customer-wise-due-invoices').append(appendString);
    });

}

function FillInvoiceData(event) {
    var invoiceId = event.dataset.invoiceid;
    var remaining = event.dataset.remainingamount;
    var paidAmount = event.dataset.paidamount;
    var invoiceDate = event.dataset.invoicedate;
    $('#txtInvoiceNo').val(invoiceId);
    $('#txtPaidAmount').val(paidAmount);
    $('#txtDueAmount').val(remaining);
    $('#txtRemainingAmount').val(remaining);
    $('#txtChequeAmount').val("0.00");
    $('#txtCashAmount').val("0.00");
    $('#txtPaymentApplied').val("0.00");
    $("#chkPayAllWaybill").prop("checked", false);
    $('#txtInvoiceDate').val(ConvertDateToUSFormat(new Date(invoiceDate)));

    var invoiceWiseWaybills = GetListById('GetDueWaybillsByInvoiceId', invoiceId);
    var waybills = JSON.parse(invoiceWiseWaybills);

    $('#waybill-list-for-invoice-payment tbody').empty();
    $('.waybill-list-for-invoice-payment tbody').empty();

    $.each(waybills, function (i, item) {
        var appendString = "";
        appendString += "<tr>";
        appendString += "<td>";
        appendString += item.WaybillNumber;
        appendString += "</td>";
        appendString += "<td>";
        appendString += ConvertDateToUSFormat(item.PickupDate);
        appendString += "</td>";
        appendString += "<td>";
        appendString += ConvertDateToUSFormat(item.DeliveryDate);
        appendString += "</td>";
        appendString += "<td>";
        appendString += item.TotalWaybillAmount;
        appendString += "</td>";
        //appendString += "<td>";
        //appendString += item.TotalTaxAmount;
        //appendString += "</td>";

        appendString += "<td>";

        var disabled = '';
        var checked = '';
        if (item.IsCleared === true) {
            disabled = 'disabled';
            checked = 'checked';
        }
        appendString += "<input type='checkbox' class='chkAddToPayment' onchange='AddWayBillToPayment(this)' data-waybillnumber='" + item.WaybillNumber + "' data-totalwaybillamount='" + item.TotalWaybillAmount + "' " + checked + " " + disabled + " />";
        appendString += "</td>";
        appendString += "</tr>";
        $('.waybill-list-for-invoice-payment tbody').append(appendString);

    });
}

function AddWayBillToPayment(event) {

    var waybillAmount = parseFloat(event.dataset.totalwaybillamount);

    var waybillNo = event.dataset.waybillnumber;
    var wbNumber =
    {
        wbillNumber: waybillNo
    };
    var index = wayBillNumberArrayForInvoicePayment.findIndex(c => c.wbillNumber === wbNumber.wbillNumber);
    if (index >= 0) {
        wayBillNumberArrayForInvoicePayment.splice(index, 1);
    }

    var chequeAmount = $('#txtChequeAmount').val() === '' ? 0 : parseFloat($('#txtChequeAmount').val());
    var dueAmount = $('#txtDueAmount').val() === '' ? 0 : parseFloat($('#txtDueAmount').val());
    
    var isChecked = event.checked;
    if (isChecked === true) {
        chequeAmount = chequeAmount + waybillAmount;
        wayBillNumberArrayForInvoicePayment.push(wbNumber);

    }
    else {
        if (chequeAmount >= waybillAmount) {
            if (chequeAmount > dueAmount) {
                chequeAmount = dueAmount;
            }
            chequeAmount = chequeAmount - waybillAmount;
        } else if (chequeAmount >= dueAmount) {
            chequeAmount = chequeAmount - dueAmount;
        }
    }

    if (chequeAmount > dueAmount) {
        chequeAmount = dueAmount;
    }

    $('#txtChequeAmount').val(chequeAmount.toFixed(2));
    $('#txtPaymentApplied').val(chequeAmount.toFixed(2));
    $('#txtPaymentApplied').trigger('change');

    if (wayBillNumberArrayForInvoicePayment.length < 1) {
        $('#txtChequeAmount').val('0.00');
        $('#txtPaymentApplied').val('0.00');
        $('#txtPaymentApplied').trigger('change');
    }
    
}

$('#txtChequeAmount').on('change', function () {
    CalculatePaymentAppliedAmount();
});
$('#txtCashAmount').on('change', function () {
    CalculatePaymentAppliedAmount();
});

$('#txtPaymentApplied').on('change', function () {
    var paymentApplied = $('#txtPaymentApplied').val() === '' ? 0 : parseFloat($('#txtPaymentApplied').val());
    var dueAmount = $('#txtDueAmount').val() === '' ? 0 : parseFloat($('#txtDueAmount').val());
    var remainingAmount = 0;
    remainingAmount = dueAmount - paymentApplied;

    $('#txtRemainingAmount').val(remainingAmount.toFixed(2));
});

function CalculatePaymentAppliedAmount() {
    var chequeAmnt = 0; 
    var cashAmnt = 0;
    var paymentApplied = 0;
    
    chequeAmnt = $('#txtChequeAmount').val() === '' ? 0 : parseFloat($('#txtChequeAmount').val());
    cashAmnt = $('#txtCashAmount').val() === '' ? 0 : parseFloat($('#txtCashAmount').val());
    paymentApplied = $('#txtPaymentApplied').val() === '' ? 0 : parseFloat($('#txtPaymentApplied').val());

    paymentApplied = chequeAmnt + cashAmnt;

    $('#txtPaymentApplied').val(paymentApplied.toFixed(2));
    $('#txtPaymentApplied').trigger('change');

}

function ClearPaymentForm() {
    $('#txtInvoiceNo').val('');
    $('#txtPaidAmount').val('');
    $('#txtDueAmount').val('');
    $('#txtInvoiceDate').val('');
    $('#chkPayAllWaybill').prop('checked', false);


    $('#ddlPaymentMethodId').val('1');
    $('#chkKeepBankingInformation').prop("checked", false);
    $('#ddlBankId').val('0');
    $('#txtChequeNo').val('');
    $('#txtChequeAmount').val('');
    $('#txtChequeDate').val('');
    $('#txtCashAmount').val('');
    $('#txtPaymentApplied').val('');
    $('#txtRemainingAmount').val('');
    $('#txtPaymentRemarks').val('');

    $('#ddlBankId').prop('disabled', false);
    $('#txtChequeNo').prop('disabled', false);
    $('#txtChequeDate').prop('disabled', false);
    $('#txtChequeAmount').prop('disabled', false);
    $('#txtCashAmount').prop('disabled', false);

    $('#ddlPaymentMethodId').trigger('change');
}

$('#btnMakePayment').unbind().on('click', function (event) {
    event.preventDefault();

    var data = {
        invoiceNo: $('#txtInvoiceNo').val() === '' ? 0 : parseInt($('#txtInvoiceNo').val()),
        billerCustomerId: $('#lblCustomerNo').text() === '' ? 0 : parseInt($('#lblCustomerNo').text()),
        paymentMethodId: $('#ddlPaymentMethodId').val() === '' ? 0 : parseInt($('#ddlPaymentMethodId').val()),
        paymentAmount: $('#txtPaymentApplied').val() === '' ? 0 : parseFloat($('#txtPaymentApplied').val()),
        ddlBankId: $('#ddlBankId').val() === '0' ? 0 : parseInt($('#ddlBankId').val()),
        chequeNo: $('#txtChequeNo').val(),
        chequeDate: $('#txtChequeDate').val(),
        chequeAmount: $('#txtChequeAmount').val() === '' ? 0 : parseFloat($('#txtChequeAmount').val()),
        cashAmount: $('#txtCashAmount').val() === '' ? 0 : parseFloat($('#txtCashAmount').val()),
        paymentRemarks: $('#txtPaymentRemarks').val()
    };

    if (data.invoiceNo <= 0) {
        bootbox.alert('Please select invoice number to make a payment.');
        return;
    }

    if (data.paymentAmount === '' || data.paymentAmount <= 0) {
        bootbox.alert('Payment amount is required. Please check and try again.');
        return;
    }

    if (data.paymentAmount > (parseFloat($('#txtDueAmount').val()))) {
        bootbox.alert('Applied amount cannot be greater than due amount.');
        return;
    }

    if (data.paymentMethodId === 1) {
        if (data.ddlBankId < 1) {
            bootbox.alert('Please select bank information');
            return;
        }
        if (data.chequeNo === '') {
            bootbox.alert('Please enter cheque number');
            return;
        }
        if (data.chequeDate === '') {
            bootbox.alert('Please enter cheque date');
            return;
        }
        if (data.chequeAmount === '' || data.chequeAmount <= 0) {
            bootbox.alert('Please enter cheque amount');
            return;
        }
    }

   

    var result = PerformPostActionWithObject('MakePayment', [data, wayBillNumberArrayForInvoicePayment]);
    //if (result.length > 0) {
        LoadDueInvoicesByCustomer(data.billerCustomerId);
        bootbox.alert('Payment successfully made.');
        $('#loadCustomersInvoiceDue').load('PartialCustomersInvoiceDueDataTable');
        wayBillNumberArrayForInvoicePayment = [];

    //} else {
    //bootbox.alert('' + result);
    //    return;
    //}

    var isApplyToNextInvoice = $('#chkKeepBankingInformation').is(':checked');

    ClearPaymentForm();
    
    if (isApplyToNextInvoice === true) {
        $('#ddlPaymentMethodId').val(data.paymentMethodId);
        $('#chkKeepBankingInformation').prop("checked",true);
        $('#ddlBankId').val(data.ddlBankId);
        $('#txtChequeNo').val(data.chequeNo);
        //$('#txtChequeAmount').val('0.00');
        $('#txtChequeDate').val(data.chequeDate);
    }
});

$('#customer-paid-invoices .btnUndoPayment').unbind().on('click', function () {

    var invoiceId = $(this).data('invoiceid');
    if (invoiceId !== '') {
        var paymentList = GetListById('Invoice/GetPaymentListByInvoiceId', invoiceId);
        $.each(paymentList, function (key, value) {
            //$('#invoice-payment-list tbody').append('<tr><td>');
        });
    }

    $('#paymentList').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#paymentList').draggable();
    $('#paymentList').modal('show');


    //var invoiceId = $(this).data('invoiceid');
    //if (invoiceId !== '') {

    //    var result = PerformPostActionWithId('Invoice/UndoPayment', invoiceId);
    //    if (result.length > 0) {
    //        LoadDueInvoicesByCustomer(data.billerCustomerId);
    //        bootbox.alert('Payment has been deleted.');
    //        $('#loadCustomersInvoiceDue').load('PartialCustomersInvoiceDueDataTable');
    //        wayBillNumberArrayForInvoicePayment = [];
    //    } else {
    //        bootbox.alert('Failed! There was an error occurred. Please try again.');
    //    }
    //}

});

$('#btnCloseModal').unbind().on('click', function (event) {
    event.preventDefault();
    $('#collectPayment').modal('hide');
});

$('#btnShowRecords').unbind().on('click', function (event) {

    event.preventDefault();
    var customerId = $('#ddlCustomerId').val();
    var selectedYear = $('#ddlYear').val();
    var isPaid = $('#chkShowPaidInvoice').is(':checked') === true ? 1 : 0;

    $('#loadCustomersInvoiceDue').load('PartialCustomersInvoiceDueDataTable?customerId=' + customerId + '&year=' + selectedYear + '&isPaid=' + isPaid);


});

$('#btnShowRecordsForPaidInvoice').unbind().on('click', function (event) {

    event.preventDefault();
    var customerId = $('#ddlCustomerIdForPaidInvoice').val();
    var isPaid = 1;

    $('#loadCustomerWiseInvoices').load('PartialGetPaidInvoicesByCustomer?customerId=' + customerId + '&isPaid=' + isPaid);

});

$('.btnViewInvoice').unbind().on('click', function (event) {
    event.preventDefault();

    var customerId = $(this).data("customerid");
    var isPaid = 1;

    $('#loadCustomerWiseInvoices').load('PartialGetPaidInvoicesByCustomer?customerId=' + customerId + '&isPaid=' + isPaid, function () {

        $('#paidInvoiceModal').modal({
            backdrop: 'static',
            keyboard: false
        });
        $('#paidInvoiceModal').draggable();
        $('#paidInvoiceModal').modal('show');

    });
});