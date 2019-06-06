
$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');
    //$('#txtDispatchDateTime').val(ConvertDatetimeToUSDatetime(new Date));


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

    var dataArray = [wayBillNumberArray];

    AddEntry('Invoice/Add', dataArray);

    event.preventDefault();
    $('#loadInvoicedDataTable').load('Invoice/PartialViewDataTable');
    $('#loadPendingDispatchDataTable').load('Invoice/PartialPendingInvoiceDataTable');
    wayBillNumberArray = [];
});

$('#invoice-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    var wbNumber = $(this).data('waybillnumber');

    GetAndFillOrderDetailsByWayBillNumber(wbNumber);
    $('#txtWayBillNo').attr('readonly', true);

});

$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    RemoveEntry('OrderDispatch/Remove', waybillNumber);
    $('#loadInvoicedDataTable').load('Invoice/PartialViewDataTable');
    $('#loadPendingDispatchDataTable').load('Invoice/PartialPendingInvoiceDataTable');
});

$('#btnDownloadDataInvoiceData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDispatchedDataTable').load('Invoice/PartialViewDataTable');

});


$('#btnDownloadPendingInvoiceData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadPendingInvoiceDataTable').load('Invoice/PartialPendingInvoiceDataTable');

});

