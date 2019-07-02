
$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');

    var currentDate = new Date();
    currentDate.setDate(currentDate.getDate() - 60);
    $('#txtStartDate').val(ConvertDateToUSFormat(currentDate));
    $('#txtToDate').val(ConvertDateToUSFormat(new Date));


    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});

var wayBillNumberArray = [];
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
            $('#loadInvoicedDataTable').load('Invoice/PartialViewDataTable');
            $('#loadPendingDispatchDataTable').load('Invoice/PartialPendingInvoiceDataTable');
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
$('#customerdues-list .lnkCollectPayment').on('click', function (event) {
    event.preventDefault();

    var id = $(this).data('customerid');
    var custName = $(this).data('customername');
    $('#txtCustomerName').val(custName);

    var customerWiseDueInvoices = GetListObjectById('GetDueInvoicesByCustomerId', id);

    var invoices = JSON.parse(customerWiseDueInvoices);

    $('#customer-wise-due-invoices tbody').empty();
    $('.customer-wise-due-invoices tbody').empty();

    $.each(invoices, function (i, item) {
        var appendString = "";
        appendString += "<tr>";
        appendString += "<td>";
        appendString += "<button id='lnkDisplayInvoice' class='lnkDisplayInvoice' onclick='LoadInvoice()' data-invoiceid='" + item.Id + "'>" + item.Id + "</button>";
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
        appendString += item.TotalInvoiceAmount - item.PaidAmount;
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

function LoadInvoice() {

    $('#customer-wise-due-invoices .lnkDisplayInvoice').on('click', function (event) {
        event.preventDefault();
       
        var id = $(this).data('invoiceid');
        
        var invoiceWiseWaybills = GetListObjectById('GetDueWaybillsByInvoiceId', id);
        
        var waybills = JSON.parse(invoiceWiseWaybills);

        alert(waybills);

        $('#waybill-list-for-invoice-payment tbody').empty();
        $('.waybill-list-for-invoice-payment tbody').empty();

        $.each(waybills, function (i, item) {
            var appendString = "";
            appendString += "<tr>";
            appendString += "<td>";
            appendString += item.WaybillNumber;
            appendString += "</td>";
            appendString += "<td>";
            appendString += item.PickupDate;
            appendString += "</td>";
            appendString += "<td>";
            appendString += item.DeliveryDate;
            appendString += "</td>";
            appendString += "<td>";
            appendString += item.TotalWaybillAmount;
            appendString += "</td>";
            appendString += "<td>";
            appendString += item.TotalTaxAmount;
            appendString += "</td>";

            appendString += "<td>";
            appendString += "<input type='checkbox' class='chkAddToPayment' data-totalwaybillamount='" + item.TotalWaybillAmount + "'/>";
            appendString += "</td>";
            appendString += "<td>";
            appendString += item.TotalInvoiceAmount - item.PaidAmount;
            appendString += "</td>";
            appendString += "</tr>";
            $('.waybill-list-for-invoice-payment tbody').append(appendString);

            if (item.IsCleared === true) {
                $('.chkAddToPayment').attr('disabled', true);
            } else {
                $('.chkAddToPayment').removeAttr('disabled');
            }

        });

    });
}


