
$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');
    //FillEmployeeDropDown();
    $('#txtPickupDateTime').val(ConvertDatetimeToUSDatetime(new Date));


    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});



$('#txtWayBillNumber').unbind('keypress').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var wayBillNumber = $('#txtWayBillNumber').val();
        var orderStatus = GetSingleObjectById('OrderPickup/GetOrderByWayBillId', wayBillNumber);
        if (orderStatus !== "") {
            orderStatus = JSON.parse(orderStatus);
        }
        else
        {
            bootbox.alert('The order number was not found. Please check and try again.');
            event.preventDefault();
            return;
        }

        if (orderStatus.IsOrderDispatched === null) {
            bootbox.alert('The order has not yet dispatched. Please dispatch then enter pickup information.');
            event.preventDefault();
            return;
        }

        if (orderStatus.IsOrderPickedup !== null) {
            bootbox.alert('The order has already been picked up or delivered. Find the order in below picked-up order list or in delivered list.');
            event.preventDefault();
            return;
        }

        $('#txtDispatchedDateTime').val(orderStatus.DispatchDatetime);
        $('#txtEmployeeName').val(orderStatus.DispatchedEmployeeName + ' (' + orderStatus.DispatchedEmployeeId + ')');
    }
});



$('#frmOrderPickupForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmOrderPickupForm').unbind('submit').submit(function (event) {
    var waitTime = $('#txtWaitTime').val();
    var pickupDate = $('#txtPickupDateTime').val();
    var dispatchedDate = $('#txtDispatchedDateTime').val();
    var wayBillNumber = $('#txtWayBillNumber').val();

    if (pickupDate <= dispatchedDate) {
        bootbox.alert('Pickup date must be greater than dispatch date.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumber, waitTime, pickupDate];

    UpdateEntry('OrderPickup/Update', dataArray);

    event.preventDefault();
    $('#loadPickedupDataTable').load('OrderPickup/PartialViewDataTable');
});

$('#pickedup-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    var wbNumber = $(this).data('waybillnumber');

    //GetAndFillOrderDetailsByWayBillNumber(wbNumber);
    $('#txtWayBillNo').attr('readonly', true);

});

$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    RemoveEntry('OrderPickup/Remove', waybillNumber);
    $('#loadPickedupDataTable').load('OrderPickup/PartialViewDataTable');
});

$('#btnDownloadDataPickedupData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadPickedupDataTable').load('OrderPickup/PartialViewDataTable');

});


