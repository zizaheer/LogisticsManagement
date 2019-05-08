
$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');

    $('#txtPassOnDateTime').val(ConvertDatetimeToUSDatetime(new Date));


    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});


$('#btnClear').on('click', function () {
    $('#txtWayBillNumber').removeAttr('readonly');
});


$('#txtWayBillNumber').unbind('keypress').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var wayBillNumber = $('#txtWayBillNumber').val();
        var orderStatus = GetSingleObjectById('OrderPassOff/GetOrderByWayBillId', wayBillNumber);
        if (orderStatus !== "") {
            orderStatus = JSON.parse(orderStatus);
        }
        else {
            bootbox.alert('The order number was not found. Please check and try again.');
            event.preventDefault();
            return;
        }

        if (orderStatus.IsOrderDispatched === null) {
            bootbox.alert('The order has not yet dispatched. Please dispatch then enter pickup information.');
            event.preventDefault();
            return;
        }

        if (orderStatus.IsOrderPickedup === null) {
            bootbox.alert('The order has not yet picked up. Please pickup and then do the pass off.');
            event.preventDefault();
            return;
        }

        if (orderStatus.IsOrderPassedOn !== null) {
            bootbox.alert('The order has already been passed on. ');
            //event.preventDefault();
            //return;
        }

        $('#txtDispatchedDateTime').val(orderStatus.DispatchDatetime);
        $('#txtEmployeeName').val(orderStatus.DispatchedEmployeeName + ' (' + orderStatus.DispatchedEmployeeId + ')');
        $('#txtPickupDateTime').val(orderStatus.PickupDatetime);

    }
});



$('#frmOrderPassOnForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmOrderPassOnForm').unbind('submit').submit(function (event) {
    var waitTime = $('#txtWaitTime').val();
    var pickupDate = $('#txtPickupDateTime').val();
    var passOnDate = $('#txtPassOnDateTime').val();
    var passOnEmployeeId = $('#ddlEmployeeId').val();
    var orderTypeId = $('#ddlOrderTypeId').val();

    var wayBillNumber = $('#txtWayBillNumber').val();

    if (passOnDate <= pickupDate) {
        bootbox.alert('Pass-on date must be greater than pickup date.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumber, waitTime, passOnEmployeeId, passOnDate, orderTypeId];

    UpdateEntry('OrderPassOff/Update', dataArray);

    event.preventDefault();
    $('#loadPassedOffDataTable').load('OrderPassOff/PartialViewDataTable');
});

$('#passoff-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    var wbNumber = $(this).data('waybillnumber');
    var orderStatus = GetSingleObjectById('OrderPassOff/GetOrderByWayBillId', wbNumber);
    if (orderStatus !== "") {
        orderStatus = JSON.parse(orderStatus);
    }
    $('#txtWayBillNumber').val(wbNumber);
    $('#txtWayBillNumber').attr('readonly', true);

    $('#txtDispatchedDateTime').val(orderStatus.DispatchDatetime);
    $('#txtEmployeeName').val(orderStatus.DispatchedEmployeeName + ' (' + orderStatus.DispatchedEmployeeId + ')');
    $('#txtPickupDateTime').val(orderStatus.PickupDatetime);

    $('#txtPassOnDateTime').val(orderStatus.PassOnDatetime);
    $('#ddlEmployeeId').val(orderStatus.PassOnEmployeeId);
    $('#txtWaitTime').val(orderStatus.PassOnWaitTime);



});

$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    RemoveEntry('OrderPassOff/Remove', waybillNumber);
    $('#loadPassedOffDataTable').load('OrderPassOff/PartialViewDataTable');
});

$('#btnDownloadDataPassedOffData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadPassedOffDataTable').load('OrderPassOff/PartialViewDataTable');

});


