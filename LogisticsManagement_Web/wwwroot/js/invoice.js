
$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');

    var currentDate = new Date();
    currentDate.setDate(currentDate.getDate() - 60);
    $('#txtStartDate').val(ConvertDateToUSFormat(currentDate));
    $('#txtToDate').val(ConvertDateToUSFormat(new Date));
    $('#txtPaymentDate').val(ConvertDateToUSFormat(new Date));
    $('#txtChequeDate').val(ConvertDateToUSFormat(new Date));


    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});

var wayBillNumberArray = [];
var wayBillNumberArrayForInvoicePayment = [];
var employeeNumber;


$('#pending-list').on('click', '.chkOrderSelected', function (event) {
    //event.preventDefault();

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

    if (dataArray.length < 1) {
        bootbox.alert('Please select waybill number/s to generate invoice');
        return;
    }

    bootbox.confirm("This will generate invoices for selected customer/s and cannot be undone. Did you see the print preview and found everything ok? ", function (result) {
        if (result === true) {
            AddEntry('Invoice/Add', [dataArray]);
            $('#loadPendingInvoiceDataTable').load('Invoice/PartialPendingInvoiceDataTable');
            wayBillNumberArray = [];
        }
    });
});

$('#btnFilter').on('click', function (event) {
    event.preventDefault();

    var startDate = $('#txtStartDate').val();
    var toDate = $('#txtToDate').val();
    var selectedCustomer = $('#ddlCustomerId').val();
    var filterData = {
        startDate: startDate,
        toDate: toDate,
        selectedCustomer: selectedCustomer
    };

    //$('#loadPendingDispatchDataTable').load('Invoice/FilterPendingInvoiceDataTable');
    PerformPostActionWithParam('Invoice/FilterPendingInvoiceDataTable', [filterData]);

    // doesnt work for some reason. check it later
});

$('#btnDownloadDataInvoiceData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDispatchedDataTable').load('Invoice/PartialViewDataTable');

});

$('#btnDownloadPendingInvoiceData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadPendingInvoiceDataTable').load('Invoice/PartialPendingInvoiceDataTable');

});


//Payment Colelction
$('#customerdues-list').on('click', '.lnkCollectPayment', function (event) {
    event.preventDefault();

    var id = $(this).data('customerid');
    var custName = $(this).data('customername');
    //$('#txtCustomerName').val(custName);
    $('#lblCustomerName').text(custName);
    $('#lblCustomerNo').text(id);

    var customerWiseDueInvoices = GetListObjectById('GetDueInvoicesByCustomerId', id);

    var invoices = JSON.parse(customerWiseDueInvoices);

    $('#customer-wise-due-invoices tbody').empty();
    $('.customer-wise-due-invoices tbody').empty();

    $('#waybill-list-for-invoice-payment tbody').empty();
    $('.waybill-list-for-invoice-payment tbody').empty();

    $.each(invoices, function (i, item) {

        var remainingAmount = item.TotalInvoiceAmount - item.PaidAmount;
        var appendString = "";
        appendString += "<tr>";
        appendString += "<td>";
        appendString += "<a style='color:#1b8eb7;cursor:pointer' role='button' id='lnkDisplayInvoice' class='lnkDisplayInvoice' onclick='LoadInvoice(this)' data-remainingamount='" + remainingAmount + "'  data-invoiceid='" + item.Id + "'>" + item.Id + "</a>";
        appendString += "</td>";
        appendString += "<td>";
        appendString += ConvertDateToUSFormat(item.CreateDate);
        appendString += "</td>";
        appendString += "<td>";
        appendString += item.TotalInvoiceAmount;
        appendString += "</td>";
        appendString += "<td>";
        if (item.PaidAmount == null) {
            appendString += 0;
        } else {
            appendString += item.PaidAmount;
        }
        appendString += "</td>";
        appendString += "<td>";
        appendString += remainingAmount;
        appendString += "</td>";
        appendString += "</tr>";
        $('.customer-wise-due-invoices').append(appendString);
    });


    //$('#loadPartialViewCustomerWiseDueInvoices').html(customerWiseDueInvoices);


    $('#collectPayment').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#collectPayment').modal('show');


});

function LoadInvoice(event) {
    var id = event.dataset.invoiceid;
    var remaining = event.dataset.remainingamount;
    $('#txtInvoiceNo').val(id);
    $('#txtDueAmount').val(remaining);
    $('#txtRemainingAmount').val(remaining);
    var invoiceWiseWaybills = GetListObjectById('GetDueWaybillsByInvoiceId', id);
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
        appendString += "<input type='checkbox' class='chkAddToPayment' onchange='AddWayBill(this)' data-waybillnumber='" + item.WaybillNumber + "' data-totalwaybillamount='" + item.TotalWaybillAmount + "'/>";
        appendString += "</td>";
        appendString += "</tr>";
        $('.waybill-list-for-invoice-payment tbody').append(appendString);

        if (item.IsCleared === true) {
            $('.chkAddToPayment').attr('checked', true);
            $('.chkAddToPayment').attr('disabled', true);
        } else {
            $('.chkAddToPayment').attr('checked', false);
            $('.chkAddToPayment').removeAttr('disabled');
        }

    });
}

function AddWayBill(event) {

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

    var paidAmount = $('#txtPaidAmount').val() === '' ? 0 : parseFloat($('#txtPaidAmount').val());
    var remainingAmnt = parseFloat($('#txtRemainingAmount').val());

    var isChecked = event.checked;
    if (isChecked === true) {
        paidAmount = paidAmount + waybillAmount;
        remainingAmnt = remainingAmnt - waybillAmount;
        if (isChecked) {
            wayBillNumberArrayForInvoicePayment.push(wbNumber);
        }
    }
    else {
        if (paidAmount >= waybillAmount) {
            paidAmount = paidAmount - waybillAmount;
            remainingAmnt = remainingAmnt + waybillAmount;
        }
    }

    $('#txtPaidAmount').val(paidAmount.toFixed(2));
    $('#txtRemainingAmount').val(remainingAmnt.toFixed(2));
}

$('#btnMakePayment').unbind().on('click', function (event) {
    event.preventDefault();

    var data = {
        invoiceNo: $('#txtInvoiceNo').val() === '' ? 0 : parseInt($('#txtInvoiceNo').val()),
        billerCustomerId: $('#lblCustomerNo').text() === '' ? 0 : parseInt($('#lblCustomerNo').text()),
        paymentAmount: $('#txtPaidAmount').val(),
        ddlBankId: $('#ddlBankId').val() === '0' ? 0 : parseInt($('#ddlBankId').val()),
        chequeNo: $('#txtChequeNo').val(),
        chequeDate: $('#txtChequeDate').val(),
        chequeAmount: $('#txtChequeAmount').val() === '' ? 0 : parseFloat($('#txtChequeAmount').val()),
        cashAmount: $('#txtCashAmount').val() === '' ? 0 : parseFloat($('#txtCashAmount').val()),
        paymentRemarks: $('#txtPaymentRemarks').val()
    }

    if (data.invoiceNo <= 0) {
        bootbox.alert('Please select invoice number to make a payment.');
        return;
    }

    if (data.paymentAmount === '' || data.paymentAmount <= 0) {
        bootbox.alert('Paid amount is required. Please select w/b numbers those are considered in this payment.');
        return;
    }

    if (data.ddlBankId > 0 || data.chequeNo !== '' || data.chequeDate !== '' || data.chequeAmount >0 ) {
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

    var totalAmnt = data.chequeAmount + data.cashAmount;
    if (totalAmnt < data.paymentAmount) {
        bootbox.alert('Total amount in Cheque/Cash cannot be less than total payment amount. Please check and try again.');
        return;
    }



    var result = PerformPostActionWithParam('MakePayment', [data, wayBillNumberArrayForInvoicePayment]);











});



