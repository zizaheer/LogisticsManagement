
$(document).ready(function () {

    MaskPhoneNumber('#txtBillingPrimaryPhoneNumber');
    MaskPhoneNumber('#txtMailingPrimaryPhoneNumber');
    //FillEmployeeDropDown();
    $('#txtDeliveryDateTime').val(ConvertDatetimeToUSDatetime(new Date));


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
        var orderStatus = GetSingleObjectById('OrderDeliver/GetOrderByWayBillId', wayBillNumber);
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

        if (orderStatus.IsOrderDelivered !== null) {
            bootbox.alert('The order has already been delivered. Please check in the delivered list below.');
            //event.preventDefault();
            //return;
        }

        $('#txtDispatchedDateTime').val(orderStatus.DispatchDatetime);
        $('#txtPickupDateTime').val(orderStatus.PickupDatetime);
        $('#txtEmployeeName').val(orderStatus.DispatchedEmployeeName);

        
    }
});



$('#frmOrderDeliverForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmOrderDeliverForm').unbind('submit').submit(function (event) {
    var waitTime = $('#txtWaitTime').val();
    var pickupDate = $('#txtPickupDateTime').val();
    var deliveryDate = $('#txtDeliveryDateTime').val();
    var receivedByName = $('#txtReceivedByName').val();
    var deliveryNote = $('#txtDeliveryNote').val();
    var receivedBySign = $('#imgSignature').val();

    var wayBillNumber = $('#txtWayBillNumber').val();

    if (deliveryDate <= pickupDate) {
        bootbox.alert('Delivery date must be greater than pickup date.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumber, waitTime, deliveryDate, deliveryNote, receivedByName, receivedBySign];

    UpdateEntry('OrderDeliver/Update', dataArray);

    event.preventDefault();
    $('#loadDeliveredDataTable').load('OrderDeliver/PartialViewDataTable');
});

$('#deliver-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    var wbNumber = $(this).data('waybillnumber');
    var orderStatus = GetSingleObjectById('OrderDeliver/GetOrderByWayBillId', wbNumber);
    if (orderStatus !== "") {
        orderStatus = JSON.parse(orderStatus);
    }
    $('#txtWayBillNumber').val(wbNumber);
    $('#txtWayBillNumber').attr('readonly', true);

    $('#txtDispatchedDateTime').val(orderStatus.DispatchDatetime);
    $('#txtEmployeeName').val(orderStatus.DispatchedEmployeeName + ' (' + orderStatus.DispatchedEmployeeId + ')');
    $('#txtPickupDateTime').val(orderStatus.PickupDatetime);

    $('#txtWaitTime').val(orderStatus.DeliveryWaitTimeInHour);
    $('#txtDeliveryDateTime').val(orderStatus.DeliverDatetime);
    $('#txtReceivedByName').val(orderStatus.ReceivedByName);
    $('#txtDeliveryNote').val(orderStatus.ProofOfDeliveryNote);
    $('#imgSignature').val(orderStatus.ReceivedBySignature);

});

$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    RemoveEntry('OrderDeliver/Remove', waybillNumber);
    $('#loadDeliveredDataTable').load('OrderDeliver/PartialViewDataTable');
});

$('#btnDownloadDataDeliveredData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDeliveredDataTable').load('OrderDeliver/PartialViewDataTable');

});


