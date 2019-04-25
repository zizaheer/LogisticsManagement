
$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');
    $('#txtDispatchDateTime').val(ConvertDatetimeToUSDatetime(new Date));


    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});

var wayBillNumberArray = [];
var employeeNumber;


$('#btnDownloadOrderData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDataTable').load('Order/PartialViewDataTable');

});



$('#orderdispatch-list').on('click', '.chkOrderSelected', function (event) {
    //event.preventDefault();

    var wbNumber =
    {
        wbillNumber: $(this).data('waybillnumber')
    };


    var index = wayBillNumberArray.findIndex(c => c.wbillNumber === wbNumber.wbillNumber);
    if (index >= 0) {
        wayBillNumberArray.splice(index, 1);
    }

    wayBillNumberArray.push(wbNumber);
});

$('#txtEmployeeNumber').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        $('#ddlEmployeeId').val($('#txtEmployeeNumber').val());
    }

});

$('#frmOrderDispatchForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmOrderDispatchForm').unbind('submit').submit(function (event) {
    employeeNumber = $('#ddlEmployeeId').val();
    dispatchDate = $('#txtDispatchDateTime').val();
    if (employeeNumber < 1) {
        bootbox.alert('Please select employee.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumberArray, employeeNumber, dispatchDate];

    UpdateEntry('OrderDispatch/Update', dataArray);

    event.preventDefault();
    $('#loadDispatchedDataTable').load('OrderDispatch/PartialViewDataTable');
    $('#loadPendingDispatchDataTable').load('OrderDispatch/PartialPendingDispatchDataTable');
    wayBillNumberArray = [];
});

$('#dispatched-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    var wbNumber = $(this).data('waybillnumber');

    GetAndFillOrderDetailsByWayBillNumber(wbNumber);
    $('#txtWayBillNo').attr('readonly', true);

});

$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    RemoveEntry('OrderDispatch/Remove', waybillNumber);
    $('#loadDispatchedDataTable').load('OrderDispatch/PartialViewDataTable');
    $('#loadPendingDispatchDataTable').load('OrderDispatch/PartialPendingDispatchDataTable');
});

$('#btnDownloadDataDispatchData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDispatchedDataTable').load('OrderDispatch/PartialViewDataTable');

});


$('#btnDownloadOrderData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadPendingDispatchDataTable').load('OrderDispatch/PartialPendingDispatchDataTable');

});

